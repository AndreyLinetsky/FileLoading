using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace HistoryLoaderData
{
    public class LoaderContext : DbContext
    {
        public LoaderContext()
            : base("name=LoaderContext")
        {
        }

        public virtual DbSet<Sku> Skus { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
    }
}
