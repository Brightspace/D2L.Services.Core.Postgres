using D2L.Services.Core.Postgres.Default;
using System;

namespace D2L.Services.Core.Postgres {
	
	/// <summary>
	/// A factory used to create an <see cref="IPostgresDatabase"/>.
	/// </summary>
	/// <threadsafety static="true" />
	public static class PostgresDatabaseProvider {
		
		/// <summary>
		/// Construct a new <see cref="IPostgresDatabase"/> instance.
		/// The constructed <c>IPostgresDatabase</c> is thread-safe, so it can
		/// safely be used as a singleton.
		/// </summary>
		/// <param name="npgsqlConnectionString">
		/// The database connection string. See the Npgsql documentation for
		/// connection string parameters.
		/// </param>
		/// <returns>A new instance of <c>IPostgresDatabase</c>.</returns>
		public static IPostgresDatabase Create( string npgsqlConnectionString ) {
			return new PostgresDatabase( npgsqlConnectionString );
		}
		
	}
	
}
