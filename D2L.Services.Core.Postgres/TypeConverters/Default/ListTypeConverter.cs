using NpgsqlTypes;
using System;
using System.Collections.Generic;

namespace D2L.Services.Core.Postgres.TypeConverters.Default {
	
	internal sealed class ListTypeConverter<T> : IPostgresTypeConverter<List<T>>, IPostgresTypeConverter<IList<T>> {
		
		internal readonly IPostgresTypeConverter<T> m_innerConverter;
		
		public ListTypeConverter(
			IPostgresTypeConverter<T> innerConverter
		) {
			m_innerConverter = innerConverter;
		}
		
		object IPostgresTypeConverter<IList<T>>.ToDbValue( IList<T> value ) {
			if( value == null ) {
				return DBNull.Value;
			}
			
			object[] dbValues = new object[ value.Count ];
			for( int i = 0; i < value.Count; i++ ) {
				dbValues[i] = m_innerConverter.ToDbValue( value[i] );
			}
			
			return dbValues;
		}
		
		object IPostgresTypeConverter<List<T>>.ToDbValue( List<T> value ) {
			IPostgresTypeConverter<IList<T>> @this = this;
			return @this.ToDbValue( value );
		}
		
		List<T> IPostgresTypeConverter<List<T>>.FromDbValue( object dbValue ) {
			if( dbValue is DBNull || dbValue == null ) {
				return null;
			}
			
			Array dbValues = (Array)dbValue;
			List<T> values = new List<T>( dbValues.Length );
			for( int i = 0; i < dbValues.LongLength; i++ ) {
				values.Add( m_innerConverter.FromDbValue( dbValues.GetValue( i ) ) );
			}
			return values;
		}
		
		IList<T> IPostgresTypeConverter<IList<T>>.FromDbValue( object dbValue ) {
			IPostgresTypeConverter<List<T>> @this = this;
			return @this.FromDbValue( dbValue );
		}
		
		NpgsqlDbType IPostgresTypeConverter<List<T>>.DatabaseType {
			get { return NpgsqlDbType.Array | m_innerConverter.DatabaseType; }
		}
		
		NpgsqlDbType IPostgresTypeConverter<IList<T>>.DatabaseType {
			get { return NpgsqlDbType.Array | m_innerConverter.DatabaseType; }
		}
		
	}
	
}
