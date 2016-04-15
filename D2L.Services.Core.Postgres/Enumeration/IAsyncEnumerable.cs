using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres.Enumeration {
	
	/// <summary>
	/// An <see cref="IEnumerable{T}" /> that supports asynchronous retrieval
	/// of its values.
	/// 
	/// Note that iterating over an <c>IAsyncEnumerable&lt;T&gt;</c> using a
	/// for each loop results in the values being retrieved synchronously. To
	/// get the values asynchronously, you must use the <c>ForEachAsync</c>
	/// methods or get the asynchronous version of the enumerator with
	/// <c>GetAsyncEnumerator</c>.
	/// </summary>
	/// <typeparam name="T">The type of objects to enumerate.</typeparam>
	/// <seealso cref="IAsyncEnumerator{T}"/>
	public interface IAsyncEnumerable<T> : IEnumerable<T> {
		
		/// <summary>
		/// Returns an enumerator that can iterate through the collection
		/// asynchronously.
		/// </summary>
		/// <returns>
		/// An <see cref="IAsyncEnumerator{T}">IAsyncEnumerator</see> that can
		/// be used to iterate through the collection asynchronously.
		/// </returns>
		IAsyncEnumerator<T> GetAsyncEnumerator();
		
		/// <summary>
		/// Fetch each item from the list asynchronously, then invoke the
		/// provided function on each value.
		/// </summary>
		/// <param name="function">The function to invoke on each value.</param>
		/// <returns></returns>
		Task ForEachAsync( Action<T> function );
		
		/// <summary>
		/// Fetch each item from the list asynchronously, then invoke the
		/// provided asynchronous function on each value.
		/// </summary>
		/// <param name="asyncFunction">
		/// The asynchronous function to invoke on each value.
		/// </param>
		/// <returns></returns>
		Task ForEachAsync( Func<T,Task> asyncFunction );
	}
	
}
