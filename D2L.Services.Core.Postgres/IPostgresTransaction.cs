using System;
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
		void Commit();
		
		/// <summary>
		/// Commit the transaction and close the connection. Calling any methods
		/// on the transaction after this point will result in an
		/// <see cref="ObjectDisposedException"/> being thrown.
		/// 
		/// Currently, this method is actually executed synchronously. It will
		/// be replaced with an actual asynchronous implementation once version
		/// 3.1 of Npgsql is released (probably late July or early August 2016).
		/// </summary>
		Task CommitAsync();
		
		/// <summary>
		/// Rollback the transaction and close the connection. Calling any
		/// methods on the transaction after this point will result in an
		/// <see cref="ObjectDisposedException"/> being thrown.
		/// </summary>
		void Rollback();
		
		/// <summary>
		/// Rollback the transaction and close the connection. Calling any
		/// methods on the transaction after this point will result in an
		/// <see cref="ObjectDisposedException"/> being thrown.
		/// 
		/// Currently, this method is actually executed synchronously. It will
		/// be replaced with an actual asynchronous implementation once version
		/// 3.1 of Npgsql is released (probably late July or early August 2016).
		/// </summary>
		Task RollbackAsync();
		
		//TODO[v2.1.0] Add support for savepoints.
		
	}
	
}
