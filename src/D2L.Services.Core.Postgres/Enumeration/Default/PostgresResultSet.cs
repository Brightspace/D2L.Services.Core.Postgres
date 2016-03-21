using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Npgsql;

namespace D2L.Services.Core.Postgres.Enumeration.Default {
	
	internal sealed partial class PostgresResultSet<Dto> : IOnlineResultSet<Dto> {
		
		private readonly IAsyncEnumerator<Dto> m_enumerator;
		
		public PostgresResultSet(
			DbDataReader reader,
			NpgsqlCommand command,
			Func<IDataRecord,Dto> dbConverter
		) {
			m_enumerator = new InternalEnumerator(
				reader,
				command,
				dbConverter
			);
		}
		
		void IDisposable.Dispose() {
			m_enumerator.SafeDispose();
		}
		
		
		async Task IAsyncEnumerable<Dto>.ForEachAsync( Action<Dto> function ) {
			while( await m_enumerator.MoveNextAsync().SafeAsync() ) {
				function( m_enumerator.Current );
			}
		}
		
		async Task IAsyncEnumerable<Dto>.ForEachAsync( Func<Dto, Task> asyncFunction ) {
			while( await m_enumerator.MoveNextAsync().SafeAsync() ) {
				await asyncFunction( m_enumerator.Current ).SafeAsync();
			}
		}
		
		
		IEnumerator<Dto> IEnumerable<Dto>.GetEnumerator() {
			return m_enumerator;
		}
		
		IEnumerator IEnumerable.GetEnumerator() {
			return m_enumerator;
		}
		
		IAsyncEnumerator<Dto> IAsyncEnumerable<Dto>.GetAsyncEnumerator() {
			return m_enumerator;
		}
		
	}
	
}
