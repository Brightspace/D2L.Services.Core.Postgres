using System;
using System.Linq;
using System.Threading.Tasks;
using D2L.Services.Core.TestFramework;
using NUnit.Framework;

namespace D2L.Services.Core.Postgres.Tests.Integration {
	
	[TestFixture, Integration, RequiresDatabase]
	internal sealed class DateTimeHandlingTests : IntegrationTestFixtureBase {
		
		private DateTime m_localTime;
		private DateTime m_utcTime;
		
		private const string INSERT_SQL = @"
			INSERT INTO datetime_table( local_timestamp, utc_timestamp )
			VALUES( :local_time, :utc_time )
			RETURNING id";
		
		[TestFixtureSetUp]
		public void TestFixtureSetUp() {
			m_localTime = RoundToMilliseconds( DateTime.Now );
			m_utcTime = RoundToMilliseconds( DateTime.UtcNow );
			
			Assert.AreEqual( DateTimeKind.Local, m_localTime.Kind );
			Assert.AreEqual( DateTimeKind.Utc, m_utcTime.Kind );
		}
		
		[TestFixtureTearDown]
		public void TestFixtureTearDown() {
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
			
			cmd = new PostgresCommand( @"
				SELECT utc_timestamp FROM datetime_table
				WHERE id = :id"
			);
			cmd.AddParameter<int>( "id", id );
			fetchedDateTime = await m_database.ExecReadScalarAsync<DateTime>( cmd ).SafeAsync();
			Assert.AreEqual( DateTimeKind.Utc, fetchedDateTime.Kind );
			Assert.AreEqual( m_utcTime, fetchedDateTime );
			
			cmd = new PostgresCommand( @"
				SELECT local_timestamp FROM datetime_table
				WHERE id = :id"
			);
			cmd.AddParameter<int>( "id", id );
			fetchedDateTime = await m_database.ExecReadScalarAsync<DateTime>( cmd ).SafeAsync();
			Assert.AreEqual( DateTimeKind.Local, fetchedDateTime.Kind );
			Assert.AreEqual( m_localTime, fetchedDateTime );
		}
		
		[Test]
		public async Task WriteAndRead_NullableDateTimeWithValue() {
			PostgresCommand cmd = new PostgresCommand( INSERT_SQL );
			cmd.AddParameter<DateTime?>( "local_time", m_localTime );
			cmd.AddParameter<DateTime?>( "utc_time", m_utcTime );
			int id = await m_database.ExecReadScalarAsync<int>( cmd ).SafeAsync();
			
			DateTime? fetchedDateTime;
			
			cmd = new PostgresCommand( @"
				SELECT utc_timestamp FROM datetime_table
				WHERE id = :id"
			);
			cmd.AddParameter<int>( "id", id );
			fetchedDateTime = await m_database.ExecReadScalarAsync<DateTime?>( cmd ).SafeAsync();
			Assert.AreEqual( DateTimeKind.Utc, fetchedDateTime.Value.Kind );
			Assert.AreEqual( m_utcTime, fetchedDateTime.Value );
			
			cmd = new PostgresCommand( @"
				SELECT local_timestamp FROM datetime_table
				WHERE id = :id"
			);
			cmd.AddParameter<int>( "id", id );
			fetchedDateTime = await m_database.ExecReadScalarAsync<DateTime?>( cmd ).SafeAsync();
			Assert.AreEqual( DateTimeKind.Local, fetchedDateTime.Value.Kind );
			Assert.AreEqual( m_localTime, fetchedDateTime.Value );
		}
		
		[Test]
		public async Task WriteAndRead_NullDateTime() {
			PostgresCommand cmd = new PostgresCommand( INSERT_SQL );
			cmd.AddParameter<DateTime?>( "local_time", null );
			cmd.AddParameter<DateTime?>( "utc_time", null );
			int id = await m_database.ExecReadScalarAsync<int>( cmd ).SafeAsync();
			
			DateTime? fetchedDateTime;
			
			cmd = new PostgresCommand( @"
				SELECT utc_timestamp FROM datetime_table
				WHERE id = :id"
			);
			cmd.AddParameter<int>( "id", id );
			fetchedDateTime = await m_database.ExecReadScalarAsync<DateTime?>( cmd ).SafeAsync();
			Assert.IsFalse( fetchedDateTime.HasValue );
			Assert.IsNull( fetchedDateTime );
			
			cmd = new PostgresCommand( @"
				SELECT local_timestamp FROM datetime_table
				WHERE id = :id"
			);
			cmd.AddParameter<int>( "id", id );
			fetchedDateTime = await m_database.ExecReadScalarAsync<DateTime?>( cmd ).SafeAsync();
			Assert.IsFalse( fetchedDateTime.HasValue );
			Assert.IsNull( fetchedDateTime );
		}
		
		
		private static DateTime RoundToMilliseconds( DateTime timestamp ) {
			return new DateTime(
				ticks: timestamp.Ticks - ( timestamp.Ticks % TimeSpan.TicksPerMillisecond ),
				kind: timestamp.Kind
			);
		}
		
	}
	
}
