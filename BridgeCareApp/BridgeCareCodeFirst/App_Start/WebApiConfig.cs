﻿using BridgeCare.App_Start;
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
using static BridgeCare.Models.BudgetReportData;

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
            container.RegisterType<INetwork, NetworkRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ISimulation, SimulationRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IBudgetReportData, BudgetReportData>(new HierarchicalLifetimeManager());
            container.RegisterType<ICostDetails, CostDetails>(new HierarchicalLifetimeManager());
            container.RegisterType<IDetailedReport, DetailedReportRepository>(new HierarchicalLifetimeManager());
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
