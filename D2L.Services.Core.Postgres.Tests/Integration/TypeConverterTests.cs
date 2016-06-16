using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using D2L.Services.Core.Postgres.Tests.Types;
using D2L.Services.Core.TestFramework;
using NUnit.Framework;
using System;

namespace D2L.Services.Core.Postgres.Tests.Integration {
	
	[TestFixture, Integration]
	internal sealed class TypeConverterTests : IntegrationTestFixtureBase {
		
		[SetUp, TestFixtureTearDown]
		public void Cleanup() {
			PostgresCommand cmd = new PostgresCommand( @"
				DELETE FROM basic_table;
				DELETE FROM array_table;
				DELETE FROM guid_table;"
			);
			m_database.ExecNonQueryAsync( cmd ).SafeWait();
		}
		
		[Test]
		public async Task BasicTest() {
			TestId id = new TestId( Guid.NewGuid() );
			
			var cmd = new PostgresCommand( @"
				INSERT INTO basic_table( id, comment )
				VALUES( :id, 'TEST' )"
			);
			cmd.AddParameter<TestId>( "id", id );
			
			await m_database.ExecNonQueryAsync( cmd ).SafeAsync();
			
			cmd = new PostgresCommand( "SELECT id FROM basic_table" );
			TestId readId;
			
			readId = await m_database.ExecReadScalarAsync<TestId>( cmd ).SafeAsync();
			Assert.AreEqual( id, readId );
			
			readId = await m_database.ExecReadFirstAsync<TestId>(
				command: cmd,
				dbConverter: record => record.Get<TestId>( "id" )
			).SafeAsync();
			Assert.AreEqual( id, readId );
		}
		
		[Test]
		public async Task NullableType_HasValue() {
			await RunNullableTypeTestAsync(
				value: new TestId( Guid.NewGuid() )
			).SafeAsync();
		}
		
		[Test]
		public async Task NullableType_NullValue() {
			await RunNullableTypeTestAsync( value: null ).SafeAsync();
		}
		
		[Test]
		public async Task ArrayConversion_SimpleCase() {
			await RunArrayTableTestAsync(
				new TestId[]{ new TestId( Guid.NewGuid() ), new TestId( Guid.NewGuid() ) },
				new string[]{ "1", "2", "3" }
			).SafeAsync();
		}
		
		[Test]
		public async Task ArrayConversion_ArrayIsNull() {
			await RunArrayTableTestAsync( null, null ).SafeAsync();
		}
		
		[Test]
		public async Task ArrayConversion_EmptyArray() {
			await RunArrayTableTestAsync(
				new TestId[]{},
				new string[]{}
			).SafeAsync();
		}
		
		[Test]
		public async Task ArrayConversion_StringArrayWithNullValues() {
			// Npgsql doesn't support reading arrays of nullables that contain
			// NULL values, but it does support arrays of string with NULLs
			await RunArrayTableTestAsync(
				null,
				new string[]{ null, "", "TEST" }
			).SafeAsync();
		}
		
		[Test]
		public async Task ArrayConversion_WriteArrayWithNullValues() {
			// Even though Npgsql doesn't read arrays of nullables correctly,
			// this library still supports adding arrays of nullables as
			// parameters.
			PostgresCommand cmd;
			
			Guid?[] guids = new Guid?[]{
				Guid.NewGuid(),
				(Guid?)null,
				Guid.Empty,
				Guid.NewGuid()
			};
			
			cmd = new PostgresCommand( @"
				INSERT INTO guid_table( guid )
				SELECT guid FROM UNNEST( :guids ) AS guid"
			);
			cmd.AddParameter<Guid?[]>( "guids", guids );
			Assert.AreEqual( guids.Length, await m_database.ExecNonQueryAsync( cmd ).SafeAsync() );
			
			cmd = new PostgresCommand(
				"SELECT guid FROM guid_table"
			);
			IReadOnlyList<Guid?> fetchedGuids =
				await m_database.ExecReadColumnOfflineAsync<Guid?>( cmd ).SafeAsync();
			
			CollectionAssert.AreEquivalent( guids, fetchedGuids );
		}

		
		private async Task RunNullableTypeTestAsync(
			TestId? value
		) {
			var cmd = new PostgresCommand( @"
				INSERT INTO guid_table( guid )
				VALUES( :test_id )
				RETURNING id"
			);
			cmd.AddParameter<TestId?>( "test_id", value );
			
			int id = await m_database.ExecReadScalarAsync<int>( cmd ).SafeAsync();
			
			TestId? readValue;
			cmd = new PostgresCommand( @"
				SELECT guid FROM guid_table
					WHERE id = :id"
			);
			cmd.AddParameter<int>( "id", id );
			
			readValue = await m_database.ExecReadScalarAsync<TestId?>( cmd ).SafeAsync();
			Assert.AreEqual( value, readValue );
			
			readValue = await m_database.ExecReadFirstAsync<TestId?>(
				command: cmd,
				dbConverter: record => record.Get<TestId?>( "guid" )
			).SafeAsync();
			Assert.AreEqual( value, readValue );
		}
		
		private async Task RunArrayTableTestAsync(
			TestId[] testIdArray,
			string[] stringArray
		) {
			PostgresCommand cmd;
			
			cmd = new PostgresCommand( @"
				INSERT INTO array_table( guid_array, string_array )
				VALUES( :guid_array, :string_array )
				RETURNING id"
			);
			cmd.AddParameter( "guid_array", testIdArray );
			cmd.AddParameter( "string_array", stringArray );
			
			int id = await m_database.ExecReadScalarAsync<int>( cmd ).SafeAsync();
			
			cmd = new PostgresCommand( @"
				SELECT * FROM array_table
				WHERE id = :id"
			);
			cmd.AddParameter( "id", id );
			
			ArrayTableDto record = await m_database.ExecReadFirstAsync(
				cmd,
				ArrayTableDto.DbConverter
			).SafeAsync();
			
			CollectionAssert.AreEqual( testIdArray, record.TestIdArray );
			CollectionAssert.AreEqual( stringArray, record.StringArray );
			
		}
		
		private sealed class ArrayTableDto {
			
			private readonly TestId[] m_testIdArray;
			private readonly string[] m_stringArray;
			
			private ArrayTableDto(
				TestId[] testIdArray,
				string[] stringArray
			) {
				m_testIdArray = testIdArray;
				m_stringArray = stringArray;
			}
			
			internal TestId[] TestIdArray { get { return m_testIdArray; } }
			internal string[] StringArray { get { return m_stringArray; } }
			
			internal static ArrayTableDto DbConverter( IDataRecord record ) {
				return new ArrayTableDto(
					record.Get<TestId[]>( "guid_array" ),
					record.Get<string[]>( "string_array" )
				);
			}
			
		}
		
	}
	
}
