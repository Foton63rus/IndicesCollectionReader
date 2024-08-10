namespace IndicesCollectionReader.Entity
{
    public class Compilation //Сборник
    {

        public int Number { get; set; }
        public int Page;
        public List<Section> Sections = new List<Section>();


    }
}
