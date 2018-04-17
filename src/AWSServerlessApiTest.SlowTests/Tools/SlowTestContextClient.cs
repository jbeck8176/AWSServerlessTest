using System;
using AWSServerlessApiTest.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AWSServerlessApiTest.SlowTests.Tools
{
	public sealed class SlowTestContextClient : DbContext
	{
		public static SQLConnectionSettings SlowTestSqlConnectionSettings => new SQLConnectionSettings
		{
			SqlServerUrl = @"localhost\sqlexpress",
			MasterDbName = "IntegrationTestClient"
		};

		/// <summary>
		/// Do not initialize using the constructor. Access in test via SlowTestContextFixture. If you are having cached reads and want a new context try .AsNoTracking() first.
		/// </summary>
		/// <param name="options"></param>
		/// 
		[Obsolete(" Do not initialize using the constructor. Access in test via SlowTestContextFixture.")]
		public SlowTestContextClient(DbContextOptions<SlowTestContextClient> options) : base(options)
		{
			//the first time init the context delete the DB
			Database.EnsureDeleted();
			//then re create the DB to make sure out tables are accurate
			Database.EnsureCreated();
		}

		//public DbSet<> ClientOrganizationInfos { get; set; }
		
		public void ResetData()
		{
			//this.ClientOrganizationInfos.RemoveRange(this.ClientOrganizationInfos);
			this.SaveChanges();
		}

	}
}
