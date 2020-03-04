using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace ScreenSaver
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Web_ScreenSaver");
            if (key == null)
                textBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\webscreensaver.txt";
            else
                textBox.Text = (string)key.GetValue("text");
        }

        private void SaveSettings()
        {
            // Create or get existing subkey
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Web_ScreenSaver");
            key.SetValue("text", textBox.Text);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
