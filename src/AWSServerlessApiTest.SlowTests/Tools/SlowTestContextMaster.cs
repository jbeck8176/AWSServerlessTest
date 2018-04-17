using System;
using System.Collections.Generic;
using System.Text;
using AWSServerlessApiTest.Data.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AWSServerlessApiTest.SlowTests.Tools
{
	public sealed class SlowTestContextMaster : DbContext
	{
		public static SQLConnectionSettings SlowTestSqlConnectionSettings => new SQLConnectionSettings
		{
			SqlServerUrl = @"localhost\sqlexpress",
			MasterDbName = "IntegrationTestMaster"
		};

		/// <summary>
		/// Do not initialize using the constructor. Access in test via SlowTestContextFixture. If you are having cached reads and want a new context try .AsNoTracking() first.
		/// </summary>
		/// <param name="options"></param>
		/// 
		[Obsolete(" Do not initialize using the constructor. Access in test via SlowTestContextFixture.")]
		public SlowTestContextMaster(DbContextOptions<SlowTestContextMaster> options) : base(options)
		{
			//the first time init the context delete the DB
			Database.EnsureDeleted();
			//then re create the DB to make sure out tables are accurate
			Database.EnsureCreated();
		}

		public DbSet<Widget> Widgets { get; set; }
	
		public void ResetData()
		{
			this.Widgets.RemoveRange(this.Widgets);
			this.SaveChanges();
		}

	}
}
