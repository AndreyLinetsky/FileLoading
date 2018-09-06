using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration;

namespace HistoryLoaderLogic.FileRows
{
    public class InventoryHistoryFileRow
    {
        public string Store { get; set; }
        public string SKU { get; set; }
        public DateTime Date { get; set; }
        public int Inv { get; set; }
    }
}
