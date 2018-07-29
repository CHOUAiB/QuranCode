using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.IO;
using Model;

public partial class MainForm : Form
{
    private Client m_client = null;
    private List<List<Word>> m_sentences = null;
    private string m_numerology_system_name = "Simplified29_Alphabet_Primes1";

    public MainForm()
    {
        InitializeComponent();
    }
    private void MainForm_Load(object sender, EventArgs e)
    {
        if (m_client == null)
        {
            m_client = new Client(m_numerology_system_name);
            if (m_client != null)
            {
                m_client.BuildSimplifiedBook(m_client.NumerologySystem.TextMode, true, false, false, false);
            }
        }

        UpdateNumberType(NumberTypeLabel);
    }
    private void AddPositionsAndDistancesCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        bool is_checked = AddPositionsAndDistancesCheckBox.Checked;
        if (m_client != null)
        {
            m_client.NumerologySystem.AddToLetterLNumber = is_checked;
            m_client.NumerologySystem.AddToLetterWNumber = is_checked;
            m_client.NumerologySystem.AddToLetterVNumber = is_checked;
            m_client.NumerologySystem.AddToLetterCNumber = is_checked;
            m_client.NumerologySystem.AddToLetterLDistance = is_checked;
            m_client.NumerologySystem.AddToLetterWDistance = is_checked;
            m_client.NumerologySystem.AddToLetterVDistance = is_checked;
            m_client.NumerologySystem.AddToLetterCDistance = is_checked;
            m_client.NumerologySystem.AddToWordWNumber = is_checked;
            m_client.NumerologySystem.AddToWordVNumber = is_checked;
            m_client.NumerologySystem.AddToWordCNumber = is_checked;
            m_client.NumerologySystem.AddToWordWDistance = is_checked;
            m_client.NumerologySystem.AddToWordVDistance = is_checked;
            m_client.NumerologySystem.AddToWordCDistance = is_checked;
            m_client.NumerologySystem.AddToVerseVNumber = is_checked;
            m_client.NumerologySystem.AddToVerseCNumber = is_checked;
            m_client.NumerologySystem.AddToVerseVDistance = is_checked;
            m_client.NumerologySystem.AddToVerseCDistance = is_checked;
            m_client.NumerologySystem.AddToChapterCNumber = is_checked;
        }
    }

    private RightToLeft m_concatenation_direction = RightToLeft.Yes;
    private void ConcatenationDirectionLabel_Click(object sender, EventArgs e)
    {
        if (m_concatenation_direction == RightToLeft.Yes)
        {
            m_concatenation_direction = RightToLeft.No;
            ConcatenationDirectionLabel.Text = "-->";
            ToolTip.SetToolTip(ConcatenationDirectionLabel, "concatenate letter values left to right");
        }
        else
        {
            m_concatenation_direction = RightToLeft.Yes;
            ConcatenationDirectionLabel.Text = "<--";
            ToolTip.SetToolTip(ConcatenationDirectionLabel, "concatenate letter values right to left");
        }
    }
    private NumberType m_number_type = NumberType.Prime;
    private void NumberTypeLabel_Click(object sender, EventArgs e)
    {
        UpdateNumberType(sender as Control);
    }
    private void UpdateNumberType(Control control)
    {
        if (control == null) return;

        if (control.Text == "")
        {
            m_number_type = NumberType.Prime;
            control.Text = "P";
            control.ForeColor = Numbers.GetNumberTypeColor(19L);
            ToolTip.SetToolTip(control, "prime concatenated letter values");
        }
        else if (control.Text == "P")
        {
            m_number_type = NumberType.AdditivePrime;
            control.Text = "AP";
            control.ForeColor = Numbers.GetNumberTypeColor(47L);
            ToolTip.SetToolTip(control, "additive prime concatenated letter values");
        }
        else if (control.Text == "AP")
        {
            m_number_type = NumberType.NonAdditivePrime;
            control.Text = "XP";
            control.ForeColor = Numbers.GetNumberTypeColor(19L);
            ToolTip.SetToolTip(control, "non-additive prime concatenated letter values");
        }
        else if (control.Text == "XP")
        {
            m_number_type = NumberType.Composite;
            control.Text = "C";
            control.ForeColor = Numbers.GetNumberTypeColor(14L);
            ToolTip.SetToolTip(control, "composite concatenated letter values");
        }
        else if (control.Text == "C")
        {
            m_number_type = NumberType.AdditiveComposite;
            control.Text = "AC";
            control.ForeColor = Numbers.GetNumberTypeColor(114L);
            ToolTip.SetToolTip(control, "additive composite concatenated letter values");
        }
        else if (control.Text == "AC")
        {
            m_number_type = NumberType.NonAdditiveComposite;
            control.Text = "XC";
            control.ForeColor = Numbers.GetNumberTypeColor(25L);
            ToolTip.SetToolTip(control, "non-additive composite concatenated letter values");
        }
        else if (control.Text == "XC")
        {
            m_number_type = NumberType.Odd;
            control.Text = "O";
            control.ForeColor = Numbers.GetNumberTypeColor(0L);
            ToolTip.SetToolTip(control, "odd concatenated letter values");
        }
        else if (control.Text == "O")
        {
            m_number_type = NumberType.Even;
            control.Text = "E";
            control.ForeColor = Numbers.GetNumberTypeColor(0L);
            ToolTip.SetToolTip(control, "even concatenated letter values");
        }
        else if (control.Text == "E")
        {
            //m_number_type = NumberType.Natural;
            //control.Text = "";
            //control.ForeColor = Numbers.GetNumberTypeColor(0L);
            //ToolTip.SetToolTip(control, "all values");
            m_number_type = NumberType.Prime;
            control.Text = "P";
            control.ForeColor = Numbers.GetNumberTypeColor(19L);
            ToolTip.SetToolTip(control, "prime concatenated letter values");
        }
    }

    private void GenerateButton_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            ListView.Items.Clear();

            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    if (m_client.Book.Chapters[0] != null)
                    {
                        List<Verse> verses = m_client.Book.Chapters[0].Verses;
                        if (verses != null)
                        {
                            string filename = Globals.DATA_FOLDER + "/" + "quran-words.txt";
                            if (File.Exists(filename))
                            {
                                List<string> quran_words = FileHelper.LoadLines(filename);

                                List<Word> words = new List<Word>();
                                foreach (Verse verse in verses)
                                {
                                    words.AddRange(verse.Words);
                                }

                                WordSubsetFinder subset_finder = new WordSubsetFinder(words);
                                m_sentences = subset_finder.Find(verses.Count, words.Count);
                                if (m_sentences != null)
                                {
                                    for (int i = 0; i < m_sentences.Count; i++)
                                    {
                                        StringBuilder str = new StringBuilder();
                                        long value = 0L;
                                        foreach (Word word in m_sentences[i])
                                        {
                                            str.Append(word.Text + " ");
                                            value += m_client.CalculateValue(word);
                                        }
                                        if (str.Length > 1)
                                        {
                                            str.Remove(str.Length - 1, 1);
                                        }

                                        string text = "";
                                        if (m_client.NumerologySystem != null)
                                        {
                                            Dictionary<char, long> letter_dictionary = m_client.NumerologySystem.LetterValues;
                                            if (letter_dictionary != null)
                                            {
                                                List<char> keys = new List<char>(letter_dictionary.Keys);
                                                List<long> values = new List<long>(letter_dictionary.Values);
                                                List<char> characters = new List<char>();
                                                foreach (char c in str.ToString())
                                                {
                                                    if (c == ' ') continue;
                                                    characters.Add(c);
                                                }

                                                // merge mathematically
                                                for (int j = 0; j < characters.Count; j++)
                                                {
                                                    long characterr_value = m_client.CalculateValue(characters[j]);

                                                    long number = 0L;
                                                    string concatenated_number = "";
                                                    if (m_concatenation_direction == RightToLeft.Yes)
                                                    {
                                                        concatenated_number = characterr_value.ToString() + values[j].ToString();
                                                    }
                                                    else
                                                    {
                                                        concatenated_number = values[j].ToString() + characterr_value.ToString();
                                                    }

                                                    if (long.TryParse(concatenated_number, out number))
                                                    {
                                                        if (Numbers.IsNumberType(number, m_number_type))
                                                        {
                                                            text += keys[j];
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (quran_words.Contains(text))
                                        {
                                            string[] parts = new string[3];
                                            parts[0] = str.ToString();
                                            parts[1] = value.ToString();
                                            parts[2] = text;
                                            ListView.Items.Add(new ListViewItem(parts, i));
                                        }

                                        this.Text = "Generator: " + m_sentences.Count + " sentences found";
                                        ValidWordCountLabel.Text = ListView.Items.Count + " valid words";
                                        ValidWordCountLabel.Refresh();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void SaveButton_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            StringBuilder str = new StringBuilder();
            foreach (ListViewItem item in ListView.Items)
            {
                if (item != null)
                {
                    if ((item.SubItems != null) && (item.SubItems.Count == 3))
                    {
                        str.AppendLine(item.SubItems[0].Text + "\t" + item.SubItems[1].Text + "\t" + item.SubItems[2].Text);
                    }
                }
            }

            string filename = "GenerateWords.txt";
            if (Directory.Exists(Globals.STATISTICS_FOLDER))
            {
                string path = Globals.STATISTICS_FOLDER + "/" + filename;
                FileHelper.SaveText(path, str.ToString());
                FileHelper.DisplayFile(path);
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
}
