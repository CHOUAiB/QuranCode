using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace InitialLetters
{
    public partial class MainForm : Form
    {
        private const string DICTIONARY_FOLDER = "Data";
        private const string HELP_FOLDER = "Help";

        List<bag_and_anagrams> m_dictionary;
        DateTime m_start_time;
        public MainForm()
        {
            InitializeComponent();

            m_filename = AppDomain.CurrentDomain.FriendlyName.Replace(".exe", ".ini");
            LoadSettings();
        }

        private string m_filename = null;
        private string m_letters = null;
        private void LoadSettings()
        {
            if (File.Exists(m_filename))
            {
                using (StreamReader reader = File.OpenText(m_filename))
                {
                    try
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            string[] parts = line.Split('=');
                            if (parts.Length == 2)
                            {
                                switch (parts[0])
                                {
                                    case "Top":
                                        {
                                            this.Top = int.Parse(parts[1]);
                                        }
                                        break;
                                    case "Left":
                                        {
                                            this.Left = int.Parse(parts[1]);
                                        }
                                        break;
                                    case "Width":
                                        {
                                            this.Width = int.Parse(parts[1]);
                                        }
                                        break;
                                    case "Height":
                                        {
                                            this.Height = int.Parse(parts[1]);
                                        }
                                        break;
                                    case "Letters":
                                        {
                                            this.m_letters = parts[1];
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    catch
                    {
                        this.Top = 0;
                        this.Left = 0;
                    }
                }
            }
            else // first start
            {
                RestoreLocation();
            }
        }
        private void SaveSettings()
        {
            try
            {
                using (StreamWriter writer = File.CreateText(m_filename))
                {
                    writer.WriteLine("[Window]");
                    writer.WriteLine("Top=" + this.Top);
                    writer.WriteLine("Left=" + this.Left);
                    writer.WriteLine("Width=" + this.Width);
                    writer.WriteLine("Height=" + this.Height);
                    writer.WriteLine("[Text]");
                    writer.WriteLine("Letters=" + this.LettersTextBox.Text); // don't use this.m_letters

                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }
        }
        private void RestoreLocation()
        {
            this.Top = (Screen.PrimaryScreen.Bounds.Height / 2) - (this.Height / 2);
            this.Left = (Screen.PrimaryScreen.Bounds.Width / 2) - (this.Width / 2);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string filename = DICTIONARY_FOLDER + "/" + "quran-words.txt";
            using (StreamReader reader = File.OpenText(filename))
            {
                ResultLabel.Text = "Compiling dictionary ...";
                ProgressBar.Minimum = 0;
                ProgressBar.Maximum = (int)reader.BaseStream.Length;
                ProgressBar.Value = 0;
                ListView_Resize(sender, e);
                try
                {
                    String line;
                    // Read and display lines from the file until the end of the file is reached.
                    int linesRead = 0;
                    Hashtable stringlists_by_bag = new Hashtable();
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.ToLower();

                        Bag aBag = new Bag(line);
                        if (!stringlists_by_bag.ContainsKey(aBag))
                        {
                            strings l = new strings();
                            l.Add(line);
                            stringlists_by_bag.Add(aBag, l);
                        }
                        else
                        {
                            strings l = (strings)stringlists_by_bag[aBag];
                            if (!l.Contains(line))
                                l.Add(line);
                        }
                        linesRead++;
                        ProgressBar.Increment((line.Length + 2) * 2);   // the +1 is for the line ending character, I'd guess.
                        // the *2 is to deal with unicode characters
                        Application.DoEvents();
                    }

                    // Now convert the hash table, which isn't useful for
                    // actually generating anagrams, into a list, which is.
                    m_dictionary = new List<bag_and_anagrams>();
                    foreach (DictionaryEntry de in stringlists_by_bag)
                    {
                        m_dictionary.Add(new bag_and_anagrams((Bag)de.Key, (strings)de.Value));
                    }
                    m_dictionary.Sort();

                    // Now just for amusement, sort the list so that the biggest bags 
                    // come first.  This might make more interesting anagrams appear first.
                    bag_and_anagrams[] sort_me = new bag_and_anagrams[m_dictionary.Count];
                    m_dictionary.CopyTo(sort_me);
                    Array.Sort(sort_me);
                    m_dictionary.Clear();
                    m_dictionary.InsertRange(0, sort_me);
                }
                catch (Exception ex)
                {
                    throw new Exception("Dictionary: " + ex.Message);
                }

                ResultLabel.Text = "Ready.";
                ListView.Enabled = true;
                LettersTextBox.Enabled = true;
                LettersTextBox.Focus();
                TypeUniqueLettersToolStripMenuItem_Click(sender, e);
            }

            //Bag.Test();
        }
        private void MainForm_Shown(object sender, EventArgs e)
        {
            if ((m_letters != null) && (m_letters.Length > 0))
            {
                LettersTextBox.Text = m_letters;
                UniqueLettersToolStripMenuItem.Checked = false;
                UniqueWordsToolStripMenuItem.Checked = false;
                AllWordsToolStripMenuItem.Checked = false;
            }
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseApplication();
        }
        private void CloseApplication()
        {
            SaveSettings();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan timespan = DateTime.Now - m_start_time;
            ElapsedTimeLabel.Text = new DateTime(timespan.Ticks).ToString("HH:mm:ss");//.fff");
        }

        private void ListView_Resize(object sender, EventArgs e)
        {
            // trial and error shows that we must make the column
            // header four pixels narrower than the containing
            // listview in order to avoid a scrollbar.
            ListView.Columns[0].Width = ListView.Width - 4;

            // if the listview is big enough to show all the items, then make sure
            // the first item is at the top.  This works around behavior (which I assume is 
            // a bug in C# or .NET or something) whereby 
            // some blank lines appear before the first item

            if (ListView.Items.Count > 0
                &&
                ListView.TopItem != null
                &&
                ListView.TopItem.Index == 0)
                ListView.EnsureVisible(0);
        }
        private void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            Clipboard.Clear();

            string selected_text = "";
            ListView me = (ListView)sender;
            foreach (ListViewItem it in me.SelectedItems)
            {
                if (selected_text.Length > 0)
                    selected_text += Environment.NewLine;
                selected_text += it.Text;
            }
            // Under some circumstances -- probably a bug in my code somewhere --
            // we can get blank lines in the listview.  And if you click on one, since it
            // has no text, selected_text will be blank; _and_, apparantly, calling
            // Clipboard.set_text with an empty string provokes an aResultLabeless violation ...
            // so avoid that AV.
            if (selected_text.Length > 0)
                Clipboard.SetText(selected_text);
        }
        private void ListView_SortColumnClick(object sender, ColumnClickEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (ListView.Sorting == SortOrder.Ascending)
                {
                    ListView.Sorting = SortOrder.Descending;
                    ListView.Columns[0].Text = "Sorted descendingly";
                }
                else
                {
                    ListView.Sorting = SortOrder.Ascending;
                    ListView.Columns[0].Text = "Sorted ascendingly";
                }

                SaveResults();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void LettersTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                GenerateAnagrams(sender, e);

            // Control-A
            if (e.KeyChar == (char)1)
                LettersTextBox.SelectAll();

            m_letters = LettersTextBox.Text + e.KeyChar.ToString();
        }

        private void SaveResults()
        {
            if (Directory.Exists(DICTIONARY_FOLDER))
            {
                string filename = "";
                if (ListView.Sorting == SortOrder.None)
                {
                    filename = DICTIONARY_FOLDER + "/" + LettersTextBox.Text + ".txt";
                }
                else if (ListView.Sorting == SortOrder.Ascending)
                {
                    filename = DICTIONARY_FOLDER + "/" + "Asc_" + LettersTextBox.Text + ".txt";
                }
                else if (ListView.Sorting == SortOrder.Descending)
                {
                    filename = DICTIONARY_FOLDER + "/" + "Desc_" + LettersTextBox.Text + ".txt";
                }

                try
                {
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                    {
                        writer.WriteLine("{0} sentences of '{1}'", ListView.Items.Count, LettersTextBox.Text);
                        writer.WriteLine("-----------------------------------");
                        foreach (ListViewItem it in ListView.Items)
                        {
                            writer.WriteLine(it.Text);
                        }
                    }
                }
                catch
                {
                    // silence IO error in case running from read-only media (CD/DVD)
                }
            }
        }
        private void ViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(DICTIONARY_FOLDER))
            {
                string filename = "";
                if (ListView.Sorting == SortOrder.None)
                {
                    filename = DICTIONARY_FOLDER + "/" + LettersTextBox.Text + ".txt";
                }
                else if (ListView.Sorting == SortOrder.Ascending)
                {
                    filename = DICTIONARY_FOLDER + "/" + "Asc_" + LettersTextBox.Text + ".txt";
                }
                else if (ListView.Sorting == SortOrder.Descending)
                {
                    filename = DICTIONARY_FOLDER + "/" + "Desc_" + LettersTextBox.Text + ".txt";
                }

                // show file content after save
                if (File.Exists(filename))
                {
                    System.Diagnostics.Process.Start("Notepad.exe", filename);
                }
            }
        }
        private void TypeUniqueLettersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LettersTextBox.Text = "ق ص ر ع س ن م ل ك ي ط ح ه ا";
            UniqueLettersToolStripMenuItem.Checked = true;
            UniqueWordsToolStripMenuItem.Checked = false;
            AllWordsToolStripMenuItem.Checked = false;
        }
        private void TypeUniqueWordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LettersTextBox.Text = "الم المص الر المر كهيعص طه طسم طس يس ص حم عسق ق ن";
            UniqueLettersToolStripMenuItem.Checked = false;
            UniqueWordsToolStripMenuItem.Checked = true;
            AllWordsToolStripMenuItem.Checked = false;
        }
        private void TypeAllWordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LettersTextBox.Text = "الم الم المص الر الر الر المر الر الر كهيعص طه طسم طس طسم الم الم الم الم يس ص حم حم عسق حم حم حم حم حم ق ن";
            UniqueLettersToolStripMenuItem.Checked = false;
            UniqueWordsToolStripMenuItem.Checked = false;
            AllWordsToolStripMenuItem.Checked = true;
        }
        private void RunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenerateAnagrams(sender, e);
        }
        private void InitialLettersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (Directory.Exists(HELP_FOLDER))
                {
                    System.Diagnostics.Process.Start(HELP_FOLDER);
                }
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    //Console.WriteLine(ex.Message);
                    MessageBox.Show(ex.Message, Application.ProductName);
                    ex = ex.InnerException;
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(String.Format(
                "Initial Letters - v{0}", Application.ProductVersion + "\r\n"
                + "\r\n"
                + "©2007 Eric Hanchrow - Anagrams" + "\r\n"
                + "http://github.com/offby1/anagrams" + "\r\n"
                + "\r\n" 
                + "©2012 Ali Adams - علي عبد الرزاق عبد الكريم القره غولي" + "\r\n"
                + "http://www.qurancode.com" + "\r\n"
                ),
                Application.ProductName);
        }

        private void GenerateAnagrams(object sender, EventArgs e)
        {
            LettersTextBox.Enabled = false;
            Bag LettersTextBox_bag = new Bag(LettersTextBox.Text);
            ListView.Items.Clear();
            TypeToolStripMenuItem.Enabled = false;
            RunToolStripMenuItem.Enabled = false;
            ViewToolStripMenuItem.Enabled = false;
            m_start_time = DateTime.Now;
            ElapsedTimeLabel.Text = "00:00:00";
            Timer.Enabled = true;
            ProgressBar.Minimum = 0;
            ProgressBar.Value = 0;
            Anagrams.anagrams(LettersTextBox_bag, m_dictionary, 0,

                // bottom of main loop
                delegate()
                {
                    ProgressBar.PerformStep();
                    Application.DoEvents();
                },

                // done pruning
                delegate(uint recursion_level, List<bag_and_anagrams> pruned_dict)
                {
                    if (recursion_level == 0)
                    {
                        ProgressBar.Maximum = pruned_dict.Count;
                        Application.DoEvents();
                    }
                },

                // found a top-level anagram
                delegate(strings words)
                {
                    string display_me = "";
                    foreach (string s in words)
                    {
                        if (display_me.Length > 0)
                            display_me += " ";
                        display_me += s;
                    }

                    ListView.Items.Add(display_me);
                    ListView.EnsureVisible(ListView.Items.Count - 1);
                    ResultLabel.Text = ListView.Items.Count.ToString() + " sentences";
                    if (ListView.Items.Count % 1000 == 0)
                    {
                        Application.DoEvents();
                    }

                });
            Timer.Enabled = false;

            ResultLabel.Text = String.Format("{0} sentences.", ListView.Items.Count);
            if (ListView.Items.Count > 0)
            {
                ListView.EnsureVisible(0);
            }
            LettersTextBox.Enabled = true;
            LettersTextBox.Focus();
            ListView.Columns[0].Text = "Click here to sort";
            TypeToolStripMenuItem.Enabled = true;
            RunToolStripMenuItem.Enabled = true;
            ViewToolStripMenuItem.Enabled = true;

            SaveResults();
        }
    }
}
