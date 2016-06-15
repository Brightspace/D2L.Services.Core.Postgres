using Npgsql;
using System;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres.Default {
	
	internal sealed class PostgresDatabase : PostgresExecutorBase, IPostgresDatabase {
		
		private readonly string m_connectionString;
		
		internal PostgresDatabase( string npgsqlConnectionString ) {
			m_connectionString = string.Copy( npgsqlConnectionString );
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
				await connection.OpenAsync().SafeAsync();
				using( NpgsqlCommand cmd = command.Build( connection ) ) {
					await action( cmd ).SafeAsync();
				}
			}
		}
		
	}
	
}
