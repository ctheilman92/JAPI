using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Threading.Tasks;

namespace JAPI.PollR
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string url = "http://localhost:4444";
                using (WebApp.Start(url))
                {
                    Console.WriteLine("Server running on {0}", url);
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: { ex.Message}");
            }
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR("/jpollr", new HubConfiguration { EnableDetailedErrors = true, });
        }
    }
}
