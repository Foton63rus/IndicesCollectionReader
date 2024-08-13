﻿namespace IndicesCollectionReader.Entity
{
    public class ChapterWithSections : IChapter// Глава с разделами
    {
        public int Number;
        public int Page;
        public List<Section> Sections = new List<Section>();
    }
}
