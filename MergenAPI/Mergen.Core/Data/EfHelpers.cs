using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Core.Data
{
    public static class EfHelpers
    {
        public static void DisableCascadeDeletes(ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}