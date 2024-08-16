using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace IndicesCollectionReader.Entity
{
    public  class ContentModelBuilder
    {
        PdfDocument pdf;
        ContentExtractor content;
        Indexes indexes = new Indexes();   // Объектная модель файла

        (int, int) chapterCashData = (0, 0);
        ChapterWithCompilations currentChapterWithCompilations = null;
        ChapterWithSections currentChapterWithSections = null;
        Compilation currentCompilation = null;
        Section currentSection = null;
        Table currentTable = null;

        public ContentModelBuilder(PdfDocument pdfDocument, ContentExtractor contentExtractor)
        {
            this.pdf = pdfDocument;
            this.content = contentExtractor;
            buildContentModel();
        }

        private void buildContentModel()
        {
            currentChapterWithCompilations = null;
            currentChapterWithSections = null;
            currentCompilation = null;
            currentSection = null;
            currentTable = null;

            //foreach (var context in content.contentData)
            for (int i = 0; i < content.contentData.Count; i++)
            {
                ContentData context = content.contentData[i];
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
                    buildTable(i);
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
        private void buildTable(int indexData)
        {
            List<IRecord> records = new List<IRecord>();
            ContentData context = content.contentData[indexData];
            int fromIndex = 0;
            int toIndex = 0;
            int lastPage = 0;
            int lastIndex = 0;
            if (indexData < content.contentData.Count - 1)
            {
                ContentData nextContext = content.contentData[indexData + 1];
                lastPage = nextContext.Page;
                lastIndex = nextContext.Index;
            }
            else
            {
                lastPage = pdf.NumberOfPages;
                lastIndex = pdf.GetPage(lastPage).Text.Length;
            }
            StringBuilder sbRawText = new StringBuilder();
            for (int i = context.Page; i <= lastPage; i++)
            {
                string pageText = pdf.GetPage(i).Text;
                if (i == context.Page)
                {
                    fromIndex = context.Index;
                }
                else
                {
                    fromIndex = 0;
                }
                if(i == lastPage)
                {
                    toIndex = lastIndex;
                }
                else
                {
                    toIndex = pageText.Length;
                }
                sbRawText.Append(pageText.Substring(fromIndex, toIndex - fromIndex));
            }
            Regex regex = new Regex(RegexTemplates.TableHeaderNumCodeKoef);
            Match match = regex.Match(sbRawText.ToString());
            if (match.Success)
            {
                regex = new Regex(RegexTemplates.TableDataNumCodeKoef);
                MatchCollection matches = regex.Matches(sbRawText.ToString());
                foreach (Match m in matches)
                {
                    RecordNumCodeKoef record = new RecordNumCodeKoef();
                    record.Number = m.Groups[1].Value.Trim();
                    record.Code1 = m.Groups[2].Value.Trim();
                    if (m.Groups.ContainsKey("3")) record.Code2 = m.Groups[3].Value.Trim();
                    record.Koef = m.Groups[4].Value.Trim();
                    records.Add(record);
                }
            }
            if(currentCompilation != null)
            {
                currentCompilation.Records = records;
            }else
            {
                Table table = new Table();
                table.Name = context.Name;
                table.Page = context.Page;
                table.Records = records;
                currentSection.Tables.Add(table);
            }
            //Console.WriteLine(sbRawText.ToString());
        }
        public string GetJSON()
        {
            return JsonConvert.SerializeObject(indexes, Formatting.Indented);
        }

        public Indexes GetModel()
        {
            return indexes;
        }
    }
}
