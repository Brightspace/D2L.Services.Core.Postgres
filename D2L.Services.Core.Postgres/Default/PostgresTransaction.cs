using Npgsql;
using System;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres.Default {
	
	internal sealed class PostgresTransaction : PostgresExecutorBase, IPostgresTransaction {
		
		private readonly NpgsqlConnection m_connection;
		private readonly NpgsqlTransaction m_transaction;
		private bool m_isDisposed = false;
		
		internal PostgresTransaction(
			string connectionString,
			PostgresIsolationLevel pgIsolationLevel
		) {
			try {
				m_connection = new NpgsqlConnection( connectionString );
				m_connection.Open();
				m_transaction = m_connection.BeginTransaction(
					pgIsolationLevel.ToAdoIsolationLevel()
				);
			} catch( Exception exception ) {
				m_transaction.SafeDispose( ref exception );
				m_connection.SafeDispose( ref exception );
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
		
		void IPostgresTransaction.Commit() {
			AssertIsOpen();
			try {
				m_transaction.Commit();
			} finally {
				((IDisposable)this).Dispose();
			}
		}
		
		void IPostgresTransaction.Rollback() {
			AssertIsOpen();
			((IDisposable)this).Dispose();
		}
		
		
		protected override void ExecuteSync(
			PostgresCommand command,
			Action<NpgsqlCommand> action
		) {
			AssertIsOpen();
			using( NpgsqlCommand cmd = command.Build( m_connection, m_transaction ) ) {
				action( cmd );
			}
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
