using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Blazor.Extensions.Storage;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

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
    }
}