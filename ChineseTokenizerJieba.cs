using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using PluginContracts;
using OutputHelperLib;
using JiebaNet.Segmenter;


namespace ChineseTokenizerJieba
{
    public class ChineseTokenizerJieba : Plugin
    {


        public string[] InputType { get; } = { "String" };
        public string OutputType { get; } = "Tokens";

        public Dictionary<int, string> OutputHeaderData { get; set; } = new Dictionary<int, string>() { { 0, "TokenizedText" } };
        public bool InheritHeader { get; } = false;

        #region Plugin Details and Info

        public string PluginName { get; } = "Chinese (Jieba)";
        public string PluginType { get; } = "Tokenizers/Segmenters";
        public string PluginVersion { get; } = "1.0.1";
        public string PluginAuthor { get; } = "Ryan L. Boyd (ryan@ryanboyd.io)";
        public string PluginDescription { get; } = "Tokenizes Chinese texts using Jieba (结巴). Built on top of the 0.39.1 port to .NET:" + Environment.NewLine + Environment.NewLine +
                                                   "https://github.com/anderscui/jieba.NET" + Environment.NewLine +
                                                   "There are several combinations of modes that can be used for this plugin. Briefly described:" + Environment.NewLine + Environment.NewLine +
                                                   "1. Precise versus Full Mode. Precise mode attempts to tokenize texts as accurately as possible and is best suited for text analysis. Full mode is faster, but is unable to resolve ambiguity." + Environment.NewLine + Environment.NewLine +
                                                   "2. HMM. Uses a Hidden Markov Model to attempt to split unregistered/unrecognized words." + Environment.NewLine + Environment.NewLine +
                                                   "3. Search Mode. Similar to Precise Mode, but best used for improving recall rates. Most suitable for search engine segmentation.";
        public string PluginTutorial { get; } = "Coming Soon";
        public bool TopLevel { get; } = false;

        private JiebaSegmenter jiebaSegmenter { get; set; }

        private string TokenizationMode { get; set; } = "3. Full Mode (no HMM)";
        private bool UseHMM { get; set; } = true;
        private bool FullMode { get; set; } = false; //note that if we're using Full Mode, we're necessarily not using Precise Mode
        private bool SearchMode { get; set; } = false;



        public Icon GetPluginIcon
        {
            get
            {
                return Properties.Resources.icon;
            }
        }

        #endregion



        public void ChangeSettings()
        {

            using (var form = new SettingsForm_ChineseTokenizerJieba(TokenizationMode))
            {


                form.Icon = Properties.Resources.icon;
                form.Text = PluginName;

                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    TokenizationMode = form.TokenizationMode;
                }

                switch (TokenizationMode)
                {
                    case "1. Precise mode (with HMM)": //when cutall is false, hmm is true
                        UseHMM = true;
                        FullMode = false;
                        SearchMode = false;
                        break;
                    case "2. Precise mode (no HMM)": //when cutall is true, hmm is true
                        UseHMM = false;
                        FullMode = false;
                        SearchMode = false;
                        break;
                    case "3. Full Mode (no HMM)": //when cutall is true, hmm is false
                        UseHMM = false;
                        FullMode = true;
                        SearchMode = false;
                        break;
                    case "4. Search Engine Mode (with HMM)": //when we use cutforsearch instead of cut
                        UseHMM = true;
                        FullMode = false;
                        SearchMode = true;
                        break;
                    case "5. Search Engine Mode (no HMM)":
                        UseHMM = false;
                        FullMode = false;
                        SearchMode = true;
                        break;

                }

            }

        }





        public Payload RunPlugin(Payload Input)
        {



            Payload pData = new Payload();
            pData.FileID = Input.FileID;
            pData.SegmentID = Input.SegmentID;


            for (int i = 0; i < Input.StringList.Count; i++)
            {


                string[] jiebaTokens = new string[] { };
                //I need to find the smartest tokenization option here. The docs say that the .cut method is best used for sentences,
                //which means that I need to find a good way to split texts into sentences (e.g., punctuation?)
                if (!SearchMode)
                {
                    jiebaTokens = jiebaSegmenter.Cut(Input.StringList[i], cutAll: FullMode, hmm: UseHMM ).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                }
                else
                {
                    jiebaTokens = jiebaSegmenter.CutForSearch(Input.StringList[i], hmm: UseHMM).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                }
                
                //takes all of the tokens and puts then in a list
                //List<string> jiebaTokenList = new List<string>();
                //foreach (string token in jiebaTokens)
                //{
                //    if (!string.IsNullOrWhiteSpace(token)) jiebaTokenList.Add(token);
                //}
                pData.StringArrayList.Add(jiebaTokens);
                pData.SegmentNumber.Add(Input.SegmentNumber[i]);
            }

            return (pData);

        }



        public void Initialize()
        {
            jiebaSegmenter = new JiebaSegmenter();
        }

        public bool InspectSettings()
        {
            return true;
        }



        public Payload FinishUp(Payload Input)
        {
            return (Input);
        }





        #region Import/Export Settings
        public void ImportSettings(Dictionary<string, string> SettingsDict)
        {
            TokenizationMode = SettingsDict["TokenizationMode"];
            UseHMM = Boolean.Parse(SettingsDict["UseHMM"]);
            FullMode = Boolean.Parse(SettingsDict["FullMode"]);
            SearchMode = Boolean.Parse(SettingsDict["SearchMode"]);
        }

        public Dictionary<string, string> ExportSettings(bool suppressWarnings)
        {
            Dictionary<string, string> SettingsDict = new Dictionary<string, string>();
            SettingsDict.Add("TokenizationMode", TokenizationMode);
            SettingsDict.Add("UseHMM", UseHMM.ToString());
            SettingsDict.Add("FullMode", FullMode.ToString());
            SettingsDict.Add("SearchMode", SearchMode.ToString());
            return (SettingsDict);
        }
        #endregion




    }
}
