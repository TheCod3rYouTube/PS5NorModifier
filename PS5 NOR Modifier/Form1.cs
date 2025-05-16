using System.Diagnostics;
using System.Runtime.InteropServices;

using PS5_NOR_Modifier.UserControls.Events;

namespace PS5_NOR_Modifier
{
    public partial class fMainForm : Form
    {
        public fMainForm()
        {
            InitializeComponent();

            LoadNorModifierControl();
            LoadUARTControl();
        }

        private void LoadUARTControl()
        {
            ucUART.statusUpdateEvent += ucUART_statusUpdateEvent;
        }

        private void LoadNorModifierControl()
        {
            ucNORModifier.statusUpdateEvent += ucNORModifier_statusUpdateEvent;
        }

        private void ucNORModifier_statusUpdateEvent(object? sender, StatusUpdateEventArgs e)
        {
            toolStripStatusLabel1.Text = e.Text;
        }

        private void ucUART_statusUpdateEvent(object? sender, StatusUpdateEventArgs e)
        {
            toolStripStatusLabel1.Text = e.Text;
        }

        /// <summary>
        /// Lauinches a URL in a new window using the default browser...
        /// </summary>
        /// <param name="url">The URL you want to launch</param>
        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        #region Donations

        /// <summary>
        /// If you modify this app, please leave my credits in, otherwise a little kitten will cry!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label4_Click(object sender, EventArgs e)
        {
            OpenUrl("https://www.streamelements.com/thecod3r/tip");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            OpenUrl("https://www.streamelements.com/thecod3r/tip");
        }


        #endregion

        private void label15_Click(object sender, EventArgs e)
        {
            OpenUrl("https://www.consolefix.shop");
        }
    }
}
