using D2L.Services.Core.TestFramework;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using System;

namespace D2L.Services.Core.Postgres.Tests.Unit {
	
	[TestFixture, Unit]
	internal sealed class PostgresCommandTests {
		
		[Test]
		public void BuildNpgsqlCommandTest() {
			const string COMMAND_TEXT = "SELECT 1 AS one";
			const int TIMEOUT_SECONDS = 88;
			
			var cmd = new PostgresCommand( COMMAND_TEXT );
			cmd.AddParameter<int?>( "arg1", 8 );
			cmd.AddParameter<int?>( "arg2", null );
			cmd.AddParameter<string>( "arg3", "{}", NpgsqlDbType.Jsonb );
			cmd.Timeout = TIMEOUT_SECONDS;
			
			NpgsqlConnection connection = new NpgsqlConnection();
			NpgsqlCommand builtCommand = cmd.Build( connection );
			
			Assert.AreEqual( COMMAND_TEXT, builtCommand.CommandText );
			Assert.AreEqual( TIMEOUT_SECONDS, builtCommand.CommandTimeout );
			Assert.AreSame( connection, builtCommand.Connection );
			AssertHasParameter( builtCommand, "arg1", 8 );
			AssertHasParameter( builtCommand, "arg2", DBNull.Value );
			AssertHasParameter( builtCommand, "arg3", "{}", NpgsqlDbType.Jsonb );
		}
		
		[Test]
		public void DateTimeMappingTest() {
			var cmd = new PostgresCommand();
			
			DateTime localTime = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Local );
			DateTime utcTime = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
			
			cmd.AddParameter<DateTime>( "local1", localTime );
			cmd.AddParameter<DateTime?>( "local2", (DateTime?)localTime );
			cmd.AddParameter<DateTime>( "utc1", utcTime );
			cmd.AddParameter<DateTime?>( "utc2", (DateTime?)utcTime );
			cmd.AddParameter<DateTime?>( "nullTime", (DateTime?)null );
			
			NpgsqlConnection connection = new NpgsqlConnection();
			NpgsqlCommand builtCommand = cmd.Build( connection );
			
			AssertHasParameter( builtCommand, "local1", localTime, NpgsqlDbType.TimestampTZ );
			AssertHasParameter( builtCommand, "local2", localTime, NpgsqlDbType.TimestampTZ );
			AssertHasParameter( builtCommand, "utc1", utcTime, NpgsqlDbType.Timestamp );
			AssertHasParameter( builtCommand, "utc2", utcTime, NpgsqlDbType.Timestamp );
			AssertHasParameter( builtCommand, "nullTime", DBNull.Value );
		}
		
		[Test]
		[ExpectedException( typeof( ArgumentException ) )]
		public void AmbiguousDateTimeType_ExpectArgumentException() {
			var cmd = new PostgresCommand();
			cmd.AddParameter(
				"arg1", 
				new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified )
			);
		}
		
		private static void AssertHasParameter(
			NpgsqlCommand cmd,
			string name,
			object value,
			NpgsqlDbType? type = null
		) {
			NpgsqlParameter parameter;
			Assert.IsTrue( cmd.Parameters.TryGetValue( name, out parameter ) );
			Assert.AreEqual( value, parameter.NpgsqlValue );
			Assert.AreEqual( value, parameter.Value );
			if( type.HasValue ) {
				Assert.AreEqual( type.Value, parameter.NpgsqlDbType );
			}
		}
		
	}
	
}
