using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mergen.Core.Data;
using Mergen.Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mergen.Core
{
    public static class ApplicationEvents
    {
        public static void ApplicationStart(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using (var serviceScope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
                if (bool.TryParse(configuration["Data:DropAndCreate"], out var dropAndCreate) && dropAndCreate)
                {
                    context.Database.EnsureDeleted();
                }

                context.Database.EnsureCreated();
                try
                {
                    bool.TryParse(configuration["Data:InMemory"], out var inMemory);
                    if (bool.TryParse(configuration["Data:Seed"], out var seed) && seed)
                    {
                        var dbSetType = typeof(DbSet<>);
                        var dbSets = typeof(DataContext).GetProperties().Where(t =>
                            t.PropertyType.IsGenericType &&
                            dbSetType.IsAssignableFrom(t.PropertyType.GetGenericTypeDefinition()));
                        foreach (var dbSet in dbSets)
                        {
                            var configSection = configuration.GetSection($"SeedData:{dbSet.Name}");
                            if (!configSection.GetChildren().Any())
                                continue;

                            var entityType = dbSet.PropertyType.GetGenericArguments()[0];
                            var entity = Activator.CreateInstance(typeof(List<>).MakeGenericType(entityType));
                            configSection.Bind(entity);
                            foreach (var ent in (IEnumerable) entity)
                            {
                                if (!inMemory && ent is Entity e)
                                    e.Id = 0;

                                context.Add(ent);
                            }
                        }

                        context.SaveChanges();
                    }

                    foreach (var setting in context.Settings)
                        configuration[setting.Key] = setting.Value;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw new Exception("SeedDataException");
                }
            }
        }
    }
}