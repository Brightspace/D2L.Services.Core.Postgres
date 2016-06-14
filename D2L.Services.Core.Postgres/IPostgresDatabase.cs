using System;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres {
	
	/// <summary>
	/// The interface of a Postgres database. Each command is executed in a new
	/// connection.
	/// </summary>
	public interface IPostgresDatabase : IPostgresExecutor {
		
		/// <summary>
		/// Begins a new transaction with the given isolation level. The
		/// transaction is automatically rolled back if it is disposed before
		/// <c>Commit()</c> is called.
		/// </summary>
		/// <remarks>
		/// If a rollback happens as a result of the transaction falling out of
		/// scope of a <c>using</c> block or by explicitly calling
		/// <see cref="IDisposable.Dispose"/> on the transaction, the rollback
		/// is performed synchronously. Rollbacks that occur as a result of a
		/// failed commit or a command throwing an exception are always done
		/// asynchronously, but you should ensure that your own code does not 
		/// otherwise dispose of a transaction without first calling
		/// <see cref="IPostgresTransaction.CommitAsync"/> or
		/// <see cref="IPostgresTransaction.RollbackAsync"/>.
		/// </remarks>
		/// <param name="isolationLevel">The isolation level.</param>
		/// <returns>The new transaction.</returns>
		Task<IPostgresTransaction> NewTransactionAsync(
			PostgresIsolationLevel isolationLevel = PostgresIsolationLevel.ReadCommitted
		);
		
	}
	
}
