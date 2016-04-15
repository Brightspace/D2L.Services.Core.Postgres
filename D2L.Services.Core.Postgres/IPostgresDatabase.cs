using System;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres {
	
	/// <summary>
	/// A class that provides methods to execute commands against a Postgres
	/// database. Each command is executed in a new connection.
	/// </summary>
	public interface IPostgresDatabase : IPostgresExecutor {
		
		/// <summary>
		/// Begins a new transaction with the given isolation level. The
		/// transaction is automatically rolled back if it is disposed before
		/// <c>Commit()</c> is called.
		/// </summary>
		/// <param name="isolationLevel">The isolation level.</param>
		/// <returns>The new transaction.</returns>
		IPostgresTransaction NewTransaction(
			PostgresIsolationLevel isolationLevel = PostgresIsolationLevel.ReadCommitted
		);
		
		/// <summary>
		/// Begins a new transaction with the given isolation level. The
		/// transaction is automatically rolled back if it is disposed before
		/// <c>Commit()</c> is called.
		/// </summary>
		/// <param name="isolationLevel">The isolation level.</param>
		/// <returns>The new transaction.</returns>
		Task<IPostgresTransaction> NewTransactionAsync(
			PostgresIsolationLevel isolationLevel = PostgresIsolationLevel.ReadCommitted
		);
		
	}
	
}
