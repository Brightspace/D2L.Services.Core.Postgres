
using System;

namespace D2L.Services.Core.Postgres {
	
	/// <summary>
	/// An exception that is thrown when a database query for a single record or
	/// scalar value returns no results.
	/// </summary>
	public class DataNotFoundException : Exception {
		
		private const string DEFAULT_MESSAGE =
			"The SQL query returned no results.";
		
		/// <summary>
		/// Initialize a new instance of the
		/// <see cref="DataNotFoundException"/> class.
		/// </summary>
		public DataNotFoundException() : base( DEFAULT_MESSAGE ) {}
		
		/// <summary>
		/// Initialize a new instance of the
		/// <see cref="DataNotFoundException"/> class.
		/// </summary>
		/// <param name="innerException">The inner exception</param>
		public DataNotFoundException(
			Exception innerException
		) : base( DEFAULT_MESSAGE, innerException ) {}
		
		/// <summary>
		/// Initialize a new instance of the
		/// <see cref="DataNotFoundException"/> class.
		/// </summary>
		/// <param name="message">The exception message</param>
		public DataNotFoundException(
			string message
		) : base( message ) {}
		
		/// <summary>
		/// Initialize a new instance of the
		/// <see cref="DataNotFoundException"/> class.
		/// </summary>
		/// <param name="message">The exception message</param>
		/// <param name="innerException">The inner exception</param>
		public DataNotFoundException(
			string message,
			Exception innerException
		) : base( message, innerException ) {}
		
	}
	
}