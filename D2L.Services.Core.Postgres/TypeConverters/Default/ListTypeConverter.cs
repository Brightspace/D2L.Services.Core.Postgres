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
			return ToDbValueInternal( value );
		}
		
		object IPostgresTypeConverter<List<T>>.ToDbValue( List<T> value ) {
			return ToDbValueInternal( value );
		}

		private object ToDbValueInternal( IList<T> value ) {
			if( value == null ) {
				return DBNull.Value;
			}

			object[] dbValues = new object[value.Count];
			for( int i = 0; i < value.Count; i++ ) {
				dbValues[i] = m_innerConverter.ToDbValue( value[i] );
			}

			return dbValues;
		}
		
		List<T> IPostgresTypeConverter<List<T>>.FromDbValue( object dbValue ) {
			return FromDbValueInternal( dbValue );
		}
		
		IList<T> IPostgresTypeConverter<IList<T>>.FromDbValue( object dbValue ) {
			return FromDbValueInternal( dbValue );
		}

		private List<T> FromDbValueInternal( object dbValue ) {
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

		NpgsqlDbType IPostgresTypeConverter<List<T>>.DatabaseType {
			get { return NpgsqlDbType.Array | m_innerConverter.DatabaseType; }
		}
		
		NpgsqlDbType IPostgresTypeConverter<IList<T>>.DatabaseType {
			get { return NpgsqlDbType.Array | m_innerConverter.DatabaseType; }
		}
		
	}
	
}
