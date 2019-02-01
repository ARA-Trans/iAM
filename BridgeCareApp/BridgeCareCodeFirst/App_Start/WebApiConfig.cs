using BridgeCare.App_Start;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Services;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using Unity;
using Unity.Lifetime;
using BridgeCare.Data;

namespace BridgeCare
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Enable CORS for the Vue App
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            var container = new UnityContainer();
            var dbContext = new BridgeCareContext();
            dbContext.Configuration.ProxyCreationEnabled = false;
            container.RegisterType<INetwork, Network>(new HierarchicalLifetimeManager());
            container.RegisterType<ISimulation, Simulations>(new HierarchicalLifetimeManager());
            container.RegisterType<IDetailedReport, DetailedReport>(new HierarchicalLifetimeManager());
            container.RegisterType<BridgeCareContext>(new HierarchicalLifetimeManager());
            container.RegisterType<IBudgetReport, BudgetReport>(new HierarchicalLifetimeManager());
            container.RegisterType<IDeficient, Deficient>(new HierarchicalLifetimeManager());
            container.RegisterType<CostDetails>(new HierarchicalLifetimeManager());
            container.RegisterType<ITarget, Targets>(new HierarchicalLifetimeManager());
            container.RegisterType<IReportCreator, ReportCreator>(new HierarchicalLifetimeManager());
            container.RegisterType<FillDetailedSheet>();
            container.RegisterType<TargetsMet>();
            container.RegisterType<TargetResults>();
            container.RegisterType<TargetReport>();
            container.RegisterType<DeficientReport>();
            container.RegisterType<FillDetailedReport>();
            container.RegisterType<Budget>();
            container.RegisterType<CellAddress>();
            config.DependencyResolver = new UnityResolver(container);

            // Set JSON formatter as default one and remove XmlFormatter

            var jsonFormatter = config.Formatters.JsonFormatter;

            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Remove the XML formatter
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            jsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
        }
    }
}
