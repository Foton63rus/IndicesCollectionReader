using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace IndicesCollectionReader.Entity
{
    public class ContentExtractor : IDisposable
    {
        /// <summary>
        /// 
        /// Объект извлекает данные только содержания !!!
        /// 
        /// pageContent - страница с началом содержания
        /// pageSource - страница с началом данных
        /// 
        /// contentData - строки содержания в формате
        /// <НОМЕР СТРОКИ>/<ИНДЕКС НАЧАЛА СТРОКИ НА СТРАНИЦЕ> <ИМЯ>
        /// $"{Page}/{Index} {Name}"
        /// 
        /// </summary>
        
        public int pageContent = 0;
        public int pageSource = 0;

        PdfDocument pdf;

        public List<ContentData> contentData = new List<ContentData>();

        public ContentExtractor(PdfDocument pdf)
        {
            this.pdf = pdf;
            Init(pdf);
            this.pdf = null;
        }

        private void Init(PdfDocument pdf) 
        {
            findContentPage();  //Находим страницу с началом содержания
            findSourcePage();   //Находим страницу с началом данных

            extractContentData();   //Извлечение данных из содержания файла
            findHeaderStrokeIndexes();  //Нахождение индексов начала имени на странице
        }

        private void findContentPage()
        {
            for (int i = 1; i <= pdf.NumberOfPages; i++)
            {
                Regex regexFindContentPage = new Regex(RegexTemplates.Content);
                Match match = regexFindContentPage.Match(pdf.GetPage(i).Text);
                if (match.Success)
                {
                    pageContent = i;
                    break;
                }
            }
            if (pageContent > 0)
            {
                Console.WriteLine($"Содержание находится на {pageContent} странице");
            }
            else
            {
                throw new ArgumentOutOfRangeException("Не возможно определить страницу с содержанием");
            }
        }
        private void findSourcePage()
        {
            string contentText = pdf.GetPage(pageContent).Text;
            Regex regexFindSourcePage = new Regex(RegexTemplates.SourcePage);
            Match match = regexFindSourcePage.Match(contentText);
            if (match.Success)
            {
                int.TryParse(match.Groups[3].Value, out pageSource);
            }
            if (pageSource > 0)
            {
                Console.WriteLine($"Общая часть находится на {pageSource} странице");
            }
            else
            {
                throw new ArgumentOutOfRangeException("Не возможно определить страницу следующую за содержанием");
            }
        }

        private void extractContentData()
        {
            for (int i = pageContent; i <= pageSource - 1; i++)
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

        private void extractLinesFromContent(string context)
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
                    string name = context.Substring(current, match.Index - current);
                    if (contentData.Any(x => x.Name == name))
                    {
                        //Уже содержит такое название
                    }
                    else
                    {
                        ContentData data = new ContentData();
                        data.Name = name;
                        data.Page = page;
                        contentData.Add(data);
                    }
                    current = match.Index + match.Length;
                }
            }
        }

        private void findHeaderStrokeIndexes()
        {
            foreach (ContentData data in contentData)
            {
                string text = pdf.GetPage(data.Page).Text;
                data.Index = text.IndexOf(data.Name.Trim());

                Console.WriteLine(data);
            }
        }

        public void Dispose()
        {
            this.pdf = null;
        }
    }
}
