﻿using BridgeCare.App_Start;
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
            dbContext.Configuration.LazyLoadingEnabled = false;
            container.RegisterType<INetwork, NetworkRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ISimulation, SimulationRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IDetailedReport, DetailedReport>(new HierarchicalLifetimeManager());
            container.RegisterType<BridgeCareContext>(new HierarchicalLifetimeManager());
            container.RegisterType<IBudget, BudgetReportData>(new HierarchicalLifetimeManager());
            container.RegisterType<IDeficient, DeficientData>(new HierarchicalLifetimeManager());
            container.RegisterType<CostDetails>(new HierarchicalLifetimeManager());
            container.RegisterType<ITarget, TargetData>(new HierarchicalLifetimeManager());
            container.RegisterType<IReportCreater, DetailedReportRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<FillDetailedSheet>();
            container.RegisterType<DeficientOrTarget>();
            container.RegisterType<TargetResults>();
            container.RegisterType<FillTargets>();
            container.RegisterType<FillDeficients>();
            container.RegisterType<FillDetailedReport>();
            container.RegisterType<FillBudget>();
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
