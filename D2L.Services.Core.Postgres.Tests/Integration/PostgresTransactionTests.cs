
using D2L.Services.Core.TestFramework;
using Npgsql;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres.Tests.Integration {

	[TestFixture, Integration, RequiresDatabase]
	internal sealed class PostgresTransactionTests : IntegrationTestFixtureBase {
		
		[OneTimeSetUp, OneTimeTearDown]
		public void Cleanup() {
			PostgresCommand cmd = new PostgresCommand(
				"DELETE FROM basic_table"
			);
			m_database.ExecNonQueryAsync( cmd ).SafeWait();
		}
		
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
			IPostgresTransaction transaction = await m_database.NewTransactionAsync().SafeAsync();
			await using( transaction.Handle ) {
				await transaction.ExecNonQueryAsync( cmd ).SafeAsync();
				await transaction.CommitAsync().SafeAsync();
				Assert.ThrowsAsync<ObjectDisposedException>(
					async() => await transaction.CommitAsync().SafeAsync()
				);
			}
		}
		
		[Test]
		public async Task DoubleRollback_DoesNotThrowException() {
			PostgresCommand cmd = new PostgresCommand( "SELECT 1" );
			IPostgresTransaction transaction = await m_database.NewTransactionAsync().SafeAsync();
			await using( transaction.Handle ) {
				await transaction.ExecNonQueryAsync( cmd ).SafeAsync();
				await transaction.RollbackAsync().SafeAsync();
				await transaction.RollbackAsync().SafeAsync();
			}
		}
		
		[Test]
		public async Task CommitThenRollback_ExpectInvalidOperationException() {
			PostgresCommand cmd = new PostgresCommand( "SELECT 1" );
			IPostgresTransaction transaction = await m_database.NewTransactionAsync().SafeAsync();
			await using( transaction.Handle ) {
				await transaction.ExecNonQueryAsync( cmd ).SafeAsync();
				await transaction.CommitAsync().SafeAsync();
				Assert.ThrowsAsync<InvalidOperationException>(
					async() => await transaction.RollbackAsync().SafeAsync()
				);
			}
		}
		
		[Test]
		public async Task RollbackThenCommit_ExpectObjectDisposedException() {
			PostgresCommand cmd = new PostgresCommand( "SELECT 1" );
			IPostgresTransaction transaction = await m_database.NewTransactionAsync().SafeAsync();
			await using( transaction.Handle ) {
				await transaction.ExecNonQueryAsync( cmd ).SafeAsync();
				await transaction.RollbackAsync().SafeAsync();
				Assert.ThrowsAsync<ObjectDisposedException>(
					async() => await transaction.CommitAsync().SafeAsync()
				);
			}
		}
		
		[Test]
		public async Task ExecCommandAfterCommit_ExpectObjectDisposedException() {
			PostgresCommand cmd = new PostgresCommand( "SELECT 1" );
			IPostgresTransaction transaction = await m_database.NewTransactionAsync().SafeAsync();
			await using( transaction.Handle ) {
				await transaction.ExecNonQueryAsync( cmd ).SafeAsync();
				await transaction.CommitAsync().SafeAsync();
				Assert.ThrowsAsync<ObjectDisposedException>(
					async() => await transaction.ExecNonQueryAsync( cmd ).SafeAsync()
				);
			}
		}
		
		[Test]
		public async Task ExecCommandAfterRollback_ExpectObjectDisposedException() {
			PostgresCommand cmd = new PostgresCommand( "SELECT 1" );
			IPostgresTransaction transaction = await m_database.NewTransactionAsync().SafeAsync();
			await using( transaction.Handle ) {
				await transaction.ExecNonQueryAsync( cmd ).SafeAsync();
				await transaction.RollbackAsync().SafeAsync();
				Assert.ThrowsAsync<ObjectDisposedException>(
					async() => await transaction.ExecNonQueryAsync( cmd ).SafeAsync()
				);
			}
		}
		
		
		private async Task RunTransactionAsync( Guid initialId ) {
			PostgresCommand cmd;
			IPostgresTransaction transaction = await m_database.NewTransactionAsync( PostgresIsolationLevel.RepeatableRead ).SafeAsync();
			await using( transaction.Handle ) {
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
