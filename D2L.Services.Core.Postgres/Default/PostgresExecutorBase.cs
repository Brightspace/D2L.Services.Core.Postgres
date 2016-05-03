using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres.Default {
	
	internal abstract class PostgresExecutorBase : IPostgresExecutor {
		
		#region Sync
		public int ExecNonQuery(
			PostgresCommand command
		) {
			int rowsAffected = -1;
			ExecuteSync( command, cmd => {
				rowsAffected = cmd.ExecuteNonQuery();
			});
			return rowsAffected;
		}
		
		public T ExecReadScalar<T>(
			PostgresCommand command
		) {
			object result = null;
			ExecuteSync( command, cmd => {
				result = cmd.ExecuteScalar();
			});
			
			if( result == null ) {
				throw new DataNotFoundException(
					"The SQL query did not return a scalar value."
				);
			}
			
			return ConvertFromDb<T>( result );
		}
		
		public T ExecReadScalarOrDefault<T>(
			PostgresCommand command,
			T defaultValue = default( T )
		) {
			object result = null;
			ExecuteSync( command, cmd => {
				result = cmd.ExecuteScalar();
			});
			
			if( result == null ) {
				return defaultValue;
			}
			
			return ConvertFromDb<T>( result );
		}
		
		public bool ExecTryReadScalar<T>(
			PostgresCommand command,
			out T value
		) {
			value = default( T );
			
			object result = null;
			ExecuteSync( command, cmd => {
				result = cmd.ExecuteScalar();
			});
			
			if( result == null ) {
				return false;
			}
			
			value = ConvertFromDb<T>( result );
			return true;
		}
		
		public Dto ExecReadFirst<Dto>(
			PostgresCommand command,
			Func<IDataRecord, Dto> dbConverter
		) {
			Dto result = default( Dto );
			ExecuteSync( command, cmd => {
				using( DbDataReader reader = cmd.ExecuteReader() ) {
					if( reader.Read() ) {
						result = dbConverter( reader );
					} else {
						throw new DataNotFoundException(
							"The SQL query did not return any records."
						);
					}
				}
			});
			
			return result;
		}
		
		public Dto ExecReadFirstOrDefault<Dto>(
			PostgresCommand command,
			Func<IDataRecord, Dto> dbConverter,
			Dto defaultValue = default( Dto )
		) {
			Dto result = defaultValue;
			ExecuteSync( command, cmd => {
				using( DbDataReader reader = cmd.ExecuteReader() ) {
					result = reader.Read() ? dbConverter( reader ) : defaultValue;
				}
			});
			return result;
		}
		
		public bool ExecTryReadFirst<Dto>(
			PostgresCommand command,
			Func<IDataRecord, Dto> dbConverter,
			out Dto dto
		) {
			bool success = false;
			Dto result = default( Dto );
			
			ExecuteSync( command, cmd => {
				using( DbDataReader reader = cmd.ExecuteReader() ) {
					if( reader.Read() ) {
						result = dbConverter( reader );
						success = true;
					}
				}
			});
			
			dto = result;
			return success;
		}
		
		public IReadOnlyList<Dto> ExecReadOffline<Dto>(
			PostgresCommand command,
			Func<IDataRecord, Dto> dbConverter
		) {
			List<Dto> results = new List<Dto>();
			ExecuteSync( command, cmd => {
				using( DbDataReader reader = cmd.ExecuteReader() ) {
					while( reader.Read() ) {
						results.Add( dbConverter( reader ) );
					}
				}
			});
			return results;
		}
		
		protected abstract void ExecuteSync(
			PostgresCommand command,
			Action<NpgsqlCommand> action
		);
		#endregion
		
		#region Async
		public async Task<int> ExecNonQueryAsync(
			PostgresCommand command
		) {
			int rowsAffected = -1;
			await ExecuteAsync( command, async cmd => {
				rowsAffected = await cmd.ExecuteNonQueryAsync().SafeAsync();
			}).SafeAsync();
			return rowsAffected;
		}
		
		public async Task<T> ExecReadScalarAsync<T>(
			PostgresCommand command
		) {
			object result = null;
			await ExecuteAsync( command, async cmd => {
				result = await cmd.ExecuteScalarAsync().SafeAsync();
			}).SafeAsync();
			
			if( result == null ) {
				throw new DataNotFoundException(
					"The SQL query did not return a scalar value."
				);
			}
			
			return ConvertFromDb<T>( result );
		}
		
		public async Task<T> ExecReadScalarOrDefaultAsync<T>(
			PostgresCommand command,
			T defaultValue = default( T )
		) {
			object result = null;
			await ExecuteAsync( command, async cmd => {
				result = await cmd.ExecuteScalarAsync().SafeAsync();
			}).SafeAsync();
			
			if( result == null ) {
				return defaultValue;
			}
			
			return ConvertFromDb<T>( result );
		}
		
		public async Task<Dto> ExecReadFirstAsync<Dto>(
			PostgresCommand command,
			Func<IDataRecord, Dto> dbConverter
		) {
			Dto result = default( Dto );
			await ExecuteAsync( command, async cmd => {
				using( DbDataReader reader = await cmd.ExecuteReaderAsync().SafeAsync() ) {
					if( await reader.ReadAsync().SafeAsync() ) {
						result = dbConverter( reader );
					} else {
						throw new DataNotFoundException(
							"The SQL query did not return any records."
						);
					}
				}
			}).SafeAsync();
			return result;
		}
		
		public async Task<Dto> ExecReadFirstOrDefaultAsync<Dto>(
			PostgresCommand command,
			Func<IDataRecord, Dto> dbConverter,
			Dto defaultValue = default( Dto )
		) {
			Dto result = defaultValue;
			await ExecuteAsync( command, async cmd => {
				using( DbDataReader reader = await cmd.ExecuteReaderAsync().SafeAsync() ) {
					result = (await reader.ReadAsync().SafeAsync()) ?
						dbConverter( reader ) : defaultValue;
				}
			}).SafeAsync();
			return result;
		}
		
		public async Task<IReadOnlyList<Dto>> ExecReadOfflineAsync<Dto>(
			PostgresCommand command,
			Func<IDataRecord, Dto> dbConverter
		) {
			List<Dto> results = new List<Dto>();
			await ExecuteAsync( command, async cmd => {
				using( DbDataReader reader = await cmd.ExecuteReaderAsync().SafeAsync() ) {
					while( await reader.ReadAsync().SafeAsync() ) {
						results.Add( dbConverter( reader ) );
					}
				}
			}).SafeAsync();
			return results;
		}
		
		protected abstract Task ExecuteAsync(
			PostgresCommand command,
			Func<NpgsqlCommand,Task> action
		);
		#endregion
		
		private T ConvertFromDb<T>( object dbValue ) {
			//TODO[v1.2.0] add support for type converters
			if( dbValue is DBNull ) {
				return (T)(object)null;
			} else {
				return (T)dbValue;
			}
		}
	}
	
}
