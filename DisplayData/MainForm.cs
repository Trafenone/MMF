using System.Timers;
using System.IO.MemoryMappedFiles;

namespace DisplayData
{
    public partial class MainForm : Form
    {
        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "lab1/data.dat");
        private readonly string _mutexName = "Global\\DataFileMutex";
        private readonly System.Timers.Timer _timer;
        private int[] _numbers;

        public MainForm()
        {
            InitializeComponent();

            Text = "Data Display";
            Width = 1000;
            Height = 660;
            DoubleBuffered = true;

            Font = new Font("Arial", 14, FontStyle.Regular);

            _timer = new System.Timers.Timer();
            _timer.Interval = 500;
            _timer.Elapsed += OnTimedEvent;
            _timer.Start();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e) => Invalidate();

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(Color.DarkViolet);

            try
            {
                using var mmf = MemoryMappedFile.OpenExisting("data.dat");
                using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
                using var mutex = new Mutex(false, _mutexName);

                mutex.WaitOne();

                try
                {
                    FileInfo fileInfo = new(_filePath);
                    int numberOfIntegers = (int)fileInfo.Length / sizeof(int);
                    _numbers = new int[numberOfIntegers];

                    for (int i = 0; i < numberOfIntegers; i++)
                    {
                        _numbers[i] = accessor.ReadInt32(i * sizeof(int));
                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                }

                int x = 10;
                int y = 10;
                int barHeight = 20;

                foreach (var number in _numbers)
                {
                    e.Graphics.DrawString(new string('*', number), Font, Brushes.Azure, x, y);
                    y += barHeight;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
