using System;
using System.Text;

namespace PdmMigration_MvVersion
{
    public class UncPath
    {
        public static string BuildUncRawPath(string uncRawPrefix, string filePath)
        {
            StringBuilder uncRawPathName = new StringBuilder();

            if (filePath.EndsWith(".Z"))
            {
                filePath = filePath.Remove(filePath.Length - 2);
            }

            uncRawPathName.Append(uncRawPrefix);
            uncRawPathName.Append(filePath);
            uncRawPathName.Replace('/', '\\');

            return uncRawPathName.ToString();
        }

        public static string BuildUncPdfPath(string uncPdfPrefix, string itemName, string itemRev)
        {
            StringBuilder uncPdfPathName = new StringBuilder();

            uncPdfPathName.Append(uncPdfPrefix + "\\" + itemName);

            if (!String.IsNullOrEmpty(itemRev))
            {
                uncPdfPathName.Append("." + itemRev);
            }

            uncPdfPathName.Append(".pdf");
            uncPdfPathName.Replace('/', '\\');

            return uncPdfPathName.ToString();
        }
    }
}
