using System;
using System.ComponentModel;
using System.Data;

namespace D2L.Services.Core.Postgres {
	
	/// <summary>
	/// Specifies the isolation level of a Postgres transaction.
	/// </summary>
	public enum PostgresIsolationLevel {
		/// <summary>
		/// The default and least strict isolation level in Postgres. Dirty
		/// reads are not possible, but nonrepeatable reads and phantom reads
		/// may occur.
		/// </summary>
		ReadCommitted,
		
		/// <summary>
		/// At this isolation level, commands within the transaction do not see
		/// any changes from other transactions that began after the start of
		/// the current transaction. This guarantees that dirty reads,
		/// nonrepeatable reads, and phantom reads cannot occur.
		/// 
		/// Transactions performed at this isolation level may need to be rolled
		/// back.
		/// </summary>
		RepeatableRead,
		
		/// <summary>
		/// The strictest isolation level in Postgres. This isolation level
		/// ensures that the result of executing transactions is consistent
		/// across all possible serial orderings of the transactions.
		/// 
		/// Transactions performed at this isolation level may need to be rolled
		/// back.
		/// </summary>
		Serializable
	}
	
	internal static class PostgresIsolationLevel_Extensions {
		
		internal static IsolationLevel ToAdoIsolationLevel(
			this PostgresIsolationLevel level
		) {
			switch( level ) {
				case PostgresIsolationLevel.ReadCommitted:
					return IsolationLevel.ReadCommitted;
				case PostgresIsolationLevel.RepeatableRead:
					return IsolationLevel.RepeatableRead;
				case PostgresIsolationLevel.Serializable:
					return IsolationLevel.Serializable;
				default:
					throw new InvalidEnumArgumentException();
			}
		}
		
	}
	
}
