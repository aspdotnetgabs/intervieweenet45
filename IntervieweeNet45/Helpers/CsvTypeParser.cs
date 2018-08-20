using IntervieweeNet45.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace IntervieweeNet45.Helpers
{
    public static class CsvTypeParser
    {
        private static ISimpleLogger _log;
        public static void InitLogger(ISimpleLogger log)
        {
            _log = log;
        }

        public static DateTime? ParseDateNullable(string strDate, string strTime = "", string rowId = "")
        {
            if (!string.IsNullOrWhiteSpace(strDate))
            {
                try
                {
                    DateTime dateOnly;
                    string[] dateFormat = {
                        "MM-dd-yyyy",
                        "MM-d-yyyy",
                        "M-dd-yyyy",
                        "M-d-yyyy",
                        "MM-dd-yy",
                        "MM/dd/yyyy",
                        "MM/d/yyyy",
                        "M/dd/yyyy",
                        "M/d/yyyy",

                        "MM-dd-yyyy HH:mm:ss",
                        "MM-d-yyyy HH:mm:ss",
                        "MM/dd/yyyy HH:mm:ss",
                        "MM/d/yyyy HH:mm:ss",

                        "MM-dd-yyyy hh:mm:ss tt",
                        "MM-d-yyyy hh:mm:ss tt",
                        "MM/dd/yyyy hh:mm:ss tt",
                        "MM/d/yyyy hh:mm:ss tt",

                        "M-dd-yyyy HH:mm:ss",
                        "M-d-yyyy HH:mm:ss",
                        "M/dd/yyyy HH:mm:ss",
                        "M/d/yyyy HH:mm:ss",

                        "M-dd-yyyy hh:mm:ss tt",
                        "M-d-yyyy hh:mm:ss tt",
                        "M/dd/yyyy hh:mm:ss tt",
                        "M/d/yyyy hh:mm:ss tt",


                        "MM-dd-yyyy HH:mm:",
                        "MM-d-yyyy HH:mm",
                        "MM/dd/yyyy HH:mm",
                        "MM/d/yyyy HH:mm",

                        "MM-dd-yyyy hh:mm tt",
                        "MM-d-yyyy hh:mm tt",
                        "MM/dd/yyyy hh:mm tt",
                        "MM/d/yyyy hh:mm tt",

                        "M-dd-yyyy HH:mm",
                        "M-d-yyyy HH:mm",
                        "M/dd/yyyy HH:mm",
                        "M/d/yyyy HH:mm",

                        "M-dd-yyyy hh:mm tt",
                        "M-d-yyyy hh:mm tt",
                        "M/dd/yyyy hh:mm tt",
                        "M/d/yyyy hh:mm tt",

                    };
                    dateOnly = DateTime.ParseExact(strDate, dateFormat, new CultureInfo("en-US"), DateTimeStyles.None);
                    //dateOnly = Convert.ToDateTime(strDate);
                    if (!string.IsNullOrWhiteSpace(strTime))
                    {
                        try
                        {
                            DateTime timeOnly;
                            string[] timeFormat = { "HH:mm:ss", "HH:mm", "H:mm:ss", "H:mm" };
                            timeOnly = DateTime.ParseExact(strTime, timeFormat, new CultureInfo("en-US"), DateTimeStyles.None);
                            return dateOnly.Date.Add(timeOnly.TimeOfDay);
                        }
                        catch
                        {
                            rowId = string.IsNullOrEmpty(rowId) ? string.Empty : "[" + rowId + "]";
                            var err = String.Format("[PARSE] Could not parse Time: {0} {1}", strDate + " " + strTime, rowId);
                            _log.LogError(err);
                        }
                    }
                    return dateOnly;
                }
                catch
                {
                    if (!string.IsNullOrWhiteSpace(strDate.Trim()))
                    {
                        rowId = string.IsNullOrEmpty(rowId) ? string.Empty : "[" + rowId + "]";
                        var err = String.Format("[PARSE] Could not parse Date: {0} {1}", strDate + " " + strTime, rowId);
                        _log.LogError(err);
                        //throw new FormatException(err);
                    }
                }

            }

            return null;
        }

        public static DateTime ParseDate(string strDate, string strTime = "", string rowId = "")
        {
            var ret = ParseDateNullable(strDate, strTime, rowId);
            if (ret == null)
                return DateTime.MinValue;
            
            return ret.Value;
        }

        public static int? ParseIntNullable(string strInt, string rowId="")
        {
            if (!string.IsNullOrWhiteSpace(strInt))
            {
                try
                {
                    return Int32.Parse(strInt, NumberStyles.Any);
                }
                catch
                {
                    if(strInt != "NaN")
                    {
                        rowId = string.IsNullOrEmpty(rowId) ? string.Empty : "[" + rowId + "]";
                        var err = String.Format("[PARSE] Could not parse Integer: {0} {1}", strInt, rowId);
                        _log.LogError(err);
                        //throw new FormatException(err);
                    }
                }
            }

            return null;
        }

        public static int ParseInt(string strInt, string rowId = "")
        {
            var ret = ParseIntNullable(strInt, rowId);
            if (ret != null)
                return ret.Value;

            return 0;
        }

        public static decimal? ParseDecimalNullable(string strDecimal, string rowId = "")
        {
            if (!string.IsNullOrWhiteSpace(strDecimal))
            {
                try
                {
                    return decimal.Parse(strDecimal);
                }
                catch
                {
                    rowId = string.IsNullOrEmpty(rowId) ? string.Empty : "[" + rowId + "]";
                    var err = String.Format("[PARSE] Could not parse Decimal: {0} {1}", strDecimal, rowId);
                    _log.LogError(err);
                    //throw new FormatException(err);
                }
            }

            return null;
        }

        public static decimal ParseDecimal(string strDecimal, string rowId = "")
        {
            var ret = ParseDecimalNullable(strDecimal, rowId);
            if (ret != null)
                return ret.Value;

            return decimal.Zero;
        }

        public static double? ParseDoubleNullable(string strDouble, string rowId = "")
        {
            if (!string.IsNullOrWhiteSpace(strDouble))
            {
                try
                {
                    return double.Parse(strDouble);
                }
                catch
                {
                    rowId = string.IsNullOrEmpty(rowId) ? string.Empty : "[" + rowId + "]";
                    var err = String.Format("[PARSE] Could not parse Double: {0} {1}", strDouble, rowId);
                    _log.LogError(err);
                }
            }

            return null;
        }

        public static double ParseDouble(string strDouble, string rowId = "")
        {
            var ret = ParseDoubleNullable(strDouble, rowId);
            if (ret != null)
                return ret.Value;

            return 0.00;
        }

    }

    public static class CsvTypeWriter
    {
        private static ISimpleLogger _log;
        public static void InitLogger(ISimpleLogger log)
        {
            _log = log;
        }

        public static string ToDateString(DateTime? dateonly, string rowId = "")
        {
            if (dateonly == null || dateonly == DateTime.MinValue)
                return string.Empty;
            else
                return dateonly.Value.ToString("MM-dd-yyyy");
        }

        public static string ToTimeString(DateTime? date, string rowId = "")
        {
            if (date == null || date.Value.Date == DateTime.MinValue)
                return string.Empty;
            else
                return date.Value.ToString("HH:mm:ss");
        }


        public static string ToIntString(int? intValue, string rowId = "")
        {
            if (intValue == null)
                return string.Empty;
            else
                return intValue.Value.ToString();
        }

        public static string ToDecimalString(decimal? decValue, string rowId = "")
        {
            if (decValue == null)
                return string.Empty;
            else
                return decValue.Value.ToString();
        }

    }

}
