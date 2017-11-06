namespace PdmMigration_MvVersion
{
    public class IsExtension
    {
        public static bool IsExt(string token)
        {
            switch (token.ToLower())
            {
                case "7z":
                case "cad":
                case "cg4":
                case "csh":
                case "csv":
                case "db":
                case "dis":
                case "dll_crea":
                case "doc":
                case "docx":
                case "dos":
                case "dot":
                case "dwg":
                case "dxf":
                case "edt":
                case "flv":
                case "gif":
                case "gp4":
                case "gwk":
                case "hp":
                case "hpdf":
                case "hpg":
                case "hpp":
                case "htm":
                case "html":
                case "ini":
                case "jpg":
                case "js":
                case "mdb":
                case "mht":
                case "mil":
                case "mpg":
                case "msg":
                case "obd":
                case "oft":
                case "pcx":
                case "pdf":
                case "plt":
                case "png":
                case "ppt":
                case "pptx":
                case "pra":
                case "prt":
                case "reg":
                case "rss":
                case "rst":
                case "rtf":
                case "smf":
                case "ss":
                case "ss_old":
                case "tif":
                case "txt":
                case "url":
                case "vsd":
                case "wdf":
                case "wrl":
                case "xls":
                case "xlsx":
                case "xlt":
                case "xps":
                case "xs":
                case "xst":
                case "z":
                case "z_old":
                case "zip":
                    return true;
                default:
                    return false;
            }
        }
    }
}
