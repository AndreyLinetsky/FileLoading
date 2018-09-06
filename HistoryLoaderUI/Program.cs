using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HistoryLoaderLogic;

namespace HistoryLoaderUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting loading history data");

            var manager = new HistoryLoaderManager();
            var result = manager.LoadInventoryHistory();

            Console.WriteLine(result.ToString());
        }
    }
}
