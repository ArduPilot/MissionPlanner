using System.Threading;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.JSInterop;
using Mono.WebAssembly.Interop;

namespace wasm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            //JSRuntime.SetCurrentJSRuntime(new MonoWebAssemblyJSRuntime());

            //JSRuntime.Current.InvokeAsync<object>("initMap", null);
        }

        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) =>
            BlazorWebAssemblyHost.CreateDefaultBuilder()
                .UseBlazorStartup<Startup>();
    }
}
