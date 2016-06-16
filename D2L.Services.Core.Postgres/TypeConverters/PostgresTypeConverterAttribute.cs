using System;

namespace D2L.Services.Core.Postgres.TypeConverters {
	
	/// <summary>
	/// Specifies the <see cref="IPostgresTypeConverter{T}" /> used to convert
	/// the type to and from a database type.
	/// </summary>
	/// <seealso cref="IPostgresTypeConverter{T}"/>
	[AttributeUsage(
		validOn: AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Class,
		AllowMultiple = false,
		Inherited = false
	)]
	public sealed class PostgresTypeConverterAttribute : Attribute {
		
		private readonly Type m_converterType;
		
		/// <summary>
		/// Specifies the <see cref="IPostgresTypeConverter{T}" /> used to
		/// convert the type to and from a database type.
		/// </summary>
		/// <param name="converterType">
		/// The type of the converter. The type must inherit from the
		/// <see cref="IPostgresTypeConverter{T}"/> interface where <c>T</c> is
		/// the type being converted.
		/// </param>
		public PostgresTypeConverterAttribute( Type converterType ) {
			m_converterType = converterType;
		}
		
		internal Type ConverterType { get { return m_converterType; } }
		
	}
}
