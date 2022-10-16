using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using FH.ParcelLogistics.Services.Authentication;
using FH.ParcelLogistics.Services.Filters;
using FH.ParcelLogistics.Services.OpenApi;
using FH.ParcelLogistics.Services.Formatters;
using FH.ParcelLogistics.Services.MappingProfiles;
using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FH.ParcelLogistics.Services {
	/// <summary>
	/// Startup
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class Startup {
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="configuration"></param>
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		/// <summary>
		/// The application configuration.
		/// </summary>
		public IConfiguration Configuration { get; }

		/// <summary>
		/// This method gets called by the runtime. Use this method to add services to the container.
		/// </summary>
		/// <param name="services"></param>
		public void ConfigureServices(IServiceCollection services) {
			// AutoMapper
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<HelperProfile>();
				cfg.AddProfile<HopProfile>();
				cfg.AddProfile<ParcelProfile>();
			});
			var mapper = config.CreateMapper();
			services.AddSingleton(mapper);
			services.AddMvc(); 

			// Add framework services.
			services
				// Don't need the full MVC stack for an API, see https://andrewlock.net/comparing-startup-between-the-asp-net-core-3-templates/
				.AddControllers(options => { options.InputFormatters.Insert(0, new InputFormatterStream()); })
				.AddNewtonsoftJson(opts => {
					opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
					opts.SerializerSettings.Converters.Add(new StringEnumConverter {
						NamingStrategy = new CamelCaseNamingStrategy()
					});
				});

			services
				.AddSwaggerGen(c => {
					c.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);

					c.SwaggerDoc("1.22.1", new OpenApiInfo {
						Title = "Parcel Logistics Service",
						Description = "Parcel Logistics Service (ASP.NET Core 6.0)",
						TermsOfService = new Uri("https://github.com/openapitools/openapi-generator"),
						Contact = new OpenApiContact {
							Name = "SKS",
							Url = new Uri("http://www.technikum-wien.at/"),
							Email = ""
						},
						License = new OpenApiLicense {
							Name = "NoLicense",
							Url = new Uri("http://localhost")
						},
						Version = "1.22.1",
					});
					c.CustomSchemaIds(type => type.FriendlyId(true));
					c.IncludeXmlComments(
						$"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetEntryAssembly().GetName().Name}.xml");

					// Include DataAnnotation attributes on Controller Action parameters as OpenAPI validation rules (e.g required, pattern, ..)
					// Use [ValidateModelState] on Actions to actually validate it in C# as well!
					c.OperationFilter<GeneratePathParamsValidationFilter>();
				});
			services
				.AddSwaggerGenNewtonsoftSupport();
		}

		/// <summary>
		/// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		/// </summary>
		/// <param name="app"></param>
		/// <param name="env"></param>
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			} else {
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseDefaultFiles();
			app.UseStaticFiles();
			app.UseSwagger(c => { c.RouteTemplate = "openapi/{documentName}/openapi.json"; })
				.UseSwaggerUI(c => {
					// set route prefix to openapi, e.g. http://localhost:8080/openapi/index.html
					c.RoutePrefix = "openapi";
					//TODO: Either use the SwaggerGen generated OpenAPI contract (generated from C# classes)
					c.SwaggerEndpoint("/openapi/1.22.1/openapi.json", "Parcel Logistics Service");

					//TODO: Or alternatively use the original OpenAPI contract that's included in the static files
					// c.SwaggerEndpoint("/openapi-original.json", "Parcel Logistics Service Original");
				});
			app.UseRouting();
			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}
	}
}