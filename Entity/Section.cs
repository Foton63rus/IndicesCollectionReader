namespace IndicesCollectionReader.Entity
{
    public class Section //Раздел
    {
        public string Name;
        public int Number;
        public int Page;
        public List<Table> Tables = new List<Table>();
    }
}
