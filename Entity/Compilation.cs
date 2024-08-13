namespace IndicesCollectionReader.Entity
{
    public class Compilation //Сборник
    {
        public int Number;
        public int Page;
        public List<IRecord> records = new List<IRecord>();
    }
}
