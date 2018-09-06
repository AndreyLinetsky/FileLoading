using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
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
