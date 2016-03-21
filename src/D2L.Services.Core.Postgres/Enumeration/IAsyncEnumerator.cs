using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres.Enumeration {
	
	/// <summary> Supports asynchronous iteration over a collection.</summary>
	/// <typeparam name="T">The type of objects to enumerate.</typeparam>
	/// <seealso cref="IAsyncEnumerable{T}"/>
	public interface IAsyncEnumerator<T> : IEnumerator, IEnumerator<T> {
		
		/// <summary>
		/// Asynchronously advances the enumerator to the next element of the
		/// collection.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the enumerator was successfully advanced to the next
		/// element; <c>false</c> if the enumerator has passed the end of the
		/// collection.
		/// </returns>
		Task<bool> MoveNextAsync();
		
	}
	
}
