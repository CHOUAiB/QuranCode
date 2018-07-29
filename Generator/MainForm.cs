using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Threading;
using Model;

public partial class MainForm : Form
{
    private string m_numerology_system_name = "Simplified29_Alphabet_Primes1";

    private Client m_client = null;
    private List<List<Word>> m_word_subsets = null;
    private List<Line> m_lines = null;

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

        m_lines = new List<Line>();

        UpdateNumberType(NumberTypeLabel);
    }

    private bool m_add_positions_to_value = false;
    private bool m_add_distances_to_previous_to_value = false;
    private bool m_add_distances_to_next_to_value = false;
    private void AddPositionsCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_add_positions_to_value = AddPositionsCheckBox.Checked;
        if (m_client != null)
        {
            m_client.NumerologySystem.AddToLetterLNumber = m_add_positions_to_value;
            m_client.NumerologySystem.AddToLetterWNumber = m_add_positions_to_value;
            m_client.NumerologySystem.AddToLetterVNumber = m_add_positions_to_value;
            m_client.NumerologySystem.AddToLetterCNumber = m_add_positions_to_value;
            m_client.NumerologySystem.AddToWordWNumber = m_add_positions_to_value;
            m_client.NumerologySystem.AddToWordVNumber = m_add_positions_to_value;
            m_client.NumerologySystem.AddToWordCNumber = m_add_positions_to_value;
            m_client.NumerologySystem.AddToVerseVNumber = m_add_positions_to_value;
            m_client.NumerologySystem.AddToVerseCNumber = m_add_positions_to_value;
            m_client.NumerologySystem.AddToChapterCNumber = m_add_distances_to_previous_to_value;
        }
    }
    private void AddDistancesToPreviousCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_add_distances_to_previous_to_value = AddDistancesToPreviousCheckBox.Checked;
        if (m_client != null)
        {
            m_client.NumerologySystem.AddToLetterLDistance = m_add_distances_to_previous_to_value;
            m_client.NumerologySystem.AddToLetterWDistance = m_add_distances_to_previous_to_value;
            m_client.NumerologySystem.AddToLetterVDistance = m_add_distances_to_previous_to_value;
            m_client.NumerologySystem.AddToLetterCDistance = m_add_distances_to_previous_to_value;
            m_client.NumerologySystem.AddToWordWDistance = m_add_distances_to_previous_to_value;
            m_client.NumerologySystem.AddToWordVDistance = m_add_distances_to_previous_to_value;
            m_client.NumerologySystem.AddToWordCDistance = m_add_distances_to_previous_to_value;
            m_client.NumerologySystem.AddToVerseVDistance = m_add_distances_to_previous_to_value;
            m_client.NumerologySystem.AddToVerseCDistance = m_add_distances_to_previous_to_value;

            m_client.NumerologySystem.AddDistancesToPrevious = m_add_distances_to_previous_to_value;
            m_client.NumerologySystem.AddDistancesWithinChapters = true;
        }
    }
    private void AddDistancesToNextCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_add_distances_to_next_to_value = AddDistancesToNextCheckBox.Checked;
        if (m_client != null)
        {
            m_client.NumerologySystem.AddToLetterLDistance = m_add_distances_to_next_to_value;
            m_client.NumerologySystem.AddToLetterWDistance = m_add_distances_to_next_to_value;
            m_client.NumerologySystem.AddToLetterVDistance = m_add_distances_to_next_to_value;
            m_client.NumerologySystem.AddToLetterCDistance = m_add_distances_to_next_to_value;
            m_client.NumerologySystem.AddToWordWDistance = m_add_distances_to_next_to_value;
            m_client.NumerologySystem.AddToWordVDistance = m_add_distances_to_next_to_value;
            m_client.NumerologySystem.AddToWordCDistance = m_add_distances_to_next_to_value;
            m_client.NumerologySystem.AddToVerseVDistance = m_add_distances_to_next_to_value;
            m_client.NumerologySystem.AddToVerseCDistance = m_add_distances_to_next_to_value;

            m_client.NumerologySystem.AddDistancesToNext = m_add_distances_to_next_to_value;
            m_client.NumerologySystem.AddDistancesWithinChapters = true;
        }
    }

    private RightToLeft m_concatenation_direction = RightToLeft.Yes;
    private void ConcatenationDirectionLabel_Click(object sender, EventArgs e)
    {
        if (m_concatenation_direction == RightToLeft.Yes)
        {
            m_concatenation_direction = RightToLeft.No;
            ConcatenationDirectionLabel.Text = "→";
            ToolTip.SetToolTip(ConcatenationDirectionLabel, "concatenate letter values left to right");
        }
        else
        {
            m_concatenation_direction = RightToLeft.Yes;
            ConcatenationDirectionLabel.Text = "←";
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

    public enum SortMethod { ById, BySentence, ByValue, ByWord }
    public enum SortOrder { Ascending, Descending }
    public class Line : IComparable<Line>
    {
        public int Id;
        public string Sentence;
        public long Value;
        public string Word;

        public static SortMethod SortMethod;
        public static SortOrder SortOrder;
        public int CompareTo(Line obj)
        {
            if (SortOrder == SortOrder.Ascending)
            {
                if (SortMethod == SortMethod.ById)
                {
                    return this.Id.CompareTo(obj.Id);
                }
                else if (SortMethod == SortMethod.BySentence)
                {
                    return this.Sentence.CompareTo(obj.Sentence);
                }
                else if (SortMethod == SortMethod.ByValue)
                {
                    return this.Value.CompareTo(obj.Value);
                }
                else if (SortMethod == SortMethod.ByWord)
                {
                    return this.Word.CompareTo(obj.Word);
                }
                else
                {
                    return this.Id.CompareTo(obj.Id);
                }
            }
            else
            {
                if (SortMethod == SortMethod.ById)
                {
                    return obj.Id.CompareTo(this.Id);
                }
                else if (SortMethod == SortMethod.BySentence)
                {
                    return obj.Sentence.CompareTo(this.Sentence);
                }
                else if (SortMethod == SortMethod.ByValue)
                {
                    return obj.Value.CompareTo(this.Value);
                }
                else if (SortMethod == SortMethod.ByWord)
                {
                    return obj.Word.CompareTo(this.Word);
                }
                else
                {
                    return obj.Id.CompareTo(this.Id);
                }
            }
        }
    }
    private void ListView_ColumnClick(object sender, ColumnClickEventArgs e)
    {
        if (ListView != null)
        {
            try
            {
                // sort method
                Line.SortMethod = (SortMethod)e.Column;

                // sort order
                if (Line.SortOrder == SortOrder.Ascending)
                {
                    Line.SortOrder = SortOrder.Descending;
                }
                else
                {
                    Line.SortOrder = SortOrder.Ascending;
                }

                // sort marker
                string sort_marker = (Line.SortOrder == SortOrder.Ascending) ? "▲" : "▼";
                foreach (ColumnHeader column in ListView.Columns)
                {
                    if (column.Text.EndsWith("▲"))
                    {
                        column.Text = column.Text.Replace("▲", " ");
                    }
                    else if (column.Text.EndsWith("▼"))
                    {
                        column.Text = column.Text.Replace("▼", " ");
                    }
                }
                ListView.Columns[e.Column].Text = ListView.Columns[e.Column].Text.Replace("  ", " " + sort_marker);
                ListView.Refresh();

                // sort
                m_lines.Sort();

                DisplayLines();
            }
            catch
            {
                // log exception
            }
        }
    }
    private void DisplayLines()
    {
        ListView.Items.Clear();
        for (int i = 0; i < m_lines.Count; i++)
        {
            string[] parts = new string[4];
            parts[0] = m_lines[i].Id.ToString();
            parts[1] = m_lines[i].Sentence.ToString();
            parts[2] = m_lines[i].Value.ToString();
            parts[3] = m_lines[i].Word;
            ListView.Items.Add(new ListViewItem(parts, i));
        }
    }

    private void GenerateButton_Click(object sender, EventArgs e)
    {
        AddPositionsCheckBox.Enabled = false;
        AddDistancesToPreviousCheckBox.Enabled = false;
        AddDistancesToNextCheckBox.Enabled = false;
        ConcatenationDirectionLabel.Enabled = false;
        NumberTypeLabel.Enabled = false;
        GenerateButton.Enabled = false;
        SaveButton.Enabled = false;

        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (ListView != null)
            {
                ListView.Items.Clear();
                Line.SortMethod = (SortMethod)0;
                Line.SortOrder = SortOrder.Ascending;
                foreach (ColumnHeader column in ListView.Columns)
                {
                    if (column.Text.EndsWith("▲"))
                    {
                        column.Text = column.Text.Replace("▲", " ");
                    }
                    else if (column.Text.EndsWith("▼"))
                    {
                        column.Text = column.Text.Replace("▼", " ");
                    }
                }
                ListView.Columns[0].Text = ListView.Columns[0].Text = "# ▲";
                ListView.Refresh();

                if (m_client != null)
                {
                    if (m_client.NumerologySystem.TextMode == "Simplified29")
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

                                        // find all 7-word 29-letter word subsets
                                        WordSubsetFinder word_subset_finder = new WordSubsetFinder(words);
                                        m_word_subsets = word_subset_finder.Find(verses.Count, words.Count);
                                        if (m_word_subsets != null)
                                        {
                                            m_lines.Clear();

                                            for (int i = 0; i < m_word_subsets.Count; i++)
                                            {
                                                // calculate word values
                                                long sentence_word_value = 0L;
                                                foreach (Word word in m_word_subsets[i])
                                                {
                                                    sentence_word_value += m_client.CalculateValue(word);
                                                    sentence_word_value += m_client.CalculateValue(word.Verse);
                                                    sentence_word_value += m_client.CalculateValue(word.Verse.Chapter);
                                                }

                                                // calculate letter values
                                                List<long> sentnece_letter_values = new List<long>();
                                                foreach (Word word in m_word_subsets[i])
                                                {
                                                    foreach (Letter letter in word.Letters)
                                                    {
                                                        long letter_value = m_client.CalculateValue(letter);
                                                        if (m_add_distances_to_previous_to_value)
                                                        {
                                                            letter_value += m_client.CalculateValue(letter.Word);
                                                            letter_value += m_client.CalculateValue(letter.Word.Verse);
                                                            letter_value += m_client.CalculateValue(letter.Word.Verse.Chapter);
                                                        }
                                                        sentnece_letter_values.Add(letter_value);
                                                    }
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

                                                // generate Quran words
                                                string generated_word = "";
                                                if (m_client.NumerologySystem != null)
                                                {
                                                    Dictionary<char, long> letter_dictionary = m_client.NumerologySystem.LetterValues;
                                                    if (letter_dictionary != null)
                                                    {
                                                        List<char> numerology_letters = new List<char>(letter_dictionary.Keys);
                                                        List<long> numerology_letter_values = new List<long>(letter_dictionary.Values);

                                                        // concatenate values of numerology letters with sentence letters
                                                        for (int j = 0; j < numerology_letters.Count; j++)
                                                        {
                                                            long number = 0L;
                                                            string concatenation = "";
                                                            if (m_concatenation_direction == RightToLeft.Yes)
                                                            {
                                                                concatenation = sentnece_letter_values[j].ToString() + numerology_letter_values[j].ToString();
                                                            }
                                                            else
                                                            {
                                                                concatenation = numerology_letter_values[j].ToString() + sentnece_letter_values[j].ToString();
                                                            }

                                                            // generate word from letter value concatenation matching number type
                                                            if (long.TryParse(concatenation, out number))
                                                            {
                                                                if (Numbers.IsNumberType(number, m_number_type))
                                                                {
                                                                    // mod 29 to select letter
                                                                    int index = ((int)number) % numerology_letters.Count;
                                                                    generated_word += numerology_letters[index];
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                // display sentence if it generates a valid quran word
                                                if (quran_word_texts.Contains(generated_word))
                                                {
                                                    Line line = new Line();
                                                    line.Id = m_lines.Count + 1;
                                                    line.Sentence = str.ToString();
                                                    line.Value = sentence_word_value;
                                                    line.Word = generated_word;
                                                    m_lines.Add(line);
                                                }

                                                // display progress
                                                this.Text = "Book Generator: " + (i + 1) + " / " + m_word_subsets.Count + " sentences processed";
                                                ProgressBar.Value = ((i + 1) * 100) / m_word_subsets.Count;
                                                WordCountLabel.Text = m_lines.Count + " words";
                                                WordCountLabel.ForeColor = Numbers.GetNumberTypeColor(m_lines.Count);
                                                WordCountLabel.Refresh();

                                                Application.DoEvents();
                                            }

                                            if (m_lines.Count == 0)
                                            {
                                                WordCountLabel.Text = "00000 words";
                                                WordCountLabel.ForeColor = Numbers.GetNumberTypeColor(0);
                                                WordCountLabel.Refresh();
                                            }

                                            DisplayLines();
                                        }
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
            AddPositionsCheckBox.Enabled = true;
            AddDistancesToPreviousCheckBox.Enabled = true;
            AddDistancesToNextCheckBox.Enabled = true;
            ConcatenationDirectionLabel.Enabled = true;
            NumberTypeLabel.Enabled = true;
            GenerateButton.Enabled = true;
            SaveButton.Enabled = true;

            this.Cursor = Cursors.Default;
        }
    }
    private void SaveButton_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (ListView != null)
            {
                StringBuilder str = new StringBuilder();
                for (int i = 0; i < ListView.Items.Count; i++)
                {
                    if (ListView.Items[i] != null)
                    {
                        if ((ListView.Items[i].SubItems != null) && (ListView.Items[i].SubItems.Count == 4))
                        {
                            str.AppendLine(ListView.Items[i].SubItems[0].Text + "\t" + ListView.Items[i].SubItems[1].Text + "\t" + ListView.Items[i].SubItems[2].Text + "\t" + ListView.Items[i].SubItems[3].Text);
                        }
                    }
                }

                //Simplified29_Alphabet_Primes1_Pos_DistP_DistN_ConcatR2L_AP_Words_Asc_ById.txt
                string filename = m_numerology_system_name 
                                + (m_add_positions_to_value ? "_Pos" : "")
                                + (m_add_distances_to_previous_to_value ? "_DistP" : "")
                                + (m_add_distances_to_next_to_value ? "_DistN" : "")
                                + "Concatenate" + ((m_concatenation_direction == RightToLeft.Yes) ? "_R2L" : "_L2R")
                                + ((m_number_type != NumberType.None) ? "_" : "") + m_number_type.ToString()
                                + "_" + Line.SortOrder.ToString()
                                + "_" + Line.SortMethod.ToString()
                                + ".txt";

                if (Directory.Exists(Globals.STATISTICS_FOLDER))
                {
                    string path = Globals.STATISTICS_FOLDER + "/" + filename;
                    FileHelper.SaveText(path, str.ToString());
                    FileHelper.DisplayFile(path);
                }
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
}
