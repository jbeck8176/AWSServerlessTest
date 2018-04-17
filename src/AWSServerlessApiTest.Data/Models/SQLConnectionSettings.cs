using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace AWSServerlessApiTest.Data.Models
{
	public interface ISQLConnectionSettings
	{
		string SqlServerUrl { get; set; }
		string MasterDbName { get; set; }
		string SqlUserName { get; set; }
		string SqlUserPassword { get; set; }
		string ProdTestPostfix { get; set; }
		string BuildConnectionString();
		string BuildConnectionString(string clientDatabaseName);
	}

	public class SQLConnectionSettings : ISQLConnectionSettings
	{
		public string SqlServerUrl { get; set; }
		public string MasterDbName { get; set; }
		public string SqlUserName { get; set; }
		public string SqlUserPassword { get; set; }
		public string ProdTestPostfix { get; set; }

		public string BuildConnectionString()
		{
			return BuildConnectionString(this.MasterDbName);
		}

		public string BuildConnectionString(string clientDatabaseName)
		{
			SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
			{
				DataSource = SqlServerUrl
			};

			builder.InitialCatalog = string.IsNullOrEmpty(ProdTestPostfix) ? clientDatabaseName : clientDatabaseName + ProdTestPostfix;
			/* dont use username and pass for demo purposes */
			//builder.UserID = this.SqlUserName;
			//builder.Password = this.SqlUserPassword;

			//use integrated security instead
			builder.IntegratedSecurity = true;
			
			return builder.ConnectionString;
		}
	}
}
