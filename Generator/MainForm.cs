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
    private List<List<Word>> m_word_subsets = null;
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
                                // load unique quran words
                                List<string> quran_word_texts = FileHelper.LoadLines(filename);

                                // setup all quran words from quran verses
                                List<Word> words = new List<Word>();
                                foreach (Verse verse in verses)
                                {
                                    words.AddRange(verse.Words);
                                }

                                //// calculate values of first 7 words in the Quran
                                //List<Word> first_7_words = new List<Word>();
                                //for (int w = 0; w < 7; w++)
                                //{
                                //    first_7_words.Add(words[w]);
                                //}
                                //List<long> first_7_word_values = new List<long>();
                                //foreach (Word word in first_7_words)
                                //{
                                //    long word_value = m_client.CalculateValue(words);
                                //    first_7_word_values.Add(word_value);
                                //}

                                // find all 7-word 29-letter word subsets
                                WordSubsetFinder word_subset_finder = new WordSubsetFinder(words);
                                m_word_subsets = word_subset_finder.Find(verses.Count, words.Count);
                                if (m_word_subsets != null)
                                {
                                    for (int i = 0; i < m_word_subsets.Count; i++)
                                    {
                                        // calculate word values
                                        List<long> word_values = new List<long>();
                                        foreach (Word word in m_word_subsets[i])
                                        {
                                            long word_value = m_client.CalculateValue(word);
                                            word_values.Add(word_value);
                                        }

                                        //// combine the first_7_word_values with the 7 word values in the word subset
                                        //for (int j = 0; j < word_values.Count; j++)
                                        //{
                                        //    long number7 = 0L;
                                        //    string concatenation7 = "";
                                        //    string text7 = "";
                                        //    if (m_concatenation_direction == RightToLeft.Yes)
                                        //    {
                                        //        concatenation7 = word_values[j].ToString() + first_7_word_values[j].ToString();
                                        //    }
                                        //    else
                                        //    {
                                        //        concatenation7 = first_7_word_values[j].ToString() + word_values[j].ToString();
                                        //    }

                                        //    // filter by number type
                                        //    if (long.TryParse(concatenation7, out number7))
                                        //    {
                                        //        if (Numbers.IsNumberType(number7, m_number_type))
                                        //        {
                                        //            text7 += first_7_words[j];
                                        //        }
                                        //    }
                                        //}

                                        // calculate vword subset value (for display puroses only)
                                        long word_subset_value = 0L;
                                        foreach (long word_value in word_values)
                                        {
                                            word_subset_value += word_value;
                                        }

                                        // build sentence from word subset
                                        StringBuilder str = new StringBuilder();
                                        foreach (Word word in m_word_subsets[i])
                                        {
                                            str.Append(word.Text + " ");
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
                                                List<char> numerology_letters = new List<char>(letter_dictionary.Keys);
                                                List<long> numerology_letter_values = new List<long>(letter_dictionary.Values);
                                                List<char> letters = new List<char>();
                                                foreach (char c in str.ToString())
                                                {
                                                    if (c == ' ') continue;
                                                    letters.Add(c);
                                                }

                                                // combine values of the 29 numerology letters with the 29 sentence letters
                                                for (int j = 0; j < letters.Count; j++)
                                                {
                                                    long value = m_client.CalculateValue(letters[j]);

                                                    long number = 0L;
                                                    string concatenation = "";
                                                    if (m_concatenation_direction == RightToLeft.Yes)
                                                    {
                                                        concatenation = value.ToString() + numerology_letter_values[j].ToString();
                                                    }
                                                    else
                                                    {
                                                        concatenation = numerology_letter_values[j].ToString() + value.ToString();
                                                    }

                                                    // filter by number type
                                                    if (long.TryParse(concatenation, out number))
                                                    {
                                                        if (Numbers.IsNumberType(number, m_number_type))
                                                        {
                                                            text += numerology_letters[j];
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        // display sentence if it generates a valid quran word only
                                        if (quran_word_texts.Contains(text))
                                        {
                                            string[] parts = new string[3];
                                            parts[0] = str.ToString();
                                            parts[1] = word_subset_value.ToString();
                                            parts[2] = text;
                                            ListView.Items.Add(new ListViewItem(parts, i));
                                        }

                                        // display progress
                                        this.Text = "Generator: " + m_word_subsets.Count + " sentences found";
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
