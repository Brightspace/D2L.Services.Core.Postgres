using Npgsql;
using System;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres.Default {
	
	internal sealed class PostgresTransaction : PostgresExecutorBase, IPostgresTransaction {
		
		private readonly NpgsqlConnection m_connection;
		private readonly NpgsqlTransaction m_transaction;
		private bool m_isDisposed = false;
		
		private PostgresTransaction(
			NpgsqlConnection openConnection,
			NpgsqlTransaction transaction
		) {
			m_connection = openConnection;
			m_transaction = transaction;
		}
		
		internal async static Task<IPostgresTransaction> ConstructAsync(
			string connectionString,
			PostgresIsolationLevel pgIsolationLevel
		) {
			NpgsqlConnection connection = null;
			NpgsqlTransaction transaction = null;
			try {
				connection = new NpgsqlConnection( connectionString );
				// OpenAsync() is actually executed synchronously in the current
				// version of Npgsql. We'll use OpenAsync() anyways in
				// anticipation of a proper implementation being added in a
				// future version of Npgsql.
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
		
		void IDisposable.Dispose() {
			if( !m_isDisposed ) {
				m_isDisposed = true;
				m_transaction.SafeDispose();
				m_connection.SafeDispose();
			}
		}
		
		//TODO[v2.0.0] When Npgsql 3.1 is released, use CommitAsync()
		Task IPostgresTransaction.CommitAsync() {
			AssertIsOpen();
			try {
				m_transaction.Commit();
			} finally {
				((IDisposable)this).Dispose();
			}
			return Task.WhenAll(); // Completed task with no result
		}
		
		//TODO[v2.0.0] When Npgsql 3.1 is released, use RollbackAsync()
		Task IPostgresTransaction.RollbackAsync() {
			AssertIsOpen();
			((IDisposable)this).Dispose();
			return Task.WhenAll(); // Completed task with no result
		}
		
		protected async override Task ExecuteAsync(
			PostgresCommand command,
			Func<NpgsqlCommand,Task> action
		) {
			AssertIsOpen();
			using( NpgsqlCommand cmd = command.Build( m_connection, m_transaction ) ) {
				await action( cmd ).SafeAsync();
			}
		}
		
		
		private void AssertIsOpen() {
			if( m_isDisposed ) {
				throw new ObjectDisposedException( "PostgresTransaction" );
			}
		}
		
	}
	
}
