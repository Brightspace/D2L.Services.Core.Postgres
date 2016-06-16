using D2L.Services.Core.Postgres.TypeConverters.Default;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace D2L.Services.Core.Postgres.TypeConverters {
	
	internal static class DbTypeConverter<T> {
		
		private static readonly Lazy<IPostgresTypeConverter<T>> s_converter =
			new Lazy<IPostgresTypeConverter<T>>( Init, isThreadSafe: true );
		
		internal static object ToDbValue( T value ) {
			return s_converter.Value.ToDbValue( value );
		}
		
		internal static T FromDbValue( object dbValue ) {
			return s_converter.Value.FromDbValue( dbValue );
		}
		
		internal static NpgsqlDbType DatabaseType {
			get { return s_converter.Value.DatabaseType; }
		}
		
		
		internal static IPostgresTypeConverter<T> Converter {
			get{ return s_converter.Value; }
		}
		
		
		private static IPostgresTypeConverter<T> Init() {
			
			if( typeof( T ) == typeof( DateTime ) ) {
				return (IPostgresTypeConverter<T>)(object)new DateTimeTypeConverter();
			}
			
			if( typeof( T ).IsArray ) {
				Type elementType = typeof( T ).GetElementType();
				object innerConverter = GetConverterForType( elementType );
				
				return CreateWrappingConverter(
					genericTypeConverter: typeof( ArrayTypeConverter<> ),
					innerType: elementType,
					innerConverter: innerConverter
				);
			}
			
			if(
				IsGenericTypeOf( typeof( T ), typeof( List<> ) ) ||
				IsGenericTypeOf( typeof( T ), typeof( IList<> ) )
			) {
				Type elementType = typeof( T ).GetGenericArguments()[0];
				object innerConverter = GetConverterForType( elementType );
				
				return CreateWrappingConverter(
					genericTypeConverter: typeof( ListTypeConverter<> ),
					innerType: elementType,
					innerConverter: innerConverter
				);
			}
			
			if( IsGenericTypeOf( typeof( T ), typeof( IEnumerable<> ) ) ) {
				Type elementType = typeof( T ).GetGenericArguments()[0];
				return CreateWrappingConverter(
					genericTypeConverter: typeof( EnumerableTypeConverter<> ),
					innerType: elementType,
					innerConverter: GetConverterForType( elementType )
				);
			}
			
			if( IsGenericTypeOf( typeof( T ), typeof( Nullable<> ) ) ) {
				Type innerType = Nullable.GetUnderlyingType( typeof( T ) );
				object innerConverter = GetConverterForType( innerType );
				
				return CreateWrappingConverter(
					genericTypeConverter: typeof( NullableTypeConverter<> ),
					innerType: innerType,
					innerConverter: innerConverter
				);
			}
			
			object[] ptcAttributes = typeof( T ).GetCustomAttributes(
				attributeType: typeof( PostgresTypeConverterAttribute ),
				inherit: false
			);
			
			if( ptcAttributes.Length > 0 ) {
				var pgTypeConverterAttr = (PostgresTypeConverterAttribute)ptcAttributes[0];
				Type converterType = pgTypeConverterAttr.ConverterType;
				
				if( !typeof( IPostgresTypeConverter<T> ).IsAssignableFrom( converterType ) ) {
					throw new InvalidCastException(
						"The type provided to a [PostgresTypeConverter] attribute " +
						"is not a type converter for the expected type."
					);
				}
				
				return (IPostgresTypeConverter<T>)Activator.CreateInstance( converterType );
			}
			
			return new DefaultTypeConverter<T>();
		}
		
		private static object GetConverterForType( Type type ) {
			return typeof( DbTypeConverter<> ).MakeGenericType( type ).GetProperty(
				name: "Converter",
				bindingAttr: BindingFlags.Static | BindingFlags.NonPublic
			).GetMethod.Invoke( null, null );
		}
		
		private static IPostgresTypeConverter<T> CreateWrappingConverter(
			Type genericTypeConverter,
			Type innerType,
			object innerConverter
		) {
			return (IPostgresTypeConverter<T>)Activator.CreateInstance(
				type: genericTypeConverter.MakeGenericType( innerType ),
				args: new[]{ innerConverter }
			);
		}
		
		private static bool IsGenericTypeOf(
			Type type,
			Type genericTypeDefinition
		) {
			return(
			    type.IsGenericType &&
			    type.GetGenericTypeDefinition() == genericTypeDefinition
			);
		}
		
	}
	
}
