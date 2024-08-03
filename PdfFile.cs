using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace IndicesCollectionReader
{
    public class PdfFile
    {
        PdfDocument pdf;
        PdfContent pdfContent;
        PdfSource PdfSource;
        
        public string Open(string path)
        {
            ClearPdfData();
            if (path == "1")
            {
                path = @"C:\Users\4ton1\OneDrive\Рабочий стол\Сборник индексов 206_.pdf";
            }
            try
            {
                pdf = PdfDocument.Open(path);
                Console.WriteLine(pdf.GetPage(120).Text);
                return ExtractPdfData(pdf);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private string ExtractPdfData(PdfDocument pdf)
        {
            StringBuilder sb = new StringBuilder();

            if (pdfContent.pageContent == 0) findContentPage();
            if (pdfContent.pageSource  == 0) findSourcePage();

            extractContentData();

            return "";
        } 
        
        private void findContentPage()
        {
            for (int i = 1; i <= pdf.NumberOfPages; i++)
            {
                Regex regexFindContentPage = new Regex( RegexTemplates.Content );
                Match match = regexFindContentPage.Match(pdf.GetPage(i).Text);
                if (match.Success)
                {
                    pdfContent.pageContent = i;
                    break;
                }
            }
            if (pdfContent.pageContent > 0)
            {
                Console.WriteLine($"Содержание находится на {pdfContent.pageContent} странице");
            }
            else
            {
                throw new ArgumentOutOfRangeException("Не возможно определить страницу с содержанием");
            }
        }
        private void findSourcePage()
        {
            string contentText = pdf.GetPage(pdfContent.pageContent).Text;
            Regex regexFindSourcePage = new Regex( RegexTemplates.SourcePage );
            Match match = regexFindSourcePage.Match(contentText);
            if (match.Success)
            {
                int.TryParse(match.Groups[3].Value, out pdfContent.pageSource);
            }
            if(pdfContent.pageSource > 0)
            {
                Console.WriteLine($"Общая часть находится на {pdfContent.pageSource} странице");
            }
            else
            {
                throw new ArgumentOutOfRangeException("Не возможно определить страницу следующую за содержанием");
            }
        }
        private void extractContentData()
        {
            for (int i = pdfContent.pageContent; i <= pdfContent.pageSource - 1; i++) //pdfContent.pageSource
            {
                Regex regexStartContent = new Regex(RegexTemplates.Content);
                Match match = regexStartContent.Match(pdf.GetPage(i).Text);
                if (match.Success && match.Index == 0)
                {
                    extractLinesFromContent(pdf.GetPage(i).Text.Substring(match.Length));
                }
                else
                {
                    Regex regexDigits = new Regex(RegexTemplates.Digits);
                    match = regexDigits.Match(pdf.GetPage(i).Text);
                    if (match.Success && match.Index == 0)
                    {
                        extractLinesFromContent(pdf.GetPage(i).Text.Substring(match.Length));
                    }
                }
            }
        }
        private void extractLinesFromContent( string context)
        {
            Regex regexLine = new Regex(RegexTemplates.ContentLines);
            MatchCollection matches = regexLine.Matches(context);
            if (matches.Count > 0)
            {
                int current = 0;
                foreach (Match match in matches)
                {
                    Console.WriteLine($"{context.Substring(current, match.Index - current)} === {match.Groups[1].Value}");
                    current = match.Index + match.Length;
                }
            }
        }
        private void ClearPdfData()
        {
            if (pdf != null) pdf.Dispose();
            pdfContent = new PdfContent();
            PdfSource = null;
        }
    }
}
