using System;
using CsvHelper.Configuration;
using HistoryLoaderData;

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
