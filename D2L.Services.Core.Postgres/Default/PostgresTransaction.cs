using Npgsql;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres.Default {

	internal sealed class PostgresTransaction : PostgresExecutorBase, IPostgresTransaction {

		private enum TransactionState {
			Uncommited,
			Commited,
			RolledBack
		}

		private readonly NpgsqlConnection m_connection;
		private readonly NpgsqlTransaction m_transaction;
		private TransactionState m_state = TransactionState.Uncommited;
		
		static internal async Task<IPostgresTransaction> ConstructAsync(
			string connectionString,
			PostgresIsolationLevel pgIsolationLevel
		) {
			NpgsqlConnection connection = null;
			NpgsqlTransaction transaction = null;
			try {
				connection = new NpgsqlConnection( connectionString );
				await connection.OpenAsync().SafeAsync();
				transaction = connection.BeginTransaction(
					pgIsolationLevel.ToAdoIsolationLevel()
				);
				return new PostgresTransaction( connection, transaction );
			} catch( Exception ) {
				await transaction.SafeDisposeAsync().SafeAsync();
				await connection.SafeDisposeAsync().SafeAsync();
				throw;
			}
		}
		
		private PostgresTransaction(
			NpgsqlConnection openConnection,
			NpgsqlTransaction transaction
		) {
			m_connection = openConnection;
			m_transaction = transaction;
		}
		
		async ValueTask IAsyncDisposable.DisposeAsync() {
			if( m_state == TransactionState.Uncommited ) {
				m_state = TransactionState.RolledBack;
				await m_transaction.SafeDisposeAsync().SafeAsync();
				await m_connection.SafeDisposeAsync().SafeAsync();
			}
		}
		
		async Task IPostgresTransaction.CommitAsync() {
			IPostgresTransaction @this = this;
			AssertIsOpen();
			
			try {
				await m_transaction.CommitAsync().SafeAsync();
			} catch( Exception commitException ) {
				try {
					await @this.RollbackAsync().SafeAsync();
				} catch( Exception rollbackException ) {
					throw new AggregateException( commitException, rollbackException );
				}
			}

			await @this.DisposeAsync().SafeAsync();
			m_state = TransactionState.Commited;
		}
		
		async Task IPostgresTransaction.RollbackAsync() {
			if( m_state == TransactionState.Commited ) {
				throw new InvalidOperationException(
					"The transaction could not be rolled back because it has " +
					"already been successfully committed."
				);
			} else if( m_state == TransactionState.Uncommited ) {
				try {
					await m_transaction.RollbackAsync().SafeAsync();
				} finally {
					await ((IAsyncDisposable)this).DisposeAsync().SafeAsync();
				}
			}
		}

		ConfiguredAsyncDisposable IPostgresTransaction.Handle => this.ConfigureAwait( false );
		
		protected override async Task ExecuteAsync(
			PostgresCommand command,
			Func<NpgsqlCommand,Task> action
		) {
			AssertIsOpen();
			NpgsqlCommand cmd = await command.BuildAsync( m_connection, m_transaction ).SafeAsync();
			await using ConfiguredAsyncDisposable handle = cmd.ConfigureAwait( false );

			try {
				await action( cmd ).SafeAsync();
			} catch( Exception commandException ) {
				try {
					await ((IPostgresTransaction)this).RollbackAsync().SafeAsync();
				} catch( Exception rollbackException ) {
					throw new AggregateException( commandException, rollbackException );
				}
				throw;
			}
		}
		
		
		private void AssertIsOpen() {
			if( m_state != TransactionState.Uncommited ) {
				throw new ObjectDisposedException( nameof( PostgresTransaction ) );
			}
		}
		
	}
	
}
