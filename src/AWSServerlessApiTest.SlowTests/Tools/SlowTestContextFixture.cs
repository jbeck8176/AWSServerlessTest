using System;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AWSServerlessApiTest.SlowTests.Tools
{
	public class SlowTestContextFixture : IDisposable
	{
		public SlowTestContextClient ClientContext;
		public SlowTestContextMaster MasterContext;
		public SlowTestContextFixture()
		{
			var clientOptions = new DbContextOptionsBuilder<SlowTestContextClient>().UseSqlServer(SlowTestContextClient.SlowTestSqlConnectionSettings.BuildConnectionString()).Options;
			ClientContext = new SlowTestContextClient(clientOptions);

			var masterOptions = new DbContextOptionsBuilder<SlowTestContextMaster>().UseSqlServer(SlowTestContextMaster.SlowTestSqlConnectionSettings.BuildConnectionString()).Options;
			MasterContext = new SlowTestContextMaster(masterOptions);
		}

		public void Dispose()
		{
			ClientContext.Dispose();
			MasterContext.Dispose();
		}
	}

	[CollectionDefinition("SlowTestContext")]
	public class SlowTestContextCollection : ICollectionFixture<SlowTestContextFixture>
	{
		// This class has no code, and is never created. Its purpose is simply
		// to be the place to apply [CollectionDefinition] and all the
		// ICollectionFixture<> interfaces.
	}
}
