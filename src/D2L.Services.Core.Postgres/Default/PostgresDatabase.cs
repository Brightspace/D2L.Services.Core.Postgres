using D2L.Services.Core.Postgres.Enumeration;
using D2L.Services.Core.Postgres.Enumeration.Default;
using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres.Default {
	
	internal sealed class PostgresDatabase : PostgresExecutorBase, IPostgresDatabase {
		
		private readonly string m_connectionString;
		
		internal PostgresDatabase( string npgsqlConnectionString ) {
			m_connectionString = npgsqlConnectionString;
		}
		
		IPostgresTransaction IPostgresDatabase.NewTransaction(
			PostgresIsolationLevel isolationLevel
		) {
			return new PostgresTransaction( m_connectionString, isolationLevel );
		}
		
		Task<IPostgresTransaction> IPostgresDatabase.NewTransactionAsync(
			PostgresIsolationLevel isolationLevel
		) {
			return PostgresTransaction.ConstructAsync( m_connectionString, isolationLevel );
		}
		
		protected override void ExecuteSync(
			PostgresCommand command,
			Action<NpgsqlCommand> action
		) {
			using( var connection = new NpgsqlConnection( m_connectionString ) ) {
				connection.Open();
				using( NpgsqlTransaction transaction = connection.BeginTransaction() ) {
					using( NpgsqlCommand cmd = command.Build( connection, transaction ) ) {
						action( cmd );
						transaction.Commit();
					}
				}
				// On an error, transaction.Rollback() is automatically done as
				// part of the Dispose() call
			}
		}
		
		protected async override Task ExecuteAsync(
			PostgresCommand command,
			Func<NpgsqlCommand,Task> action
		) {
			using( var connection = new NpgsqlConnection( m_connectionString ) ) {
				// OpenAsync() is actually executed synchronously in the current
				// version of Npgsql. We'll use OpenAsync() anyways in
				// anticipation of a proper implementation being added in a
				// future version of Npgsql.
				await connection.OpenAsync().SafeAsync();
				using( NpgsqlTransaction transaction = connection.BeginTransaction() ) {
					using( NpgsqlCommand cmd = command.Build( connection, transaction ) ) {
						await action( cmd ).SafeAsync();
						transaction.Commit();
					}
				}
				// On an error, transaction.Rollback() is automatically done as
				// part of the Dispose() call
			}
		}
		
		
		public override IOnlineResultSet<Dto> ExecReadOnline<Dto>(
			PostgresCommand command,
			Func<IDataRecord, Dto> dbConverter
		) {
			NpgsqlConnection connection = null;
			NpgsqlTransaction transaction = null;
			NpgsqlCommand cmd = null;
			try {
				connection = new NpgsqlConnection( m_connectionString );
				connection.Open();
				
				transaction = connection.BeginTransaction();
				cmd = command.Build( connection, transaction );
				
				// The PostgresResultSet is now responsible for disposing the
				// data reader, transaction, and connection
				return new PostgresResultSet<Dto>(
					reader: cmd.ExecuteReader(),
					command: cmd,
					dbConverter: dbConverter
				);
			} catch( Exception exception ) {
				cmd.SafeDispose( ref exception );
				transaction.SafeDispose( ref exception );
				connection.SafeDispose( ref exception );
				throw exception;
			}
		}
		
		public async override Task<IOnlineResultSet<Dto>> ExecReadOnlineAsync<Dto>(
			PostgresCommand command,
			Func<IDataRecord, Dto> dbConverter
		) {
			NpgsqlConnection connection = null;
			NpgsqlTransaction transaction = null;
			NpgsqlCommand cmd = null;
			try {
				connection = new NpgsqlConnection( m_connectionString );
				await connection.OpenAsync().SafeAsync();
				
				transaction = connection.BeginTransaction();
				cmd = command.Build( connection, transaction );
				
				// The PostgresResultSet is now responsible for disposing the
				// data reader, transaction, and connection
				return new PostgresResultSet<Dto>(
					reader: await cmd.ExecuteReaderAsync().SafeAsync(),
					command: cmd,
					dbConverter: dbConverter
				);
			} catch( Exception exception ) {
				cmd.SafeDispose( ref exception );
				transaction.SafeDispose( ref exception );
				connection.SafeDispose( ref exception );
				throw exception;
			}
		}
		
	}
	
}
