using System.Windows.Forms;

namespace ChineseTokenizerJieba
{
    internal partial class SettingsForm_ChineseTokenizerJieba : Form
    {


        #region Get and Set Options

        public string TokenizationMode { get; set; }

       #endregion



        public SettingsForm_ChineseTokenizerJieba(string SelectedModel)
        {
            InitializeComponent();

            ModelSelectionBox.Items.Add("1. Precise mode (with HMM)"); //when cutall is false, hmm is true
            ModelSelectionBox.Items.Add("2. Precise mode (no HMM)"); //when cutall is true, hmm is true
            ModelSelectionBox.Items.Add("3. Full Mode (no HMM)"); //when cutall is true, hmm is false
            ModelSelectionBox.Items.Add("4. Search Engine Mode (with HMM)"); //when we use cutforsearch instead of cut
            ModelSelectionBox.Items.Add("5. Search Engine Mode (no HMM)"); //when we use cutforsearch instead of cut

            try
            {
                ModelSelectionBox.SelectedIndex = ModelSelectionBox.FindStringExact(SelectedModel);
            }
            catch
            {
                ModelSelectionBox.SelectedIndex = 0;
            }

        }



       

        private void OKButton_Click(object sender, System.EventArgs e)
        {
            this.TokenizationMode = ModelSelectionBox.SelectedItem.ToString();
            this.DialogResult = DialogResult.OK;
        }
    }
}
