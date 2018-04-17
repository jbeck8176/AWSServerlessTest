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

### Build and Deploy ###

Once the AWS Api Gateway and Lambda is setup you will then have to alter a few of the deployment scripts. All scripts can be found in /build

### Build Step 1 (01_Restore.cmd): ###
project name agnostic, should never have to change.
### Build Step 2 (02_Build.cmd): ###
project name agnostic, should never have to change.
### Build Step 3 (03_UnitTest.cmd): ###
This is dependant on the name of the unit test project.
```
dotnet test ..\src\[FOLDER OF UNIT TEST PROJECT]\
```
### Build Step 4 (04_SlowTest.cmd): ###
This is dependant on the name of the slow test project.
```
dotnet test ..\src\[FOLDER OF SLOW TEST PROJECT]\
```
### Build Step 5 (05_Package.cmd): ###
This is dependant on the name of the api project.
```
cd ..\src\[FOLDER OF THE API PROJECT]\
dotnet lambda package
```
### Build Step 6 (06_Deploy.cmd): ###
This is dependant on the name of the api project, name if the AWS Lambda name, and the framework version.
```
cd ..\src\[FOLDER OF THE API PROJECT]\
dotnet lambda deploy-function [NAME OF LAMBDA FUNCTION] -pac .\bin\Release\[FRAMEWORK]\[NAME OF API PROJECT].zip
```
