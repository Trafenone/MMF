using System.IO.MemoryMappedFiles;

string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "lab1/data.dat");
string _mutexName = "Global\\DataFileMutex";
int size = 30;
int minValue = 10;
int maxValue = 100;

var random = new Random();

try
{
    using var mutex = new Mutex(false, _mutexName);

    int[] numbers = GetNumbers(size, minValue, maxValue, random);

    using var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Create, "data.dat", numbers.Length * sizeof(int), MemoryMappedFileAccess.ReadWrite);
    using var accessor = mmf.CreateViewAccessor(0, numbers.Length * sizeof(int), MemoryMappedFileAccess.ReadWrite);


    mutex.WaitOne();

    for (int i = 0; i < numbers.Length; i++)
    {
        accessor.Write(i * sizeof(int), numbers[i]);
    }

    mutex.ReleaseMutex();

    Console.WriteLine("File created and filled with random numbers.");
    Console.ReadLine();
}
catch (Exception exception)
{
    Console.WriteLine("Error: " + exception.Message);
}

static int[] GetNumbers(int size, int minValue, int maxValue, Random random)
{
    int[] numbers = new int[size];
    for (int i = 0; i < numbers.Length; i++)
    {
        numbers[i] = random.Next(minValue, maxValue);
    }

    return numbers;
}