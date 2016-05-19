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
		/// <remarks>
		/// If a rollback happens as a result of the transaction being disposed,
		/// the rollback is performed synchronously. To enforce asynchronous
		/// rollbacks, call <see cref="IPostgresTransaction.RollbackAsync"/>
		/// on the transaction before disposing it.
		/// </remarks>
		/// <param name="isolationLevel">The isolation level.</param>
		/// <returns>The new transaction.</returns>
		Task<IPostgresTransaction> NewTransactionAsync(
			PostgresIsolationLevel isolationLevel = PostgresIsolationLevel.ReadCommitted
		);
		
	}
	
}
