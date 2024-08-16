using IndicesCollectionReader.Entity;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace IndicesCollectionReader
{
    public class PdfFile
    {
        //пока что не ведется импорт нормативных таблиц
        PdfDocument pdf;
        ContentExtractor content;

        //Dictionary<string, int> sourceContentPages = new Dictionary<string, int>(); //Содержание: Текст и номер страницы
        Indexes indexes = new Indexes();   // Объектная модель файла

        (int, int) chapterCashData = (0, 0);
        ChapterWithCompilations currentChapterWithCompilations = null;
        ChapterWithSections currentChapterWithSections = null;
        Compilation currentCompilation = null;
        Section currentSection = null;
        Table currentTable = null;

        public string Open(string path)
        {
            ClearPdfData();
            try
            {
                pdf = PdfDocument.Open(path);
                //Console.WriteLine(pdf.GetPage(12).Text);
                return ExtractPdfData(pdf);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private string ExtractPdfData(PdfDocument pdf)
        {

            content = new ContentExtractor(pdf);   //Извлечение данных из содержания файла
            buildContentModel();    //Создание объектной модели из содержания файла

            string s = JsonConvert.SerializeObject(indexes, Formatting.Indented);

            return s;
        }

        private void buildContentModel()
        {
            currentChapterWithCompilations = null;
            currentChapterWithSections = null;
            currentCompilation = null;
            currentSection = null;
            currentTable = null;

            foreach (var context in content.contentData)
            {
                if (context.Name.StartsWith("ОБЩАЯ ЧАСТЬ"))
                {
                    //Можно предусмотреть выгрузку данных из общей части
                }
                else if (context.Name.StartsWith("Глава"))
                {
                    buildChapter(context);
                }
                else if (context.Name.StartsWith("Раздел"))
                {
                    buildSection(context);
                }
                else if (context.Name.StartsWith("Сборник"))
                {
                    buildCompilation(context);
                }
                else if (context.Name.StartsWith("Нормативная"))
                {
                    /*Можно добавить выгрузку нормативные таблицы по применению норм накладных расходов, сметной прибыли и
                    коэффициентов, учитывающих дополнительные затраты, связанные с производством работ в
                    зимнее время*/
                }
                else
                {
                    //остается только таблицы в разделах, начинающиеся с номеров
                    buildTable(context);
                }
            }
        }
        private void buildChapter(ContentData context)
        {
            int number = 0;
            Regex regex = new Regex(RegexTemplates.ChapterNumber);
            Match match = regex.Match(context.Name);
            if (match.Success)
            {
                int.TryParse(match.Groups[1].Value, out number);
            }
            int page = context.Page;
            chapterCashData = (number, page);

            currentChapterWithCompilations = null;
            currentChapterWithSections = null;
            currentCompilation = null;
            currentSection = null;
            currentTable = null;
        }
        private void buildSection(ContentData context)
        {
            if (chapterCashData != (0, 0))
            {
                currentChapterWithCompilations = null;
                currentChapterWithSections = new ChapterWithSections(chapterCashData.Item1, chapterCashData.Item2);
                indexes.Chapters.Add(currentChapterWithSections);
                chapterCashData = (0, 0);
            }
            currentSection = new Section();
            currentTable = null;
            Regex regex = new Regex(RegexTemplates.SectionNumber);
            Match match = regex.Match(context.Name);
            if (match.Success)
            {
                int.TryParse(match.Groups[1].Value, out currentSection.Number);
            }
            currentSection.Page = context.Page;
            currentChapterWithSections.Sections.Add(currentSection);
        }
        private void buildCompilation(ContentData context)
        {
            if (chapterCashData != (0, 0))
            {
                currentChapterWithSections = null;
                currentChapterWithCompilations = new ChapterWithCompilations(chapterCashData.Item1, chapterCashData.Item2);
                indexes.Chapters.Add(currentChapterWithCompilations);
                chapterCashData = (0, 0);
            }
            currentCompilation = new Compilation();
            Regex regex = new Regex(RegexTemplates.CompilationNumber);
            Match match = regex.Match(context.Name);
            if (match.Success)
            {
                int.TryParse(match.Groups[1].Value, out currentCompilation.Number);
            }
            currentCompilation.Page = context.Page;
            currentChapterWithCompilations.Compilations.Add(currentCompilation);
        }
        private void buildTable(ContentData context)
        {
            Regex regex = new Regex(RegexTemplates.TableNumberHeader);
            Match match = regex.Match(context.Name);
            currentTable = new Table();
            if (match.Success)
            {
                int.TryParse(match.Groups[1].Value, out currentTable.Number);
            }
            currentTable.Name = context.Name.Trim();
            currentTable.Page = context.Page;
            currentSection.Tables.Add(currentTable);

            try
            {
                //extractRecords(currentTable.Page, currentTable.Name);    //Извлечение данных таблиц и сборников
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
        private void extractRecords(int page, string tableHeader)
        {
            int currentPage = currentTable.Page;
            Regex regex = new Regex(tableHeader + @"\s+");
            Match match = regex.Match(pdf.GetPage(currentPage).Text);
            if (match.Success)
            {
                string text = pdf.GetPage(currentPage).Text;
                string tableDirty = text.Substring(match.Index + match.Length);
                regex = new Regex(RegexTemplates.TableHeaderNumCodeKoef);
                match = regex.Match(tableDirty);
                if (match.Success)
                {
                    tableDirty = text.Substring(match.Index + match.Length);
                }
                Console.WriteLine(tableDirty);
            }
            for (int i = currentTable.Page; i <= pdf.NumberOfPages; i++)
            {

            }
        }
        
        private void ClearPdfData()
        {
            indexes = new Indexes();
            if (pdf != null) pdf.Dispose();
        }
    }
}
