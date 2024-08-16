using IndicesCollectionReader;

PdfFile pdfFile;

while (true)
{
    string input = Console.ReadLine();

    if (string.IsNullOrEmpty(input))
    {
        Console.WriteLine("Укажи путь к файлу");
        continue;
    }

    switch (input)
    {
        default:
            pdfFile = new PdfFile();
            string JSON = pdfFile.Open(input);
            Console.Clear();
            Console.WriteLine("writing json");
            File.WriteAllText($"{input}.json", JSON);
            Console.WriteLine("Done");
            break;
    }
}