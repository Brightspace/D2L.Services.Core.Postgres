
using System;
using System.Threading.Tasks;
using D2L.Services.Core.TestFramework;
using Npgsql;
using NUnit.Framework;

namespace D2L.Services.Core.Postgres.Tests.Integration {
	
	[TestFixture, Integration, RequiresDatabase]
	internal sealed class PostgresTransactionTests : IntegrationTestFixtureBase {
		
		[Test, Timed]
		public async Task TransactionRollback_ExpectCorrectErrorClass() {
			
			Guid id = Guid.NewGuid();
			PostgresCommand cmd = new PostgresCommand( @"
				INSERT INTO basic_table( id )
				VALUES( :id )"
			);
			cmd.AddParameter<Guid>( "id", id );
			await m_database.ExecNonQueryAsync( cmd ).SafeAsync();
			
			Task transactionA = RunTransactionAsync( id );
			Task transactionB = RunTransactionAsync( id );
			
			try {
				await transactionA.SafeAsync();
				await transactionB.SafeAsync();
			} catch( PostgresException exception ) {
				Assert.AreEqual( PostgresErrorClass.TransactionRollback, exception.GetErrorClass() );
				Assert.Pass();
			}
			
			Assert.Fail( "Expected a transaction to be rolled back." );
		}
		
		[Test]
		public async Task DoubleCommit_ExpectObjectDisposedException() {
			PostgresCommand cmd = new PostgresCommand( "SELECT 1" );
			using( IPostgresTransaction transaction = await m_database.NewTransactionAsync().SafeAsync() ) {
				await transaction.ExecNonQueryAsync( cmd ).SafeAsync();
				await transaction.CommitAsync().SafeAsync();
				Assert.Throws<ObjectDisposedException>(
					async() => await transaction.CommitAsync().SafeAsync()
				);
			}
		}
		
		[Test]
		public async Task DoubleRollback_DoesNotThrowException() {
			PostgresCommand cmd = new PostgresCommand( "SELECT 1" );
			using( IPostgresTransaction transaction = await m_database.NewTransactionAsync().SafeAsync() ) {
				await transaction.ExecNonQueryAsync( cmd ).SafeAsync();
				await transaction.RollbackAsync().SafeAsync();
				await transaction.RollbackAsync().SafeAsync();
			}
		}
		
		[Test]
		public async Task ExecCommandAfterCommit_ExpectObjectDisposedException() {
			PostgresCommand cmd = new PostgresCommand( "SELECT 1" );
			using( IPostgresTransaction transaction = await m_database.NewTransactionAsync().SafeAsync() ) {
				await transaction.ExecNonQueryAsync( cmd ).SafeAsync();
				await transaction.CommitAsync().SafeAsync();
				Assert.Throws<ObjectDisposedException>(
					async() => await transaction.ExecNonQueryAsync( cmd ).SafeAsync()
				);
			}
		}
		
		[Test]
		public async Task ExecCommandAfterRollback_ExpectObjectDisposedException() {
			PostgresCommand cmd = new PostgresCommand( "SELECT 1" );
			using( IPostgresTransaction transaction = await m_database.NewTransactionAsync().SafeAsync() ) {
				await transaction.ExecNonQueryAsync( cmd ).SafeAsync();
				await transaction.RollbackAsync().SafeAsync();
				Assert.Throws<ObjectDisposedException>(
					async() => await transaction.ExecNonQueryAsync( cmd ).SafeAsync()
				);
			}
		}
		
		
		private async Task RunTransactionAsync( Guid initialId ) {
			PostgresCommand cmd;
			using( IPostgresTransaction transaction =
				await m_database.NewTransactionAsync( PostgresIsolationLevel.Serializable ).SafeAsync()
			) {
				cmd = new PostgresCommand( @"
					SELECT * FROM basic_table
						WHERE id = :id"
				);
				cmd.AddParameter<Guid>( "id", initialId );
				
				await transaction.ExecNonQueryAsync( cmd ).SafeAsync();
				
				await Task.Yield();
				await Task.Delay( TimeSpan.FromSeconds( 1.25 ) ).SafeAsync();
				
				cmd = new PostgresCommand( @"
					UPDATE basic_table
					SET id = :new_id
						WHERE id = :id"
				);
				cmd.AddParameter<Guid>( "id", initialId );
				cmd.AddParameter<Guid>( "new_id", Guid.NewGuid() );
				
				await transaction.ExecNonQueryAsync( cmd ).SafeAsync();
				
				await transaction.CommitAsync().SafeAsync();
			}
		}
		
	}
	
}
