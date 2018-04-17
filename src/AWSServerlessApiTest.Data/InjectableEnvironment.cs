using System;
using System.Collections.Generic;
using System.Text;

namespace AWSServerlessApiTest.Data
{
	/// <summary>
	/// This interface is to wrap the environment object so it can be injected for test mocking
	/// </summary>
	public interface IInjectableEnvironment
	{
		string GetEnvironmentVariable(string variable);
		string GetEnvironmentVariable(string variable, EnvironmentVariableTarget target);
	}

	/// <summary>
	/// Implementation of the IInjectableEnvironment interface. Simply wraps the .net environment object.
	/// </summary>
	public class InjectableEnvironment : IInjectableEnvironment
	{
		public string GetEnvironmentVariable(string variable)
		{
			return Environment.GetEnvironmentVariable(variable);
		}

		public string GetEnvironmentVariable(string variable, EnvironmentVariableTarget target)
		{
			return Environment.GetEnvironmentVariable(variable, target);
		}
	}
}
