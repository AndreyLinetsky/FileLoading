using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
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
