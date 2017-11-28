using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PdmMigration_MvVersion
{
    public class XmlTicket
    {
        public static void JobTicketGenerator(Dictionary<string, List<PdmItem>> dictionary, List<string> batchLines)
        {
            foreach (KeyValuePair<string, List<PdmItem>> kvp in dictionary)
            {   
                //if there is only one kvp, then we already have a pdf somewhere in theory
                if (kvp.Value.Count < 2)
                {
                    StringBuilder sourcePdfBuilder = new StringBuilder();

                    //find pdf and copy to correct folder; build batch file
                    if (kvp.Value[0].UncRaw.EndsWith(".pdf"))
                    {
                        sourcePdfBuilder.Append(kvp.Value[0].UncRaw.Replace("web\\", "web\\pdf\\"));
                    }
                    else
                    {
                        sourcePdfBuilder.Append(kvp.Value[0].UncRaw.Replace("web\\", "web\\pdf\\") + ".pdf");
                    }

                    if (File.Exists(sourcePdfBuilder.ToString()))
                    {
                        batchLines.Add("Copy " + sourcePdfBuilder.ToString() + " " + Program.uncPdfPrefix + "\\" + kvp.Key + ".pdf");
                        continue;
                    }
                    else
                    {
                        //do nothing and build the job ticket
                        batchLines.Add("REM FILE DOES NOT EXIST: " + sourcePdfBuilder.ToString());
                    }
                }

                StringBuilder jobTicket = new StringBuilder();

                jobTicket.AppendLine("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>");
                jobTicket.AppendLine("<?AdlibExpress applanguage = \"USA\" appversion = \"4.11.0\" dtdversion = \"2.6\" ?>");
                jobTicket.AppendLine("<!DOCTYPE JOBS SYSTEM \"" + Program.adlibDTD + "\">");
                jobTicket.AppendLine("<JOBS xmlns:JOBS=\"http://www.adlibsoftware.com\" xmlns:JOB=\"http://www.adlibsoftware.com\">");
                jobTicket.AppendLine("<JOB>");
                jobTicket.AppendLine("<JOB:DOCINPUTS>");

                DateTime mostRecentDate = DateTime.MinValue;
                foreach (var i in kvp.Value)
                {
                    //Find most recent date in list
                    if (i.FileDateTime > mostRecentDate)
                    {
                        mostRecentDate = i.FileDateTime;
                    }
                }

                var orderedItemShtNums = kvp.Value.OrderBy(x => x.ItemShtNum);

                foreach (var i in orderedItemShtNums)
                {
                    string filename = i.FileName;

                    if (filename.EndsWith(".Z") || filename.EndsWith("._"))
                    {
                        filename = filename.Remove(filename.Length - 2, 2);
                    }

                    if (filename.EndsWith(".pra"))
                    {
                        filename += ".plt";
                    }

                    if (i.PdfAble)
                    {
                        if (i.FileDateTime == mostRecentDate)
                        {
                            jobTicket.AppendLine("<JOB:DOCINPUT FILENAME=\"" + filename + "\" FOLDER=\"" + Program.uncRawPrefix + i.FilePath.Replace("/", "\\") + "\"/>");
                        }
                        else
                        {
                            jobTicket.AppendLine("<!-- SKIPPING(OLDER DATE): " + filename + ", " + i.FilePath.Replace("/", "\\") + " -->");
                        }
                    }
                    else
                    {
                        jobTicket.AppendLine("<!-- SKIPPING(NOT PDF-ABLE): " + filename + ", " + i.FilePath.Replace("/", "\\") + " -->");
                        Console.WriteLine("THIS IS NOT PDF-ABLE: " + filename + ", " + i.FilePath);
                    }
                }

                jobTicket.AppendLine("</JOB:DOCINPUTS>");
                jobTicket.AppendLine("<JOB:DOCOUTPUTS>");
                jobTicket.AppendLine("<JOB:DOCOUTPUT FILENAME=\"" + kvp.Key + ".pdf\" FOLDER=\"" + Program.uncPdfPrefix + "\\\" DOCTYPE=\"PDF\" />");
                jobTicket.AppendLine("</JOB:DOCOUTPUTS>");
                jobTicket.AppendLine("<JOB:SETTINGS>");
                jobTicket.AppendLine("<JOB:PDFSETTINGS JPEGCOMPRESSIONLEVEL=\"5\" MONOIMAGECOMPRESSION=\"Default\" GRAYSCALE=\"No\" PAGECOMPRESSION=\"Yes\" DOWNSAMPLEIMAGES=\"No\" RESOLUTION=\"1200\" PDFVERSION=\"PDFVersion15\" PDFVERSIONINHERIT=\"No\" PAGES=\"All\" />");
                jobTicket.AppendLine("</JOB:SETTINGS>");
                jobTicket.AppendLine("</JOB>");
                jobTicket.AppendLine("</JOBS>");

                string jobFileName = Program.jobTicketLocation + mostRecentDate.ToString("yyyy-MM-dd") + "_" + kvp.Key + ".xml";
                Console.WriteLine(jobFileName);
                File.WriteAllText(jobFileName, jobTicket.ToString());
            }
            File.WriteAllLines(Program.batchFile, batchLines);
        }
    }
}