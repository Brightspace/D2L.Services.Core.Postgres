using D2L.Services.Core.Postgres.TypeConverters;
using D2L.Services.Core.Postgres.TypeConverters.Default;
using D2L.Services.Core.TestFramework;
using NpgsqlTypes;
using NUnit.Framework;
using System;

namespace D2L.Services.Core.Postgres.Tests.Unit {
	
	[TestFixture, Unit]
	public sealed class DefaultTypeConverterTests {
		
		/*
		 * DefaultTypeConverter uses Reflection to access the internal
		 * TypeHandlerRegistry in Npgsql. If this test breaks, that likely
		 * means that the internals of Npgsql have changed.
		 */
		
		[Test]
		public void GetDatabaseTypeTest() {
			VerifyDbType<int>( NpgsqlDbType.Integer );
			VerifyDbType<long>( NpgsqlDbType.Bigint );
			VerifyDbType<decimal>( NpgsqlDbType.Numeric );
			VerifyDbType<string>( NpgsqlDbType.Text );
			VerifyDbType<Guid>( NpgsqlDbType.Uuid );
		}
		
		private static void VerifyDbType<T>( NpgsqlDbType expectedType ) {
			IPostgresTypeConverter<T> converter = new DefaultTypeConverter<T>();
			Assert.AreEqual( expectedType, converter.DatabaseType );
		}
		
	}
	
}
