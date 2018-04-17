using System.Data;
using System.Data.SqlClient;
using AWSServerlessApiTest.Data.Models;
using Dapper;

namespace AWSServerlessApiTest.Data.Repositories
{
	public interface IWidgetRepository
	{
		string GetWidgetNameById(int widgetId);
	}

	public class WidgetRepository : IWidgetRepository
	{
	    private readonly ISQLConnectionSettings _sqlConnectionSettings;
	    private IDbConnection ClientDbConnection => new SqlConnection(_sqlConnectionSettings.BuildConnectionString());


	    public WidgetRepository(ISQLConnectionSettings sqlConnectionSettings)
	    {
		    _sqlConnectionSettings = sqlConnectionSettings;
	    }

		public string GetWidgetNameById(int widgetId)
	    {
		    var sqlStatment = @"SELECT TOP 1 [WidgetName] FROM [Custom_Widgets]
								WHERE [Id] = @widgetId";

		    using (var connection = ClientDbConnection)
		    {
			    return connection.QueryFirstOrDefault<string>(sqlStatment, new { widgetId });
		    }
		}

    }
}
