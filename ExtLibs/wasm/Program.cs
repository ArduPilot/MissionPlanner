
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.IndexedDB.Framework;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Sotsera.Blazor.Toaster.Core.Models;
using Tewr.Blazor.FileReader;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.Tasks;
using Blazor.IndexedDB.Framework;
using BlazorWorker.Core;
using GMap.NET.MapProviders;
using MissionPlanner.ArduPilot;
using MissionPlanner.Utilities;
using Sotsera.Blazor.Toaster.Core.Models;
using Tewr.Blazor.FileReader;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Blazor.Extensions.Storage;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Zeroconf;
using Blazor.Extensions.WebUSB;
using System.Threading;

namespace wasm
{

    public class FirstMiddleware : DelegatingHandler
    {
        public Task<byte[]> GetByteArrayAsync(string requestUri) 
        {
            return null;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Interfering code before sending the request
            var response = await base.SendAsync(request, cancellationToken);
            // Interfering code after sending the request

            return response;
        }
    }

    public class Program
    {
        private static log4net.ILog log;


        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient
                {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});

            builder.Services.AddTransient<FirstMiddleware>();

            builder.Services.AddHttpClient();

            builder.Services.AddFileReaderService(options => options.UseWasmSharedBuffer = true);

            builder.Services.AddStorage();

            builder.Services.AddSpeechSynthesis();

            builder.Services.AddScoped<IIndexedDbFactory, IndexedDbFactory>();

            builder.Services.AddWorkerFactory();

            builder.Services.AddToaster(config =>
            {
                //example customizations
                config.PositionClass = Defaults.Classes.Position.TopRight;
                config.PreventDuplicates = true;
                config.NewestOnTop = true;
            });

            builder.Services.UseWebUSB(); // Makes IUSB available to the DI container

            {

                //bool result1 = WebRequest.RegisterPrefix("http://", new Startup.FakeRequestFactory());
                //Console.WriteLine("webrequestmod " + result1);
                //bool result2 = WebRequest.RegisterPrefix("https://", new Startup.FakeRequestFactory());
                //Console.WriteLine("webrequestmod " + result2);



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

                log =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


                log.Info("test");

                log.Info("Configure Done");

                //System.Net.WebRequest.get_InternalDefaultWebProxy

                //WebRequest.DefaultWebProxy = GlobalProxySelection.GetEmptyWebProxy();

                Directory.CreateDirectory(@"/home/web_user/Desktop");

                BinaryLog.onFlightMode += (firmware, modeno) =>
                {
                    try
                    {
                        if (firmware == "")
                            return null;

                        var modes = Common.getModesList((Firmwares) Enum.Parse(typeof(Firmwares), firmware));
                        string currentmode = null;

                        foreach (var mode in modes)
                        {
                            if (mode.Key == modeno)
                            {
                                currentmode = mode.Value;
                                break;
                            }
                        }

                        return currentmode;
                    }
                    catch
                    {
                        return null;
                    }
                };

                CustomMessageBox.ShowEvent += (text, caption, buttons, icon, yestext, notext) =>
                {
                    Console.WriteLine("CustomMessageBox " + caption + " " + text);


                    return CustomMessageBox.DialogResult.OK;
                };

                MissionPlanner.Utilities.Download.RequestModification += Download_RequestModification;
            }

            var app = builder.Build();
            await app.RunAsync();
        }

        private static void Download_RequestModification(object sender, HttpRequestMessage e)
        {
            Console.WriteLine("Download_RequestModification Set No-Cors");
            e.SetBrowserRequestMode(BrowserRequestMode.NoCors);
        }
    }



    // Represents the database
        public class ExampleDb : IndexedDb
        {
            public ExampleDb(IJSRuntime jSRuntime, string name, int version) : base(jSRuntime, name, version) { }

            // These are like tables. Declare as many of them as you want.
            public IndexedSet<Person> People { get; set; }
        }

        public class Person
        {
            [Key]
            public long Id { get; set; }

            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }
        }

}
