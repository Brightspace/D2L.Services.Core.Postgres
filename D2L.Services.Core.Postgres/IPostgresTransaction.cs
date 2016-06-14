using Npgsql;
using System;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres {
	
	/// <summary>
	/// The interface of a transaction against a Postgres database.
	/// </summary>
	/// <seealso cref="IPostgresDatabase"/>
	public interface IPostgresTransaction : IPostgresExecutor, IDisposable {
		
		/// <summary>
		/// Commit the transaction and close the connection. Calling any methods
		/// on the transaction after this point will result in an
		/// <see cref="ObjectDisposedException"/> being thrown.
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
		Task CommitAsync();
		
		/// <summary>
		/// Rollback the transaction and close the connection. Calling any
		/// methods on the transaction after this point (except for another
		/// <c>RollbackAsync()</c> call, which will do nothing) will result in
		/// an <see cref="ObjectDisposedException"/> being thrown. If the
		/// transaction has already been rolled back, the method returns
		/// immediately.
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
		/// <exception cref="ObjectDisposedException">
		/// The transaction has already been successfully committed and cannot
		/// be rolled back.
		/// </exception>
		Task RollbackAsync();
		
	}
	
}
