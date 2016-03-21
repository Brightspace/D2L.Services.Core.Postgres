using System;

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
		/// Rollback the transaction and close the connection. Calling any
		/// methods on the transaction after this point will result in an
		/// <see cref="ObjectDisposedException"/> being thrown.
		/// </summary>
		void Rollback();
		
		//TODO[v1.2.0] Add support for savepoints.
		
	}
	
}
