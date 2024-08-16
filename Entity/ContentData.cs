namespace IndicesCollectionReader.Entity
{
    public class ContentData
    {
        public string Name;
        public int Page;
        public int Index;

        public override string ToString()
        {
            return $"{Page}/{Index} {Name}";
        }
    }
}
