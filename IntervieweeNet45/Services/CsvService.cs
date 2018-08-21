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

        public void BulkImportJob(int entity, string delimiter)
        {
            using (var db = new IntervieweeDbContext())
            {
                if(entity == 1)
                    EFBatchOperation.For(db, db.Interviewees).InsertAll(ImportCsv<Interviewee>(typeof(Interviewee).Name + ".csv", delimiter, true));
                else if (entity == 2)
                    EFBatchOperation.For(db, db.Provinces).InsertAll(ImportCsv<Province>(typeof(Province).Name + ".csv", delimiter, true));
                else if (entity == 3)
                    EFBatchOperation.For(db, db.CityMuns).InsertAll(ImportCsv<CityMun>(typeof(CityMun).Name + ".csv", delimiter, true));
                else if (entity == 4)
                    EFBatchOperation.For(db, db.Barangays).InsertAll(ImportCsv<Barangay>(typeof(Barangay).Name + ".csv", delimiter, true));

            }
        }

        private IList<T> ImportCsv<T>(string fileName, string delimiter, bool clearTable)
        {
            var tableName = typeof(T).GetTypeInfo().GetCustomAttribute<TableAttribute>().Name;
            if (clearTable)
            {
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

                _log.LogInformation($"[BULK IMPORTING...] \n=====| Type: {CsvType}, Filename: {fileName}, Tablename: {tableName} |=====");
                using (TextReader reader = System.IO.File.OpenText(filePath))
                {
                    var csv = new CsvReader(reader);
                    csv.Configuration.Delimiter = delimiter; // Set the delimeter

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
