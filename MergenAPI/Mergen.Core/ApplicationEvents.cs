using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mergen.Core.Data;
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
                    if (bool.TryParse(configuration["Data:InMemory"], out var inMemory) && inMemory)
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
                                context.Add(ent);
                            }
                        }

                        context.SaveChanges();
                    }
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