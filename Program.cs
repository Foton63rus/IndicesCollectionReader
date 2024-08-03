using IndicesCollectionReader;

PdfFile pdfFile;

while(true)
{
    string input = Console.ReadLine();

    if ( string.IsNullOrEmpty(input) )
    {
        Console.WriteLine("Укажи путь к файлу");
        continue;
    }

    switch (input)
    {
        default:
            pdfFile = new PdfFile();
            Console.WriteLine(pdfFile.Open(input));
            break;
    }
}