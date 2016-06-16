using System;
using NpgsqlTypes;

namespace D2L.Services.Core.Postgres.TypeConverters {
	
	/// <summary>
	/// An interface for implementing a Postgres data type converter for objects
	/// of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type being converted.</typeparam>
	public interface IPostgresTypeConverter<T> {
		
		/// <summary>
		/// Convert the type into a simple C# type that Npgsql can write to the
		/// database. (See http://www.npgsql.org/doc/types.html )
		/// 
		/// If <typeparamref name="T"/> is a class or interface, and
		/// <paramref name="value"/> is <c>null</c>, you should return
		/// <see cref="DBNull">DBNull.Value</see>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>The value converted into a simple type.</returns>
		object ToDbValue( T value );
		
		/// <summary>
		/// Convert a value from the simple C# type read from the database by
		/// Npgsql into the desired type <typeparamref name="T"/>. (See
		/// http://www.npgsql.org/doc/types.html )
		/// 
		/// If <typeparamref name="T"/> is a class or interface, you should
		/// handle the case where <paramref name="dbValue"/> is
		/// <see cref="DBNull">DBNull.Value</see>.
		/// </summary>
		/// <param name="dbValue">The value read from the database.</param>
		/// <returns>The converted value.</returns>
		T FromDbValue( object dbValue );
		
		/// <summary>
		/// The PostgreSQL database type corresponding to the C# type being
		/// converted.
		/// </summary>
		NpgsqlDbType DatabaseType { get; }
		
	}
	
}
