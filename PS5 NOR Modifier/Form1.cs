using PS5_NOR_Modifier.Common.Helpers;
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

        #region Donations

        /// <summary>
        /// If you modify this app, please leave my credits in, otherwise a little kitten will cry!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label4_Click(object sender, EventArgs e)
        {
            Browser.OpenUrl("https://www.streamelements.com/thecod3r/tip");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Browser.OpenUrl("https://www.streamelements.com/thecod3r/tip");
        }


        #endregion

        private void label15_Click(object sender, EventArgs e)
        {
            Browser.OpenUrl("https://www.consolefix.shop");
        }
    }
}
