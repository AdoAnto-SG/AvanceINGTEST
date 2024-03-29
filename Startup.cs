﻿using FindexMapper.Service.Common;
using FindexMapper.Service.Data;
using FindexMapper.Service.Data.Entities;
using FindexMapper.Service.Data.Interfaces;
using FindexMapper.Service.Data.Repositories;
using FindexMapper.Service.Interfaces;
using FindexMapper.Service.Services.ControlNumbers;
using Integrador.Converters;
using Integrador.Mappers.Cancellations;
using Integrador.Mappers.DividedInvoices;
using Integrador.Mappers.Export;
using Integrador.Mappers.FromApi;
using Integrador.Mappers.Invoices;
using Integrador.Mappers.TaxCredits;
using Integrador.Models;
using Integrador.Services;

namespace Integrador
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add serices to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());

            services.Configure<SenderInfo>(
                Configuration.GetSection(nameof(SenderInfo)));

            services.AddScoped<IParseService, ParseService>();
            services.AddScoped<IInvalidationParseService, InvalidationService>();
            services.AddScoped<IInvoiceMapper, InvoiceMapper>();
            services.AddScoped<ITaxCreditMapper, TaxCreditMapper>();
            services.AddScoped<IDividedInvoiceMapper, DividedInvoiceMapper>();
            services.AddScoped<IInvoiceFromApiMapper, InvoiceFromApiMapper>();
            services.AddScoped<IExportMapper, ExportMapper>();
            services.AddScoped<IExcludedSubjectMapper, ExcludedSubjectMapper>();
            services.AddScoped<ICancellationMapper, CancellationMapper>();
            services.AddScoped<IControlNumberService, ControlNumberService>();
            services.AddScoped<IMySqlDatabaseProvider, MySqlDatabaseProvider>();
            services.AddScoped<ISourceProvider, SourceProvider>();
            services.AddScoped<IRepository<Catalog>, CatalogRepository>();
            services.AddScoped<IRepository<CatalogItem>, CatalogItemRepository>();
            services.AddScoped<ISqLiteDatabaseProvider, SqLiteDatabaseProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
