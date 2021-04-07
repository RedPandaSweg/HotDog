using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using BackendService.DataObjects;
using BackendService.Models;
using Owin;

namespace BackendService
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            //For more information on Web API tracing, see http://go.microsoft.com/fwlink/?LinkId=620686 
            config.EnableSystemDiagnosticsTracing();

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);

            // Use Entity Framework Code First to create database tables based on your DbContext
            Database.SetInitializer(new ZUMOInitializer());

            MobileAppSettingsDictionary settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                // This middleware is intended to be used locally for debugging. By default, HostName will
                // only have a value when running in an App Service application.
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }
            app.UseWebApi(config);
        }
    }

    public class ZUMOInitializer : CreateDatabaseIfNotExists<Context>
    {
        protected override void Seed(Context context)
        {
            //set up db with standard ingredients
            List<Ingredient> ingredients = new List<Ingredient>
            {
                new Ingredient
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "Bun",
                    Name = "Wheat Bun",
                    Stock = 100,
                    Threshold = 20,
                    Price = new decimal(0.5),
                    Cost = new decimal(0.2),
                    PriceTag = "Wheat Bun (0,50 €)"
                },
                new Ingredient
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "Bun",
                    Name = "Glutenfree Bun",
                    Stock = 50,
                    Threshold = 10,
                    Price = new decimal(1),
                    Cost = new decimal(0.3),
                    PriceTag = "Glutenfree Bun (1,00 €)"
                },
                new Ingredient
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "Sausage",
                    Name = "Wiener",
                    Stock = 100,
                    Threshold = 20,
                    Price = new decimal(0.5),
                    Cost = new decimal(0.2),
                    PriceTag = "Wiener (0,50 €)"
                },
                new Ingredient
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "Sausage",
                    Name = "Vegan",
                    Stock = 50,
                    Threshold = 10,
                    Price = new decimal(1.0),
                    Cost = new decimal(0.5),
                    PriceTag = "Vegan (1,00 €)"
                },
                new Ingredient
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "Topping",
                    Name = "Roasted Onions",
                    Stock = 100,
                    Threshold = 20,
                    Price = new decimal(0.2),
                    Cost = new decimal(0.05),
                    PriceTag = "Roasted Onions (0,20 €)"
                },
                new Ingredient
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "Topping",
                    Name = "Diced Pickles",
                    Stock = 100,
                    Threshold = 20,
                    Price = new decimal(0.25),
                    Cost = new decimal(0.05),
                    PriceTag = "Diced Pickles (0,25 €)"
                },
                new Ingredient
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "Topping",
                    Name = "Sauerkraut",
                    Stock = 100,
                    Threshold = 20,
                    Price = new decimal(0.4),
                    Cost = new decimal(0.1),
                    PriceTag = "Sauerkraut (0,40 €)"
                },
                new Ingredient
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "Sauce",
                    Name = "Ketchup",
                    Stock = 100,
                    Threshold = 20,
                    Price = new decimal(0.2),
                    Cost = new decimal(0.05),
                    PriceTag = "Ketchup (0,20 €)"
                },
                new Ingredient
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "Sauce",
                    Name = "Mustard",
                    Stock = 100,
                    Threshold = 20,
                    Price = new decimal(0.2),
                    Cost = new decimal(0.05),
                    PriceTag = "Mustard (0,20 €)"
                },
                new Ingredient
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "Sauce",
                    Name = "Mayonaise",
                    Stock = 50,
                    Threshold = 10,
                    Price = new decimal(0.2),
                    Cost = new decimal(0.05),
                    PriceTag = "Mayonaise (0,20 €)"
                }
            };

            foreach (Ingredient ingredient in ingredients)
            {
                context.Set<Ingredient>().Add(ingredient);
            }

            base.Seed(context);
        }
    }
}

