using System;
using System.Threading.Tasks;
using D2L.Services.Core.TestFramework;
using NUnit.Framework;

namespace D2L.Services.Core.Postgres.Tests.Integration {
	
	[TestFixture, Integration, RequiresDatabase]
	internal sealed class DateTimeHandlingTests : IntegrationTestFixtureBase {
		
		private const string INSERT_SQL = @"
			INSERT INTO datetime_table( local_timestamp, utc_timestamp )
			VALUES( :local_time, :utc_time )
			RETURNING id";
		
		private const string SELECT_LOCAL_SQL = @"
			SELECT local_timestamp FROM datetime_table
			WHERE id = :id";
		
		private const string SELECT_UTC_SQL = @"
			SELECT utc_timestamp FROM datetime_table
			WHERE id = :id";
		
		private DateTime m_localTime;
		private DateTime m_utcTime;
		
		[OneTimeSetUp]
		public void TestFixtureSetUp() {
			m_localTime = RoundToMilliseconds( DateTime.Now );
			m_utcTime = RoundToMilliseconds( DateTime.UtcNow );
			
			Assert.AreEqual( DateTimeKind.Local, m_localTime.Kind );
			Assert.AreEqual( DateTimeKind.Utc, m_utcTime.Kind );
		}
		
		[SetUp, OneTimeTearDown]
		public void Cleanup() {
			PostgresCommand cmd = new PostgresCommand(
				"DELETE FROM datetime_table"
			);
			m_database.ExecNonQueryAsync( cmd ).SafeWait();
		}
		
		
		[Test]
		public async Task WriteAndRead_DateTime() {
			PostgresCommand cmd = new PostgresCommand( INSERT_SQL );
			cmd.AddParameter<DateTime>( "local_time", m_localTime );
			cmd.AddParameter<DateTime>( "utc_time", m_utcTime );
			int id = await m_database.ExecReadScalarAsync<int>( cmd ).SafeAsync();
			
			DateTime fetchedDateTime;
			
			cmd = new PostgresCommand( SELECT_UTC_SQL );
			cmd.AddParameter<int>( "id", id );
			fetchedDateTime = await m_database.ExecReadScalarAsync<DateTime>( cmd ).SafeAsync();
			Assert.AreEqual( DateTimeKind.Utc, fetchedDateTime.Kind );
			Assert.AreEqual( m_utcTime, fetchedDateTime );
			
			cmd = new PostgresCommand( SELECT_LOCAL_SQL );
			cmd.AddParameter<int>( "id", id );
			fetchedDateTime = await m_database.ExecReadScalarAsync<DateTime>( cmd ).SafeAsync();
			Assert.AreEqual( DateTimeKind.Local, fetchedDateTime.Kind );
			Assert.AreEqual( m_localTime, fetchedDateTime );
		}
		
		[Test]
		public async Task WriteAndRead_NullableDateTime_WithValue() {
			PostgresCommand cmd = new PostgresCommand( INSERT_SQL );
			cmd.AddParameter<DateTime?>( "local_time", m_localTime );
			cmd.AddParameter<DateTime?>( "utc_time", m_utcTime );
			int id = await m_database.ExecReadScalarAsync<int>( cmd ).SafeAsync();
			
			DateTime? fetchedDateTime;
			
			cmd = new PostgresCommand( SELECT_UTC_SQL );
			cmd.AddParameter<int>( "id", id );
			fetchedDateTime = await m_database.ExecReadScalarAsync<DateTime?>( cmd ).SafeAsync();
			Assert.AreEqual( DateTimeKind.Utc, fetchedDateTime.Value.Kind );
			Assert.AreEqual( m_utcTime, fetchedDateTime.Value );
			
			cmd = new PostgresCommand( SELECT_LOCAL_SQL );
			cmd.AddParameter<int>( "id", id );
			fetchedDateTime = await m_database.ExecReadScalarAsync<DateTime?>( cmd ).SafeAsync();
			Assert.AreEqual( DateTimeKind.Local, fetchedDateTime.Value.Kind );
			Assert.AreEqual( m_localTime, fetchedDateTime.Value );
		}
		
		[Test]
		public async Task WriteAndRead_NullableDateTime_NullValue() {
			PostgresCommand cmd = new PostgresCommand( INSERT_SQL );
			cmd.AddParameter<DateTime?>( "local_time", null );
			cmd.AddParameter<DateTime?>( "utc_time", null );
			int id = await m_database.ExecReadScalarAsync<int>( cmd ).SafeAsync();
			
			DateTime? fetchedDateTime;
			
			cmd = new PostgresCommand( SELECT_UTC_SQL );
			cmd.AddParameter<int>( "id", id );
			fetchedDateTime = await m_database.ExecReadScalarAsync<DateTime?>( cmd ).SafeAsync();
			Assert.IsFalse( fetchedDateTime.HasValue );
			Assert.IsNull( fetchedDateTime );
			
			cmd = new PostgresCommand( SELECT_LOCAL_SQL );
			cmd.AddParameter<int>( "id", id );
			fetchedDateTime = await m_database.ExecReadScalarAsync<DateTime?>( cmd ).SafeAsync();
			Assert.IsFalse( fetchedDateTime.HasValue );
			Assert.IsNull( fetchedDateTime );
		}
		
		[Test]
		public async Task WriteAndRead_DateTimeArray() {
			DateTime[] localTimestamps = {
				RoundToMilliseconds( DateTime.Now ),
				DotNetExtensions.UNIX_EPOCH.ToLocalTime()
			};
			
			DateTime[] utcTimestamps = {
				RoundToMilliseconds( DateTime.UtcNow ),
				DotNetExtensions.UNIX_EPOCH
			};
			
			PostgresCommand cmd = new PostgresCommand( @"
				INSERT INTO datetime_array_table( utc_timestamps, local_timestamps )
				VALUES( :utc_times, :local_times )
				RETURNING id"
			);
			cmd.AddParameter<DateTime[]>( "utc_times", utcTimestamps );
			cmd.AddParameter<DateTime[]>( "local_times", localTimestamps );
			
			int id = await m_database.ExecReadScalarAsync<int>( cmd ).SafeAsync();
			DateTime[] fetchedTimestamps;
			
			// Verify UTC timestamps
			
			cmd = new PostgresCommand( @"
				SELECT utc_timestamps FROM datetime_array_table
				WHERE id = :id"
			);
			cmd.AddParameter<int>( "id", id );
			
			fetchedTimestamps = await m_database.ExecReadScalarAsync<DateTime[]>( cmd ).SafeAsync();
			foreach( DateTime timestamp in fetchedTimestamps ) {
				Assert.AreEqual( DateTimeKind.Utc, timestamp.Kind );
			}
			CollectionAssert.AreEqual( utcTimestamps, fetchedTimestamps );
			
			// Verify local timestamps
			
			cmd = new PostgresCommand( @"
				SELECT local_timestamps FROM datetime_array_table
				WHERE id = :id"
			);
			cmd.AddParameter<int>( "id", id );
			
			fetchedTimestamps = await m_database.ExecReadScalarAsync<DateTime[]>( cmd ).SafeAsync();
			foreach( DateTime timestamp in fetchedTimestamps ) {
				Assert.AreEqual( DateTimeKind.Local, timestamp.Kind );
			}
			CollectionAssert.AreEqual( localTimestamps, fetchedTimestamps );
		}
		
		
		private static DateTime RoundToMilliseconds( DateTime timestamp ) {
			return new DateTime(
				ticks: timestamp.Ticks - ( timestamp.Ticks % TimeSpan.TicksPerMillisecond ),
				kind: timestamp.Kind
			);
		}
		
	}
	
}
