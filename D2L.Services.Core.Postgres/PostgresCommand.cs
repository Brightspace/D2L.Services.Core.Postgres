using D2L.Services.Core.Postgres.TypeConverters;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres {

	/// <summary>
	/// Represents a SQL statement to execute against a PostgreSQL database.
	/// <para>
	/// When executing a command that contains multiple semicolon-separated
	/// statements, an error in any statement will cause the entire command to
	/// be rolled back, even if it is not in an explicit transaction.
	/// </para>
	/// </summary>
	/// <threadsafety instance="false" />
	public sealed class PostgresCommand {
		
		private readonly List<NpgsqlParameter> m_parameters = new List<NpgsqlParameter>();
		private string m_sql;
		
		/// <summary>
		/// Initialize a new Postgres command with the given SQL query.
		/// </summary>
		/// <param name="sql">The text of the query.</param>
		/// <param name="prepared">Prepare the transaction for fast re-use</param>
		public PostgresCommand(
			string sql,
			bool prepared = true
		) {
			m_sql = sql;
			this.Prepared = prepared;
			this.Timeout = null;
		}
		
		/// <summary>
		/// Initialize a new Postgres command with an empty SQL query.
		/// </summary>
		public PostgresCommand() 
			: this( string.Empty, false ) {}
		
		/// <summary>
		/// Initialize a new Postgres command, copying the SQL query text and
		/// parameters from the provided command.
		/// </summary>
		/// <param name="template">The existing command to copy.</param>
		public PostgresCommand( PostgresCommand template )
			: this( template.m_sql, template.Prepared ) {
			m_parameters.AddRange( template.m_parameters );
		}
		
		/// <summary>Append the given text to the SQL query.</summary>
		/// <param name="sql">The text to append to the query.</param>
		/// <seealso cref="AppendLine"/>
		public void Append( string sql ) {
			m_sql = string.Concat( m_sql, sql );
		}
		
		/// <summary>
		/// Append the given text to the SQL query. A new line is automatically
		/// inserted between the current SQL query and the new text.
		/// </summary>
		/// <param name="sql">The text to append to the query.</param>
		/// <seealso cref="Append"/>
		public void AppendLine( string sql ) {
			m_sql = string.Concat( m_sql, Environment.NewLine, sql );
		}
		
		/// <summary>
		/// Add a value to a parameter. To use parameters in your SQL query
		/// string, prefix the parameter name in the SQL text with a colon, then
		/// call this method with the parameter name (without the : prefix) and
		/// value.
		/// 
		/// This method guesses the parameter's database type from its C# type.
		/// For some types (eg. json), it is necessary to explicitly specify the
		/// database type by using the form of this method that takes a database
		/// type as a 3rd argument.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="value">The value to use for the parameter.</param>
		public void AddParameter<T>( string name, T value ) {
			this.AddParameter( name, value, DbTypeConverter<T>.DatabaseType );
		}
		
		/// <summary>
		/// Add a value to a parameter. To use parameters in your SQL query
		/// string, prefix the parameter name in the SQL text with a colon, then
		/// call this method with the parameter name (without the : prefix),
		/// value, and database type.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="value">The value to use for the parameter.</param>
		/// <param name="dbType">The database type of the parameter.</param>
		public void AddParameter<T>( string name, T value, NpgsqlDbType dbType ) {
			var parameter = new NpgsqlParameter(
				parameterName: name,
				value: DbTypeConverter<T>.ToDbValue( value )
			);
			
			parameter.NpgsqlDbType = dbType;
			m_parameters.Add( parameter );
		}
		
		/// <summary>
		/// The time to wait (in seconds) while trying to execute a command
		/// before terminating the attempt and generating an error. Set to 0 for
		/// no time limit. If <c>null</c>, the value of <c>CommandTimeout</c>
		/// from the connection string is used (Defaults to 30 if not specified
		/// in the connection string).
		/// </summary>
		public int? Timeout { get; set; }

		/// <summary>
		/// Whether or not the query should be prepared so that it doesn't need to
		/// be parsed each execution. Defaults to <c>true</c>, but should be disabled
		/// for dynamically generated SQL.
		/// </summary>
		public bool Prepared { get; set; }
		
		
		internal async Task<NpgsqlCommand> BuildAsync(
			NpgsqlConnection connection,
			NpgsqlTransaction transaction = null
		) {
			NpgsqlCommand cmd = new NpgsqlCommand( m_sql, connection, transaction );
			foreach( NpgsqlParameter parameter in m_parameters ) {
				cmd.Parameters.Add( parameter.Clone() );
			}
			
			if( this.Timeout.HasValue ) {
				cmd.CommandTimeout = this.Timeout.Value;
			}

			if( this.Prepared ) {
				await cmd.PrepareAsync().SafeAsync();
			}
			
			return cmd;
		}
		
	}
	
}
