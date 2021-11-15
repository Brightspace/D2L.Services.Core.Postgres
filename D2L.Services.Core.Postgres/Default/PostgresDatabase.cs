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
		
		protected override async Task ExecuteAsync(
			PostgresCommand command,
			Func<NpgsqlCommand,Task> action
		) {
			NpgsqlConnection connection = new NpgsqlConnection( m_connectionString );
			await using var connectionHandle = connection.ConfigureAwait( false );

			await connection.OpenAsync().SafeAsync();

			NpgsqlCommand cmd = await command.BuildAsync( connection ).SafeAsync();
			await using var commandHandle = cmd.ConfigureAwait( false );

			await action( cmd ).SafeAsync();
		}
		
	}
	
}
