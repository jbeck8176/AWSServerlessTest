using System;
using AWSServerlessApiTest.Data.Models;
using AWSServerlessApiTest.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AWSServerlessApiTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
