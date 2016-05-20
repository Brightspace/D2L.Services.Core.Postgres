using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres {
	
	/// <summary>
	/// A base interface for methods common to <see cref="IPostgresDatabase"/>
	/// and <see cref="IPostgresTransaction"/>. See those interfaces for more
	/// information.
	/// </summary>
	public interface IPostgresExecutor {
		
		/// <summary>
		/// Execute a SQL command, but do not read the results.
		/// </summary>
		/// <param name="command">The SQL command to execute.</param>
		/// <returns>The number of rows affected by the query.</returns>
		/// <exception cref="PostgresException">
		/// The SQL command raises an error. This exception is thrown when an
		/// error is reported by the PostgreSQL backend. Other errors such as
		/// network issues result in an <see cref="NpgsqlException"/> instead.
		/// </exception>
		/// <exception cref="NpgsqlException">
		/// An error unrelated to PostgreSQL occurs (eg. network disconnection).
		/// For errors thrown by the PostgreSQL backend, a
		/// <see cref="PostgresException"/> is thrown instead. To catch both
		/// types of problems, catch type <see cref="DbException"/>.
		/// </exception>
		Task<int> ExecNonQueryAsync( PostgresCommand command );
		
		/// <summary>
		/// Execute a SQL command and return the first column of the first row
		/// of the result set. Throws an exception if the result set is empty.
		/// </summary>
		/// <param name="command">The SQL command to execute.</param>
		/// <returns>
		/// The first column of the first row of the result set.
		/// </returns>
		/// <exception cref="DataNotFoundException">
		/// The result set is empty
		/// </exception>
		/// <exception cref="PostgresException">
		/// The SQL command raises an error. This exception is thrown when an
		/// error is reported by the PostgreSQL backend. Other errors such as
		/// network issues result in an <see cref="NpgsqlException"/> instead.
		/// </exception>
		/// <exception cref="NpgsqlException">
		/// An error unrelated to PostgreSQL occurs (eg. network disconnection).
		/// For errors thrown by the PostgreSQL backend, a
		/// <see cref="PostgresException"/> is thrown instead. To catch both
		/// types of problems, catch type <see cref="DbException"/>.
		/// </exception>
		Task<T> ExecReadScalarAsync<T>( PostgresCommand command );
		
		/// <summary>
		/// Execute a SQL command and return the first column of the first row
		/// of the result set. If the result set is empty, the given default
		/// value is returned instead.
		/// </summary>
		/// <param name="command">The SQL command to execute.</param>
		/// <param name="defaultValue">
		/// The value to return if the result set is empty
		/// </param>
		/// <returns>
		/// The first column of the first row of the result set, or
		/// <paramref name="defaultValue"/> if the result set is empty.
		/// </returns>
		/// <exception cref="PostgresException">
		/// The SQL command raises an error. This exception is thrown when an
		/// error is reported by the PostgreSQL backend. Other errors such as
		/// network issues result in an <see cref="NpgsqlException"/> instead.
		/// </exception>
		/// <exception cref="NpgsqlException">
		/// An error unrelated to PostgreSQL occurs (eg. network disconnection).
		/// For errors thrown by the PostgreSQL backend, a
		/// <see cref="PostgresException"/> is thrown instead. To catch both
		/// types of problems, catch type <see cref="DbException"/>.
		/// </exception>
		Task<T> ExecReadScalarOrDefaultAsync<T>(
			PostgresCommand command,
			T defaultValue = default( T )
		);
		
		/// <summary>
		/// Execute a SQL command and return the first record in the result set.
		/// Throws an exception if the result set is empty.
		/// </summary>
		/// <param name="command">The SQL command to execute.</param>
		/// <param name="dbConverter">A converter for the data record.</param>
		/// <returns>The first record in the result set.</returns>
		/// <exception cref="DataNotFoundException">
		/// The result set is empty
		/// </exception>
		/// <exception cref="PostgresException">
		/// The SQL command raises an error. This exception is thrown when an
		/// error is reported by the PostgreSQL backend. Other errors such as
		/// network issues result in an <see cref="NpgsqlException"/> instead.
		/// </exception>
		/// <exception cref="NpgsqlException">
		/// An error unrelated to PostgreSQL occurs (eg. network disconnection).
		/// For errors thrown by the PostgreSQL backend, a
		/// <see cref="PostgresException"/> is thrown instead. To catch both
		/// types of problems, catch type <see cref="DbException"/>.
		/// </exception>
		Task<Dto> ExecReadFirstAsync<Dto>(
			PostgresCommand command,
			Func<IDataRecord,Dto> dbConverter
		);
		
		/// <summary>
		/// Execute a SQL command and return the first record in the result set.
		/// If the result set is empty, returns the given default value.
		/// </summary>
		/// <param name="command">The SQL command to execute.</param>
		/// <param name="dbConverter">A converter for the data record.</param>
		/// <param name="defaultValue">
		/// The default value to return if the result set is empty.
		/// </param>
		/// <returns>
		/// The first record in the result set, or
		/// <paramref name="defaultValue"/> if the result set is empty.
		/// </returns>
		/// <exception cref="PostgresException">
		/// The SQL command raises an error. This exception is thrown when an
		/// error is reported by the PostgreSQL backend. Other errors such as
		/// network issues result in an <see cref="NpgsqlException"/> instead.
		/// </exception>
		/// <exception cref="NpgsqlException">
		/// An error unrelated to PostgreSQL occurs (eg. network disconnection).
		/// For errors thrown by the PostgreSQL backend, a
		/// <see cref="PostgresException"/> is thrown instead. To catch both
		/// types of problems, catch type <see cref="DbException"/>.
		/// </exception>
		Task<Dto> ExecReadFirstOrDefaultAsync<Dto>(
			PostgresCommand command,
			Func<IDataRecord,Dto> dbConverter,
			Dto defaultValue = default( Dto )
		);
		
		/// <summary>
		/// Execute a SQL command and return the result set. The entire result
		/// set is loaded into an <see cref="IReadOnlyList{Dto}"/> before the
		/// task becomes completed.
		/// </summary>
		/// <param name="command">The SQL command to execute.</param>
		/// <param name="dbConverter">A converter for the data record.</param>
		/// <returns>The result set.</returns>
		/// <exception cref="PostgresException">
		/// The SQL command raises an error. This exception is thrown when an
		/// error is reported by the PostgreSQL backend. Other errors such as
		/// network issues result in an <see cref="NpgsqlException"/> instead.
		/// </exception>
		/// <exception cref="NpgsqlException">
		/// An error unrelated to PostgreSQL occurs (eg. network disconnection).
		/// For errors thrown by the PostgreSQL backend, a
		/// <see cref="PostgresException"/> is thrown instead. To catch both
		/// types of problems, catch type <see cref="DbException"/>.
		/// </exception>
		Task<IReadOnlyList<Dto>> ExecReadOfflineAsync<Dto>(
			PostgresCommand command,
			Func<IDataRecord,Dto> dbConverter
		);
		
	}
	
}
