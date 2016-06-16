using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using D2L.Services.Core.TestFramework;
using Npgsql;
using NUnit.Framework;

namespace D2L.Services.Core.Postgres.Tests.Integration {
	
	// Tests common to both IPostgresDatabase and IPostgresTransaction
	
	[TestFixture, Integration, RequiresDatabase]
	internal sealed class PostgresExecutorTests : IntegrationTestFixtureBase {
		
		[SetUp]
		public void SetUp() {
			CleanDatabaseAsync().SafeWait();
		}
		
		[TestFixtureTearDown]
		public void TestFixtureTearDown() {
			CleanDatabaseAsync().SafeWait();
		}
		
		// Test both PostgresDatabase and PostgresTransaction in the same test
		// by calling this helper function with useTransaction as true and false
		private async Task RunAsync(
			Func<IPostgresExecutor,Task> function,
			bool useTransaction
		) {
			if( useTransaction ) {
				using(
					IPostgresTransaction transaction =
					await m_database.NewTransactionAsync().SafeAsync()
				) {
					await function( transaction ).SafeAsync();
					await transaction.CommitAsync().SafeAsync();
				}
			} else {
				await function( m_database ).SafeAsync();
			}
		}
		
		[TestCase( false ), TestCase( true )]
		public async Task ExecNonQueryTest( bool useTransaction ) {
			PostgresCommand cmd;
			int rowsAffected = 0;
			
			cmd = new PostgresCommand( @"
				INSERT INTO basic_table( id )
					VALUES( :parameter )"
			);
			cmd.AddParameter<Guid>( "parameter", Guid.NewGuid() );
			
			await RunAsync(
				async executor => rowsAffected = await executor.ExecNonQueryAsync( cmd ).SafeAsync(),
				useTransaction
			).SafeAsync();
			
			Assert.AreEqual( 1, rowsAffected );
			
			
			cmd = new PostgresCommand(
				"DELETE FROM basic_table"
			);
			
			await RunAsync(
				async executor => rowsAffected = await executor.ExecNonQueryAsync( cmd ).SafeAsync(),
				useTransaction
			).SafeAsync();
			
			Assert.AreEqual( 1, rowsAffected );
			
			await RunAsync(
				async executor => rowsAffected = await executor.ExecNonQueryAsync( cmd ).SafeAsync(),
				useTransaction
			).SafeAsync();
			
			Assert.AreEqual( 0, rowsAffected );
		}
		
		[TestCase( false ), TestCase( true )]
		public async Task ExecReadScalarTest( bool useTransaction ) {
			PostgresCommand cmd;
			Guid testId1 = Guid.NewGuid();
			Guid testId2 = Guid.NewGuid();
			const string TEST_COMMENT = "ExecReadScalarTest";
			
			cmd = new PostgresCommand( @"
				INSERT INTO basic_table( id, comment ) VALUES( :id1, :comment1 );
				INSERT INTO basic_table( id ) VALUES( :id2 );"
			);
			cmd.AddParameter<Guid>( "id1", testId1 );
			cmd.AddParameter<string>( "comment1", TEST_COMMENT );
			cmd.AddParameter<Guid>( "id2", testId2 );
			Assert.AreEqual( 2, await m_database.ExecNonQueryAsync( cmd ).SafeAsync() );
			
			string fetchedComment = "";
			PostgresCommand fetchCommand = new PostgresCommand( @"
				SELECT comment FROM basic_table
					WHERE id = :id"
			);
			
			cmd = new PostgresCommand( fetchCommand );
			cmd.AddParameter<Guid>( "id", testId1 );
			await RunAsync(
				async executor => fetchedComment = await executor.ExecReadScalarAsync<string>( cmd ).SafeAsync(),
				useTransaction
			).SafeAsync();
			Assert.AreEqual( TEST_COMMENT, fetchedComment );
			
			cmd = new PostgresCommand( fetchCommand );
			cmd.AddParameter<Guid>( "id", testId2 );
			await RunAsync(
				async executor => fetchedComment = await executor.ExecReadScalarAsync<string>( cmd ).SafeAsync(),
				useTransaction
			).SafeAsync();
			Assert.IsNull( fetchedComment );
			
			cmd = new PostgresCommand( fetchCommand );
			cmd.AddParameter<Guid>( "id", Guid.NewGuid() );
			Assert.Throws<DataNotFoundException>( async() =>
				await RunAsync(
					async executor => fetchedComment = await executor.ExecReadScalarAsync<string>( cmd ).SafeAsync(),
					useTransaction
				).SafeAsync()
			);
			
		}
		
		[TestCase( false ), TestCase( true )]
		public async Task ExecReadScalarOrDefaultTest( bool useTransaction ) {
			PostgresCommand cmd;
			Guid testId1 = Guid.NewGuid();
			Guid testId2 = Guid.NewGuid();
			const string TEST_COMMENT = "ExecReadScalarOrDefaultTest";
			const string DEFAULT_STRING = "NOT FOUND";
			
			cmd = new PostgresCommand( @"
				INSERT INTO basic_table( id, comment ) VALUES( :id1, :comment1 );
				INSERT INTO basic_table( id ) VALUES( :id2 );"
			);
			cmd.AddParameter<Guid>( "id1", testId1 );
			cmd.AddParameter<string>( "comment1", TEST_COMMENT );
			cmd.AddParameter<Guid>( "id2", testId2 );
			Assert.AreEqual( 2, await m_database.ExecNonQueryAsync( cmd ).SafeAsync() );
			
			string fetchedComment = "";
			PostgresCommand fetchCommand = new PostgresCommand( @"
				SELECT comment FROM basic_table
					WHERE id = :id"
			);
			
			cmd = new PostgresCommand( fetchCommand );
			cmd.AddParameter<Guid>( "id", testId1 );
			await RunAsync(
				async executor => fetchedComment = await executor.ExecReadScalarOrDefaultAsync<string>( cmd, DEFAULT_STRING ).SafeAsync(),
				useTransaction
			).SafeAsync();
			Assert.AreEqual( TEST_COMMENT, fetchedComment );
			
			cmd = new PostgresCommand( fetchCommand );
			cmd.AddParameter<Guid>( "id", testId2 );
			await RunAsync(
				async executor => fetchedComment = await executor.ExecReadScalarOrDefaultAsync<string>( cmd, DEFAULT_STRING ).SafeAsync(),
				useTransaction
			).SafeAsync();
			Assert.IsNull( fetchedComment );
			
			cmd = new PostgresCommand( fetchCommand );
			cmd.AddParameter<Guid>( "id", Guid.NewGuid() );
			await RunAsync(
				async executor => fetchedComment = await executor.ExecReadScalarOrDefaultAsync<string>( cmd, DEFAULT_STRING ).SafeAsync(),
				useTransaction
			).SafeAsync();
			Assert.AreEqual( DEFAULT_STRING, fetchedComment );
			
		}
		
		[TestCase( false ), TestCase( true )]
		public async Task ExecReadFirstTest( bool useTransaction ) {
			PostgresCommand cmd;
			Guid testId1 = Guid.NewGuid();
			Guid testId2 = Guid.NewGuid();
			const string TEST_COMMENT = "ExecReadFirstTest";
			
			cmd = new PostgresCommand( @"
				INSERT INTO basic_table( id, comment ) VALUES( :id1, :comment1 );
				INSERT INTO basic_table( id ) VALUES( :id2 );"
			);
			cmd.AddParameter<Guid>( "id1", testId1 );
			cmd.AddParameter<string>( "comment1", TEST_COMMENT );
			cmd.AddParameter<Guid>( "id2", testId2 );
			Assert.AreEqual( 2, await m_database.ExecNonQueryAsync( cmd ).SafeAsync() );
			
			TestRecord fetchedRecord = null;
			PostgresCommand fetchCommand = new PostgresCommand( @"
				SELECT id, comment FROM basic_table
					WHERE id = :id"
			);
			
			cmd = new PostgresCommand( fetchCommand );
			cmd.AddParameter<Guid>( "id", testId1 );
			await RunAsync(
				async executor => fetchedRecord = await executor.ExecReadFirstAsync<TestRecord>( cmd, TestRecord.DbConverter ).SafeAsync(),
				useTransaction
			).SafeAsync();
			Assert.AreEqual( testId1, fetchedRecord.Id );
			Assert.AreEqual( TEST_COMMENT, fetchedRecord.Comment );
			
			cmd = new PostgresCommand( fetchCommand );
			cmd.AddParameter<Guid>( "id", testId2 );
			await RunAsync(
				async executor => fetchedRecord = await executor.ExecReadFirstAsync<TestRecord>( cmd, TestRecord.DbConverter ).SafeAsync(),
				useTransaction
			).SafeAsync();
			Assert.AreEqual( testId2, fetchedRecord.Id );
			Assert.IsNull( fetchedRecord.Comment );
			
			cmd = new PostgresCommand( fetchCommand );
			cmd.AddParameter<Guid>( "id", Guid.NewGuid() );
			Assert.Throws<DataNotFoundException>( async() =>
				await RunAsync(
					async executor => fetchedRecord = await executor.ExecReadFirstAsync<TestRecord>( cmd, TestRecord.DbConverter ).SafeAsync(),
					useTransaction
				).SafeAsync()
			);
			
			cmd = new PostgresCommand( @"
				SELECT id, comment FROM basic_table
				ORDER BY comment ASC NULLS LAST"
			);
			await RunAsync(
				async executor => fetchedRecord = await executor.ExecReadFirstAsync<TestRecord>( cmd, TestRecord.DbConverter ).SafeAsync(),
				useTransaction
			).SafeAsync();
			Assert.AreEqual( testId1, fetchedRecord.Id );
			Assert.AreEqual( TEST_COMMENT, fetchedRecord.Comment );
			
		}
		
		[TestCase( false ), TestCase( true )]
		public async Task ExecReadFirstOrDefaultTest( bool useTransaction ) {
			PostgresCommand cmd;
			Guid testId1 = Guid.NewGuid();
			Guid testId2 = Guid.NewGuid();
			const string TEST_COMMENT = "ExecReadFirstOrDefaultTest";
			
			cmd = new PostgresCommand( @"
				INSERT INTO basic_table( id, comment ) VALUES( :id1, :comment1 );
				INSERT INTO basic_table( id ) VALUES( :id2 );"
			);
			cmd.AddParameter<Guid>( "id1", testId1 );
			cmd.AddParameter<string>( "comment1", TEST_COMMENT );
			cmd.AddParameter<Guid>( "id2", testId2 );
			Assert.AreEqual( 2, await m_database.ExecNonQueryAsync( cmd ).SafeAsync() );
			
			TestRecord fetchedRecord = null;
			PostgresCommand fetchCommand = new PostgresCommand( @"
				SELECT id, comment FROM basic_table
					WHERE id = :id"
			);
			
			cmd = new PostgresCommand( fetchCommand );
			cmd.AddParameter<Guid>( "id", testId1 );
			await RunAsync(
				async executor => fetchedRecord = await executor.ExecReadFirstOrDefaultAsync<TestRecord>( cmd, TestRecord.DbConverter, null ).SafeAsync(),
				useTransaction
			).SafeAsync();
			Assert.AreEqual( testId1, fetchedRecord.Id );
			Assert.AreEqual( TEST_COMMENT, fetchedRecord.Comment );
			
			cmd = new PostgresCommand( fetchCommand );
			cmd.AddParameter<Guid>( "id", testId2 );
			await RunAsync(
				async executor => fetchedRecord = await executor.ExecReadFirstOrDefaultAsync<TestRecord>( cmd, TestRecord.DbConverter, null ).SafeAsync(),
				useTransaction
			).SafeAsync();
			Assert.AreEqual( testId2, fetchedRecord.Id );
			Assert.IsNull( fetchedRecord.Comment );
			
			cmd = new PostgresCommand( fetchCommand );
			cmd.AddParameter<Guid>( "id", Guid.NewGuid() );
			await RunAsync(
				async executor => fetchedRecord = await executor.ExecReadFirstOrDefaultAsync<TestRecord>( cmd, TestRecord.DbConverter, null ).SafeAsync(),
				useTransaction
			).SafeAsync();
			Assert.IsNull( fetchedRecord );
			
		}
		
		[TestCase( false ), TestCase( true )]
		public async Task ExecReadOfflineTest( bool useTransaction ) {
			IReadOnlyList<TestRecord> results = null;
			PostgresCommand selectCommand = new PostgresCommand(
				"SELECT id, comment FROM basic_table"
			);
			
			await RunAsync(
				async executor => results = await executor.ExecReadOfflineAsync<TestRecord>(
					selectCommand,
					TestRecord.DbConverter
				).SafeAsync(),
				useTransaction
			).SafeAsync();
			CollectionAssert.IsEmpty( results );
			
			Guid testId1 = Guid.NewGuid();
			Guid testId2 = Guid.NewGuid();
			const string TEST_COMMENT = "ExecReadOfflineTest";
			
			PostgresCommand insertCommand = new PostgresCommand( @"
				INSERT INTO basic_table( id, comment ) VALUES( :id1, :comment1 );
				INSERT INTO basic_table( id ) VALUES( :id2 );"
			);
			insertCommand.AddParameter<Guid>( "id1", testId1 );
			insertCommand.AddParameter<string>( "comment1", TEST_COMMENT );
			insertCommand.AddParameter<Guid>( "id2", testId2 );
			Assert.AreEqual( 2, await m_database.ExecNonQueryAsync( insertCommand ).SafeAsync() );
			
			await RunAsync(
				async executor => results = await executor.ExecReadOfflineAsync<TestRecord>(
					selectCommand,
					TestRecord.DbConverter
				).SafeAsync(),
				useTransaction
			).SafeAsync();
			CollectionAssert.AreEquivalent(
				new[]{
					new TestRecord( testId1, TEST_COMMENT ),
					new TestRecord( testId2, null )
				},
				results
			);
		}
		
		[TestCase( false ), TestCase( true )]
		public async Task ExecReadColumnOfflineTest( bool useTransaction ) {
			IReadOnlyList<string> results = null;
			PostgresCommand selectCommand = new PostgresCommand(
				"SELECT comment FROM basic_table"
			);
			
			await RunAsync(
				async executor => results = await executor.ExecReadColumnOfflineAsync<string>( selectCommand ).SafeAsync(),
				useTransaction
			).SafeAsync();
			CollectionAssert.IsEmpty( results );
			
			const string TEST_COMMENT = "ExecReadColumnOfflineTest";
			
			PostgresCommand insertCommand = new PostgresCommand( @"
				INSERT INTO basic_table( id, comment ) VALUES( :id1, :comment1 );
				INSERT INTO basic_table( id ) VALUES( :id2 );"
			);
			insertCommand.AddParameter<Guid>( "id1", Guid.NewGuid() );
			insertCommand.AddParameter<string>( "comment1", TEST_COMMENT );
			insertCommand.AddParameter<Guid>( "id2", Guid.NewGuid() );
			Assert.AreEqual( 2, await m_database.ExecNonQueryAsync( insertCommand ).SafeAsync() );
			
			await RunAsync(
				async executor => results = await executor.ExecReadColumnOfflineAsync<string>( selectCommand ).SafeAsync(),
				useTransaction
			).SafeAsync();
			CollectionAssert.AreEquivalent(
				new[]{ TEST_COMMENT, null },
				results
			);
		}
		
		[TestCase( false ), TestCase( true )]
		public async Task BindNullValuedParameterTest( bool useTransaction ) {
			PostgresCommand cmd;
			Guid testId = Guid.NewGuid();
			
			cmd = new PostgresCommand(
				"INSERT INTO basic_table( id, comment ) VALUES( :id, :comment )"
			);
			cmd.AddParameter<Guid>( "id", testId );
			cmd.AddParameter<string>( "comment", null );
			
			int rowsAdded = 0;
			await RunAsync(
				async executor => rowsAdded = await executor.ExecNonQueryAsync( cmd ).SafeAsync(),
				useTransaction
			).SafeAsync();
			Assert.AreEqual( 1, rowsAdded );
			
			
			cmd = new PostgresCommand( @"
				SELECT comment FROM basic_table
					WHERE id = :id"
			);
			cmd.AddParameter<Guid>( "id", testId );
			
			string fetchedComment = string.Empty;
			await RunAsync(
				async executor => fetchedComment = await executor.ExecReadScalarAsync<string>( cmd ).SafeAsync(),
				useTransaction
			).SafeAsync();
			Assert.IsNull( fetchedComment );
			
		}
		
		[TestCase( false ), TestCase( true )]
		[Description(
			"If an error occurs in any statement in a single multistatement " +
			"command, the entire command should be rolled back, even if it " +
			"is not in an explicit transaction." )]
		public async Task MultiStatementCommand_RollbackOnError( bool useTransaction ) {
			PostgresCommand cmd;
			
			Guid id = Guid.NewGuid();
			
			cmd = new PostgresCommand( @"
				INSERT INTO basic_table( id )
					VALUES( :id );
				INSERT INTO basic_table( id )
					VALUES( :id )"
			);
			cmd.AddParameter<Guid>( "id", id );
			
			// Expect primary key violation
			bool caughtException = false;
			try {
				await RunAsync(
					executor => executor.ExecNonQueryAsync( cmd ),
					useTransaction
				).SafeAsync();
			} catch( PostgresException exception ) {
				caughtException = true;
				Assert.AreEqual(
					PostgresErrorClass.IntegrityConstraintViolation,
					exception.GetErrorClass()
				);
			}
			Assert.IsTrue( caughtException );
			
			// The first command should have been rolled back, so there should
			// be no data in the table with the inserted id
			cmd = new PostgresCommand( @"
				SELECT count(*)
				FROM basic_table
					WHERE id = :id"
			);
			cmd.AddParameter<Guid>( "id", id );
			
			long numRows = 0;
			await RunAsync(
				async executor => numRows = await executor.ExecReadScalarAsync<long>( cmd ).SafeAsync(),
				useTransaction
			).SafeAsync();
			
			Assert.AreEqual( 0, numRows );
			
		}
		
		private Task CleanDatabaseAsync() {
			PostgresCommand cmd = new PostgresCommand( @"
				DELETE FROM basic_table;
				DELETE FROM datetime_table;"
			);
			return m_database.ExecNonQueryAsync( cmd );
		}
		
		private sealed class TestRecord {
			private readonly Guid m_id;
			private readonly string m_comment;
			
			public TestRecord( Guid id, string comment ) {
				m_id = id;
				m_comment = comment;
			}
			
			public Guid Id { get { return m_id; } }
			public string Comment { get { return m_comment; } }
			
			internal static TestRecord DbConverter( IDataRecord record ) {
				return new TestRecord(
					record.Get<Guid>( "id" ),
					record.Get<string>( "comment" )
				);
			}
			
			public override bool Equals( object obj ) {
				TestRecord that = obj as TestRecord;
				return(
				    that != null &&
				    object.Equals( this.Id, that.Id ) &&
				    object.Equals( this.Comment, that.Comment )
				);
			}
			
			public override int GetHashCode() {
				return m_id.GetHashCode();
			}
		}
		
	}
}
