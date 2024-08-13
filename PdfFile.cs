using IndicesCollectionReader.Entity;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace IndicesCollectionReader
{
    public class PdfFile
    {
        PdfDocument pdf;
        PdfContent pdfContent;
        Dictionary<string, int> sourceContentPages = new Dictionary<string, int>(); //Содержание: Текст и номер страницы
        Indexes indexes = new Indexes();   // Объектная модель файла

        (int, int) chapterCashData = (0, 0);
        ChapterWithCompilations currentChapterWithCompilations = null;
        ChapterWithSections currentChapterWithSections = null;
        Compilation currentCompilation = null;
        Section currentSection = null;

        public string Open(string path)
        {
            ClearPdfData();
            if (path == "1")
            {
                path = @"C:\Users\4ton1\OneDrive\Рабочий стол\Сборник индексов 212_.pdf";
            }
            try
            {
                pdf = PdfDocument.Open(path);
                Console.WriteLine(pdf.GetPage(2).Text);
                return ExtractPdfData(pdf);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private string ExtractPdfData(PdfDocument pdf)
        {
            if (pdfContent.pageContent == 0) findContentPage();
            if (pdfContent.pageSource  == 0) findSourcePage();

            extractContentData();   //Извлечение данных из содержания файла
            buildContentModel();    //Создание объектной модели из содержания файла

            string s = JsonConvert.SerializeObject(indexes, Formatting.Indented);

            return s;
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
        private void buildContentModel()
        {
            currentChapterWithCompilations = null;
            currentChapterWithSections = null;
            currentCompilation = null;
            currentSection = null;

            foreach (var context in sourceContentPages)
            {

                if (context.Key.StartsWith("Глава"))
                {
                    buildChapter(context);
                }
                else if (context.Key.StartsWith("Раздел"))
                {
                    buildSection(context);
                }
                else if (context.Key.StartsWith("Сборник"))
                {
                    buildCompilation(context);
                }
                else
                {

                }
            }
        }
        private void buildChapter(KeyValuePair<string, int> context)
        {
            int number = 0;
            Regex regex = new Regex(RegexTemplates.ChapterNumber);
            Match match = regex.Match(context.Key);
            if (match.Success)
            {
                int.TryParse(match.Groups[1].Value, out number);
            }
            int page = context.Value;
            chapterCashData = (number, page);

            currentChapterWithCompilations = null;
            currentChapterWithSections = null;
            currentCompilation = null;
            currentSection = null;
        }
        private void buildSection(KeyValuePair<string, int> context)
        {
            if (chapterCashData != (0, 0))
            {
                currentChapterWithCompilations = null;
                currentChapterWithSections = new ChapterWithSections();
                currentChapterWithSections.Number = chapterCashData.Item1;
                currentChapterWithSections.Page = chapterCashData.Item2;
                indexes.chapters.Add(currentChapterWithSections);
                chapterCashData = (0, 0);
            }
            currentSection = new Section();
            Regex regex = new Regex(RegexTemplates.SectionNumber);
            Match match = regex.Match(context.Key);
            if (match.Success)
            {
                int.TryParse(match.Groups[1].Value, out currentSection.Number);
            }
            currentSection.Page = context.Value;
            currentChapterWithSections.Sections.Add(currentSection);
        }
        private void buildCompilation(KeyValuePair<string, int> context)
        {
            if (chapterCashData != (0, 0))
            {
                currentChapterWithSections = null;
                currentChapterWithCompilations = new ChapterWithCompilations();
                currentChapterWithCompilations.Number = chapterCashData.Item1;
                currentChapterWithCompilations.Page = chapterCashData.Item2;
                indexes.chapters.Add(currentChapterWithCompilations);
                chapterCashData = (0, 0);
            }
            currentCompilation = new Compilation();
            Regex regex = new Regex(RegexTemplates.CompilationNumber);
            Match match = regex.Match(context.Key);
            if (match.Success)
            {
                int.TryParse(match.Groups[1].Value, out currentCompilation.Number);
            }
            currentCompilation.Page = context.Value;
            currentChapterWithCompilations.compilations.Add(currentCompilation);
        }
        private void buildTable(KeyValuePair<string, int> context)
        {

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
                    int page;
                    int.TryParse(match.Groups[1].Value, out page);
                    if (!sourceContentPages.ContainsKey(context.Substring(current, match.Index - current)))
                    {
                        sourceContentPages.Add($"{context.Substring(current, match.Index - current)}", page);
                    }
                    Console.WriteLine($"{context.Substring(current, match.Index - current)} === {match.Groups[1].Value}");
                    current = match.Index + match.Length;
                }
            }
        }
        private void ClearPdfData()
        {
            
            indexes = new Indexes();
            if (sourceContentPages != null) { 
                sourceContentPages.Clear(); 
                sourceContentPages = new Dictionary<string, int>(); 
            }
            if (pdf != null) pdf.Dispose();
            pdfContent = new PdfContent();
        }
    }
}
