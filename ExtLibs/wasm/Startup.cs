using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Blazor.FileReader;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace wasm
{
    public class Startup
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ConfigureServices(IServiceCollection services)
        {
            services.Add(new ServiceDescriptor(typeof(IFileReaderService), typeof(FileReaderService), ServiceLifetime.Transient));
        }

        public async void Configure(IBlazorApplicationBuilder app)
        {
            app.AddComponent<App>("app");

            log4net.Repository.Hierarchy.Hierarchy hierarchy =
                (Hierarchy) log4net.LogManager.GetRepository(Assembly.GetAssembly(typeof(Startup)));

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
            patternLayout.ActivateOptions();

            var cca = new ConsoleAppender();
            cca.Layout = patternLayout;
            cca.ActivateOptions();
            hierarchy.Root.AddAppender(cca);

            hierarchy.Root.Level = Level.Debug;
            hierarchy.Configured = true;

            log.Info("test");

            log.Info("Configure Done");
        }
    }
}
