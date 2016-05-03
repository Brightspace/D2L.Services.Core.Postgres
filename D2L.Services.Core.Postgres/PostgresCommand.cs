using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;

namespace D2L.Services.Core.Postgres {
	
	/// <summary>
	/// Represents a SQL statement to execute against a PostgreSQL database.
	/// </summary>
	public sealed class PostgresCommand {
		
		private readonly List<NpgsqlParameter> m_parameters;
		private int? m_timeout = null;
		private string m_sql;
		
		/// <summary>
		/// Initialize a new Postgres command with the given SQL query.
		/// </summary>
		/// <param name="sql">The text of the query.</param>
		public PostgresCommand( string sql ) {
			m_sql = sql;
			m_parameters = new List<NpgsqlParameter>();
		}
		
		/// <summary>
		/// Initialize a new Postgres command with an empty SQL query.
		/// </summary>
		public PostgresCommand() 
			: this( string.Empty ) {}
		
		/// <summary>
		/// Initialize a new Postgres command, copying the SQL query text and
		/// parameters from the provided command.
		/// </summary>
		/// <param name="template">The existing command to copy.</param>
		public PostgresCommand( PostgresCommand template )
			: this( template.m_sql ) {
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
		/// string, prefix the parameter name in the SQL text with a colon. then
		/// call this method using that parameter name (without the : prefix).
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="value">The value to use for the parameter.</param>
		public void AddParameter<T>( string name, T value ) {
			var parameter = new NpgsqlParameter(
				parameterName: name,
				value: ToDbValue( value )
			);
			
			m_parameters.Add( parameter );
		}
		
		/// <summary>
		/// Add a value to a parameter. To use parameters in your SQL query
		/// string, prefix the parameter name in the SQL text with a colon. then
		/// call this method using that parameter name (without the : prefix).
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="value">The value to use for the parameter.</param>
		/// <param name="dbType">The database type of the parameter.</param>
		public void AddParameter<T>( string name, T value, NpgsqlDbType dbType ) {
			var parameter = new NpgsqlParameter(
				parameterName: name,
				value: ToDbValue( value )
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
		public int? Timeout {
			get { return m_timeout; }
			set { m_timeout = value; }
		}
		
		
		internal NpgsqlCommand Build(
			NpgsqlConnection connection,
			NpgsqlTransaction transaction
		) {
			NpgsqlCommand cmd = new NpgsqlCommand( m_sql, connection, transaction );
			foreach( NpgsqlParameter parameter in m_parameters ) {
				cmd.Parameters.Add( parameter );
			}
			
			if( m_timeout.HasValue ) {
				cmd.CommandTimeout = m_timeout.Value;
			}
			
			return cmd;
		}
		
		private static object ToDbValue( object value ) {
			//TODO[v1.1.0] add support for type converters
			if( value == null ) {
				return DBNull.Value;
			} else {
				return value;
			}
		}
		
	}
	
}
