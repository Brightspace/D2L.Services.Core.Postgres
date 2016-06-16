using System;

namespace D2L.Services.Core.Postgres {
	
	internal static class DbTypeConverter {
		
		internal static object ToDbValue<T>( T value ) {
			if( (object)value == null ) {
				return DBNull.Value;
			} else {
				return value;
			}
		}
		
		internal static T FromDbValue<T>( object dbValue ) {
			if( dbValue is DBNull ) {
				return (T)(object)null;
			}
			
			if( dbValue is DateTime ) {
				
				/* When reading into a DateTime object, Npgsql sets the kind to
				 * Local for timestampz (timestamp with timezone) types, and
				 * Unspecified for timestamp (timestamp without timezone) types.
				 * While this is technically correct, we will always assume that
				 * timestamps without time zones are in UTC time. */
				
				if( ((DateTime)dbValue).Kind == DateTimeKind.Unspecified ) {
					dbValue = DateTime.SpecifyKind(
						(DateTime)dbValue,
						DateTimeKind.Utc
					);
				}
			}
			
			return (T)dbValue;
		}
		
	}
	
}
