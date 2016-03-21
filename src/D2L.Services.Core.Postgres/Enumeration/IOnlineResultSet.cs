using System;

namespace D2L.Services.Core.Postgres.Enumeration {
	
	/// <summary>
	/// An <see cref="IAsyncEnumerable{Dto}"/> that allows the synchronous or
	/// asynchronous retrieval of results from a database query one record at a
	/// time. The connection to the database is held open until either the
	/// result set is fully enumerated or <c>Dispose()</c> is called explicitly.
	/// </summary>
	/// <typeparam name="Dto">A DTO representing the data record.</typeparam>
	public interface IOnlineResultSet<Dto> : IAsyncEnumerable<Dto>, IDisposable {}
	
}
