using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FH.ParcelLogistics.Services {
	/// <summary>
	/// Program
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class Program {
		/// <summary>
		/// Main
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args) {
			CreateHostBuilder(args).Build().Run();
		}

		/// <summary>
		/// Create the host builder.
		/// </summary>
		/// <param name="args"></param>
		/// <returns>IHostBuilder</returns>
		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder => {
					webBuilder.UseStartup<Startup>()
						.UseUrls("http://0.0.0.0:8080/");
				});
	}
}