using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace D2L.Services.Core.Postgres {
	
	[EditorBrowsable( EditorBrowsableState.Never )]
	internal static class IDisposable_Extensions {
		
		internal static void SafeDispose(
			this IDisposable disposable,
			ref Exception currentException
		) {
			try {
				disposable.SafeDispose();
			} catch( Exception newException ) {
				if( currentException == null ) {
					currentException = newException;
				} else if( currentException is AggregateException ) {
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
		
	}
	
}
