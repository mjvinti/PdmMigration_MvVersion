using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PdmMigration_MvVersion
{
    class Program
    {
        public static string catalogFile = @"";
        public static string inputFile = @"";
        public static string batchFile = @"";
        public static string serverName = "";
        public static string outputFile = @"";
        public static string misfitToys = @"";
        public static string jobTicketLocation = @"";
        public static string uncRawPrefix = @"";
        public static string uncPdfPrefix = @"";
        public static string adlibDTD = @"";
        public static DateTime recentDateTime = DateTime.MinValue;
        public static bool isWindows = false;
        public static bool isLuDateTime = false;
        public static bool isIeDateTime = false;

        public static void LoadConfig()
        {
            catalogFile = ConfigurationManager.AppSettings["catalogFile"];
            inputFile = ConfigurationManager.AppSettings["inputFile"];
            batchFile = ConfigurationManager.AppSettings["batchFile"];
            serverName = ConfigurationManager.AppSettings["serverName"];
            outputFile = ConfigurationManager.AppSettings["outputFile"];
            misfitToys = ConfigurationManager.AppSettings["misfitToys"];
            jobTicketLocation = ConfigurationManager.AppSettings["jobTicketLocation"];
            uncRawPrefix = ConfigurationManager.AppSettings["uncRawPrefix"];
            uncPdfPrefix = ConfigurationManager.AppSettings["uncPdfPrefix"];
            adlibDTD = ConfigurationManager.AppSettings["adlibDTD"];
            recentDateTime = DateTime.Parse(ConfigurationManager.AppSettings["recentDateTime"]);
            isWindows = Convert.ToBoolean(ConfigurationManager.AppSettings["isWindows"]);
            isLuDateTime = Convert.ToBoolean(ConfigurationManager.AppSettings["isLuDateTime"]);
            isIeDateTime = Convert.ToBoolean(ConfigurationManager.AppSettings["isIeDateTime"]);

            Console.WriteLine(catalogFile);
            Console.WriteLine(inputFile);
            Console.WriteLine(batchFile);
            Console.WriteLine(serverName);
            Console.WriteLine(outputFile);
            Console.WriteLine(misfitToys);
            Console.WriteLine(jobTicketLocation);
            Console.WriteLine(uncRawPrefix);
            Console.WriteLine(uncPdfPrefix);
            Console.WriteLine(recentDateTime.ToString());
            Console.WriteLine(isWindows);
            Console.WriteLine(isLuDateTime);
            Console.WriteLine(isIeDateTime);
        }

        static void Main(string[] args)
        {
            LoadConfig();

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
            XmlTicket.JobTicketGenerator(dictionary, batchLines);
        }
    }
}
