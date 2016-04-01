using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres.Enumeration.Default {
	
	internal sealed partial class PostgresResultSet<Dto> {
		
		private sealed class InternalEnumerator : IAsyncEnumerator<Dto> {
			
			private readonly DbDataReader m_reader;
			private readonly NpgsqlCommand m_command;
			private readonly Func<IDataRecord,Dto> m_dbConverter;
			
			private bool m_disposed = false;
			
			internal InternalEnumerator(
				DbDataReader reader,
				NpgsqlCommand command,
				Func<IDataRecord,Dto> dbConverter
			) {
				m_reader = reader;
				m_command = command;
				m_dbConverter = dbConverter;
			}
			
			~InternalEnumerator() {
				if( m_disposed ) {
					return;
				}
				
				try {
					Trace.TraceError(
						"An online database result set is no longer being "+
						"used, but it has not been disposed. You must dispose "+
						"the result set either by fully enumerating it or by "+
						"explicitly calling Dispose()."
					);
				} catch {}
			}
			
			void IDisposable.Dispose() {
				if( m_disposed ) {
					return;
				}
				
				NpgsqlConnection connection = m_command.Connection;
				NpgsqlTransaction transaction = m_command.Transaction;
				try {
					m_reader.Dispose();
					transaction.Commit();
				} finally {
					m_disposed = true;
					// Using 'using' statements ensures that Dispose() gets
					// called on everything even if one of the Dipose()
					// methods throws an exception
					using( connection ) {
						using( transaction ) {
							using( m_command ) {}
						}
					}
				}
			}
			
			async Task<bool> IAsyncEnumerator<Dto>.MoveNextAsync() {
				AssertNotDisposed();
				if( await m_reader.ReadAsync().SafeAsync() ) {
					return true;
				} else {
					((IDisposable)this).Dispose();
					return false;
				}
			}
			
			bool IEnumerator.MoveNext() {
				AssertNotDisposed();
				if( m_reader.Read() ) {
					return true;
				} else {
					((IDisposable)this).Dispose();
					return false;
				}
			}
			
			void IEnumerator.Reset() {
				throw new NotSupportedException(
					"Multiple enumeration of an online Postgres result set."
				);
			}
			
			object IEnumerator.Current {
				get { return ((IEnumerator<Dto>)this).Current; }
			}
			
			Dto IEnumerator<Dto>.Current {
				get {
					AssertNotDisposed();
					return m_dbConverter( m_reader );
				}
			}
			
			private void AssertNotDisposed() {
				if( m_disposed ) {
					throw new ObjectDisposedException( "InternalEnumerator" );
				}
			}
			
		}
		
	}
	
}
