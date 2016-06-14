using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Npgsql;
using System;
using System.Data;

namespace D2L.Services.Core.Postgres {
	
	// Disable warnings about no documentation for 'this' parameters and the
	// PostgresExtensionMethods class itself
	#pragma warning disable 1591
	#pragma warning disable 1573
	
	[EditorBrowsable( EditorBrowsableState.Never )]
	public static class PostgresExtensionMethods {
		
		/// <summary>
		/// Read a field from the database record in the column with the given
		/// name. Null values are automatically converted from
		/// <c>DBNull.Value</c> to <c>null</c>.
		/// </summary>
		/// <param name="columnName">The name of the column to read</param>
		/// <typeparam name="T">The C# data type of the field</typeparam>
		/// <returns>The value of the field</returns>
		public static T Get<T>(
			this IDataRecord record,
			string columnName
		) {
			int index = record.GetOrdinal( columnName );
			return DbTypeConverter.FromDbValue<T>( record.GetValue( index ) );
		}
		
		/// <summary>
		/// Execute a SQL command and return the first column of the result set.
		/// The entire column is loaded into an <see cref="IReadOnlyList{T}"/>
		/// before the task becomes completed. Intended to be used for queries
		/// whose result set contains exactly one column.
		/// </summary>
		/// <param name="command">The SQL command to execute.</param>
		/// <returns>The first column of the result set.</returns>
		/// <exception cref="PostgresException">
		/// The SQL command raises an error. This exception is thrown when an
		/// error is reported by the PostgreSQL backend. Other errors such as
		/// network issues result in an <see cref="NpgsqlException"/> instead,
		/// which is a base class of this exception.
		/// </exception>
		/// <exception cref="NpgsqlException">
		/// This exception is thrown when server-related issues occur.
		/// PostgreSQL specific errors raise a <see cref="PostgresException"/>,
		/// which is a subclass of this exception.
		/// </exception>
		public static Task<IReadOnlyList<T>> ExecReadColumnOfflineAsync<T>(
			this IPostgresExecutor database,
			PostgresCommand command
		) {
			return database.ExecReadOfflineAsync<T>(
				command,
				record => DbTypeConverter.FromDbValue<T>( record.GetValue( 0 ) )
			);
		}
		
		/// <summary>
		/// Gets the <see cref="PostgresErrorClass"/> of the current
		/// <see cref="PostgresException"/>.
		/// </summary>
		/// <returns>
		/// The <see cref="PostgresErrorClass"/> of the current
		/// <see cref="PostgresException"/>
		/// </returns>
		public static PostgresErrorClass GetErrorClass(
			this PostgresException exception
		) {
			switch( exception.SqlState.Substring( 0, 2 ) ) {
				case "00": return PostgresErrorClass.SuccessfulCompletion;
				case "01": return PostgresErrorClass.Warning;
				case "02": return PostgresErrorClass.NoData;
				case "03": return PostgresErrorClass.SqlStatementNotYetComplete;
				case "08": return PostgresErrorClass.ConnectionException;
				case "09": return PostgresErrorClass.TriggeredActionException;
				case "0A": return PostgresErrorClass.FeatureNotSupported;
				case "0B": return PostgresErrorClass.InvalidTransactionInitiation;
				case "0F": return PostgresErrorClass.LocatorException;
				case "0L": return PostgresErrorClass.InvalidGrantor;
				case "0P": return PostgresErrorClass.InvalidRoleSpecification;
				case "0Z": return PostgresErrorClass.DiagnosticsException;
				case "20": return PostgresErrorClass.CaseNotFound;
				case "21": return PostgresErrorClass.CardinalityViolation;
				case "22": return PostgresErrorClass.DataException;
				case "23": return PostgresErrorClass.IntegrityConstraintViolation;
				case "24": return PostgresErrorClass.InvalidCursorState;
				case "25": return PostgresErrorClass.InvalidTransactionState;
				case "26": return PostgresErrorClass.InvalidSqlStatementName;
				case "27": return PostgresErrorClass.TriggeredDataChangeViolation;
				case "28": return PostgresErrorClass.InvalidAuthorizationSpecification;
				case "2B": return PostgresErrorClass.DependentPrivilegeDescriptorsStillExist;
				case "2D": return PostgresErrorClass.InvalidTransactionTermination;
				case "2F": return PostgresErrorClass.SqlRoutineException;
				case "34": return PostgresErrorClass.InvalidCursorName;
				case "38": return PostgresErrorClass.ExternalRoutineException;
				case "39": return PostgresErrorClass.ExternalRoutineInvocationException;
				case "3B": return PostgresErrorClass.SavepointException;
				case "3D": return PostgresErrorClass.InvalidCatalogName;
				case "3F": return PostgresErrorClass.InvalidSchemaName;
				case "40": return PostgresErrorClass.TransactionRollback;
				case "42": return PostgresErrorClass.SyntaxErrorOrAccessRuleViolation;
				case "44": return PostgresErrorClass.WithCheckOptionViolation;
				case "53": return PostgresErrorClass.InsufficientResources;
				case "54": return PostgresErrorClass.ProgramLimitExceeded;
				case "55": return PostgresErrorClass.ObjectNotInPrerequisiteState;
				case "57": return PostgresErrorClass.OperatorIntervention;
				case "58": return PostgresErrorClass.SystemError;
				case "F0": return PostgresErrorClass.ConfigurationFileError;
				case "HV": return PostgresErrorClass.ForeignDataWrapperError;
				case "P0": return PostgresErrorClass.PlPgsqlError;
				case "XX": return PostgresErrorClass.InternalError;
				default:   return PostgresErrorClass.UnknownErrorClass;
			}
		}
		
	}
	
	#pragma warning restore 1573
	#pragma warning restore 1591
	
}
