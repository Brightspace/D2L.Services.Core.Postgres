using Npgsql;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres {

	/// <summary>
	/// The interface of a transaction against a Postgres database.
	/// </summary>
	/// <threadsafety instance="false" />
	/// <seealso cref="IPostgresDatabase"/>
	public interface IPostgresTransaction : IPostgresExecutor, IAsyncDisposable {
		
		/// <summary>
		/// Commit the transaction and close the connection. If the commit
		/// fails, then the transaction will be rolled back.
		/// <para>
		/// PRECONDITION: The transaction has not yet been disposed.
		/// </para>
		/// <para>
		/// POSTCONDITION: The transaction is disposed, and the connection is
		/// closed.
		/// </para>
		/// </summary>
		/// <exception cref="PostgresException">
		/// The commit raises an error. This exception is thrown when an error
		/// is reported by the PostgreSQL backend. Other errors such as network
		/// issues result in an <see cref="NpgsqlException"/> instead, which is
		/// a base class of this exception.
		/// </exception>
		/// <exception cref="NpgsqlException">
		/// This exception is thrown when server-related issues occur.
		/// PostgreSQL specific errors raise a <see cref="PostgresException"/>,
		/// which is a subclass of this exception.
		/// </exception>
		/// <exception cref="AggregateException">
		/// The transaction failed to commit, then the attempt to rollback the
		/// transaction also failed. This likely means that the connection to
		/// the database server was lost.
		/// </exception>
		Task CommitAsync();
		
		/// <summary>
		/// Rollback the transaction and close the connection. It is safe to
		/// call this method on a transaction which has already been rolled back
		/// (in which case, this call is a NOP).
		/// <para>
		/// PRECONDITION: The transaction has not been (successfully) committed
		/// </para>
		/// <para>
		/// POSTCONDITION: The transaction is disposed, and the connection is
		/// closed.
		/// </para>
		/// </summary>
		/// <exception cref="PostgresException">
		/// The rollback raises an error. This exception is thrown when an error
		/// is reported by the PostgreSQL backend. Other errors such as network
		/// issues result in an <see cref="NpgsqlException"/> instead, which is
		/// a base class of this exception.
		/// </exception>
		/// <exception cref="NpgsqlException">
		/// This exception is thrown when server-related issues occur.
		/// PostgreSQL specific errors raise a <see cref="PostgresException"/>,
		/// which is a subclass of this exception.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// The transaction has already been successfully committed and cannot
		/// be rolled back.
		/// </exception>
		Task RollbackAsync();

		/// <summary>
		/// Safely get a handle for the transaction that you can await
		/// </summary>
		/// <example>
		/// <code>await using var handle = transaction.Handle;</code>
		/// </example>
		ConfiguredAsyncDisposable Handle { get; }
		
	}
	
}
