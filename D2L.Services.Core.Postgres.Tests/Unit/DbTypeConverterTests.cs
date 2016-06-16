using System.Collections.Generic;
using D2L.Services.Core.Postgres.Tests.Types;
using D2L.Services.Core.Postgres.TypeConverters;
using D2L.Services.Core.Postgres.TypeConverters.Default;
using D2L.Services.Core.TestFramework;
using NUnit.Framework;
using System;

namespace D2L.Services.Core.Postgres.Tests.Unit {
	
	[TestFixture, Unit]
	internal sealed class DbTypeConverterTests {
		
		[Test]
		public void CustomTypeConverter_Simple() {
			var converter = DbTypeConverter<TestId>.Converter;
			Assert.IsTrue( converter is TestId.PgConverter );
		}
		
		[Test]
		public void CustomTypeConverter_Nullable() {
			var converter = DbTypeConverter<TestId?>.Converter;
			Assert.IsTrue( converter is NullableTypeConverter<TestId> );
			
			var innerConverter = ((NullableTypeConverter<TestId>)converter).m_innerConverter;
			Assert.IsTrue( innerConverter is TestId.PgConverter );
		}
		
		[Test]
		public void CustomTypeConverter_Array() {
			var converter = DbTypeConverter<TestId[]>.Converter;
			Assert.IsTrue( converter is ArrayTypeConverter<TestId> );
			
			var innerConverter = ((ArrayTypeConverter<TestId>)converter).m_innerConverter;
			Assert.IsTrue( innerConverter is TestId.PgConverter );
		}
		
		[Test]
		public void CustomTypeConverter_List() {
			var converter = DbTypeConverter<List<TestId>>.Converter;
			Assert.IsTrue( converter is ListTypeConverter<TestId> );
			
			var innerConverter = ((ListTypeConverter<TestId>)converter).m_innerConverter;
			Assert.IsTrue( innerConverter is TestId.PgConverter );
		}
		
		[Test]
		public void CustomTypeConverter_IList() {
			var converter = DbTypeConverter<IList<TestId>>.Converter;
			Assert.IsTrue( converter is ListTypeConverter<TestId> );
			
			var innerConverter = ((ListTypeConverter<TestId>)converter).m_innerConverter;
			Assert.IsTrue( innerConverter is TestId.PgConverter );
		}
		
		[Test]
		public void CustomTypeConverter_IEnumerable() {
			var converter = DbTypeConverter<IEnumerable<TestId>>.Converter;
			Assert.IsTrue( converter is EnumerableTypeConverter<TestId> );
			
			var listConverter = ((EnumerableTypeConverter<TestId>)converter).m_listConverter;
			Assert.IsTrue( listConverter is ListTypeConverter<TestId> );
			
			var innerConverter = ((ListTypeConverter<TestId>)listConverter).m_innerConverter;
			Assert.IsTrue( innerConverter is TestId.PgConverter );
		}
		
		[Test]
		public void CustomTypeConverter_ArrayOfNullables() {
			var converter = DbTypeConverter<TestId?[]>.Converter;
			Assert.IsTrue( converter is ArrayTypeConverter<TestId?> );
			
			var nullableConverter = ((ArrayTypeConverter<TestId?>)converter).m_innerConverter;
			Assert.IsTrue( nullableConverter is NullableTypeConverter<TestId> );
			
			var innerConverter = ((NullableTypeConverter<TestId>)nullableConverter).m_innerConverter;
			Assert.IsTrue( innerConverter is TestId.PgConverter ); 
		}
		
		[Test]
		public void CustomTypeConverter_IEnumerableOfNullables() {
			var converter = DbTypeConverter<IEnumerable<TestId?>>.Converter;
			Assert.IsTrue( converter is EnumerableTypeConverter<TestId?> );
			
			var listConverter = ((EnumerableTypeConverter<TestId?>)converter).m_listConverter;
			Assert.IsTrue( listConverter is ListTypeConverter<TestId?> );
			
			var nullableConverter = ((ListTypeConverter<TestId?>)listConverter).m_innerConverter;
			Assert.IsTrue( nullableConverter is NullableTypeConverter<TestId> );
			
			var innerConverter = ((NullableTypeConverter<TestId>)nullableConverter).m_innerConverter;
			Assert.IsTrue( innerConverter is TestId.PgConverter );
		}
		
		[Test]
		public void DateTimeConverter_Simple() {
			var converter = DbTypeConverter<DateTime>.Converter;
			Assert.IsTrue( converter is DateTimeTypeConverter );
		}
		
		[Test]
		public void DateTimeConverter_IEnumerableOfNullables() {
			var converter = DbTypeConverter<IEnumerable<DateTime?>>.Converter;
			Assert.IsTrue( converter is EnumerableTypeConverter<DateTime?> );
			
			var listConverter = ((EnumerableTypeConverter<DateTime?>)converter).m_listConverter;
			Assert.IsTrue( listConverter is ListTypeConverter<DateTime?> );
			
			var nullableConverter = ((ListTypeConverter<DateTime?>)listConverter).m_innerConverter;
			Assert.IsTrue( nullableConverter is NullableTypeConverter<DateTime> );
			
			var innerConverter = ((NullableTypeConverter<DateTime>)nullableConverter).m_innerConverter;
			Assert.IsTrue( innerConverter is DateTimeTypeConverter );
		}
		
	}
	
}
