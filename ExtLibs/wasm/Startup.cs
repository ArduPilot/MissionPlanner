using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Blazor.Extensions.Storage;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Microsoft.AspNetCore.Components.Builder;
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

namespace wasm
{
    public class Startup
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddStorage();

            services.AddSpeechSynthesis();

            services.AddScoped<IIndexedDbFactory, IndexedDbFactory>();

            services.AddWorkerFactory();

            //services.Add(new ServiceDescriptor(typeof(IFileReaderService), typeof(FileReaderService), ServiceLifetime.Transient));

            var x = System.Runtime.CompilerServices.Unsafe.Unbox<int>(1);
            services.AddFileReaderService();

            // Add the library to the DI system
            services.AddToaster(config =>
            {
                //example customizations
                config.PositionClass = Defaults.Classes.Position.TopRight;
                config.PreventDuplicates = true;
                config.NewestOnTop = false;
            });

            //services.UseWebUSB(); // Makes IUSB available to the DI container
        }

        public class FakeRequest : WebRequest
        {
            private Uri _uri;

            public FakeRequest(Uri uri)
            {
                _uri = uri;
            }

            public override Uri RequestUri
            {
                get { return _uri; }
            }

            public override Stream GetRequestStream()
            {
                return base.GetRequestStream();
            }

            public override WebResponse GetResponse()
            {
                throw new Exception("Not ASYNC");
                return new FakeWebResponce(getStream().Result);
            }

            async Task<Stream> getStream()
            {
                return await new HttpClient().GetStreamAsync(RequestUri);
            }

            public override IWebProxy Proxy
            {
                get { return EmptyWebProxy.Instance; }
                set { }
            } 
        }

        public class FakeWebResponce: WebResponse
        {
            private readonly Stream _getResult;

            public FakeWebResponce(Stream getResult)
            {
                _getResult = getResult;
            }

            public override Stream GetResponseStream()
            {
                return _getResult;
            }
        }

        public class FakeRequestFactory : IWebRequestCreate
        {
            public WebRequest Create(Uri uri)
            {
                return new FakeRequest(uri);
            }
        }

//IBlazorApplicationBuilder
        //IComponentsApplicationBuilder
        public void Configure(IComponentsApplicationBuilder app)
        {
            bool result1 = WebRequest.RegisterPrefix("http://", new FakeRequestFactory());
            Console.WriteLine("webrequestmod " + result1);
            bool result2 = WebRequest.RegisterPrefix("https://", new FakeRequestFactory());
            Console.WriteLine("webrequestmod " + result2);

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
        }
    }
}