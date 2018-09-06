using System;
using CsvHelper.Configuration;
using HistoryLoaderData;

namespace HistoryLoaderLogic.Mappers
{
    public sealed class StoreMapper : ClassMap<Store>
    {
        public StoreMapper()
        {
            Map(m => m.Name).Name("Store");
            Map(m => m.ID).Name("Store");
        }
    }
}
