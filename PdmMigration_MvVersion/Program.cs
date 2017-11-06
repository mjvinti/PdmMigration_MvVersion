using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PdmMigration_MvVersion
{
    class Program
    {
        public static string catalogFile = @"C:\Users\mvinti\Desktop\PDM\PdmMigration_Remote_2017-11-03\EA\PDM-Catalog_2017-11-01.csv";
        public static string inputFile = @"C:\Users\mvinti\Desktop\PDM\PdmMigration_Remote_2017-11-03\EA\EA_2017-11-01.txt";
        public static string batchFile = @"C:\Users\mvinti\Desktop\PDM\PdmMigration_Remote_2017-11-03\EA\singlePdfCopy.bat";
        public static string serverName = "pdm.moog.com";
        public static string outputFile = @"C:\Users\mvinti\Desktop\PDM\PdmMigration_Remote_2017-11-03\EA\EA_import2_2017-11-01.txt";
        public static string misfitToys = @"C:\Users\mvinti\Desktop\PDM\PdmMigration_Remote_2017-11-03\EA\EA_importMisfits2_2017-11-01.txt";
        public static string jobTicketLocation = @"C:\Users\mvinti\Desktop\PDM\PdmMigration_Remote_2017-11-03\EA\jobTickets\";
        public static string uncRawPrefix = @"\\eacmpnas01.moog.com\Vol5_Data\PDM\EA";
        public static string uncPdfPrefix = @"\\eacmpnas01.moog.com\Vol5_Data\PDM\EA\tcpdf";
        public static DateTime recentDateTime = DateTime.MinValue;
        public static bool isWindows = false;
        public static bool isLuDateTime = false;
        public static bool isIeDateTime = false;

        static void Main(string[] args)
        {
            Dictionary<string, List<PdmItem>> dictionary = new Dictionary<string, List<PdmItem>>();
            Hashtable pdmCatalogTable = PdmHashtable.LoadPdmCatalog();
            List<string> delimitedDataField = new List<string> { "FILE_SIZE,LAST_ACCESSED,ITEM,REV,SHEET,SERVER,UNC_RAW,UNC_PDF" };
            List<string> islandOfMisfitToys = new List<string>();
            List<string> batchLines = new List<string>();

            //parse extract file
            StreamReader file = new StreamReader(inputFile);
            string inputLine;

            while ((inputLine = file.ReadLine()) != null)
            {
                Console.WriteLine(inputLine);
                PdmItem pdmItem = new PdmItem();
                pdmItem.ParseExtractLine(inputLine);

                if (pdmItem.FileDateTime < recentDateTime)
                {
                    continue;
                }

                if (!pdmCatalogTable.ContainsKey(pdmItem.FileName))
                {
                    pdmItem.IsMisfit = true;
                    islandOfMisfitToys.Add("Not in catalog: " + inputLine);
                    continue;
                }

                if (pdmItem.IsMisfit)
                {
                    islandOfMisfitToys.Add("Misfit: " + inputLine);
                }
                else
                {
                    delimitedDataField.Add(pdmItem.GetOutputLine());
                }

                //logic to handle no revs
                string uID;
                if (String.IsNullOrEmpty(pdmItem.ItemRev))
                {
                    uID = pdmItem.ItemName;
                }
                else
                {
                    uID = pdmItem.ItemName + "." + pdmItem.ItemRev;
                }

                if (!dictionary.Keys.Contains(uID))
                {
                    List<PdmItem> pdmItems = new List<PdmItem>();
                    pdmItems.Add(pdmItem);
                    dictionary.Add(uID, pdmItems);
                }
                else
                {
                    dictionary[uID].Add(pdmItem);
                }
            }

            //output all misfits to file
            File.WriteAllLines(misfitToys, islandOfMisfitToys);

            //Comment this next code until misfits are reviewed and corrected in source extract file
            //generate file for Graig
            File.WriteAllLines(outputFile, delimitedDataField);

            //generate XML job tickets
            //XmlTicket.JobTicketGenerator(dictionary, batchLines);
        }
    }
}
