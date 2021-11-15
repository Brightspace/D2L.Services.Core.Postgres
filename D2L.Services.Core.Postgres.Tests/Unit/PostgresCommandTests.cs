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
			
			var cmd = new PostgresCommand( COMMAND_TEXT, prepared: false );
			cmd.AddParameter<int?>( "arg1", 8 );
			cmd.AddParameter<int?>( "arg2", null );
			cmd.AddParameter<string>( "arg3", "{}", NpgsqlDbType.Jsonb );
			cmd.Timeout = TIMEOUT_SECONDS;
			
			NpgsqlConnection connection = new NpgsqlConnection();
			NpgsqlCommand builtCommand = cmd.BuildAsync( connection ).SafeWait();
			
			Assert.AreEqual( COMMAND_TEXT, builtCommand.CommandText );
			Assert.AreEqual( TIMEOUT_SECONDS, builtCommand.CommandTimeout );
			Assert.AreSame( connection, builtCommand.Connection );
			AssertHasParameter( builtCommand, "arg1", 8, NpgsqlDbType.Integer );
			AssertHasParameter( builtCommand, "arg2", DBNull.Value, NpgsqlDbType.Integer );
			AssertHasParameter( builtCommand, "arg3", "{}", NpgsqlDbType.Jsonb );
		}
		
		private static void AssertHasParameter(
			NpgsqlCommand cmd,
			string name,
			object value,
			NpgsqlDbType type
		) {
			NpgsqlParameter parameter;
			Assert.IsTrue( cmd.Parameters.TryGetValue( name, out parameter ) );
			Assert.AreEqual( value, parameter.NpgsqlValue );
			Assert.AreEqual( value, parameter.Value );
			Assert.AreEqual( type, parameter.NpgsqlDbType );
		}
		
	}
	
}
