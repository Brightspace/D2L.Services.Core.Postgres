using NpgsqlTypes;
using System;

namespace D2L.Services.Core.Postgres.TypeConverters.Default {
	
	internal sealed class NullableTypeConverter<T> : IPostgresTypeConverter<T?> where T : struct {
		
		internal readonly IPostgresTypeConverter<T> m_innerConverter;
		
		public NullableTypeConverter(
			IPostgresTypeConverter<T> innerConverter
		) {
			m_innerConverter = (IPostgresTypeConverter<T>)innerConverter;
		}
		
		object IPostgresTypeConverter<T?>.ToDbValue( T? value ) {
			if( value.HasValue ) {
				return m_innerConverter.ToDbValue( value.Value );
			}
			return DBNull.Value;
		}
		
		T? IPostgresTypeConverter<T?>.FromDbValue( object dbValue ) {
			if( dbValue is DBNull || dbValue == null ) {
				return (T?)null;
			}
			return m_innerConverter.FromDbValue( dbValue );
		}
		
		NpgsqlDbType IPostgresTypeConverter<T?>.DatabaseType {
			get { return m_innerConverter.DatabaseType; }
		}
		
	}
	
}
