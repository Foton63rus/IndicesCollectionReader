namespace IndicesCollectionReader.Entity
{
    public class ChapterWithCompilations : IChapter// Глава со сборниками
    {
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
