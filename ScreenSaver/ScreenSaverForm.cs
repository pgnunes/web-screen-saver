using CefSharp.WinForms;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenSaver
{
    public partial class ScreenSaverForm : Form
    {
        #region Win32 API functions

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        #endregion

        private Point mouseLocation;
        private bool previewMode = false;
        private Random rand = new Random();
        private readonly ChromiumWebBrowser browser;
        private ArrayList urlList = new ArrayList();

        public ScreenSaverForm()
        {
            InitializeComponent();
        }

        public ScreenSaverForm(Rectangle Bounds)
        {
            browser = new ChromiumWebBrowser("");
            InitializeComponentBrowser(browser);
            Task.Factory.StartNew(() => LoopURLs());
        }
        public void LoopURLs()
        {
            LoadURLFile();

            while (true)
            {
                foreach (Tuple<int, string> url in urlList)
                {
                    browser.Load(url.Item2);
                    Thread.Sleep(url.Item1 * 1000);
                }
            }

        }

        public void LoadURLFile()
        {
            string urlLine;
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Web_ScreenSaver");
            if (key == null)
            {
                GenerateEmpySampleFile();
                key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Web_ScreenSaver");
            }

            // Read the file contentss
            System.IO.StreamReader file = new System.IO.StreamReader((string)key.GetValue("text"));
            while ((urlLine = file.ReadLine()) != null)
            {
                string[] currURL = urlLine.Trim().Split(' ');
                if (currURL.Length == 2)
                {
                    urlList.Add(Tuple.Create(Int32.Parse(currURL[0]), currURL[1]));
                }
            }
            file.Close();
        }

        public void GenerateEmpySampleFile()
        {
            // set a default on user home
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Web_ScreenSaver");
            string userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string sampleFile = userHome + "\\webscreensaver.txt";
            key.SetValue("text", sampleFile);

            // create the file with sample content
            string[] lines = { "5 https://github.com/pgnunes/web-screen-saver",
                "5 https://play.grafana.org/d/000000029/prometheus-demo-dashboard?orgId=1&refresh=5m",
                "5 https://www.tradingview.com/symbols/EURUSD/",
                "5 https://earth.nullschool.net/",
                "5 https://www.ventusky.com/"};
            System.IO.File.WriteAllLines(@sampleFile, lines);
        }

        public ScreenSaverForm(IntPtr PreviewWndHandle)
        {
            InitializeComponent();

            // Set the preview window as the parent of this window
            SetParent(this.Handle, PreviewWndHandle);

            // Make this a child window so it will close when the parent dialog closes
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000));

            // Place our window inside the parent
            Rectangle ParentRect;
            GetClientRect(PreviewWndHandle, out ParentRect);
            Size = ParentRect.Size;
            Location = new Point(0, 0);

            // Make text smaller
            textLabel.Font = new System.Drawing.Font("Arial", 6);

            previewMode = true;
        }

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            LoadSettings();

            Cursor.Hide();
            TopMost = true;

            moveTimer.Interval = 1000;
            moveTimer.Tick += new EventHandler(moveTimer_Tick);
            moveTimer.Start();
        }

        private void moveTimer_Tick(object sender, System.EventArgs e)
        {
            // Move text to new location
            textLabel.Left = rand.Next(Math.Max(1, Bounds.Width - textLabel.Width));
            textLabel.Top = rand.Next(Math.Max(1, Bounds.Height - textLabel.Height));
        }

        private void LoadSettings()
        {
            // Use the string from the Registry if it exists
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Demo_ScreenSaver");
            if (key == null)
                textLabel.Text = "Web Screen Saver";
            else
                textLabel.Text = (string)key.GetValue("text");
        }

        private void ScreenSaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!previewMode)
            {
                if (!mouseLocation.IsEmpty)
                {
                    // Terminate if mouse is moved a significant distance
                    if (Math.Abs(mouseLocation.X - e.X) > 5 ||
                        Math.Abs(mouseLocation.Y - e.Y) > 5)
                        Application.Exit();
                }

                // Update current mouse location
                mouseLocation = e.Location;
            }
        }

        private void ScreenSaverForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }

        private void ScreenSaverForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }
    }
}
