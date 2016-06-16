using NpgsqlTypes;
using System;
using D2L.Services.Core.Postgres.TypeConverters;

namespace D2L.Services.Core.Postgres.Tests.Types {
	
	[PostgresTypeConverter( typeof( TestId.PgConverter ) )]
	internal struct TestId {
		
		private readonly Guid m_uuid;
		
		public TestId( Guid id ) {
			m_uuid = id;
		}
		
		public override bool Equals( object obj ) {
			if( obj is TestId ) {
				return object.Equals( m_uuid, ((TestId)obj).m_uuid );
			} else if( obj != null && obj is TestId? ) {
				return object.Equals( m_uuid, ((TestId?)obj).Value.m_uuid );
			}
			return false;
		}
		
		public override int GetHashCode() {
			return m_uuid.GetHashCode();
		}
		
		public static bool operator ==( TestId left, TestId right ) {
			return left.Equals( right );
		}
		
		public static bool operator !=( TestId left, TestId right ) {
			return !left.Equals( right );
		}
		
		internal sealed class PgConverter : IPostgresTypeConverter<TestId> {
			
			object IPostgresTypeConverter<TestId>.ToDbValue( TestId value ) {
				return value.m_uuid;
			}
			
			TestId IPostgresTypeConverter<TestId>.FromDbValue( object dbValue ) {
				return new TestId( (Guid)dbValue );
			}
			
			NpgsqlDbType IPostgresTypeConverter<TestId>.DatabaseType {
				get { return NpgsqlDbType.Uuid; }
			}
			
		}
		
	}
	
}
