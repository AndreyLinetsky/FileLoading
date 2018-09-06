using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using CsvHelper.Configuration;
using HistoryLoaderData;
using HistoryLoaderLogic.FileRows;

namespace HistoryLoaderLogic.Mappers
{
    public sealed class InventoryHistoryMapper : ClassMap<InventoryHistory>
    {
        public InventoryHistoryMapper()
        {
            Map(m => m.Date).Name("Date");
            Map(m => m.Inv).Name("Inv");
            Map(m => m.StoreID).Name("Store");
            Map(m => m.SKUID).Name("SKU");
        }
    }
}
