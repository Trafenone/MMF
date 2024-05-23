using System.IO.MemoryMappedFiles;

string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "lab1/data.dat");
string mutexName = "Global\\DataFileMutex";

while (true)
{
    Console.WriteLine("Press SPACE to start sorting in descending order...");

    try
    {
        using var mmf = MemoryMappedFile.OpenExisting("data.dat");
        using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.ReadWrite);
        using var mutex = new Mutex(false, mutexName);

        if (Console.ReadKey(true).Key == ConsoleKey.Spacebar)
        {

            FileInfo fileInfo = new(filePath);
            int numberOfIntegers = (int)fileInfo.Length / sizeof(int);
            int[] numbers = new int[numberOfIntegers];

            Console.WriteLine("Sorting in descending order started...");

            mutex.WaitOne();

            for (int i = 0; i < numberOfIntegers; i++)
            {
                numbers[i] = accessor.ReadInt32(i * sizeof(int));
            }

            for (int i = 0; i < numbers.Length - 1; i++)
            {
                int maxIndex = i;
                for (int j = i + 1; j < numbers.Length; j++)
                {
                    if (numbers[j] > numbers[maxIndex])
                    {
                        maxIndex = j;
                    }
                }
                if (maxIndex != i)
                {
                    (numbers[maxIndex], numbers[i]) = (numbers[i], numbers[maxIndex]);
                    for (int k = 0; k < numberOfIntegers; k++)
                    {
                        accessor.Write(k * sizeof(int), numbers[k]);
                    }
                }

                Thread.Sleep(50);
            }

            mutex.ReleaseMutex();

            Console.WriteLine("Sorting in descending order completed.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }

}