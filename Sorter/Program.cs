using System.IO.MemoryMappedFiles;

string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "lab1/data.dat");
string mutexName = "Global\\SortetMutex";

while (true)
{
    try
    {
        using var mmf = MemoryMappedFile.OpenExisting("data.dat");
        //using var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, "data.dat", 0, MemoryMappedFileAccess.ReadWrite);
        using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.ReadWrite);
        using var mutex = new Mutex(false, mutexName);

        try
        {
            FileInfo fileInfo = new(filePath);
            int numberOfIntegers = (int)fileInfo.Length / sizeof(int);
            int[] numbers = new int[numberOfIntegers];

            Console.WriteLine("Press SPACE to start sorting...");

            if (Console.ReadKey(true).Key == ConsoleKey.Spacebar)
            {
                mutex.WaitOne();

                Console.WriteLine("Sorting started...");

                for (int i = 0; i < numberOfIntegers; i++)
                {
                    numbers[i] = accessor.ReadInt32(i * sizeof(int));
                }

                for (int i = 0; i < numbers.Length - 1; i++)
                {
                    for (int j = 0; j < numbers.Length - i - 1; j++)
                    {
                        if (numbers[j] > numbers[j + 1])
                        {
                            (numbers[j + 1], numbers[j]) = (numbers[j], numbers[j + 1]);
                            for (int k = 0; k < numberOfIntegers; k++)
                            {
                                accessor.Write(k * sizeof(int), numbers[k]);
                            }

                            Thread.Sleep(50);
                        }
                    }
                }
            }
        }
        finally
        {
            mutex.ReleaseMutex();
        }

        Console.WriteLine("Sorting completed.");
    }
    catch (Exception e)
    {
        Console.WriteLine("Error: " + e.Message);
    }
}