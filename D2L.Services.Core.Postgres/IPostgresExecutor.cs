using D2L.Services.Core.Postgres.Enumeration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres {
	
	/// <summary>
	/// A base interface for methods common to <see cref="IPostgresDatabase"/>
	/// and <see cref="IPostgresTransaction"/>. See those interfaces for more
	/// information.
	/// </summary>
	public interface IPostgresExecutor {
		
		#region sync
		/// <summary>
		/// Execute a SQL command, but do not read the results.
		/// </summary>
		/// <param name="command">The SQL command to execute.</param>
		/// <returns>The number of rows affected by the query.</returns>
		/// <exception cref="NpgsqlException">
		/// The SQL command raises an error
		/// </exception>
		int ExecNonQuery( PostgresCommand command );
		
		/// <summary>
		/// Execute a SQL command and return the first column of the first row
		/// of the result set. Throws an exception if the result set is empty.
		/// </summary>
		/// <param name="command">The SQL command to execute.</param>
		/// <returns>
		/// The first column of the first row of the result set.
		/// </returns>
		/// <exception cref="NpgsqlException">
		/// The SQL command raises an error
		/// </exception>
		/// <exception cref="DataNotFoundException">
		/// The result set is empty
		/// </exception>
		T ExecReadScalar<T>( PostgresCommand command );
		
		/// <summary>
		/// Execute a SQL command and return the first column of the first row
		/// of the result set. If the result set is empty, the given default
		/// value is returned instead.
		/// </summary>
		/// <returns>
		/// The first column of the first row of the result set, or
		/// <paramref name="defaultValue"/> if there are no results.
		/// </returns>
		/// <param name="command">The SQL command to execute.</param>
		/// <param name="defaultValue">
		/// The value to return if the result set is empty
		/// </param>
		/// <exception cref="NpgsqlException">
		/// The SQL command raises an error
		/// </exception>
		T ExecReadScalarOrDefault<T>(
			PostgresCommand command,
			T defaultValue = default( T )
		);
		
		/// <summary>
		/// Execute a SQL command and return whether or not there were any
		/// results. If there is at least one record in the result set, the
		/// first column of the first row is placed in <paramref name="value"/>.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the result set is non-empty. <c>false</c> otherwise.
		/// </returns>
		/// <param name="command">The SQL command to execute.</param>
		/// <param name="value">
		/// The first column of the first row in the result set.
		/// </param>
		/// <exception cref="NpgsqlException">
		/// The SQL command raises an error
		/// </exception>
		bool ExecTryReadScalar<T>(
			PostgresCommand command,
			out T value
		);
		
		/// <summary>
		/// Execute a SQL command and return the first record in the result set.
		/// Throws an exception if the result set is empty.
		/// </summary>
		/// <returns>The first record in the result set.</returns>
		/// <param name="command">The SQL command to execute.</param>
		/// <param name="dbConverter">A converter for the data record.</param>
		/// <exception cref="NpgsqlException">
		/// The SQL command raises an error
		/// </exception>
		/// <exception cref="DataNotFoundException">
		/// The result set is empty
		/// </exception>
		Dto ExecReadFirst<Dto>(
			PostgresCommand command,
			Func<IDataRecord,Dto> dbConverter
		);
		
		/// <summary>
		/// Execute a SQL command and return the first record in the result set.
		/// If the result set is empty, returns the given default value.
		/// </summary>
		/// <returns>The first record in the result set.</returns>
		/// <param name="command">The SQL command to execute.</param>
		/// <param name="dbConverter">A converter for the data record.</param>
		/// <param name="defaultValue">
		/// The default value to return if the result set is empty.
		/// </param>
		/// <exception cref="NpgsqlException">
		/// The SQL command raises an error
		/// </exception>
		Dto ExecReadFirstOrDefault<Dto>(
			PostgresCommand command,
			Func<IDataRecord,Dto> dbConverter,
			Dto defaultValue = default( Dto )
		);
		
		/// <summary>
		/// Execute a SQL command and return whether or not there were any
		/// results. If there is at least one record in the result set, the
		/// first record is placed in <paramref name="dto"/>.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the result set is non-empty. <c>false</c> otherwise.
		/// </returns>
		/// <param name="command">The SQL command to execute.</param>
		/// <param name="dbConverter">A converter for the data record.</param>
		/// <param name="dto">The first record in the result set.</param>
		/// <exception cref="NpgsqlException">
		/// The SQL command raises an error
		/// </exception>
		bool ExecTryReadFirst<Dto>(
			PostgresCommand command,
			Func<IDataRecord,Dto> dbConverter,
			out Dto dto
		);
		
		/// <summary>
		/// Execute a SQL command and return the result set. The entire result
		/// set is immediately loaded into an <see cref="IReadOnlyList{Dto}"/>.
		/// To load the results one by one, use the
		/// <see cref="ExecReadOnline{Dto}"/> method instead.
		/// </summary>
		/// <param name="command">The SQL command to execute.</param>
		/// <param name="dbConverter">A converter for the data record.</param>
		/// <returns>The result set.</returns>
		IReadOnlyList<Dto> ExecReadOffline<Dto>(
			PostgresCommand command,
			Func<IDataRecord,Dto> dbConverter
		);
		
		/// <summary>
		/// Execute a SQL command and return the result set. Records in the
		/// result set are read one at a time as the
		/// <see cref="IOnlineResultSet{Dto}"/> is enumerated, and the database
		/// connection is held open until the result set is disposed, which is
		/// done when it is fully enumerated or when <c>Dispose()</c> is called
		/// explicitly.
		/// 
		/// When this method is called on an <see cref="IPostgresTransaction"/>,
		/// the returned <see cref="IOnlineResultSet{Dto}"/> becomes responsible
		/// for committing the transaction and closing the connection. No
		/// further methods may be called on the transaction after calling
		/// <c>ExecReadOnline</c>, and the underlying database connection and
		/// transaction will not be closed when the
		/// <see cref="IPostgresTransaction"/> is disposed. Instead, the
		/// transaction is automatically commited once the
		/// <see cref="IOnlineResultSet{Dto}"/> is disposed.
		/// </summary>
		/// <param name="command">The SQL command to execute.</param>
		/// <param name="dbConverter">A converter for the data record.</param>
		/// <returns>An iterator over the result set.</returns>
		IOnlineResultSet<Dto> ExecReadOnline<Dto>(
			PostgresCommand command,
			Func<IDataRecord,Dto> dbConverter
		);
		#endregion
		
		#region async
		/// <summary>
		/// Execute a SQL command, but do not read the results.
		/// </summary>
		/// <param name="command">The SQL command to execute.</param>
		/// <returns>The number of rows affected by the query.</returns>
		/// <exception cref="NpgsqlException">
		/// The SQL command raises an error
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
		/// <exception cref="NpgsqlException">
		/// The SQL command raises an error
		/// </exception>
		/// <exception cref="DataNotFoundException">
		/// The result set is empty
		/// </exception>
		Task<T> ExecReadScalarAsync<T>( PostgresCommand command );
		
		/// <summary>
		/// Execute a SQL command and return the first column of the first row
		/// of the result set. If the result set is empty, the given default
		/// value is returned instead.
		/// </summary>
		/// <returns>
		/// The first column of the first row of the result set, or
		/// <paramref name="defaultValue"/> if there are no results.
		/// </returns>
		/// <param name="command">The SQL command to execute.</param>
		/// <param name="defaultValue">
		/// The value to return if the result set is empty
		/// </param>
		/// <exception cref="NpgsqlException">
		/// The SQL command raises an error
		/// </exception>
		Task<T> ExecReadScalarOrDefaultAsync<T>(
			PostgresCommand command,
			T defaultValue = default( T )
		);
		
		/// <summary>
		/// Execute a SQL command and return the first record in the result set.
		/// Throws an exception if the result set is empty.
		/// </summary>
		/// <returns>The first record in the result set.</returns>
		/// <param name="command">The SQL command to execute.</param>
		/// <param name="dbConverter">A converter for the data record.</param>
		/// <exception cref="NpgsqlException">
		/// The SQL command raises an error
		/// </exception>
		/// <exception cref="DataNotFoundException">
		/// The result set is empty
		/// </exception>
		Task<Dto> ExecReadFirstAsync<Dto>(
			PostgresCommand command,
			Func<IDataRecord,Dto> dbConverter
		);
		
		/// <summary>
		/// Execute a SQL command and return the first record in the result set.
		/// If the result set is empty, returns the given default value.
		/// </summary>
		/// <returns>The first record in the result set.</returns>
		/// <param name="command">The SQL command to execute.</param>
		/// <param name="dbConverter">A converter for the data record.</param>
		/// <param name="defaultValue">
		/// The default value to return if the result set is empty.
		/// </param>
		/// <exception cref="NpgsqlException">
		/// The SQL command raises an error
		/// </exception>
		Task<Dto> ExecReadFirstOrDefaultAsync<Dto>(
			PostgresCommand command,
			Func<IDataRecord,Dto> dbConverter,
			Dto defaultValue = default( Dto )
		);
		
		/// <summary>
		/// Execute a SQL command and return the result set. The entire result
		/// set is loaded into an <see cref="IReadOnlyList{Dto}"/> before the
		/// task becomes completed. To read the results one by one, use the
		/// <see cref="ExecReadOnlineAsync{Dto}"/> method instead.
		/// </summary>
		/// <param name="command">The SQL command to execute.</param>
		/// <param name="dbConverter">A converter for the data record.</param>
		/// <returns>The result set.</returns>
		Task<IReadOnlyList<Dto>> ExecReadOfflineAsync<Dto>(
			PostgresCommand command,
			Func<IDataRecord,Dto> dbConverter
		);
		
		/// <summary>
		/// Execute a SQL command and return the result set. Records in the
		/// result set are read one at a time as the
		/// <see cref="IOnlineResultSet{Dto}"/> is enumerated, and the database
		/// connection is held open until the result set is disposed, which is
		/// done when it is fully enumerated or when <c>Dispose()</c> is called
		/// explicitly.
		/// 
		/// When this method is called on an <see cref="IPostgresTransaction"/>,
		/// the returned <see cref="IOnlineResultSet{Dto}"/> becomes responsible
		/// for committing the transaction and closing the connection. No
		/// further methods may be called on the transaction after calling
		/// <c>ExecReadOnlineAsync</c>, and the underlying database connection
		/// and transaction will not be closed when the
		/// <see cref="IPostgresTransaction"/> is disposed. Instead, the
		/// transaction is automatically commited once the
		/// <see cref="IOnlineResultSet{Dto}"/> is disposed.
		/// </summary>
		/// <param name="command">The SQL command to execute.</param>
		/// <param name="dbConverter">A converter for the data record.</param>
		/// <returns>An iterator over the result set.</returns>
		Task<IOnlineResultSet<Dto>> ExecReadOnlineAsync<Dto>(
			PostgresCommand command,
			Func<IDataRecord,Dto> dbConverter
		);
		#endregion
		
	}
	
}
