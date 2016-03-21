
using System;

namespace D2L.Services.Core.Postgres {
	
	/// <summary>Identifies the error class of a Postgres error</summary>
	public enum PostgresErrorClass {
		
		/// <summary>Class 00 - Successful Completion</summary>
		SuccessfulCompletion,
		
		/// <summary>Class 01 - Warning</summary>
		Warning,
		
		/// <summary>Class 02 - No Data</summary>
		NoData,
		
		/// <summary>Class 03 - SQL Statement Not Yet Complete</summary>
		SqlStatementNotYetComplete,
		
		/// <summary>Class 08 - Connection Exception</summary>
		ConnectionException,
		
		/// <summary>Class 09 - Triggered Action Exception</summary>
		TriggeredActionException,
		
		/// <summary>Class 0A - Feature Not Supported</summary>
		FeatureNotSupported,
		
		/// <summary>Class 0B - Invalid Transaction Initiation</summary>
		InvalidTransactionInitiation,
		
		/// <summary>Class 0F - Locator Exception</summary>
		LocatorException,
		
		/// <summary>Class 0L - Invalid Grantor</summary>
		InvalidGrantor,
		
		/// <summary>Class 0P - Invalid Role Specification</summary>
		InvalidRoleSpecification,
		
		/// <summary>Class 0Z - Diagnostics Exception</summary>
		DiagnosticsException,
		
		/// <summary>Class 20 - Case Not Found</summary>
		CaseNotFound,
		
		/// <summary>Class 21 - Cardinality Violation</summary>
		CardinalityViolation,
		
		/// <summary>Class 22 - Data Exception</summary>
		DataException,
		
		/// <summary>Class 23 - Integrity Constraint Violation</summary>
		IntegrityConstraintViolation,
		
		/// <summary>Class 24 - Invalid Cursor State</summary>
		InvalidCursorState,
		
		/// <summary>Class 25 - Invalid Transaction State</summary>
		InvalidTransactionState,
		
		/// <summary>Class 26 - Invalid SQL Statement Name</summary>
		InvalidSqlStatementName,
		
		/// <summary>Class 27 - Triggered Data Change Violation</summary>
		TriggeredDataChangeViolation,
		
		/// <summary>Class 28 - Invalid Authorization Specification</summary>
		InvalidAuthorizationSpecification,
		
		/// <summary>Class 2B - Dependent Privilege Descriptors Still Exist</summary>
		DependentPrivilegeDescriptorsStillExist,
		
		/// <summary>Class 2D - Invalid Transaction Termination</summary>
		InvalidTransactionTermination,
		
		/// <summary>Class 2F - SQL Routine Exception</summary>
		SqlRoutineException,
		
		/// <summary>Class 34 - Invalid Cursor Name</summary>
		InvalidCursorName,
		
		/// <summary>Class 38 - External Routine Exception</summary>
		ExternalRoutineException,
		
		/// <summary>Class 39 - External Routine Invocation Exception</summary>
		ExternalRoutineInvocationException,
		
		/// <summary>Class 3B - Savepoint Exception</summary>
		SavepointException,
		
		/// <summary>Class 3D - Invalid Catalog Name</summary>
		InvalidCatalogName,
		
		/// <summary>Class 3F - Invalid Schema Name</summary>
		InvalidSchemaName,
		
		/// <summary>Class 40 - Transaction Rollback</summary>
		TransactionRollback,
		
		/// <summary>Class 42 - Syntax Error or Access Rule Violation</summary>
		SyntaxErrorOrAccessRuleViolation,
		
		/// <summary>Class 44 - WITH CHECK OPTION Violation</summary>
		WithCheckOptionViolation,
		
		/// <summary>Class 53 - Insufficient Resources</summary>
		InsufficientResources,
		
		/// <summary>Class 54 - Program Limit Exceeded</summary>
		ProgramLimitExceeded,
		
		/// <summary>Class 55 - Object Not In Prerequisite State</summary>
		ObjectNotInPrerequisiteState,
		
		/// <summary>Class 57 - Operator Intervention</summary>
		OperatorIntervention,
		
		/// <summary>Class 58 - System Error (errors external to PostgreSQL itself)</summary>
		SystemError,
		
		/// <summary>Class F0 - Configuration File Error</summary>
		ConfigurationFileError,
		
		/// <summary>Class HV - Foreign Data Wrapper Error (SQL/MED)</summary>
		ForeignDataWrapperError,
		
		/// <summary>Class P0 - PL/pgSQL Error</summary>
		PlPgsqlError,
		
		/// <summary>Class XX - Internal Error</summary>
		InternalError,
		
		/// <summary>Unknown Error Class</summary>
		UnknownErrorClass
		
	}
}