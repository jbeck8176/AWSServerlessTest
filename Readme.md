# ASP.NET Core Web API Serverless Application

This project shows how to run an ASP.NET Core Web API project as an AWS Lambda exposed through Amazon API Gateway. The NuGet package [Amazon.Lambda.AspNetCoreServer](https://www.nuget.org/packages/Amazon.Lambda.AspNetCoreServer) contains a Lambda function that is used to translate requests from API Gateway into the ASP.NET Core framework and then the responses from ASP.NET Core back to API Gateway.

This project was started with the intention of providing a example and POC for migrating siloable API actions away from our current RenovoLive2 API backend and over to a AWS Lambda based .NET Core 2 Web API. 


### Configuring AWS SDK for .NET ###

To integrate the AWS SDK for .NET with the dependency injection system built into ASP.NET Core the NuGet package [AWSSDK.Extensions.NETCore.Setup](https://www.nuget.org/packages/AWSSDK.Extensions.NETCore.Setup/) is referenced. In the Startup.cs file the Amazon S3 client is added to the dependency injection framework. The S3ProxyController will get its S3 service client from there.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();

    // Add S3 to the ASP.NET Core dependency injection framework.
    services.AddAWSService<Amazon.S3.IAmazonS3>();
}
```


### Other dependancy setup ###
To make the sql connection work with our client and master databases pull the serialized sql connection settings from the environment variables.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();

    var sqlConnectionSettingsString = Environment.GetEnvironmentVariable("SQLConnectionSettings");

    var sqlConnectionSettings = new SQLConnectionSettings();

    if (!string.IsNullOrEmpty(sqlConnectionSettingsString))
    {
        sqlConnectionSettings = JsonConvert.DeserializeObject<SQLConnectionSettings>(sqlConnectionSettingsString);
    }

    services.AddSingleton<ISQLConnectionSettings>(sqlConnectionSettings);
    services.AddTransient<IWidgetRepository, WidgetRepository>();
}
```

### Project Files ###

* aws-lambda-tools-defaults.json - Default argument settings for use with Visual Studio and command line deployment tools for AWS. Always leave profile blank. The configuration should always be "Release" and the framework should match the project and the setting in the AWS console. There are other settings, but they are set and shouldn't be changed.
* LambdaEntryPoint.cs - class that derives from **Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction**. The code in this file bootstraps the ASP.NET Core hosting framework. The Lambda function is defined in the base class.
* LocalEntryPoint.cs - for local development this contains the executable Main function which bootstraps the ASP.NET Core hosting framework with Kestrel, as for typical ASP.NET Core applications.
* Startup.cs - usual ASP.NET Core Startup class used to configure the services ASP.NET Core will use.
* web.config - used for local development.
* Controllers\ValuesController - example Web API controller
* Controllers\WidgetController - example Web API controller

### Slow Test Tools ###
Since this particular project uses Dapper as its primary ORM we needed something a bit more sophisticated to generate the slow test data base for us. For this we leverage `Microsoft.EntityFrameworkCore.SqlServer` and use its tools to delete and generate slow test databases.

To register an object to have a table generated for it properties will need added to `SlowTestContextMaster` or `SlowTestContextMaster`.

```csharp
public sealed class SlowTestContextMaster : DbContext
{
    public SlowTestContextMaster(DbContextOptions<SlowTestContextMaster> options) : base(options)
    {
        // The first time init the context delete the DB
        Database.EnsureDeleted();
        // Then re create the DB to make sure out tables are accurate
        Database.EnsureCreated();
    }

    // Add property here for EF to generate a table for it. By default table name will reflect this property name
    public DbSet<Widget> Widgets { get; set; }

    public void ResetData()
    {
        // An entry also has to be added here for each property like below. this is what clears the slow test data before each test.
        this.Widgets.RemoveRange(this.Widgets);
        this.SaveChanges();
    }
}
```

If you need the table named something other than what you want the EF entity named you can do so on the Model by adding annotations.
```csharp
// EF will generate the SQL table with this name and handle the mapping
[Table("Custom_Widgets")]
public class Widget
{
    // Every table has to have a primary key for EF. The DatabaseGenerated annotation allows us to generate Ids from AutoFixture and insert them into the slow test database.
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    // EF will normally name the column in SQL to match the propery name. To change the SQL column name use the column annotation.
    [Column("WidgetName")]
    public string Name { get; set; }
}
```

Now that we have our contexts set up we need to set up how xUnit will use them. Since xUnit initializes each test in its own context sharing a static instance won't work. To ensure all of the tests get the same db context a fixture decorated with a `CollectionDefinition` annotation should be used.
```csharp
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
```

To provide access to the contexts the test class needs decorated with a `Collection` annotation. Adding that annotation tells xUnit to "inject" that dependancy into the constructor.
```csharp
[Collection("SlowTestContext")]
public class WidgetRepositoryTest
{
    private readonly SlowTestContextMaster _slowTestContext;
    private readonly IFixture _fixture;

    private readonly IWidgetRepository _widgetRepository;

    public WidgetRepositoryTest(SlowTestContextFixture slowTestContextFixture)
    {
        _fixture = new Fixture();
        _slowTestContext = slowTestContextFixture.MasterContext;
        _slowTestContext.ResetData();

        _widgetRepository = new WidgetRepository(SlowTestContextMaster.SlowTestSqlConnectionSettings);
    }

    [Fact]
    public void GetWidgetName_should_get_widget_name()
    {
        var fakeWidget = _fixture.Create<Widget>();
        _slowTestContext.Widgets.Add(fakeWidget);
        _slowTestContext.SaveChanges();

        var result = _widgetRepository.GetWidgetNameById(fakeWidget.Id);

        Assert.Equal(fakeWidget.Name, result);
    }
}
```

### Build and Deploy ###

Once the AWS Api Gateway and Lambda is setup you will then have to alter a few of the deployment scripts. All scripts can be found in /build

1. **Build Step 1 (01_Restore.cmd):**
    * project name agnostic, should never have to change.
2. **Build Step 2 (02_Build.cmd):**
    * project name agnostic, should never have to change.
3. **Build Step 3 (03_UnitTest.cmd):**
    * This is dependant on the name of the unit test project.
    ```
    dotnet test ..\src\[FOLDER OF UNIT TEST PROJECT]\
    ```
4. **Build Step 4 (04_SlowTest.cmd):**
    * This is dependant on the name of the slow test project.
    ```
    dotnet test ..\src\[FOLDER OF SLOW TEST PROJECT]\
    ```
5. **Build Step 5 (05_Package.cmd):**
    * This is dependant on the name of the api project.
    ```
    cd ..\src\[FOLDER OF THE API PROJECT]\
    dotnet lambda package
    ```
6. **Build Step 6 (06_Deploy.cmd):**
    * This is dependant on the name of the api project, name if the AWS Lambda name, and the framework version.
    ```
    cd ..\src\[FOLDER OF THE API PROJECT]\
    dotnet lambda deploy-function [NAME OF LAMBDA FUNCTION] -pac .\bin\Release\[FRAMEWORK]\[NAME OF API PROJECT].zip
    ```
