using Npgsql;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres {
	
	/// <summary>
	/// A class that represents a transaction to be made in a Postgres database.
	/// </summary>
	/// <seealso cref="IPostgresDatabase"/>
	public interface IPostgresTransaction : IPostgresExecutor, IDisposable {
		
		/// <summary>
		/// Commit the transaction and close the connection. Calling any methods
		/// on the transaction after this point will result in an
		/// <see cref="ObjectDisposedException"/> being thrown.
		/// </summary>
		/// <exception cref="PostgresException">
		/// The commit raises an error. This exception is thrown when an
		/// error is reported by the PostgreSQL backend. Other errors such as
		/// network issues result in an <see cref="NpgsqlException"/> instead.
		/// </exception>
		/// <exception cref="NpgsqlException">
		/// An error unrelated to PostgreSQL occurs (eg. network disconnection).
		/// For errors thrown by the PostgreSQL backend, a
		/// <see cref="PostgresException"/> is thrown instead. To catch both
		/// types of problems, catch type <see cref="DbException"/>.
		/// </exception>
		Task CommitAsync();
		
		/// <summary>
		/// Rollback the transaction and close the connection. Calling any
		/// methods on the transaction after this point will result in an
		/// <see cref="ObjectDisposedException"/> being thrown.
		/// </summary>
		/// <exception cref="PostgresException">
		/// The rollback raises an error. This exception is thrown when an
		/// error is reported by the PostgreSQL backend. Other errors such as
		/// network issues result in an <see cref="NpgsqlException"/> instead.
		/// </exception>
		/// <exception cref="NpgsqlException">
		/// An error unrelated to PostgreSQL occurs (eg. network disconnection).
		/// For errors thrown by the PostgreSQL backend, a
		/// <see cref="PostgresException"/> is thrown instead. To catch both
		/// types of problems, catch type <see cref="DbException"/>.
		/// </exception>
		Task RollbackAsync();
		
		/// <summary>
		/// Disposes the <see cref="IPostgresTransaction"/>. If a rollback must
		/// be performed, it is done asynchronously.
		/// </summary>
		/// <remarks>
		/// It is safe to call this on an <see cref="IPostgresTransaction"/>
		/// that has already been committed, rolled back, or disposed. In this
		/// case, the method does nothing and returns immediately.
		/// </remarks>
		/// <exception cref="PostgresException">
		/// The rollback raises an error. This exception is thrown when an
		/// error is reported by the PostgreSQL backend. Other errors such as
		/// network issues result in an <see cref="NpgsqlException"/> instead.
		/// </exception>
		/// <exception cref="NpgsqlException">
		/// An error unrelated to PostgreSQL occurs (eg. network disconnection).
		/// For errors thrown by the PostgreSQL backend, a
		/// <see cref="PostgresException"/> is thrown instead. To catch both
		/// types of problems, catch type <see cref="DbException"/>.
		/// </exception>
		Task DisposeAsync();
		
		//TODO[v1.3.0] Add support for savepoints.
		
	}
	
}
