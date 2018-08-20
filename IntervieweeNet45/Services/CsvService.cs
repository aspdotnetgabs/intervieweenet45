using IntervieweeNet45.Helpers;
using IntervieweeNet45.Models;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EntityFramework.Utilities;

namespace IntervieweeNet45.Services
{
    public class CsvService
    {
        private ISimpleLogger _log = new SimpleLogger();

        public void BulkImportJob()
        {
            using (var db = new IntervieweeDbContext())
            {
                EFBatchOperation.For(db, db.Interviewees).InsertAll(ImportCsv<Interviewee>(typeof(Interviewee).Name + ".csv", true));
            }
        }

        private IList<T> ImportCsv<T>(string fileName, bool clearTable)
        {
            if(clearTable)
            {
                var tableName = typeof(T).GetTypeInfo().GetCustomAttribute<TableAttribute>().Name;
                using (var db = new IntervieweeDbContext())
                {
                    db.Database.ExecuteSqlCommand("DELETE FROM [" + tableName + "]");
                }
                _log.LogInformation($"All {tableName} records deleted.");
            }

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", fileName);
            if (File.Exists(filePath))
            {
                CsvTypeParser.InitLogger(_log);
                string CsvType = typeof(T).Name;

                using (TextReader reader = System.IO.File.OpenText(filePath))
                {
                    var csv = new CsvReader(reader);
                    csv.Configuration.Delimiter = ","; // Set the delimeter

                    csv.Configuration.RegisterClassMap<CsvMapper<T>>();
                    csv.Configuration.HeaderValidated = null;
                    csv.Configuration.MissingFieldFound = null;
                    csv.Configuration.IgnoreQuotes = true;
                    csv.Configuration.IgnoreBlankLines = true;
                    csv.Configuration.BadDataFound = context =>
                    {
                        _log.LogError($"[{CsvType}] Bad data found on row '{context.RawRecord}'.");
                    };

                    var csvMapResult = csv.GetRecords<T>().ToList();
                    var csvResultCount = csvMapResult.Count;
                    _log.LogInformation($"[END OF BULK IMPORT]: {CsvType} | TOTAL: {csvMapResult.Count} / {csvResultCount}");

                    return csvMapResult;
                }

            }
            else
            {
                _log.LogError($"File not found: {fileName}");
                return new List<T>();
            }

        }


    }
}
