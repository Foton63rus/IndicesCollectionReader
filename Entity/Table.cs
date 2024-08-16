namespace IndicesCollectionReader.Entity
{
    public class Table
    {
        public string Name;
        public int Number;
        public int Page;
        public List<IRecord> Records = new List<IRecord>();
        /*public Table(int number, int page)
        { 
            Number = number;
            Page = page;
        }*/
    }
}
