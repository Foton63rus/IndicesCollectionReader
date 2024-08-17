namespace IndicesCollectionReader.Entity
{
    public class ChapterWithCompilations : IChapter// Глава со сборниками
    {
        public string Name;
        public int Number;
        public int Page;
        public List<Compilation> Compilations = new List<Compilation>();
        public ChapterWithCompilations(int number, int page)
        {
            Number = number;
            Page = page;
        }
    }
}
