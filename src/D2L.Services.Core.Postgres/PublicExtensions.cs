using Npgsql;
using System;
using System.Data;

namespace D2L.Services.Core.Postgres {
	
	#pragma warning disable 1591
	// Disable warning about no documentation for PostgresExtensionMethods class
	
	public static class PostgresExtensionMethods {
		
		/// <summary>
		/// Read a field from the database record under the column with the
		/// given name. Null values are automatically converted from
		/// <c>DBNull.Value</c> to <c>null</c>.
		/// </summary>
		/// <param name="record">The database data record</param>
		/// <param name="columnName">The name of the column to read</param>
		/// <typeparam name="T">The C# data type of the field</typeparam>
		/// <returns>The value of the field</returns>
		public static T Get<T>(
			this IDataRecord record,
			string columnName
		) {
			//TODO[v1.1.0] add support for type converters
			int index = record.GetOrdinal( columnName );
			if( record.IsDBNull( index ) ) {
				return (T)(object)null;
			} else {
				return (T)record.GetValue( index );
			}
		}
		
		/// <summary>Gets the error class of a Postgres error.</summary>
		/// <param name="exception">The exception</param>
		/// <returns>The error class of the Postgres error</returns>
		public static PostgresErrorClass GetErrorClass(
			this NpgsqlException exception
		) {
			switch( exception.Code.Substring( 0, 2 ) ) {
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
	
	#pragma warning restore 1591
	
}
