using System;
using System.Collections.Generic;
using System.Configuration;
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
                        var inventoryHistoryRows = ReadFile<InventoryHistory, InventoryHistoryMapper>(inventoryHistoryFileName);
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
       
        private void InsertInventoryHistoryData(LoaderContext context, IEnumerable<InventoryHistory> rows)
        {
            var connectionString = context.Database.Connection.ConnectionString;
            using (var sqlBulk = new SqlBulkCopy(connectionString))
            {
                sqlBulk.BatchSize = int.Parse(ConfigurationManager.AppSettings["batchSize"]);
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
