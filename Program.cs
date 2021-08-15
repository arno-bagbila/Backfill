using System;
using System.IO;

namespace Backfill
{
   class Program
   {
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
            Console.WriteLine("You are about to generate the sql scripts to import colors");
         }
      }

      private static void PrintUserName(string userName)
      {
         Console.WriteLine($"{userName} you can carry on the backfilling");
      }
   }
}
