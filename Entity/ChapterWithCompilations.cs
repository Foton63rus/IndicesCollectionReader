namespace IndicesCollectionReader.Entity
{
    public class ChapterWithCompilations : IChapter// Глава со сборниками
    {
        public int Number;
        public int Page;
        public List<Compilation> compilations = new List<Compilation>();
    }
}
