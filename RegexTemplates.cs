namespace IndicesCollectionReader
{
    public static class RegexTemplates
    {
        public static string Content = @"\s?\d?\s?Содержание(\s+)";
        public static string SourcePage = @"(ОБЩАЯ\s+ЧАСТЬ)\s+(\.+)\s+(\d+)";
        public static string ContentLines = @"\.+\s+(\d+)\s*";
        public static string Digits = @"\s?\d+\s+";
        public static string SourceHeader = @"\s?\d+\s\w+\s[(]\w+[)]\s\w+\s\w+[,]\s+\d+[/]\d+\s+";
        public static string ChapterNumber = @"^Глава\s+(\d+).";
        public static string SectionNumber = @"^Раздел\s+(\d+).";
        public static string CompilationNumber = @"^Сборник\s+(\d+)\s?.\s+";
        public static string TableNumberHeader = @"^\s?(\d+).\s+(.+)";

        public static string TableHeaderNumCodeKoef = @"^№\s+п/п\s+Шифр\s+Коэффи-\s+циент\s+";
    }
}
