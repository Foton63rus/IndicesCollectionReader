namespace IndicesCollectionReader.Entity
{
    public class Table
    {
        public string Name;
        public int Page;
        public string Header;
        public List<IRecord> Records = new List<IRecord>();
    }
}
