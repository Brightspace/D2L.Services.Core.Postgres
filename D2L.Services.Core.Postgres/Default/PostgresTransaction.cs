using Npgsql;
using System;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres.Default {
	
	internal sealed class PostgresTransaction : PostgresExecutorBase, IPostgresTransaction {
		
		private readonly NpgsqlConnection m_connection;
		private readonly NpgsqlTransaction m_transaction;
		private bool m_isDisposed = false;
		private bool m_hasCommitted = false;
		
		internal async static Task<IPostgresTransaction> ConstructAsync(
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
			} catch( Exception exception ) {
				transaction.SafeDispose( ref exception );
				connection.SafeDispose( ref exception );
				throw exception;
			}
		}
		
		private PostgresTransaction(
			NpgsqlConnection openConnection,
			NpgsqlTransaction transaction
		) {
			m_connection = openConnection;
			m_transaction = transaction;
		}
		
		void IDisposable.Dispose() {
			if( !m_isDisposed ) {
				m_isDisposed = true;
				m_transaction.SafeDispose();
				m_connection.SafeDispose();
			}
		}
		
		async Task IPostgresTransaction.CommitAsync() {
			AssertIsOpen();
			
			Exception commitException = null;
			try {
				await m_transaction.CommitAsync().SafeAsync();
			} catch( Exception exception ) {
				commitException = exception;
			}
			
			IPostgresTransaction @this = this;
			try {
				if( commitException != null ) {
					await @this.RollbackAsync().SafeAsync();
				} else {
					@this.Dispose();
				}
			} catch( Exception disposeException ) {
				throw commitException ?? disposeException;
			}
			
			if( commitException != null ) {
				throw commitException;
			}
			
			m_hasCommitted = true;
		}
		
		async Task IPostgresTransaction.RollbackAsync() {
			if( m_hasCommitted ) {
				throw new ObjectDisposedException( "PostgresTransaction" );
			} else if( !m_isDisposed ) {
				try {
					if( !m_transaction.IsCompleted ) {
						await m_transaction.RollbackAsync().SafeAsync();
					}
				} finally {
					((IDisposable)this).Dispose();
				}
			}
		}
		
		protected async override Task ExecuteAsync(
			PostgresCommand command,
			Func<NpgsqlCommand,Task> action
		) {
			AssertIsOpen();
			using( NpgsqlCommand cmd = command.Build( m_connection, m_transaction ) ) {
				Exception exception = null;
				try {
					await action( cmd ).SafeAsync();
				} catch( Exception ex ) {
					exception = ex;
				}
				
				if( exception != null ) {
					try {
						await ((IPostgresTransaction)this).RollbackAsync().SafeAsync();
					} finally {
						throw exception;
					}
				}
			}
		}
		
		
		private void AssertIsOpen() {
			if( m_isDisposed ) {
				throw new ObjectDisposedException( "PostgresTransaction" );
			}
		}
		
	}
	
}
