using NpgsqlTypes;
using System;

namespace D2L.Services.Core.Postgres.TypeConverters.Default {
	
	internal sealed class ArrayTypeConverter<T> : IPostgresTypeConverter<T[]> {
		
		internal readonly IPostgresTypeConverter<T> m_innerConverter;
		
		public ArrayTypeConverter(
			IPostgresTypeConverter<T> innerConverter
		) {
			m_innerConverter = innerConverter;
		}
		
		object IPostgresTypeConverter<T[]>.ToDbValue( T[] value ) {
			if( value == null ) {
				return DBNull.Value;
			}
			
			object[] dbValues = new object[ value.LongLength ];
			for( long i = 0; i < value.LongLength; i++ ) {
				dbValues[i] = m_innerConverter.ToDbValue( value[i] );
			}
			
			return dbValues;
		}
		
		T[] IPostgresTypeConverter<T[]>.FromDbValue( object dbValue ) {
			if( dbValue is DBNull || dbValue == null ) {
				return null;
			}
			
			Array dbValues = (Array)dbValue;
			T[] values = new T[ dbValues.LongLength ];
			for( long i = 0; i < dbValues.LongLength; i++ ) {
				values[i] = m_innerConverter.FromDbValue( dbValues.GetValue( i ) );
			}
			return values;
		}
		
		NpgsqlDbType IPostgresTypeConverter<T[]>.DatabaseType {
			get { return NpgsqlDbType.Array | m_innerConverter.DatabaseType; }
		}
		
	}
	
}
