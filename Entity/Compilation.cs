namespace IndicesCollectionReader.Entity
{
    public class Compilation //Сборник
    {
        public string Name;
        public string header;
        public int Number;
        public int Page;
        public List<IRecord> Records = new List<IRecord>();
    }
}
