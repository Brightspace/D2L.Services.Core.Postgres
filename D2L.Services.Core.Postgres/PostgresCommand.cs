using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;

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
		private int? m_timeout = null;
		private string m_sql;
		
		/// <summary>
		/// Initialize a new Postgres command with the given SQL query.
		/// </summary>
		/// <param name="sql">The text of the query.</param>
		public PostgresCommand( string sql ) {
			m_sql = sql;
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
			if( value is DateTime ) {
				switch( ((DateTime)(object)value).Kind ) {
					case DateTimeKind.Utc:
						AddParameter<T>( name, value, NpgsqlDbType.Timestamp );
						return;
					case DateTimeKind.Local:
						AddParameter<T>( name, value, NpgsqlDbType.TimestampTZ );
						return;
					default:
						throw new ArgumentException(
							message: "Cannot infer Postgres type from a DateTime of kind Unspecified.",
							paramName: "value"
						);
				}
			}
			
			var parameter = new NpgsqlParameter(
				parameterName: name,
				value: DbTypeConverter.ToDbValue( value )
			);
			
			m_parameters.Add( parameter );
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
				value: DbTypeConverter.ToDbValue( value )
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
			NpgsqlTransaction transaction = null
		) {
			NpgsqlCommand cmd = new NpgsqlCommand( m_sql, connection, transaction );
			foreach( NpgsqlParameter parameter in m_parameters ) {
				cmd.Parameters.Add( parameter.Clone() );
			}
			
			if( m_timeout.HasValue ) {
				cmd.CommandTimeout = m_timeout.Value;
			}
			
			return cmd;
		}
		
	}
	
}
