using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Backfill
{
   class Program
   {

      private const int COLORS_SHEET_COLUMNS_NUMBER = 1;
      private const string COLOR_TEMPLATE = @".\TemplateScripts\color.sql";
      private const string COLOR_OUTPUT_SCRIPT = @".\color";

      static void Main(string[] args)
      {
         string userName;
         do
         {
            Console.WriteLine("Please enter user name:");
            userName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userName))
            {
               Console.WriteLine("You must enter a valid name in order to carry on the backfill!");
            }
         } while (string.IsNullOrWhiteSpace(userName));

         PrintUserName(userName);

         bool fileExists;
         string path;
         do
         {
            Console.WriteLine("Please enter the path to the excel file: ");

            path = Console.ReadLine();
            fileExists = File.Exists(path);

            if (!fileExists)
            {
               Console.WriteLine("No excel file exists at this location");
            }

         } while (!fileExists);

         string backfillOption;
         do
         {
            Console.WriteLine("What type of data do you want to backfill?: \"color\", \"country\", \"flavour\", \"category\" or \"beer\":");
            backfillOption = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(backfillOption) || (backfillOption != "color" &&
                                                              backfillOption != "country" &&
                                                              backfillOption != "flavour" &&
                                                              backfillOption != "category" &&
                                                              backfillOption != "beer"))
            {
               Console.WriteLine("Option cannot be null or empty or different from color, country, flavour, category or beer");
            }
         } while (string.IsNullOrWhiteSpace(backfillOption) || (backfillOption != "color" &&
                                                                backfillOption != "country" &&
                                                                backfillOption != "flavour" && 
                                                                backfillOption != "category" && 
                                                                backfillOption != "beer"));

         if (backfillOption == "color")
         {
            GenerateColors(path);
         }

         Console.WriteLine("Done");
         Console.ReadKey();
      }

      #region Internals

      private static void PrintUserName(string userName)
      {
         Console.WriteLine($"{userName} you can carry on the backfilling");
      }

      private static void GenerateColors(string path)
      {
         var dataTables = GetDataTables(path);

         var colorTable =
            dataTables.FirstOrDefault(d => string.Equals(d.TableName, "colors", StringComparison.OrdinalIgnoreCase));

         ValidateTable(colorTable, COLORS_SHEET_COLUMNS_NUMBER, "colors");

         var colorBuiltValues = new List<string>();
         var colorUId = string.Empty;

         colorTable.Rows.RemoveAt(0);
         foreach (DataRow row in colorTable.Rows)
         {
            colorUId = GetMandatoryValue(row, 1, "colors", "ColorUId", r => colorTable.Rows.IndexOf(r));
            if (!Guid.TryParse(colorUId, out _))
            {
               throw new Exception(
                  $"ColorUId must be a guid in the colors sheet at the row {colorTable.Rows.IndexOf(row) + 2}");
            }

            var name = GetMandatoryValue(row, 0, "colors", "Name", r => colorTable.Rows.IndexOf(r));

            colorBuiltValues.Add($"  ('{colorUId}', '{name}')");
         }

         var colorScript = File.ReadAllText(COLOR_TEMPLATE);

         var colorScriptUpdated = colorScript.Replace("{colorValues}", string.Join($",{Environment.NewLine}", colorBuiltValues));

         File.WriteAllText($"{COLOR_OUTPUT_SCRIPT}.{DateTime.Now:HHmmddMMyyyy}.sql", colorScriptUpdated);

      }

      private static IEnumerable<DataTable> GetDataTables(string path)
      {

         Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
         using var stream = File.Open(path, FileMode.Open, FileAccess.Read);
         using var reader = ExcelReaderFactory.CreateReader(stream);
         return from table in reader.AsDataSet().Tables.OfType<DataTable>() select table;
      }

      private static void ValidateTable(DataTable table, int tableColumnsNumber, string tableName)
      {
         if (table == null)
         {
            throw new Exception($"There is no {tableName} sheet in the file.");
         }

         if (table.Columns.Count < tableColumnsNumber)
         {
            throw new Exception($"Missing columns in the {tableName} sheet.");
         }

         if (table.Rows.Count < 2)
         {
            throw new Exception($"There are no rows in {tableName} sheet.");
         }
      }

      private static string GetMandatoryValue(DataRow row, int colIndex, string tableName, string colName, Func<DataRow, int> rowIndexGetter)
      {
         var value = row.ItemArray[colIndex]?.ToString();
         if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "null", StringComparison.OrdinalIgnoreCase))
         {
            throw new Exception($"{colName} is null or empty in '{tableName}' sheet at row {rowIndexGetter(row) + 2}");
         }

         return value;
      }


      #endregion

   }
}
