namespace PdmMigration_MvVersion
{
    class IsExtPdfAble
    {
        public static bool IsPdfAble(string fileName)
        {
            if (fileName.EndsWith(".Z"))
            {
                return false;
            }

            if (fileName.EndsWith("._"))
            {
                return false;
            }

            if (fileName.ToLower().EndsWith(".zip"))
            {
                return false;
            }

            if (fileName.ToLower().EndsWith(".doc"))
            {
                return false;
            }

            if (fileName.ToLower().EndsWith(".docx"))
            {
                return false;
            }

            if (fileName.ToLower().EndsWith(".mpg"))
            {
                return false;
            }

            if (fileName.ToLower().EndsWith(".flv"))
            {
                return false;
            }

            return true;
        }
    }
}
