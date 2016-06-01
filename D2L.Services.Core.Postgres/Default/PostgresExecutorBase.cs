using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres.Default {
	
	internal abstract class PostgresExecutorBase : IPostgresExecutor {
		
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
			
			return DbTypeConverter.FromDbValue<T>( result );
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
			
			return DbTypeConverter.FromDbValue<T>( result );
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
		
	}
	
}
