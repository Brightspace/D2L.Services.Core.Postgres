using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace D2L.Services.Core.Postgres.TypeConverters.Default {
	
	internal sealed class EnumerableTypeConverter<T> : IPostgresTypeConverter<IEnumerable<T>> {
		
		internal readonly IPostgresTypeConverter<IList<T>> m_listConverter;
		
		public EnumerableTypeConverter(
			IPostgresTypeConverter<T> innerConverter
		) {
			m_listConverter = new ListTypeConverter<T>( innerConverter );
		}
		
		object IPostgresTypeConverter<IEnumerable<T>>.ToDbValue( IEnumerable<T> value ) {
			if( value == null ) {
				return DBNull.Value;
			}
			return m_listConverter.ToDbValue( value as IList<T> ?? value.ToList() );
		}
		
		IEnumerable<T> IPostgresTypeConverter<IEnumerable<T>>.FromDbValue( object dbValue ) {
			return m_listConverter.FromDbValue( dbValue );
		}
		
		NpgsqlDbType IPostgresTypeConverter<IEnumerable<T>>.DatabaseType {
			get { return m_listConverter.DatabaseType; }
		}
		
	}
	
}
