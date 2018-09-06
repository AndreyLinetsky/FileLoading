using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using BenGribaudoLLC.IEnumerableHelpers.DataReaderAdapter;
using CsvHelper;
using CsvHelper.Configuration;
using HistoryLoaderData;
using HistoryLoaderLogic.Extensions;
using HistoryLoaderLogic.FileRows;
using HistoryLoaderLogic.Mappers;

namespace HistoryLoaderLogic
{
    public class HistoryLoaderManager
    {
        public StringBuilder LoadInventoryHistory()
        {
            var result = new StringBuilder();

            using (var context = new LoaderContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var storeFileName = ConfigurationManager.AppSettings["storeFile"];
                        var storeRows = ReadFile<Store, StoreMapper>(storeFileName);
                        InsertStoreData(context, storeRows);

                        var skuFileName = ConfigurationManager.AppSettings["skuFile"];
                        var skuRows = ReadFile<Sku, SkuMapper>(skuFileName);
                        InsertSkuData(context, skuRows);

                        var inventoryHistoryFileName = ConfigurationManager.AppSettings["inventoryHistoryFile"];
                        //var inventoryHistoryFileRows = ReadFile<InventoryHistoryFileRow, InventoryHistoryMapper>(inventoryHistoryFileName).ToList();

                        var inventoryHistoryRows = ReadFile<InventoryHistory, InventoryHistoryMapper>(inventoryHistoryFileName);
                        //var inventoryHistoryDataTable = ConvertInventoryHistoryFileRowsToDataTable(inventoryHistoryFileRows, result, context);
                        InsertInventoryHistoryData(context, inventoryHistoryRows);

                        transaction.Commit();

                        result.AppendLine("History loading succeeded");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result.AppendLine($"Program aborted - {ex.Message}");
                    }
                }
            }

            return result;
        }

        private IEnumerable<TEntity> ReadFile<TEntity, TEntityMapper>(string fileName) where TEntityMapper : ClassMap
        {
            using (var csv = new CsvReader(File.OpenText(fileName)))
            {
                csv.Configuration.RegisterClassMap<TEntityMapper>();
                var rows = csv.GetRecords<TEntity>().ToList();

                return rows;
            }
        }

        private IEnumerable<InventoryHistory> ReadInventoryHistoryFile(string fileName, LoaderContext context, StringBuilder result)
        {
            var rows = new List<InventoryHistory>();
            var count = 0;
            using (var reader = new StreamReader(fileName))
            {
               
                while (!reader.EndOfStream)
                {
                    count++;
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var storeName = values[0];
                    var skuName = values[1];
                    var store = context.Stores.FirstOrDefault(t => t.Name == storeName);
                    if (store == null)
                    {
                        result.AppendLine($"Store {values[0]} does not exist in stores table");
                        continue;
                    }

                    var sku = context.Skus.FirstOrDefault(t => t.Name == skuName);
                    if (sku == null)
                    {
                        result.AppendLine($"Sku {values[1]} does not exist in sku table");
                        continue;
                    }

                    rows.Add(new InventoryHistory
                    {
                        Inv = int.Parse(values[3]),
                        SKUID = sku.ID,
                        Date = DateTime.Parse(values[2]),
                        StoreID = store.ID
                    });
                    Console.WriteLine(count);
                }
            }

            return rows;
        }
        private IEnumerable<InventoryHistory> ConvertInventoryHistoryFileRowsToDataTable(IEnumerable<InventoryHistoryFileRow> fileRows, StringBuilder result, LoaderContext context)
        {
            //var dt = new DataTable();
            //dt.Columns.Add("StoreID");
            //dt.Columns.Add("SKUID");
            //dt.Columns.Add("Date");
            //dt.Columns.Add("Inv");
            var rows = new List<InventoryHistory>();

            var count = 0;
            Parallel.ForEach(fileRows, fileRow =>
            {
                count++;
                var store = context.Stores.FirstOrDefault(t => t.Name == fileRow.Store);
                if (store == null)
                {
                    result.AppendLine($"Store {fileRow.Store} does not exist in stores table");
                    return;
                }

                var sku = context.Skus.FirstOrDefault(t => t.Name == fileRow.SKU);
                if (sku == null)
                {
                    result.AppendLine($"Sku {fileRow.SKU} does not exist in sku table");
                    return;
                }

                rows.Add(new InventoryHistory
                {
                    Inv = fileRow.Inv,
                    SKUID = sku.ID,
                    Date = fileRow.Date,
                    StoreID = store.ID
                });
                Console.WriteLine(count);
            });

            return rows;
        }


        private void InsertInventoryHistoryData(LoaderContext context, IEnumerable<InventoryHistory> rows)
        {
            var connectionString = context.Database.Connection.ConnectionString;
            using (var sqlBulk = new SqlBulkCopy(connectionString))
            {
                sqlBulk.BatchSize = int.Parse(ConfigurationManager.AppSettings["batchSize"]);
                sqlBulk.BulkCopyTimeout = 0;
                sqlBulk.DestinationTableName = "InventoryHistory";
                sqlBulk.WriteToServer(rows.AsDataReaderOfObjects());
            }
        }


        private void InsertSkuData(LoaderContext context, IEnumerable<Sku> rows)
        {
            var uniqueRows = rows.DistinctBy(t => t.Name);
            context.Skus.AddRange(uniqueRows);
            context.SaveChanges();
        }

        private void InsertStoreData(LoaderContext context, IEnumerable<Store> rows)
        {
            var uniqueRows = rows.DistinctBy(t => t.Name);
            context.Stores.AddRange(uniqueRows);
            context.SaveChanges();
        }
    }
}
