using CsvHelper.Configuration;
using IntervieweeNet45.Models;
using IntervieweeNet45.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace IntervieweeNet45.Helpers
{
    sealed class CsvMapper<T> : ClassMap<T>
    {
        public CsvMapper()
        {
            //AutoMap(); // Automap fields having the same column header name as the class properties 

            var classProps = typeof(T).GetProperties();
            // Enables logging of Primary key
            var pKey = string.Empty;
            var pCsvKey = classProps.FirstOrDefault(p => p.CustomAttributes.Any(attr => attr.AttributeType == typeof(CsvKey)));
            if (pCsvKey != null)
                pKey = pCsvKey.Name;
            else
            {
                var KeyAttrib = classProps.FirstOrDefault(p => p.CustomAttributes.Any(attr => attr.AttributeType == typeof(KeyAttribute)));
                if (KeyAttrib != null)
                    pKey = KeyAttrib.Name;
                else
                    pKey = "Id";
            }

            foreach (var prop in classProps)
            {
                var IsCsvIgnore = prop.CustomAttributes.Any(attr => attr.AttributeType == typeof(CsvIgnore));

                if (!IsCsvIgnore) // prop.Name != "Id" && 
                {
                    var className = typeof(T);
                    var columnType = prop.PropertyType;
                    var columnName = prop.Name;
                    // Underscore suffix as workaround for column name which is a C# reserved words
                    var csvColumnName = columnName.Any(x => x.ToString().EndsWith("_")) ? columnName.TrimEnd('_') : columnName;

                    // For columns containing spaces in Nerp CSV
                    var csvNerpNameAttribute = (CsvColumnName)prop.GetCustomAttribute(typeof(CsvColumnName));
                    if (csvNerpNameAttribute != null) csvColumnName = csvNerpNameAttribute.ColumnName;

                    var parameterExpression = Expression.Parameter(typeof(T), "x");
                    var memberExpression = Expression.PropertyOrField(parameterExpression, prop.Name);
                    var memberExpressionConversion = Expression.Convert(memberExpression, columnType);

                    if (columnType == typeof(int?))
                    {
                        var exp = Expression.Lambda<Func<T, int?>>(memberExpressionConversion, parameterExpression);
                        Map(exp).Name(columnName).ConvertUsing(row => CsvTypeParser.ParseIntNullable(row.GetField<String>(csvColumnName), pKey + ": " + row.GetField<String>(pKey) + ", " + csvColumnName));
                    }
                    else if (columnType == typeof(int))
                    {
                        var exp = Expression.Lambda<Func<T, int>>(memberExpressionConversion, parameterExpression);
                        Map(exp).Name(columnName).ConvertUsing(row => CsvTypeParser.ParseInt(row.GetField<String>(csvColumnName), pKey + ": " + row.GetField<String>(pKey) + ", " + csvColumnName));
                    }
                    else if (columnType == typeof(decimal?))
                    {
                        var exp = Expression.Lambda<Func<T, decimal?>>(memberExpressionConversion, parameterExpression);
                        Map(exp).Name(columnName).ConvertUsing(row => CsvTypeParser.ParseDecimalNullable(row.GetField<String>(csvColumnName), pKey + ": " + row.GetField<String>(pKey) + ", " + csvColumnName));

                    }
                    else if (columnType == typeof(decimal))
                    {
                        var exp = Expression.Lambda<Func<T, decimal>>(memberExpressionConversion, parameterExpression);
                        Map(exp).Name(columnName).ConvertUsing(row => CsvTypeParser.ParseDecimal(row.GetField<String>(csvColumnName), pKey + ": " + row.GetField<String>(pKey) + ", " + csvColumnName));
                    }
                    else if (columnType == typeof(DateTime?))
                    {
                        if((CsvMergeToDate)prop.GetCustomAttribute(typeof(CsvMergeToDate)) == null)
                        {
                            // If has CsvMergeTotime attribute, then merge time to date column
                            var csvMergeToTimeAttribute = (CsvMergeToTime)prop.GetCustomAttribute(typeof(CsvMergeToTime));
                            var columnTime = csvMergeToTimeAttribute != null ? csvMergeToTimeAttribute.DateColumnName : string.Empty;
                            // Underscore suffix as workaround for column name which is a C# reserved words
                            columnTime = columnTime.Any(x => x.ToString().EndsWith("_")) ? columnTime.TrimEnd('_') : columnTime;

                            var exp = Expression.Lambda<Func<T, DateTime?>>(memberExpressionConversion, parameterExpression);
                            if (string.IsNullOrEmpty(columnTime))
                                Map(exp).Name(columnName).ConvertUsing(row => CsvTypeParser.ParseDateNullable(row.GetField<String>(csvColumnName), string.Empty, pKey + ": " + row.GetField<String>(pKey) + ", " + csvColumnName));
                            else
                                Map(exp).Name(columnName).ConvertUsing(row => CsvTypeParser.ParseDateNullable(row.GetField<String>(csvColumnName), row.GetField<String>(columnTime), pKey + ": " + row.GetField<String>(pKey) + ", " + csvColumnName));
                        }
                    }
                    else if (columnType == typeof(double?))
                    {
                        var exp = Expression.Lambda<Func<T, double?>>(memberExpressionConversion, parameterExpression);
                        Map(exp).Name(columnName).ConvertUsing(row => CsvTypeParser.ParseDoubleNullable(row.GetField<String>(csvColumnName), pKey + ": " + row.GetField<String>(pKey) + ", " + csvColumnName));

                    }
                    else if (columnType == typeof(double))
                    {
                        var exp = Expression.Lambda<Func<T, double>>(memberExpressionConversion, parameterExpression);
                        Map(exp).Name(columnName).ConvertUsing(row => CsvTypeParser.ParseDouble(row.GetField<String>(csvColumnName), pKey + ": " + row.GetField<String>(pKey) + ", " + csvColumnName));
                    }
                    else if (columnType == typeof(string)) // Type string for all other columns
                    {
                        var exp = Expression.Lambda<Func<T, string>>(memberExpressionConversion, parameterExpression);
                        Map(exp).Name(columnName).ConvertUsing(row => row.GetField<String>(csvColumnName));
                    }
                }

            }
        }
    }

    sealed class CsvWriteMapper<T> : ClassMap<T>
    {
        public CsvWriteMapper()
        {
            // AutoMap(); // Automap fields having the same column header name as the class properties 
            foreach (var prop in typeof(T).GetProperties().Where(x=> !x.CustomAttributes.Any(attr => attr.AttributeType == typeof(CsvMergeToDate))))
            {
                var IsCsvIgnore = prop.CustomAttributes.Any(attr => attr.AttributeType == typeof(CsvIgnore));

                if (!IsCsvIgnore) // prop.Name != "Id"
                {
                    var columnName = prop.Name;
                    var columnType = prop.PropertyType;

                    // Workaround for column name which is a C# reserved words
                    var csvColumnName = columnName.Any(x => x.ToString().EndsWith("_")) ? columnName.TrimEnd('_') : columnName;
                    // For columns containing spaces in Nerp CSV
                    var csvNerpNameAttribute = (CsvColumnName)prop.GetCustomAttribute(typeof(CsvColumnName));
                    if (csvNerpNameAttribute != null) csvColumnName = csvNerpNameAttribute.ColumnName;

                    var parameterExpression = Expression.Parameter(typeof(T), "x");
                    var memberExpression = Expression.PropertyOrField(parameterExpression, prop.Name);
                    var memberExpressionConversion = Expression.Convert(memberExpression, columnType);



                    if (columnType == typeof(DateTime?))
                    {
                        var exp = Expression.Lambda<Func<T, DateTime?>>(memberExpressionConversion, parameterExpression);

                        var csvMergeToTimeAttribute = (CsvMergeToTime)prop.GetCustomAttribute(typeof(CsvMergeToTime));
                        if (csvMergeToTimeAttribute != null)
                        {
                            var columnTime = csvMergeToTimeAttribute.DateColumnName;
                            // Underscore suffix as workaround for column name which is a C# reserved words
                            columnTime = columnTime.Any(x => x.ToString().EndsWith("_")) ? columnTime.TrimEnd('_') : columnTime;
                            // Get time from this date column
                            Map(exp).Name(columnTime).ConvertUsing(row => row.GetField<DateTime?>(columnName)).TypeConverterOption.Format("HH:mm:ss");
                        }
                        else
                        {
                            Map(exp).Name(csvColumnName).ConvertUsing(row => row.GetField<DateTime?>(columnName)).TypeConverterOption.Format("MM-dd-yyyy");
                        }
                    }
                    else if (columnType == typeof(decimal?))
                    {
                        var exp = Expression.Lambda<Func<T, decimal?>>(memberExpressionConversion, parameterExpression);
                        Map(exp).Name(csvColumnName);
                    }
                    else if (columnType == typeof(decimal))
                    {
                        var exp = Expression.Lambda<Func<T, decimal>>(memberExpressionConversion, parameterExpression);
                        Map(exp).Name(csvColumnName);
                    }
                    else if (columnType == typeof(int?))
                    {
                        var exp = Expression.Lambda<Func<T, int?>>(memberExpressionConversion, parameterExpression);
                        Map(exp).Name(csvColumnName);
                    }
                    else if (columnType == typeof(int))
                    {
                        var exp = Expression.Lambda<Func<T, int>>(memberExpressionConversion, parameterExpression);
                        Map(exp).Name(csvColumnName);
                    }
                    else if (columnType == typeof(double?))
                    {
                        var exp = Expression.Lambda<Func<T, double?>>(memberExpressionConversion, parameterExpression);
                        Map(exp).Name(csvColumnName);
                    }
                    else if (columnType == typeof(double))
                    {
                        var exp = Expression.Lambda<Func<T, double>>(memberExpressionConversion, parameterExpression);
                        Map(exp).Name(csvColumnName);
                    }
                    else
                    {
                        // All other type (string)
                        var exp = Expression.Lambda<Func<T, string>>(memberExpressionConversion, parameterExpression);
                        Map(exp).Name(csvColumnName).ConvertUsing(row => row.GetField<string>(columnName));
                    }
                }
            }
        }

    }
}

#region CustomAttributes
namespace IntervieweeNet45.Models
{
    public class CsvIgnore : Attribute { }

    public class CsvKey : Attribute { }

    public class CsvMergeToDate : Attribute
    {
        readonly string _date;
        public CsvMergeToDate(string DateColumnName) { _date = DateColumnName; }
        public string DateColumnName { get { return _date; } }
    }

    public class CsvMergeToTime : Attribute
    {
        readonly string _time;
        public CsvMergeToTime(string TimeOnlyColumnName) { _time = TimeOnlyColumnName; }
        public string DateColumnName { get { return _time; } }
    }

    public class CsvColumnName : Attribute
    {
        readonly string _name;
        public CsvColumnName(string ColumnName) { _name = ColumnName; }
        public string ColumnName { get { return _name; } }
    }

}
#endregion
