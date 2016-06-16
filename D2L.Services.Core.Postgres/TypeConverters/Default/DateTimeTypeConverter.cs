using NpgsqlTypes;
using System;

namespace D2L.Services.Core.Postgres.TypeConverters.Default {
	
	internal sealed class DateTimeTypeConverter : IPostgresTypeConverter<DateTime> {
		
		object IPostgresTypeConverter<DateTime>.ToDbValue( DateTime value ) {
			return value;
		}
		
		DateTime IPostgresTypeConverter<DateTime>.FromDbValue( object dbValue ) {
			DateTime timestamp = (DateTime)dbValue;
			/* When reading into a DateTime object, Npgsql sets the kind to
			 * Local for timestampz (timestamp with timezone) types, and
			 * Unspecified for timestamp (timestamp without timezone) types.
			 * While this is technically correct, we will always assume that
			 * timestamps without time zones are in UTC time.
			 */
			if( timestamp.Kind == DateTimeKind.Unspecified ) {
				return DateTime.SpecifyKind(
					timestamp,
					DateTimeKind.Utc
				);
			}
			return timestamp;
		}
		
		NpgsqlDbType IPostgresTypeConverter<DateTime>.DatabaseType {
			/* Always return Timestamp (without time zone), even if the DateTime
			 * is in local time. This works fine because of the way that Npgsql
			 * handles DateTime. So long as the database column type matches the
			 * DateTime kind, the correct time will be used.
			 * 
			 * See DateTimeHandlingTests.cs for proof of this.
			 */
			get { return NpgsqlDbType.Timestamp; }
		}
		
	}
	
}
