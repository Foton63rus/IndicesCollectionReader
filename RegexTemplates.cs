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

        public static string TableHeaderNumCodeKoefKStoimostiPererabotkiVsegoVTCHZPl = @"№\s+п\/п\s+Шифр\s+Коэффициенты\s+к\s+стоимости\s+перебазировки\s+всего\s+в\s+т.ч.\s+з\/пл.";
        public static string TableDataNumCodeKoefKStoimostiPererabotkiVsegoVTCHZPl = @"(\d+)\s+(\d+.\d+-\d+-\d+)\s+(\d+.\d+-\d+-\d+\s+)?(\d+,\d+)\s+(\d+,\d+)\s*";

        public static string TableHeaderNumCodeKoefKStoimostiPererabotkiDopSekcii = @"№\s+п\/п\s+Шифр\s+Коэффициенты\s+к\s+стоимости\s+перебазировки\s+дополнительной\s+секции\s+";
        public static string TableDataNumCodeKoefKStoimostiPererabotkiDopSekcii = TableDataNumCodeKoef;

        public static string TableHeaderNumCodeCommonKoefZPEMMROther = @"№\s+п\/п\s+Шифр\s+Общий\s+коэффи-\s+циент\s+Коэффициенты\s+к\s+статьям\s+затрат\s+ЗП\s+ЭМ\s+МР\s+Прочие\s+затраты\s+";
        public static string TableDataNumCodeKoefx5 = @"(\d+)\s+(\d+.\d+-\d+-\d+)\s+(\d+.\d+-\d+-\d+\s+)?((\d+,\d+)|-)\s+((\d+,\d+)|-)\s+((\d+,\d+)|-)\s+((\d+,\d+)|-)\s+((\d+,\d+)|-)\s+";

        public static string TableHeaderNumCodeKoef = @"№\s+п\/п\s+Шифр\s+Коэффи-\s+циент\s+";
        public static string TableDataNumCodeKoef = @"(\d+)\s+(\d+.\d+-\d+-\d+)\s+(\d+.\d+-\d+-\d+\s+)?(\d+,\d+)\s*";

        public static string TableHeaderNumCodeKoefEMMR = @"№\s+п\/п\s+Шифр\s+Коэффициент\s+ЭМ\s+МР\s+";
        public static string TableDataNumCodeKoefEMMR = @"(\d+)\s+(\d+.\d+-\d+-\d+)\s+(\d+.\d+-\d+-\d+\s+)?((\d+,\d+)|-)\s+((\d+,\d+)|-)\s+";
    }
}
