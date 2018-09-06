using System;
using CsvHelper.Configuration;
using HistoryLoaderData;

namespace HistoryLoaderLogic.Mappers
{
    public sealed class SkuMapper : ClassMap<Sku>
    {
        public SkuMapper()
        {
            Map(m => m.Name).Name("SKU");
            Map(m => m.ID).Name("SKU");
        }
    }
}
