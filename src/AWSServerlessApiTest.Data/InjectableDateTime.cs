using System;

namespace AWSServerlessApiTest.Data
{
	public interface IInjectableDateTime
	{
		DateTime Now { get; }
		DateTime UtcNow { get; }
	}

	public class InjectableDateTime : IInjectableDateTime
	{
		public DateTime Now => DateTime.Now;
		public DateTime UtcNow => DateTime.UtcNow;
	}
}
