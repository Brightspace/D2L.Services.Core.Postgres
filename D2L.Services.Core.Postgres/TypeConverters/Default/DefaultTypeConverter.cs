using NpgsqlTypes;
using System;
using System.Reflection;

namespace D2L.Services.Core.Postgres.TypeConverters.Default {
	
	internal sealed class DefaultTypeConverter<T> : IPostgresTypeConverter<T> {
		
		private readonly NpgsqlDbType m_dbType;
		
		public DefaultTypeConverter() {
			// Npgsql's built-in type converter registry is internal, so we need
			// to use Reflection in order to access it. This only needs to be
			// done once per type.
			Assembly npgsqlAssembly = Assembly.GetAssembly( typeof( NpgsqlDbType ) );
			Type typeMapperType = npgsqlAssembly.GetType( "Npgsql.TypeMapping.GlobalTypeMapper" );
			object typeMapper = typeMapperType.GetProperty(
				name: "Instance",
				bindingAttr: BindingFlags.Static | BindingFlags.Public
			).GetMethod.Invoke( null, new object[] {} );

			m_dbType = (NpgsqlDbType)typeMapperType.GetMethod(
				name: "ToNpgsqlDbType",
				bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
				binder: null,
				types: new Type[]{ typeof( Type ) },
				modifiers: null
			).Invoke( typeMapper, new object[]{ typeof( T ) } );
		}
		
		object IPostgresTypeConverter<T>.ToDbValue( T value ) {
			if( (object)value == null ) {
				return DBNull.Value;
			}
			return value;
		}
		
		T IPostgresTypeConverter<T>.FromDbValue( object dbValue ) {
			return dbValue is DBNull ? (T)(object)null : (T)dbValue;
		}
		
		NpgsqlDbType IPostgresTypeConverter<T>.DatabaseType {
			get { return m_dbType; }
		}
		
	}
	
}
