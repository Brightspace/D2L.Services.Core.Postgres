using System;
using NUnit.Framework;

namespace D2L.Services.Core.Postgres.Tests {
	
	[TestFixture]
	internal abstract class IntegrationTestFixtureBase {
		
		protected IPostgresDatabase m_database;
		
		private const string TEST_CONNECTION_STRING =
			"User ID=postgres;" +
			"Password=postgres;" +
			"Host=localhost;" +
			"Port=5432;" +
			"Database=postgres-library-test;";
		
		[TestFixtureSetUp]
		public void TestFixtureSetUpBase() {
			m_database = PostgresDatabaseProvider.Create( TEST_CONNECTION_STRING );
		}
		
	}
	
}
