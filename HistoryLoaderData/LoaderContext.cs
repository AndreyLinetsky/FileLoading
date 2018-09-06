using System;
using System.Data.Entity;

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
