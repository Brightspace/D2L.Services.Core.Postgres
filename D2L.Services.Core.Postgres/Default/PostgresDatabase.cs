using Npgsql;
using System;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres.Default {
	
	internal sealed class PostgresDatabase : PostgresExecutorBase, IPostgresDatabase {
		
		private readonly string m_connectionString;
		
		internal PostgresDatabase( string npgsqlConnectionString ) {
			m_connectionString = npgsqlConnectionString;
		}
		
		Task<IPostgresTransaction> IPostgresDatabase.NewTransactionAsync(
			PostgresIsolationLevel isolationLevel
		) {
			return PostgresTransaction.ConstructAsync( m_connectionString, isolationLevel );
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
		
	}
	
}
