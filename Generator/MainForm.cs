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
    private Client m_client = null;
    private string m_numerology_system_name = "Simplified29_Alphabet_Primes1";
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

                if (m_client.NumerologySystem != null)
                {
                    m_client.NumerologySystem.AddToLetterCNumber = false;
                    m_client.NumerologySystem.AddToWordCNumber = false;
                    m_client.NumerologySystem.AddToVerseCNumber = false;
                    m_client.NumerologySystem.AddToChapterCNumber = false;

                    m_client.NumerologySystem.AddToLetterLDistance = true;
                    m_client.NumerologySystem.AddToLetterWDistance = true;
                    m_client.NumerologySystem.AddToLetterVDistance = true;
                    m_client.NumerologySystem.AddToLetterCDistance = false;
                    m_client.NumerologySystem.AddToWordWDistance = true;
                    m_client.NumerologySystem.AddToWordVDistance = true;
                    m_client.NumerologySystem.AddToWordCDistance = false;
                    m_client.NumerologySystem.AddToVerseVDistance = true;
                    m_client.NumerologySystem.AddToVerseCDistance = false;

                    m_client.NumerologySystem.AddDistancesToPrevious = false;
                    m_client.NumerologySystem.AddDistancesToNext = false;
                    m_client.NumerologySystem.AddDistancesWithinChapters = true;
                }
            }

            PopulateNumerologySystemComboBox();
            if (NumerologySystemComboBox.Items.Count > 0)
            {
                if (NumerologySystemComboBox.Items.Contains(m_numerology_system_name))
                {
                    NumerologySystemComboBox.SelectedItem = m_numerology_system_name;
                }
                else
                {
                    NumerologySystemComboBox.SelectedIndex = 0;
                }
            }
        }

        m_lines = new List<Line>();

        m_number_type = NumberType.Prime;
        NumberTypeLabel.Text = "P";
        NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(19L);
        ToolTip.SetToolTip(NumberTypeLabel, "use prime " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
    }
    private void PopulateNumerologySystemComboBox()
    {
        NumerologySystemComboBox.SelectedIndexChanged -= new EventHandler(NumerologySystemComboBox_SelectedIndexChanged);
        try
        {
            if (m_client != null)
            {
                if (m_client.LoadedNumerologySystems != null)
                {
                    NumerologySystemComboBox.Items.Clear();
                    foreach (NumerologySystem numerology_system in m_client.LoadedNumerologySystems.Values)
                    {
                        string text_mode = m_client.NumerologySystem.TextMode;
                        if (numerology_system.Name.StartsWith(text_mode))
                        {
                            if (numerology_system.Name.Contains("_N_")) continue;
                            if (numerology_system.Name.Contains("Zero")) continue;

                            NumerologySystemComboBox.Items.Add(numerology_system.Name);
                        }
                    }
                }
            }
        }
        finally
        {
            NumerologySystemComboBox.SelectedIndexChanged += new EventHandler(NumerologySystemComboBox_SelectedIndexChanged);
        }
    }
    private void NumerologySystemComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        m_numerology_system_name = NumerologySystemComboBox.SelectedItem.ToString();
        if (m_client != null)
        {
            m_client.LoadNumerologySystem(m_numerology_system_name);
        }
    }
    private void NumerologySystemComboBox_MouseHover(object sender, EventArgs e)
    {
        if (m_client != null)
        {
            if (m_client.NumerologySystem != null)
            {
                StringBuilder str = new StringBuilder();
                foreach (char c in m_client.NumerologySystem.Keys)
                {
                    str.AppendLine(c.ToString() + "\t" + m_client.NumerologySystem[c].ToString());
                }
                ToolTip.SetToolTip(NumerologySystemComboBox, str.ToString());
            }
        }
    }

    private bool m_add_word_verse_values_to_value = false;
    private bool m_add_positions_to_value = false;
    private bool m_add_distances_to_previous_to_value = false;
    private bool m_add_distances_to_next_to_value = false;
    private void AddWordVerseValuesCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_add_word_verse_values_to_value = AddWordVerseValuesCheckBox.Checked;
    }
    private void AddPositionsCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_add_positions_to_value = AddPositionsCheckBox.Checked;
        if (m_client != null)
        {
            if (m_client.NumerologySystem != null)
            {
                m_client.NumerologySystem.AddToLetterLNumber = m_add_positions_to_value;
                m_client.NumerologySystem.AddToLetterWNumber = m_add_positions_to_value;
                m_client.NumerologySystem.AddToLetterVNumber = m_add_positions_to_value;
                m_client.NumerologySystem.AddToWordWNumber = m_add_positions_to_value;
                m_client.NumerologySystem.AddToWordVNumber = m_add_positions_to_value;
                m_client.NumerologySystem.AddToVerseVNumber = m_add_positions_to_value;
            }
        }
        AddPositionsCheckBox.Refresh();
    }
    private void AddDistancesToPreviousCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_add_distances_to_previous_to_value = AddDistancesToPreviousCheckBox.Checked;
        if (m_client != null)
        {
            if (m_client.NumerologySystem != null)
            {
                m_client.NumerologySystem.AddDistancesToPrevious = m_add_distances_to_previous_to_value;
            }
        }
        AddDistancesToPreviousCheckBox.Refresh();
    }
    private void AddDistancesToNextCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_add_distances_to_next_to_value = AddDistancesToNextCheckBox.Checked;
        if (m_client != null)
        {
            if (m_client.NumerologySystem != null)
            {
                m_client.NumerologySystem.AddDistancesToNext = m_add_distances_to_next_to_value;
            }
        }
        AddDistancesToNextCheckBox.Refresh();
    }

    private bool m_interlace = false;
    private void InterlaceLabel_Click(object sender, EventArgs e)
    {
        m_interlace = !m_interlace;
        InterlaceLabel.Text = m_interlace ? "§" : "- -";
        ToolTip.SetToolTip(InterlaceLabel, m_interlace ? "interlace digits of letter values" : "concatenate letter values");

        if (m_combination_direction == RightToLeft.Yes)
            ToolTip.SetToolTip(DirectionLabel, m_interlace ? "interlace digits of letter values: BABABA" : "concatenate letter values right to left: BBBAAA");
        else
            ToolTip.SetToolTip(DirectionLabel, m_interlace ? "interlace digits of letter values: ABABAB" : "concatenate letter values left to right: AAABBB");

        switch (m_number_type)
        {
            case NumberType.Prime:
                ToolTip.SetToolTip(NumberTypeLabel, "use prime " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                break;
            case NumberType.AdditivePrime:
                ToolTip.SetToolTip(NumberTypeLabel, "use additive prime " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                break;
            case NumberType.NonAdditivePrime:
                ToolTip.SetToolTip(NumberTypeLabel, "use non-additive prime " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                break;
            case NumberType.Composite:
                ToolTip.SetToolTip(NumberTypeLabel, "use composite " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                break;
            case NumberType.AdditiveComposite:
                ToolTip.SetToolTip(NumberTypeLabel, "use additive composite " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                break;
            case NumberType.NonAdditiveComposite:
                ToolTip.SetToolTip(NumberTypeLabel, "use non-additive composite " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                break;
        }

        InterlaceLabel.Refresh();
    }
    private RightToLeft m_combination_direction = RightToLeft.Yes;
    private void DirectionLabel_Click(object sender, EventArgs e)
    {
        string message = null;
        if (m_combination_direction == RightToLeft.Yes)
        {
            m_combination_direction = RightToLeft.No;
            DirectionLabel.Text = "→";
            message = m_interlace ? "interlace digits of letter values: ABABAB" : "concatenate letter values left to right: AAABBB";
        }
        else
        {
            m_combination_direction = RightToLeft.Yes;
            DirectionLabel.Text = "←";
            message = m_interlace ? "interlace digits of letter values: BABABA" : "concatenate letter values right to left: BBBAAA";
        }
        ToolTip.SetToolTip(DirectionLabel, message);
        DirectionLabel.Refresh();
    }
    private NumberType m_number_type = NumberType.Prime;
    private void NumberTypeLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Shift)
        {
            switch (m_number_type)
            {
                case NumberType.NonAdditiveComposite:
                    {
                        m_number_type = NumberType.AdditiveComposite;
                        NumberTypeLabel.Text = "AC";
                        NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(114L);
                        ToolTip.SetToolTip(NumberTypeLabel, "use additive composite " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                    }
                    break;
                case NumberType.AdditiveComposite:
                    {
                        m_number_type = NumberType.Composite;
                        NumberTypeLabel.Text = "C";
                        NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(14L);
                        ToolTip.SetToolTip(NumberTypeLabel, "use composite " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                    }
                    break;
                case NumberType.Composite:
                    {
                        m_number_type = NumberType.NonAdditivePrime;
                        NumberTypeLabel.Text = "XP";
                        NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(19L);
                        ToolTip.SetToolTip(NumberTypeLabel, "use non-additive prime " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                    }
                    break;
                case NumberType.NonAdditivePrime:
                    {
                        m_number_type = NumberType.AdditivePrime;
                        NumberTypeLabel.Text = "AP";
                        NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(47L);
                        ToolTip.SetToolTip(NumberTypeLabel, "use additive prime " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                    }
                    break;
                case NumberType.AdditivePrime:
                    {
                        m_number_type = NumberType.Prime;
                        NumberTypeLabel.Text = "P";
                        NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(19L);
                        ToolTip.SetToolTip(NumberTypeLabel, "use prime " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                    }
                    break;
                case NumberType.Prime:
                    {
                        m_number_type = NumberType.NonAdditiveComposite;
                        NumberTypeLabel.Text = "XC";
                        NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(25L);
                        ToolTip.SetToolTip(NumberTypeLabel, "use non-additive composite " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                    }
                    break;
            }
        }
        else
        {
            switch (m_number_type)
            {
                case NumberType.Prime:
                    {
                        m_number_type = NumberType.AdditivePrime;
                        NumberTypeLabel.Text = "AP";
                        NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(47L);
                        ToolTip.SetToolTip(NumberTypeLabel, "use additive prime " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                    }
                    break;
                case NumberType.AdditivePrime:
                    {
                        m_number_type = NumberType.NonAdditivePrime;
                        NumberTypeLabel.Text = "XP";
                        NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(19L);
                        ToolTip.SetToolTip(NumberTypeLabel, "use non-additive prime " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                    }
                    break;
                case NumberType.NonAdditivePrime:
                    {
                        m_number_type = NumberType.Composite;
                        NumberTypeLabel.Text = "C";
                        NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(14L);
                        ToolTip.SetToolTip(NumberTypeLabel, "use composite " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                    }
                    break;
                case NumberType.Composite:
                    {
                        m_number_type = NumberType.AdditiveComposite;
                        NumberTypeLabel.Text = "AC";
                        NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(114L);
                        ToolTip.SetToolTip(NumberTypeLabel, "use additive composite " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                    }
                    break;
                case NumberType.AdditiveComposite:
                    {
                        m_number_type = NumberType.NonAdditiveComposite;
                        NumberTypeLabel.Text = "XC";
                        NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(25L);
                        ToolTip.SetToolTip(NumberTypeLabel, "use non-additive composite " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                    }
                    break;
                case NumberType.NonAdditiveComposite:
                    {
                        m_number_type = NumberType.Prime;
                        NumberTypeLabel.Text = "P";
                        NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(19L);
                        ToolTip.SetToolTip(NumberTypeLabel, "use prime " + (m_interlace ? "interlaced" : "concatenated") + " letter values only");
                    }
                    break;
            }
        }
        NumberTypeLabel.Refresh();
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

            // sort items
            m_lines.Sort();

            // display items
            UpdateListView();
        }
    }
    private void ClearListView()
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
    }
    private void UpdateListView()
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
        ListView.Refresh();
    }

    private void GenerateButton_Click(object sender, EventArgs e)
    {
        NumerologySystemComboBox.Enabled = false;
        AddWordVerseValuesCheckBox.Enabled = false;
        AddPositionsCheckBox.Enabled = false;
        AddDistancesToPreviousCheckBox.Enabled = false;
        AddDistancesToNextCheckBox.Enabled = false;
        DirectionLabel.Enabled = false;
        NumberTypeLabel.Enabled = false;
        GenerateButton.Enabled = false;
        SaveButton.Enabled = false;
        ClearListView();

        this.Cursor = Cursors.WaitCursor;
        try
        {
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
                                        }

                                        // calculate letter values
                                        List<long> sentence_letter_values = new List<long>();
                                        foreach (Word word in m_word_subsets[i])
                                        {
                                            foreach (Letter letter in word.Letters)
                                            {
                                                long letter_value = m_client.CalculateValue(letter);
                                                if (m_add_word_verse_values_to_value)
                                                {
                                                    letter_value += m_client.CalculateValue(letter.Word);
                                                    letter_value += m_client.CalculateValue(letter.Word.Verse);
                                                }
                                                sentence_letter_values.Add(letter_value);
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

                                                // interlace or concatenate values of numerology letters with sentence letters
                                                for (int j = 0; j < numerology_letters.Count; j++)
                                                {
                                                    long number = 0L;
                                                    string combination = "";
                                                    string AAA = numerology_letter_values[j].ToString();
                                                    string BBB = sentence_letter_values[j].ToString();

                                                    if (m_interlace)
                                                    {
                                                        int a = AAA.Length;
                                                        int b = BBB.Length;
                                                        int min = Math.Min(a, b);

                                                        string ABABAB = null;
                                                        for (int d = 0; d < min; d++)
                                                        {
                                                            ABABAB += AAA[d].ToString() + BBB[d].ToString();
                                                        }
                                                        if (a > min)
                                                        {
                                                            ABABAB += AAA.Substring(min);
                                                        }
                                                        else
                                                        {
                                                            ABABAB += BBB.Substring(min);
                                                        }

                                                        string BABABA = null;
                                                        for (int d = 0; d < min; d++)
                                                        {
                                                            BABABA += BBB[d].ToString() + AAA[d].ToString();
                                                        }
                                                        if (a > min)
                                                        {
                                                            BABABA += AAA.Substring(min);
                                                        }
                                                        else
                                                        {
                                                            BABABA += BBB.Substring(min);
                                                        }

                                                        combination = (m_combination_direction == RightToLeft.Yes) ? BABABA : ABABAB;
                                                    }
                                                    else
                                                    {
                                                        string AAABBB = AAA + BBB;
                                                        string BBBAAA = BBB + AAA;
                                                        combination = (m_combination_direction == RightToLeft.Yes) ? BBBAAA : AAABBB;
                                                    }

                                                    // generate word from letter value combinations matching the number type
                                                    if (long.TryParse(combination, out number))
                                                    {
                                                        if (Numbers.IsNumberType(number, m_number_type))
                                                        {
                                                            // mod 29 to select letter
                                                            int index = (int)((long)number % (long)numerology_letters.Count);
                                                            generated_word += numerology_letters[index];
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        // add sentence if it generates a valid quran word
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
                                        this.Text = "Generator: " + (i + 1) + " / " + m_word_subsets.Count + " sentences processed";
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

                                    UpdateListView();
                                }
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            NumerologySystemComboBox.Enabled = true;
            AddWordVerseValuesCheckBox.Enabled = true;
            AddPositionsCheckBox.Enabled = true;
            AddDistancesToPreviousCheckBox.Enabled = true;
            AddDistancesToNextCheckBox.Enabled = true;
            DirectionLabel.Enabled = true;
            NumberTypeLabel.Enabled = true;
            GenerateButton.Enabled = true;
            SaveButton.Enabled = true;

            this.Cursor = Cursors.Default;
        }
    }
    private void AutoGenerateButton_Click(object sender, EventArgs e)
    {
        NumerologySystemComboBox.Enabled = false;
        AddWordVerseValuesCheckBox.Enabled = false;
        AddPositionsCheckBox.Enabled = false;
        AddDistancesToPreviousCheckBox.Enabled = false;
        AddDistancesToNextCheckBox.Enabled = false;
        DirectionLabel.Enabled = false;
        NumberTypeLabel.Enabled = false;
        GenerateButton.Enabled = false;
        SaveButton.Enabled = false;

        // prepare for next state
        m_interlace = true;
        m_combination_direction = RightToLeft.No;
        m_number_type = NumberType.NonAdditiveComposite;

        this.Cursor = Cursors.WaitCursor;
        try
        {
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

                                // find all 7-word 29-letter word subsets
                                WordSubsetFinder word_subset_finder = new WordSubsetFinder(words);
                                m_word_subsets = word_subset_finder.Find(verses.Count, words.Count);
                                if (m_word_subsets != null)
                                {
                                    for (int h = 0; h < 2; h++)
                                    {
                                        AddWordVerseValuesCheckBox.Checked = (h == 1);
                                        for (int k = 0; k < 2; k++)
                                        {
                                            AddPositionsCheckBox.Checked = (k == 1);
                                            for (int l = 0; l < 2; l++)
                                            {
                                                AddDistancesToPreviousCheckBox.Checked = (l == 1);
                                                for (int m = 0; m < 2; m++)
                                                {
                                                    AddDistancesToNextCheckBox.Checked = (m == 1);
                                                    for (int n = 0; n < 2; n++)
                                                    {
                                                        InterlaceLabel_Click(sender, e);
                                                        for (int o = 0; o < 2; o++)
                                                        {
                                                            DirectionLabel_Click(sender, e);
                                                            for (int p = 0; p < 6; p++)
                                                            {
                                                                NumberTypeLabel_Click(sender, e);
                                                                if (m_lines != null)
                                                                {
                                                                    m_lines.Clear();
                                                                    for (int i = 0; i < m_word_subsets.Count; i++)
                                                                    {
                                                                        // calculate word values
                                                                        long sentence_word_value = 0L;
                                                                        foreach (Word word in m_word_subsets[i])
                                                                        {
                                                                            sentence_word_value += m_client.CalculateValue(word);
                                                                        }

                                                                        // calculate letter values
                                                                        List<long> sentence_letter_values = new List<long>();
                                                                        foreach (Word word in m_word_subsets[i])
                                                                        {
                                                                            foreach (Letter letter in word.Letters)
                                                                            {
                                                                                long letter_value = m_client.CalculateValue(letter);
                                                                                if (m_add_word_verse_values_to_value)
                                                                                {
                                                                                    letter_value += m_client.CalculateValue(letter.Word);
                                                                                    letter_value += m_client.CalculateValue(letter.Word.Verse);
                                                                                }
                                                                                sentence_letter_values.Add(letter_value);
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

                                                                                // interlace or concatenate values of numerology letters with sentence letters
                                                                                for (int j = 0; j < numerology_letters.Count; j++)
                                                                                {
                                                                                    long number = 0L;
                                                                                    string combination = "";
                                                                                    string AAA = numerology_letter_values[j].ToString();
                                                                                    string BBB = sentence_letter_values[j].ToString();

                                                                                    if (m_interlace)
                                                                                    {
                                                                                        int a = AAA.Length;
                                                                                        int b = BBB.Length;
                                                                                        int min = Math.Min(a, b);

                                                                                        string ABABAB = null;
                                                                                        for (int d = 0; d < min; d++)
                                                                                        {
                                                                                            ABABAB += AAA[d].ToString() + BBB[d].ToString();
                                                                                        }
                                                                                        if (a > min)
                                                                                        {
                                                                                            ABABAB += AAA.Substring(min);
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            ABABAB += BBB.Substring(min);
                                                                                        }

                                                                                        string BABABA = null;
                                                                                        for (int d = 0; d < min; d++)
                                                                                        {
                                                                                            BABABA += BBB[d].ToString() + AAA[d].ToString();
                                                                                        }
                                                                                        if (a > min)
                                                                                        {
                                                                                            BABABA += AAA.Substring(min);
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            BABABA += BBB.Substring(min);
                                                                                        }

                                                                                        combination = (m_combination_direction == RightToLeft.Yes) ? BABABA : ABABAB;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        string AAABBB = AAA + BBB;
                                                                                        string BBBAAA = BBB + AAA;
                                                                                        combination = (m_combination_direction == RightToLeft.Yes) ? BBBAAA : AAABBB;
                                                                                    }

                                                                                    // generate word from letter value combinations matching the number type
                                                                                    if (long.TryParse(combination, out number))
                                                                                    {
                                                                                        if (Numbers.IsNumberType(number, m_number_type))
                                                                                        {
                                                                                            // mod 29 to select letter
                                                                                            int index = (int)((long)number % (long)numerology_letters.Count);
                                                                                            generated_word += numerology_letters[index];
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }

                                                                        // add sentence if it generates a valid quran word
                                                                        if (quran_word_texts.Contains(generated_word))
                                                                        {
                                                                            Line line = new Line();
                                                                            line.Id = m_lines.Count + 1;
                                                                            line.Sentence = str.ToString();
                                                                            line.Value = sentence_word_value;
                                                                            line.Word = generated_word;
                                                                            m_lines.Add(line);
                                                                        }

                                                                        //// display progress
                                                                        //this.Text = "Generator: " + (i + 1) + " / " + m_word_subsets.Count + " sentences processed";
                                                                        //ProgressBar.Value = ((i + 1) * 100) / m_word_subsets.Count;
                                                                        //ProgressBar.Refresh();
                                                                        //WordCountLabel.Text = m_lines.Count + " words";
                                                                        //WordCountLabel.ForeColor = Numbers.GetNumberTypeColor(m_lines.Count);
                                                                        //WordCountLabel.Refresh();
                                                                    }

                                                                    this.Text = "Generator: " + m_word_subsets.Count + " sentences processed";
                                                                    ProgressBar.Value = 100;
                                                                    ProgressBar.Refresh();
                                                                    WordCountLabel.Text = m_lines.Count + " words";
                                                                    WordCountLabel.ForeColor = Numbers.GetNumberTypeColor(m_lines.Count);
                                                                    WordCountLabel.Refresh();

                                                                    UpdateListView();

                                                                    SaveButton_Click(sender, e);

                                                                    Application.DoEvents();
                                                                }
                                                            } // for NumberType
                                                        } // for Direction
                                                    } // for Interlace
                                                } // for AddDistancesToNext
                                            } // for AddDistancesToPrevious
                                        } // for AddPositions
                                    } // for AddWordVerseValues
                                }
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            NumerologySystemComboBox.Enabled = true;
            AddWordVerseValuesCheckBox.Enabled = true;
            AddPositionsCheckBox.Enabled = true;
            AddDistancesToPreviousCheckBox.Enabled = true;
            AddDistancesToNextCheckBox.Enabled = true;
            DirectionLabel.Enabled = true;
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
            if (m_lines != null)
            {
                StringBuilder str = new StringBuilder();
                for (int i = 0; i < m_lines.Count; i++)
                {
                    if (m_lines[i] != null)
                    {
                        str.AppendLine(m_lines[i].Id + "\t" + m_lines[i].Sentence + "\t" + m_lines[i].Value + "\t" + m_lines[i].Word);
                    }
                }

                string filename =
                    m_numerology_system_name
                    + (m_add_positions_to_value ? "_n" : "__")
                    + (m_add_distances_to_previous_to_value ? "_-d" : "_-_")
                    + (m_add_distances_to_next_to_value ? "_d-" : "__-")
                    + (m_interlace ? "_interlace" : "_concatenate")
                    + ((m_combination_direction == RightToLeft.Yes) ? "_r" : "_l")
                    + ((m_number_type != NumberType.None) ? "_" : "")
                    + (
                        (m_number_type == NumberType.Prime) ? "_P" :
                        (m_number_type == NumberType.AdditivePrime) ? "AP" :
                        (m_number_type == NumberType.NonAdditivePrime) ? "XP" :
                        (m_number_type == NumberType.Composite) ? "_C" :
                        (m_number_type == NumberType.AdditiveComposite) ? "AC" :
                        (m_number_type == NumberType.NonAdditiveComposite) ? "XC" : ""
                        )
                    + ((sender == SaveButton) ? ((Line.SortOrder == SortOrder.Ascending) ? "_asc" : "_desc") : "")
                    + ((sender == SaveButton) ? Line.SortMethod.ToString().Substring(2) : "")
                    + ".txt";

                if (Directory.Exists(Globals.STATISTICS_FOLDER))
                {
                    string path = Globals.STATISTICS_FOLDER + "/" + filename;
                    FileHelper.SaveText(path, str.ToString());
                }

                if (sender == SaveButton)
                {
                    ViewInNotpad(filename);
                }
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void ViewInNotpad(string filename)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (Directory.Exists(Globals.STATISTICS_FOLDER))
            {
                string path = Globals.STATISTICS_FOLDER + "/" + filename;
                FileHelper.DisplayFile(path);
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
}
