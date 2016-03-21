using D2L.Services.Core.Postgres.Enumeration;
using D2L.Services.Core.Postgres.Enumeration.Default;
using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres.Default {
	
	internal sealed class PostgresTransaction : PostgresExecutorBase, IPostgresTransaction {
		
		private readonly NpgsqlConnection m_connection;
		private readonly NpgsqlTransaction m_transaction;
		private bool m_isDisposed = false;
		
		// Whether or not the PostgresTransaction is responsible for ending the
		// transaction and closing the connection when disposed.
		private bool m_isResponsible = true;
		
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
			if( m_isResponsible && !m_isDisposed ) {
				m_isDisposed = true;
				m_transaction.SafeDispose();
				m_connection.SafeDispose();
			}
		}
		
		void IPostgresTransaction.Commit() {
			AssertIsOpenAndResponsible();
			try {
				m_transaction.Commit();
			} finally {
				((IDisposable)this).Dispose();
			}
		}
		
		void IPostgresTransaction.Rollback() {
			AssertIsOpenAndResponsible();
			((IDisposable)this).Dispose();
		}
		
		
		protected override void ExecuteSync(
			PostgresCommand command,
			Action<NpgsqlCommand> action
		) {
			AssertIsOpenAndResponsible();
			using( NpgsqlCommand cmd = command.Build( m_connection, m_transaction ) ) {
				action( cmd );
			}
		}
		
		protected async override Task ExecuteAsync(
			PostgresCommand command,
			Func<NpgsqlCommand,Task> action
		) {
			AssertIsOpenAndResponsible();
			using( NpgsqlCommand cmd = command.Build( m_connection, m_transaction ) ) {
				await action( cmd ).SafeAsync();
			}
		}
		
		
		public override IOnlineResultSet<Dto> ExecReadOnline<Dto>(
			PostgresCommand command,
			Func<IDataRecord, Dto> dbConverter
		) {
			AssertIsOpenAndResponsible();
			
			NpgsqlCommand cmd = null;
			try {
				cmd = command.Build( m_connection, m_transaction );
				
				// The PostgresResultSet is now responsible for disposing the
				// data reader, command, transaction, and connection
				m_isResponsible = false;
				return new PostgresResultSet<Dto>(
					reader: cmd.ExecuteReader(),
					command: cmd,
					dbConverter: dbConverter
				);
			} catch( Exception exception ) {
				cmd.SafeDispose( ref exception );
				m_transaction.SafeDispose( ref exception );
				m_connection.SafeDispose( ref exception );
				m_isDisposed = true;
				throw exception;
			}
			
		}
		
		public async override Task<IOnlineResultSet<Dto>> ExecReadOnlineAsync<Dto>(
			PostgresCommand command,
			Func<IDataRecord, Dto> dbConverter
		) {
			AssertIsOpenAndResponsible();
			
			NpgsqlCommand cmd = null;
			try {
				cmd = command.Build( m_connection, m_transaction );
				
				// The PostgresResultSet is now responsible for disposing the
				// data reader, transaction, and connection
				m_isResponsible = false;
				return new PostgresResultSet<Dto>(
					reader: await cmd.ExecuteReaderAsync().SafeAsync(),
					command: cmd,
					dbConverter: dbConverter
				);
			} catch( Exception exception ) {
				cmd.SafeDispose( ref exception );
				m_transaction.SafeDispose( ref exception );
				m_connection.SafeDispose( ref exception );
				m_isDisposed = true;
				throw exception;
			}
		}
		
		private void AssertIsOpenAndResponsible() {
			if( !m_isResponsible ) {
				throw new InvalidOperationException(
					"You may not invoke methods on an IPostgresTransaction " +
					"after calling ExecReadOnline() or ExecReadOnlineAsync()."
				);
			} else if( m_isDisposed ) {
				throw new ObjectDisposedException( "PostgresTransaction" );
			}
		}
		
	}
	
}
