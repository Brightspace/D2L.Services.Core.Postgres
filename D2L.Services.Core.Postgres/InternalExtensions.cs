using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace D2L.Services.Core.Postgres {
	
	internal static class InternalExtensions {
		
		internal static void SafeDispose( this IDisposable disposable ) {
			if( disposable != null ) {
				disposable.Dispose();
			}
		}
		
		internal static void SafeDispose(
			this IDisposable disposable,
			ref Exception currentException
		) {
			if( disposable == null ) {
				return;
			}
			
			try {
				disposable.Dispose();
			} catch( Exception newException ) {
				if( currentException is AggregateException ) {
					List<Exception> exceptions = new List<Exception>();
					exceptions.AddRange( ((AggregateException)currentException).InnerExceptions );
					exceptions.Add( newException );
					currentException = new AggregateException( exceptions );
				} else {
					currentException = new AggregateException(
						currentException,
						newException
					);
				}
			}
		}
		
		internal static ConfiguredTaskAwaitable SafeAsync( this Task task ) {
			return task.ConfigureAwait( continueOnCapturedContext: false );
		}
		
		internal static ConfiguredTaskAwaitable<T> SafeAsync<T>( this Task<T> task ) {
			return task.ConfigureAwait( continueOnCapturedContext: false );
		}
		
	}
	
}
