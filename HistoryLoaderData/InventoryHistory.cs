using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace HistoryLoaderData
{
    public class InventoryHistory
    {
        public string StoreID { get; set; }

        public string SKUID { get; set; }

        public DateTime Date { get; set; }

        public int Inv { get; set; }

    }
}
