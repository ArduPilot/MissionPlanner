
using System.ComponentModel.DataAnnotations;
using Blazor.IndexedDB.Framework;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.JSInterop;

namespace wasm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args)
        {
            return BlazorWebAssemblyHost.CreateDefaultBuilder()
                .UseBlazorStartup<Startup>();
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
