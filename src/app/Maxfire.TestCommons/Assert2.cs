using System;
using Xunit;
using Xunit.Sdk;

namespace Maxfire.TestCommons
{
	public static class Assert2
	{
		public static TException Throws<TException>(Assert.ThrowsDelegate testCode)
			where TException : Exception
		{
			Type exceptionType = typeof(TException);
			Exception exception = Record.Exception(testCode);

			if (exception == null)
				throw new ThrowsException(exceptionType);

			while (true)
			{
				if (exceptionType.Equals(exception.GetType()))
					break;

				if (exception.InnerException != null)
					exception = exception.InnerException;
				else
					throw new ThrowsException(exceptionType, exception);
			}
				
			return (TException)exception;
		}

		public static TException ThrowsInnerException<TException>(Assert.ThrowsDelegate testCode) 
			where TException : Exception
		{
			Type exceptionType = typeof (TException);
			Exception exception = Record.Exception(testCode);

			if (exception == null)
				throw new ThrowsException(exceptionType);

			while (exception.InnerException != null)
				exception = exception.InnerException;
			
			if (!exceptionType.Equals(exception.GetType()))
				throw new ThrowsException(exceptionType, exception);

			return (TException)exception;
		}
	}
}