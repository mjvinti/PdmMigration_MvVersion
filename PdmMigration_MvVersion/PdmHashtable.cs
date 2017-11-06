using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PdmMigration_MvVersion
{
    class PdmHashtable
    {
        public static Hashtable LoadPdmCatalog()
        {
            Hashtable pdmHashTable = new Hashtable();

            //load Pdm Catalog File
            StreamReader sr = new StreamReader(Program.catalogFile);
            string headerLine = sr.ReadLine();
            string catalogLine;

            while ((catalogLine = sr.ReadLine()) != null)
            {
                var pdmCatalogItem = new PdmItem();
                List<string> pdmCatalog = catalogLine.Split(',').ToList();

                pdmCatalogItem.Server = pdmCatalog[0];
                pdmCatalogItem.FileName = pdmCatalog[2];

                if (!pdmHashTable.ContainsKey(pdmCatalogItem.FileName))
                {
                    pdmHashTable.Add(pdmCatalogItem.FileName, null);
                }
            }
            return pdmHashTable;
        }
    }
}
