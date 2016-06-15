using Npgsql;
using System;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres {
	
	/// <summary>
	/// The interface of a transaction against a Postgres database.
	/// </summary>
	/// <threadsafety instance="false" />
	/// <seealso cref="IPostgresDatabase"/>
	public interface IPostgresTransaction : IPostgresExecutor, IDisposable {
		
		/// <summary>
		/// Commit the transaction and close the connection. If the commit
		/// fails, then the transaction will be rolled back. After this call,
		/// the transaction will necessarily be disposed and the connection will
		/// be closed.
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
		/// Rollback the transaction and close the connection. After this call,
		/// the transaction will necessarily be disposed and the connection will
		/// be closed. It is safe to call this method on a transaction which has
		/// already been rolled back (in which case, this call is a NOP);
		/// however, calling this method on a transaction that has successfully
		/// committed is an error and will result in an
		/// <see cref="InvalidOperationException"/> being thrown.
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
		
	}
	
}
