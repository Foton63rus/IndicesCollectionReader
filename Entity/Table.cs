namespace IndicesCollectionReader.Entity
{
    public class Table
    {
        public int Number;
        public int Page;
        public List<IRecord> records = new List<IRecord>();
    }
}
