using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using Model;

public partial class MainForm : Form
{
    #region 01. Framework
    ///////////////////////////////////////////////////////////////////////////////
    // TextBox has no Ctrl+A by default, so add it
    private void FixMicrosoft(object sender, KeyPressEventArgs e)
    {
        // stop annoying beep due to parent not having an AcceptButton
        if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Escape))
        {
            e.Handled = true;
        }
        // enable Ctrl+A to SelectAll
        if ((ModifierKeys == Keys.Control) && (e.KeyChar == (char)1))
        {
            TextBoxBase control = (sender as TextBoxBase);
            if (control != null)
            {
                control.SelectAll();
                e.Handled = true;
            }
        }
    }
    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (sender is TextBoxBase)
        {
            TextBoxBase control = (sender as TextBoxBase);
            if (control != null)
            {
                if (e.KeyCode == Keys.Tab)
                {
                    control.Text.Insert(control.SelectionStart, "\t");
                    //e.Handled = true;
                }
                else
                {
                    if (ModifierKeys == Keys.Control)
                    {
                        if (e.KeyCode == Keys.A)
                        {
                            control.SelectAll();
                        }
                        else if (e.KeyCode == Keys.F)
                        {
                            // Find dialog
                        }
                        else if (e.KeyCode == Keys.H)
                        {
                            // Replace dialog
                        }
                        else if (e.KeyCode == Keys.S)
                        {
                            // Save As dialog
                        }
                    }
                }
            }
        }
    }
    private void DownloadFile(string url, string path)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            Downloader.Download(url, path, 30000);

            // Async example //
            //using (WebClient web_client = new WebClient())
            //{
            //    web_client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(DownloadDataCompleted);
            //    web_client.DownloadDataAsync(new Uri(url));
            //}
            //private void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
            //{
            //    // WARNING: runs on different thread to UI thread
            //    byte[] raw = e.Result;
            //}
        }
        catch
        {
            // silence
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 02. Constants
    ///////////////////////////////////////////////////////////////////////////////
    private static int c = 140;
    private static Color[] CHAPTER_INITIALIZATION_COLORS =
    { 
        /* InitializationType.Key */                  Color.Black,
        /* InitializationType.PartiallyInitialized */ Color.FromArgb(c+32, c+0, 0),
        /* InitializationType.FullyInitialized */     Color.FromArgb(c+64, c+32, 0),
        /* InitializationType.DoublyInitialized */    Color.FromArgb(c+96, c+64, 0),
        /* InitializationType.NonInitialized */       Color.FromArgb(64, 64, 64),
    };

    private const int DEFAULT_WINDOW_WIDTH = 1134;
    private const int DEFAULT_WINDOW_HEIGHT = 768;
    private const int DEFAULT_INFORMATION_BOX_TOP = 478;
    private const int DEFAULT_AUDIO_VOLUME = 1000;
    private const string VERSE_ADDRESS_TRANSLATION_SEPARATOR = " ";
    private const string VERSE_ADDRESS_MAIN_SEPARATOR = "\t";
    private const string SUM_SYMBOL = "Ʃ";
    private const string SPACE_GAP = "     ";
    private const string CAPTION_SEPARATOR = " ► ";
    private const float DEFAULT_FONT_SIZE = 14.0F;
    private const float DEFAULT_TRANSALTION_FONT_SIZE = 11.0F;
    private const float DEFAULT_TEXT_ZOOM_FACTOR = 1.0F;
    private const int SELECTON_SCOPE_TEXT_MAX_LENGTH = 32;  // for longer text, use elipses (...)
    private const int DEFAULT_RADIX = 10;                   // base for current numbering system. Decimal by default.
    private const int RADIX_NINTEEN = 19;                   // base for current numbering system. 19 for OverItNineteen.
    private const int DEFAULT_DIVISOR = 19;                 // Green background if number is divisible by divisor. 19 for OverItNineteen.
    private const int MIN_DIVISOR = 2;                      // minimum divisor
    private const int MAX_DIVISOR = 9999;                   // maximum divisor
    private const float DEFAULT_DPI_X = 96.0F;              // 100% = 96.0F,   125% = 120.0F,   150% = 144.0F
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 03. MainForm
    ///////////////////////////////////////////////////////////////////////////////
    private float m_dpi_x = DEFAULT_DPI_X;
    private string m_ini_filename = null;
    private Client m_client = null;
    private AboutBox m_about_box = null;
    private string m_current_text = null;
    public MainForm()
    {
        InitializeComponent();
        this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

        m_about_box = new AboutBox();

        using (Graphics graphics = this.CreateGraphics())
        {
            m_dpi_x = graphics.DpiX;
            if (m_dpi_x != DEFAULT_DPI_X)
            {
                if (m_dpi_x == 120.0F)
                {
                    // adjust GUI to fit into 125%
                    MainSplitContainer.Height = (int)(MainSplitContainer.Height / (m_dpi_x / DEFAULT_DPI_X)) + 96;
                    MainSplitContainer.SplitterDistance = 215;
                }
            }
        }

        FindByTextButton.Enabled = true;
        FindBySimilarityButton.Enabled = false;
        FindByNumbersButton.Enabled = false;
        FindByFrequencyButton.Enabled = false;

        m_ini_filename = AppDomain.CurrentDomain.FriendlyName.Replace(".exe", ".ini");

        // must initialize here as it is null
        m_active_textbox = MainTextBox;

        this.MainTextBox.HideSelection = false; // this won't shift the text to the left
        //this.MainTextBox.HideSelection = true; // this WILL shift the text to the left
        this.SearchResultTextBox.HideSelection = false; // this won't shift the text to the left
        //this.SearchResultTextBox.HideSelection = true; // this WILL shift the text to the left

        this.MainTextBox.MouseWheel += new MouseEventHandler(MainTextBox_MouseWheel);
        this.SearchResultTextBox.MouseWheel += new MouseEventHandler(MainTextBox_MouseWheel);
        this.TranslationTextBox.MouseWheel += new MouseEventHandler(MainTextBox_MouseWheel);
    }
    private void MainForm_Load(object sender, EventArgs e)
    {
        bool splash_screen_done = false;
        try
        {
            SplashForm splash_form = new SplashForm();
            if (splash_form != null)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    using (splash_form)
                    {
                        splash_form.Show();
                        while (!splash_screen_done)
                        {
                            Application.DoEvents();
                        }
                        splash_form.Close();
                    }
                }, null);

                splash_form.Version += " - " + Globals.SHORT_VERSION;

                InitializeControls();
                splash_form.Information = "Initializing Server ...";
                splash_form.Progress = 10;
                Thread.Sleep(100);


                string numerology_system_name = LoadNumerologySystemName();
                m_client = new Client(numerology_system_name);
                if (m_client != null)
                {
                    if (m_client.NumerologySystem != null)
                    {
                        LoadApplicationFolders();

                        LoadTextModeSettings();
                        splash_form.Information = "Building book ...";
                        string text_mode = m_client.NumerologySystem.TextMode;
                        m_client.BuildSimplifiedBook(text_mode, m_with_bism_Allah, m_waw_as_word, m_shadda_as_letter);
                        EnableFindByTextControls();
                        splash_form.Progress = 40;
                        Thread.Sleep(100);

                        if (m_client.Book != null)
                        {
                            UpdateNumericMinMax();

                            splash_form.Information = "Loading translation info ...";
                            PopulateTranslatorsCheckedListBox();
                            PopulateTranslatorComboBox();
                            splash_form.Progress = 50;
                            Thread.Sleep(100);

                            splash_form.Information = "Loading recitation info ...";
                            PopulateRecitationsCheckedListBox();
                            PopulateReciterComboBox();
                            splash_form.Progress = 60;
                            Thread.Sleep(100);

                            splash_form.Information = "Loading chapter names ...";
                            PopulateChapterComboBox();
                            PopulateChaptersListBox();
                            splash_form.Progress = 63;
                            Thread.Sleep(100);

                            splash_form.Information = "Loading user settings ...";
                            LoadApplicationSettings();
                            splash_form.Progress = 70;
                            Thread.Sleep(100);

                            if (m_client.Selection != null)
                            {
                                splash_form.Information = "Loading numerology systems ...";
                                PopulateTextModeComboBox();
                                UpdateNumerologySystemControls();
                                splash_form.Progress = 80;
                                Thread.Sleep(100);

                                splash_form.Information = "Loading bookmarks and notes ...";
                                m_client.LoadBookmarks();
                                UpdateBookmarkButtons();
                                splash_form.Progress = 90;
                                Thread.Sleep(100);

                                splash_form.Information = "Loading search history ...";
                                m_client.LoadHistoryItems();
                                UpdateSelectionHistoryButtons();
                                splash_form.Progress = 95;
                                Thread.Sleep(100);

                                splash_form.Information = "Loading help messages ...";
                                if (m_client.HelpMessages != null)
                                {
                                    if (m_client.HelpMessages.Count > 0)
                                    {
                                        HelpMessageLabel.Text = m_client.HelpMessages[0];
                                    }
                                }
                                splash_form.Progress = 98;
                                Thread.Sleep(100);

                                if (ReciterComboBox.SelectedItem != null)
                                {
                                    RecitationGroupBox.Text = ReciterComboBox.SelectedItem.ToString() + "                                 ";
                                }
                                ToolTip.SetToolTip(PlayerVolumeTrackBar, "Volume " + (m_audio_volume / (1000 / PlayerVolumeTrackBar.Maximum)).ToString() + "%");

                                // prepare before Shown
                                this.ClientSplitContainer.SplitterDistance = m_information_box_top;
                                this.TabControl.SelectedIndex = m_information_page_index;

                                // must be before DisplaySelection for Verse.IncludeNumber to take effect
                                ApplyWordWrapSettings();

                                this.m_player_looping = !this.m_player_looping;
                                PlayerRepeatLabel_Click(null, null);
                                this.m_player_looping_all = !this.m_player_looping_all;
                                PlayerRepeatAllLabel_Click(null, null);

                                SetupFont();

                                UpdateTextModeOptions();

                                splash_form.Information = "Preparing text to display ...";
                                DisplaySelection(false);
                                splash_form.Progress = 100;
                                Thread.Sleep(100);
                            }

                            if (
                                 (m_text_display_mode == TextDisplayMode.None) ||
                                 (m_text_display_mode == TextDisplayMode.TranslationOnly)
                               )
                            {
                                // fill MainTextBox.Text with anything,
                                // don't leave empty to allow live statistics
                                MainTextBox.Text = "Fast Mode";
                            }
                        }
                        this.Activate(); // bring to foreground
                    }
                }
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
            splash_screen_done = true;
            Thread.Sleep(100);  // prevent race-condition to allow splashform.Close()
        }
    }
    private void MainForm_Shown(object sender, EventArgs e)
    {
        MainTextBox.AlignToStart();

        // start user at chapter list box
        ChaptersListBox.Focus();
    }
    private void MainForm_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            if (AutoCompleteListBox.Focused)
            {
                AutoCompleteListBox_DoubleClick(null, null);
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }
        if (e.KeyCode == Keys.Tab)
        {
            e.Handled = false;
        }
        else if (e.KeyCode == Keys.Escape)
        {
            HandleEscapeKeyPress(null, null);
            e.Handled = true; // stop annoying beep
        }
        else if (e.Control && (e.KeyCode == Keys.Down))
        {
        }
        else if (e.Control && (e.KeyCode == Keys.Up))
        {
        }
        else if (e.Control && (e.KeyCode == Keys.A)) // SelectAll chapters
        {
            if (ChaptersListBox.Focused)
            {
                ChaptersListBox.SelectedIndexChanged -= new EventHandler(ChaptersListBox_SelectedIndexChanged);
                for (int i = 0; i < ChaptersListBox.Items.Count - 1; i++)
                {
                    ChaptersListBox.SelectedIndices.Add(i);
                }
                ChaptersListBox.SelectedIndexChanged += new EventHandler(ChaptersListBox_SelectedIndexChanged);
                ChaptersListBox.SelectedIndices.Add(ChaptersListBox.Items.Count - 1);
            }
            else if (AutoCompleteListBox.Focused)
            {
                AutoCompleteListBox.SelectedIndexChanged -= new EventHandler(AutoCompleteListBox_SelectedIndexChanged);
                for (int i = 0; i < AutoCompleteListBox.Items.Count - 1; i++)
                {
                    AutoCompleteListBox.SelectedIndices.Add(i);
                }
                AutoCompleteListBox.SelectedIndexChanged += new EventHandler(AutoCompleteListBox_SelectedIndexChanged);
                AutoCompleteListBox.SelectedIndices.Add(AutoCompleteListBox.Items.Count - 1);
            }
            else
            {
                e.Handled = false;
            }
        }
        else
        {
            if (!e.Alt && !e.Control && !e.Shift)
            {
                if ((e.KeyCode == Keys.Back) || (e.KeyCode == Keys.BrowserBack))
                {
                    if (m_active_textbox != null)
                    {
                        if (
                            ((m_active_textbox.Focused) && (m_readonly_translation)) ||
                            (SelectionHistoryBackwardButton.Focused) ||
                            (SelectionHistoryForwardButton.Focused) ||
                            (SelectionHistoryCounterLabel.Focused)
                           )
                        {
                            SelectionHistoryBackwardButton_Click(null, null);
                            e.Handled = true; // stop annoying beep
                        }
                    }
                }
                else if ((e.KeyCode == Keys.BrowserForward))
                {
                    if (m_active_textbox != null)
                    {
                        if (
                            ((m_active_textbox.Focused) && (m_readonly_translation)) ||
                            (SelectionHistoryBackwardButton.Focused) ||
                            (SelectionHistoryForwardButton.Focused) ||
                            (SelectionHistoryCounterLabel.Focused)
                           )
                        {
                            SelectionHistoryForwardButton_Click(null, null);
                            e.Handled = true; // stop annoying beep
                        }
                    }
                }
                else if (e.KeyCode == Keys.F1)
                {
                    HelpMessageLabel.Visible = true;
                }
                else if (e.KeyCode == Keys.F2)
                {
                    HelpMessageLabel.Visible = false;
                }
                else if (e.KeyCode == Keys.F3)
                {
                    if (m_found_verses_displayed)
                    {
                        SelectNextFindMatch();
                    }
                    else
                    {
                        NextBookmarkButton_Click(null, null);
                    }
                }
                else if (e.KeyCode == Keys.F4)
                {
                    if (m_active_textbox != null)
                    {
                        if (m_active_textbox.Focused)
                        {
                            this.Cursor = Cursors.WaitCursor;
                            try
                            {
                                DoFindExactText(m_active_textbox);
                            }
                            finally
                            {
                                this.Cursor = Cursors.Default;
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.F5)
                {
                    if (m_active_textbox != null)
                    {
                        if (m_active_textbox.Focused)
                        {
                            this.Cursor = Cursors.WaitCursor;
                            try
                            {
                                DoFindExactWords(m_active_textbox);
                            }
                            finally
                            {
                                this.Cursor = Cursors.Default;
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.F6)
                {
                    if (m_active_textbox != null)
                    {
                        if (m_active_textbox.Focused)
                        {
                            this.Cursor = Cursors.WaitCursor;
                            try
                            {
                                DoFindSimilarWords(m_active_textbox);
                            }
                            finally
                            {
                                this.Cursor = Cursors.Default;
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.F7)
                {
                    if (m_active_textbox != null)
                    {
                        if (m_active_textbox.Focused)
                        {
                            this.Cursor = Cursors.WaitCursor;
                            try
                            {
                                DoFindSimilarVerses(m_active_textbox);
                            }
                            finally
                            {
                                this.Cursor = Cursors.Default;
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.F8)
                {
                    if (m_active_textbox != null)
                    {
                        if (m_active_textbox.Focused)
                        {
                            this.Cursor = Cursors.WaitCursor;
                            try
                            {
                                DoFindRelatedWords(m_active_textbox);
                            }
                            finally
                            {
                                this.Cursor = Cursors.Default;
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.F9)
                {
                    if (m_active_textbox != null)
                    {
                        if (m_active_textbox.Focused)
                        {
                            this.Cursor = Cursors.WaitCursor;
                            try
                            {
                                DoFindSameValue(m_active_textbox);
                            }
                            finally
                            {
                                this.Cursor = Cursors.Default;
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.F10)
                {
                }
                else if (e.KeyCode == Keys.F11)
                {
                    WordWrapLabel_Click(null, null);
                }
                else if (e.KeyCode == Keys.F12)
                {
                    if (this.WindowState != FormWindowState.Maximized)
                    {
                        this.WindowState = FormWindowState.Maximized;
                        this.FormBorderStyle = FormBorderStyle.None;
                    }
                    else
                    {
                        this.WindowState = FormWindowState.Normal;
                        this.FormBorderStyle = FormBorderStyle.Sizable;
                    }
                }
                else
                {
                    // let editor process key
                }
            }
            else if (!e.Alt && !e.Control && e.Shift)
            {
                if ((e.KeyCode == Keys.Back) || (e.KeyCode == Keys.BrowserBack))
                {
                    if (m_active_textbox != null)
                    {
                        if (
                            ((m_active_textbox.Focused) && (m_readonly_translation)) ||
                            (SelectionHistoryBackwardButton.Focused) ||
                            (SelectionHistoryForwardButton.Focused) ||
                            (SelectionHistoryCounterLabel.Focused)
                           )
                        {
                            SelectionHistoryForwardButton_Click(null, null);
                            e.Handled = true; // stop annoying beep
                        }
                    }
                }
                else if ((e.KeyCode == Keys.BrowserForward))
                {
                    if (m_active_textbox != null)
                    {
                        if (
                            ((m_active_textbox.Focused) && (m_readonly_translation)) ||
                            (SelectionHistoryBackwardButton.Focused) ||
                            (SelectionHistoryForwardButton.Focused) ||
                            (SelectionHistoryCounterLabel.Focused)
                           )
                        {
                            SelectionHistoryBackwardButton_Click(null, null);
                            e.Handled = true; // stop annoying beep
                        }
                    }
                }
                else if (e.KeyCode == Keys.F1)
                {
                }
                else if (e.KeyCode == Keys.F2)
                {
                }
                else if (e.KeyCode == Keys.F3)
                {
                    if (m_found_verses_displayed)
                    {
                        SelectPreviousFindMatch();
                    }
                    else
                    {
                        PreviousBookmarkButton_Click(null, null);
                    }
                }
                else if (e.KeyCode == Keys.F4)
                {
                }
                else if (e.KeyCode == Keys.F5)
                {
                }
                else if (e.KeyCode == Keys.F6)
                {
                }
                else if (e.KeyCode == Keys.F7)
                {
                }
                else if (e.KeyCode == Keys.F8)
                {
                }
                else if (e.KeyCode == Keys.F9)
                {
                }
                else if (e.KeyCode == Keys.F10)
                {
                }
                else if (e.KeyCode == Keys.F11)
                {
                }
                else if (e.KeyCode == Keys.F12)
                {
                }
                else
                {
                    // let editor process key
                }
            }
        }
    }
    private void MainForm_Resize(object sender, EventArgs e)
    {
        if (m_player != null)
        {
            if (m_player.Closed)
            {
                Verse verse = GetVerse(CurrentVerseIndex);
                if (verse != null)
                {
                    if (m_active_textbox != null)
                    {
                        int start = m_active_textbox.SelectionStart;
                        int length = m_active_textbox.SelectionLength;
                        m_active_textbox.AlignToLineStart();
                        m_active_textbox.Select(start, length);
                    }
                }
            }
        }
    }
    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        //// prevent user from closing from the X close button
        //if (e.CloseReason == CloseReason.UserClosing)
        //{
        //    e.Cancel = true;
        //    this.Visible = false;
        //}
    }
    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        CloseApplication();
    }
    private void CloseApplication()
    {
        try
        {
            // save current note (if any)
            BookmarkTextBox_Leave(null, null);

            if (m_client != null)
            {
                // save bookmarks of all texts of current client
                m_client.SaveBookmarks();

                // save user history
                m_client.SaveHistoryItems();
            }

            // save application options
            SaveApplicationOptions();

            PlayerStopLabel_Click(null, null);
        }
        catch
        {
            // silence IO error in case running from read-only media (CD/DVD)
        }
    }
    private void HandleEscapeKeyPress(object sender, KeyEventArgs e)
    {
        if (NumerologySystemComboBox.DroppedDown)
        {
            NumerologySystemComboBox.DroppedDown = false;
        }
        else if (TranslatorComboBox.DroppedDown)
        {
            TranslatorComboBox.DroppedDown = false;
        }
        else if (ReciterComboBox.DroppedDown)
        {
            ReciterComboBox.DroppedDown = false;
        }
        else if (BookmarkTextBox.Focused)
        {
            BookmarkTextBox.Text = null;
        }
        else if (TranslatorsCheckedListBox.Visible)
        {
            TranslationsCancelSettingsLabel_Click(null, null);
        }
        else if (RecitationsDownloadGroupBox.Visible)
        {
            RecitationsCancelSettingsLabel_Click(null, null);
        }
        else if (TranslationTextBox.Focused)
        {
            DisplayTranslations(new List<Verse>(m_translated_verses));
        }
        else if (!m_readonly_translation)
        {
            EditVerseTranslationLabel_Click(null, null);
        }
        else
        {
            if (m_found_verses_displayed)
            {
                if (ChaptersListBox.SelectedIndices.Count > 0)
                {
                    ChaptersListBox.SelectedIndices.Clear();
                    int pos = m_find_result_header.IndexOf(" of ");
                    if (pos != -1)
                    {
                        m_find_result_header = m_find_result_header.Substring(pos + 4);
                    }

                    m_client.FilterChapters = null;
                    ClearFindMatches(); // clear m_find_matches for F3 to work correctly in filtered result
                    if (m_search_type == SearchType.Numbers)
                    {
                        switch (m_numbers_result_type)
                        {
                            case NumbersResultType.VerseRanges:
                                DisplayFoundVerseRanges(false, false);
                                break;
                            case NumbersResultType.VerseSets:
                                DisplayFoundVerseSets(false, false);
                                break;
                            case NumbersResultType.Chapters:
                                DisplayFoundChapters(false, false);
                                break;
                            case NumbersResultType.ChapterRanges:
                                DisplayFoundChapterRanges(false, false);
                                break;
                            case NumbersResultType.ChapterSets:
                                DisplayFoundChapterSets(false, false);
                                break;
                            default:
                                DisplayFoundVerses(false, false);
                                break;
                        }
                    }
                    else
                    {
                        DisplayFoundVerses(false, false);
                    }
                }
                else
                {
                    SwitchActiveTextBox();
                }
            }
            else
            {
                SwitchActiveTextBox();
            }
        }
    }
    private void SwitchActiveTextBox()
    {
        if (m_active_textbox != null)
        {
            if (m_found_verses_displayed)
            {
                SwitchToMainTextBox();
            }
            else
            {
                SwitchToSearchResultTextBox();
            }

            PlayerStopLabel_Click(null, null);

            // this code has been moved out of SelectionChanged and brought to MouseClick and KeyUp
            // to keep all verse translations visible until the user clicks a verse then show one verse translation
            if (m_active_textbox.SelectionLength == 0)
            {
                Verse verse = GetVerse(CurrentVerseIndex);
                if (verse != null)
                {
                    DisplayTranslations(verse);
                }
                else
                {
                    TranslationTextBox.WordWrap = m_active_textbox.WordWrap;
                    TranslationTextBox.Text = null;
                    TranslationTextBox.Refresh();

                    m_readonly_translation = false;
                    TranslationTextBox_ToggleReadOnly();
                    EditVerseTranslationLabel.Visible = false;
                }
            }
            else
            {
                // selected text is dealt with by CalculateAndDisplayCounts 
            }

            UpdateHeaderLabel();

            m_active_textbox.Focus();
            MainTextBox_SelectionChanged(m_active_textbox, null);
        }
    }
    private void UpdateNumericMinMax()
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                PageNumericUpDown.Minimum = 1;
                PageNumericUpDown.Maximum = m_client.Book.Pages.Count;
                StationNumericUpDown.Minimum = 1;
                StationNumericUpDown.Maximum = m_client.Book.Stations.Count;
                PartNumericUpDown.Minimum = 1;
                PartNumericUpDown.Maximum = m_client.Book.Parts.Count;
                GroupNumericUpDown.Minimum = 1;
                GroupNumericUpDown.Maximum = m_client.Book.Groups.Count;
                HalfNumericUpDown.Minimum = 1;
                HalfNumericUpDown.Maximum = m_client.Book.Halfs.Count;
                QuarterNumericUpDown.Minimum = 1;
                QuarterNumericUpDown.Maximum = m_client.Book.Quarters.Count;
                BowingNumericUpDown.Minimum = 1;
                BowingNumericUpDown.Maximum = m_client.Book.Bowings.Count;
                PageNumericUpDown.Minimum = 1;
                PageNumericUpDown.Maximum = m_client.Book.Pages.Count;
                VerseNumericUpDown.Minimum = 1;
                VerseNumericUpDown.Maximum = m_client.Book.Verses.Count;
            }
        }
    }
    private void UpdateVersesToCurrentTextMode(ref List<Verse> verses)
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                if (m_client.Book.Verses != null)
                {
                    List<Verse> temp = new List<Verse>();
                    if (verses != null)
                    {
                        foreach (Verse verse in verses)
                        {
                            temp.Add(m_client.Book.Verses[verse.Number - 1]);
                        }
                    }
                    verses = temp;
                }
            }
        }
    }
    private void LoadApplicationSettings()
    {
        try
        {
            // must be after the populates...
            LoadApplicationOptions();

            RadixValueLabel.Text = m_radix.ToString();

            // WARNING: updates size BUT loses the font face in right-to-left RichTextBox
            //SetFontSize(m_font_size);
            // so use ZoomFactor instead
            MainTextBox.ZoomFactor = m_text_zoom_factor;
            SearchResultTextBox.ZoomFactor = m_text_zoom_factor;
            ScaleInformationBoxFont(m_text_zoom_factor);

            PlayerVolumeTrackBar.Value = m_audio_volume / (1000 / PlayerVolumeTrackBar.Maximum);
            PlayerVerseSilenceGapTrackBar.Value = (int)(m_silence_between_verses * (PlayerVerseSilenceGapTrackBar.Maximum / 2));
            SetToolTipPlayerVerseSilenceGapTrackBar();
            PlayerSelectionSilenceGapTrackBar.Value = m_silence_between_selections;
            SetToolTipPlayerSelectionSilenceGapTrackBar();
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
    }
    private void LoadApplicationFolders()
    {
        if (File.Exists(m_ini_filename))
        {
            try
            {
                using (StreamReader reader = File.OpenText(m_ini_filename))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (!String.IsNullOrEmpty(line))
                        {
                            string[] parts = line.Split('=');
                            if (parts.Length == 2)
                            {
                                switch (parts[0])
                                {
                                    // [Folders]
                                    case "NumbersFolder":
                                        {
                                            Numbers.NUMBERS_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "FontsFolder":
                                        {
                                            Globals.FONTS_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "ImagesFolder":
                                        {
                                            Globals.IMAGES_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "DataFolder":
                                        {
                                            Globals.DATA_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "AudioFolder":
                                        {
                                            Globals.AUDIO_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "TranslationsFolder":
                                        {
                                            Globals.TRANSLATIONS_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "RulesFolder":
                                        {
                                            Globals.RULES_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "ValuesFolder":
                                        {
                                            Globals.VALUES_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "StatisticsFolder":
                                        {
                                            Globals.STATISTICS_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "BookmarksFolder":
                                        {
                                            Globals.BOOKMARKS_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "HistoryFolder":
                                        {
                                            Globals.HISTORY_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "HelpFolder":
                                        {
                                            Globals.HELP_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // silence Parse exceptions
                // continue with next INI entry
            }
        }
    }
    private string LoadNumerologySystemName()
    {
        if (File.Exists(m_ini_filename))
        {
            using (StreamReader reader = File.OpenText(m_ini_filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!String.IsNullOrEmpty(line))
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            if (parts[0] == "NumerologySystem")
                            {
                                try
                                {
                                    return parts[1].Trim();
                                }
                                catch
                                {
                                    return NumerologySystem.DEFAULT_NAME;
                                }
                            }
                        }
                    }
                }
            }
        }
        return NumerologySystem.DEFAULT_NAME;
    }
    private string LoadTextModeSettings()
    {
        if (File.Exists(m_ini_filename))
        {
            using (StreamReader reader = File.OpenText(m_ini_filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!String.IsNullOrEmpty(line))
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            switch (parts[0])
                            {
                                case "WithBismAllah":
                                    {
                                        try
                                        {
                                            this.m_with_bism_Allah = bool.Parse(parts[1].Trim());
                                        }
                                        catch
                                        {
                                            this.m_with_bism_Allah = true;
                                        }
                                    }
                                    break;
                                case "WawAsWord":
                                    {
                                        try
                                        {
                                            this.m_waw_as_word = bool.Parse(parts[1].Trim());
                                        }
                                        catch
                                        {
                                            this.m_waw_as_word = false;
                                        }
                                    }
                                    break;
                                case "ShaddaAsLetter":
                                    {
                                        try
                                        {
                                            this.m_shadda_as_letter = bool.Parse(parts[1].Trim());
                                        }
                                        catch
                                        {
                                            this.m_shadda_as_letter = false;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }
        return NumerologySystem.DEFAULT_NAME;
    }
    private void LoadApplicationOptions()
    {
        try
        {
            if (m_client != null)
            {
                if (File.Exists(m_ini_filename))
                {
                    // Selection.Scope and Selection.Indexes are immutable/readonly so create a new Selection to replace m_client.Selection 
                    SelectionScope selection_scope = SelectionScope.Book;
                    List<int> selection_indexes = new List<int>();

                    using (StreamReader reader = File.OpenText(m_ini_filename))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (!String.IsNullOrEmpty(line))
                            {
                                string[] parts = line.Split('=');
                                if (parts.Length == 2)
                                {
                                    switch (parts[0])
                                    {
                                        // [Window]
                                        case "Top":
                                            {
                                                try
                                                {
                                                    this.Top = int.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.Top = 100;
                                                }
                                            }
                                            break;
                                        case "Left":
                                            {
                                                try
                                                {
                                                    this.Left = int.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.Left = 100;
                                                }
                                            }
                                            break;
                                        case "Width":
                                            {
                                                try
                                                {
                                                    this.Width = int.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.Width = DEFAULT_WINDOW_WIDTH;
                                                }
                                            }
                                            break;
                                        case "Height":
                                            {
                                                try
                                                {
                                                    this.Height = int.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.Height = DEFAULT_WINDOW_HEIGHT;
                                                }
                                            }
                                            break;
                                        case "InformationBoxTop":
                                            {
                                                try
                                                {
                                                    this.m_information_box_top = int.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.m_information_box_top = DEFAULT_INFORMATION_BOX_TOP;
                                                }
                                            }
                                            break;
                                        case "InformationPageIndex":
                                            {
                                                try
                                                {
                                                    this.m_information_page_index = int.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.m_information_page_index = 0;
                                                }
                                            }
                                            break;
                                        case "Translator":
                                            {
                                                try
                                                {
                                                    int index = int.Parse(parts[1].Trim());
                                                    if (index < this.TranslatorComboBox.Items.Count)
                                                    {
                                                        this.TranslatorComboBox.SelectedIndex = index;
                                                    }
                                                    else
                                                    {
                                                        this.TranslatorComboBox.SelectedItem = -1;
                                                    }
                                                }
                                                catch
                                                {
                                                    if (this.TranslatorComboBox.Items.Count >= 3)
                                                    {
                                                        this.TranslatorComboBox.SelectedItem = m_client.Book.TranslationInfos[Client.DEFAULT_TRANSLATION].Name;
                                                    }
                                                    else
                                                    {
                                                        this.TranslatorComboBox.SelectedIndex = -1;
                                                    }
                                                }
                                            }
                                            break;
                                        // [Numbers]
                                        case "Radix":
                                            {
                                                try
                                                {
                                                    m_radix = int.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    m_radix = DEFAULT_RADIX;
                                                }
                                                RadixValueLabel.Text = m_radix.ToString();
                                            }
                                            break;
                                        case "GreenDivisor":
                                            {
                                                try
                                                {
                                                    m_divisor = int.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    m_divisor = DEFAULT_DIVISOR;
                                                }
                                                DivisorValueLabel.Text = m_divisor.ToString();
                                            }
                                            break;
                                        // [Display]
                                        case "MainTextWordWrap":
                                            {
                                                try
                                                {
                                                    this.m_word_wrap_main_textbox = bool.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.m_word_wrap_main_textbox = false;
                                                }
                                            }
                                            break;
                                        case "SearchResultWordWrap":
                                            {
                                                try
                                                {
                                                    this.m_word_wrap_search_textbox = bool.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.m_word_wrap_search_textbox = false;
                                                }
                                            }
                                            break;
                                        case "SelectionScope":
                                            {
                                                try
                                                {
                                                    selection_scope = (SelectionScope)Enum.Parse(typeof(SelectionScope), parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    selection_scope = SelectionScope.Chapter;
                                                }
                                            }
                                            break;
                                        case "SelectionIndexes":
                                            {
                                                try
                                                {
                                                    string part = parts[1].Trim();
                                                    string[] sub_parts = part.Split('+');
                                                    selection_indexes.Clear();
                                                    for (int i = 0; i < sub_parts.Length; i++)
                                                    {
                                                        int index = int.Parse(sub_parts[i].Trim()) - 1;
                                                        selection_indexes.Add(index);
                                                    }
                                                    m_client.Selection = new Selection(m_client.Book, selection_scope, selection_indexes);
                                                }
                                                catch
                                                {
                                                    selection_indexes.Add(0);
                                                }
                                            }
                                            break;
                                        case "FontSize":
                                            {
                                                try
                                                {
                                                    this.m_font_size = float.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.m_font_size = DEFAULT_FONT_SIZE;
                                                }
                                            }
                                            break;
                                        case "TextZoomFactor":
                                            {
                                                try
                                                {
                                                    this.m_text_zoom_factor = float.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.m_text_zoom_factor = DEFAULT_TEXT_ZOOM_FACTOR;
                                                }
                                            }
                                            break;
                                        // [Audio]
                                        case "Reciter":
                                            {
                                                try
                                                {
                                                    int index = int.Parse(parts[1].Trim());
                                                    if (index < this.ReciterComboBox.Items.Count)
                                                    {
                                                        this.ReciterComboBox.SelectedIndex = index;
                                                    }
                                                    else
                                                    {
                                                        this.ReciterComboBox.SelectedItem = -1;
                                                    }
                                                }
                                                catch
                                                {
                                                    this.ReciterComboBox.SelectedItem = m_client.Book.RecitationInfos[Client.DEFAULT_RECITATION].Reciter;
                                                }
                                            }
                                            break;
                                        case "Volume":
                                            {
                                                try
                                                {
                                                    this.m_audio_volume = int.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.m_audio_volume = DEFAULT_AUDIO_VOLUME;
                                                }
                                            }
                                            break;
                                        case "VerseRepetitions":
                                            {
                                                try
                                                {
                                                    this.m_player_looping_count = int.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.m_player_looping_count = int.MaxValue; // infinite
                                                }
                                            }
                                            break;
                                        case "VerseRepetitionsEnabled":
                                            {
                                                try
                                                {
                                                    this.m_player_looping = bool.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.m_player_looping = false;
                                                }
                                            }
                                            break;
                                        case "SelectionRepetitions":
                                            {
                                                try
                                                {
                                                    this.m_player_looping_all_count = int.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.m_player_looping_all_count = int.MaxValue; // infinite
                                                }
                                            }
                                            break;
                                        case "SelectionRepetitionsEnabled":
                                            {
                                                try
                                                {
                                                    this.m_player_looping_all = bool.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.m_player_looping_all = false;
                                                }
                                            }
                                            break;
                                        case "SilenceBetweenVerses":
                                            {
                                                try
                                                {
                                                    this.m_silence_between_verses = float.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.m_silence_between_verses = 0.0F;
                                                }
                                            }
                                            break;
                                        case "SilenceBetweenSelections":
                                            {
                                                try
                                                {
                                                    this.m_silence_between_selections = int.Parse(parts[1].Trim());
                                                }
                                                catch
                                                {
                                                    this.m_silence_between_selections = 0;
                                                }
                                            }
                                            break;
                                        // [Downloads]
                                        case "TranslationUrlPrefix":
                                            {
                                                try
                                                {
                                                    TranslationInfo.UrlPrefix = parts[1].Trim();
                                                }
                                                catch
                                                {
                                                    TranslationInfo.UrlPrefix = TranslationInfo.DEFAULT_URL_PREFIX;
                                                }
                                            }
                                            break;
                                        case "TranslationFileType":
                                            {
                                                try
                                                {
                                                    TranslationInfo.FileType = parts[1].Trim();
                                                }
                                                catch
                                                {
                                                    TranslationInfo.FileType = TranslationInfo.DEFAULT_FILE_TYPE;
                                                }
                                            }
                                            break;
                                        case "TranslationIconUrlPrefix":
                                            {
                                                try
                                                {
                                                    TranslationInfo.IconUrlPrefix = parts[1].Trim();
                                                }
                                                catch
                                                {
                                                    TranslationInfo.IconUrlPrefix = TranslationInfo.DEFAULT_ICON_URL_PREFIX;
                                                }
                                            }
                                            break;
                                        case "RecitationUrlPrefix":
                                            {
                                                try
                                                {
                                                    RecitationInfo.UrlPrefix = parts[1].Trim();
                                                }
                                                catch
                                                {
                                                    RecitationInfo.UrlPrefix = RecitationInfo.DEFAULT_URL_PREFIX;
                                                }
                                            }
                                            break;
                                        case "RecitationFileType":
                                            {
                                                try
                                                {
                                                    RecitationInfo.FileType = parts[1].Trim();
                                                }
                                                catch
                                                {
                                                    RecitationInfo.FileType = RecitationInfo.DEFAULT_FILE_TYPE;
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                else // first Application launch
                {
                    this.StartPosition = FormStartPosition.CenterScreen;
                    this.Width = DEFAULT_WINDOW_WIDTH;
                    this.Height = DEFAULT_WINDOW_HEIGHT;

                    if (this.ChapterComboBox.Items.Count > 1)
                    {
                        this.ChapterComboBox.SelectedIndex = 0;
                    }

                    string[] parts = NumerologySystem.DEFAULT_NAME.Split('_');
                    if (parts.Length == 3)
                    {
                        if (this.TextModeComboBox.Items.Count > 1)
                        {
                            this.TextModeComboBox.SelectedItem = parts[0];
                        }
                        if (this.NumerologySystemComboBox.Items.Count > 1)
                        {
                            this.NumerologySystemComboBox.SelectedItem = parts[1] + "_" + parts[2];
                        }
                    }

                    if (this.TextModeComboBox.Items.Count > 1)
                    {
                        this.TextModeComboBox.SelectedIndex = 0;
                    }

                    if (this.TranslatorComboBox.Items.Count >= 3)
                    {
                        this.TranslatorComboBox.SelectedItem = m_client.Book.TranslationInfos[Client.DEFAULT_TRANSLATION].Name;
                    }

                    // select chapter Al-Fatiha as default
                    m_client.Selection = new Selection(m_client.Book, SelectionScope.Chapter, new List<int>() { 0 });
                }
            }
        }
        catch
        {
            // silence Parse exceptions
            // continue with next INI entry
        }
    }
    private void SaveApplicationOptions()
    {
        try
        {
            if (m_client != null)
            {
                using (StreamWriter writer = new StreamWriter(m_ini_filename, false, Encoding.Unicode))
                {
                    if (this.WindowState == FormWindowState.Maximized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }

                    writer.WriteLine("[Window]");
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        // restore or width/height will be saved as 0
                        writer.WriteLine("Top" + "=" + (Screen.PrimaryScreen.WorkingArea.Height - DEFAULT_WINDOW_HEIGHT) / 2);
                        writer.WriteLine("Left" + "=" + (Screen.PrimaryScreen.WorkingArea.Width - DEFAULT_WINDOW_WIDTH) / 2);
                        writer.WriteLine("Width" + "=" + DEFAULT_WINDOW_WIDTH);
                        writer.WriteLine("Height" + "=" + DEFAULT_WINDOW_HEIGHT);
                    }
                    else
                    {
                        writer.WriteLine("Top" + "=" + this.Top);
                        writer.WriteLine("Left" + "=" + this.Left);
                        writer.WriteLine("Width" + "=" + this.Width);
                        writer.WriteLine("Height" + "=" + this.Height);
                    }
                    writer.WriteLine("InformationBoxTop" + "=" + this.m_information_box_top);
                    writer.WriteLine("InformationPageIndex" + "=" + this.m_information_page_index);
                    writer.WriteLine("Translator" + "=" + this.TranslatorComboBox.SelectedIndex);
                    writer.WriteLine();

                    writer.WriteLine("[Numbers]");
                    writer.WriteLine("Radix" + "=" + m_radix);
                    writer.WriteLine("GreenDivisor" + "=" + m_divisor);
                    if (m_client.NumerologySystem != null)
                    {
                        writer.WriteLine("NumerologySystem" + "=" + m_client.NumerologySystem.Name);
                    }
                    writer.WriteLine();

                    writer.WriteLine("[Text]");
                    writer.WriteLine("WithBismAllah" + "=" + this.m_with_bism_Allah);
                    writer.WriteLine("WawAsWord" + "=" + this.m_waw_as_word);
                    writer.WriteLine("ShaddaAsLetter" + "=" + this.m_shadda_as_letter);
                    writer.WriteLine();

                    writer.WriteLine("[Display]");
                    writer.WriteLine("MainTextWordWrap" + "=" + this.m_word_wrap_main_textbox);
                    writer.WriteLine("SearchResultWordWrap" + "=" + this.m_word_wrap_search_textbox);
                    if (m_client != null)
                    {
                        if (m_client.Selection != null)
                        {
                            writer.WriteLine("SelectionScope" + "=" + (int)m_client.Selection.Scope);
                            StringBuilder str = new StringBuilder("SelectionIndexes=");
                            if (m_client.Selection.Indexes.Count > 0)
                            {
                                foreach (int index in m_client.Selection.Indexes)
                                {
                                    str.Append((index + 1).ToString() + "+");
                                }
                                if (str.Length > 1)
                                {
                                    str.Remove(str.Length - 1, 1);
                                }
                            }
                            writer.WriteLine(str);
                        }
                    }
                    writer.WriteLine("FontSize" + "=" + this.m_font_size);
                    writer.WriteLine("TextZoomFactor" + "=" + this.m_text_zoom_factor);
                    writer.WriteLine();

                    writer.WriteLine("[Audio]");
                    writer.WriteLine("Reciter" + "=" + this.ReciterComboBox.SelectedIndex);
                    writer.WriteLine("Volume" + "=" + this.m_audio_volume);
                    writer.WriteLine("VerseRepetitions" + "=" + this.m_player_looping_count);
                    writer.WriteLine("VerseRepetitionsEnabled" + "=" + this.m_player_looping);
                    writer.WriteLine("SelectionRepetitions" + "=" + this.m_player_looping_all_count);
                    writer.WriteLine("SelectionRepetitionsEnabled" + "=" + this.m_player_looping_all);
                    writer.WriteLine("SilenceBetweenVerses" + "=" + this.m_silence_between_verses);
                    writer.WriteLine("SilenceBetweenSelections" + "=" + this.m_silence_between_selections);
                    writer.WriteLine();

                    writer.WriteLine("[Downloads]");
                    writer.WriteLine("TranslationUrlPrefix" + "=" + TranslationInfo.UrlPrefix);
                    writer.WriteLine("TranslationFileType" + "=" + TranslationInfo.FileType);
                    writer.WriteLine("TranslationIconUrlPrefix" + "=" + TranslationInfo.IconUrlPrefix);
                    writer.WriteLine("RecitationUrlPrefix" + "=" + RecitationInfo.UrlPrefix);
                    writer.WriteLine("RecitationFileType" + "=" + RecitationInfo.FileType);
                    writer.WriteLine();

                    writer.WriteLine("[Folders]");
                    writer.WriteLine("NumbersFolder=" + Numbers.NUMBERS_FOLDER);
                    writer.WriteLine("FontsFolder=" + Globals.FONTS_FOLDER);
                    writer.WriteLine("ImagesFolder=" + Globals.IMAGES_FOLDER);
                    writer.WriteLine("DataFolder=" + Globals.DATA_FOLDER);
                    writer.WriteLine("AudioFolder=" + Globals.AUDIO_FOLDER);
                    writer.WriteLine("TranslationsFolder=" + Globals.TRANSLATIONS_FOLDER);
                    writer.WriteLine("RulesFolder=" + Globals.RULES_FOLDER);
                    writer.WriteLine("ValuesFolder=" + Globals.VALUES_FOLDER);
                    writer.WriteLine("StatisticsFolder=" + Globals.STATISTICS_FOLDER);
                    writer.WriteLine("BookmarksFolder=" + Globals.BOOKMARKS_FOLDER);
                    writer.WriteLine("HistoryFolder=" + Globals.HISTORY_FOLDER);
                    writer.WriteLine("HelpFolder=" + Globals.HELP_FOLDER);
                }
            }
        }
        catch
        {
            // silence IO errors in case running from read-only media (CD/DVD)
        }
    }
    private void InitializeControls()
    {
        VersionLabel.Text = Globals.SHORT_VERSION;

        // use Right-Click for going to Related Words instead of showing context menu
        RegisterContextMenu(MainTextBox);
        RegisterContextMenu(SearchResultTextBox);
        RegisterContextMenu(TranslationTextBox);
        RegisterContextMenu(RelatedWordsTextBox);
        RegisterContextMenu(UserTextTextBox);
        RegisterContextMenu(FindByTextTextBox);
        RegisterContextMenu(FindByFrequencyPhraseTextBox);
        RegisterContextMenu(ValueTextBox);
        RegisterContextMenu(NthPrimeTextBox);
        RegisterContextMenu(NthAdditivePrimeTextBox);
        RegisterContextMenu(NthPurePrimeTextBox);

        SetupToolTips();
    }
    private void SetupToolTips()
    {
        this.ToolTip.SetToolTip(this.WebsiteLinkLabel, "اللهُمَّ صَلِّ على مُحَمَّدٍ وءالِ مُحَمَّدٍ");
        this.ToolTip.SetToolTip(this.PlayerPreviousLabel, "Previous verse");
        this.ToolTip.SetToolTip(this.PlayerPlayLabel, "Play");
        this.ToolTip.SetToolTip(this.PlayerNextLabel, "Next verse");
        this.ToolTip.SetToolTip(this.PlayerStopLabel, "Stop");
        this.ToolTip.SetToolTip(this.PlayerRepeatLabel, "Repeat verse");
        this.ToolTip.SetToolTip(this.PlayerRepeatAllLabel, "Repeat selection");
        this.ToolTip.SetToolTip(this.PlayerRepeatCounterLabel, "Verse repetitions");
        this.ToolTip.SetToolTip(this.PlayerRepeatAllCounterLabel, "Selection repetitions");
        this.ToolTip.SetToolTip(this.PlayerMuteLabel, "Mute");
        this.ToolTip.SetToolTip(this.PlayerVerseSilenceGapTrackBar, "Silence between verses");
        this.ToolTip.SetToolTip(this.PlayerSelectionSilenceGapTrackBar, "Silence between selections");
        this.ToolTip.SetToolTip(this.TextModeComboBox, "Letter simplification system\r\nنظام تبسيط الحروف");
        this.ToolTip.SetToolTip(this.NumerologySystemComboBox, "Letter valuation system\r\nنظام تقييم الحروف");
        this.ToolTip.SetToolTip(this.ChaptersTextBox, "Chapters in selection\r\nعدد السور");
        this.ToolTip.SetToolTip(this.VersesTextBox, "Verses in selection\r\nعدد الءايات");
        this.ToolTip.SetToolTip(this.WordsTextBox, "Words in selection\r\nعدد الكلمات");
        this.ToolTip.SetToolTip(this.LettersTextBox, "Letters in selection\r\nعدد الحروف");
        this.ToolTip.SetToolTip(this.DecimalChaptersTextBox, "Chapters in selection\r\nعدد السور");
        this.ToolTip.SetToolTip(this.DecimalVersesTextBox, "Verses in selection\r\nعدد الءايات");
        this.ToolTip.SetToolTip(this.DecimalWordsTextBox, "Words in selection\r\nعدد الكلمات");
        this.ToolTip.SetToolTip(this.DecimalLettersTextBox, "Letters in selection\r\nعدد الحروف");
        this.ToolTip.SetToolTip(this.ChapterNumberSumTextBox, "Sum of chapter numbers\r\nمجموع أرقام االسور");
        this.ToolTip.SetToolTip(this.VerseNumberSumTextBox, "Sum of verse numbers in their chapters\r\nمجموع أرقام الءايات في سورها");
        this.ToolTip.SetToolTip(this.WordNumberSumTextBox, "Sum of word numbers in their verses\r\nمجموع أرقام الكلمات في ءاياتها");
        this.ToolTip.SetToolTip(this.LetterNumberSumTextBox, "Sum of letter numbers in their words\r\nمجموع أرقام الحروف في كلماتها");
        this.ToolTip.SetToolTip(this.ValueTextBox, "Value of selection\r\nالقيمة حسب نظام تقييم الحروف الحالي");
        this.ToolTip.SetToolTip(this.PrimeFactorsTextBox, "Prime factors of Value\r\nالعوامل الأولية للقيمة");
        this.ToolTip.SetToolTip(this.SearchScopeLabel, "Entire Book");
        this.ToolTip.SetToolTip(this.FindByTextExactSearchTypeLabel, "search for exact word or expression");
        this.ToolTip.SetToolTip(this.FindByTextProximitySearchTypeLabel, "search for any or all words in any order");
        this.ToolTip.SetToolTip(this.FindByTextRootSearchTypeLabel, "search for all words with given root(s)");
        this.ToolTip.SetToolTip(this.FindBySimilarityCurrentVerseTypeLabel, "find similar verses to the current verse");
        this.ToolTip.SetToolTip(this.FindBySimilarityAllVersesTypeLabel, "find similar verses to all verses in the Quran");
        this.ToolTip.SetToolTip(this.FindBySimilarityPercentageTrackBar, "similarity percentage");
        this.ToolTip.SetToolTip(this.FindBySimilaritySimilarWordsRadioButton, "verses with similar words in any order");
        this.ToolTip.SetToolTip(this.FindBySimilaritySimilarTextRadioButton, "verses with similar letters and order");
        this.ToolTip.SetToolTip(this.FindBySimilaritySimilarFirstHalfRadioButton, "verses with similar words in first half");
        this.ToolTip.SetToolTip(this.FindBySimilaritySimilarLastHalfRadioButton, "verses with similar words in last half");
        this.ToolTip.SetToolTip(this.FindBySimilaritySimilarFirstWordRadioButton, "verses with similar first word");
        this.ToolTip.SetToolTip(this.FindBySimilaritySimilarLastWordRadioButton, "verses with similar last word");
        this.ToolTip.SetToolTip(this.FindByTextAtVerseEndRadioButton, "find at the end of the verse");
        this.ToolTip.SetToolTip(this.FindByTextAtVerseStartRadioButton, "find at the beginning of the verse");
        this.ToolTip.SetToolTip(this.FindByTextAtVerseAnywhereRadioButton, "find anywhere in the verse");
        this.ToolTip.SetToolTip(this.FindByTextAtVerseMiddleRadioButton, "find in the middle of the verse");
        this.ToolTip.SetToolTip(this.FindByTextAtWordEndRadioButton, "find at the end of the word");
        this.ToolTip.SetToolTip(this.FindByTextAtWordStartRadioButton, "find at the beginning of the word");
        this.ToolTip.SetToolTip(this.FindByTextAtWordAnywhereRadioButton, "find anywhere in the word");
        this.ToolTip.SetToolTip(this.FindByTextAtWordMiddleRadioButton, "find in the middle of the word");
        this.ToolTip.SetToolTip(this.FindByTextMultiplicityCheckBox, "find verses with given number of text repetitions");
        this.ToolTip.SetToolTip(this.FindByTextAllWordsRadioButton, "find verses with all words in any order");
        this.ToolTip.SetToolTip(this.FindByTextAnyWordRadioButton, "find verses with at least one word");
        this.ToolTip.SetToolTip(this.DigitSumTextBox, "Digit sum");
        this.ToolTip.SetToolTip(this.DigitalRootTextBox, "Digital root");
        this.ToolTip.SetToolTip(this.NthPrimeTextBox, "Find prime by index");
        this.ToolTip.SetToolTip(this.NthAdditivePrimeTextBox, "Find additive prime by index");
        this.ToolTip.SetToolTip(this.NthPurePrimeTextBox, "Find pure prime by index");
        this.ToolTip.SetToolTip(this.ChapterComboBox, "C, C-C, C:V, C:V-C, C-C:V, C:V-C:V or any combination" + "\r\n" + "36  40-46  15:87  18:9-25  1-2:5  24:35-27:62  2:29,41:9-12");
        this.ToolTip.SetToolTip(this.ChapterVerseNumericUpDown, "ءاية");
        this.ToolTip.SetToolTip(this.ChapterWordNumericUpDown, "كلمة");
        this.ToolTip.SetToolTip(this.ChapterLetterNumericUpDown, "حرف");
        this.ToolTip.SetToolTip(this.PartNumericUpDown, "جزء");
        this.ToolTip.SetToolTip(this.PageNumericUpDown, "صفحة");
        this.ToolTip.SetToolTip(this.StationNumericUpDown, "منزل");
        this.ToolTip.SetToolTip(this.GroupNumericUpDown, "حزب");
        this.ToolTip.SetToolTip(this.QuarterNumericUpDown, "نصف حزب");
        this.ToolTip.SetToolTip(this.QuarterNumericUpDown, "ربع حزب");
        this.ToolTip.SetToolTip(this.BowingNumericUpDown, "ركوع");
        this.ToolTip.SetToolTip(this.VerseNumericUpDown, "ءاية");
        this.ToolTip.SetToolTip(this.WordNumericUpDown, "كلمة");
        this.ToolTip.SetToolTip(this.LetterNumericUpDown, "حرف");
        this.ToolTip.SetToolTip(this.SearchScopeLabel, "Search scope");
        this.ToolTip.SetToolTip(this.FindByTextTextBox, "text to search for in Arabic or any installed language");
        this.ToolTip.SetToolTip(this.FindByTextWordnessCheckBox, "find verses with whole word only");
        this.ToolTip.SetToolTip(this.FindByTextCaseSensitiveCheckBox, "case sensitive for non-Arabic languages");
        this.ToolTip.SetToolTip(this.FindByNumbersResultTypeWordsLabel, "find words within verses");
        this.ToolTip.SetToolTip(this.FindByNumbersResultTypeSentencesLabel, "find sentences across verses");
        this.ToolTip.SetToolTip(this.FindByNumbersResultTypeVersesLabel, "find verses");
        this.ToolTip.SetToolTip(this.FindByNumbersResultTypeChaptersLabel, "find chapters");
        this.ToolTip.SetToolTip(this.FindByFrequencyResultTypeWordsLabel, "find words within verses");
        this.ToolTip.SetToolTip(this.FindByFrequencyResultTypeSentencesLabel, "find sentences across verses");
        this.ToolTip.SetToolTip(this.FindByFrequencyResultTypeVersesLabel, "find verses");
        this.ToolTip.SetToolTip(this.FindByFrequencyResultTypeChaptersLabel, "find chapters");
        this.ToolTip.SetToolTip(this.FindByFrequencySearchTypeDuplicateLettersLabel, "include duplicate phrase letters");
        this.ToolTip.SetToolTip(this.FindByFrequencySearchTypeUniqueLettersLabel, "exclude duplicate phrase letters");
    }
    // speed up by only showing text when needed
    private enum TextDisplayMode { None, QuranOnly, TranslationOnly, Both };
    TextDisplayMode m_text_display_mode = TextDisplayMode.Both;
    private void ClientSplitContainer_Resize(object sender, EventArgs e)
    {
        if (this.ClientSplitContainer.Width == 0)
        {
            m_text_display_mode = TextDisplayMode.None;
        }
        else // find out what it should be from SplitterDistance
        {
            ClientSplitContainer_SplitterMoved(null, null);
        }
    }
    private void ClientSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
    {
        if (this.ClientSplitContainer.Width > 0)
        {
            m_information_box_top = this.ClientSplitContainer.SplitterDistance;
            if (m_information_box_top <= 40)
            {
                m_text_display_mode = TextDisplayMode.TranslationOnly;
            }
            else if (m_information_box_top > (ClientSplitContainer.Height - 40))
            {
                m_text_display_mode = TextDisplayMode.QuranOnly;
            }
            else
            {
                m_text_display_mode = TextDisplayMode.Both;
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 04. ContextMenu
    ///////////////////////////////////////////////////////////////////////////////
    private string m_clipboard_text = null;
    private string RemovePunctuationMarks(string text)
    {
        if (!String.IsNullOrEmpty(text))
        {
            if (m_language_type == LanguageType.LeftToRight)
            {
                text = text.Replace(".", "");
                text = text.Replace(",", "");
                text = text.Replace(";", "");
                text = text.Replace(":", "");
                text = text.Replace("?", "");
                text = text.Replace("/", "");
                text = text.Replace(")", "");
                text = text.Replace("(", "");
                text = text.Replace(">", "");
                text = text.Replace("<", "");
                text = text.Replace("[", "");
                text = text.Replace("]", "");
                text = text.Replace("{", "");
                text = text.Replace("}", "");
                text = text.Replace("-", "");
                text = text.Replace("\"", "");
                text = text.Replace("\'", "");
                text = text.Replace("!", "");
                text = text.Replace("`", "");
                text = text.Replace("@", "");
                text = text.Replace("#", "");
                text = text.Replace("$", "");
                text = text.Replace("%", "");
                text = text.Replace("^", "");
                text = text.Replace("&", "");
                text = text.Replace("|", "");
                text = text.Replace("*", "");
                text = text.Replace("=", "");
            }
        }
        return text;
    }
    private void SimplifyClipboardTextBeforePaste()
    {
        m_clipboard_text = Clipboard.GetText(TextDataFormat.UnicodeText);
        if ((m_clipboard_text != null) && (m_clipboard_text.Length > 0))
        {
            if (m_client != null)
            {
                if (m_client.NumerologySystem != null)
                {
                    string text = m_clipboard_text.SimplifyTo(m_client.NumerologySystem.TextMode);
                    if ((text != null) && (m_clipboard_text.Length > 0))
                    {
                        Clipboard.SetText(text, TextDataFormat.UnicodeText);
                    }
                }
            }
        }
    }
    private void RestoreClipboardTextAfterPaste()
    {
        if ((m_clipboard_text != null) && (m_clipboard_text.Length > 0))
        {
            Clipboard.SetText(m_clipboard_text, TextDataFormat.UnicodeText);
        }
    }
    private void MenuItem_Undo(object sender, EventArgs e)
    {
        if (sender is MenuItem)
        {
            if ((sender as MenuItem).Parent is ContextMenu)
            {
                if (((sender as MenuItem).Parent as ContextMenu).SourceControl is TextBoxBase)
                {
                    (((sender as MenuItem).Parent as ContextMenu).SourceControl as TextBoxBase).Undo();
                }
            }
        }
    }
    private void MenuItem_Cut(object sender, EventArgs e)
    {
        if (sender is MenuItem)
        {
            if ((sender as MenuItem).Parent is ContextMenu)
            {
                if (((sender as MenuItem).Parent as ContextMenu).SourceControl is TextBoxBase)
                {
                    (((sender as MenuItem).Parent as ContextMenu).SourceControl as TextBoxBase).Cut();
                }
            }
        }
    }
    private void MenuItem_Copy(object sender, EventArgs e)
    {
        if (sender is MenuItem)
        {
            if ((sender as MenuItem).Parent is ContextMenu)
            {
                if (((sender as MenuItem).Parent as ContextMenu).SourceControl is TextBoxBase)
                {
                    TextBoxBase control = (((sender as MenuItem).Parent as ContextMenu).SourceControl as TextBoxBase);
                    bool nothing_selected = false;
                    if (control.SelectionLength == 0)
                    {
                        nothing_selected = true;
                        control.SelectAll();
                    }
                    (((sender as MenuItem).Parent as ContextMenu).SourceControl as TextBoxBase).Copy();
                    if (nothing_selected)
                    {
                        control.DeselectAll();
                    }
                }
            }
        }
    }
    private void MenuItem_Paste(object sender, EventArgs e)
    {
        if (sender is MenuItem)
        {
            if ((sender as MenuItem).Parent is ContextMenu)
            {
                if (((sender as MenuItem).Parent as ContextMenu).SourceControl is TextBoxBase)
                {
                    SimplifyClipboardTextBeforePaste();
                    Thread.Sleep(100); // must give chance for Clipboard to refresh its content before Paste
                    (((sender as MenuItem).Parent as ContextMenu).SourceControl as TextBoxBase).Paste();
                    RestoreClipboardTextAfterPaste();
                }
            }
        }
    }
    private void MenuItem_SelectAll(object sender, EventArgs e)
    {
        if (sender is MenuItem)
        {
            if ((sender as MenuItem).Parent is ContextMenu)
            {
                if (((sender as MenuItem).Parent as ContextMenu).SourceControl is TextBoxBase)
                {
                    (((sender as MenuItem).Parent as ContextMenu).SourceControl as TextBoxBase).SelectAll();
                    (((sender as MenuItem).Parent as ContextMenu).SourceControl as TextBoxBase).KeyDown += new KeyEventHandler(TextBox_KeyDown);
                }
            }
        }
    }
    private void MenuItem_ExactText(object sender, EventArgs e)
    {
        if (sender is MenuItem)
        {
            if ((sender as MenuItem).Parent is ContextMenu)
            {
                Control control = ((sender as MenuItem).Parent as ContextMenu).SourceControl;
                if ((control == MainTextBox) || (control == SearchResultTextBox))
                {
                    this.Cursor = Cursors.WaitCursor;
                    try
                    {
                        DoFindExactText(control);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }
    }
    private void MenuItem_ExactWords(object sender, EventArgs e)
    {
        if (sender is MenuItem)
        {
            if ((sender as MenuItem).Parent is ContextMenu)
            {
                Control control = ((sender as MenuItem).Parent as ContextMenu).SourceControl;
                if ((control == MainTextBox) || (control == SearchResultTextBox))
                {
                    this.Cursor = Cursors.WaitCursor;
                    try
                    {
                        DoFindExactWords(control);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }
    }
    private void MenuItem_SimilarWords(object sender, EventArgs e)
    {
        if (sender is MenuItem)
        {
            if ((sender as MenuItem).Parent is ContextMenu)
            {
                Control control = ((sender as MenuItem).Parent as ContextMenu).SourceControl;
                if ((control == MainTextBox) || (control == SearchResultTextBox))
                {
                    this.Cursor = Cursors.WaitCursor;
                    try
                    {
                        DoFindSimilarWords(control);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }
    }
    private void MenuItem_SimilarVerses(object sender, EventArgs e)
    {
        if (sender is MenuItem)
        {
            if ((sender as MenuItem).Parent is ContextMenu)
            {
                Control control = ((sender as MenuItem).Parent as ContextMenu).SourceControl;
                if ((control == MainTextBox) || (control == SearchResultTextBox))
                {
                    this.Cursor = Cursors.WaitCursor;
                    try
                    {
                        DoFindSimilarVerses(control);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }
    }
    private void MenuItem_RelatedWords(object sender, EventArgs e)
    {
        if (sender is MenuItem)
        {
            if ((sender as MenuItem).Parent is ContextMenu)
            {
                Control control = ((sender as MenuItem).Parent as ContextMenu).SourceControl;
                if ((control == MainTextBox) || (control == SearchResultTextBox))
                {
                    this.Cursor = Cursors.WaitCursor;
                    try
                    {
                        DoFindRelatedWords(control);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }
    }
    private void MenuItem_SameValue(object sender, EventArgs e)
    {
        if (sender is MenuItem)
        {
            if ((sender as MenuItem).Parent is ContextMenu)
            {
                Control control = ((sender as MenuItem).Parent as ContextMenu).SourceControl;
                if ((control == MainTextBox) || (control == SearchResultTextBox))
                {
                    this.Cursor = Cursors.WaitCursor;
                    try
                    {
                        DoFindSameValue(control);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }
    }
    private void DoFindExactText(object sender)
    {
        if (sender is TextBoxBase)
        {
            string text = (sender as TextBoxBase).SelectedText.Trim();
            if (text.Length == 0) // no selection, get word under mouse pointer
            {
                m_clicked_word = GetWordAtCursor();
                if (m_clicked_word == null)
                {
                    return;
                }
                text = m_clicked_word.Text;
            }

            if (m_with_diacritics)
            {
                DoFindExactTextHarakat(text);
            }
            else
            {
                DoFindExactText(text);
            }
        }
    }
    private void DoFindExactText(string text)
    {
        if (m_client != null)
        {
            ClearFindMatches();

            m_client.FindPhrases(text, LanguageType.RightToLeft, null, TextLocationInVerse.Anywhere, TextLocationInWord.Anywhere, false, TextWordness.Any, -1, false);
            if (m_client.FoundPhrases != null)
            {
                int phrase_count = GetPhraseCount(m_client.FoundPhrases);
                if (m_client.FoundVerses != null)
                {
                    int verse_count = m_client.FoundVerses.Count;
                    m_find_result_header = phrase_count + " matches in " + verse_count + ((verse_count == 1) ? " verse" : " verses") + " with " + text + " anywhere " + " in " + m_client.SearchScope.ToString();
                    DisplayFoundVerses(true, true);
                }
            }
        }
    }
    private void DoFindExactTextHarakat(string text)
    {
        if (m_client != null)
        {
            ClearFindMatches();

            m_client.FindPhrases(text, LanguageType.RightToLeft, null, TextLocationInVerse.Anywhere, TextLocationInWord.Anywhere, false, TextWordness.Any, -1, true);
            if (m_client.FoundPhrases != null)
            {
                int phrase_count = GetPhraseCount(m_client.FoundPhrases);
                if (m_client.FoundVerses != null)
                {
                    int verse_count = m_client.FoundVerses.Count;
                    m_find_result_header = phrase_count + " matches in " + verse_count + ((verse_count == 1) ? " verse" : " verses") + " with " + text + " anywhere " + " in " + m_client.SearchScope.ToString();
                    DisplayFoundVerses(true, true);
                }
            }
        }
    }
    private void DoFindExactWords(object sender)
    {
        if (sender is TextBoxBase)
        {
            string text = (sender as TextBoxBase).SelectedText.Trim();
            if (text.Length == 0) // no selection, get word under mouse pointer
            {
                m_clicked_word = GetWordAtCursor();
                if (m_clicked_word == null)
                {
                    return;
                }
                text = m_clicked_word.Text;
            }

            DoFindExactWords(text);
        }
    }
    private void DoFindExactWords(string text)
    {
        if (m_client != null)
        {
            ClearFindMatches();

            m_client.FindPhrases(text, LanguageType.RightToLeft, null, ProximitySearchType.AllWords, false, TextWordness.WholeWord, false);
            if (m_client.FoundPhrases != null)
            {
                int phrase_count = GetPhraseCount(m_client.FoundPhrases);
                if (m_client.FoundVerses != null)
                {
                    int verse_count = m_client.FoundVerses.Count;
                    m_find_result_header = phrase_count + " matches in " + verse_count + ((verse_count == 1) ? " verse" : " verses") + " with " + text + " anywhere " + " in " + m_client.SearchScope.ToString();
                    DisplayFoundVerses(true, true);
                }
            }
        }
    }
    private void DoFindSimilarWords(object sender)
    {
        if (sender is TextBoxBase)
        {
            string text = (sender as TextBoxBase).SelectedText.Trim();
            if (text.Length == 0) // no selection, get word under mouse pointer
            {
                m_clicked_word = GetWordAtCursor();
                if (m_clicked_word == null)
                {
                    return;
                }
                text = m_clicked_word.Text;
            }

            DoFindSimilarWords(text);
        }
    }
    private void DoFindSimilarWords(string text)
    {
        if (m_client != null)
        {
            ClearFindMatches();

            double similarity_percentage = 0.66D;
            SimilarityMethod find_by_similarity_method = SimilarityMethod.SimilarWords;

            m_client.FindPhrases(text, similarity_percentage);
            if (m_client.FoundPhrases != null)
            {
                string similarity_search_source = " to " + text + " ";
                int phrase_count = GetPhraseCount(m_client.FoundPhrases);
                if (m_client.FoundVerses != null)
                {
                    m_find_result_header = m_client.FoundVerses.Count + ((m_client.FoundVerses.Count == 1) ? " verse" : " verses") + " with " + find_by_similarity_method.ToString() + similarity_search_source + " in " + m_client.SearchScope.ToString();
                    DisplayFoundVerses(true, true);
                }
            }
        }
    }
    private void DoFindSimilarVerses(object sender)
    {
        if (sender is TextBoxBase)
        {
            Verse verse = GetVerseAtCursor();
            if (verse != null)
            {
                DoFindSimilarVerses(verse);
            }
        }
    }
    private void DoFindSimilarVerses(Verse verse)
    {
        if (m_client != null)
        {
            ClearFindMatches();

            double similarity_percentage = 0.66D;
            SimilarityMethod find_by_similarity_method = SimilarityMethod.SimilarWords;

            m_client.FindVerses(verse, SimilarityMethod.SimilarText, similarity_percentage);
            if (m_client.FoundVerses != null)
            {
                m_find_result_header = m_client.FoundVerses.Count + ((m_client.FoundVerses.Count == 1) ? " verse" : " verses") + " with " + find_by_similarity_method.ToString() + " in " + m_client.SearchScope.ToString();
                DisplayFoundVerses(true, true);
            }
        }
    }
    private void DoFindRelatedWords(object sender)
    {
        if (sender is TextBoxBase)
        {
            ClearFindMatches();

            string text = (sender as TextBoxBase).SelectedText.Trim();
            if (text.Length == 0)
            {
                m_clicked_word = GetWordAtCursor();
                if (m_clicked_word == null)
                {
                    return;
                }
                text = m_clicked_word.Text;
            }
            text = RemovePunctuationMarks(text);

            FindByTextTextBox.Text = text;
            FindByTextTextBox.Refresh();

            int multiplicity = -1;
            FindByRoot(text, multiplicity, m_with_diacritics);
        }
    }
    private void DoFindSameValue(object sender)
    {
        if (sender is TextBoxBase)
        {
            if (m_client != null)
            {
                long value = 0L;
                if ((sender as TextBoxBase).SelectionLength > 0)
                {
                    try
                    {
                        value = long.Parse(ValueTextBox.Text);
                    }
                    catch
                    {
                        // leave value = 0L
                    }
                }
                else
                {
                    Verse verse = GetVerseAtCursor();
                    if (verse != null)
                    {
                        value = m_client.CalculateValue(verse);
                    }
                    else
                    {
                        // leave value = 0L
                    }
                }
                DoFindSameValue(value);
            }
        }
    }
    private void DoFindSameValue(long value)
    {
        if (m_client != null)
        {
            ClearFindMatches();

            int match_count = 0;
            List<Verse> found_verses = new List<Verse>();
            List<Phrase> found_phrases = new List<Phrase>();

            string text = "value" + "" + "=" + value.ToString();

            NumberQuery query = new NumberQuery();
            query.Value = value;

            int w_match_count = m_client.FindWords(query);
            if (w_match_count > 0)
            {
                match_count += w_match_count;
                if (m_client.FoundVerses != null)
                {
                    found_verses.InsertRange(0, new List<Verse>(m_client.FoundVerses));
                }
                if (m_client.FoundPhrases != null)
                {
                    found_phrases.InsertRange(0, new List<Phrase>(m_client.FoundPhrases));
                }
            }

            int s_match_count = m_client.FindSentences(query);
            if (s_match_count > 0)
            {
                match_count += s_match_count;
                if (m_client.FoundVerses != null)
                {
                    found_verses.InsertRange(0, new List<Verse>(m_client.FoundVerses));
                }
                if (m_client.FoundPhrases != null)
                {
                    found_phrases.InsertRange(0, new List<Phrase>(m_client.FoundPhrases));
                }
            }

            m_client.FoundVerses = found_verses;
            m_client.FoundPhrases = found_phrases;
            if (m_client.FoundVerses != null)
            {
                m_find_result_header = match_count + ((match_count == 1) ? " match" : " matches") + " in " + m_client.FoundVerses.Count + ((m_client.FoundVerses.Count == 1) ? " verse" : " verses") + " with " + text + " in " + m_client.SearchScope.ToString();
                DisplayFoundVerses(true, true);
            }
        }
    }
    private void RegisterContextMenu(TextBoxBase control)
    {
        ContextMenu ContextMenu = new ContextMenu();
        if ((control != MainTextBox) && (control != SearchResultTextBox))
        {
            MenuItem EditUndoMenuItem = new MenuItem("Undo\t\tCtrl+Z");
            EditUndoMenuItem.Click += new EventHandler(MenuItem_Undo);
            ContextMenu.MenuItems.Add(EditUndoMenuItem);

            MenuItem MenuItemSeparator1 = new MenuItem("-");
            ContextMenu.MenuItems.Add(MenuItemSeparator1);

            MenuItem EditCutMenuItem = new MenuItem("Cut\t\tCtrl+X");
            EditCutMenuItem.Click += new EventHandler(MenuItem_Cut);
            ContextMenu.MenuItems.Add(EditCutMenuItem);

            MenuItem EditCopyMenuItem = new MenuItem("Copy\t\tCtrl+C");
            EditCopyMenuItem.Click += new EventHandler(MenuItem_Copy);
            ContextMenu.MenuItems.Add(EditCopyMenuItem);

            MenuItem EditPasteMenuItem = new MenuItem("Paste\t\tCtrl+V");
            EditPasteMenuItem.Click += new EventHandler(MenuItem_Paste);
            ContextMenu.MenuItems.Add(EditPasteMenuItem);

            MenuItem MenuItemSeparator2 = new MenuItem("-");
            ContextMenu.MenuItems.Add(MenuItemSeparator2);

            MenuItem EditSelectAllMenuItem = new MenuItem("Select All\tCtrl+A");
            EditSelectAllMenuItem.Click += new EventHandler(MenuItem_SelectAll);
            ContextMenu.MenuItems.Add(EditSelectAllMenuItem);
        }
        else
        {
            MenuItem EditCopyAllMenuItem = new MenuItem("Copy All\t\tCtrl+C");
            EditCopyAllMenuItem.Click += new EventHandler(MenuItem_Copy);
            ContextMenu.MenuItems.Add(EditCopyAllMenuItem);

            MenuItem MenuItemSeparator1 = new MenuItem("-");
            ContextMenu.MenuItems.Add(MenuItemSeparator1);

            MenuItem FindExactTextMenuItem = new MenuItem("Exact Text\tF4");
            FindExactTextMenuItem.Click += new EventHandler(MenuItem_ExactText);
            ContextMenu.MenuItems.Add(FindExactTextMenuItem);

            MenuItem FindExactWordsMenuItem = new MenuItem("Exact Words\tF5");
            FindExactWordsMenuItem.Click += new EventHandler(MenuItem_ExactWords);
            ContextMenu.MenuItems.Add(FindExactWordsMenuItem);

            MenuItem MenuItemSeparator2 = new MenuItem("-");
            ContextMenu.MenuItems.Add(MenuItemSeparator2);

            MenuItem FindSimilarWordsMenuItem = new MenuItem("Similar Words\tF6");
            FindSimilarWordsMenuItem.Click += new EventHandler(MenuItem_SimilarWords);
            ContextMenu.MenuItems.Add(FindSimilarWordsMenuItem);

            MenuItem FindSimilarVersesMenuItem = new MenuItem("Similar Verses\tF7");
            FindSimilarVersesMenuItem.Click += new EventHandler(MenuItem_SimilarVerses);
            ContextMenu.MenuItems.Add(FindSimilarVersesMenuItem);

            MenuItem MenuItemSeparator3 = new MenuItem("-");
            ContextMenu.MenuItems.Add(MenuItemSeparator3);

            MenuItem FindRelatedWordsMenuItem = new MenuItem("Related Words\tF8");
            FindRelatedWordsMenuItem.Click += new EventHandler(MenuItem_RelatedWords);
            ContextMenu.MenuItems.Add(FindRelatedWordsMenuItem);

            MenuItem MenuItemSeparator4 = new MenuItem("-");
            ContextMenu.MenuItems.Add(MenuItemSeparator4);

            MenuItem FindSameValueMenuItem = new MenuItem("Same Value\tF9");
            FindSameValueMenuItem.Click += new EventHandler(MenuItem_SameValue);
            ContextMenu.MenuItems.Add(FindSameValueMenuItem);
        }

        ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
        ContextMenu.Collapse += new EventHandler(ContextMenu_Collapse);

        control.ContextMenu = ContextMenu;
    }
    private void ContextMenu_Popup(object sender, EventArgs e)
    {
        if (m_active_textbox != null)
        {
            if (m_active_textbox.SelectionLength == 0)
            {
                m_active_textbox.ContextMenu.MenuItems[0].Text = "Copy All\t\tCtrl+C";
            }
            else
            {
                m_active_textbox.ContextMenu.MenuItems[0].Text = "Copy\t\tCtrl+C";
            }
        }
        //this.Cursor = Cursors.Arrow;
        //this.Refresh();
    }
    private void ContextMenu_Collapse(object sender, EventArgs e)
    {
        //this.Cursor = Cursors.IBeam;
        //this.Refresh();
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 06. MainTextBox
    ///////////////////////////////////////////////////////////////////////////////
    private float m_text_zoom_factor = DEFAULT_TEXT_ZOOM_FACTOR;
    private Point m_previous_location = new Point(0, 0);
    private float m_min_zoom_factor = 0.1F;
    private float m_max_zoom_factor = 2.0F;
    private float m_zoom_factor_increment = 0.1F;
    private float m_error_margin = 0.001F;

    private float m_font_size = DEFAULT_FONT_SIZE;
    private void SetupFont()
    {
        if (Directory.Exists(Globals.FONTS_FOLDER))
        {
            DirectoryInfo folder = new DirectoryInfo(Globals.FONTS_FOLDER);
            if (folder != null)
            {
                FileInfo[] files = folder.GetFiles("*.ttf");
                if ((files != null) && (files.Length > 0))
                {
                    foreach (FileInfo file in files)
                    {
                        try
                        {
                            if (!String.IsNullOrEmpty(file.FullName))
                            {
                                Font font = FontBuilder.Build(file.FullName, m_font_size * ((file.Name.Contains("Mushaf")) ? 1.33F : 1));
                                MainTextBox.Font = font;
                                SearchResultTextBox.Font = font;
                            }
                        }
                        catch
                        {
                            // skip non-conformant font
                        }
                    }
                }
            }
        }
    }
    private void ScaleInformationBoxFont(float zoom_factor)
    {
        Font font = new Font(TranslationTextBox.Font.Name, DEFAULT_TRANSALTION_FONT_SIZE * zoom_factor, FontStyle.Bold);
        if (font != null)
        {
            TranslationTextBox.Font = font;
            TranslationTextBox.Refresh();

            RelatedWordsTextBox.Font = font;
            RelatedWordsTextBox.Refresh();

            UserTextTextBox.Font = font;
            UserTextTextBox.Refresh();
        }
    }
    private void MainTextBox_TextChanged(object sender, EventArgs e)
    {
        //ApplyFont(); // don't do as it disables Undo/Redo
    }
    private void MainTextBox_SelectionChanged(object sender, EventArgs e)
    {
        if (
             ((sender != null) && (sender == m_active_textbox)) &&
             (
               (m_active_textbox.Focused) ||
               (ChapterWordNumericUpDown.Focused) ||
               (ChapterLetterNumericUpDown.Focused) ||
               (WordNumericUpDown.Focused) ||
               (LetterNumericUpDown.Focused)
             )
           )
        {
            if (m_client != null)
            {
                m_selection_mode = false;

                Verse previous_verse = GetVerse(CurrentVerseIndex);
                Verse verse = GetVerseAtCursor();
                if (verse != null)
                {
                    if (verse != previous_verse)
                    {
                        CurrentVerseIndex = GetVerseIndex(verse);
                        UpdatePlayerButtons(verse);
                        UpdateHeaderLabel();
                    }

                    CalculateCurrentValue();

                    BuildLetterFrequencies();
                    DisplayLetterFrequencies();

                    DisplayCurrentPositions();
                }
            }
        }
    }
    private void MainTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        try
        {
            if ((e.Control) && (e.KeyCode == Keys.V))
            {
                if ((e.Control) && (e.KeyCode == Keys.V))
                {
                    if (m_active_textbox != null)
                    {
                        SimplifyClipboardTextBeforePaste();
                        Thread.Sleep(100); // must give chance for Clipboard to refresh its content before Paste
                        m_active_textbox.Paste();
                        RestoreClipboardTextAfterPaste();
                        e.Handled = true;
                    }
                }
            }
        }
        finally
        {
            UpdateMouseCursor();
        }
    }
    private void MainTextBox_KeyUp(object sender, KeyEventArgs e)
    {
        try
        {
            bool NavigationKeys = (
            e.KeyCode == Keys.Up ||
            e.KeyCode == Keys.Right ||
            e.KeyCode == Keys.Down ||
            e.KeyCode == Keys.Left ||
            e.KeyCode == Keys.Home ||
            e.KeyCode == Keys.End);

            if (NavigationKeys)
            {
                // this code has been moved out of SelectionChanged and brought to MouseClick and KeyUp
                // to keep all verse translations visible until the user clicks a verse then show one verse translation
                if (m_active_textbox != null)
                {
                    if (m_active_textbox.SelectionLength == 0)
                    {
                        Verse verse = GetVerse(CurrentVerseIndex);
                        if (verse != null)
                        {
                            DisplayTranslations(verse);
                        }
                    }
                    else
                    {
                        // selected text is dealt with by CalculateAndDisplayCounts 
                    }
                }

                // in all cases
                m_clicked_word = GetWordAtCursor();
                if (m_clicked_word != null)
                {
                    DisplayRelatedWords(m_clicked_word);
                }
            }
        }
        finally
        {
            UpdateMouseCursor();
        }
    }
    private void MainTextBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == ' ')
        {
            if ((MainTextBox.Focused) && (!m_readonly_translation)) return;
            if ((SearchResultTextBox.Focused) && (!m_readonly_translation)) return;
            if (FindByTextTextBox.Focused) return;
            if (ChapterComboBox.Focused) return;
            if (BookmarkTextBox.Focused) return;
            if (FindByFrequencyPhraseTextBox.Focused) return;

            if (m_player != null)
            {
                if ((m_player.Playing) || (m_player.Paused))
                {
                    PlayerPlayLabel_Click(null, null);
                }
            }
        }

        e.Handled = true; // stop annoying beep
    }
    private void MainTextBox_Enter(object sender, EventArgs e)
    {
        SearchGroupBox_Leave(null, null);
        this.AcceptButton = null;
        UpdateMouseCursor();
    }
    private void MainTextBox_MouseEnter(object sender, EventArgs e)
    {
        //if (m_active_textbox != null)
        //{
        //    m_active_textbox.Focus();
        //}
    }
    private void MainTextBox_MouseLeave(object sender, EventArgs e)
    {
        // stop cursor flicker
        if (m_active_textbox != null)
        {
            if (m_active_textbox.Cursor != Cursors.Default)
            {
                m_active_textbox.Cursor = Cursors.Default;
            }
        }
    }
    private void MainTextBox_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            // in case we come from UserTextTextBox
            if (m_active_textbox != null)
            {
                m_active_textbox.Focus();
                MainTextBox_SelectionChanged(m_active_textbox, null);

                // set cursor at mouse RIGHT-click location so we know which word to get related words for
                int start = m_active_textbox.GetCharIndexFromPosition(e.Location);
                if (
                     (start <= m_active_textbox.SelectionStart)
                     ||
                     (start > (m_active_textbox.SelectionStart + m_active_textbox.SelectionLength))
                   )
                {
                    m_active_textbox.Select(start, 0);
                }
            }
        }
    }
    private void MainTextBox_MouseMove(object sender, MouseEventArgs e)
    {
        // stop flickering
        if (
            (Math.Abs(m_previous_location.X - e.X) < 4)
            &&
            (Math.Abs(m_previous_location.Y - e.Y) < 4)
           )
        {
            return;
        }
        m_previous_location = e.Location;

        Word word = GetWordAtPointer(e);
        if (word != null)
        {
            //// always diplay word info at application caption
            //this.Text = Application.ProductName + " | " + GetSummarizedSearchScope();
            //UpdateFindMatchCaption();
            //this.Text += SPACE_GAP +
            //(
            //    word.Verse.Chapter.Name + SPACE_GAP +
            //    "verse " + word.Verse.NumberInChapter + "-" + word.Verse.Number + SPACE_GAP +
            //    "word " + word.NumberInVerse + "-" + word.NumberInChapter + "-" + word.Number + SPACE_GAP +
            //    word.Transliteration + SPACE_GAP +
            //    word.Text + SPACE_GAP +
            //    word.Meaning + SPACE_GAP +
            //    word.Occurrence.ToString() + "/" + word.Occurrences.ToString()
            //);

            // update m_active_word
            m_clicked_word = GetWordAtPointer(e);
            if (m_clicked_word != null)
            {
                if (ModifierKeys == Keys.Control)
                {
                    string word_info = GetWordInfo(m_clicked_word) + "\r\n\r\n";
                    word_info += GetWordRelatedWords(m_clicked_word);
                    ToolTip.SetToolTip(m_active_textbox, word_info);
                }
                else
                {
                    string word_info = GetWordSummary(m_clicked_word);
                    ToolTip.SetToolTip(m_active_textbox, word_info);
                }
            }
        }
    }
    private void MainTextBox_MouseUp(object sender, MouseEventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            // go to related words to word under mouse pointer
            this.Cursor = Cursors.WaitCursor;
            try
            {
                DoFindRelatedWords(sender);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        else
        {
            m_clicked_word = GetWordAtPointer(e);
            if (m_clicked_word != null)
            {
                DisplayRelatedWords(m_clicked_word);
            }

            Verse verse = GetVerse(CurrentVerseIndex);
            if (verse != null)
            {
                m_player_looping_i = 0;

                //DisplayTranslations(verse); // CalculateAndDisplayCounts will display translations of selected verses
            }
        }
    }
    private Word m_clicked_word = null;
    private Word m_info_word = null;
    private string GetWordInfo(Word word)
    {
        if (word != null)
        {
            return
                word.Transliteration + SPACE_GAP +
                word.Text + SPACE_GAP +
                word.Meaning + "\r\n" +
                word.Verse.Chapter.Number + ". " + word.Verse.Chapter.Name + SPACE_GAP +
                "verse  " + word.Verse.NumberInChapter + "-" + word.Verse.Number + SPACE_GAP +
                "word  " + word.NumberInVerse + "-" + word.NumberInChapter + "-" + word.Number;
        }
        return null;
    }
    private string GetWordSummary(Word word)
    {
        if (word != null)
        {
            return
                word.Transliteration + SPACE_GAP +
                word.Text + SPACE_GAP +
                word.Meaning + "\r\n" +
                word.Verse.Chapter.Number + ". " + word.Verse.Chapter.Name + SPACE_GAP +
                "verse  " + word.Verse.NumberInChapter + "-" + word.Verse.Number + SPACE_GAP +
                "word  " + word.NumberInVerse + "-" + word.NumberInChapter + "-" + word.Number;
        }
        return null;
    }
    private string GetWordRelatedWords(Word word)
    {
        if (word != null)
        {
            if (m_client != null)
            {
                string related_words_lines = null;
                int words_per_line = 0;
                int max_words_per_line = 10;
                List<Word> related_words = m_client.Book.GetRelatedWords(word, m_with_diacritics);
                related_words = related_words.RemoveDuplicates();
                if (related_words != null)
                {
                    StringBuilder str = new StringBuilder();
                    if (related_words.Count > 0)
                    {
                        str.AppendLine("Related words = " + related_words.Count.ToString());
                        foreach (Word related_word in related_words)
                        {
                            words_per_line++;
                            str.Append(related_word.Text + (((words_per_line % max_words_per_line) == 0) ? "\r\n" : "\t"));
                        }
                        if (str.Length > 1)
                        {
                            str.Remove(str.Length - 1, 1); // \t
                        }
                        str.AppendLine();
                        str.AppendLine();
                        str.AppendLine("Ctrl+Click a word to display its related verses");
                        related_words_lines = str.ToString();
                    }
                }
                return related_words_lines;
            }
        }
        return null;
    }
    private void DisplayRelatedWords(Word word)
    {
        if (
             (m_text_display_mode == TextDisplayMode.Both) ||
             (m_text_display_mode == TextDisplayMode.TranslationOnly)
           )
        {
            if (word != null)
            {
                if (word != null)
                {
                    if (TabControl.SelectedTab == RelatedWordsTabPage)
                    {
                        RelatedWordsTextBox.Text = GetWordInfo(m_clicked_word) + "\r\n\r\n" + GetWordRelatedWords(m_clicked_word);
                        RelatedWordsTextBox.Refresh();

                        m_info_word = word;
                    }
                }
            }
        }
    }
    private void RelatedWordsTextBox_TextChanged(object sender, EventArgs e)
    {
        RelatedWordsButton.Enabled = (RelatedWordsTextBox.Text.Length > 0);
    }
    private void RelatedWordsButton_Click(object sender, EventArgs e)
    {
        if (m_info_word != null)
        {
            FindRelatedWords(m_info_word);
        }
    }
    private void FindRelatedWords(Word word)
    {
        if (word != null)
        {
            ClearFindMatches();

            if (m_clicked_word != null)
            {
                string text = word.Text;
                text = RemovePunctuationMarks(text);

                FindByTextTextBox.Text = text;
                FindByTextTextBox.Refresh();

                int multiplicity = -1;
                FindByRoot(text, multiplicity, m_with_diacritics);
            }
        }
    }
    private void UpdateMouseCursor()
    {
        if (m_active_textbox != null)
        {
            if (ModifierKeys == Keys.Control)
            {
                // stop cursor flicker
                if (m_active_textbox.Cursor != Cursors.Hand)
                {
                    m_active_textbox.Cursor = Cursors.Hand;
                }
            }
            else
            {
                // stop cursor flicker
                if (m_active_textbox.Cursor != Cursors.IBeam)
                {
                    m_active_textbox.Cursor = Cursors.IBeam;
                }
            }
        }
    }
    private void MainTextBox_Click(object sender, EventArgs e)
    {
        // this code has been moved out of SelectionChanged and brought to MouseClick and KeyUp
        // to keep all verse translations visible until the user clicks a verse then show one verse translation
        if (m_active_textbox != null)
        {
            if (m_active_textbox.SelectionLength == 0)
            {
                Verse verse = GetVerse(CurrentVerseIndex);
                if (verse != null)
                {
                    DisplayTranslations(verse);
                }
            }
            else
            {
                // selected text is dealt with by CalculateAndDisplayCounts 
            }
        }
    }
    private void MainTextBox_DoubleClick(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            //MainTextBox.TextChanged -= new EventHandler(MainTextBox_TextChanged);
            MainTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
            MainTextBox.BeginUpdate();

            if (ModifierKeys == Keys.None)
            {
                if (m_found_verses_displayed)
                {
                    Verse verse = GetVerse(CurrentVerseIndex);
                    if (verse != null)
                    {
                        if (verse.Chapter != null)
                        {
                            if (m_client != null)
                            {
                                // select chapter and display it and colorize target verse
                                m_client.Selection = new Selection(m_client.Book, SelectionScope.Chapter, new List<int>() { verse.Chapter.Number - 1 });
                                if (m_client.Selection != null)
                                {
                                    SwitchToMainTextBox();

                                    BookmarkTextBox.Enabled = true;
                                    // display selection's note (if any)
                                    DisplayNote(m_client.GetBookmark(m_client.Selection));

                                    m_selection_mode = false;

                                    AutoCompleteHeaderLabel.Visible = false;
                                    AutoCompleteListBox.Visible = false;
                                    AutoCompleteListBox.SendToBack();

                                    this.Text = Application.ProductName + " | " + GetSummarizedSearchScope();
                                    UpdateSearchScope();

                                    DisplaySelectionText();

                                    m_current_selection_verse_index = 0;

                                    MainTextBox.ClearHighlight();
                                    MainTextBox.AlignToStart();
                                    HighlightVerse(verse);
                                    UpdateHeaderLabel();

                                    CalculateCurrentValue();

                                    UpdateVersePositions(verse);

                                    BuildLetterFrequencies();
                                    DisplayLetterFrequencies();

                                    DisplayTranslations(verse);

                                    //if (add_to_history)
                                    {
                                        AddSearchHistoryItem();
                                    }

                                    // change focuse to newly displayed control
                                    MainTextBox.Focus();
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            MainTextBox.EndUpdate();
            MainTextBox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
            //MainTextBox.TextChanged += new EventHandler(MainTextBox_TextChanged);
            this.Cursor = Cursors.Default;
        }
    }
    private void MainTextBox_MouseWheel(object sender, MouseEventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            if (m_active_textbox != null)
            {
                ZoomOutLabel.Enabled = true;
                ZoomInLabel.Enabled = true;

                m_text_zoom_factor = m_active_textbox.ZoomFactor;
                if (m_active_textbox.ZoomFactor <= (m_min_zoom_factor + m_error_margin))
                {
                    MainTextBox.ZoomFactor = m_min_zoom_factor;
                    SearchResultTextBox.ZoomFactor = m_min_zoom_factor;
                    ZoomOutLabel.Enabled = false;
                    ZoomInLabel.Enabled = true;
                }
                else if (m_active_textbox.ZoomFactor >= (m_max_zoom_factor - m_error_margin))
                {
                    MainTextBox.ZoomFactor = m_max_zoom_factor;
                    SearchResultTextBox.ZoomFactor = m_max_zoom_factor;
                    ZoomOutLabel.Enabled = true;
                    ZoomInLabel.Enabled = false;
                }

                MainTextBox.ZoomFactor = m_text_zoom_factor;
                SearchResultTextBox.ZoomFactor = m_text_zoom_factor;
                ScaleInformationBoxFont(m_text_zoom_factor);
            }
        }
    }
    private void ZoomInLabel_Click(object sender, EventArgs e)
    {
        if (m_text_zoom_factor <= (m_max_zoom_factor - m_zoom_factor_increment + m_error_margin))
        {
            m_text_zoom_factor += m_zoom_factor_increment;

            MainTextBox.ZoomFactor = m_text_zoom_factor;
            SearchResultTextBox.ZoomFactor = m_text_zoom_factor;
            ScaleInformationBoxFont(m_text_zoom_factor);
        }
        // re-check same condition after zoom_factor update
        ZoomInLabel.Enabled = (m_text_zoom_factor <= (m_max_zoom_factor - m_zoom_factor_increment + m_error_margin));
        ZoomOutLabel.Enabled = true;
    }
    private void ZoomOutLabel_Click(object sender, EventArgs e)
    {
        if (m_text_zoom_factor >= (m_min_zoom_factor + m_zoom_factor_increment - m_error_margin))
        {
            m_text_zoom_factor -= m_zoom_factor_increment;

            MainTextBox.ZoomFactor = m_text_zoom_factor;
            SearchResultTextBox.ZoomFactor = m_text_zoom_factor;
            ScaleInformationBoxFont(m_text_zoom_factor);
        }
        // re-check same condition after zoom_factor update
        ZoomOutLabel.Enabled = (m_text_zoom_factor >= (m_min_zoom_factor + m_zoom_factor_increment - m_error_margin));
        ZoomInLabel.Enabled = true;
    }
    // wordwrap mode
    private bool m_word_wrap_main_textbox = false;
    private bool m_word_wrap_search_textbox = false;
    private void ApplyWordWrapSettings()
    {
        try
        {
            MainTextBox.BeginUpdate();
            SearchResultTextBox.BeginUpdate();

            MainTextBox.WordWrap = m_word_wrap_main_textbox;
            SearchResultTextBox.WordWrap = m_word_wrap_search_textbox;

            Verse.IncludeNumber = m_word_wrap_main_textbox;

            UpdateWordWrapLabel(m_word_wrap_main_textbox);
        }
        finally
        {
            MainTextBox.EndUpdate();
            SearchResultTextBox.EndUpdate();
        }
    }
    private void UpdateWordWrapLabel(bool word_wrap)
    {
        if (word_wrap)
        {
            if (File.Exists(Globals.IMAGES_FOLDER + "/" + "arrow_left.png"))
            {
                WordWrapLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "arrow_left.png");
            }
            ToolTip.SetToolTip(WordWrapLabel, "Unwrap text");
        }
        else
        {
            if (File.Exists(Globals.IMAGES_FOLDER + "/" + "arrow_rotate_anticlockwise.png"))
            {
                WordWrapLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "arrow_rotate_anticlockwise.png");
            }
            ToolTip.SetToolTip(WordWrapLabel, "Wrap text");
        }
    }
    private void WordWrapLabel_Click(object sender, EventArgs e)
    {
        ToggleWordWrap();
    }
    // add/remove Verse.EndMark, wrap/unwrap and redisplay
    private void ToggleWordWrap() // F11
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_active_textbox != null)
            {
                //m_active_textbox.TextChanged -= new EventHandler(MainTextBox_TextChanged);
                m_active_textbox.BeginUpdate();

                Verse curent_verse = null;
                if (m_selection_mode == false)
                {
                    curent_verse = GetVerse(CurrentVerseIndex);
                }

                m_active_textbox.WordWrap = !m_active_textbox.WordWrap;
                if (m_found_verses_displayed)
                {
                    m_word_wrap_search_textbox = m_active_textbox.WordWrap;
                    Verse.IncludeNumber = false;

                    UpdateWordWrapLabel(m_word_wrap_search_textbox);

                    // no text is changed so no need to redisplay and recolorize
                    //DisplayFoundVerses(false);
                }
                else
                {
                    m_word_wrap_main_textbox = m_active_textbox.WordWrap;
                    Verse.IncludeNumber = m_word_wrap_main_textbox;

                    UpdateWordWrapLabel(m_word_wrap_main_textbox);

                    // re-display as verse changed IncludeNumber
                    DisplaySelection(false);
                }

                if (curent_verse != null)
                {
                    HighlightVerse(curent_verse);
                }
            }
        }
        finally
        {
            m_active_textbox.EndUpdate();
            //m_active_textbox.TextChanged += new EventHandler(MainTextBox_TextChanged);
            this.Cursor = Cursors.Default;
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 07. Verses
    ///////////////////////////////////////////////////////////////////////////////
    // navigation
    private int m_current_selection_verse_index = 0;
    private int m_current_found_verse_index = 0;
    private int CurrentVerseIndex
    {
        get
        {
            if (m_found_verses_displayed)
            {
                return m_current_found_verse_index;
            }
            else
            {
                return m_current_selection_verse_index;
            }
        }
        set
        {
            if (m_client != null)
            {
                if (m_found_verses_displayed)
                {
                    if (m_client.FoundVerses != null)
                    {
                        if ((value >= 0) && (value < m_client.FoundVerses.Count))
                        {
                            m_current_found_verse_index = value;
                        }
                    }
                }
                else
                {
                    if (m_client.Selection != null)
                    {
                        if (m_client.Selection.Verses != null)
                        {
                            if ((value >= 0) && (value < m_client.Selection.Verses.Count))
                            {
                                m_current_selection_verse_index = value;
                            }
                        }
                    }
                }
            }
        }
    }
    private Verse GetCurrentVerse()
    {
        return GetVerse(CurrentVerseIndex);
    }
    private List<Verse> GetCurrentVerses()
    {
        List<Verse> result = new List<Verse>();
        char[] separators = { '\n', Constants.OPEN_BRACKET[0] };
        string[] lines = m_current_text.Split(separators);
        int current_verse_index = CurrentVerseIndex;
        for (int i = current_verse_index; i < current_verse_index + lines.Length; i++)
        {
            result.Add(GetVerse(i));
        }
        return result;
    }
    private Verse GetVerse(int verse_index)
    {
        if (m_client != null)
        {
            List<Verse> verses = null;
            if (m_found_verses_displayed)
            {
                verses = m_client.FoundVerses;
            }
            else // m_curent_verses displayed
            {
                if (m_client.Selection != null)
                {
                    verses = m_client.Selection.Verses;
                }
            }

            if (verses != null)
            {
                if ((verse_index >= 0) && (verse_index < verses.Count))
                {
                    return verses[verse_index];
                }
            }
        }
        return null;
    }
    private int GetVerseDisplayStart(Verse verse)
    {
        int start = 0;
        if (m_client != null)
        {
            if (verse != null)
            {
                List<Verse> verses = null;
                if (m_found_verses_displayed)
                {
                    verses = m_client.FoundVerses;
                }
                else
                {
                    if (m_client.Selection != null)
                    {
                        verses = m_client.Selection.Verses;
                    }
                }

                if (verses != null)
                {
                    foreach (Verse v in verses)
                    {
                        if (v == verse) break;

                        if (m_found_verses_displayed)
                        {//                            \t                  \n
                            start += v.Address.Length + 1 + v.Text.Length + 1;
                        }
                        else
                        {
                            start += v.Text.Length + v.Endmark.Length;
                        }
                    }
                }
            }
        }
        return start;
    }
    private int GetVerseDisplayLength(Verse verse)
    {
        int length = 0;
        if (verse != null)
        {
            if (m_found_verses_displayed)
            {//                                \t                       \n
                length = verse.Address.Length + 1 + verse.Text.Length + 1;
            }
            else
            {//                                 { # }  or  \n
                length = verse.Text.Length + verse.Endmark.Length;
            }
        }
        return length;
    }
    private int GetWordDisplayStart(Word word) //??? should be int word_index in RichTextBox
    {
        int start = 0;
        if (word != null)
        {
            if (m_client != null)
            {
                List<Verse> verses = null;
                if (m_found_verses_displayed)
                {
                    verses = m_client.FoundVerses;
                }
                else
                {
                    if (m_client.Selection != null)
                    {
                        verses = m_client.Selection.Verses;
                    }
                }

                foreach (Verse verse in verses)
                {
                    if (verse == word.Verse)  //??? this ill bring first matching word only
                    {
                        start += word.Position;
                        break;
                    }
                    start += GetVerseDisplayLength(verse);
                }
            }
        }
        return start;
    }
    private int GetWordDisplayLength(Word word)
    {
        if (word != null)
        {
            if (word.Text != null)
            {
                return word.Text.Length + 1;
            }
        }
        return 0;
    }
    // highlighting verse/word
    private Verse m_previous_highlighted_verse = null;
    private void HighlightVerse(Verse verse)
    {
        if (m_active_textbox != null)
        {
            try
            {
                m_active_textbox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
                m_active_textbox.BeginUpdate();

                // de-highlight previous verse
                if (m_previous_highlighted_verse != null)
                {
                    int start = GetVerseDisplayStart(m_previous_highlighted_verse);
                    int length = GetVerseDisplayLength(m_previous_highlighted_verse);
                    m_active_textbox.ClearHighlight(start, length);
                }

                // highlight this verse
                if (verse != null)
                {
                    int start = GetVerseDisplayStart(verse);
                    int length = GetVerseDisplayLength(verse);
                    m_active_textbox.Highlight(start, length - 1, Color.Lavender); // -1 so de-highlighting can clean the last \n at the end of all text

                    // ####### re-wire MainTextBox_SelectionChanged event
                    m_active_textbox.EndUpdate();
                    m_active_textbox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
                    CalculateCurrentValue(); // will update translation too !!!

                    // move cursor to verse start
                    m_active_textbox.Select(start, 0);

                    // updates verse position and value when cursor goes to start of verse
                    CurrentVerseIndex = GetVerseIndex(verse);
                    UpdatePlayerButtons(verse);

                    // backup highlighted verse
                    m_previous_highlighted_verse = verse;
                }
                else
                {
                    m_active_textbox.EndUpdate();
                    m_active_textbox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
                }
            }
            finally
            {
                // ####### already re-wired above
                ////m_active_textbox.EndUpdate();
                ////m_active_textbox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
            }
        }
    }
    private Word m_previous_highlighted_word = null;
    private void HighlightWord(Word word)
    {
        if (m_active_textbox != null)
        {
            int backup_selection_start = m_active_textbox.SelectionStart;
            int backup_selection_length = m_active_textbox.SelectionLength;

            // de-highlight previous word
            if (m_previous_highlighted_word != null)
            {
                int start = GetWordDisplayStart(m_previous_highlighted_word);
                int length = GetWordDisplayLength(m_previous_highlighted_word);
                m_active_textbox.ClearHighlight(start, length);
            }

            // highlight this word
            if (word != null)
            {
                int start = GetWordDisplayStart(word);
                int length = GetWordDisplayLength(word);
                m_active_textbox.Highlight(start, length - 1, Color.Lavender); // -1 so de-highlighting can clean the last \n at the end of all text

                // backup highlighted word
                m_previous_highlighted_word = word;
            }

            //??? BAD DESIGN: if backup_selection is outside visible area, then this line will scroll to it and loses highlight above
            m_active_textbox.Select(backup_selection_start, backup_selection_length);
        }
    }
    private GoldenRatioScope m_golden_ratio_scope = GoldenRatioScope.None;
    private GoldenRatioOrder m_golden_ratio_order = GoldenRatioOrder.LongShort;
    private void GoldenRatioScopeLabel_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (ModifierKeys == Keys.Shift)
            {
                switch (m_golden_ratio_scope)
                {
                    case GoldenRatioScope.None:
                        {
                            m_golden_ratio_scope = GoldenRatioScope.Sentence;
                            if (File.Exists("Images/golden_sentence.png"))
                            {
                                GoldenRatioScopeLabel.Image = new Bitmap("Images/golden_sentence.png");
                                ToolTip.SetToolTip(GoldenRatioScopeLabel, "Sentence-level golden ratio");
                            }
                        }
                        break;
                    case GoldenRatioScope.Letter:
                        {
                            m_golden_ratio_scope = GoldenRatioScope.None;
                            if (File.Exists("Images/golden_none.png"))
                            {
                                GoldenRatioScopeLabel.Image = new Bitmap("Images/golden_none.png");
                                ToolTip.SetToolTip(GoldenRatioScopeLabel, "Golden ratio colorization");
                            }
                        }
                        break;
                    case GoldenRatioScope.Word:
                        {
                            m_golden_ratio_scope = GoldenRatioScope.Letter;
                            if (File.Exists("Images/golden_letter.png"))
                            {
                                GoldenRatioScopeLabel.Image = new Bitmap("Images/golden_letter.png");
                                ToolTip.SetToolTip(GoldenRatioScopeLabel, "Letter-level golden ratio");
                            }
                        }
                        break;
                    case GoldenRatioScope.Sentence:
                        {
                            m_golden_ratio_scope = GoldenRatioScope.Word;
                            if (File.Exists("Images/golden_word.png"))
                            {
                                GoldenRatioScopeLabel.Image = new Bitmap("Images/golden_word.png");
                                ToolTip.SetToolTip(GoldenRatioScopeLabel, "Word-level golden ratio");
                            }
                        }
                        break;
                }
            }
            else
            {
                switch (m_golden_ratio_scope)
                {
                    case GoldenRatioScope.None:
                        {
                            m_golden_ratio_scope = GoldenRatioScope.Letter;
                            if (File.Exists("Images/golden_letter.png"))
                            {
                                GoldenRatioScopeLabel.Image = new Bitmap("Images/golden_letter.png");
                                ToolTip.SetToolTip(GoldenRatioScopeLabel, "Letter-level golden ratio");
                            }
                        }
                        break;
                    case GoldenRatioScope.Letter:
                        {
                            m_golden_ratio_scope = GoldenRatioScope.Word;
                            if (File.Exists("Images/golden_word.png"))
                            {
                                GoldenRatioScopeLabel.Image = new Bitmap("Images/golden_word.png");
                                ToolTip.SetToolTip(GoldenRatioScopeLabel, "Word-level golden ratio");
                            }
                        }
                        break;
                    case GoldenRatioScope.Word:
                        {
                            m_golden_ratio_scope = GoldenRatioScope.Sentence;
                            if (File.Exists("Images/golden_sentence.png"))
                            {
                                GoldenRatioScopeLabel.Image = new Bitmap("Images/golden_sentence.png");
                                ToolTip.SetToolTip(GoldenRatioScopeLabel, "Sentence-level golden ratio");
                            }
                        }
                        break;
                    case GoldenRatioScope.Sentence:
                        {
                            m_golden_ratio_scope = GoldenRatioScope.None;
                            if (File.Exists("Images/golden_none.png"))
                            {
                                GoldenRatioScopeLabel.Image = new Bitmap("Images/golden_none.png");
                                ToolTip.SetToolTip(GoldenRatioScopeLabel, "Golden ratio colorization");
                            }
                        }
                        break;
                }
            }

            GoldenRatioOrderLabel.Enabled = (m_golden_ratio_scope != GoldenRatioScope.None);

            DisplaySelectionText();
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void GoldenRatioOrderLabel_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            switch (m_golden_ratio_order)
            {
                case GoldenRatioOrder.LongShort:
                    {
                        m_golden_ratio_order = GoldenRatioOrder.ShortLong;
                        if (File.Exists("Images/golden_sl.png"))
                        {
                            GoldenRatioOrderLabel.Image = new Bitmap("Images/golden_sl.png");
                            ToolTip.SetToolTip(GoldenRatioOrderLabel, "Golden ratio ~= 0.618 + 1");
                        }
                    }
                    break;
                case GoldenRatioOrder.ShortLong:
                    {
                        m_golden_ratio_order = GoldenRatioOrder.LongShort;
                        if (File.Exists("Images/golden_ls.png"))
                        {
                            GoldenRatioOrderLabel.Image = new Bitmap("Images/golden_ls.png");
                            ToolTip.SetToolTip(GoldenRatioOrderLabel, "Golden ratio ~= 1 + 0.618");
                        }
                    }
                    break;
            }

            DisplaySelectionText();
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void ColorizeGoldenRatios()
    {
        if (m_client != null)
        {
            if ((m_client.NumerologySystem != null) && (m_client.NumerologySystem.TextMode == "Original"))
            {
                ColorizeGoldenRatiosInOriginalText();
            }
            else
            {
                ColorizeGoldenRatiosInSimplifiedText();
            }
        }
    }
    private void ColorizeGoldenRatiosInOriginalText()
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            MainTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
            MainTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
            MainTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
            MainTextBox.BeginUpdate();

            if (m_client != null)
            {
                if (m_client.Selection != null)
                {
                    int verse_index = 0;
                    foreach (Verse verse in m_client.Selection.Verses)
                    {
                        if (verse != null)
                        {
                            int length = GetVerseDisplayLength(verse);
                            int start = GetVerseDisplayStart(verse);
                            verse_index += length; // move to next verse

                            if (m_client.NumerologySystem != null)
                            {
                                int verse_letter_count = verse.LetterCount;

                                int golden_letters = 0;
                                switch (m_golden_ratio_order)
                                {
                                    case GoldenRatioOrder.LongShort:
                                        {
                                            golden_letters = (int)Math.Round(((double)verse_letter_count / Numbers.PHI), 0);
                                        }
                                        break;
                                    case GoldenRatioOrder.ShortLong:
                                        {
                                            golden_letters = verse_letter_count - (int)Math.Round(((double)verse_letter_count / Numbers.PHI), 0);
                                        }
                                        break;
                                }

                                int golden_space_stopmarks_diacritics = 0;
                                bool colorize = false;
                                int count = 0;
                                for (int i = 0; i < verse.Text.Length; i++)
                                {
                                    if (Constants.ARABIC_LETTERS.Contains(verse.Text[i]))
                                    {
                                        count++;
                                        if (count == golden_letters)
                                        {
                                            switch (m_golden_ratio_scope)
                                            {
                                                case GoldenRatioScope.None:
                                                    {
                                                        colorize = false;
                                                    }
                                                    break;
                                                case GoldenRatioScope.Letter:
                                                    {
                                                        colorize = true;
                                                    }
                                                    break;
                                                case GoldenRatioScope.Word:
                                                    {
                                                        for (int j = 1; j < verse.Text.Length - i; j++)
                                                        {
                                                            if (Constants.ARABIC_LETTERS.Contains(verse.Text[i + j]))
                                                            {
                                                                break;
                                                            }
                                                            if (verse.Text[i + j] == ' ')
                                                            {
                                                                colorize = true;
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case GoldenRatioScope.Sentence:
                                                    {
                                                        for (int j = 1; j < verse.Text.Length - i; j++)
                                                        {
                                                            if (Constants.ARABIC_LETTERS.Contains(verse.Text[i + j]))
                                                            {
                                                                break;
                                                            }
                                                            else if (Constants.STOPMARKS.Contains(verse.Text[i + j]))
                                                            {
                                                                colorize = true;
                                                            }
                                                            else
                                                            {
                                                                continue; // skip space, harakaat, or QURANMARKS 
                                                            }
                                                        }
                                                    }
                                                    break;
                                            }

                                            // in all cases
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        golden_space_stopmarks_diacritics++;
                                    }
                                }
                                if (!colorize) continue; // skip verse

                                int golden_length = golden_letters + golden_space_stopmarks_diacritics;

                                MainTextBox.Colorize(start, golden_length, Color.Navy);
                                MainTextBox.Colorize(start + golden_length, verse.Text.Length - golden_length, Color.Red);

                                // reset color back to Navy for subsequent display
                                if (MainTextBox.Text.Length > 0)
                                {
                                    MainTextBox.Colorize(0, 1, Color.Navy);
                                }

                                MainTextBox.AlignToStart();
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            MainTextBox.EndUpdate();
            MainTextBox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
            this.Cursor = Cursors.Default;
        }
    }
    private void ColorizeGoldenRatiosInSimplifiedText()
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            MainTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
            MainTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
            MainTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
            MainTextBox.BeginUpdate();

            if (m_client != null)
            {
                if (m_client.Selection != null)
                {
                    int verse_index = 0;
                    foreach (Verse verse in m_client.Selection.Verses)
                    {
                        if (verse != null)
                        {
                            int length = GetVerseDisplayLength(verse);
                            int start = GetVerseDisplayStart(verse);
                            verse_index += length; // move to next verse

                            if (m_client.NumerologySystem != null)
                            {
                                int verse_letter_count = verse.LetterCount;

                                int golden_letters = 0;
                                switch (m_golden_ratio_order)
                                {
                                    case GoldenRatioOrder.LongShort:
                                        {
                                            golden_letters = (int)Math.Round(((double)verse_letter_count / Numbers.PHI), 0);
                                        }
                                        break;
                                    case GoldenRatioOrder.ShortLong:
                                        {
                                            golden_letters = verse_letter_count - (int)Math.Round(((double)verse_letter_count / Numbers.PHI), 0);
                                        }
                                        break;
                                }

                                bool colorize = false;
                                int letter_count = 0;
                                int space_count = 0;
                                bool break_outer_loop = false;

                                if (verse.Words != null)
                                {
                                    foreach (Word word in verse.Words)
                                    {
                                        if ((word.Letters != null) && (word.Letters.Count > 0))
                                        {
                                            foreach (Letter letter in word.Letters)
                                            {
                                                letter_count++;
                                                if (letter_count == golden_letters)
                                                {
                                                    switch (m_golden_ratio_scope)
                                                    {
                                                        case GoldenRatioScope.None:
                                                            {
                                                                colorize = false;
                                                            }
                                                            break;
                                                        case GoldenRatioScope.Letter:
                                                            {
                                                                colorize = true;
                                                            }
                                                            break;
                                                        case GoldenRatioScope.Word:
                                                            {
                                                                if (letter.NumberInWord == word.Letters.Count) // last letter
                                                                {
                                                                    colorize = true;
                                                                }
                                                                else
                                                                {
                                                                    break_outer_loop = true;
                                                                    break; // break if not last letter
                                                                }
                                                            }
                                                            break;
                                                        case GoldenRatioScope.Sentence:
                                                            {
                                                                if (letter.NumberInWord == word.Letters.Count) // last letter
                                                                {
                                                                    if (word.Stopmark != Stopmark.None)
                                                                    {
                                                                        colorize = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        break_outer_loop = true;
                                                                        break; // break if not end of sentence
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    break_outer_loop = true;
                                                                    break; // break if not last letter
                                                                }
                                                            }
                                                            break;
                                                    }

                                                    // in all cases
                                                    break_outer_loop = true;
                                                    break;
                                                }
                                            }
                                            if (break_outer_loop)
                                            {
                                                break; // outer loop
                                            }

                                            space_count++;
                                        }
                                    }
                                }

                                if (!colorize) continue; // skip verse

                                int gloden_length = golden_letters + space_count;

                                MainTextBox.Colorize(start, gloden_length, Color.Navy);
                                MainTextBox.Colorize(start + gloden_length, verse.Text.Length - gloden_length, Color.Red);

                                // reset color back to Navy for subsequent display
                                if (MainTextBox.Text.Length > 0)
                                {
                                    MainTextBox.Colorize(0, 1, Color.Navy);
                                }

                                MainTextBox.AlignToStart();
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            MainTextBox.EndUpdate();
            MainTextBox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
            this.Cursor = Cursors.Default;
        }
    }
    // helpers
    private Verse GetVerseAtCursor()
    {
        if (m_active_textbox != null)
        {
            int start = m_active_textbox.SelectionStart;
            return GetVerseAtChar(start);
        }
        return null;
    }
    private Word GetWordAtCursor()
    {
        if (m_active_textbox != null)
        {
            int char_index = m_active_textbox.SelectionStart;
            if (char_index > 0)
            {
                return GetWordAtChar(char_index);
            }
        }
        return null;
    }
    private Letter GetLetterAtCursor()
    {
        if (m_active_textbox != null)
        {
            int char_index = m_active_textbox.SelectionStart;
            if (char_index > 0)
            {
                return GetLetterAtChar(char_index);
            }
        }
        return null;
    }
    private Verse GetVerseAtPointer(MouseEventArgs e)
    {
        return GetVerseAtLocation(e.Location);
    }
    private Word GetWordAtPointer(MouseEventArgs e)
    {
        return GetWordAtLocation(e.Location);
    }
    private Letter GetLetterAtPointer(MouseEventArgs e)
    {
        return GetLetterAtLocation(e.Location);
    }
    private Verse GetVerseAtLocation(Point mouse_location)
    {
        if (m_active_textbox != null)
        {
            int char_index = m_active_textbox.GetCharIndexFromPosition(mouse_location);
            if (char_index > 0)
            {
                return GetVerseAtChar(char_index);
            }
        }
        return null;
    }
    private Word GetWordAtLocation(Point mouse_location)
    {
        if (m_active_textbox != null)
        {
            int char_index = m_active_textbox.GetCharIndexFromPosition(mouse_location);
            if (char_index > 0)
            {
                return GetWordAtChar(char_index);
            }
        }
        return null;
    }
    private Letter GetLetterAtLocation(Point mouse_location)
    {
        if (m_active_textbox != null)
        {
            int char_index = m_active_textbox.GetCharIndexFromPosition(mouse_location);
            if (char_index > 0)
            {
                return GetLetterAtChar(char_index);
            }
        }
        return null;
    }
    // helper helpers
    private Verse GetVerseAtChar(int char_index)
    {
        if (m_client != null)
        {
            List<Verse> verses = null;
            if (m_found_verses_displayed)
            {
                verses = m_client.FoundVerses;
            }
            else
            {
                if (m_client.Selection != null)
                {
                    verses = m_client.Selection.Verses;
                }
            }

            if (verses != null)
            {
                Verse scanned_verse = null;
                foreach (Verse verse in verses)
                {
                    int start = GetVerseDisplayStart(verse);
                    if (char_index < start)
                    {
                        return scanned_verse;
                    }
                    scanned_verse = verse;
                }
                return scanned_verse;
            }
        }
        return null;
    }
    private Word GetWordAtChar(int char_index)
    {
        Word word = null;
        if (m_client != null)
        {
            if (m_found_verses_displayed)
            {
                List<Verse> verses = m_client.FoundVerses;
                if (verses != null)
                {
                    foreach (Verse verse in verses)
                    {
                        int length = GetVerseDisplayLength(verse);
                        if (char_index >= length)
                        {
                            char_index -= length;
                        }
                        else
                        {
                            // verse found, remove verse address
                            char_index -= verse.Address.Length + 1; // \t

                            int word_index = CalculateWordIndex(verse, char_index);
                            if ((word_index >= 0) && (word_index < verse.Words.Count))
                            {
                                word = verse.Words[word_index];
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (m_client.Selection != null)
                {
                    List<Verse> verses = m_client.Selection.Verses;
                    if (verses != null)
                    {
                        foreach (Verse verse in verses)
                        {
                            if ((char_index >= verse.Text.Length) && (char_index < (verse.Text.Length + verse.Endmark.Length - 1)))
                            {
                                return null; // don't return a word at verse Endmark
                            }

                            int length = GetVerseDisplayLength(verse);
                            if (char_index >= length)
                            {
                                char_index -= length;
                            }
                            else
                            {
                                int word_index = CalculateWordIndex(verse, char_index);
                                if ((word_index >= 0) && (word_index < verse.Words.Count))
                                {
                                    word = verse.Words[word_index];
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        return word;
    }
    private Letter GetLetterAtChar(int char_index)
    {
        if (m_client != null)
        {
            if (m_found_verses_displayed)
            {
                List<Verse> verses = m_client.FoundVerses;
                if (verses != null)
                {
                    foreach (Verse verse in verses)
                    {
                        int length = GetVerseDisplayLength(verse);
                        if (char_index >= length)
                        {
                            char_index -= length;
                        }
                        else
                        {
                            // remove verse address
                            char_index -= verse.Address.Length + 1; // \t

                            int letter_index = CalculateLetterIndex(verse, char_index);
                            if ((letter_index >= 0) && (letter_index < verse.LetterCount))
                            {
                                return verse.GetLetter(letter_index);
                            }
                        }
                    }
                }
            }
            else
            {
                if (m_client.Selection != null)
                {
                    List<Verse> verses = m_client.Selection.Verses;
                    if (verses != null)
                    {
                        foreach (Verse verse in verses)
                        {
                            int length = GetVerseDisplayLength(verse);
                            if (char_index >= length)
                            {
                                char_index -= length;
                            }
                            else
                            {
                                int letter_index = CalculateLetterIndex(verse, char_index);
                                if ((letter_index >= 0) && (letter_index < verse.LetterCount))
                                {
                                    return verse.GetLetter(letter_index);
                                }
                            }
                        }
                    }
                }
            }
        }
        return null;
    }
    // helper helper helpers
    /// <summary>
    /// Use only when no duplicate verses are displayed like with VerseRanges or ChapterRanges
    /// </summary>
    /// <param name="verse"></param>
    /// <returns>index of first matching verse</returns>
    private int GetVerseIndex(Verse verse)
    {
        if (m_client != null)
        {
            List<Verse> verses = null;
            if (m_found_verses_displayed)
            {
                verses = m_client.FoundVerses;
            }
            else
            {
                if (m_client.Selection != null)
                {
                    verses = m_client.Selection.Verses;
                }
            }

            if (verses != null)
            {
                int verse_index = -1;
                foreach (Verse v in verses)
                {
                    verse_index++;
                    if (v == verse)
                    {
                        return verse_index;
                    }
                }
            }
        }
        return -1;
    }
    private int CalculateWordIndex(Verse verse, int char_index)
    {
        int word_index = -1;
        if (verse != null)
        {
            string[] word_texts = verse.Text.Split();
            foreach (string word_text in word_texts)
            {
                // skip stopmarks (1-letter words), except real Quranic 1-letter words
                if (
                     (word_text.Length == 1)
                     &&
                     !((word_text == "ص") || (word_text == "ق") || (word_text == "ن") || (word_text == "و"))
                   )
                {
                    // skip stopmark words
                    char_index -= word_text.Length + 1; // 1 for space
                }
                else
                {
                    word_index++;

                    if (char_index < word_text.Length)
                    {
                        break;
                    }
                    char_index -= word_text.Length + 1; // 1 for space
                }
            }
        }
        return word_index;
    }
    private int CalculateLetterIndex(Verse verse, int char_index)
    {
        int letter_index = -1;
        if (verse != null)
        {
            // before verse start
            if (char_index < 0)
            {
                char_index = 0;
            }
            // after verse end
            else if (char_index >= verse.Text.Length)
            {
                char_index = verse.Text.Length - 1;
            }

            for (int i = 0; i <= char_index; i++)
            {
                if (Constants.ARABIC_LETTERS.Contains(verse.Text[i]))
                {
                    letter_index++;
                }
            }
        }
        return letter_index;
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 08. Chapters
    ///////////////////////////////////////////////////////////////////////////////
    private void PopulateChapterComboBox()
    {
        try
        {
            ChapterComboBox.SelectedIndexChanged -= new EventHandler(ChapterComboBox_SelectedIndexChanged);
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    ChapterComboBox.BeginUpdate();
                    ChapterComboBox.Items.Clear();
                    if (m_client.Book.Chapters != null)
                    {
                        foreach (Chapter chapter in m_client.Book.Chapters)
                        {
                            ChapterComboBox.Items.Add(chapter.Number + " - " + chapter.Name);
                        }
                    }
                }
            }
        }
        finally
        {
            ChapterComboBox.EndUpdate();
            ChapterComboBox.SelectedIndexChanged += new EventHandler(ChapterComboBox_SelectedIndexChanged);
        }
    }
    private void PopulateChaptersListBox()
    {
        try
        {
            for (int i = 0; i < 3; i++) ChaptersListBox.SelectedIndexChanged -= new EventHandler(ChaptersListBox_SelectedIndexChanged);
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    ChaptersListBox.BeginUpdate();

                    ChaptersListBox.Items.Clear();
                    ChaptersListBox.ClearItemColors(); // cannot override Items.Clear cos not virtual so use this wrapper method
                    if (m_client.Book.Chapters != null)
                    {
                        if (m_found_verses_displayed)
                        {
                            foreach (Chapter chapter in m_client.Book.Chapters)
                            {
                                ChaptersListBox.Items.Add(String.Format("{0,-3} {2,-3}  {1}", chapter.Number, chapter.Name, chapter.Verses.Count));

                                int match_count = 0;
                                if (m_matches_per_chapter != null)
                                {
                                    match_count = m_matches_per_chapter[chapter.Number - 1];
                                }

                                // use color shading to represent match_count visually
                                Color color = ChaptersListBox.BackColor;
                                if (match_count > 0)
                                {
                                    int red = 224;
                                    int green = 224;
                                    int blue = 255;
                                    green -= (match_count * 16);
                                    if (green < 0)
                                    {
                                        red += green;
                                        green = 0;
                                    }
                                    if (red < 0)
                                    {
                                        blue += red;
                                        red = 0;
                                    }
                                    if (blue < 0)
                                    {
                                        blue = 0;
                                    }
                                    color = Color.FromArgb(red, green, blue);
                                }
                                ChaptersListBox.SetItemColor(chapter.Number - 1, color);

                                int matching_chapters = 0;
                                if (m_matches_per_chapter != null)
                                {
                                    foreach (int chapter_match_count in m_matches_per_chapter)
                                    {
                                        if (chapter_match_count > 0)
                                        {
                                            matching_chapters++;
                                        }
                                    }
                                }
                                ChapterGroupBox.ForeColor = Color.Black;
                                ChapterGroupBox.Text = ((matching_chapters > 99) ? "" : ((matching_chapters > 9) ? " " : "  ")) + matching_chapters + " Chapters        ";
                                this.ToolTip.SetToolTip(this.ChapterGroupBox, "Found chapters");
                            }
                        }
                        else // selection displayed
                        {
                            foreach (Chapter chapter in m_client.Book.Chapters)
                            {
                                ChaptersListBox.Items.Add(String.Format("{0,-3} {2,-3}  {1}", chapter.Number, chapter.Name, chapter.Verses.Count));
                                ChaptersListBox.SetItemColor(chapter.Number - 1, CHAPTER_INITIALIZATION_COLORS[(int)chapter.InitializationType]);
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            ChaptersListBox.EndUpdate();
            ChaptersListBox.SelectedIndexChanged += new EventHandler(ChaptersListBox_SelectedIndexChanged);
        }
    }
    private void DisplayChapterRevelationInfo()
    {
        if (m_found_verses_displayed) return;

        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                if (ChapterComboBox.SelectedIndex > -1)
                {
                    int index = ChapterComboBox.SelectedIndex;
                    if (m_client.Book.Chapters != null)
                    {
                        Chapter chapter = m_client.Book.Chapters[index];
                        if (chapter != null)
                        {
                            string arabic_revelation_place = null;
                            switch (chapter.RevelationPlace)
                            {
                                case RevelationPlace.Makkah:
                                    arabic_revelation_place = "مكّية";
                                    break;
                                case RevelationPlace.Medina:
                                    arabic_revelation_place = "مدنيّة";
                                    break;
                                default:
                                    arabic_revelation_place = "";
                                    break;
                            }
                            ChapterGroupBox.Text = "     " + chapter.RevelationOrder.ToString() + " - " + arabic_revelation_place + "        ";
                            //ChapterGroupBox.Text = "     " + chapter.RevelationOrder.ToString().ToNth() + " - " + arabic_revelation_place + "        ";
                        }
                    }
                }
                else
                {
                    ChapterGroupBox.Text = "";
                }
                this.ToolTip.SetToolTip(this.ChapterGroupBox, "Revelation التنزيل");

                UpdateChapterGroupBoxTextColor();
            }
        }
    }
    private void UpdateSelection()
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                if (ChaptersListBox.SelectedIndices.Count > 0)
                {
                    SelectionScope scope = SelectionScope.Chapter;
                    List<int> indexes = new List<int>();
                    for (int i = 0; i < ChaptersListBox.SelectedIndices.Count; i++)
                    {
                        int selected_index = ChaptersListBox.SelectedIndices[i];
                        if (m_client.Book.Chapters != null)
                        {
                            if ((selected_index >= 0) && (selected_index < m_client.Book.Chapters.Count))
                            {
                                Chapter chapter = m_client.Book.Chapters[selected_index];
                                if (chapter != null)
                                {
                                    indexes.Add(chapter.Number - 1);
                                }
                            }
                        }
                    }
                    m_client.Selection = new Selection(m_client.Book, scope, indexes);
                }
            }
        }
    }
    private void UpdateChaptersListBox()
    {
        if (m_client != null)
        {
            if (m_client.Selection != null)
            {
                try
                {
                    ChaptersListBox.SelectedIndexChanged -= new EventHandler(ChaptersListBox_SelectedIndexChanged);
                    if (m_found_verses_displayed)
                    {
                        //??? wrongly removes selections of FindChapters result
                        //??? selects found chapters losing all color-shade information
                        //if (m_client.FoundVerses != null)
                        //{
                        //    List<Chapter> chapters = m_client.Book.GetChapters(m_client.FoundVerses);
                        //    ChaptersListBox.SelectedIndices.Clear();
                        //    foreach (Chapter chapter in chapters)
                        //    {
                        //        ChaptersListBox.SelectedIndices.Add(chapter.Number - 1);
                        //    }
                        //}
                    }
                    else
                    {
                        if (m_client.Selection.Chapters != null)
                        {
                            ChaptersListBox.SelectedIndices.Clear();
                            foreach (Chapter chapter in m_client.Selection.Chapters)
                            {
                                ChaptersListBox.SelectedIndices.Add(chapter.Number - 1);
                            }
                        }
                    }
                }
                finally
                {
                    ChaptersListBox.SelectedIndexChanged += new EventHandler(ChaptersListBox_SelectedIndexChanged);
                }
            }
        }
    }
    private void UpdateMinMaxChapterVerseWordLetter(int chapter_index)
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                if (m_client.Book.Chapters != null)
                {
                    if ((chapter_index >= 0) && (chapter_index < m_client.Book.Chapters.Count))
                    {
                        Chapter chapter = m_client.Book.Chapters[chapter_index];
                        if (chapter != null)
                        {
                            try
                            {
                                ChapterVerseNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                                ChapterWordNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                                ChapterLetterNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);

                                ChapterVerseNumericUpDown.Minimum = 1;
                                ChapterVerseNumericUpDown.Maximum = chapter.Verses.Count;

                                ChapterWordNumericUpDown.Minimum = 1;
                                ChapterWordNumericUpDown.Maximum = chapter.WordCount;

                                ChapterLetterNumericUpDown.Minimum = 1;
                                ChapterLetterNumericUpDown.Maximum = chapter.LetterCount;
                            }
                            finally
                            {
                                ChapterVerseNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                                ChapterWordNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                                ChapterLetterNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                            }
                        }
                    }
                }
            }
        }
    }
    private void UpdateChapterGroupBoxTextColor()
    {
        Verse verse = GetCurrentVerse();
        if (verse != null)
        {
            ChapterGroupBox.ForeColor = CHAPTER_INITIALIZATION_COLORS[(int)verse.Chapter.InitializationType];
            ChapterGroupBox.Refresh();
        }
    }

    private void ChapterComboBox_KeyDown(object sender, KeyEventArgs e)
    {
        bool SeparatorKeys = (
            ((e.KeyCode == Keys.Subtract) && (e.Modifiers != Keys.Shift))           // HYPHEN
            || ((e.KeyCode == Keys.OemMinus) && (e.Modifiers != Keys.Shift))        // HYPHEN
            || ((e.KeyCode == Keys.Oemcomma) && (e.Modifiers != Keys.Shift))        // COMMA
            || ((e.KeyCode == Keys.OemSemicolon) && (e.Modifiers == Keys.Shift))    // COLON
            );

        bool NumericKeys = (
            ((e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            && e.Modifiers != Keys.Shift);

        bool EditKeys = (
            (e.KeyCode == Keys.A && e.Modifiers == Keys.Control) ||
            (e.KeyCode == Keys.Z && e.Modifiers == Keys.Control) ||
            (e.KeyCode == Keys.X && e.Modifiers == Keys.Control) ||
            (e.KeyCode == Keys.C && e.Modifiers == Keys.Control) ||
            (e.KeyCode == Keys.V && e.Modifiers == Keys.Control) ||
            e.KeyCode == Keys.Delete ||
            e.KeyCode == Keys.Back);

        bool NavigationKeys = (
            e.KeyCode == Keys.Up ||
            e.KeyCode == Keys.Right ||
            e.KeyCode == Keys.Down ||
            e.KeyCode == Keys.Left ||
            e.KeyCode == Keys.Home ||
            e.KeyCode == Keys.End);

        bool ExecuteKeys = (e.KeyCode == Keys.Enter);

        if (ExecuteKeys)
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    try
                    {
                        string text = ChapterComboBox.Text;
                        if (!String.IsNullOrEmpty(text))
                        {
                            // 1, 3-4, 5:55, 3-4:19, 6:19-23, 24:35-27:62
                            SelectionScope scope = SelectionScope.Verse;
                            List<int> indexes = new List<int>();

                            foreach (string part in text.Split(','))
                            {
                                string[] range_parts = part.Split('-');
                                if (range_parts.Length == 1) // 1 | 5:55
                                {
                                    string[] sub_range_parts = part.Split(':');
                                    if (sub_range_parts.Length == 1) // 1
                                    {
                                        int chapter_number;
                                        if (int.TryParse(sub_range_parts[0], out chapter_number))
                                        {
                                            Chapter chapter = null;
                                            if (m_client.Book.Chapters != null)
                                            {
                                                foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                {
                                                    if (book_chapter.Number == chapter_number)
                                                    {
                                                        chapter = book_chapter;
                                                        break;
                                                    }
                                                }

                                                if (chapter != null)
                                                {
                                                    foreach (Verse verse in chapter.Verses)
                                                    {
                                                        indexes.Add(verse.Number - 1);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (sub_range_parts.Length == 2) // 5:55
                                    {
                                        int chapter_number;
                                        if (int.TryParse(sub_range_parts[0], out chapter_number)) // 5:55
                                        {
                                            int verse_number_in_chapter;
                                            if (int.TryParse(sub_range_parts[1], out verse_number_in_chapter))
                                            {
                                                Chapter chapter = null;
                                                if (m_client.Book.Chapters != null)
                                                {
                                                    foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                    {
                                                        if (book_chapter.Number == chapter_number)
                                                        {
                                                            chapter = book_chapter;
                                                            break;
                                                        }
                                                    }

                                                    if (chapter != null)
                                                    {
                                                        if (((verse_number_in_chapter - 1 >= 0) && ((verse_number_in_chapter - 1) < chapter.Verses.Count)))
                                                        {
                                                            int from_verse_index = chapter.Verses[verse_number_in_chapter - 1].Number - 1;
                                                            indexes.Add(from_verse_index);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (range_parts.Length == 2) // 3-4, 3-4:19, 6:19-23, 24:35-27:62
                                {
                                    int from_chapter_number;
                                    int to_chapter_number;
                                    if (int.TryParse(range_parts[0], out from_chapter_number)) // 3-4
                                    {
                                        if (int.TryParse(range_parts[1], out to_chapter_number))
                                        {
                                            if (from_chapter_number <= to_chapter_number)
                                            {
                                                for (int number = from_chapter_number; number <= to_chapter_number; number++)
                                                {
                                                    Chapter chapter = null;
                                                    if (m_client.Book.Chapters != null)
                                                    {
                                                        foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                        {
                                                            if (book_chapter.Number == number)
                                                            {
                                                                chapter = book_chapter;
                                                                break;
                                                            }
                                                        }

                                                        if (chapter != null)
                                                        {
                                                            foreach (Verse verse in chapter.Verses)
                                                            {
                                                                indexes.Add(verse.Number - 1);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else // 3-4:19
                                        {
                                            // range_parts[0] == 3
                                            // range_parts[1] == 4:19
                                            string[] to_range_parts = range_parts[1].Split(':'); // 4:19
                                            if (to_range_parts.Length == 2)
                                            {
                                                int from_verse_number_in_chapter = 1; // not specified so start from beginning of chapter

                                                if (int.TryParse(to_range_parts[0], out to_chapter_number))  // 4
                                                {
                                                    int to_verse_number_in_chapter;
                                                    if (int.TryParse(to_range_parts[1], out to_verse_number_in_chapter)) // 19
                                                    {
                                                        Chapter from_chapter = null;
                                                        if (m_client.Book.Chapters != null)
                                                        {
                                                            foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                            {
                                                                if (book_chapter.Number == from_chapter_number)
                                                                {
                                                                    from_chapter = book_chapter;
                                                                    break;
                                                                }
                                                            }

                                                            if (from_chapter != null)
                                                            {
                                                                if (((from_verse_number_in_chapter - 1 >= 0) && ((from_verse_number_in_chapter - 1) < from_chapter.Verses.Count)))
                                                                {
                                                                    int from_verse_index = from_chapter.Verses[from_verse_number_in_chapter - 1].Number - 1;

                                                                    Chapter to_chapter = null;
                                                                    foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                                    {
                                                                        if (book_chapter.Number == to_chapter_number)
                                                                        {
                                                                            to_chapter = book_chapter;
                                                                            break;
                                                                        }
                                                                    }
                                                                    if (to_chapter != null)
                                                                    {
                                                                        if (((to_verse_number_in_chapter - 1 >= 0) && ((to_verse_number_in_chapter - 1) < to_chapter.Verses.Count)))
                                                                        {
                                                                            int to_verse_index = to_chapter.Verses[to_verse_number_in_chapter - 1].Number - 1;
                                                                            for (int i = from_verse_index; i <= to_verse_index; i++)
                                                                            {
                                                                                indexes.Add(i);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else // "range_parts[0]" contains a colon ':'  // "6:19"-23, "24:35"-27:62
                                    {
                                        //int from_chapter_number;
                                        //int to_chapter_number;
                                        string[] from_parts = range_parts[0].Split(':');
                                        if (from_parts.Length == 2)
                                        {
                                            int from_verse_number_in_chapter;
                                            if (int.TryParse(from_parts[0], out from_chapter_number))
                                            {
                                                if (int.TryParse(from_parts[1], out from_verse_number_in_chapter))
                                                {
                                                    string[] to_parts = range_parts[1].Split(':'); // "range_parts[1]" may or may not contain a colon ':'  // 6:19-"23", 24:35-"27:62"
                                                    if (to_parts.Length == 1) // 6:19-"23"
                                                    {
                                                        int to_verse_number_in_chapter;
                                                        if (int.TryParse(to_parts[0], out to_verse_number_in_chapter))
                                                        {
                                                            if (from_verse_number_in_chapter <= to_verse_number_in_chapter)  // XX:19-23
                                                            {
                                                                Chapter from_chapter = null;
                                                                if (m_client.Book.Chapters != null)
                                                                {
                                                                    foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                                    {
                                                                        if (book_chapter.Number == from_chapter_number)
                                                                        {
                                                                            from_chapter = book_chapter;
                                                                            break;
                                                                        }
                                                                    }

                                                                    if (from_chapter != null)
                                                                    {
                                                                        if (((from_verse_number_in_chapter - 1 >= 0) && ((from_verse_number_in_chapter - 1) < from_chapter.Verses.Count)))
                                                                        {
                                                                            int from_verse_index = from_chapter.Verses[from_verse_number_in_chapter - 1].Number - 1;
                                                                            int to_verse_index = from_chapter.Verses[to_verse_number_in_chapter - 1].Number - 1;
                                                                            for (int i = from_verse_index; i <= to_verse_index; i++)
                                                                            {
                                                                                indexes.Add(i);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (to_parts.Length == 2) // 24:35-"27:62"
                                                    {
                                                        int to_verse_number_in_chapter;
                                                        if (int.TryParse(to_parts[0], out to_chapter_number))
                                                        {
                                                            if (int.TryParse(to_parts[1], out to_verse_number_in_chapter))
                                                            {
                                                                if (from_chapter_number <= to_chapter_number)  // 24:XX-27:XX // only worry about chapters
                                                                {
                                                                    Chapter from_chapter = null;
                                                                    if (m_client.Book.Chapters != null)
                                                                    {
                                                                        foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                                        {
                                                                            if (book_chapter.Number == from_chapter_number)
                                                                            {
                                                                                from_chapter = book_chapter;
                                                                                break;
                                                                            }
                                                                        }

                                                                        if (from_chapter != null)
                                                                        {
                                                                            if (((from_verse_number_in_chapter - 1 >= 0) && ((from_verse_number_in_chapter - 1) < from_chapter.Verses.Count)))
                                                                            {
                                                                                int from_verse_index = from_chapter.Verses[from_verse_number_in_chapter - 1].Number - 1;
                                                                                Chapter to_chapter = null;
                                                                                foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                                                {
                                                                                    if (book_chapter.Number == to_chapter_number)
                                                                                    {
                                                                                        to_chapter = book_chapter;
                                                                                        break;
                                                                                    }
                                                                                }
                                                                                if (to_chapter != null)
                                                                                {
                                                                                    if (((to_verse_number_in_chapter - 1 >= 0) && ((to_verse_number_in_chapter - 1) < to_chapter.Verses.Count)))
                                                                                    {
                                                                                        int to_verse_index = to_chapter.Verses[to_verse_number_in_chapter - 1].Number - 1;
                                                                                        for (int i = from_verse_index; i <= to_verse_index; i++)
                                                                                        {
                                                                                            indexes.Add(i);
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            m_client.Selection = new Selection(m_client.Book, scope, indexes);

                            PlayerStopLabel_Click(null, null);

                            DisplaySelection(true);
                        }
                    }
                    catch
                    {
                        // log exception
                    }
                }
            }
        }

        // reject all other keys
        if (!(SeparatorKeys || NumericKeys || EditKeys || NavigationKeys))
        {
            e.SuppressKeyPress = true;
            e.Handled = true;
        }
    }
    private void ChapterComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                List<Chapter> chapters = m_client.Book.Chapters;
                int index = ChapterComboBox.SelectedIndex;
                if ((index >= 0) && (index < chapters.Count))
                {
                    int chapter_index = chapters[index].Number - 1;

                    if (
                         ChapterComboBox.Focused ||
                         ChapterVerseNumericUpDown.Focused ||
                         ChapterWordNumericUpDown.Focused ||
                         ChapterLetterNumericUpDown.Focused ||
                         PageNumericUpDown.Focused ||
                         StationNumericUpDown.Focused ||
                         PartNumericUpDown.Focused ||
                         GroupNumericUpDown.Focused ||
                         HalfNumericUpDown.Focused ||
                         QuarterNumericUpDown.Focused ||
                         BowingNumericUpDown.Focused ||
                         VerseNumericUpDown.Focused ||
                         WordNumericUpDown.Focused ||
                         LetterNumericUpDown.Focused
                     )
                    {
                        UpdateSelection();
                    }
                    else if ((sender == PreviousBookmarkButton) || (sender == NextBookmarkButton))
                    {
                    }
                    else if ((sender == SelectionHistoryBackwardButton) || (sender == SelectionHistoryForwardButton))
                    {
                    }
                    else
                    {
                    }

                    PlayerStopLabel_Click(null, null);

                    DisplaySelection(false);
                }
            }
        }
    }

    private int m_previous_selected_index = -1;
    private void ChaptersListBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                int selected_index = ChaptersListBox.IndexFromPoint(e.Location);
                if (selected_index != m_previous_selected_index)
                {
                    m_previous_selected_index = selected_index;
                    if ((selected_index >= 0) && (selected_index < m_client.Book.Chapters.Count))
                    {
                        Chapter chapter = m_client.Book.Chapters[selected_index];
                        if (chapter != null)
                        {
                            int match_count = 0;
                            if (m_matches_per_chapter != null)
                            {
                                if ((selected_index >= 0) && (selected_index < m_matches_per_chapter.Length))
                                {
                                    match_count = m_matches_per_chapter[selected_index];
                                }
                            }

                            if (chapter.Verses != null)
                            {
                                if (chapter.Verses.Count > 2)
                                {
                                    ToolTip.SetToolTip(ChaptersListBox,
                                        chapter.Number.ToString() + " - " + chapter.TransliteratedName + " - " + chapter.EnglishName + "\r\n" +
                                        chapter.RevelationPlace.ToString() + " - " + chapter.RevelationOrder.ToString() + " \t " + chapter.Number.ToString() + "\r\n" +
                                        "Verses  \t\t " + chapter.Verses.Count.ToString() + "\r\n" +
                                        "Words   \t\t " + chapter.WordCount.ToString() + "\r\n" +
                                        "Letters \t\t " + chapter.LetterCount.ToString() + "\r\n" +
                                        "Unique Letters \t " + chapter.UniqueLetters.Count.ToString() + "\r\n" +
                                        (m_found_verses_displayed ? ("Matches" + "\t\t" + match_count.ToString() + "\r\n") : "") +
                                        "\r\n" +
                                        chapter.Verses[0].Text + ((selected_index == 41) ? ("\r\n" + chapter.Verses[1].Text) : "")
                                    );
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void ChaptersListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (sender == ChaptersListBox)
        {
            PlayerStopLabel_Click(null, null);

            if (m_found_verses_displayed)
            {
                // set chapter filter
                List<Chapter> chapters = new List<Chapter>();
                foreach (int index in ChaptersListBox.SelectedIndices)
                {
                    chapters.Add(m_client.Book.Chapters[index]);
                }
                m_client.FilterChapters = chapters;

                int pos = m_find_result_header.IndexOf(" of ");
                if (pos != -1)
                {
                    m_find_result_header = m_find_result_header.Substring(pos + 4);
                }
                int selected_chapters_match_count = 0;
                foreach (int index in ChaptersListBox.SelectedIndices)
                {
                    if (m_matches_per_chapter != null)
                    {
                        if ((index >= 0) && (index < m_matches_per_chapter.Length))
                        {
                            selected_chapters_match_count += m_matches_per_chapter[index];
                        }
                    }
                }
                m_find_result_header = selected_chapters_match_count + " of " + m_find_result_header;

                ClearFindMatches(); // clear m_find_matches for F3 to work correctly in filtered result
                if (m_search_type == SearchType.Numbers)
                {
                    switch (m_numbers_result_type)
                    {
                        case NumbersResultType.VerseRanges:
                            DisplayFoundVerseRanges(false, false);
                            break;
                        case NumbersResultType.VerseSets:
                            DisplayFoundVerseSets(false, false);
                            break;
                        case NumbersResultType.Chapters:
                            DisplayFoundChapters(false, false);
                            break;
                        case NumbersResultType.ChapterRanges:
                            DisplayFoundChapterRanges(false, false);
                            break;
                        case NumbersResultType.ChapterSets:
                            DisplayFoundChapterSets(false, false);
                            break;
                        default:
                            DisplayFoundVerses(false, false);
                            break;
                    }
                }
                else
                {
                    DisplayFoundVerses(false, false);
                }
            }
            else
            {
                UpdateSelection();
                DisplaySelection(true);
            }

            ChaptersListBox.Focus();
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 09. Display Selection
    ///////////////////////////////////////////////////////////////////////////////
    private bool m_selection_mode = false;
    private int m_word_number_in_verse = -1;
    private int m_letter_number_in_verse = -1;
    private int m_word_number_in_chapter = -1;
    private int m_letter_number_in_chapter = -1;
    private void NumericUpDown_Enter(object sender, EventArgs e)
    {
        SearchGroupBox_Leave(null, null);
        this.AcceptButton = null;

        // Ctrl+Click factorizes number
        if (ModifierKeys == Keys.Control)
        {
            long value = 0L;
            if (sender == ChapterComboBox)
            {
                if (ChapterComboBox.SelectedIndex != -1)
                {
                    string[] parts = ChapterComboBox.Text.Split('-');
                    if (parts.Length > 0)
                    {
                        value = long.Parse(parts[0]);
                    }
                }
            }
            else if (sender is NumericUpDown)
            {
                try
                {
                    value = (long)(sender as NumericUpDown).Value;
                }
                catch
                {
                    value = -1L; // error
                }
            }
            else if (sender is TextBox)
            {
                try
                {
                    value = long.Parse((sender as TextBox).Text);
                }
                catch
                {
                    value = -1L; // error
                }
            }
            else
            {
                value = -1L; // error
            }
            FactorizeValue(value, "Position", false);
        }
    }
    private void NumericUpDown_Leave(object sender, EventArgs e)
    {
        this.AcceptButton = null;
    }
    private void NumericUpDown_ValueChanged(object sender, EventArgs e)
    {
        Control control = sender as NumericUpDown;
        if (control != null)
        {
            if (control.Focused)
            {
                DisplayNumericSelection(control);
            }
        }
    }
    private void DisplayNumericSelection(Control control)
    {
        if (control is NumericUpDown)
        {
            if (control.Focused)
            {
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        ChapterVerseNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        ChapterWordNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        ChapterLetterNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        PageNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        StationNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        PartNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        GroupNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        HalfNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        QuarterNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        BowingNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        VerseNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        WordNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        LetterNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    }

                    int number = (int)((control as NumericUpDown).Value);

                    // backup number before as it will be overwritten with verse.Number
                    // if control is WordNumericUpDown OR LetterNumericUpDown or
                    // if control is ChapterWordNumericUpDown OR ChapterLetterNumericUpDown 
                    int word_number = 0;
                    int letter_number = 0;
                    if ((control == WordNumericUpDown) || (control == ChapterLetterNumericUpDown))
                    {
                        word_number = number;
                    }
                    else if ((control == LetterNumericUpDown) || (control == ChapterLetterNumericUpDown))
                    {
                        letter_number = number;
                    }

                    if (m_client != null)
                    {
                        if (m_client.Book != null)
                        {
                            if (m_client.Book.Verses != null)
                            {
                                SelectionScope scope = SelectionScope.Book;

                                if (control == ChapterVerseNumericUpDown)
                                {
                                    scope = SelectionScope.Verse;

                                    if (m_client.Book.Chapters != null)
                                    {
                                        int verse_number_in_chapter = (int)ChapterVerseNumericUpDown.Value;

                                        int selected_index = ChapterComboBox.SelectedIndex;
                                        if ((selected_index >= 0) && (selected_index < m_client.Book.Chapters.Count))
                                        {
                                            Chapter chapter = m_client.Book.Chapters[selected_index];
                                            if (chapter != null)
                                            {
                                                if (chapter.Verses != null)
                                                {
                                                    if (chapter.Verses != null)
                                                    {
                                                        if (chapter.Verses.Count > verse_number_in_chapter - 1)
                                                        {
                                                            Verse verse = chapter.Verses[verse_number_in_chapter - 1];
                                                            if (verse != null)
                                                            {
                                                                number = verse.Number;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if ((control == ChapterWordNumericUpDown) || (control == ChapterLetterNumericUpDown))
                                {
                                    scope = SelectionScope.Verse;

                                    if (m_client.Book.Chapters != null)
                                    {
                                        int selected_index = ChapterComboBox.SelectedIndex;
                                        if ((selected_index >= 0) && (selected_index < m_client.Book.Chapters.Count))
                                        {
                                            Chapter chapter = m_client.Book.Chapters[selected_index];
                                            if (chapter != null)
                                            {
                                                if (chapter.Verses != null)
                                                {
                                                    Verse verse = null;
                                                    if (control == ChapterWordNumericUpDown)
                                                    {
                                                        word_number = number + chapter.Verses[0].Words[0].Number - 1;
                                                        verse = m_client.Book.GetVerseByWordNumber(word_number);
                                                    }
                                                    else if (control == ChapterLetterNumericUpDown)
                                                    {
                                                        letter_number = number + chapter.Verses[0].Words[0].Letters[0].Number - 1;
                                                        verse = m_client.Book.GetVerseByLetterNumber(letter_number);
                                                    }

                                                    if (verse != null)
                                                    {
                                                        number = verse.Number;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (control == PageNumericUpDown)
                                {
                                    if (m_client.Book.Pages != null)
                                    {
                                        scope = SelectionScope.Page;
                                    }
                                }
                                else if (control == StationNumericUpDown)
                                {
                                    if (m_client.Book.Stations != null)
                                    {
                                        scope = SelectionScope.Station;
                                    }
                                }
                                else if (control == PartNumericUpDown)
                                {
                                    if (m_client.Book.Parts != null)
                                    {
                                        scope = SelectionScope.Part;
                                    }
                                }
                                else if (control == GroupNumericUpDown)
                                {
                                    if (m_client.Book.Groups != null)
                                    {
                                        scope = SelectionScope.Group;
                                    }
                                }
                                else if (control == HalfNumericUpDown)
                                {
                                    if (m_client.Book.Halfs != null)
                                    {
                                        scope = SelectionScope.Half;
                                    }
                                }
                                else if (control == QuarterNumericUpDown)
                                {
                                    if (m_client.Book.Quarters != null)
                                    {
                                        scope = SelectionScope.Quarter;
                                    }
                                }
                                else if (control == BowingNumericUpDown)
                                {
                                    if (m_client.Book.Bowings != null)
                                    {
                                        scope = SelectionScope.Bowing;
                                    }
                                }
                                else if (control == VerseNumericUpDown)
                                {
                                    if (m_client.Book.Verses != null)
                                    {
                                        scope = SelectionScope.Verse;
                                    }
                                }
                                else if (control == WordNumericUpDown)
                                {
                                    Verse verse = m_client.Book.GetVerseByWordNumber(word_number);
                                    if (verse != null)
                                    {
                                        scope = SelectionScope.Verse;
                                        number = verse.Number;
                                    }
                                }
                                else if (control == LetterNumericUpDown)
                                {
                                    Verse verse = m_client.Book.GetVerseByLetterNumber(letter_number);
                                    if (verse != null)
                                    {
                                        scope = SelectionScope.Verse;
                                        number = verse.Number;
                                    }
                                }
                                else
                                {
                                    // do nothing
                                }

                                // if selection has changed
                                if (m_client.Selection != null)
                                {
                                    if (
                                        (m_client.Selection.Scope != scope)
                                        ||
                                        ((m_client.Selection.Indexes.Count > 0) && (m_client.Selection.Indexes[0] != (number - 1)))
                                       )
                                    {
                                        List<int> indexes = new List<int>() { number - 1 };
                                        m_client.Selection = new Selection(m_client.Book, scope, indexes);

                                        PlayerStopLabel_Click(null, null);

                                        DisplaySelection(true);
                                    }
                                }

                                if ((control == WordNumericUpDown) || (control == ChapterWordNumericUpDown))
                                {
                                    SelectWord(word_number);
                                }
                                else if ((control == LetterNumericUpDown) || (control == ChapterLetterNumericUpDown))
                                {
                                    SelectLetter(letter_number);
                                }
                                else
                                {
                                    // unknown
                                }
                            }
                        }
                    }
                }
                finally
                {
                    ChapterVerseNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    ChapterWordNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    ChapterLetterNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    PageNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    StationNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    PartNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    GroupNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    HalfNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    QuarterNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    BowingNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    VerseNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    WordNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    LetterNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                }
            }
        }
    }
    private void DisplaySelection(bool add_to_history)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            SwitchToMainTextBox();

            //MainTextBox.TextChanged -= new EventHandler(MainTextBox_TextChanged);
            MainTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
            MainTextBox.BeginUpdate();

            BookmarkTextBox.Enabled = true;

            m_selection_mode = true;

            AutoCompleteHeaderLabel.Visible = false;
            AutoCompleteListBox.Visible = false;
            AutoCompleteListBox.SendToBack();

            this.Text = Application.ProductName + " | " + GetSummarizedSearchScope();
            UpdateSearchScope();

            DisplaySelectionText();

            CalculateCurrentValue();

            DisplaySelectionPositions();

            BuildLetterFrequencies();
            DisplayLetterFrequencies();

            MainTextBox.ClearHighlight();
            MainTextBox.AlignToStart();

            m_current_selection_verse_index = 0;
            CurrentVerseIndex = 0;
            UpdateHeaderLabel();

            if (m_client != null)
            {
                if (m_client.Selection != null)
                {
                    DisplayTranslations(m_client.Selection.Verses);
                    if (m_client.Selection.Verses.Count > 0)
                    {
                        UpdatePlayerButtons(m_client.Selection.Verses[0]);
                    }

                    if (add_to_history)
                    {
                        AddSelectionHistoryItem();
                    }

                    // display selection's note (if any)
                    DisplayNote(m_client.GetBookmark(m_client.Selection));
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            MainTextBox.EndUpdate();
            MainTextBox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
            //MainTextBox.TextChanged += new EventHandler(MainTextBox_TextChanged);
            this.Cursor = Cursors.Default;
        }
    }
    private void DisplaySelectionText()
    {
        if (
             (m_text_display_mode == TextDisplayMode.Both) ||
             (m_text_display_mode == TextDisplayMode.QuranOnly)
           )
        {
            if (m_client != null)
            {
                if (m_client.Selection != null)
                {
                    List<Verse> verses = m_client.Selection.Verses;
                    if (verses != null)
                    {
                        if (verses.Count > 0)
                        {
                            StringBuilder str = new StringBuilder();
                            foreach (Verse verse in verses)
                            {
                                str.Append(verse.Text + verse.Endmark);
                            }
                            if (str.Length > 1)
                            {
                                str.Remove(str.Length - 1, 1); // last space in " {###} "   OR  \n
                            }
                            m_current_text = str.ToString();
                        }
                    }

                    MainTextBox.Text = m_current_text;

                    ColorizeGoldenRatios();
                }
            }
        }
    }
    private void UpdateLanguageType(string text)
    {
        if (text.IsArabic())
        {
            SetLanguageType(LanguageType.RightToLeft);
        }
        else
        {
            SetLanguageType(LanguageType.LeftToRight);
        }
        EnableFindByTextControls();
    }
    private void DisplaySelectionPositions()
    {
        if (m_client != null)
        {
            if (m_client.Selection != null)
            {
                List<Verse> verses = m_client.Selection.Verses;
                if (verses != null)
                {
                    if (verses.Count > 0)
                    {
                        Verse verse = verses[0];
                        if (verse != null)
                        {
                            // show postion of selection in the Quran visually
                            UpdateProgressBar(verse);

                            if (verse.Chapter != null)
                            {
                                UpdateMinMaxChapterVerseWordLetter(verse.Chapter.Number - 1);
                            }

                            if (ChapterComboBox.Items.Count > 0)
                            {
                                // without this guard, we cannot select more than 1 chapter in ChaptersListBox and
                                // we cannot move backward/forward inside the ChaptersListBox using Backspace
                                if (!ChaptersListBox.Focused)
                                {
                                    UpdateChaptersListBox();
                                }
                            }
                            UpdateVersePositions(verse);

                            Bookmark bookmark = m_client.GotoBookmark(m_client.Selection.Scope, m_client.Selection.Indexes);
                            if (bookmark != null)
                            {
                                BookmarkTextBox.ForeColor = m_note_view_color;
                                BookmarkTextBox.Text = bookmark.Note;
                                string hint = "Creation Time" + "\t" + bookmark.CreatedTime + "\r\n"
                                            + "Last Modified" + "\t" + bookmark.LastModifiedTime;
                                ToolTip.SetToolTip(BookmarkTextBox, hint);
                                UpdateBookmarkButtons();
                            }
                            else
                            {
                                DisplayNoteWritingInstruction();
                            }
                        }
                    }
                }
            }
        }
    }
    private Chapter m_old_chapter = null;
    private Verse m_old_verse = null;
    private void DisplayCurrentPositions()
    {
        if (m_active_textbox != null)
        {
            if (m_active_textbox.Lines != null)
            {
                if (m_active_textbox.Lines.Length > 0)
                {
                    Verse verse = GetVerse(CurrentVerseIndex);
                    if (verse != null)
                    {
                        if (m_old_verse != verse)
                        {
                            m_old_verse = verse;

                            // show postion of verse in the Quran visually
                            ProgressBar.Minimum = 1;
                            ProgressBar.Maximum = verse.Book.Verses.Count;
                            ProgressBar.Value = verse.Number;
                            ProgressBar.Refresh();

                            if (verse.Chapter != null)
                            {
                                if (m_old_chapter != verse.Chapter)
                                {
                                    m_old_chapter = verse.Chapter;
                                    UpdateMinMaxChapterVerseWordLetter(verse.Chapter.Number - 1);
                                }
                            }

                            if (ChapterComboBox.Items.Count > 0)
                            {
                                // without this guard, we cannot select more than 1 chapter in ChaptersListBox and
                                // we cannot move backward/forward inside the ChaptersListBox using Backspace
                                if (!ChaptersListBox.Focused)
                                {
                                    UpdateChaptersListBox();
                                }
                            }
                        }
                        UpdateVersePositions(verse);
                    }
                }
            }
        }
    }
    private void UpdateVersePositions(Verse verse)
    {
        if (m_active_textbox != null)
        {
            if (verse != null)
            {
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        ChapterComboBox.SelectedIndexChanged -= new EventHandler(ChapterComboBox_SelectedIndexChanged);
                        ChapterVerseNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        ChapterWordNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        ChapterLetterNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        PageNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        StationNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        PartNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        GroupNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        HalfNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        QuarterNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        BowingNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        VerseNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        WordNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        LetterNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    }

                    if (verse.Chapter != null)
                    {
                        if (ChapterComboBox.SelectedIndex != verse.Chapter.Number - 1)
                        {
                            ChapterComboBox.SelectedIndex = verse.Chapter.Number - 1;
                            DisplayChapterRevelationInfo();
                        }
                    }

                    if ((verse.NumberInChapter >= 1) && (verse.NumberInChapter <= verse.Chapter.Verses.Count))
                    {
                        if (verse.Chapter != null)
                        {
                            if (ChapterVerseNumericUpDown.Value != verse.NumberInChapter)
                            {
                                ChapterVerseNumericUpDown.Value = (verse.NumberInChapter > ChapterVerseNumericUpDown.Maximum) ? ChapterVerseNumericUpDown.Maximum : verse.NumberInChapter;
                            }
                        }
                    }

                    if (verse.Page != null)
                    {
                        if (PageNumericUpDown.Value != verse.Page.Number)
                        {
                            PageNumericUpDown.Value = verse.Page.Number;
                        }
                    }
                    if (verse.Station != null)
                    {
                        if (StationNumericUpDown.Value != verse.Station.Number)
                        {
                            StationNumericUpDown.Value = verse.Station.Number;
                        }
                    }
                    if (verse.Part != null)
                    {
                        if (PartNumericUpDown.Value != verse.Part.Number)
                        {
                            PartNumericUpDown.Value = verse.Part.Number;
                        }
                    }
                    if (verse.Group != null)
                    {
                        if (GroupNumericUpDown.Value != verse.Group.Number)
                        {
                            GroupNumericUpDown.Value = verse.Group.Number;
                        }
                    }
                    if (verse.Half != null)
                    {
                        if (HalfNumericUpDown.Value != verse.Half.Number)
                        {
                            HalfNumericUpDown.Value = verse.Half.Number;
                        }
                    }
                    if (verse.Quarter != null)
                    {
                        if (QuarterNumericUpDown.Value != verse.Quarter.Number)
                        {
                            QuarterNumericUpDown.Value = verse.Quarter.Number;
                        }
                    }
                    if (verse.Bowing != null)
                    {
                        if (BowingNumericUpDown.Value != verse.Bowing.Number)
                        {
                            BowingNumericUpDown.Value = verse.Bowing.Number;
                        }
                    }
                    if (VerseNumericUpDown.Value != verse.Number)
                    {
                        VerseNumericUpDown.Value = verse.Number;
                    }

                    int char_index = m_active_textbox.SelectionStart;
                    int line_index = m_active_textbox.GetLineFromCharIndex(char_index);

                    Word word = GetWordAtChar(char_index);
                    if (word != null)
                    {
                        int word_index_in_verse = word.NumberInVerse - 1;
                        Letter letter = GetLetterAtChar(char_index);
                        if (letter == null) letter = GetLetterAtChar(char_index - 1); // (Ctrl_End)
                        if (letter != null)
                        {
                            int letter_index_in_verse = letter.NumberInVerse - 1;
                            int word_number = verse.Words[0].Number + word_index_in_verse;
                            if (word_number > WordNumericUpDown.Maximum)
                            {
                                WordNumericUpDown.Value = WordNumericUpDown.Maximum;
                            }
                            else if (word_number < WordNumericUpDown.Minimum)
                            {
                                WordNumericUpDown.Value = WordNumericUpDown.Minimum;
                            }
                            else
                            {
                                if (WordNumericUpDown.Value != word_number)
                                {
                                    WordNumericUpDown.Value = word_number;
                                }
                            }

                            if (verse.Words.Count > 0)
                            {
                                if (verse.Words[0].Letters.Count > 0)
                                {
                                    int letter_number = verse.Words[0].Letters[0].Number + letter_index_in_verse;
                                    if (letter_number > LetterNumericUpDown.Maximum)
                                    {
                                        LetterNumericUpDown.Value = LetterNumericUpDown.Maximum;
                                    }
                                    else if (letter_number < LetterNumericUpDown.Minimum)
                                    {
                                        LetterNumericUpDown.Value = LetterNumericUpDown.Minimum;
                                    }
                                    else
                                    {
                                        if (LetterNumericUpDown.Value != letter_number)
                                        {
                                            LetterNumericUpDown.Value = letter_number;
                                        }
                                    }
                                }
                            }

                            m_word_number_in_verse = word_index_in_verse + 1;
                            m_letter_number_in_verse = letter_index_in_verse + 1;
                            int word_count = 0;
                            int letter_count = 0;
                            if (verse.Chapter != null)
                            {
                                foreach (Verse chapter_verse in verse.Chapter.Verses)
                                {
                                    if (chapter_verse.NumberInChapter < verse.NumberInChapter)
                                    {
                                        word_count += chapter_verse.Words.Count;
                                        letter_count += chapter_verse.LetterCount;
                                    }
                                }
                            }
                            m_word_number_in_chapter = word_count + m_word_number_in_verse;
                            m_letter_number_in_chapter = letter_count + m_letter_number_in_verse;

                            if (m_word_number_in_chapter > ChapterWordNumericUpDown.Maximum)
                            {
                                ChapterWordNumericUpDown.Value = ChapterWordNumericUpDown.Maximum;
                            }
                            else if (m_word_number_in_chapter < ChapterWordNumericUpDown.Minimum)
                            {
                                ChapterWordNumericUpDown.Value = ChapterWordNumericUpDown.Minimum;
                            }
                            else
                            {
                                if (ChapterWordNumericUpDown.Value != m_word_number_in_chapter)
                                {
                                    ChapterWordNumericUpDown.Value = m_word_number_in_chapter;
                                }
                            }

                            if (m_letter_number_in_chapter > ChapterLetterNumericUpDown.Maximum)
                            {
                                ChapterLetterNumericUpDown.Value = ChapterLetterNumericUpDown.Maximum;
                            }
                            else if (m_letter_number_in_chapter < ChapterLetterNumericUpDown.Minimum)
                            {
                                ChapterLetterNumericUpDown.Value = ChapterLetterNumericUpDown.Minimum;
                            }
                            else
                            {
                                if (ChapterLetterNumericUpDown.Value != m_letter_number_in_chapter)
                                {
                                    ChapterLetterNumericUpDown.Value = m_letter_number_in_chapter;
                                }
                            }

                            ColorizePositionNumbers();
                            ColorizePositionControls();

                            UpdatePlayerButtons(verse);
                        }
                    }
                }
                catch
                {
                    // ignore poosible error due to non-Arabic search result
                    // showing verses with more words than the words in the Arabic verse
                    // and throwing exception when assigned to WordNumericUpDown.Value or LetterNumericUpDown.Value
                }
                finally
                {
                    ChapterComboBox.SelectedIndexChanged += new EventHandler(ChapterComboBox_SelectedIndexChanged);
                    ChapterVerseNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    ChapterWordNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    ChapterLetterNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    PageNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    StationNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    PartNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    GroupNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    HalfNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    QuarterNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    BowingNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    VerseNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    WordNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    LetterNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                }
            }
        }
    }
    private void ColorizePositionNumbers()
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                if ((ChapterComboBox.SelectedIndex >= 0) && (ChapterComboBox.SelectedIndex < m_client.Book.Chapters.Count))
                {
                    int chapter_number = m_client.Book.Chapters[ChapterComboBox.SelectedIndex].Number;
                    ChapterComboBox.ForeColor = GetNumberTypeColor(chapter_number);
                }
            }
        }

        ChapterVerseNumericUpDown.ForeColor = GetNumberTypeColor((int)ChapterVerseNumericUpDown.Value);
        ChapterWordNumericUpDown.ForeColor = GetNumberTypeColor((int)ChapterWordNumericUpDown.Value);
        ChapterLetterNumericUpDown.ForeColor = GetNumberTypeColor((int)ChapterLetterNumericUpDown.Value);
        PageNumericUpDown.ForeColor = GetNumberTypeColor((int)PageNumericUpDown.Value);
        StationNumericUpDown.ForeColor = GetNumberTypeColor((int)StationNumericUpDown.Value);
        PartNumericUpDown.ForeColor = GetNumberTypeColor((int)PartNumericUpDown.Value);
        GroupNumericUpDown.ForeColor = GetNumberTypeColor((int)GroupNumericUpDown.Value);
        HalfNumericUpDown.ForeColor = GetNumberTypeColor((int)QuarterNumericUpDown.Value);
        QuarterNumericUpDown.ForeColor = GetNumberTypeColor((int)QuarterNumericUpDown.Value);
        BowingNumericUpDown.ForeColor = GetNumberTypeColor((int)BowingNumericUpDown.Value);
        VerseNumericUpDown.ForeColor = GetNumberTypeColor((int)VerseNumericUpDown.Value);
        WordNumericUpDown.ForeColor = GetNumberTypeColor((int)WordNumericUpDown.Value);
        LetterNumericUpDown.ForeColor = GetNumberTypeColor((int)LetterNumericUpDown.Value);

        ChapterComboBox.Refresh();
        ChapterVerseNumericUpDown.Refresh();
        ChapterWordNumericUpDown.Refresh();
        ChapterLetterNumericUpDown.Refresh();
        PageNumericUpDown.Refresh();
        StationNumericUpDown.Refresh();
        PartNumericUpDown.Refresh();
        GroupNumericUpDown.Refresh();
        HalfNumericUpDown.Refresh();
        QuarterNumericUpDown.Refresh();
        BowingNumericUpDown.Refresh();
        VerseNumericUpDown.Refresh();
        WordNumericUpDown.Refresh();
        LetterNumericUpDown.Refresh();
    }
    private void ColorizePositionControls()
    {
        if (m_client != null)
        {
            // Clear BackColors
            ChapterComboBox.BackColor = SystemColors.Window;
            ChapterVerseNumericUpDown.BackColor = SystemColors.Window;
            ChapterWordNumericUpDown.BackColor = SystemColors.Window;
            ChapterLetterNumericUpDown.BackColor = SystemColors.Window;
            PageNumericUpDown.BackColor = SystemColors.Window;
            StationNumericUpDown.BackColor = SystemColors.Window;
            PartNumericUpDown.BackColor = SystemColors.Window;
            GroupNumericUpDown.BackColor = SystemColors.Window;
            HalfNumericUpDown.BackColor = SystemColors.Window;
            QuarterNumericUpDown.BackColor = SystemColors.Window;
            BowingNumericUpDown.BackColor = SystemColors.Window;
            VerseNumericUpDown.BackColor = SystemColors.Window;
            WordNumericUpDown.BackColor = SystemColors.Window;
            LetterNumericUpDown.BackColor = SystemColors.Window;

            if (m_client.Selection != null)
            {
                switch (m_client.Selection.Scope)
                {
                    case SelectionScope.Book:
                        {
                        }
                        break;
                    case SelectionScope.Station:
                        {
                            StationNumericUpDown.BackColor = Color.FromArgb(255, 255, 192);
                        }
                        break;
                    case SelectionScope.Part:
                        {
                            PartNumericUpDown.BackColor = Color.FromArgb(255, 255, 192);
                        }
                        break;
                    case SelectionScope.Group:
                        {
                            GroupNumericUpDown.BackColor = Color.FromArgb(255, 255, 192);
                        }
                        break;
                    case SelectionScope.Half:
                        {
                            HalfNumericUpDown.BackColor = Color.FromArgb(255, 255, 192);
                        }
                        break;
                    case SelectionScope.Quarter:
                        {
                            QuarterNumericUpDown.BackColor = Color.FromArgb(255, 255, 192);
                        }
                        break;
                    case SelectionScope.Bowing:
                        {
                            BowingNumericUpDown.BackColor = Color.FromArgb(255, 255, 192);
                        }
                        break;
                    case SelectionScope.Chapter:
                        {
                            ChapterComboBox.BackColor = Color.FromArgb(255, 255, 192);
                        }
                        break;
                    case SelectionScope.Verse:
                        {
                            ChapterVerseNumericUpDown.BackColor = Color.FromArgb(255, 255, 192);
                            VerseNumericUpDown.BackColor = Color.FromArgb(255, 255, 192);
                        }
                        break;
                    case SelectionScope.Word:
                        {
                            WordNumericUpDown.BackColor = Color.FromArgb(255, 255, 192);
                            ChapterWordNumericUpDown.BackColor = Color.FromArgb(255, 255, 192);
                        }
                        break;
                    case SelectionScope.Letter:
                        {
                            LetterNumericUpDown.BackColor = Color.FromArgb(255, 255, 192);
                            ChapterLetterNumericUpDown.BackColor = Color.FromArgb(255, 255, 192);
                        }
                        break;
                    case SelectionScope.Page:
                        {
                            PageNumericUpDown.BackColor = Color.FromArgb(255, 255, 192);
                        }
                        break;
                    default: // Unknown
                        {
                            MessageBox.Show("Unknown selection scope.", Application.ProductName);
                        }
                        break;
                }
            }
        }
    }
    private void UpdateProgressBar(Verse verse)
    {
        if (m_client != null)
        {
            if (m_client.Selection != null)
            {
                switch (m_client.Selection.Scope)
                {
                    case SelectionScope.Book:
                        {
                            ProgressBar.Minimum = 1;
                            ProgressBar.Maximum = 1;
                            ProgressBar.Value = 1;
                        }
                        break;
                    case SelectionScope.Station:
                        {
                            ProgressBar.Minimum = 1;
                            ProgressBar.Maximum = verse.Book.Stations.Count;
                            ProgressBar.Value = verse.Station.Number;
                        }
                        break;
                    case SelectionScope.Part:
                        {
                            ProgressBar.Minimum = 1;
                            ProgressBar.Maximum = verse.Book.Parts.Count;
                            ProgressBar.Value = verse.Part.Number;
                        }
                        break;
                    case SelectionScope.Group:
                        {
                            ProgressBar.Minimum = 1;
                            ProgressBar.Maximum = verse.Book.Groups.Count;
                            ProgressBar.Value = verse.Group.Number;
                        }
                        break;
                    case SelectionScope.Half:
                        {
                            ProgressBar.Minimum = 1;
                            ProgressBar.Maximum = verse.Book.Halfs.Count;
                            ProgressBar.Value = verse.Half.Number;
                        }
                        break;
                    case SelectionScope.Quarter:
                        {
                            ProgressBar.Minimum = 1;
                            ProgressBar.Maximum = verse.Book.Quarters.Count;
                            ProgressBar.Value = verse.Quarter.Number;
                        }
                        break;
                    case SelectionScope.Bowing:
                        {
                            ProgressBar.Minimum = 1;
                            ProgressBar.Maximum = verse.Book.Bowings.Count;
                            ProgressBar.Value = verse.Bowing.Number;
                        }
                        break;
                    case SelectionScope.Chapter:
                        {
                            ProgressBar.Minimum = 1;
                            ProgressBar.Maximum = verse.Book.Chapters.Count;
                            ProgressBar.Value = verse.Chapter.Number;
                        }
                        break;
                    case SelectionScope.Verse:
                    case SelectionScope.Word:
                    case SelectionScope.Letter:
                        {
                            ProgressBar.Minimum = 1;
                            ProgressBar.Maximum = verse.Book.Verses.Count;
                            ProgressBar.Value = verse.Number;
                        }
                        break;
                    case SelectionScope.Page:
                        {
                            ProgressBar.Minimum = 1;
                            ProgressBar.Maximum = verse.Book.Pages.Count;
                            ProgressBar.Value = verse.Page.Number;
                        }
                        break;
                }
                ProgressBar.Refresh();
            }
        }
    }
    private void SelectWord(int word_number)
    {
        if (m_active_textbox != null)
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    Verse verse = m_client.Book.GetVerseByWordNumber(word_number);
                    if (verse != null)
                    {
                        word_number -= verse.Words[0].Number;
                        if ((word_number >= 0) && (word_number < verse.Words.Count))
                        {
                            Word word = verse.Words[word_number];
                            m_active_textbox.Select(word.Position, word.Text.Length);
                        }
                    }
                }
            }
        }
    }
    private void SelectLetter(int letter_number)
    {
        if (m_active_textbox != null)
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    Word word = m_client.Book.GetWordByLetterNumber(letter_number);
                    if (word != null)
                    {
                        letter_number -= word.Letters[0].Number;
                        if ((letter_number >= 0) && (letter_number < word.Letters.Count))
                        {
                            int letter_position = word.Position + letter_number;
                            int letter_length = 1;
                            m_active_textbox.Select(letter_position, letter_length);
                        }
                    }
                }
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 10. Bookmarks/Notes
    ///////////////////////////////////////////////////////////////////////////////
    private string m_note_writing_instruction = "write a note";
    private Color m_note_writing_instruction_color = Color.Gray;
    private Color m_note_edit_color = Color.Black;
    private Color m_note_view_color = Color.Blue;
    private void BookmarkTextBox_Enter(object sender, EventArgs e)
    {
        SearchGroupBox_Leave(null, null);

        BookmarkTextBox.ForeColor = m_note_edit_color;
        if (!String.IsNullOrEmpty(BookmarkTextBox.Text))
        {
            if (BookmarkTextBox.Text.StartsWith(m_note_writing_instruction))
            {
                BookmarkTextBox.Text = null;
            }
        }
    }
    private void BookmarkTextBox_Leave(object sender, EventArgs e)
    {
        AddBookmarkButton_Click(null, null);
        this.AcceptButton = null;
    }
    private void BookmarkTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            if (!String.IsNullOrEmpty(BookmarkTextBox.Text))
            {
                if (BookmarkTextBox.Text.Length > 0)
                {
                    AddBookmarkButton_Click(null, null);
                }
                else
                {
                    DeleteBookmarkLabel_Click(null, null);
                }
            }
        }
        else
        {
            BookmarkTextBox.ForeColor = m_note_edit_color;
        }
        UpdateBookmarkButtons();
    }
    private void DisplayNoteWritingInstruction()
    {
        DeleteBookmarkLabel.Enabled = false;
        ClearBookmarksLabel.Enabled = false;

        BookmarkTextBox.ForeColor = m_note_writing_instruction_color;
        if (BookmarkTextBox.Focused)
        {
            BookmarkTextBox.Text = null;
        }
        else
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    if (m_client.Selection != null)
                    {
                        if (m_client.Selection.Scope == SelectionScope.Book)
                        {
                            BookmarkTextBox.Text = m_note_writing_instruction + " for "
                                + m_client.Selection.Scope.ToString();
                        }
                        else if ((m_client.Selection.Scope == SelectionScope.Verse) || (m_client.Selection.Scope == SelectionScope.Word) || (m_client.Selection.Scope == SelectionScope.Letter))
                        {
                            BookmarkTextBox.Text = m_note_writing_instruction + " for Chapter "
                                + (ChapterComboBox.SelectedIndex + 1).ToString() + " Verse "
                                + (ChapterVerseNumericUpDown.Value).ToString();
                        }
                        else
                        {
                            StringBuilder str = new StringBuilder();
                            if (m_client.Selection.Indexes.Count > 0)
                            {
                                foreach (int index in m_client.Selection.Indexes)
                                {
                                    str.Append((index + 1).ToString() + "+");
                                }
                                if (str.Length > 1)
                                {
                                    str.Remove(str.Length - 1, 1);
                                }
                            }

                            BookmarkTextBox.Text = m_note_writing_instruction + " for "
                                         + m_client.Selection.Scope.ToString() + " "
                                         + str.ToString();
                        }
                    }
                }
            }
        }

        BookmarkTextBox.Refresh();
        ToolTip.SetToolTip(BookmarkTextBox, null);

        UpdateBookmarkButtons();
    }
    private void DisplayNote(Bookmark bookmark)
    {
        if (bookmark != null)
        {
            if (bookmark.Selection != null)
            {
                BookmarkTextBox.Text = bookmark.Note;
                BookmarkTextBox.ForeColor = m_note_view_color;

                string hint = "Creation Time" + "\t" + bookmark.CreatedTime + "\r\n"
                     + "Last Modified" + "\t" + bookmark.LastModifiedTime;
                ToolTip.SetToolTip(BookmarkTextBox, hint);
            }
        }
        else
        {
            DisplayNoteWritingInstruction();
        }
    }
    private void DisplayBookmark(Bookmark bookmark)
    {
        if (bookmark != null)
        {
            if (bookmark.Selection != null)
            {
                if (m_client != null)
                {
                    m_client.Selection = new Selection(m_client.Book, bookmark.Selection.Scope, bookmark.Selection.Indexes);
                    if (m_client.Selection != null)
                    {
                        PlayerStopLabel_Click(null, null);

                        DisplaySelection(false);

                        BookmarkTextBox.Text = bookmark.Note;
                        BookmarkTextBox.ForeColor = m_note_view_color;
                        string hint = "Creation Time" + "\t" + bookmark.CreatedTime + "\r\n"
                             + "Last Modified" + "\t" + bookmark.LastModifiedTime;
                        ToolTip.SetToolTip(BookmarkTextBox, hint);
                        MainTextBox.Focus();

                        UpdateBookmarkButtons();
                    }
                }
            }
        }
    }
    private void AddBookmarkButton_Click(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(BookmarkTextBox.Text))
        {
            if (BookmarkTextBox.Text.StartsWith(m_note_writing_instruction))
            {
                // ignore it
            }
            else if (BookmarkTextBox.Text.Length == 0)
            {
                DeleteBookmarkLabel_Click(null, null);
            }
            else //if (!BookmarkTextBox.Text.StartsWith(m_note_writing_instruction))
            {
                if (m_client != null)
                {
                    if (m_client.Book != null)
                    {
                        if (m_client.Selection != null)
                        {
                            Selection selection = new Selection(m_client.Book, m_client.Selection.Scope, m_client.Selection.Indexes);
                            Bookmark bookmark = m_client.CreateBookmark(selection, BookmarkTextBox.Text);

                            BookmarkTextBox.ForeColor = m_note_view_color;
                            UpdateBookmarkButtons();
                        }
                    }
                }
            }
        }
    }
    private void PreviousBookmarkButton_Click(object sender, EventArgs e)
    {
        if (m_client != null)
        {
            Bookmark bookmark = m_client.GotoPreviousBookmark();
            if (bookmark != null)
            {
                DisplayBookmark(bookmark);
            }
        }
    }
    private void NextBookmarkButton_Click(object sender, EventArgs e)
    {
        if (m_client != null)
        {
            Bookmark bookmark = m_client.GotoNextBookmark();
            if (bookmark != null)
            {
                DisplayBookmark(bookmark);
            }
        }
    }
    private void BookmarkCounterLabel_Click(object sender, EventArgs e)
    {
        if (m_client != null)
        {
            if (m_client.Bookmarks != null)
            {
                if (m_client.Bookmarks.Count > 0)
                {
                    DisplayBookmark(m_client.CurrentBookmark);

                    // call again so the chapter is selected in ChapterListBox
                    DisplayBookmark(m_client.CurrentBookmark);
                }
            }
        }
    }
    private void DeleteBookmarkLabel_Click(object sender, EventArgs e)
    {
        if (m_client != null)
        {
            PlayerStopLabel_Click(null, null);

            // remove existing bookmark (if any)
            m_client.DeleteCurrentBookmark();

            Bookmark bookmark = m_client.CurrentBookmark;
            if (bookmark != null)
            {
                DisplayBookmark(bookmark);
            }
            else
            {
                DisplaySelection(false);
            }
        }
    }
    private void ClearBookmarksLabel_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(
            "Delete all bookmarks and notes?",
            Application.ProductName,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question) == DialogResult.Yes)
        {
            if (m_client != null)
            {
                m_client.ClearBookmarks();

                PlayerStopLabel_Click(null, null);

                DisplaySelection(false);
            }
        }
    }
    private void UpdateBookmarkButtons()
    {
        if (m_client != null)
        {
            if (m_client.Bookmarks != null)
            {
                PreviousBookmarkButton.Enabled = (m_client.Bookmarks.Count > 0) && (m_client.CurrentBookmarkIndex > 0);
                NextBookmarkButton.Enabled = (m_client.Bookmarks.Count > 0) && (m_client.CurrentBookmarkIndex < m_client.Bookmarks.Count - 1);
                BookmarkCounterLabel.Text = (m_client.CurrentBookmarkIndex + 1).ToString() + " / " + m_client.Bookmarks.Count.ToString();
                DeleteBookmarkLabel.Enabled = (!BookmarkTextBox.Text.StartsWith(m_note_writing_instruction)) && (!m_found_verses_displayed) && (m_client.Bookmarks.Count > 0);
                ClearBookmarksLabel.Enabled = (!BookmarkTextBox.Text.StartsWith(m_note_writing_instruction)) && (!m_found_verses_displayed) && (m_client.Bookmarks.Count > 0);
                ClearBookmarksLabel.BackColor = (!BookmarkTextBox.Text.StartsWith(m_note_writing_instruction)) && (!m_found_verses_displayed) && (m_client.Bookmarks.Count > 0) ? Color.LightCoral : SystemColors.ControlLight;
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 11. Audio Player
    ///////////////////////////////////////////////////////////////////////////////
    private MP3Player m_player = new MP3Player();
    private int m_audio_volume = DEFAULT_AUDIO_VOLUME;
    private string m_downloaded_audio_filename = null;
    private bool m_first_play = true;
    private float m_silence_between_verses = 0.0F; // in verse lengths
    private int m_silence_between_selections = 0;  // in milliseconds
    private int m_silence_time_between_verses = 0;
    private int m_silence_time_between_selections = 0;
    private bool m_break_playing = false;
    private bool m_player_looping = false;
    private bool m_player_looping_all = false;
    private int m_player_looping_count = int.MaxValue;        // unlimited verse repetitions
    private int m_player_looping_all_count = int.MaxValue;    // unlimited selection repetitions
    private int m_player_looping_i = 0;
    private int m_player_looping_all_i = 0;
    private void PlayVerse(int index)
    {
        Verse verse = GetVerse(index);
        if (verse != null)
        {
            HighlightVerse(verse);
            PlayVerse(verse);
        }
    }
    private void PlayVerse(Verse verse)
    {
        if (m_active_textbox != null)
        {
            if (verse != null)
            {
                if (m_player != null)
                {
                    // open verse mp3 file
                    if (m_player.Closed)
                    {
                        PlayerOpenAudioFile(verse);
                    }

                    if (m_player.Opened)
                    {
                        if (m_player.MuteAll)
                        {
                            m_player.VolumeAll = 0;
                        }
                        else
                        {
                            m_player.VolumeAll = m_audio_volume;
                        }

                        // play verse
                        m_player.Play();

                        // wait till finish
                        WaitForPlayToFinish();

                        // get verse time length before stop
                        int verse_time_length = (int)m_player.Length;

                        // stop verse
                        if (m_player.Opened)
                        {
                            m_player.Stop();
                            m_player.Close();
                        }

                        // and go into silence if needed
                        m_silence_time_between_verses = (int)(verse_time_length * m_silence_between_verses);
                        if (m_silence_time_between_verses > 0)
                        {
                            WaitFor(m_silence_time_between_verses);
                        }
                        else
                        {
                            // go into silence if last verse in chapter (on last repeat only)
                            if (verse.NumberInChapter == verse.Chapter.Verses.Count)
                            {
                                if (m_player_looping)
                                {
                                    if (m_player_looping_i == m_player_looping_count - 1)
                                    {
                                        WaitFor(3000);
                                    }
                                }
                                else
                                {
                                    WaitFor(3000);
                                }
                            }
                        }

                        // sleep in case of Prostration (Sijood)
                        switch (verse.ProstrationType)
                        {
                            case ProstrationType.None:
                                {
                                    // do nothing
                                }
                                break;
                            case ProstrationType.Obligatory:
                                {
                                    WaitFor(10000);
                                }
                                break;
                            case ProstrationType.Recommended:
                                {
                                    WaitFor(5000);
                                }
                                break;
                        }
                    }
                }

                // simulate mouse click to continue playing next verse and not restart from 1
                m_active_textbox.Focus();
                m_selection_mode = false;
            }
            else // invalid verse
            {
                PlayerStopLabel_Click(null, null);
                MessageBox.Show("No verse available.", Application.ProductName);
            }
        }
    }
    private void WaitForPlayToFinish()
    {
        if (m_player != null)
        {
            while ((m_player.Length - m_player.Position) > 50UL)
            {
                // stop immediately if requested
                if (m_break_playing)
                {
                    break;
                }

                Application.DoEvents();
                Thread.Sleep(50);
            }
        }
    }
    private void WaitFor(int milliseconds)
    {
        DateTime end = DateTime.Now.AddMilliseconds(milliseconds);
        while (DateTime.Now < end)
        {
            Application.DoEvents();
            Thread.Sleep(50);
        }
    }
    private void PlayerPlayAudhuBillah()
    {
        if (m_player != null)
        {
            m_player.Open(Globals.AUDIO_FOLDER + "/" + "audhubillah.mp3");
            if (m_player.Opened)
            {
                if (File.Exists(Globals.IMAGES_FOLDER + "/" + "player_pause.png"))
                {
                    PlayerPlayLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "player_pause.png");
                }
                ToolTip.SetToolTip(PlayerPlayLabel, "Pause");
                PlayerPlayLabel.Refresh();

                m_player.VolumeAll = m_audio_volume;

                m_player.Play();
                WaitForPlayToFinish();
            }
            else
            {
                PlayerStopLabel_Click(null, null);
                AskUserToDownloadAudioFilesManually();
            }
        }
    }
    private void PlayerPlayBismAllah()
    {
        if (m_player != null)
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    if (m_client.Book.Chapters[0] != null)
                    {
                        if (m_client.Book.Chapters[0].Verses.Count > 0)
                        {
                            try
                            {
                                // download file if not on disk
                                DownloadVerseAudioFile(m_client.Book.Chapters[0].Verses[0]);

                                // open only, don't play
                                m_player.Open(m_downloaded_audio_filename);

                                if (m_player.Opened)
                                {
                                    if (File.Exists(Globals.IMAGES_FOLDER + "/" + "player_pause.png"))
                                    {
                                        PlayerPlayLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "player_pause.png");
                                    }
                                    ToolTip.SetToolTip(PlayerPlayLabel, "Pause");
                                    PlayerPlayLabel.Refresh();

                                    m_player.VolumeAll = m_audio_volume;

                                    m_player.Play();
                                    WaitForPlayToFinish();
                                }
                            }
                            catch
                            {
                                PlayerStopLabel_Click(null, null);
                                AskUserToDownloadAudioFilesManually();
                            }
                        }
                    }
                }
            }
        }
    }
    private void PlayerOpenAudioFile(Verse verse)
    {
        if (verse != null)
        {
            if (m_player != null)
            {
                // on first play
                if (m_first_play)
                {
                    // play AudhuBillah always at start
                    PlayerPlayAudhuBillah();

                    // play BismAllah for any Quran verse except 1:1 and chapter 9
                    if (verse.Chapter != null)
                    {
                        if ((verse.Chapter.Number != 1) && (verse.Chapter.Number != 9))
                        {
                            PlayerPlayBismAllah();
                        }
                        else // either chapter 1 or 9
                        {
                            if (verse.Chapter.Number == 1)
                            {
                                if (verse.NumberInChapter > 1) // not 1:1
                                {
                                    PlayerPlayBismAllah();
                                }
                            }
                            else if (verse.Chapter.Number == 9)
                            {
                                // do nothing
                            }
                        }
                    }

                    m_first_play = false;
                }
                else // on subsequent play
                {
                    if (verse.Chapter != null)
                    {
                        // play BismAllah before verse 1 of any chapter except chapter 1 and 9
                        // play AudhuBillah before verse 1 of chapter 9
                        if (verse.NumberInChapter == 1)
                        {
                            if (verse.Chapter != null)
                            {
                                if ((verse.Chapter.Number != 1) && (verse.Chapter.Number != 9))
                                {
                                    PlayerPlayBismAllah();
                                }
                                else if (verse.Chapter.Number == 9)
                                {
                                    PlayerPlayAudhuBillah();
                                }
                            }
                        }
                    }
                }

                // load verse for playing (including verse 1 of chapter 1
                try
                {
                    // download file if not on disk
                    DownloadVerseAudioFile(verse);

                    // open only, don't play
                    m_player.Open(m_downloaded_audio_filename);
                }
                catch
                {
                    PlayerStopLabel_Click(null, null);
                    AskUserToDownloadAudioFilesManually();
                }
            }
        }
        else // invalid verse
        {
            PlayerStopLabel_Click(null, null);
            MessageBox.Show("No verse available.", Application.ProductName);
        }
    }
    private void PlayerPreviousLabel_Click(object sender, EventArgs e)
    {
        try
        {
            ulong position = 0UL;
            if (m_player != null)
            {
                if (m_player.Opened)
                {
                    if ((m_player.Playing) || (m_player.Paused))
                    {
                        position = m_player.Position;
                        m_player.Stop();
                        m_player.Close();
                    }
                }
            }

            m_player_looping_i = 0;
            PlayerRepeatCounterLabel.Text = (m_player_looping_i + 1).ToString() + " / " + ((m_player_looping_count == int.MaxValue) ? "*" : m_player_looping_count.ToString());
            ToolTip.SetToolTip(PlayerRepeatCounterLabel, PlayerRepeatCounterLabel.Text);
            PlayerRepeatCounterLabel.Refresh();

            if (File.Exists(Globals.IMAGES_FOLDER + "/" + "player_pause.png"))
            {
                PlayerPlayLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "player_pause.png");
            }
            ToolTip.SetToolTip(PlayerPlayLabel, "Pause");
            PlayerPlayLabel.Refresh();

            m_break_playing = false;
            if (position < 3000UL)
            {
                CurrentVerseIndex--;
            }
            PlayFromCurrentVerse();
        }
        catch
        {
            PlayerStopLabel_Click(null, null);
        }
    }
    private void PlayerNextLabel_Click(object sender, EventArgs e)
    {
        try
        {
            if (m_player != null)
            {
                if (m_player.Opened)
                {
                    if ((m_player.Playing) || (m_player.Paused))
                    {
                        m_player.Stop();
                        m_player.Close();
                    }
                }
            }

            m_player_looping_i = 0;
            PlayerRepeatCounterLabel.Text = (m_player_looping_i + 1).ToString() + " / " + ((m_player_looping_count == int.MaxValue) ? "*" : m_player_looping_count.ToString());
            ToolTip.SetToolTip(PlayerRepeatCounterLabel, PlayerRepeatCounterLabel.Text);
            PlayerRepeatCounterLabel.Refresh();

            if (File.Exists(Globals.IMAGES_FOLDER + "/" + "player_pause.png"))
            {
                PlayerPlayLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "player_pause.png");
            }
            ToolTip.SetToolTip(PlayerPlayLabel, "Pause");
            PlayerPlayLabel.Refresh();

            m_break_playing = false;
            CurrentVerseIndex++;
            PlayFromCurrentVerse();
        }
        catch
        {
            PlayerStopLabel_Click(null, null);
        }
    }
    private void PlayerPlayLabel_Click(object sender, EventArgs e)
    {
        if (m_player != null)
        {
            //m_break_playing = false;
            PlayerStopLabel.Enabled = true;
            PlayerStopLabel.Refresh();

            if ((m_player.Closed) || (m_player.Stopped) || (m_player.Paused))
            {
                if (File.Exists(Globals.IMAGES_FOLDER + "/" + "player_pause.png"))
                {
                    PlayerPlayLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "player_pause.png");
                }
                ToolTip.SetToolTip(PlayerPlayLabel, "Pause");
                PlayerPlayLabel.Refresh();

                if ((m_player.Closed) || (m_player.Stopped))
                {
                    m_break_playing = false;
                    PlayFromCurrentVerse();
                }
                else if (m_player.Paused)
                {
                    m_player.Play(); // resume
                }
            }
            else if (m_player.Playing)
            {
                if (File.Exists(Globals.IMAGES_FOLDER + "/" + "player_play.png"))
                {
                    PlayerPlayLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "player_play.png");
                }
                ToolTip.SetToolTip(PlayerPlayLabel, "Play");
                PlayerPlayLabel.Refresh();

                m_player.Pause(); // pause play
            }
        }
    }
    private void PlayerStopLabel_Click(object sender, EventArgs e)
    {
        if (m_player != null)
        {
            m_break_playing = true;

            if (m_player.Opened)
            {
                if ((m_player.Playing) || (m_player.Paused))
                {
                    m_player.Stop();
                    m_player.Close();
                }
            }
        }
        if (File.Exists(Globals.IMAGES_FOLDER + "/" + "player_play.png"))
        {
            PlayerPlayLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "player_play.png");
        }
        PlayerPlayLabel.Refresh();

        PlayerStopLabel.Enabled = false;
        PlayerStopLabel.Refresh();

        m_player_looping_i = 0;
        PlayerRepeatCounterLabel.Text = (m_player_looping_i + 1).ToString() + " / " + ((m_player_looping_count == int.MaxValue) ? "*" : m_player_looping_count.ToString());
        ToolTip.SetToolTip(PlayerRepeatCounterLabel, PlayerRepeatCounterLabel.Text);
        PlayerRepeatCounterLabel.Refresh();

        m_player_looping_all_i = 0;
        PlayerRepeatAllCounterLabel.Text = (m_player_looping_all_i + 1).ToString() + " / " + ((m_player_looping_all_count == int.MaxValue) ? "*" : m_player_looping_all_count.ToString());
        ToolTip.SetToolTip(PlayerRepeatAllCounterLabel, PlayerRepeatAllCounterLabel.Text);
        PlayerRepeatAllCounterLabel.Refresh();
    }
    private void PlayerRepeatLabel_Click(object sender, EventArgs e)
    {
        m_player_looping = !m_player_looping;
        if (m_player_looping)
        {
            if (File.Exists(Globals.IMAGES_FOLDER + "/" + "player_repeat_on.png"))
            {
                PlayerRepeatLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "player_repeat_on.png");
            }
            PlayerRepeatCounterLabel.Visible = true;
            PlayerVerseSilenceGapTrackBar.Enabled = true;
        }
        else
        {
            if (File.Exists(Globals.IMAGES_FOLDER + "/" + "player_repeat.png"))
            {
                PlayerRepeatLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "player_repeat.png");
            }
            PlayerRepeatCounterLabel.Visible = false;
            PlayerVerseSilenceGapTrackBar.Enabled = false;
        }
        PlayerRepeatLabel.Refresh();

        PlayerRepeatCounterLabel.Text = (m_player_looping_i + 1).ToString() + " / " + ((m_player_looping_count == int.MaxValue) ? "*" : m_player_looping_count.ToString());
        ToolTip.SetToolTip(PlayerRepeatCounterLabel, PlayerRepeatCounterLabel.Text);
        PlayerRepeatCounterLabel.Refresh();
    }
    private void PlayerRepeatCounterLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Shift)
        {
            switch (m_player_looping_count)
            {
                case int.MaxValue: m_player_looping_count = 7; break;
                case 7: m_player_looping_count = 5; break;
                case 5: m_player_looping_count = 3; break;
                case 3: m_player_looping_count = 2; break;
                case 2: m_player_looping_count = int.MaxValue; break;
            }
        }
        else
        {
            switch (m_player_looping_count)
            {
                case int.MaxValue: m_player_looping_count = 2; break;
                case 2: m_player_looping_count = 3; break;
                case 3: m_player_looping_count = 5; break;
                case 5: m_player_looping_count = 7; break;
                case 7: m_player_looping_count = int.MaxValue; break;
            }
        }

        PlayerRepeatCounterLabel.Text = (m_player_looping_i + 1).ToString() + " / " + ((m_player_looping_count == int.MaxValue) ? "*" : m_player_looping_count.ToString());
        ToolTip.SetToolTip(PlayerRepeatCounterLabel, PlayerRepeatCounterLabel.Text);
        PlayerRepeatCounterLabel.Refresh();
    }
    private void PlayerRepeatAllLabel_Click(object sender, EventArgs e)
    {
        m_player_looping_all = !m_player_looping_all;
        if (m_player_looping_all)
        {
            if (File.Exists(Globals.IMAGES_FOLDER + "/" + "player_repeat_all_on.png"))
            {
                PlayerRepeatAllLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "player_repeat_all_on.png");
            }
            PlayerRepeatAllCounterLabel.Visible = true;
            PlayerSelectionSilenceGapTrackBar.Enabled = true;
        }
        else
        {
            if (File.Exists(Globals.IMAGES_FOLDER + "/" + "player_repeat_all.png"))
            {
                PlayerRepeatAllLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "player_repeat_all.png");
            }
            PlayerRepeatAllCounterLabel.Visible = false;
            PlayerSelectionSilenceGapTrackBar.Enabled = false;
        }
        PlayerRepeatAllLabel.Refresh();

        PlayerRepeatAllCounterLabel.Text = (m_player_looping_all_i + 1).ToString() + " / " + ((m_player_looping_all_count == int.MaxValue) ? "*" : m_player_looping_all_count.ToString());
        ToolTip.SetToolTip(PlayerRepeatAllCounterLabel, PlayerRepeatAllCounterLabel.Text);
        PlayerRepeatAllCounterLabel.Refresh();
    }
    private void PlayerRepeatAllCounterLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Shift)
        {
            switch (m_player_looping_all_count)
            {
                case int.MaxValue: m_player_looping_all_count = 7; break;
                case 7: m_player_looping_all_count = 5; break;
                case 5: m_player_looping_all_count = 3; break;
                case 3: m_player_looping_all_count = 2; break;
                case 2: m_player_looping_all_count = int.MaxValue; break;
            }
        }
        else
        {
            switch (m_player_looping_all_count)
            {
                case int.MaxValue: m_player_looping_all_count = 2; break;
                case 2: m_player_looping_all_count = 3; break;
                case 3: m_player_looping_all_count = 5; break;
                case 5: m_player_looping_all_count = 7; break;
                case 7: m_player_looping_all_count = int.MaxValue; break;
            }
        }

        PlayerRepeatAllCounterLabel.Text = (m_player_looping_all_i + 1).ToString() + " / " + ((m_player_looping_all_count == int.MaxValue) ? "*" : m_player_looping_all_count.ToString());
        ToolTip.SetToolTip(PlayerRepeatAllCounterLabel, PlayerRepeatAllCounterLabel.Text);
        PlayerRepeatAllCounterLabel.Refresh();
    }
    private void PlayerMuteLabel_Click(object sender, EventArgs e)
    {
        if (m_player != null)
        {
            m_player.MuteAll = !m_player.MuteAll;
            if (m_player.MuteAll)
            {
                m_player.VolumeAll = 0;
                if (File.Exists(Globals.IMAGES_FOLDER + "/" + "player_muted.png"))
                {
                    PlayerMuteLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "player_muted.png");
                }
            }
            else
            {
                m_player.VolumeAll = m_audio_volume;
                if (File.Exists(Globals.IMAGES_FOLDER + "/" + "player_vol_hi.png"))
                {
                    PlayerMuteLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "player_vol_hi.png");
                }
            }
            PlayerMuteLabel.Refresh();
        }
    }
    private void PlayerVolumeTrackBar_Scroll(object sender, EventArgs e)
    {
        if (m_player != null)
        {
            m_audio_volume = PlayerVolumeTrackBar.Value * (1000 / PlayerVolumeTrackBar.Maximum);
            m_player.VolumeAll = m_audio_volume;
            ToolTip.SetToolTip(PlayerVolumeTrackBar, "Volume " + (m_audio_volume / (1000 / PlayerVolumeTrackBar.Maximum)).ToString() + "%");
        }
    }
    private void PlayerVerseSilenceGapTrackBar_Scroll(object sender, EventArgs e)
    {
        m_silence_between_verses = (float)PlayerVerseSilenceGapTrackBar.Value / (PlayerVerseSilenceGapTrackBar.Maximum / 2);
        SetToolTipPlayerVerseSilenceGapTrackBar();
    }
    private void SetToolTipPlayerVerseSilenceGapTrackBar()
    {
        if (m_silence_between_verses == 0.0F)
        {
            ToolTip.SetToolTip(PlayerVerseSilenceGapTrackBar, "no silence between verse replays");
        }
        else
        {
            ToolTip.SetToolTip(PlayerVerseSilenceGapTrackBar, m_silence_between_verses.ToString("0.0") + " verses silence between verse replays");
        }
    }
    private void PlayerSelectionSilenceGapTrackBar_Scroll(object sender, EventArgs e)
    {
        m_silence_between_selections = PlayerSelectionSilenceGapTrackBar.Value;
        SetToolTipPlayerSelectionSilenceGapTrackBar();
    }
    private void SetToolTipPlayerSelectionSilenceGapTrackBar()
    {
        switch (m_silence_between_selections)
        {
            case 0:
                m_silence_time_between_selections = 0 * 1000;
                ToolTip.SetToolTip(PlayerSelectionSilenceGapTrackBar, "no silence between selection replays");
                break;
            case 1:
                m_silence_time_between_selections = 10 * 1000;
                ToolTip.SetToolTip(PlayerSelectionSilenceGapTrackBar, "10 seconds silence between selection replays");
                break;
            case 2:
                m_silence_time_between_selections = 60 * 1000;
                ToolTip.SetToolTip(PlayerSelectionSilenceGapTrackBar, "1 minute silence between selection replays");
                break;
            case 3:
                m_silence_time_between_selections = 5 * 60 * 1000;
                ToolTip.SetToolTip(PlayerSelectionSilenceGapTrackBar, "5 minutes silence between selection replays");
                break;
            case 4:
                m_silence_time_between_selections = 15 * 60 * 1000;
                ToolTip.SetToolTip(PlayerSelectionSilenceGapTrackBar, "15 minutes silence between selection replays");
                break;
            case 5:
                m_silence_time_between_selections = 1 * 60 * 60 * 1000;
                ToolTip.SetToolTip(PlayerSelectionSilenceGapTrackBar, "1 hour silence between selection replays");
                break;
            case 6:
                m_silence_time_between_selections = 2 * 60 * 60 * 1000;
                ToolTip.SetToolTip(PlayerSelectionSilenceGapTrackBar, "2 hours silence between selection replays");
                break;
            case 7:
                m_silence_time_between_selections = 6 * 60 * 60 * 1000;
                ToolTip.SetToolTip(PlayerSelectionSilenceGapTrackBar, "6 hours silence between selection replays");
                break;
            case 8:
                m_silence_time_between_selections = 12 * 60 * 60 * 1000;
                ToolTip.SetToolTip(PlayerSelectionSilenceGapTrackBar, "12 hours silence between selection replays");
                break;
            case 9:
                m_silence_time_between_selections = 24 * 60 * 60 * 1000;
                ToolTip.SetToolTip(PlayerSelectionSilenceGapTrackBar, "1 day silence between selection replays");
                break;
            case 10:
                m_silence_time_between_selections = new Random().Next(10, 24 * 60 * 60) * 1000; // 10s to 24hr
                ToolTip.SetToolTip(PlayerSelectionSilenceGapTrackBar, "Random silence between selection replays");
                break;
            default:
                m_silence_time_between_selections = 0 * 1000;
                ToolTip.SetToolTip(PlayerSelectionSilenceGapTrackBar, "no silence between selection replays");
                break;
        }
    }
    private void PlayFromCurrentVerse()
    {
        if (m_player != null)
        {
            if (m_client != null)
            {
                // if silent_mode or previous verses finished, play current verse
                if ((m_player.Length - m_player.Position) < 50UL)
                {
                    List<Verse> verses = null;
                    if (m_found_verses_displayed)
                    {
                        verses = m_client.FoundVerses;
                    }
                    else
                    {
                        if (m_client.Selection != null)
                        {
                            verses = m_client.Selection.Verses;
                        }
                    }

                    if (verses != null)
                    {
                        int start = CurrentVerseIndex;
                        for (int i = start; i < verses.Count; i++)
                        {
                            // stop immediately if requested
                            if (m_break_playing)
                            {
                                break;
                            }

                            // play verse, loop if required
                            if (m_player_looping)
                            {
                                PlayerRepeatCounterLabel.Text = (m_player_looping_i + 1).ToString() + " / " + ((m_player_looping_count == int.MaxValue) ? "*" : m_player_looping_count.ToString());
                                ToolTip.SetToolTip(PlayerRepeatCounterLabel, PlayerRepeatCounterLabel.Text);
                                PlayerRepeatCounterLabel.Refresh();

                                // play verse
                                PlayVerse(i);

                                if (m_player_looping_i < m_player_looping_count - 1) // -1 as already played once
                                {
                                    m_player_looping_i++;
                                    i--; // for's i++ will replay same verse(i)
                                }
                                else
                                {
                                    m_player_looping_i = 0;
                                    PlayerRepeatCounterLabel.Text = (m_player_looping_i + 1).ToString() + " / " + ((m_player_looping_count == int.MaxValue) ? "*" : m_player_looping_count.ToString());
                                    ToolTip.SetToolTip(PlayerRepeatCounterLabel, PlayerRepeatCounterLabel.Text);
                                    PlayerRepeatCounterLabel.Refresh();
                                }
                            }
                            else
                            {
                                PlayerRepeatCounterLabel.Text = (m_player_looping_i + 1).ToString() + " / " + ((m_player_looping_count == int.MaxValue) ? "*" : m_player_looping_count.ToString());
                                ToolTip.SetToolTip(PlayerRepeatCounterLabel, PlayerRepeatCounterLabel.Text);
                                PlayerRepeatCounterLabel.Refresh();

                                if (m_player_looping_i == 0) // play verse once if no m_player_looping
                                {
                                    // play verse
                                    PlayVerse(i);

                                    // if m_player_looping was ADDED while verse was playing
                                    if (m_player_looping) // ***
                                    {
                                        // replay verse
                                        m_player_looping_i++;
                                        i--; // for's i++ will replay same verse(i)
                                    }
                                }
                                else // if m_player_looping was REMOVED while verse was playing
                                {
                                    // reset m_player_looping_i to move to next verse (in the check above ***) 
                                    m_player_looping_i = 0;
                                }
                            }

                            // if finished playing, loop all selection if required
                            if (i == verses.Count - 1)
                            {
                                if (m_player_looping_all)
                                {
                                    m_player_looping_all_i++;
                                    PlayerRepeatAllCounterLabel.Text = (m_player_looping_all_i + 1).ToString() + " / " + ((m_player_looping_all_count == int.MaxValue) ? "*" : m_player_looping_all_count.ToString());
                                    ToolTip.SetToolTip(PlayerRepeatAllCounterLabel, PlayerRepeatAllCounterLabel.Text);
                                    PlayerRepeatAllCounterLabel.Refresh();

                                    // and go into silence if needed
                                    if (m_silence_time_between_selections > 0)
                                    {
                                        WaitFor(m_silence_time_between_selections);
                                    }

                                    if (m_player_looping_all_i < m_player_looping_all_count)
                                    {
                                        i = -1; // for's i++ will reset it to 0
                                    }
                                    else
                                    {
                                        m_player_looping_all_i = 0;

                                        PlayerRepeatAllCounterLabel.Text = (m_player_looping_all_i + 1).ToString() + " / " + ((m_player_looping_all_count == int.MaxValue) ? "*" : m_player_looping_all_count.ToString());
                                        ToolTip.SetToolTip(PlayerRepeatAllCounterLabel, PlayerRepeatAllCounterLabel.Text);
                                        PlayerRepeatAllCounterLabel.Refresh();
                                    }
                                }
                            }
                        }
                    }
                }
                PlayerStopLabel_Click(null, null);
            }
        }
    }
    private void UpdatePlayerButtons(Verse verse)
    {
        if (m_client != null)
        {
            List<Verse> verses = null;
            if (m_found_verses_displayed)
            {
                verses = m_client.FoundVerses;
            }
            else
            {
                if (m_client.Selection != null)
                {
                    verses = m_client.Selection.Verses;
                }
            }
            if (verses != null)
            {
                if (verses.Count > 0)
                {
                    PlayerPreviousLabel.Enabled = (verse.Number != verses[0].Number);
                    PlayerNextLabel.Enabled = (verse.Number != verses[verses.Count - 1].Number);
                }
                else
                {
                    PlayerPreviousLabel.Enabled = false;
                    PlayerNextLabel.Enabled = false;
                }
            }
        }
    }
    #endregion
    #region 12. Audio Downloader
    ///////////////////////////////////////////////////////////////////////////////
    private List<string> m_downloaded_reciter_folders = null;
    private void AskUserToDownloadAudioFilesManually()
    {
        if (MessageBox.Show("Cannot auto-download audio file for this verse.\r\n\r\n" +
                            "You need to download all audio files and unzip them to folder:\r\n" +
                            Application.StartupPath + "\\" + Globals.AUDIO_FOLDER + "\\" + m_reciter + "\\" + "\r\n\r\nDo you want to download them now?",
                            Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            Control control = new Control();
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    if (m_client.Book.RecitationInfos != null)
                    {
                        foreach (string key in m_client.Book.RecitationInfos.Keys)
                        {
                            if (m_client.Book.RecitationInfos[key].Folder == m_reciter)
                            {
                                control.Tag = RecitationInfo.DEFAULT_URL_PREFIX + m_client.Book.RecitationInfos[key].Url;
                                LinkLabel_Click(control, null);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    private void DownloadVerseAudioFile(Verse verse)
    {
        // mirror remote_folder locally
        string audio_folder = Globals.AUDIO_FOLDER + "/" + m_reciter;
        if (!Directory.Exists(audio_folder))
        {
            Directory.CreateDirectory(audio_folder);
        }

        if (Directory.Exists(audio_folder))
        {
            // generate audio_filename from verse address
            string audio_filename = null;
            string full_audio_folder = null;
            if (verse == null)
            {
                audio_filename = "001000" + "." + RecitationInfo.FileType; // audhubillah
                full_audio_folder = audio_folder + "/" + "001";
            }
            else
            {
                if (verse.Chapter != null)
                {
                    audio_filename = verse.Chapter.Number.ToString("000") + verse.NumberInChapter.ToString("000") + "." + RecitationInfo.FileType;
                    full_audio_folder = audio_folder + "/" + verse.Chapter.Number.ToString("000");
                }
            }

            // fill up local_audio_filename to return to caller
            m_downloaded_audio_filename = full_audio_folder + "/" + audio_filename;
            string outer_downloaded_audio_filename = audio_folder + "/" + audio_filename;
            string backup_downloaded_audio_filename = Application.StartupPath[0].ToString() + ":" + "/" + "Quran" + "/" + full_audio_folder + "/" + audio_filename;
            // audio file exists in normal folder
            if (File.Exists(m_downloaded_audio_filename))
            {
                // no need to download
            }
            // audio file exists in outer folder
            else if (File.Exists(outer_downloaded_audio_filename))
            {
                if (!Directory.Exists(full_audio_folder))
                {
                    Directory.CreateDirectory(full_audio_folder);
                }

                if (Directory.Exists(full_audio_folder))
                {
                    File.Move(outer_downloaded_audio_filename, m_downloaded_audio_filename);
                }
            }
            // audio file exists in backup folder
            else if (File.Exists(backup_downloaded_audio_filename))
            {
                m_downloaded_audio_filename = backup_downloaded_audio_filename;
            }
            else
            {
                // try to download audio file
                if (m_client != null)
                {
                    if (m_client.Book != null)
                    {
                        if (m_client.Book.RecitationInfos != null)
                        {
                            string recitation_url = null;
                            foreach (string key in m_client.Book.RecitationInfos.Keys)
                            {
                                if (m_client.Book.RecitationInfos[key].Folder == m_reciter)
                                {
                                    recitation_url = RecitationInfo.UrlPrefix + m_client.Book.RecitationInfos[key].Url + "/" + audio_filename;
                                    break;
                                }
                            }

                            DownloadFile(recitation_url, m_downloaded_audio_filename);
                        }
                    }
                }
            }
        }
    }
    private string GetVerseAudioFilename(int verse_index)
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                if (m_client.Book.Verses != null)
                {
                    if ((verse_index >= 0) && (verse_index < m_client.Book.Verses.Count))
                    {
                        Verse verse = m_client.Book.Verses[verse_index];
                        if (verse != null)
                        {
                            if (verse.Chapter != null)
                            {
                                return (verse.Chapter.Number.ToString("000") + verse.NumberInChapter.ToString("000") + "." + RecitationInfo.FileType);
                            }
                        }
                    }
                }
            }
        }
        return "000000.mp3";
    }
    private string GetVerseAudioFilepath(int verse_index)
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                if (m_client.Book.Verses != null)
                {
                    if ((verse_index >= 0) && (verse_index < m_client.Book.Verses.Count))
                    {
                        Verse verse = m_client.Book.Verses[verse_index];
                        if (verse != null)
                        {
                            if (verse.Chapter != null)
                            {
                                return (verse.Chapter.Number.ToString("000") + "/" + verse.Chapter.Number.ToString("000") + verse.NumberInChapter.ToString("000") + "." + RecitationInfo.FileType);
                            }
                        }
                    }
                }
            }
        }
        return "000/000000.mp3";
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 13. Audio Recitations
    ///////////////////////////////////////////////////////////////////////////////
    private string m_reciter = Client.DEFAULT_RECITATION;
    private void PopulateRecitationsCheckedListBox()
    {
        try
        {
            // TRICK: to disable item in a list, just ignore user check using this trick
            RecitationsCheckedListBox.ItemCheck += new ItemCheckEventHandler(RecitationsCheckedListBox_ItemCheck);

            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    if (m_client.Book.RecitationInfos != null)
                    {
                        RecitationsCheckedListBox.SelectedIndexChanged -= new EventHandler(RecitationsCheckedListBox_SelectedIndexChanged);
                        RecitationsCheckedListBox.BeginUpdate();
                        RecitationsCheckedListBox.Items.Clear();
                        foreach (string key in m_client.Book.RecitationInfos.Keys)
                        {
                            string reciter = m_client.Book.RecitationInfos[key].Reciter;
                            RecitationsCheckedListBox.Items.Add(reciter);
                        }
                    }
                }
            }
        }
        finally
        {
            RecitationsCheckedListBox.EndUpdate();
            RecitationsCheckedListBox.SelectedIndexChanged += new EventHandler(RecitationsCheckedListBox_SelectedIndexChanged);
        }
    }
    private void RecitationsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        if (e.CurrentValue == CheckState.Indeterminate)
        {
            e.NewValue = e.CurrentValue;
        }
    }
    private void PopulateReciterComboBox()
    {
        try
        {
            ReciterComboBox.BeginUpdate();
            ReciterComboBox.SelectedIndexChanged -= new EventHandler(ReciterComboBox_SelectedIndexChanged);

            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    if (m_client.Book.RecitationInfos != null)
                    {
                        ReciterComboBox.Items.Clear();
                        foreach (string key in m_client.Book.RecitationInfos.Keys)
                        {
                            string reciter = m_client.Book.RecitationInfos[key].Reciter;
                            ReciterComboBox.Items.Add(reciter);
                        }
                        if (ReciterComboBox.Items.Count > 3)
                        {
                            ReciterComboBox.SelectedIndex = 3;
                        }
                    }
                }
            }
        }
        finally
        {
            ReciterComboBox.EndUpdate();
            ReciterComboBox.SelectedIndexChanged += new EventHandler(ReciterComboBox_SelectedIndexChanged);
        }
    }
    private void UpdateRecitationsCheckedListBox()
    {
        try
        {
            /////////////////////////////////////////////////////////////////////////////
            // foreach reciter -> foreach verse, if audio file exist and valid then check
            /////////////////////////////////////////////////////////////////////////////

            if (m_downloaded_reciter_folders == null)
            {
                m_downloaded_reciter_folders = new List<string>();
            }
            m_downloaded_reciter_folders.Clear();

            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    if (m_client.Book.RecitationInfos != null)
                    {
                        foreach (string reciter_folder in m_client.Book.RecitationInfos.Keys)
                        {
                            bool fully_downloaded = true;
                            for (int i = 0; i < m_client.Book.Verses.Count; i++)
                            {
                                string download_folder = Globals.AUDIO_FOLDER + "/" + reciter_folder;
                                string filename = GetVerseAudioFilepath(i); // e.g. i=8 ==> 002/002001.mp3
                                string path = download_folder + "/" + filename;
                                if (File.Exists(path)) // file exist
                                {
                                    long filesize = (new FileInfo(path)).Length;
                                    if (filesize < 1024) // invalid file
                                    {
                                        fully_downloaded = false;
                                        break;
                                    }
                                }
                                else // file not found
                                {
                                    fully_downloaded = false;
                                    break;
                                }
                            }

                            int index = 0;
                            string reciter = m_client.Book.RecitationInfos[reciter_folder].Reciter;
                            for (int i = 0; i < RecitationsCheckedListBox.Items.Count; i++)
                            {
                                if (RecitationsCheckedListBox.Items[i].ToString() == reciter)
                                {
                                    index = i;
                                }
                            }

                            if (fully_downloaded)
                            {
                                RecitationsCheckedListBox.SetItemCheckState(index, CheckState.Indeterminate);
                                m_downloaded_reciter_folders.Add(reciter_folder);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
    }
    private void RecitationsCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
    }
    private void RecitationsCheckedListBox_MouseUp(object sender, MouseEventArgs e)
    {
        if (RecitationsCheckedListBox.SelectedItem != null)
        {
            string reciter = RecitationsCheckedListBox.SelectedItem.ToString();

            string reciter_folder = null;
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    if (m_client.Book.RecitationInfos != null)
                    {
                        foreach (string key in m_client.Book.RecitationInfos.Keys)
                        {
                            if (reciter == m_client.Book.RecitationInfos[key].Reciter)
                            {
                                reciter_folder = key;
                                break;
                            }
                        }
                    }
                }
            }

            if (m_downloaded_reciter_folders.Contains(reciter_folder))
            {
                RecitationsCheckedListBox.SetItemCheckState(RecitationsCheckedListBox.SelectedIndex, CheckState.Indeterminate);
            }
        }
    }
    private void ReciterComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ReciterComboBox.SelectedItem != null)
        {
            string reciter = ReciterComboBox.SelectedItem.ToString();
            RecitationGroupBox.Text = reciter + "                                 ";

            // update m_recitation_folder
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    if (m_client.Book.RecitationInfos != null)
                    {
                        foreach (string key in m_client.Book.RecitationInfos.Keys)
                        {
                            if (m_client.Book.RecitationInfos[key].Reciter == reciter)
                            {
                                m_reciter = m_client.Book.RecitationInfos[key].Folder;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    private void RecitationsApplySettingsLabel_Click(object sender, EventArgs e)
    {
        if (!RecitationsDownloadGroupBox.Visible)
        {
            UpdateRecitationsCheckedListBox();

            RecitationsDownloadGroupBox.Visible = true;
            RecitationsCancelSettingsLabel.Visible = true;
            RecitationsDownloadGroupBox.BringToFront();

            if (File.Exists(Globals.IMAGES_FOLDER + "/" + "apply.png"))
            {
                RecitationsApplySettingsLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "apply.png");
            }
            ToolTip.SetToolTip(RecitationsApplySettingsLabel, "Download complete Quran recitations");
        }
        else
        {
            RecitationsDownloadGroupBox.Visible = false;
            RecitationsCancelSettingsLabel.Visible = false;

            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    if (File.Exists(Globals.IMAGES_FOLDER + "/" + "settings.png"))
                    {
                        RecitationsApplySettingsLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "settings.png");
                    }
                    ToolTip.SetToolTip(RecitationsApplySettingsLabel, "Add/Remove recitations");

                    if (m_client.Book.RecitationInfos != null)
                    {
                        try
                        {
                            List<string> keys_to_download = new List<string>();
                            foreach (int cheched_index in RecitationsCheckedListBox.CheckedIndices)
                            {
                                if (RecitationsCheckedListBox.GetItemCheckState(cheched_index) != CheckState.Indeterminate)
                                {
                                    foreach (string key in m_client.Book.RecitationInfos.Keys)
                                    {
                                        string reciter = RecitationsCheckedListBox.Items[cheched_index].ToString();
                                        if (m_client.Book.RecitationInfos[key].Reciter == reciter)
                                        {
                                            keys_to_download.Add(key);
                                            break;
                                        }
                                    }
                                }
                            }

                            foreach (string reciter_folder in m_client.Book.RecitationInfos.Keys)
                            {
                                if (keys_to_download.Contains(reciter_folder))
                                {
                                    ProgressBar.Minimum = 1;
                                    ProgressBar.Maximum = m_client.Book.Verses.Count;
                                    ProgressBar.Value = 1;
                                    ProgressBar.Refresh();

                                    for (int i = 0; i < m_client.Book.Verses.Count; i++)
                                    {
                                        string download_folder = Globals.AUDIO_FOLDER + "/" + reciter_folder;
                                        string filename = GetVerseAudioFilename(i); // e.g. i=8 ==> 002001.mp3
                                        string full_filename = GetVerseAudioFilepath(i); // e.g. i=8 ==> 002/002001.mp3
                                        string full_path = download_folder + "/" + full_filename;
                                        if (File.Exists(full_path)) // file exist
                                        {
                                            long filesize = (new FileInfo(full_path)).Length;
                                            if (filesize < 1024) // if < 1kb invalid file then re-download
                                            {
                                                DownloadFile(RecitationInfo.UrlPrefix + m_client.Book.RecitationInfos[reciter_folder].Url + "/" + filename, full_path);
                                            }
                                        }
                                        else // file not found so download it
                                        {
                                            DownloadFile(RecitationInfo.UrlPrefix + m_client.Book.RecitationInfos[reciter_folder].Url + "/" + filename, full_path);
                                        }

                                        ProgressBar.Value = i + 1;
                                        ProgressBar.Refresh();

                                        Application.DoEvents();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, Application.ProductName);
                        }
                        finally
                        {
                            if (m_client.Selection != null)
                            {
                                List<Verse> verses = m_client.Selection.Verses;
                                if (verses.Count > 0)
                                {
                                    ProgressBar.Minimum = 1;
                                    ProgressBar.Maximum = m_client.Book.Verses.Count;
                                    ProgressBar.Value = verses[0].Number;
                                    ProgressBar.Refresh();
                                }
                            }
                        }
                    }
                }
            }
        }

        RecitationsApplySettingsLabel.Refresh();
    }
    private void RecitationsCancelSettingsLabel_Click(object sender, EventArgs e)
    {
        RecitationsDownloadGroupBox.Visible = false;
        RecitationsDownloadGroupBox.Refresh();
        RecitationsCancelSettingsLabel.Visible = RecitationsDownloadGroupBox.Visible;
        RecitationsCancelSettingsLabel.Refresh();
        if (File.Exists(Globals.IMAGES_FOLDER + "/" + "settings.png"))
        {
            RecitationsApplySettingsLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "settings.png");
        }
        ToolTip.SetToolTip(RecitationsApplySettingsLabel, "Setup recitations");
        PopulateRecitationsCheckedListBox();
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 14. Translations
    ///////////////////////////////////////////////////////////////////////////////
    private int m_information_page_index = 0;
    private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
    {
        m_information_page_index = TabControl.SelectedIndex;
    }
    private int m_information_box_top = DEFAULT_INFORMATION_BOX_TOP;
    // translation
    private List<string> m_selected_translations = new List<string>();
    private void PopulateTranslatorsCheckedListBox()
    {
        try
        {
            // TRICK: to disable item in a list, just ignore user check using this trick
            TranslatorsCheckedListBox.ItemCheck += new ItemCheckEventHandler(TranslatorsCheckedListBox_ItemCheck);

            TranslatorsCheckedListBox.SelectedIndexChanged -= new EventHandler(TranslatorsCheckedListBox_SelectedIndexChanged);
            TranslatorsCheckedListBox.BeginUpdate();
            TranslatorsCheckedListBox.Items.Clear();

            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    if (m_client.Book.Verses != null)
                    {
                        if (m_client.Book.Verses.Count > 0)
                        {
                            m_selected_translations.Clear();
                            foreach (string key in m_client.Book.Verses[0].Translations.Keys)
                            {
                                m_selected_translations.Add(key);
                            }

                            // populate TranslatorsCheckedListBox
                            if (m_client.Book.TranslationInfos != null)
                            {
                                foreach (string key in m_client.Book.TranslationInfos.Keys)
                                {
                                    string name = m_client.Book.TranslationInfos[key].Name;
                                    bool is_checked = m_selected_translations.Contains(key);
                                    TranslatorsCheckedListBox.Items.Add(name, is_checked);
                                }

                                // disable list item if default so user cannot uncheck it
                                for (int i = 0; i < TranslatorsCheckedListBox.Items.Count; i++)
                                {
                                    string item_text = TranslatorsCheckedListBox.Items[i].ToString();
                                    foreach (string key in m_client.Book.TranslationInfos.Keys)
                                    {
                                        string name = m_client.Book.TranslationInfos[key].Name;
                                        if (name == item_text)
                                        {
                                            if (
                                                (key == Client.DEFAULT_EMLAAEI_TEXT) ||
                                                (key == Client.DEFAULT_TRANSLATION) ||
                                                (key == Client.DEFAULT_TRANSLITERATION) ||
                                                (key == Client.DEFAULT_WORD_MEANINGS)
                                               )
                                            {
                                                TranslatorsCheckedListBox.SetItemCheckState(i, CheckState.Indeterminate);
                                            }
                                            break;
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
            TranslatorsCheckedListBox.Sorted = true;
            TranslatorsCheckedListBox.EndUpdate();
            TranslatorsCheckedListBox.SelectedIndexChanged += new EventHandler(TranslatorsCheckedListBox_SelectedIndexChanged);
        }
    }
    private void TranslatorsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        if (e.CurrentValue == CheckState.Indeterminate)
        {
            e.NewValue = e.CurrentValue;
        }
    }
    private void PopulateTranslatorComboBox()
    {
        try
        {
            TranslatorComboBox.SelectedIndexChanged -= new EventHandler(TranslatorComboBox_SelectedIndexChanged);
            TranslatorComboBox.BeginUpdate();

            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    if (m_client.Book.Verses != null)
                    {
                        if (m_client.Book.Verses.Count > 0)
                        {
                            if (m_client.Book.Verses[0].Translations != null)
                            {
                                if (m_client.Book.Verses[0].Translations.Count == 0)
                                {
                                    DownloadTranslations();
                                }

                                string backup_translation_name = null;
                                if (TranslatorComboBox.SelectedItem != null)
                                {
                                    backup_translation_name = TranslatorComboBox.SelectedItem.ToString();
                                }

                                if (m_client.Book.TranslationInfos != null)
                                {
                                    TranslatorComboBox.Items.Clear();
                                    foreach (string key in m_client.Book.Verses[0].Translations.Keys)
                                    {
                                        string name = m_client.Book.TranslationInfos[key].Name;
                                        TranslatorComboBox.Items.Add(name);
                                    }

                                    if (!String.IsNullOrEmpty(backup_translation_name))
                                    {
                                        bool found = false;
                                        for (int i = 0; i < TranslatorComboBox.Items.Count; i++)
                                        {
                                            if (TranslatorComboBox.Items[i].ToString() == backup_translation_name)
                                            {
                                                found = true;
                                                break;
                                            }
                                        }
                                        if (found)
                                        {
                                            this.TranslatorComboBox.SelectedItem = backup_translation_name;
                                        }
                                        else
                                        {
                                            this.TranslatorComboBox.SelectedItem = m_client.Book.TranslationInfos[Client.DEFAULT_TRANSLATION].Name;
                                        }
                                    }
                                    else // if all translations were cleared, we still have the 3 mandatory ones at minimum
                                    {
                                        if (this.TranslatorComboBox.Items.Count >= 3)
                                        {
                                            this.TranslatorComboBox.SelectedItem = m_client.Book.TranslationInfos[Client.DEFAULT_TRANSLATION].Name;
                                        }
                                        else // if user deleted one or more of the 3 mandatory translations manually
                                        {
                                            if (this.TranslatorComboBox.Items.Count > 0)
                                            {
                                                this.TranslatorComboBox.SelectedItem = 0;
                                            }
                                            else // if no transaltion at all was left
                                            {
                                                TranslatorComboBox.SelectedIndex = -1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch
        {
            TranslatorComboBox.SelectedIndex = -1;
        }
        finally
        {
            TranslatorComboBox.Sorted = true;
            TranslatorComboBox.EndUpdate();
            TranslatorComboBox.SelectedIndexChanged += new EventHandler(TranslatorComboBox_SelectedIndexChanged);
        }
    }
    private void TranslatorsCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
    }
    private void TranslationsApplySettingsLabel_Click(object sender, EventArgs e)
    {
        TranslatorsCheckedListBox.Visible = !TranslatorsCheckedListBox.Visible;
        TranslationsCancelSettingsLabel.Visible = TranslatorsCheckedListBox.Visible;

        if (TranslatorsCheckedListBox.Visible)
        {
            TranslatorsCheckedListBox.BringToFront();
            TranslationsCancelSettingsLabel.BringToFront();

            if (File.Exists(Globals.IMAGES_FOLDER + "/" + "apply.png"))
            {
                TranslationsApplySettingsLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "apply.png");
            }
            ToolTip.SetToolTip(TranslationsApplySettingsLabel, "Download translations");
        }
        else // download any newly checked translation(s)
        {
            TranslatorsCheckedListBox.SendToBack();
            TranslationsCancelSettingsLabel.SendToBack();

            if (File.Exists(Globals.IMAGES_FOLDER + "/" + "settings.png"))
            {
                TranslationsApplySettingsLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "settings.png");
            }
            ToolTip.SetToolTip(TranslationsApplySettingsLabel, "Add/Remove translations");

            int index_of_first_new_translation = DownloadTranslations();
            if ((index_of_first_new_translation >= 0) && (index_of_first_new_translation < TranslatorComboBox.Items.Count))
            {
                TranslatorComboBox.SelectedIndex = index_of_first_new_translation;
            }
            this.AcceptButton = null;
        }
    }
    private void TranslationsCancelSettingsLabel_Click(object sender, EventArgs e)
    {
        TranslatorsCheckedListBox.Visible = false;
        TranslatorsCheckedListBox.SendToBack();
        TranslationsCancelSettingsLabel.SendToBack();
        TranslationsCancelSettingsLabel.Visible = false;

        if (File.Exists(Globals.IMAGES_FOLDER + "/" + "settings.png"))
        {
            TranslationsApplySettingsLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "settings.png");
        }
        ToolTip.SetToolTip(TranslationsApplySettingsLabel, "Add/Remove translations");

        // remove any new user checkes 
        PopulateTranslatorsCheckedListBox();

        this.AcceptButton = null;
    }
    private void TranslatorComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (m_selection_mode)
        {
            if (m_client != null)
            {
                List<Verse> verses = null;
                if (m_found_verses_displayed)
                {
                    verses = m_client.FoundVerses;
                }
                else
                {
                    if (m_client.Selection != null)
                    {
                        verses = m_client.Selection.Verses;
                    }
                }

                if (verses != null)
                {
                    DisplayTranslations(verses);
                }
            }
        }
        else
        {
            Verse verse = GetVerse(CurrentVerseIndex);
            if (verse != null)
            {
                DisplayTranslations(verse);
            }
        }
    }
    private void AllTranslatorsCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        TranslatorComboBox_SelectedIndexChanged(null, null);

        TranslatorComboBox.Enabled = !AllTranslatorsCheckBox.Checked;
    }
    private int DownloadTranslations()
    {
        int index_of_first_new_transaltion = -1;

        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                m_selected_translations.Clear();
                if (m_client.Book.TranslationInfos != null)
                {
                    try
                    {
                        foreach (string key in m_client.Book.TranslationInfos.Keys)
                        {
                            if (
                                (key == Client.DEFAULT_EMLAAEI_TEXT) ||
                                (key == Client.DEFAULT_TRANSLATION) ||
                                (key == Client.DEFAULT_TRANSLITERATION) ||
                                (key == Client.DEFAULT_WORD_MEANINGS)
                               )
                            {
                                m_selected_translations.Add(key);
                            }
                            else
                            {
                                foreach (int index in TranslatorsCheckedListBox.CheckedIndices)
                                {
                                    if (m_client.Book.TranslationInfos[key].Name == TranslatorsCheckedListBox.Items[index].ToString())
                                    {
                                        m_selected_translations.Add(key);
                                        break;
                                    }
                                }
                            }
                        }

                        ProgressBar.Minimum = 0;
                        ProgressBar.Maximum = m_selected_translations.Count;
                        ProgressBar.Value = 0;
                        ProgressBar.Refresh();

                        string[] keys = new string[m_client.Book.TranslationInfos.Keys.Count];
                        m_client.Book.TranslationInfos.Keys.CopyTo(keys, 0);
                        foreach (string key in keys)
                        {
                            if (m_selected_translations.Contains(key))
                            {
                                ProgressBar.Value++;
                                ProgressBar.Refresh();

                                string translations_path = Globals.TRANSLATIONS_FOLDER + "/" + key + ".txt";
                                string offline_path = Globals.TRANSLATIONS_OFFLINE_FOLDER + "/" + key + ".txt";

                                // delete file in translations_path if invalid
                                if (File.Exists(translations_path))
                                {
                                    long filesize = (new FileInfo(translations_path)).Length;
                                    if (filesize < 1024) // < 1kb invalid file
                                    {
                                        File.Delete(translations_path);
                                    }
                                }

                                // delete file in offline_path if invalid
                                if (File.Exists(offline_path))
                                {
                                    long filesize = (new FileInfo(offline_path)).Length;
                                    if (filesize < 1024) // < 1kb invalid file
                                    {
                                        File.Delete(offline_path);
                                    }
                                }

                                if (!File.Exists(translations_path))
                                {
                                    // download file to offline_path
                                    if (!File.Exists(offline_path))
                                    {
                                        DownloadFile(TranslationInfo.UrlPrefix + m_client.Book.TranslationInfos[key].Url, offline_path);
                                    }

                                    // copy to translations_path
                                    if (File.Exists(offline_path))
                                    {
                                        long filesize = (new FileInfo(offline_path)).Length;
                                        if (filesize < 1024) // < 1kb invalid file
                                        {
                                            File.Delete(offline_path);
                                            m_client.UnloadTranslation(key);
                                        }
                                        else // copy valid file
                                        {
                                            File.Copy(offline_path, translations_path);
                                            m_client.LoadTranslation(key);
                                        }
                                    }

                                    // get index of first new translation
                                    if (index_of_first_new_transaltion == -1)
                                    {
                                        int index_of_new_transaltion = -1;
                                        foreach (int index in TranslatorsCheckedListBox.CheckedIndices)
                                        {
                                            index_of_new_transaltion++;
                                            if (m_client.Book.TranslationInfos[key].Name == TranslatorsCheckedListBox.Items[index].ToString())
                                            {
                                                index_of_first_new_transaltion = index_of_new_transaltion;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else // unload translation
                            {
                                if (File.Exists(Globals.TRANSLATIONS_FOLDER + "/" + key + ".txt"))
                                {
                                    m_client.UnloadTranslation(key);
                                    File.Delete(Globals.TRANSLATIONS_FOLDER + "/" + key + ".txt");
                                }
                            }

                            Application.DoEvents();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, Application.ProductName);
                    }
                    finally
                    {
                        PopulateTranslatorsCheckedListBox();
                        PopulateTranslatorComboBox();

                        if (m_client.Selection != null)
                        {
                            List<Verse> verses = m_client.Selection.Verses;
                            if (verses.Count > 0)
                            {
                                ProgressBar.Minimum = 1;
                                ProgressBar.Maximum = m_client.Book.Verses.Count;
                                ProgressBar.Value = verses[0].Number;
                                ProgressBar.Refresh();
                            }
                        }
                    }
                }
            }
        }

        return index_of_first_new_transaltion;
    }
    private List<Verse> m_translated_verses = new List<Verse>();
    private void DisplayTranslations(List<Verse> verses)
    {
        if (
             (m_text_display_mode == TextDisplayMode.Both) ||
             (m_text_display_mode == TextDisplayMode.TranslationOnly)
           )
        {
            if (verses != null)
            {
                if (verses.Count > 0)
                {
                    if (verses.Count == 1)
                    {
                        DisplayTranslations(verses[0]);
                    }
                    else // multiple verses
                    {
                        StringBuilder str = new StringBuilder();
                        if (verses.Count > 0)
                        {
                            if (verses.Count == 1)
                            {
                                foreach (string key in m_selected_translations)
                                {
                                    if (verses[0].Translations.ContainsKey(key))
                                    {
                                        str.AppendLine("[" + key.Pad(13) + "]\t" + verses[0].PaddedAddress + VERSE_ADDRESS_TRANSLATION_SEPARATOR + verses[0].Translations[key]);
                                    }
                                }
                                if (str.Length > 2)
                                {
                                    str.Remove(str.Length - 2, 2);
                                }
                            }
                            else
                            {
                                if (TranslatorComboBox.SelectedItem != null)
                                {
                                    if (m_client != null)
                                    {
                                        string name = TranslatorComboBox.SelectedItem.ToString();
                                        string key = m_client.GetTranslationKey(name);
                                        if (verses[0].Translations.ContainsKey(key))
                                        {
                                            foreach (Verse verse in verses)
                                            {
                                                str.AppendLine(verse.PaddedAddress + VERSE_ADDRESS_TRANSLATION_SEPARATOR + verse.Translations[key]);
                                            }
                                            if (str.Length > 2)
                                            {
                                                str.Remove(str.Length - 2, 2);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (m_active_textbox != null)
                        {
                            TranslationTextBox.WordWrap = m_active_textbox.WordWrap;
                        }
                        TranslationTextBox.Text = str.ToString();
                        TranslationTextBox.Refresh();

                        m_translated_verses.Clear();
                        m_translated_verses.AddRange(verses);

                        m_readonly_translation = false;
                        TranslationTextBox_ToggleReadOnly();
                        EditVerseTranslationLabel.Visible = false;
                    }
                }
                else
                {
                    TranslationTextBox.WordWrap = false;
                    TranslationTextBox.Text = null;
                    TranslationTextBox.Refresh();

                    m_readonly_translation = false;
                    TranslationTextBox_ToggleReadOnly();
                    EditVerseTranslationLabel.Visible = false;
                }
            }
        }
    }
    private void DisplayTranslations(Verse verse)
    {
        if (
             (m_text_display_mode == TextDisplayMode.Both) ||
             (m_text_display_mode == TextDisplayMode.TranslationOnly)
           )
        {
            if (verse != null)
            {
                if (AllTranslatorsCheckBox.Checked)
                {
                    //[ar.emlaaei   ]	001:006 اهْدِنَا الصِّرَاطَ الْمُسْتَقِيمَ
                    //[en.pickthall ]	001:006 Show us the straight path,
                    //[en.qarai     ]	001:006 Guide us on the straight path,
                    //[en.transliter]	001:006 Ihdina alssirata almustaqeema
                    //[en.wordbyword]	001:006 Guide us	(to) the path,	the straight,
                    StringBuilder str = new StringBuilder();
                    if (m_selected_translations.Count > 0)
                    {
                        foreach (string key in m_selected_translations)
                        {
                            if (verse.Translations.ContainsKey(key))
                            {
                                str.AppendLine("[" + key.Pad(13) + "]\t" + verse.PaddedAddress + VERSE_ADDRESS_TRANSLATION_SEPARATOR + verse.Translations[key]);
                            }
                        }
                        if (str.Length > 2)
                        {
                            str.Remove(str.Length - 2, 2);
                        }
                    }

                    m_translated_verses.Clear();
                    m_translated_verses.Add(verse);

                    if (m_active_textbox != null)
                    {
                        TranslationTextBox.WordWrap = m_active_textbox.WordWrap;
                    }
                    TranslationTextBox.Text = str.ToString();
                    TranslationTextBox.Refresh();

                    m_readonly_translation = false;
                    TranslationTextBox_ToggleReadOnly();
                    EditVerseTranslationLabel.Visible = false;
                }
                else
                {
                    DisplayTranslationAllowingEdit(verse);
                }
            }
            else
            {
                TranslationTextBox.WordWrap = false;
                TranslationTextBox.Text = null;
                TranslationTextBox.Refresh();

                m_readonly_translation = false;
                TranslationTextBox_ToggleReadOnly();
                EditVerseTranslationLabel.Visible = false;
            }
        }
    }
    // helper methods
    private void DisplayTranslationAllowingEdit(Verse verse)
    {
        if (
             (m_text_display_mode == TextDisplayMode.Both) ||
             (m_text_display_mode == TextDisplayMode.TranslationOnly)
           )
        {
            if (verse != null)
            {
                StringBuilder str = new StringBuilder();
                if (TranslatorComboBox.SelectedItem != null)
                {
                    if (m_client != null)
                    {
                        string name = TranslatorComboBox.SelectedItem.ToString();
                        string key = m_client.GetTranslationKey(name);
                        if (verse.Translations.ContainsKey(key))
                        {
                            str.Append(verse.PaddedAddress + VERSE_ADDRESS_TRANSLATION_SEPARATOR + verse.Translations[key]);
                        }
                    }
                }
                TranslationTextBox.WordWrap = true;
                TranslationTextBox.Text = str.ToString();
                TranslationTextBox.Refresh();
                m_translated_verses.Clear();
                m_translated_verses.Add(verse);

                m_readonly_translation = false;
                TranslationTextBox_ToggleReadOnly();
                EditVerseTranslationLabel.Visible = true;
            }
        }
    }
    // readonly mode
    private bool m_readonly_translation = true;
    private void EditVerseTranslationLabel_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (!m_readonly_translation)
            {
                SaveTranslation();

                if (m_player != null)
                {
                    if (m_player.Paused)
                    {
                        PlayerPlayLabel_Click(null, null);
                    }
                }
            }
            else
            {
                if (m_player != null)
                {
                    if (m_player.Playing)
                    {
                        PlayerPlayLabel_Click(null, null);
                    }
                }
            }

            TranslationTextBox_ToggleReadOnly();
        }
        finally
        {
            Thread.Sleep(100);
            this.Cursor = Cursors.Default;
        }
    }
    private void TranslationTextBox_ToggleReadOnly()
    {
        m_readonly_translation = !m_readonly_translation;

        TranslationTextBox.ReadOnly = m_readonly_translation;
        TranslationTextBox.BackColor = m_readonly_translation ? Color.LightGray : SystemColors.Window;

        if (m_readonly_translation)
        {
            if (File.Exists(Globals.IMAGES_FOLDER + "/" + "text_unlocked.png"))
            {
                EditVerseTranslationLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "text_unlocked.png");
            }
            ToolTip.SetToolTip(EditVerseTranslationLabel, "Edit translation");
        }
        else
        {
            if (File.Exists(Globals.IMAGES_FOLDER + "/" + "save.png"))
            {
                EditVerseTranslationLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "save.png");
            }
            ToolTip.SetToolTip(EditVerseTranslationLabel, "Save translation");
        }
    }
    private void SaveTranslation()
    {
        if (m_client != null)
        {
            Verse verse = GetVerse(CurrentVerseIndex);
            if (verse != null)
            {
                string translation = Client.DEFAULT_TRANSLATION;
                if (TranslatorComboBox.SelectedItem != null)
                {
                    translation = m_client.GetTranslationKey(TranslatorComboBox.SelectedItem.ToString());
                }

                int index = TranslationTextBox.Text.IndexOf(VERSE_ADDRESS_TRANSLATION_SEPARATOR);
                verse.Translations[translation] = TranslationTextBox.Text.Substring(index + 1);

                m_client.SaveTranslation(translation);
            }
        }
    }
    private void TranslationTextBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (ModifierKeys == Keys.Control)
            {
                if (e.KeyChar == 19) // Ctrl+S == 19 
                {
                    if (!m_readonly_translation)
                    {
                        SaveTranslation();

                        if (m_player != null)
                        {
                            if (m_player.Paused)
                            {
                                PlayerPlayLabel_Click(null, null);
                            }
                        }

                        TranslationTextBox_ToggleReadOnly();

                        e.Handled = true; // stop annoying beep for no default button defined
                    }
                }
            }
        }
        finally
        {
            Thread.Sleep(100);
            this.Cursor = Cursors.Default;
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 15. Search Setup
    ///////////////////////////////////////////////////////////////////////////////
    private SearchType m_search_type = SearchType.Text; // named with private to indicate must set via Property, not directly by field
    private LanguageType m_language_type = LanguageType.RightToLeft;
    private TextSearchType m_text_search_type = TextSearchType.Exact;
    private TextLocationInVerse m_text_location_in_verse = TextLocationInVerse.Anywhere;
    private TextLocationInWord m_text_location_in_word = TextLocationInWord.Anywhere;
    private ProximitySearchType m_proximity_search_type = ProximitySearchType.AllWords;
    private void SearchGroupBox_Enter(object sender, EventArgs e)
    {
    }
    private void SearchGroupBox_Leave(object sender, EventArgs e)
    {
        if (!AutoCompleteListBox.Focused)
        {
            AutoCompleteHeaderLabel.Visible = false;
            AutoCompleteListBox.Visible = false;
        }
    }
    private void ClearFindMatches()
    {
        this.Refresh(); // prevent controls from disappearing in heavy calculations

        PlayerStopLabel_Click(null, null);

        m_find_matches = new List<FindMatch>();
        m_find_match_index = -1;
    }
    private void UpdateFindByTextOptions()
    {
        if (FindByTextAtVerseAnywhereRadioButton.Checked)
        {
            m_text_location_in_verse = TextLocationInVerse.Anywhere;
        }
        else if (FindByTextAtVerseStartRadioButton.Checked)
        {
            m_text_location_in_verse = TextLocationInVerse.AtStart;
        }
        else if (FindByTextAtVerseMiddleRadioButton.Checked)
        {
            m_text_location_in_verse = TextLocationInVerse.AtMiddle;
        }
        else if (FindByTextAtVerseEndRadioButton.Checked)
        {
            m_text_location_in_verse = TextLocationInVerse.AtEnd;
        }

        if (FindByTextAtWordAnywhereRadioButton.Checked)
        {
            m_text_location_in_word = TextLocationInWord.Anywhere;
        }
        else if (FindByTextAtWordStartRadioButton.Checked)
        {
            m_text_location_in_word = TextLocationInWord.AtStart;
        }
        else if (FindByTextAtWordMiddleRadioButton.Checked)
        {
            m_text_location_in_word = TextLocationInWord.AtMiddle;
        }
        else if (FindByTextAtWordEndRadioButton.Checked)
        {
            m_text_location_in_word = TextLocationInWord.AtEnd;
        }
    }
    private int GetPhraseCount(List<Phrase> phrases)
    {
        int count = 0;
        foreach (Phrase phrase in phrases)
        {
            if (!String.IsNullOrEmpty(phrase.Text))
            {
                count++;
            }
        }
        return count;
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 16. Search AutoComplete
    ///////////////////////////////////////////////////////////////////////////////
    private bool m_auto_complete_list_double_click = false;
    private Dictionary<string, int> m_auto_complete_words = null;
    private void AutoCompleteListBox_Enter(object sender, EventArgs e)
    {
        this.AcceptButton = null;
    }
    private void AutoCompleteListBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            AutoCompleteListBox_DoubleClick(sender, e);
        }
        else if (e.KeyCode == Keys.Space)
        {
            FindByTextTextBox.Text += " ";
            FindByTextTextBox.Focus();
        }
        else if ((e.KeyCode == Keys.Left) || (e.KeyCode == Keys.Right))
        {
            FindByTextTextBox.Focus();
        }
        FindByTextTextBox.SelectionStart = FindByTextTextBox.Text.Length;
    }
    private void AutoCompleteListBox_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            // set cursor at mouse RIGHT-click location so we know which word to Find
            if (AutoCompleteListBox.SelectedIndices.Count == 1)
            {
                AutoCompleteListBox.SelectedIndex = -1;
            }
            AutoCompleteListBox.SelectedIndex = AutoCompleteListBox.IndexFromPoint(e.X, e.Y);
        }
    }
    private void AutoCompleteListBox_Click(object sender, EventArgs e)
    {
        // do nothing
    }
    private void AutoCompleteListBox_DoubleClick(object sender, EventArgs e)
    {
        m_auto_complete_list_double_click = true;
        if (AutoCompleteListBox.Items.Count > 0)
        {
            AddNextWordToFindText();
        }
        else
        {
            FindByTextButton_Click(null, null);
        }
        m_auto_complete_list_double_click = false;
    }
    private void AutoCompleteListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (m_auto_complete_words != null)
        {
            int count = 0;
            int total = 0;
            if (AutoCompleteListBox.SelectedIndices.Count > 1)
            {
                // update total(unique) counts
                foreach (object item in AutoCompleteListBox.SelectedItems)
                {
                    string[] parts = item.ToString().Split();
                    foreach (string key in m_auto_complete_words.Keys)
                    {
                        if (key == parts[parts.Length - 1])
                        {
                            string entry = String.Format("{0,-3} {1,10}", m_auto_complete_words[key], key);
                            total += m_auto_complete_words[key];
                            count++;

                            break;
                        }
                    }
                }
            }
            else
            {
                // restore total(unique) counts
                foreach (string key in m_auto_complete_words.Keys)
                {
                    string entry = String.Format("{0,-3} {1,10}", m_auto_complete_words[key], key);
                    total += m_auto_complete_words[key];
                    count++;
                }
            }
            AutoCompleteHeaderLabel.Text = total.ToString() + " (" + count.ToString() + ")";
            AutoCompleteHeaderLabel.Refresh();
        }
    }
    private void AddNextWordToFindText()
    {
        if (AutoCompleteListBox.SelectedItem != null)
        {
            string word_to_add = AutoCompleteListBox.SelectedItem.ToString();
            int pos = word_to_add.LastIndexOf(' ');
            if (pos > -1)
            {
                word_to_add = word_to_add.Substring(pos + 1);
            }

            string text = FindByTextTextBox.Text;
            int index = text.LastIndexOf(' ');
            if (index != -1)
            {
                if (text.Length > index + 1)
                {
                    if ((text[index + 1] == '+') || (text[index + 1] == '-'))
                    {
                        index++;
                    }
                }

                text = text.Substring(0, index + 1);
                text += word_to_add;
                FindByTextTextBox.Text = text + " ";
                m_edited_by_hand = false;
            }
            else
            {
                FindByTextTextBox.Text = word_to_add + " ";
                m_edited_by_hand = false;
            }
            FindByTextTextBox.Refresh();
            FindByTextTextBox.SelectionStart = FindByTextTextBox.Text.Length;
        }
    }
    private void ClearAutoCompleteListBox()
    {
        AutoCompleteListBox.Items.Clear();
    }
    private void PopulateAutoCompleteListBox()
    {
        if (m_text_search_type == TextSearchType.Exact)
        {
            PopulateAutoCompleteListBoxWithCurrentOrNextWords();
        }
        else if (m_text_search_type == TextSearchType.Root)
        {
            PopulateAutoCompleteListBoxWithRoots();
        }
        else if (m_text_search_type == TextSearchType.Proximity)
        {
            PopulateAutoCompleteListBoxWithCurrentWords();
        }
    }
    private void PopulateAutoCompleteListBoxWithCurrentOrNextWords()
    {
        AutoCompleteListBox.SelectedIndexChanged -= new EventHandler(AutoCompleteListBox_SelectedIndexChanged);

        try
        {
            if (m_client != null)
            {
                //SearchGroupBox.Text = " Search by Exact words      ";
                //SearchGroupBox.Refresh();
                //AutoCompleteHeaderLabel.Text = "000 (00)";
                //ToolTip.SetToolTip(AutoCompleteHeaderLabel, "total (unique)");
                //AutoCompleteHeaderLabel.Refresh();

                AutoCompleteListBox.BeginUpdate();
                AutoCompleteListBox.Items.Clear();

                string text = FindByTextTextBox.Text;
                if (!String.IsNullOrEmpty(text))
                {
                    if (text.EndsWith(" "))
                    {
                        m_auto_complete_words = m_client.GetNextWords(text, m_text_location_in_verse, m_text_location_in_word, m_with_diacritics);
                    }
                    else
                    {
                        m_auto_complete_words = m_client.GetCurrentWords(text, m_text_location_in_verse, m_text_location_in_word, m_with_diacritics);
                    }

                    if (m_auto_complete_words != null)
                    {
                        int count = 0;
                        int total = 0;
                        foreach (string key in m_auto_complete_words.Keys)
                        {
                            //string value_str = found_words[key].ToString().PadRight(3, ' ');
                            //string key_str = key.PadLeft(10, ' ');
                            //string entry = String.Format("{0} {1}", value_str, key_str);
                            string entry = String.Format("{0,-3} {1,10}", m_auto_complete_words[key], key);
                            AutoCompleteListBox.Items.Add(entry);
                            total += m_auto_complete_words[key];
                            count++;
                        }

                        if (AutoCompleteListBox.Items.Count > 0)
                        {
                            AutoCompleteListBox.SelectedIndex = 0;
                        }
                        else // no match [either current text_mode doesn't have a match or it was last word in verse]
                        {
                            // m_auto_complete_list_double_click == false if input was via keyboard
                            // m_auto_complete_list_double_click == true  if input was via double click
                            // if no more word when double click, then it means it was the last word in the verse
                            // else the user has entered non-matching text

                            // if last word in verse, remove the extra space after it
                            if ((m_auto_complete_list_double_click) && (AutoCompleteListBox.Items.Count == 0) && (FindByTextTextBox.Text.EndsWith(" ")))
                            {
                                FindByTextTextBox.TextChanged -= new EventHandler(FindByTextTextBox_TextChanged);
                                try
                                {
                                    FindByTextTextBox.Text = FindByTextTextBox.Text.Remove(FindByTextTextBox.Text.Length - 1);
                                }
                                finally
                                {
                                    FindByTextTextBox.TextChanged += new EventHandler(FindByTextTextBox_TextChanged);
                                }
                            }
                        }

                        AutoCompleteHeaderLabel.Text = total.ToString() + " (" + count.ToString() + ")";
                        AutoCompleteHeaderLabel.Refresh();
                    }
                }
            }
        }
        finally
        {
            AutoCompleteListBox.EndUpdate();
            AutoCompleteListBox.SelectedIndexChanged += new EventHandler(AutoCompleteListBox_SelectedIndexChanged);
        }
    }
    private void PopulateAutoCompleteListBoxWithCurrentWords()
    {
        AutoCompleteListBox.SelectedIndexChanged -= new EventHandler(AutoCompleteListBox_SelectedIndexChanged);

        try
        {
            if (m_client != null)
            {
                //SearchGroupBox.Text = " Search by Proximity        ";
                //SearchGroupBox.Refresh();
                //AutoCompleteHeaderLabel.Text = "000 (00)";
                //ToolTip.SetToolTip(AutoCompleteHeaderLabel, "total (unique)");
                //AutoCompleteHeaderLabel.Refresh();

                AutoCompleteListBox.BeginUpdate();
                AutoCompleteListBox.Items.Clear();

                string text = FindByTextTextBox.Text;
                if (!String.IsNullOrEmpty(text))
                {
                    string[] text_parts = text.Split();
                    text = text_parts[text_parts.Length - 1];
                    if (!String.IsNullOrEmpty(text))
                    {
                        m_auto_complete_words = m_client.GetCurrentWords(text, m_text_location_in_verse, m_text_location_in_word, m_with_diacritics);
                        if (m_auto_complete_words != null)
                        {
                            int count = 0;
                            int total = 0;
                            foreach (string key in m_auto_complete_words.Keys)
                            {
                                //string value_str = found_words[key].ToString().PadRight(3, ' ');
                                //string key_str = key.PadLeft(10, ' ');
                                //string entry = String.Format("{0} {1}", value_str, key_str);
                                string entry = String.Format("{0,-3} {1,10}", m_auto_complete_words[key], key);
                                AutoCompleteListBox.Items.Add(entry);
                                total += m_auto_complete_words[key];
                                count++;
                            }


                            if (AutoCompleteListBox.Items.Count > 0)
                            {
                                AutoCompleteListBox.SelectedIndex = 0;
                            }
                            else
                            {
                                // if not a valid word, keep word as is
                            }

                            AutoCompleteHeaderLabel.Text = total.ToString() + " (" + count.ToString() + ")";
                            AutoCompleteHeaderLabel.Refresh();
                        }
                    }
                }
            }
        }
        finally
        {
            AutoCompleteListBox.EndUpdate();
            AutoCompleteListBox.SelectedIndexChanged += new EventHandler(AutoCompleteListBox_SelectedIndexChanged);
        }
    }
    private void PopulateAutoCompleteListBoxWithRoots()
    {
        AutoCompleteListBox.SelectedIndexChanged -= new EventHandler(AutoCompleteListBox_SelectedIndexChanged);

        try
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    //SearchGroupBox.Text = " Search by Roots            ";
                    //SearchGroupBox.Refresh();
                    AutoCompleteHeaderLabel.Text = "0 Roots";
                    ToolTip.SetToolTip(AutoCompleteHeaderLabel, "total roots");
                    AutoCompleteHeaderLabel.Refresh();

                    AutoCompleteListBox.BeginUpdate();
                    AutoCompleteListBox.Items.Clear();

                    string text = FindByTextTextBox.Text;

                    // to support multi root search take the last word a user is currently writing
                    string[] text_parts = text.Split();
                    if (text_parts.Length > 0)
                    {
                        text = text_parts[text_parts.Length - 1];
                    }

                    List<string> found_roots = null;
                    if (text.Length == 0)
                    {
                        found_roots = m_client.Book.GetRoots();
                    }
                    else if (!String.IsNullOrEmpty(text))
                    {
                        switch (m_text_location_in_word)
                        {
                            case TextLocationInWord.AtStart:
                                {
                                    found_roots = m_client.Book.GetRootsStartingWith(text, m_with_diacritics);
                                }
                                break;
                            case TextLocationInWord.AtMiddle:
                            case TextLocationInWord.AtEnd:
                            case TextLocationInWord.Anywhere:
                                {
                                    found_roots = m_client.Book.GetRootsContaining(text, m_with_diacritics);
                                }
                                break;
                        }
                    }

                    if (found_roots != null)
                    {
                        int count = 0;
                        foreach (string root in found_roots)
                        {
                            string entry = root.PadLeft(14, ' ');
                            //string entry = String.Format("{0,14}", root);
                            AutoCompleteListBox.Items.Add(entry);
                            count++;
                        }

                        if (AutoCompleteListBox.Items.Count > 0)
                        {
                            AutoCompleteListBox.SelectedIndex = 0;
                        }
                        else
                        {
                            // if not a valid root, put word as is so we can find same rooted words
                            AutoCompleteListBox.Items.Add(text);
                        }
                        AutoCompleteHeaderLabel.Text = count.ToString() + " Roots";
                        AutoCompleteHeaderLabel.Refresh();
                    }
                }
            }
        }
        finally
        {
            AutoCompleteListBox.EndUpdate();
            AutoCompleteListBox.SelectedIndexChanged += new EventHandler(AutoCompleteListBox_SelectedIndexChanged);
        }
    }
    private void FindSelectedWordsMenuItem_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_client != null)
            {
                // get startup text from FindTextBox
                string[] startup_words = FindByTextTextBox.Text.Split();
                int count = startup_words.Length;
                // ignore last partial word
                if (!FindByTextTextBox.Text.EndsWith(" "))
                {
                    count--;
                }
                string startup_text = "";
                for (int i = 0; i < count; i++)
                {
                    startup_text += startup_words[i] + " ";
                }
                if (startup_text.Length > 0)
                {
                    startup_text = startup_text.Remove(startup_text.Length - 1, 1);
                }

                // get selected word texts
                List<string> word_texts = new List<string>();
                if (AutoCompleteListBox.SelectedItems.Count > 0)
                {
                    char[] separators = { ' ' };
                    foreach (object item in AutoCompleteListBox.SelectedItems)
                    {
                        string[] parts = item.ToString().Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            word_texts.Add(parts[1]);
                        }
                    }
                }

                // setup search parameters
                string text = "";
                string translation = Client.DEFAULT_TRANSLATION;
                bool case_sensitive = FindByTextCaseSensitiveCheckBox.Checked;

                // update m_text_location_in_verse and m_text_location_in_word
                UpdateFindByTextOptions();

                TextWordness wordness = TextWordness.WholeWord;

                int multiplicity = -1;
                bool with_diacritics = false;

                List<Phrase> total_phrases = new List<Phrase>();
                List<Verse> total_verses = new List<Verse>();
                if (word_texts.Count > 0)
                {
                    foreach (string word_text in word_texts)
                    {
                        if (startup_text.Length > 0)
                        {
                            text = startup_text + " " + word_text;
                        }
                        else
                        {
                            text = word_text;
                        }

                        if (!String.IsNullOrEmpty(text))
                        {
                            m_client.FindPhrases(text, m_language_type, translation, m_text_location_in_verse, m_text_location_in_word, case_sensitive, wordness, multiplicity, with_diacritics);
                            total_phrases = total_phrases.Union(m_client.FoundPhrases);
                            total_verses = total_verses.Union(m_client.FoundVerses);
                        }
                    }

                    // write final result to m_client
                    m_client.FoundPhrases = total_phrases;
                    m_client.FoundVerses = total_verses;
                }

                // display results
                if (m_client.FoundPhrases != null)
                {
                    int phrase_count = GetPhraseCount(m_client.FoundPhrases);
                    if (m_client.FoundVerses != null)
                    {
                        int verse_count = m_client.FoundVerses.Count;
                        m_find_result_header = phrase_count + " matches in " + verse_count + ((verse_count == 1) ? " verse" : " verses") + " with " + text + " " + m_text_location_in_verse.ToString() + " " + m_text_location_in_verse.ToString() + " in " + m_client.SearchScope.ToString();
                        DisplayFoundVerses(true, true);
                    }
                }
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void SaveWordFrequenciesMenuItem_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            string text = FindByTextTextBox.Text;

            if (AutoCompleteHeaderLabel.Text.Length >= 5) // minimum is "0 (0)"
            {
                string[] header_parts = AutoCompleteHeaderLabel.Text.Split();
                if (header_parts.Length == 2)
                {
                    string total_str = header_parts[0];
                    string count_str = header_parts[1].Substring(1, header_parts[1].Length - 2);

                    string filename = Globals.STATISTICS_FOLDER + "/" + text + ".txt";
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                        {
                            StringBuilder str = new StringBuilder();
                            str.AppendLine("-----------------");
                            str.AppendLine("Word" + "\t" + "Frequency");
                            str.AppendLine("-----------------");

                            char[] separators = { ' ' };
                            if (AutoCompleteListBox.SelectedItems.Count > 1)
                            {
                                foreach (object item in AutoCompleteListBox.SelectedItems)
                                {
                                    string[] parts = item.ToString().Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                    if (parts.Length == 2)
                                    {
                                        str.AppendLine(parts[1] + "\t" + parts[0]);
                                    }
                                }
                            }
                            else
                            {
                                foreach (object item in AutoCompleteListBox.Items)
                                {
                                    string[] parts = item.ToString().Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                    if (parts.Length == 2)
                                    {
                                        str.AppendLine(parts[1] + "\t" + parts[0]);
                                    }
                                }
                            }
                            str.AppendLine("-----------------");
                            str.AppendLine("Count = " + count_str);
                            str.AppendLine("Total = " + total_str);

                            writer.Write(str.ToString());
                        }

                        // show file content after save
                        if (File.Exists(filename))
                        {
                            System.Diagnostics.Process.Start("Notepad.exe", filename);
                        }
                    }
                    catch
                    {
                        // silence IO error in case running from read-only media (CD/DVD)
                    }
                }
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 17. Search By Text
    ///////////////////////////////////////////////////////////////////////////////
    private bool m_with_diacritics = false;
    private void SetLanguageType(LanguageType language_type)
    {
        if (language_type == LanguageType.RightToLeft)
        {
            m_language_type = language_type;
        }
        else if (language_type == LanguageType.LeftToRight)
        {
            if (m_text_search_type == TextSearchType.Root)
            {
                m_language_type = LanguageType.RightToLeft;
            }
            else
            {
                m_language_type = language_type;
            }
        }
    }
    private void FindByTextExactSearchTypeLabel_Click(object sender, EventArgs e)
    {
        m_text_search_type = TextSearchType.Exact;
        PopulateAutoCompleteListBoxWithCurrentOrNextWords();
        FindByTextAtVerseAnywhereRadioButton.Checked = true;

        EnableFindByTextControls();
        FindByTextControls_Enter(null, null);
    }
    private void FindByTextProximitySearchTypeLabel_Click(object sender, EventArgs e)
    {
        m_text_search_type = TextSearchType.Proximity;
        PopulateAutoCompleteListBoxWithCurrentWords();
        FindByTextAllWordsRadioButton.Checked = true;

        EnableFindByTextControls();
        FindByTextControls_Enter(null, null);
    }
    private void FindByTextRootSearchTypeLabel_Click(object sender, EventArgs e)
    {
        m_text_search_type = TextSearchType.Root;
        PopulateAutoCompleteListBoxWithRoots();

        EnableFindByTextControls();
        FindByTextControls_Enter(null, null);
    }
    private void FindByTextRadioButton_CheckedChanged(object sender, EventArgs e)
    {
        UpdateFindByTextOptions();
        PopulateAutoCompleteListBox();
    }
    private void FindByTextWithDiacriticsCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_with_diacritics = FindByTextWithDiacriticsCheckBox.Checked;

        if (m_client.NumerologySystem.TextMode != "Original")
        {
            UpdateKeyboard(m_client.NumerologySystem.TextMode);
        }

        PopulateAutoCompleteListBox();
    }
    private void FindByTextWordnessCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        EnableFindByTextControls();
    }
    private void FindByTextCaseSensitiveCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        EnableFindByTextControls();
    }
    private void FindByTextMultiplicityCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        FindByTextMultiplicityNumericUpDown.Enabled = FindByTextMultiplicityCheckBox.Checked;
    }
    private void FindByTextMultiplicityNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
        // do nothing
    }
    private void FindByTextMultiplicityNumericUpDown_KeyDown(object sender, KeyEventArgs e)
    {
        // do nothing
    }
    private void FindByTextControls_Enter(object sender, EventArgs e)
    {
        this.AcceptButton = FindByTextButton;

        FindByTextButton.Enabled = true;
        FindBySimilarityButton.Enabled = false;
        FindByNumbersButton.Enabled = false;
        FindByRevelationPlaceButton.Enabled = false;
        FindByProstrationTypeButton.Enabled = false;
        FindByFrequencyButton.Enabled = false;

        AutoCompleteHeaderLabel.Visible = true;
        AutoCompleteListBox.Visible = true;
        AutoCompleteHeaderLabel.BringToFront();
        AutoCompleteListBox.BringToFront();

        //ResetFindByTextResultTypeLabels();
        ResetFindBySimilarityResultTypeLabels();
        ResetFindByNumbersResultTypeLabels();
        ResetFindByFrequencyResultTypeLabels();
    }
    private void FindByTextPanel_Leave(object sender, EventArgs e)
    {
        SearchGroupBox_Leave(null, null);
    }
    private bool m_edited_by_hand = false;
    private void FindByTextTextBox_TextChanged(object sender, EventArgs e)
    {
        m_edited_by_hand = true;

        EnableFindByTextControls();

        PopulateAutoCompleteListBox();

        UpdateLanguageType(FindByTextTextBox.Text);
    }
    private void FindByTextTextBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        FixMicrosoft(sender, e);

        if (e.KeyChar == ' ')
        {
            // prevent double spaces
            if (FindByTextTextBox.SelectionStart > 0)
            {
                if (FindByTextTextBox.Text[FindByTextTextBox.SelectionStart - 1] == ' ')
                {
                    e.Handled = true;
                }
            }
        }
    }
    private void FindByTextTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            if (e.KeyCode == Keys.A)
            {
                if (sender is TextBoxBase)
                {
                    (sender as TextBoxBase).SelectAll();
                }
            }
        }
        else if ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down))
        {
            AutoCompleteListBox.Focus();
        }
    }
    private void FindByTextButton_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            ClearFindMatches();

            switch (m_text_search_type)
            {
                case TextSearchType.Exact:
                    {
                        if (AutoCompleteListBox.SelectedItems.Count > 1)
                        {
                            FindSelectedWordsMenuItem_Click(null, null);
                        }
                        else
                        {
                            FindByExact();
                        }
                    }
                    break;
                case TextSearchType.Proximity:
                    {
                        FindByProximity();
                    }
                    break;
                case TextSearchType.Root:
                    {
                        FindByRoot();
                    }
                    break;
                default:
                    {
                        FindByExact();
                    }
                    break;
            }

            SearchGroupBox_Leave(null, null);
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void FindByExact()
    {
        string text = FindByTextTextBox.Text;
        if (text.Length > 0)
        {
            FindByExact(text);
        }
    }
    private void FindByExact(string text)
    {
        if (m_client != null)
        {
            ClearFindMatches();

            if (!m_edited_by_hand)
            {
                text = text.Trim(); // as space was added by AutoComplete double-click
            }

            if (!String.IsNullOrEmpty(text))
            {
                string translation = Client.DEFAULT_TRANSLATION;
                if (TranslatorComboBox.SelectedItem != null)
                {
                    translation = m_client.GetTranslationKey(TranslatorComboBox.SelectedItem.ToString());
                }

                UpdateFindByTextOptions();

                bool case_sensitive = FindByTextCaseSensitiveCheckBox.Checked;

                TextWordness wordness = TextWordness.Any;
                switch (FindByTextWordnessCheckBox.CheckState)
                {
                    case CheckState.Checked:
                        wordness = TextWordness.WholeWord;
                        break;
                    case CheckState.Indeterminate:
                        wordness = TextWordness.PartOfWord;
                        break;
                    case CheckState.Unchecked:
                        wordness = TextWordness.Any;
                        break;
                    default:
                        wordness = TextWordness.Any;
                        break;
                }

                int multiplicity = FindByTextMultiplicityNumericUpDown.Enabled ? (int)FindByTextMultiplicityNumericUpDown.Value : -1;

                FindByExact(text, m_language_type, translation, m_text_location_in_verse, m_text_location_in_word, case_sensitive, wordness, multiplicity, m_with_diacritics);
            }
        }
    }
    private void FindByExact(string text, LanguageType language_type, string translation, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, bool case_sensitive, TextWordness wordness, int multiplicity, bool with_diacritics)
    {
        m_search_type = SearchType.Text;

        if (m_client != null)
        {
            if (!String.IsNullOrEmpty(text))
            {
                m_client.FindPhrases(text, language_type, translation, text_location_in_verse, text_location_in_word, case_sensitive, wordness, multiplicity, m_with_diacritics);
                if (m_client.FoundPhrases != null)
                {
                    int phrase_count = GetPhraseCount(m_client.FoundPhrases);
                    if (m_client.FoundVerses != null)
                    {
                        int verse_count = m_client.FoundVerses.Count;
                        if (multiplicity == 0)
                        {
                            m_find_result_header = verse_count + ((verse_count == 1) ? " verse" : " verses") + " without " + text + " " + text_location_in_verse.ToString() + " " + text_location_in_word.ToString() + " in " + m_client.SearchScope.ToString();
                        }
                        else
                        {
                            m_find_result_header = phrase_count + " matches in " + verse_count + ((verse_count == 1) ? " verse" : " verses") + " with " + text + " " + text_location_in_verse.ToString() + " " + text_location_in_word.ToString() + " in " + m_client.SearchScope.ToString();
                        }
                        DisplayFoundVerses(true, true);
                    }
                }
            }
        }
    }
    private void FindByProximity()
    {
        string text = FindByTextTextBox.Text;
        if (text.Length > 0)
        {
            FindByProximity(text);
        }
    }
    private void FindByProximity(string text)
    {
        if (m_client != null)
        {
            ClearFindMatches();

            if (!String.IsNullOrEmpty(text))
            {
                string translation = Client.DEFAULT_TRANSLATION;
                if (TranslatorComboBox.SelectedItem != null)
                {
                    translation = m_client.GetTranslationKey(TranslatorComboBox.SelectedItem.ToString());
                }

                if (FindByTextAllWordsRadioButton.Checked)
                {
                    m_proximity_search_type = ProximitySearchType.AllWords;
                }
                else if (FindByTextAnyWordRadioButton.Checked)
                {
                    m_proximity_search_type = ProximitySearchType.AnyWord;
                }

                bool case_sensitive = FindByTextCaseSensitiveCheckBox.Checked;

                TextWordness wordness = TextWordness.Any;
                switch (FindByTextWordnessCheckBox.CheckState)
                {
                    case CheckState.Checked:
                        wordness = TextWordness.WholeWord;
                        break;
                    case CheckState.Indeterminate:
                        wordness = TextWordness.PartOfWord;
                        break;
                    case CheckState.Unchecked:
                        wordness = TextWordness.Any;
                        break;
                    default:
                        wordness = TextWordness.Any;
                        break;
                }

                FindByProximity(text, m_language_type, translation, m_proximity_search_type, case_sensitive, wordness, m_with_diacritics);
            }
        }
    }
    private void FindByProximity(string text, LanguageType language_type, string translation, ProximitySearchType proximity_search_type, bool case_sensitive, TextWordness wordness, bool with_diacritics)
    {
        m_search_type = SearchType.Text;

        if (m_client != null)
        {
            if (!String.IsNullOrEmpty(text))
            {
                m_client.FindPhrases(text, language_type, translation, proximity_search_type, case_sensitive, wordness, m_with_diacritics);
                if (m_client.FoundPhrases != null)
                {
                    int phrase_count = GetPhraseCount(m_client.FoundPhrases);
                    if (m_client.FoundVerses != null)
                    {
                        int verse_count = m_client.FoundVerses.Count;
                        m_find_result_header = phrase_count + " matches in " + verse_count + ((verse_count == 1) ? " verse" : " verses") + " with " + text + " " + proximity_search_type.ToString() + " in " + m_client.SearchScope.ToString();
                        DisplayFoundVerses(true, true);
                    }
                }
            }
        }
    }
    private void FindByRoot()
    {
        ClearFindMatches();

        if (FindByTextTextBox.Text.Length > 0)
        {
            string root = FindByTextTextBox.Text.Trim();
            int multiplicity = FindByTextMultiplicityNumericUpDown.Enabled ? (int)FindByTextMultiplicityNumericUpDown.Value : -1;
            FindByRoot(root, multiplicity, m_with_diacritics);
        }
    }
    private void FindByRoot(string root, int multiplicity, bool with_diacritics)
    {
        m_search_type = SearchType.Text;

        if (m_client != null)
        {
            if (!String.IsNullOrEmpty(root))
            {
                m_client.FindPhrases(root, multiplicity, with_diacritics);
                if (m_client.FoundPhrases != null)
                {
                    int phrase_count = GetPhraseCount(m_client.FoundPhrases);
                    if (m_client.FoundVerses != null)
                    {
                        int verse_count = m_client.FoundVerses.Count;
                        m_find_result_header = ((m_client.FoundPhrases != null) ? phrase_count + " matches in " : "") + verse_count + ((verse_count == 1) ? " verse" : " verses") + ((multiplicity == 0) ? " without " : " with root ") + root + " in " + m_client.SearchScope.ToString();
                        DisplayFoundVerses(true, true);
                    }
                }
            }
        }
    }
    private void FindByTextKeyboardLabel_Click(object sender, EventArgs e)
    {
        Control control = (sender as Control);
        if (control != null)
        {
            control.BackColor = Color.LightSteelBlue;
            control.Refresh();

            // prevent double spaces
            if (control == FindByTextSpaceLabel)
            {
                if (FindByTextTextBox.SelectionStart > 0)
                {
                    if (FindByTextTextBox.Text[FindByTextTextBox.SelectionStart - 1] == ' ')
                    {
                        return;
                    }
                }
            }

            string letter = control.Text[0].ToString();
            int pos = FindByTextTextBox.SelectionStart;
            int len = FindByTextTextBox.SelectionLength;
            if (pos >= 0)
            {
                if (len > 0)
                {
                    FindByTextTextBox.Text = FindByTextTextBox.Text.Remove(pos, len);
                }
                else
                {
                    // do nothing
                }
                FindByTextTextBox.Text = FindByTextTextBox.Text.Insert(pos, letter);
                FindByTextTextBox.SelectionStart = pos + 1;
                FindByTextTextBox.Refresh();
            }

            Thread.Sleep(100);
            control.BackColor = Color.LightGray;
            control.Refresh();

            FindByTextKeyboardLabel_MouseEnter(sender, e);
            FindByTextControls_Enter(null, null);

            FindByTextTextBox.Focus();
        }
    }
    private void FindByTextBackspaceLabel_Click(object sender, EventArgs e)
    {
        Control control = (sender as Control);
        if (control != null)
        {
            control.BackColor = Color.LightSteelBlue;
            control.Refresh();

            int pos = FindByTextTextBox.SelectionStart;
            int len = FindByTextTextBox.SelectionLength;
            if ((len == 0) && (pos > 0))        // delete character prior to cursor
            {
                FindByTextTextBox.Text = FindByTextTextBox.Text.Remove(pos - 1, 1);
                FindByTextTextBox.SelectionStart = pos - 1;
            }
            else if ((len > 0) && (pos >= 0))   // delete current highlighted characters
            {
                FindByTextTextBox.Text = FindByTextTextBox.Text.Remove(pos, len);
                FindByTextTextBox.SelectionStart = pos;
            }
            else                  // nothing to delete
            {
            }
            FindByTextTextBox.Refresh();

            Thread.Sleep(100);
            control.BackColor = Color.LightGray;
            control.Refresh();

            FindByTextKeyboardLabel_MouseEnter(sender, e);
            FindByTextControls_Enter(null, null);

            FindByTextTextBox.Focus();
        }
    }
    private void FindByTextKeyboardLabel_MouseEnter(object sender, EventArgs e)
    {
        Control control = (sender as Control);
        if (control != null)
        {
            if (control == FindByTextBackspaceLabel)
            {
                control.BackColor = Color.DarkGray;
            }
            else
            {
                control.BackColor = Color.White;
            }
            control.Refresh();
        }
    }
    private void FindByTextKeyboardLabel_MouseLeave(object sender, EventArgs e)
    {
        Control control = (sender as Control);
        if (control != null)
        {
            control.BackColor = Color.LightGray;
            control.Refresh();
        }
    }
    private void FindByTextKeyboardModifierLabel_MouseLeave(object sender, EventArgs e)
    {
        Control control = (sender as Control);
        if (control != null)
        {
            control.BackColor = Color.Silver;
            control.Refresh();
        }
    }
    private void FindByTextOrLabel_MouseHover(object sender, EventArgs e)
    {
        char[] idhaar_characters = { 'ء', 'أ', 'إ', 'ح', 'خ', 'ع', 'غ', 'ه', 'ة', 'ى' };
        char[] wasl_characters = { 'ٱ' };
        char[] med_characters = { 'ا', 'آ' };
        char[] iqlaab_characters = { 'ب' };
        char[] idghaam_characters = { 'ر', 'ل' };
        char[] idghaam_ghunna_characters = { 'م', 'ن', 'و', 'ؤ', 'ي', 'ئ' };
        char[] ikhfaa_characters = { 'ت', 'ث', 'ج', 'د', 'ذ', 'ز', 'س', 'ش', 'ص', 'ض', 'ط', 'ظ', 'ف', 'ق', 'ك' };

        Control control = (sender as Control);
        if (control != null)
        {
            string character_sound = null;

            if (character_sound == null)
            {
                foreach (char character in med_characters)
                {
                    if (character == control.Text[0])
                    {
                        character_sound = "مدّ";
                        break;
                    }
                }
            }
            if (character_sound == null)
            {
                foreach (char character in wasl_characters)
                {
                    if (character == control.Text[0])
                    {
                        character_sound = "إيصال";
                        break;
                    }
                }
            }
            if (character_sound == null)
            {
                foreach (char character in iqlaab_characters)
                {
                    if (character == control.Text[0])
                    {
                        character_sound = "إقلاب";
                        break;
                    }
                }
            }
            if (character_sound == null)
            {
                foreach (char character in idghaam_ghunna_characters)
                {
                    if (character == control.Text[0])
                    {
                        character_sound = "إدغام بغنة";
                        break;
                    }
                }
            }
            if (character_sound == null)
            {
                foreach (char character in idghaam_characters)
                {
                    if (character == control.Text[0])
                    {
                        //character_sound = "إدغام بلا غنة";
                        character_sound = "إدغام";
                        break;
                    }
                }
            }
            if (character_sound == null)
            {
                foreach (char character in idhaar_characters)
                {
                    if (character == control.Text[0])
                    {
                        character_sound = "إظهار";
                        break;
                    }
                }
            }
            if (character_sound == null)
            {
                foreach (char character in ikhfaa_characters)
                {
                    if (character == control.Text[0])
                    {
                        //character_sound = "إخفاء بغنة";
                        character_sound = "إخفاء";
                        break;
                    }
                }
            }

            int start = "FindByText".Length;
            int length = control.Name.Length - start - "Label".Length;
            ToolTip.SetToolTip(control, control.Name.Substring(start, length) + " " + character_sound);
        }
    }
    private void ResetFindByTextResultTypeLabels()
    {
        FindByTextExactSearchTypeLabel.BackColor = Color.DarkGray;
        FindByTextExactSearchTypeLabel.BorderStyle = BorderStyle.None;
        FindByTextProximitySearchTypeLabel.BackColor = Color.DarkGray;
        FindByTextProximitySearchTypeLabel.BorderStyle = BorderStyle.None;
        FindByTextRootSearchTypeLabel.BackColor = Color.DarkGray;
        FindByTextRootSearchTypeLabel.BorderStyle = BorderStyle.None;
    }
    private void UpdateKeyboard(string text_mode)
    {
        FindByTextHamzaLabel.Visible = FindByTextWithDiacriticsCheckBox.Checked;
        FindByTextTaaMarbootaLabel.Visible = FindByTextWithDiacriticsCheckBox.Checked;
        FindByTextElfMaqsuraLabel.Visible = FindByTextWithDiacriticsCheckBox.Checked;
        FindByTextElfWaslLabel.Visible = FindByTextWithDiacriticsCheckBox.Checked;
        FindByTextHamzaAboveElfLabel.Visible = FindByTextWithDiacriticsCheckBox.Checked;
        FindByTextHamzaBelowElfLabel.Visible = FindByTextWithDiacriticsCheckBox.Checked;
        FindByTextElfMedLabel.Visible = FindByTextWithDiacriticsCheckBox.Checked;
        FindByTextHamzaAboveWawLabel.Visible = FindByTextWithDiacriticsCheckBox.Checked;
        FindByTextHamzaAboveYaaLabel.Visible = FindByTextWithDiacriticsCheckBox.Checked;

        FindByTextWithDiacriticsCheckBox.Text = "A";
        ToolTip.SetToolTip(FindByTextWithDiacriticsCheckBox, "All texts inclduing emlaai  مع الإملائي");

        if (text_mode == "Simplified28")
        {
            // do nothing
        }
        else if (text_mode == "Simplified29")
        {
            FindByTextHamzaLabel.Visible = true;
        }
        else if (text_mode == "Simplified30")
        {
            FindByTextTaaMarbootaLabel.Visible = true;
            FindByTextElfMaqsuraLabel.Visible = true;
        }
        else if (text_mode == "Simplified31")
        {
            FindByTextHamzaLabel.Visible = true;
            FindByTextTaaMarbootaLabel.Visible = true;
            FindByTextElfMaqsuraLabel.Visible = true;
        }
        else if (text_mode == "Simplified37")
        {
            FindByTextHamzaLabel.Visible = true;

            FindByTextTaaMarbootaLabel.Visible = true;
            FindByTextElfMaqsuraLabel.Visible = true;

            FindByTextElfWaslLabel.Visible = true;
            FindByTextHamzaAboveElfLabel.Visible = true;
            FindByTextHamzaBelowElfLabel.Visible = true;
            FindByTextElfMedLabel.Visible = true;
            FindByTextHamzaAboveWawLabel.Visible = true;
            FindByTextHamzaAboveYaaLabel.Visible = true;
        }
        else if (text_mode == "Original")
        {
            FindByTextHamzaLabel.Visible = true;

            FindByTextTaaMarbootaLabel.Visible = true;
            FindByTextElfMaqsuraLabel.Visible = true;

            FindByTextElfWaslLabel.Visible = true;
            FindByTextHamzaAboveElfLabel.Visible = true;
            FindByTextHamzaBelowElfLabel.Visible = true;
            FindByTextElfMedLabel.Visible = true;
            FindByTextHamzaAboveWawLabel.Visible = true;
            FindByTextHamzaAboveYaaLabel.Visible = true;

            FindByTextWithDiacriticsCheckBox.Text = "ā";
            ToolTip.SetToolTip(FindByTextWithDiacriticsCheckBox, "with diacritics  مع الحركات");
        }
        else
        {
            // do nothing
        }
    }
    private void EnableFindByTextControls()
    {
        FindByTextExactSearchTypeLabel.BackColor = (m_text_search_type == TextSearchType.Exact) ? Color.SteelBlue : Color.DarkGray;
        FindByTextProximitySearchTypeLabel.BackColor = (m_text_search_type == TextSearchType.Proximity) ? Color.SteelBlue : Color.DarkGray;
        FindByTextRootSearchTypeLabel.BackColor = (m_text_search_type == TextSearchType.Root) ? Color.SteelBlue : Color.DarkGray;
        FindByTextExactSearchTypeLabel.BorderStyle = (m_text_search_type == TextSearchType.Exact) ? BorderStyle.Fixed3D : BorderStyle.None;
        FindByTextProximitySearchTypeLabel.BorderStyle = (m_text_search_type == TextSearchType.Proximity) ? BorderStyle.Fixed3D : BorderStyle.None;
        FindByTextRootSearchTypeLabel.BorderStyle = (m_text_search_type == TextSearchType.Root) ? BorderStyle.Fixed3D : BorderStyle.None;

        FindByTextAtVerseStartRadioButton.Enabled = (m_text_search_type == TextSearchType.Exact);
        FindByTextAtVerseMiddleRadioButton.Enabled = (m_text_search_type == TextSearchType.Exact);
        FindByTextAtVerseEndRadioButton.Enabled = (m_text_search_type == TextSearchType.Exact);
        FindByTextAtVerseAnywhereRadioButton.Enabled = (m_text_search_type == TextSearchType.Exact);

        FindByTextAllWordsRadioButton.Enabled = (m_text_search_type == TextSearchType.Proximity);
        FindByTextAnyWordRadioButton.Enabled = (m_text_search_type == TextSearchType.Proximity)
                                                && (!FindByTextTextBox.Text.Contains("-"))
                                                && (!FindByTextTextBox.Text.Contains("+"));
        FindByTextPlusLabel.Visible = ((m_text_search_type == TextSearchType.Proximity) || (m_text_search_type == TextSearchType.Root));
        FindByTextMinusLabel.Visible = ((m_text_search_type == TextSearchType.Proximity) || (m_text_search_type == TextSearchType.Root));

        FindByTextWordnessCheckBox.Enabled = ((m_text_search_type == TextSearchType.Exact) || (m_text_search_type == TextSearchType.Proximity));

        FindByTextAtWordStartRadioButton.Enabled = (m_text_search_type == TextSearchType.Exact);
        FindByTextAtWordMiddleRadioButton.Enabled = (m_text_search_type == TextSearchType.Exact);
        FindByTextAtWordEndRadioButton.Enabled = (m_text_search_type == TextSearchType.Exact);
        FindByTextAtWordAnywhereRadioButton.Enabled = (m_text_search_type == TextSearchType.Exact);

        FindByTextMultiplicityCheckBox.Enabled = ((m_text_search_type == TextSearchType.Exact) || (m_text_search_type == TextSearchType.Root));
        FindByTextMultiplicityNumericUpDown.Enabled = (FindByTextMultiplicityCheckBox.Enabled) && (FindByTextMultiplicityCheckBox.Checked);

        FindByTextCaseSensitiveCheckBox.Enabled = (m_language_type == LanguageType.LeftToRight);
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 18. Search By Similarity
    ///////////////////////////////////////////////////////////////////////////////
    private SimilaritySearchSource m_similarity_search_source = SimilaritySearchSource.CurrentVerse;
    private void FindBySimilarityCurrentVerseTypeLabel_Click(object sender, EventArgs e)
    {
        m_similarity_search_source = SimilaritySearchSource.CurrentVerse;
        FindBySimilarityPercentageTrackBar.Value = 73;
        FindBySimilarityControls_Enter(null, null);
    }
    private void FindBySimilarityAllVersesTypeLabel_Click(object sender, EventArgs e)
    {
        m_similarity_search_source = SimilaritySearchSource.AllVerses;
        FindBySimilarityPercentageTrackBar.Value = 100;
        FindBySimilarityControls_Enter(null, null);
    }
    private void FindBySimilarityRadioButton_CheckedChanged(object sender, EventArgs e)
    {
        //if (m_similarity_search_source == SimilaritySearchSource.CurrentVerse)
        //{
        //    FindBySimilarityButton_Click(null, null);
        //}
    }
    private void FindBySimilarityPercentageTrackBar_ValueChanged(object sender, EventArgs e)
    {
        FindBySimilarityButton.Text = FindBySimilarityPercentageTrackBar.Value.ToString() + "% Find";
        //if (m_similarity_search_source == SimilaritySearchSource.CurrentVerse)
        //{
        //    FindBySimilarityButton_Click(null, null);
        //}
    }
    private void FindBySimilarityControls_Enter(object sender, EventArgs e)
    {
        this.AcceptButton = FindBySimilarityButton;

        FindByTextButton.Enabled = false;
        FindBySimilarityButton.Enabled = true;
        FindByNumbersButton.Enabled = false;
        FindByRevelationPlaceButton.Enabled = false;
        FindByProstrationTypeButton.Enabled = false;
        FindByFrequencyButton.Enabled = false;

        AutoCompleteHeaderLabel.Visible = false;
        AutoCompleteListBox.Visible = false;

        ResetFindByTextResultTypeLabels();
        ResetFindBySimilarityResultTypeLabels();
        ResetFindByNumbersResultTypeLabels();
        ResetFindByFrequencyResultTypeLabels();

        switch (m_similarity_search_source)
        {
            case SimilaritySearchSource.CurrentVerse:
                {
                    FindBySimilarityCurrentVerseTypeLabel.BackColor = Color.SteelBlue;
                    FindBySimilarityCurrentVerseTypeLabel.BorderStyle = BorderStyle.Fixed3D;
                }
                break;
            case SimilaritySearchSource.AllVerses:
                {
                    FindBySimilarityAllVersesTypeLabel.BackColor = Color.SteelBlue;
                    FindBySimilarityAllVersesTypeLabel.BorderStyle = BorderStyle.Fixed3D;
                }
                break;
        }
    }
    private void ResetFindBySimilarityResultTypeLabels()
    {
        FindBySimilarityCurrentVerseTypeLabel.BackColor = Color.DarkGray;
        FindBySimilarityCurrentVerseTypeLabel.BorderStyle = BorderStyle.None;
        FindBySimilarityAllVersesTypeLabel.BackColor = Color.DarkGray;
        FindBySimilarityAllVersesTypeLabel.BorderStyle = BorderStyle.None;
    }
    private void FindBySimilarityButton_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            FindBySimilarity();
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void FindBySimilarity()
    {
        m_search_type = SearchType.Similarity;

        if (m_client != null)
        {
            ClearFindMatches();

            SimilarityMethod find_by_similarity_method = SimilarityMethod.SimilarText;
            if (FindBySimilaritySimilarTextRadioButton.Checked)
            {
                find_by_similarity_method = SimilarityMethod.SimilarText;
            }
            else if (FindBySimilaritySimilarWordsRadioButton.Checked)
            {
                find_by_similarity_method = SimilarityMethod.SimilarWords;
            }
            else if (FindBySimilaritySimilarFirstHalfRadioButton.Checked)
            {
                find_by_similarity_method = SimilarityMethod.SimilarFirstHalf;
            }
            else if (FindBySimilaritySimilarLastHalfRadioButton.Checked)
            {
                find_by_similarity_method = SimilarityMethod.SimilarLastHalf;
            }
            else if (FindBySimilaritySimilarFirstWordRadioButton.Checked)
            {
                find_by_similarity_method = SimilarityMethod.SimilarFirstWord;
            }
            else if (FindBySimilaritySimilarLastWordRadioButton.Checked)
            {
                find_by_similarity_method = SimilarityMethod.SimilarLastWord;
            }
            else
            {
                //
            }

            double similarity_percentage = (double)FindBySimilarityPercentageTrackBar.Value / 100.0D;

            string similarity_search_source = null;
            if (m_similarity_search_source == SimilaritySearchSource.CurrentVerse)
            {
                Verse verse = GetVerse(CurrentVerseIndex);
                if (verse != null)
                {
                    if (verse.Chapter != null)
                    {
                        m_client.FindVerses(verse, find_by_similarity_method, similarity_percentage);
                        similarity_search_source = " to verse " + verse.Chapter.Name + " " + verse.NumberInChapter + " ";
                    }
                }
                if (m_client.FoundVerses != null)
                {
                    int verse_count = m_client.FoundVerses.Count;
                    m_find_result_header = verse_count + ((verse_count == 1) ? " verse" : " verses") + " with " + find_by_similarity_method.ToString() + similarity_search_source + " in " + m_client.SearchScope.ToString();

                    DisplayFoundVerses(true, true);
                }
            }
            else if (m_similarity_search_source == SimilaritySearchSource.AllVerses)
            {
                m_client.FindVersess(find_by_similarity_method, similarity_percentage);
                similarity_search_source = null;

                if (m_client.FoundVerses != null)
                {
                    int verse_count = m_client.FoundVerses.Count;
                    m_find_result_header = verse_count + ((verse_count == 1) ? " verse" : " verses") + " with " + find_by_similarity_method.ToString() + similarity_search_source + " in " + m_client.SearchScope.ToString();

                    DisplayFoundVerseRanges(true, true);
                }
            }
            else
            {
                //
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 19. Search By Numbers
    ///////////////////////////////////////////////////////////////////////////////
    private NumbersResultType m_numbers_result_type = NumbersResultType.Verses;
    private bool m_find_by_numbers_sets = false;
    private void FindByNumbersSetsCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_find_by_numbers_sets = FindByNumbersSetsCheckBox.Checked;
    }
    private void FindByNumbersLabel_Click(object sender, EventArgs e)
    {
        FindByNumbersControls_Enter(null, null);
    }
    private void FindByNumbersResultTypeWordsLabel_Click(object sender, EventArgs e)
    {
        m_numbers_result_type = NumbersResultType.Words;
        //                           num   Cs     Vs     Ws    Ls    uLs   value dsum  droot
        EnableFindByNumbersControls(true, false, false, true, true, true, true, true, true);

        FindByNumbersControls_Enter(null, null);
        FindByNumbersNumberNumericUpDown.Value = 0;
        FindByNumbersChaptersNumericUpDown.Value = 0;
        FindByNumbersVersesNumericUpDown.Value = 0;
        FindByNumbersWordsNumericUpDown.Value = 1;
        FindByNumbersLettersNumericUpDown.Focus();
    }
    private void FindByNumbersResultTypeSentencesLabel_Click(object sender, EventArgs e)
    {
        m_numbers_result_type = NumbersResultType.Sentences;
        //                           num   Cs     Vs     Ws    Ls    uLs   value dsum  droot
        EnableFindByNumbersControls(true, false, false, true, true, true, true, true, true);

        FindByNumbersControls_Enter(null, null);
        FindByNumbersNumberNumericUpDown.Value = 0;
        FindByNumbersChaptersNumericUpDown.Value = 0;
        FindByNumbersVersesNumericUpDown.Value = 0;
        FindByNumbersWordsNumericUpDown.Value = 0; // must be 0 for any sentence length
        FindByNumbersWordsNumericUpDown.Focus();
    }
    private void FindByNumbersResultTypeVersesLabel_Click(object sender, EventArgs e)
    {
        m_numbers_result_type = NumbersResultType.Verses;
        //                           num   Cs     Vs    Ws    Ls    uLs   value dsum  droot
        EnableFindByNumbersControls(true, false, true, true, true, true, true, true, true);

        FindByNumbersControls_Enter(null, null);
        FindByNumbersNumberNumericUpDown.Value = 0;
        FindByNumbersChaptersNumericUpDown.Value = 0;
        FindByNumbersVersesNumericUpDown.Value = 1;
        FindByNumbersWordsNumericUpDown.Value = 0;
        FindByNumbersWordsNumericUpDown.Focus();
    }
    private void FindByNumbersResultTypeChaptersLabel_Click(object sender, EventArgs e)
    {
        m_numbers_result_type = NumbersResultType.Chapters;
        //                           num   Cs    Vs    Ws    Ls    uLs   value dsum  droot
        EnableFindByNumbersControls(true, true, true, true, true, true, true, true, true);

        FindByNumbersControls_Enter(null, null);
        FindByNumbersNumberNumericUpDown.Value = 0;
        FindByNumbersChaptersNumericUpDown.Value = 1;
        FindByNumbersVersesNumericUpDown.Value = 0;
        FindByNumbersWordsNumericUpDown.Value = 0;
        FindByNumbersVersesNumericUpDown.Focus();
    }
    private void EnableFindByNumbersControls(
                                                bool enable_number,
                                                bool enable_chapters,
                                                bool enable_verses,
                                                bool enable_words,
                                                bool enable_letters,
                                                bool enable_unique_letters,
                                                bool enable_value,
                                                bool enable_value_digit_sum,
                                                bool enable_value_digital_root
                                            )
    {
        bool not_number_number_type = ((FindByNumbersNumberNumberTypeLabel.Text.Length == 0) || (Char.IsDigit(FindByNumbersNumberNumberTypeLabel.Text[0])));
        FindByNumbersNumberLabel.Enabled = enable_number;
        FindByNumbersNumberComparisonOperatorLabel.Enabled = enable_number && not_number_number_type;
        FindByNumbersNumberNumericUpDown.Enabled = enable_number && not_number_number_type;
        FindByNumbersNumberNumberTypeLabel.Enabled = enable_number;
        if (enable_number == false)
        {
            FindByNumbersNumberComparisonOperatorLabel.Text = "=";
            FindByNumbersNumberNumericUpDown.Value = 0;
        }

        bool not_chapters_number_type = ((FindByNumbersChaptersNumberTypeLabel.Text.Length == 0) || (Char.IsDigit(FindByNumbersChaptersNumberTypeLabel.Text[0])));
        FindByNumbersChaptersLabel.Enabled = enable_chapters;
        FindByNumbersChaptersComparisonOperatorLabel.Enabled = enable_chapters && not_chapters_number_type;
        FindByNumbersChaptersNumericUpDown.Enabled = enable_chapters && not_chapters_number_type;
        FindByNumbersChaptersNumberTypeLabel.Enabled = enable_chapters;
        if (enable_chapters == false)
        {
            FindByNumbersChaptersComparisonOperatorLabel.Text = "=";
            FindByNumbersChaptersNumericUpDown.Value = 0;
        }

        bool not_verses_number_type = ((FindByNumbersVersesNumberTypeLabel.Text.Length == 0) || (Char.IsDigit(FindByNumbersVersesNumberTypeLabel.Text[0])));
        FindByNumbersVersesLabel.Enabled = enable_verses;
        FindByNumbersVersesComparisonOperatorLabel.Enabled = enable_verses && not_verses_number_type;
        FindByNumbersVersesNumericUpDown.Enabled = enable_verses && not_verses_number_type;
        FindByNumbersVersesNumberTypeLabel.Enabled = enable_verses;
        if (enable_verses == false)
        {
            FindByNumbersVersesComparisonOperatorLabel.Text = "=";
            FindByNumbersVersesNumericUpDown.Value = 0;
        }

        bool not_words_number_type = ((FindByNumbersWordsNumberTypeLabel.Text.Length == 0) || (Char.IsDigit(FindByNumbersWordsNumberTypeLabel.Text[0])));
        FindByNumbersWordsLabel.Enabled = enable_words;
        FindByNumbersWordsComparisonOperatorLabel.Enabled = enable_words && not_words_number_type;
        FindByNumbersWordsNumericUpDown.Enabled = enable_words && not_words_number_type;
        FindByNumbersWordsNumberTypeLabel.Enabled = enable_words;
        if (enable_words == false)
        {
            FindByNumbersWordsComparisonOperatorLabel.Text = "=";
            FindByNumbersWordsNumericUpDown.Value = 0;
        }

        bool not_letters_number_type = ((FindByNumbersLettersNumberTypeLabel.Text.Length == 0) || (Char.IsDigit(FindByNumbersLettersNumberTypeLabel.Text[0])));
        FindByNumbersLettersLabel.Enabled = enable_letters;
        FindByNumbersLettersComparisonOperatorLabel.Enabled = enable_letters && not_letters_number_type;
        FindByNumbersLettersNumericUpDown.Enabled = enable_letters && not_letters_number_type;
        FindByNumbersLettersNumberTypeLabel.Enabled = enable_letters;
        if (enable_letters == false)
        {
            FindByNumbersLettersComparisonOperatorLabel.Text = "=";
            FindByNumbersLettersNumericUpDown.Value = 0;
        }

        bool not_unique_letters_number_type = ((FindByNumbersUniqueLettersNumberTypeLabel.Text.Length == 0) || (Char.IsDigit(FindByNumbersUniqueLettersNumberTypeLabel.Text[0])));
        FindByNumbersUniqueLettersLabel.Enabled = enable_unique_letters;
        FindByNumbersUniqueLettersComparisonOperatorLabel.Enabled = enable_unique_letters && not_unique_letters_number_type;
        FindByNumbersUniqueLettersNumericUpDown.Enabled = enable_unique_letters && not_unique_letters_number_type;
        FindByNumbersUniqueLettersNumberTypeLabel.Enabled = enable_unique_letters;
        if (enable_unique_letters == false)
        {
            FindByNumbersUniqueLettersComparisonOperatorLabel.Text = "=";
            FindByNumbersUniqueLettersNumericUpDown.Value = 0;
        }

        bool not_value_number_type = ((FindByNumbersValueNumberTypeLabel.Text.Length == 0) || (Char.IsDigit(FindByNumbersValueNumberTypeLabel.Text[0])));
        FindByNumbersValueLabel.Enabled = enable_value;
        FindByNumbersValueComparisonOperatorLabel.Enabled = enable_value && not_value_number_type;
        FindByNumbersValueNumericUpDown.Enabled = enable_value && not_value_number_type;
        FindByNumbersValueNumberTypeLabel.Enabled = enable_value;
        if (enable_value == false)
        {
            FindByNumbersValueComparisonOperatorLabel.Text = "=";
            FindByNumbersValueNumericUpDown.Value = 0;
        }

        bool not_value_digit_sum_number_type = ((FindByNumbersValueDigitSumNumberTypeLabel.Text.Length == 0) || (Char.IsDigit(FindByNumbersValueDigitSumNumberTypeLabel.Text[0])));
        FindByNumbersValueDigitSumLabel.Enabled = enable_value_digit_sum;
        FindByNumbersValueDigitSumComparisonOperatorLabel.Enabled = enable_value_digit_sum && not_value_digit_sum_number_type;
        FindByNumbersValueDigitSumNumericUpDown.Enabled = enable_value_digit_sum && not_value_digit_sum_number_type;
        FindByNumbersValueDigitSumNumberTypeLabel.Enabled = enable_value_digit_sum;
        if (enable_value_digit_sum == false)
        {
            FindByNumbersValueDigitSumComparisonOperatorLabel.Text = "=";
            FindByNumbersValueDigitSumNumericUpDown.Value = 0;
        }

        bool not_value_digital_root_number_type = ((FindByNumbersValueDigitalRootNumberTypeLabel.Text.Length == 0) || (Char.IsDigit(FindByNumbersValueDigitalRootNumberTypeLabel.Text[0])));
        FindByNumbersValueDigitalRootLabel.Enabled = enable_value_digital_root;
        FindByNumbersValueDigitalRootComparisonOperatorLabel.Enabled = enable_value_digital_root && not_value_digital_root_number_type;
        FindByNumbersValueDigitalRootNumericUpDown.Enabled = enable_value_digital_root && not_value_digital_root_number_type;
        FindByNumbersValueDigitalRootNumberTypeLabel.Enabled = enable_value_digital_root;
        if (enable_value_digital_root == false)
        {
            FindByNumbersValueDigitalRootComparisonOperatorLabel.Text = "=";
            FindByNumbersValueDigitalRootNumericUpDown.Value = 0;
        }
    }
    private void ResetFindByNumbersControls()
    {
        FindByNumbersNumberComparisonOperatorLabel.Text = "=";
        FindByNumbersNumberNumericUpDown.Value = 0;
        FindByNumbersNumberNumberTypeLabel.Text = null;

        FindByNumbersChaptersComparisonOperatorLabel.Text = "=";
        FindByNumbersChaptersNumericUpDown.Value = 0;
        FindByNumbersChaptersNumberTypeLabel.Text = null;

        FindByNumbersVersesComparisonOperatorLabel.Text = "=";
        FindByNumbersVersesNumericUpDown.Value = 0;
        FindByNumbersVersesNumberTypeLabel.Text = null;

        FindByNumbersWordsComparisonOperatorLabel.Text = "=";
        FindByNumbersWordsNumericUpDown.Value = 0;
        FindByNumbersWordsNumberTypeLabel.Text = null;

        FindByNumbersLettersComparisonOperatorLabel.Text = "=";
        FindByNumbersLettersNumericUpDown.Value = 0;
        FindByNumbersLettersNumberTypeLabel.Text = null;

        FindByNumbersUniqueLettersComparisonOperatorLabel.Text = "=";
        FindByNumbersUniqueLettersNumericUpDown.Value = 0;
        FindByNumbersUniqueLettersNumberTypeLabel.Text = null;

        FindByNumbersValueComparisonOperatorLabel.Text = "=";
        FindByNumbersValueNumericUpDown.Value = 0;
        FindByNumbersValueNumberTypeLabel.Text = null;

        FindByNumbersValueDigitSumComparisonOperatorLabel.Text = "=";
        FindByNumbersValueDigitSumNumericUpDown.Value = 0;
        FindByNumbersValueDigitSumNumberTypeLabel.Text = null;

        FindByNumbersValueDigitalRootComparisonOperatorLabel.Text = "=";
        FindByNumbersValueDigitalRootNumericUpDown.Value = 0;
        FindByNumbersValueDigitalRootNumberTypeLabel.Text = null;
    }
    private void ResetFindByNumbersNumberTypeControl(Control control)
    {
        if (control != null)
        {
            control.Text = null;
            control.ForeColor = Color.Black;
            ToolTip.SetToolTip(control, null);
        }
    }
    private void ResetFindByNumbersResultTypeLabels()
    {
        FindByNumbersResultTypeWordsLabel.BackColor = Color.DarkGray;
        FindByNumbersResultTypeWordsLabel.BorderStyle = BorderStyle.None;

        FindByNumbersResultTypeSentencesLabel.BackColor = Color.DarkGray;
        FindByNumbersResultTypeSentencesLabel.BorderStyle = BorderStyle.None;

        FindByNumbersResultTypeVersesLabel.BackColor = Color.DarkGray;
        FindByNumbersResultTypeVersesLabel.BorderStyle = BorderStyle.None;

        FindByNumbersResultTypeChaptersLabel.BackColor = Color.DarkGray;
        FindByNumbersResultTypeChaptersLabel.BorderStyle = BorderStyle.None;

        UpdateFindByNumbersResultType();
    }
    private void ResetFindByNumbersComparisonOperatorLabels()
    {
        FindByNumbersNumberComparisonOperatorLabel.Text = "=";
        FindByNumbersChaptersComparisonOperatorLabel.Text = "=";
        FindByNumbersVersesComparisonOperatorLabel.Text = "=";
        FindByNumbersWordsComparisonOperatorLabel.Text = "=";
        FindByNumbersLettersComparisonOperatorLabel.Text = "=";
        FindByNumbersUniqueLettersComparisonOperatorLabel.Text = "=";
        FindByNumbersValueComparisonOperatorLabel.Text = "=";
        FindByNumbersValueDigitSumComparisonOperatorLabel.Text = "=";
        FindByNumbersValueDigitalRootComparisonOperatorLabel.Text = "=";

        FindByNumbersNumberComparisonOperatorLabel.Enabled = false;
        FindByNumbersChaptersComparisonOperatorLabel.Enabled = false;
        FindByNumbersVersesComparisonOperatorLabel.Enabled = false;
        FindByNumbersWordsComparisonOperatorLabel.Enabled = false;
        FindByNumbersLettersComparisonOperatorLabel.Enabled = false;
        FindByNumbersUniqueLettersComparisonOperatorLabel.Enabled = false;
        FindByNumbersValueComparisonOperatorLabel.Enabled = false;
        FindByNumbersValueDigitSumComparisonOperatorLabel.Enabled = false;
        FindByNumbersValueDigitalRootComparisonOperatorLabel.Enabled = false;
    }
    private void UpdateFindByNumbersResultType()
    {
        switch (m_numbers_result_type)
        {
            case NumbersResultType.Words:
            case NumbersResultType.WordRanges:
            case NumbersResultType.WordSets:
                {
                    if ((FindByNumbersWordsNumericUpDown.Value == 1) && (FindByNumbersWordsNumberTypeLabel.Text.Length == 0))
                    {
                        m_numbers_result_type = NumbersResultType.Words;
                    }
                    else if ((FindByNumbersWordsNumericUpDown.Value != 1) || (FindByNumbersWordsNumberTypeLabel.Text.Length > 0))
                    {
                        if (!m_find_by_numbers_sets)
                        {
                            m_numbers_result_type = NumbersResultType.WordRanges;
                        }
                        else
                        {
                            m_numbers_result_type = NumbersResultType.WordSets;
                        }
                    }
                }
                break;
            case NumbersResultType.Sentences:
                {
                    m_numbers_result_type = NumbersResultType.Sentences;
                }
                break;
            case NumbersResultType.Verses:
            case NumbersResultType.VerseRanges:
            case NumbersResultType.VerseSets:
                {
                    if ((FindByNumbersVersesNumericUpDown.Value == 1) && (FindByNumbersVersesNumberTypeLabel.Text.Length == 0))
                    {
                        m_numbers_result_type = NumbersResultType.Verses;
                    }
                    else if ((FindByNumbersVersesNumericUpDown.Value != 1) || (FindByNumbersVersesNumberTypeLabel.Text.Length > 0))
                    {
                        if (!m_find_by_numbers_sets)
                        {
                            m_numbers_result_type = NumbersResultType.VerseRanges;
                        }
                        else
                        {
                            m_numbers_result_type = NumbersResultType.VerseSets;
                        }
                    }
                }
                break;
            case NumbersResultType.Chapters:
            case NumbersResultType.ChapterRanges:
            case NumbersResultType.ChapterSets:
                {
                    if ((FindByNumbersChaptersNumericUpDown.Value == 1) && (FindByNumbersChaptersNumberTypeLabel.Text.Length == 0))
                    {
                        m_numbers_result_type = NumbersResultType.Chapters;
                    }
                    else if ((FindByNumbersChaptersNumericUpDown.Value != 1) || (FindByNumbersChaptersNumberTypeLabel.Text.Length > 0))
                    {
                        if (!m_find_by_numbers_sets)
                        {
                            m_numbers_result_type = NumbersResultType.ChapterRanges;
                        }
                        else
                        {
                            m_numbers_result_type = NumbersResultType.ChapterSets;
                        }
                    }
                }
                break;
            default:
                break;
        }


        // reset Words label
        FindByNumbersResultTypeWordsLabel.Text = "W";
        ToolTip.SetToolTip(FindByNumbersResultTypeWordsLabel, "find words");
        FindByNumbersNumberLabel.Text = "number";
        ToolTip.SetToolTip(FindByNumbersNumberLabel, "word number in chapter");
        // reset Sentences label
        FindByNumbersResultTypeSentencesLabel.Text = "S";
        ToolTip.SetToolTip(FindByNumbersResultTypeSentencesLabel, "find sentences across verses");
        // reset Verses label
        FindByNumbersResultTypeVersesLabel.Text = "V";
        ToolTip.SetToolTip(FindByNumbersResultTypeVersesLabel, "find verses");
        FindByNumbersNumberLabel.Text = "number";
        ToolTip.SetToolTip(FindByNumbersNumberLabel, "verse number");
        // reset Chapters label
        FindByNumbersResultTypeChaptersLabel.Text = "C";
        ToolTip.SetToolTip(FindByNumbersResultTypeChaptersLabel, "find chapters");
        FindByNumbersNumberLabel.Text = "number";
        ToolTip.SetToolTip(FindByNumbersNumberLabel, "chapter number");

        // overwrite label and tooltips
        if (m_numbers_result_type == NumbersResultType.Words)
        {
            FindByNumbersResultTypeWordsLabel.Text = "W";
            ToolTip.SetToolTip(FindByNumbersResultTypeWordsLabel, "find words");
            FindByNumbersNumberLabel.Text = "number";
            ToolTip.SetToolTip(FindByNumbersNumberLabel, "word number in chapter");
        }
        if (m_numbers_result_type == NumbersResultType.WordRanges)
        {
            FindByNumbersResultTypeWordsLabel.Text = "-W-";
            ToolTip.SetToolTip(FindByNumbersResultTypeWordsLabel, "find word ranges");
            FindByNumbersNumberLabel.Text = "sum";
            ToolTip.SetToolTip(FindByNumbersNumberLabel, "sum of word numbers in chapter");
        }
        if (m_numbers_result_type == NumbersResultType.WordSets)
        {
            FindByNumbersResultTypeWordsLabel.Text = "Ws";
            ToolTip.SetToolTip(FindByNumbersResultTypeWordsLabel, "find word sets");
            FindByNumbersNumberLabel.Text = "sum";
            ToolTip.SetToolTip(FindByNumbersNumberLabel, "sum of word numbers in chapter");
        }
        else if (m_numbers_result_type == NumbersResultType.Verses)
        {
            FindByNumbersResultTypeVersesLabel.Text = "V";
            ToolTip.SetToolTip(FindByNumbersResultTypeVersesLabel, "find verses");
            FindByNumbersNumberLabel.Text = "number";
            ToolTip.SetToolTip(FindByNumbersNumberLabel, "verse number");
        }
        else if (m_numbers_result_type == NumbersResultType.VerseRanges)
        {
            FindByNumbersResultTypeVersesLabel.Text = "-V-";
            ToolTip.SetToolTip(FindByNumbersResultTypeVersesLabel, "find verse ranges");
            FindByNumbersNumberLabel.Text = "sum";
            ToolTip.SetToolTip(FindByNumbersNumberLabel, "sum of verse numbers");
        }
        else if (m_numbers_result_type == NumbersResultType.VerseSets)
        {
            FindByNumbersResultTypeVersesLabel.Text = "Vs";
            ToolTip.SetToolTip(FindByNumbersResultTypeVersesLabel, "find verse sets");
            FindByNumbersNumberLabel.Text = "sum";
            ToolTip.SetToolTip(FindByNumbersNumberLabel, "sum of verse numbers");
        }
        else if (m_numbers_result_type == NumbersResultType.Chapters)
        {
            FindByNumbersResultTypeChaptersLabel.Text = "C";
            ToolTip.SetToolTip(FindByNumbersResultTypeChaptersLabel, "find chapters");
            FindByNumbersNumberLabel.Text = "number";
            ToolTip.SetToolTip(FindByNumbersNumberLabel, "chapter number");
        }
        else if (m_numbers_result_type == NumbersResultType.ChapterRanges)
        {
            FindByNumbersResultTypeChaptersLabel.Text = "-C-";
            ToolTip.SetToolTip(FindByNumbersResultTypeChaptersLabel, "find chapter ranges");
            FindByNumbersNumberLabel.Text = "sum";
            ToolTip.SetToolTip(FindByNumbersNumberLabel, "sum of chapter numbers");
        }
        else if (m_numbers_result_type == NumbersResultType.ChapterSets)
        {
            FindByNumbersResultTypeChaptersLabel.Text = "Cs";
            ToolTip.SetToolTip(FindByNumbersResultTypeChaptersLabel, "find chapter sets");
            FindByNumbersNumberLabel.Text = "sum";
            ToolTip.SetToolTip(FindByNumbersNumberLabel, "sum of chapter numbers");
        }

        switch (m_numbers_result_type)
        {
            case NumbersResultType.Words:
            case NumbersResultType.WordRanges:
            case NumbersResultType.WordSets:
                {
                    FindByNumbersWordsComparisonOperatorLabel.Text = "=";
                    FindByNumbersWordsComparisonOperatorLabel.Enabled = false;
                    FindByNumbersWordsNumberTypeLabel.Text = "";
                    FindByNumbersWordsNumberTypeLabel.Enabled = false;
                }
                break;
            case NumbersResultType.Sentences:
                {
                    FindByNumbersNumberLabel.Enabled = false;
                    FindByNumbersNumberComparisonOperatorLabel.Enabled = false;
                    FindByNumbersNumberNumericUpDown.Enabled = false;
                    FindByNumbersNumberNumberTypeLabel.Enabled = false;
                    FindByNumbersNumberComparisonOperatorLabel.Text = "=";
                    FindByNumbersNumberNumericUpDown.Value = 0;
                }
                break;
            case NumbersResultType.Verses:
            case NumbersResultType.VerseRanges:
            case NumbersResultType.VerseSets:
                {
                    FindByNumbersVersesComparisonOperatorLabel.Text = "=";
                    FindByNumbersVersesComparisonOperatorLabel.Enabled = false;
                    FindByNumbersVersesNumberTypeLabel.Text = "";
                    FindByNumbersVersesNumberTypeLabel.Enabled = false;
                }
                break;
            case NumbersResultType.Chapters:
            case NumbersResultType.ChapterRanges:
            case NumbersResultType.ChapterSets:
                {
                    FindByNumbersChaptersComparisonOperatorLabel.Text = "=";
                    FindByNumbersChaptersComparisonOperatorLabel.Enabled = false;
                    FindByNumbersChaptersNumberTypeLabel.Text = "";
                    FindByNumbersChaptersNumberTypeLabel.Enabled = false;
                }
                break;
            default:
                break;
        }
    }
    private bool UpdateComparisonOperator(Control control)
    {
        if (control == null) return false;

        if (ModifierKeys == Keys.Shift)
        {
            if (control.Text == "=")
            {
                control.Text = "%";
                ToolTip.SetToolTip(control, "divisible by (with remainder)");
            }
            else if (control.Text == "%")
            {
                control.Text = "X";
                ToolTip.SetToolTip(control, "multiple of");
            }
            else if (control.Text == "X")
            {
                control.Text = "≥";
                ToolTip.SetToolTip(control, "greater than or equals to");
            }
            else if (control.Text == "≠")
            {
                control.Text = "=";
                ToolTip.SetToolTip(control, "equals to");
            }
            else if (control.Text == "<")
            {
                control.Text = "≠";
                ToolTip.SetToolTip(control, "not equals to");
            }
            else if (control.Text == "≤")
            {
                control.Text = "<";
                ToolTip.SetToolTip(control, "less than");
            }
            else if (control.Text == ">")
            {
                control.Text = "≤";
                ToolTip.SetToolTip(control, "less than or equals to");
            }
            else if (control.Text == "≥")
            {
                control.Text = ">";
                ToolTip.SetToolTip(control, "greater than");
            }
            else
            {
                // do nothing
            }
        }
        else
        {
            if (control.Text == "=")
            {
                control.Text = "≠";
                ToolTip.SetToolTip(control, "not equals to");
            }
            else if (control.Text == "≠")
            {
                control.Text = "<";
                ToolTip.SetToolTip(control, "less than");
            }
            else if (control.Text == "<")
            {
                control.Text = "≤";
                ToolTip.SetToolTip(control, "less than or equals to");
            }
            else if (control.Text == "≤")
            {
                control.Text = ">";
                ToolTip.SetToolTip(control, "greater than");
            }
            else if (control.Text == ">")
            {
                control.Text = "≥";
                ToolTip.SetToolTip(control, "greater than or equals to");
            }
            else if (control.Text == "≥")
            {
                control.Text = "X";
                ToolTip.SetToolTip(control, "multiple of");
            }
            else if (control.Text == "X")
            {
                control.Text = "%";
                ToolTip.SetToolTip(control, "divisible by (with remainder)");
            }
            else if (control.Text == "%")
            {
                control.Text = "=";
                ToolTip.SetToolTip(control, "equals to");
            }
            else
            {
                // do nothing
            }
        }
        return true;
    }
    private bool UpdateNumberType(Control control)
    {
        if (control == null) return false;

        // inc/dec remainder
        int remainder;
        if (int.TryParse(control.Text, out remainder))
        {
            if (control.Tag != null)
            {
                int max;
                if (int.TryParse(control.Tag.ToString(), out max))
                {
                    int max_remainder = max - 1;
                    if (max_remainder < -1) max_remainder = -1;

                    if (ModifierKeys != Keys.Shift)
                    {
                        remainder++;
                        if (remainder > max_remainder) remainder = -1;
                    }
                    else
                    {
                        remainder--;
                        if (remainder < -1) remainder = max_remainder;
                    }

                    control.Text = remainder.ToString();
                }
            }
            return false;
        }
        else
        {
            if (ModifierKeys != Keys.Shift)
            {
                if (control.Text == "")
                {
                    control.Text = "P";
                    control.ForeColor = GetNumberTypeColor(19);
                    ToolTip.SetToolTip(control, "prime = divisible by itself and 1 only");
                }
                else if (control.Text == "P")
                {
                    control.Text = "AP";
                    control.ForeColor = GetNumberTypeColor(47);
                    ToolTip.SetToolTip(control, "additive prime = prime with prime digit sum");
                }
                else if (control.Text == "AP")
                {
                    control.Text = "PP";
                    control.ForeColor = GetNumberTypeColor(313);
                    ToolTip.SetToolTip(control, "pure prime = additive prime with prime digits");
                }
                else if (control.Text == "PP")
                {
                    control.Text = "C";
                    control.ForeColor = GetNumberTypeColor(14);
                    ToolTip.SetToolTip(control, "composite = divisible by prime(s) below it");
                }
                else if (control.Text == "C")
                {
                    control.Text = "AC";
                    control.ForeColor = GetNumberTypeColor(114);
                    ToolTip.SetToolTip(control, "additive composite = composite with composite digit sum");
                }
                else if (control.Text == "AC")
                {
                    control.Text = "PC";
                    control.ForeColor = GetNumberTypeColor(9);
                    ToolTip.SetToolTip(control, "pure composite = additive composite with composite digits");
                }
                else if (control.Text == "PC")
                {
                    control.Text = "";
                    control.ForeColor = Color.Olive;
                    ToolTip.SetToolTip(control, "");
                }
            }
            else
            {
                if (control.Text == "")
                {
                    control.Text = "PC";
                    control.ForeColor = GetNumberTypeColor(9);
                    ToolTip.SetToolTip(control, "pure composite = additive composite with composite digits");
                }
                else if (control.Text == "PC")
                {
                    control.Text = "AC";
                    control.ForeColor = GetNumberTypeColor(114);
                    ToolTip.SetToolTip(control, "additive composite = composite with composite digit sum");
                }
                else if (control.Text == "AC")
                {
                    control.Text = "C";
                    control.ForeColor = GetNumberTypeColor(14);
                    ToolTip.SetToolTip(control, "composite = divisible by prime(s) below it");
                }
                else if (control.Text == "C")
                {
                    control.Text = "PP";
                    control.ForeColor = GetNumberTypeColor(313);
                    ToolTip.SetToolTip(control, "pure prime = additive prime with prime digits");
                }
                else if (control.Text == "PP")
                {
                    control.Text = "AP";
                    control.ForeColor = GetNumberTypeColor(47);
                    ToolTip.SetToolTip(control, "additive prime = prime with prime digit sum");
                }
                else if (control.Text == "AP")
                {
                    control.Text = "P";
                    control.ForeColor = GetNumberTypeColor(19);
                    ToolTip.SetToolTip(control, "prime = divisible by itself and 1 only");
                }
                else if (control.Text == "P")
                {
                    control.Text = null;
                    control.ForeColor = control.BackColor;
                    ToolTip.SetToolTip(control, "");
                }
            }
            return true;
        }
    }
    private void FindByNumbersComparisonOperatorLabel_Click(object sender, EventArgs e)
    {
        UpdateNumberTypeLabelTags();

        Control control = sender as Control;
        if (control != null)
        {
            if (UpdateComparisonOperator(control))
            {
                if (control == FindByNumbersNumberComparisonOperatorLabel)
                {
                    if (FindByNumbersNumberComparisonOperatorLabel.Text == "%")
                    {
                        int remainder = -1;
                        FindByNumbersNumberNumberTypeLabel.Tag = remainder;
                        FindByNumbersNumberNumberTypeLabel.Text = remainder.ToString();
                        FindByNumbersNumberNumberTypeLabel.ForeColor = Color.Black;
                        FindByNumbersNumberNumberTypeLabel.Enabled = true;
                        ToolTip.SetToolTip(FindByNumbersNumberNumberTypeLabel, "remainder");
                    }
                    else
                    {
                        FindByNumbersNumberNumberTypeLabel.Text = "";
                        ToolTip.SetToolTip(FindByNumbersNumberNumberTypeLabel, null);
                    }
                }
                else if (control == FindByNumbersChaptersComparisonOperatorLabel)
                {
                    if (FindByNumbersChaptersComparisonOperatorLabel.Text == "%")
                    {
                        int remainder = -1;
                        FindByNumbersChaptersNumberTypeLabel.Tag = remainder;
                        FindByNumbersChaptersNumberTypeLabel.Text = remainder.ToString();
                        FindByNumbersChaptersNumberTypeLabel.ForeColor = Color.Black;
                        FindByNumbersChaptersNumberTypeLabel.Enabled = true;
                        ToolTip.SetToolTip(FindByNumbersChaptersNumberTypeLabel, "remainder");
                    }
                    else
                    {
                        FindByNumbersChaptersNumberTypeLabel.Text = "";
                        ToolTip.SetToolTip(FindByNumbersChaptersNumberTypeLabel, null);
                    }
                }
                else if (control == FindByNumbersVersesComparisonOperatorLabel)
                {
                    if (FindByNumbersVersesComparisonOperatorLabel.Text == "%")
                    {
                        int remainder = -1;
                        FindByNumbersVersesNumberTypeLabel.Tag = remainder;
                        FindByNumbersVersesNumberTypeLabel.Text = remainder.ToString();
                        FindByNumbersVersesNumberTypeLabel.ForeColor = Color.Black;
                        FindByNumbersVersesNumberTypeLabel.Enabled = true;
                        ToolTip.SetToolTip(FindByNumbersVersesNumberTypeLabel, "remainder");
                    }
                    else
                    {
                        FindByNumbersVersesNumberTypeLabel.Text = "";
                        ToolTip.SetToolTip(FindByNumbersVersesNumberTypeLabel, null);
                    }
                }
                else if (control == FindByNumbersWordsComparisonOperatorLabel)
                {
                    if (FindByNumbersWordsComparisonOperatorLabel.Text == "%")
                    {
                        int remainder = -1;
                        FindByNumbersWordsNumberTypeLabel.Tag = remainder;
                        FindByNumbersWordsNumberTypeLabel.Text = remainder.ToString();
                        FindByNumbersWordsNumberTypeLabel.ForeColor = Color.Black;
                        FindByNumbersWordsNumberTypeLabel.Enabled = true;
                        ToolTip.SetToolTip(FindByNumbersWordsNumberTypeLabel, "remainder");
                    }
                    else
                    {
                        FindByNumbersWordsNumberTypeLabel.Text = "";
                        ToolTip.SetToolTip(FindByNumbersWordsNumberTypeLabel, null);
                    }
                }
                else if (control == FindByNumbersLettersComparisonOperatorLabel)
                {
                    if (FindByNumbersLettersComparisonOperatorLabel.Text == "%")
                    {
                        int remainder = -1;
                        FindByNumbersLettersNumberTypeLabel.Tag = remainder;
                        FindByNumbersLettersNumberTypeLabel.Text = remainder.ToString();
                        FindByNumbersLettersNumberTypeLabel.ForeColor = Color.Black;
                        FindByNumbersLettersNumberTypeLabel.Enabled = true;
                        ToolTip.SetToolTip(FindByNumbersLettersNumberTypeLabel, "remainder");
                    }
                    else
                    {
                        FindByNumbersLettersNumberTypeLabel.Text = "";
                        ToolTip.SetToolTip(FindByNumbersLettersNumberTypeLabel, null);
                    }
                }
                else if (control == FindByNumbersUniqueLettersComparisonOperatorLabel)
                {
                    if (FindByNumbersUniqueLettersComparisonOperatorLabel.Text == "%")
                    {
                        int remainder = -1;
                        FindByNumbersUniqueLettersNumberTypeLabel.Tag = remainder;
                        FindByNumbersUniqueLettersNumberTypeLabel.Text = remainder.ToString();
                        FindByNumbersUniqueLettersNumberTypeLabel.ForeColor = Color.Black;
                        FindByNumbersUniqueLettersNumberTypeLabel.Enabled = true;
                        ToolTip.SetToolTip(FindByNumbersUniqueLettersNumberTypeLabel, "remainder");
                    }
                    else
                    {
                        FindByNumbersUniqueLettersNumberTypeLabel.Text = "";
                        ToolTip.SetToolTip(FindByNumbersUniqueLettersNumberTypeLabel, null);
                    }
                }
                else if (control == FindByNumbersValueComparisonOperatorLabel)
                {
                    if (FindByNumbersValueComparisonOperatorLabel.Text == "%")
                    {
                        int remainder = -1;
                        FindByNumbersValueNumberTypeLabel.Tag = remainder;
                        FindByNumbersValueNumberTypeLabel.Text = remainder.ToString();
                        FindByNumbersValueNumberTypeLabel.ForeColor = Color.Black;
                        FindByNumbersValueNumberTypeLabel.Enabled = true;
                        ToolTip.SetToolTip(FindByNumbersValueNumberTypeLabel, "remainder");
                    }
                    else
                    {
                        FindByNumbersValueNumberTypeLabel.Text = "";
                        ToolTip.SetToolTip(FindByNumbersValueNumberTypeLabel, null);
                    }
                }
                else if (control == FindByNumbersValueDigitSumComparisonOperatorLabel)
                {
                    if (FindByNumbersValueDigitSumComparisonOperatorLabel.Text == "%")
                    {
                        int remainder = -1;
                        FindByNumbersValueDigitSumNumberTypeLabel.Tag = remainder;
                        FindByNumbersValueDigitSumNumberTypeLabel.Text = remainder.ToString();
                        FindByNumbersValueDigitSumNumberTypeLabel.ForeColor = Color.Black;
                        FindByNumbersValueDigitSumNumberTypeLabel.Enabled = true;
                        ToolTip.SetToolTip(FindByNumbersValueDigitSumNumberTypeLabel, "remainder");
                    }
                    else
                    {
                        FindByNumbersValueDigitSumNumberTypeLabel.Text = "";
                        ToolTip.SetToolTip(FindByNumbersValueDigitSumNumberTypeLabel, null);
                    }
                }
                else if (control == FindByNumbersValueDigitalRootComparisonOperatorLabel)
                {
                    if (FindByNumbersValueDigitalRootComparisonOperatorLabel.Text == "%")
                    {
                        int remainder = -1;
                        FindByNumbersValueDigitalRootNumberTypeLabel.Tag = remainder;
                        FindByNumbersValueDigitalRootNumberTypeLabel.Text = remainder.ToString();
                        FindByNumbersValueDigitalRootNumberTypeLabel.ForeColor = Color.Black;
                        FindByNumbersValueDigitalRootNumberTypeLabel.Enabled = true;
                        ToolTip.SetToolTip(FindByNumbersValueDigitalRootNumberTypeLabel, "remainder");
                    }
                    else
                    {
                        FindByNumbersValueDigitalRootNumberTypeLabel.Text = "";
                        ToolTip.SetToolTip(FindByNumbersValueDigitalRootNumberTypeLabel, null);
                    }
                }
                else
                {
                    // do nothing
                }

                FindByNumbersControls_Enter(null, null);
            }
        }
    }
    private void FindByNumbersNumberTypeLabel_Click(object sender, EventArgs e)
    {
        UpdateNumberTypeLabelTags();

        Control control = sender as Control;
        if (control != null)
        {
            if (UpdateNumberType(control))
            {
                if (control == FindByNumbersNumberNumberTypeLabel)
                {
                    FindByNumbersNumberComparisonOperatorLabel.Enabled = (control.Text.Length == 0);
                    FindByNumbersNumberNumericUpDown.Enabled = (control.Text == "");
                    if (control.Text.Length > 0)
                    {
                        FindByNumbersNumberComparisonOperatorLabel.Text = "=";
                        FindByNumbersNumberNumericUpDown.Value = 0;
                    }
                    else
                    {
                        FindByNumbersNumberNumericUpDown.Focus();
                    }
                }
                else if (control == FindByNumbersChaptersNumberTypeLabel)
                {
                    FindByNumbersChaptersComparisonOperatorLabel.Enabled = (control.Text.Length == 0);
                    FindByNumbersChaptersNumericUpDown.Enabled = (control.Text == "");
                    if (control.Text.Length > 0)
                    {
                        //if (m_numbers_result_type == FindByNumbersResultType.Chapters)
                        //{
                        //    m_numbers_result_type = FindByNumbersResultType.ChapterRanges;
                        //    FindByNumbersResultTypeChaptersLabel.Text = "Chapters";
                        //}
                        FindByNumbersChaptersComparisonOperatorLabel.Text = "=";
                        FindByNumbersChaptersNumericUpDown.Value = 0;
                    }
                    else
                    {
                        //if (m_numbers_result_type == FindByNumbersResultType.ChapterRanges)
                        //{
                        //    m_numbers_result_type = FindByNumbersResultType.Chapters;
                        //    FindByNumbersResultTypeChaptersLabel.Text = "CHAPTER";
                        //}
                        FindByNumbersChaptersNumericUpDown.Focus();
                    }
                }
                else if (control == FindByNumbersVersesNumberTypeLabel)
                {
                    FindByNumbersVersesComparisonOperatorLabel.Enabled = (control.Text.Length == 0);
                    FindByNumbersVersesNumericUpDown.Enabled = (control.Text == "");
                    if (control.Text.Length > 0)
                    {
                        //if (m_numbers_result_type == FindByNumbersResultType.Verses)
                        //{
                        //    m_numbers_result_type = FindByNumbersResultType.VerseRanges;
                        //    FindByNumbersResultTypeVersesLabel.Text = "Verses";
                        //}
                        FindByNumbersVersesComparisonOperatorLabel.Text = "=";
                        FindByNumbersVersesNumericUpDown.Value = 0;
                    }
                    else
                    {
                        //if (m_numbers_result_type == FindByNumbersResultType.VerseRanges)
                        //{
                        //    m_numbers_result_type = FindByNumbersResultType.Verses;
                        //    FindByNumbersResultTypeVersesLabel.Text = "VERSES";
                        //}
                        FindByNumbersVersesNumericUpDown.Focus();
                    }
                }
                else if (control == FindByNumbersWordsNumberTypeLabel)
                {
                    FindByNumbersWordsComparisonOperatorLabel.Enabled = (control.Text.Length == 0);
                    FindByNumbersWordsNumericUpDown.Enabled = (control.Text == "");
                    if (control.Text.Length > 0)
                    {
                        //if (m_numbers_result_type == FindByNumbersResultType.Words)
                        //{
                        //    m_numbers_result_type = FindByNumbersResultType.WordRanges;
                        //    FindByNumbersResultTypeWordsLabel.Text = "Words";
                        //}
                        FindByNumbersWordsComparisonOperatorLabel.Text = "=";
                        FindByNumbersWordsNumericUpDown.Value = 0;
                    }
                    else
                    {
                        //if (m_numbers_result_type == FindByNumbersResultType.WordRanges)
                        //{
                        //    m_numbers_result_type = FindByNumbersResultType.Words;
                        //    FindByNumbersResultTypeWordsLabel.Text = "WORD";
                        //}
                        FindByNumbersWordsNumericUpDown.Focus();
                    }
                }
                else if (control == FindByNumbersLettersNumberTypeLabel)
                {
                    FindByNumbersLettersComparisonOperatorLabel.Enabled = (control.Text.Length == 0);
                    FindByNumbersLettersNumericUpDown.Enabled = (control.Text == "");
                    if (control.Text.Length > 0)
                    {
                        FindByNumbersLettersComparisonOperatorLabel.Text = "=";
                        FindByNumbersLettersNumericUpDown.Value = 0;
                    }
                    else
                    {
                        FindByNumbersLettersNumericUpDown.Focus();
                    }
                }
                else if (control == FindByNumbersUniqueLettersNumberTypeLabel)
                {
                    FindByNumbersUniqueLettersComparisonOperatorLabel.Enabled = (control.Text.Length == 0);
                    FindByNumbersUniqueLettersNumericUpDown.Enabled = (control.Text == "");
                    if (control.Text.Length > 0)
                    {
                        FindByNumbersUniqueLettersComparisonOperatorLabel.Text = "=";
                        FindByNumbersUniqueLettersNumericUpDown.Value = 0;
                    }
                    else
                    {
                        FindByNumbersUniqueLettersNumericUpDown.Focus();
                    }
                }
                else if (control == FindByNumbersValueNumberTypeLabel)
                {
                    FindByNumbersValueComparisonOperatorLabel.Enabled = (control.Text.Length == 0);
                    FindByNumbersValueNumericUpDown.Enabled = (control.Text == "");
                    if (control.Text.Length > 0)
                    {
                        FindByNumbersValueComparisonOperatorLabel.Text = "=";
                        FindByNumbersValueNumericUpDown.Value = 0;
                    }
                    else
                    {
                        FindByNumbersValueNumericUpDown.Focus();
                    }
                }
                else if (control == FindByNumbersValueDigitSumNumberTypeLabel)
                {
                    FindByNumbersValueDigitSumComparisonOperatorLabel.Enabled = (control.Text.Length == 0);
                    FindByNumbersValueDigitSumNumericUpDown.Enabled = (control.Text == "");
                    if (control.Text.Length > 0)
                    {
                        FindByNumbersValueDigitSumComparisonOperatorLabel.Text = "=";
                        FindByNumbersValueDigitSumNumericUpDown.Value = 0;
                    }
                    else
                    {
                        FindByNumbersValueDigitSumNumericUpDown.Focus();
                    }
                }
                else if (control == FindByNumbersValueDigitalRootNumberTypeLabel)
                {
                    FindByNumbersValueDigitalRootComparisonOperatorLabel.Enabled = (control.Text.Length == 0);
                    FindByNumbersValueDigitalRootNumericUpDown.Enabled = (control.Text == "");
                    if (control.Text.Length > 0)
                    {
                        FindByNumbersValueDigitalRootComparisonOperatorLabel.Text = "=";
                        FindByNumbersValueDigitalRootNumericUpDown.Value = 0;
                    }
                    else
                    {
                        FindByNumbersValueDigitalRootNumericUpDown.Focus();
                    }
                }
                else
                {
                    // do nothing
                }

                FindByNumbersControls_Enter(null, null);
            }
        }
    }
    private void FindByNumbersControl_EnabledChanged(object sender, EventArgs e)
    {
        Control control = sender as Control;
        if (control != null)
        {
            control.BackColor = (control.Enabled) ? SystemColors.Window : Color.LightGray;
        }
    }
    private void FindByNumbersControls_Enter(object sender, EventArgs e)
    {
        this.AcceptButton = FindByNumbersButton;

        FindByTextButton.Enabled = false;
        FindBySimilarityButton.Enabled = false;
        FindByNumbersButton.Enabled = true;
        FindByRevelationPlaceButton.Enabled = false;
        FindByProstrationTypeButton.Enabled = false;
        FindByFrequencyButton.Enabled = false;

        AutoCompleteHeaderLabel.Visible = false;
        AutoCompleteListBox.Visible = false;

        ResetFindByTextResultTypeLabels();
        ResetFindBySimilarityResultTypeLabels();
        ResetFindByNumbersResultTypeLabels();
        ResetFindByFrequencyResultTypeLabels();

        switch (m_numbers_result_type)
        {
            case NumbersResultType.Words:
            case NumbersResultType.WordRanges:
            case NumbersResultType.WordSets:
                {
                    FindByNumbersResultTypeWordsLabel.BackColor = Color.SteelBlue;
                    FindByNumbersResultTypeWordsLabel.BorderStyle = BorderStyle.Fixed3D;
                }
                break;
            case NumbersResultType.Sentences:
                {
                    FindByNumbersResultTypeSentencesLabel.BackColor = Color.SteelBlue;
                    FindByNumbersResultTypeSentencesLabel.BorderStyle = BorderStyle.Fixed3D;
                }
                break;
            case NumbersResultType.Verses:
            case NumbersResultType.VerseRanges:
            case NumbersResultType.VerseSets:
                {
                    FindByNumbersResultTypeVersesLabel.BackColor = Color.SteelBlue;
                    FindByNumbersResultTypeVersesLabel.BorderStyle = BorderStyle.Fixed3D;
                }
                break;
            case NumbersResultType.Chapters:
            case NumbersResultType.ChapterRanges:
            case NumbersResultType.ChapterSets:
                {
                    FindByNumbersResultTypeChaptersLabel.BackColor = Color.SteelBlue;
                    FindByNumbersResultTypeChaptersLabel.BorderStyle = BorderStyle.Fixed3D;
                }
                break;
            default:
                break;
        }
    }
    private void FindByNumbersNumericUpDown_Leave(object sender, EventArgs e)
    {
        NumericUpDown control = sender as NumericUpDown;
        if (control != null)
        {
            if (String.IsNullOrEmpty(control.Text))
            {
                control.Value = 0;
                control.Refresh();
            }
        }

        UpdateNumberTypeLabelTags();
    }
    private void FindByNumbersNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
        UpdateFindByNumbersResultType();

        UpdateNumberTypeLabelTags();

        // don't auto-find as user may not have finished setting all parameters yet
        // some operations take too long and would frustrate user
        //FindByNumbers();
    }
    private void UpdateNumberTypeLabelTags()
    {
        FindByNumbersNumberNumberTypeLabel.Tag = (int)FindByNumbersNumberNumericUpDown.Value;
        FindByNumbersChaptersNumberTypeLabel.Tag = (int)FindByNumbersChaptersNumericUpDown.Value;
        FindByNumbersVersesNumberTypeLabel.Tag = (int)FindByNumbersVersesNumericUpDown.Value;
        FindByNumbersWordsNumberTypeLabel.Tag = (int)FindByNumbersWordsNumericUpDown.Value;
        FindByNumbersLettersNumberTypeLabel.Tag = (int)FindByNumbersLettersNumericUpDown.Value;
        FindByNumbersUniqueLettersNumberTypeLabel.Tag = (int)FindByNumbersUniqueLettersNumericUpDown.Value;
        FindByNumbersValueNumberTypeLabel.Tag = (int)FindByNumbersValueNumericUpDown.Value;
        FindByNumbersValueDigitSumNumberTypeLabel.Tag = (int)FindByNumbersValueDigitSumNumericUpDown.Value;
        FindByNumbersValueDigitalRootNumberTypeLabel.Tag = (int)FindByNumbersValueDigitalRootNumericUpDown.Value;

        int number;
        if (int.TryParse(FindByNumbersNumberNumberTypeLabel.Text, out number))
        {
            if (number >= (int)FindByNumbersNumberNumericUpDown.Value)
            {
                number = (int)FindByNumbersNumberNumericUpDown.Value - 1;
                if (number < 0) number = 0;
                FindByNumbersNumberNumberTypeLabel.Text = number.ToString();
            }
        }

        if (int.TryParse(FindByNumbersChaptersNumberTypeLabel.Text, out number))
        {
            if (number >= (int)FindByNumbersChaptersNumericUpDown.Value)
            {
                number = (int)FindByNumbersChaptersNumericUpDown.Value - 1;
                if (number < 0) number = 0;
                FindByNumbersChaptersNumberTypeLabel.Text = number.ToString();
            }
        }

        if (int.TryParse(FindByNumbersVersesNumberTypeLabel.Text, out number))
        {
            if (number >= (int)FindByNumbersVersesNumericUpDown.Value)
            {
                number = (int)FindByNumbersVersesNumericUpDown.Value - 1;
                if (number < 0) number = 0;
                FindByNumbersVersesNumberTypeLabel.Text = number.ToString();
            }
        }

        if (int.TryParse(FindByNumbersWordsNumberTypeLabel.Text, out number))
        {
            if (number >= (int)FindByNumbersWordsNumericUpDown.Value)
            {
                number = (int)FindByNumbersWordsNumericUpDown.Value - 1;
                if (number < 0) number = 0;
                FindByNumbersWordsNumberTypeLabel.Text = number.ToString();
            }
        }

        if (int.TryParse(FindByNumbersLettersNumberTypeLabel.Text, out number))
        {
            if (number >= (int)FindByNumbersLettersNumericUpDown.Value)
            {
                number = (int)FindByNumbersLettersNumericUpDown.Value - 1;
                if (number < 0) number = 0;
                FindByNumbersLettersNumberTypeLabel.Text = number.ToString();
            }
        }

        if (int.TryParse(FindByNumbersUniqueLettersNumberTypeLabel.Text, out number))
        {
            if (number >= (int)FindByNumbersUniqueLettersNumericUpDown.Value)
            {
                number = (int)FindByNumbersUniqueLettersNumericUpDown.Value - 1;
                if (number < 0) number = 0;
                FindByNumbersUniqueLettersNumberTypeLabel.Text = number.ToString();
            }
        }

        if (int.TryParse(FindByNumbersValueNumberTypeLabel.Text, out number))
        {
            if (number >= (int)FindByNumbersValueNumericUpDown.Value)
            {
                number = (int)FindByNumbersValueNumericUpDown.Value - 1;
                if (number < 0) number = 0;
                FindByNumbersValueNumericUpDown.Text = number.ToString();
            }
        }

        if (int.TryParse(FindByNumbersValueDigitSumNumberTypeLabel.Text, out number))
        {
            if (number >= (int)FindByNumbersValueDigitSumNumericUpDown.Value)
            {
                number = (int)FindByNumbersValueDigitSumNumericUpDown.Value - 1;
                if (number < 0) number = 0;
                FindByNumbersValueDigitSumNumberTypeLabel.Text = number.ToString();
            }
        }

        if (int.TryParse(FindByNumbersValueDigitalRootNumberTypeLabel.Text, out number))
        {
            if (number >= (int)FindByNumbersValueDigitalRootNumericUpDown.Value)
            {
                number = (int)FindByNumbersValueDigitalRootNumericUpDown.Value - 1;
                if (number < 0) number = 0;
                FindByNumbersValueDigitalRootNumberTypeLabel.Text = number.ToString();
            }
        }
    }
    private void FindByNumbersButton_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            FindByNumbers();
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void FindByNumbers()
    {
        m_search_type = SearchType.Numbers;

        if (m_client != null)
        {
            ClearFindMatches();

            // 1. number types
            string number_symbol = FindByNumbersNumberNumberTypeLabel.Enabled ? FindByNumbersNumberNumberTypeLabel.Text : "";
            NumberType number_number_type =
                (number_symbol == "PP") ? NumberType.PurePrime :
                (number_symbol == "AP") ? NumberType.AdditivePrime :
                (number_symbol == "P") ? NumberType.Prime :
                (number_symbol == "PC") ? NumberType.PureComposite :
                (number_symbol == "AC") ? NumberType.AdditiveComposite :
                (number_symbol == "C") ? NumberType.Composite :
                (number_symbol == "") ? NumberType.None :
                                        NumberType.Any;
            string chapter_count_symbol = FindByNumbersChaptersNumberTypeLabel.Enabled ? FindByNumbersChaptersNumberTypeLabel.Text : "";
            NumberType chapter_count_number_type =
                (chapter_count_symbol == "PP") ? NumberType.PurePrime :
                (chapter_count_symbol == "AP") ? NumberType.AdditivePrime :
                (chapter_count_symbol == "P") ? NumberType.Prime :
                (chapter_count_symbol == "PC") ? NumberType.PureComposite :
                (chapter_count_symbol == "AC") ? NumberType.AdditiveComposite :
                (chapter_count_symbol == "C") ? NumberType.Composite :
                (chapter_count_symbol == "") ? NumberType.None :
                                               NumberType.Any;
            string verse_count_symbol = FindByNumbersVersesNumberTypeLabel.Enabled ? FindByNumbersVersesNumberTypeLabel.Text : "";
            NumberType verse_count_number_type =
                (verse_count_symbol == "PP") ? NumberType.PurePrime :
                (verse_count_symbol == "AP") ? NumberType.AdditivePrime :
                (verse_count_symbol == "P") ? NumberType.Prime :
                (verse_count_symbol == "PC") ? NumberType.PureComposite :
                (verse_count_symbol == "AC") ? NumberType.AdditiveComposite :
                (verse_count_symbol == "C") ? NumberType.Composite :
                (verse_count_symbol == "") ? NumberType.None :
                                             NumberType.Any;
            string word_count_symbol = FindByNumbersWordsNumberTypeLabel.Enabled ? FindByNumbersWordsNumberTypeLabel.Text : "";
            NumberType word_count_number_type =
                (word_count_symbol == "PP") ? NumberType.PurePrime :
                (word_count_symbol == "AP") ? NumberType.AdditivePrime :
                (word_count_symbol == "P") ? NumberType.Prime :
                (word_count_symbol == "PC") ? NumberType.PureComposite :
                (word_count_symbol == "AC") ? NumberType.AdditiveComposite :
                (word_count_symbol == "C") ? NumberType.Composite :
                (word_count_symbol == "") ? NumberType.None :
                                            NumberType.Any;
            string letter_count_symbol = FindByNumbersLettersNumberTypeLabel.Enabled ? FindByNumbersLettersNumberTypeLabel.Text : "";
            NumberType letter_count_number_type =
                (letter_count_symbol == "PP") ? NumberType.PurePrime :
                (letter_count_symbol == "AP") ? NumberType.AdditivePrime :
                (letter_count_symbol == "P") ? NumberType.Prime :
                (letter_count_symbol == "PC") ? NumberType.PureComposite :
                (letter_count_symbol == "AC") ? NumberType.AdditiveComposite :
                (letter_count_symbol == "C") ? NumberType.Composite :
                (letter_count_symbol == "") ? NumberType.None :
                                              NumberType.Any;
            string unique_letter_count_symbol = FindByNumbersUniqueLettersNumberTypeLabel.Enabled ? FindByNumbersUniqueLettersNumberTypeLabel.Text : "";
            NumberType unique_letter_count_number_type =
                (unique_letter_count_symbol == "PP") ? NumberType.PurePrime :
                (unique_letter_count_symbol == "AP") ? NumberType.AdditivePrime :
                (unique_letter_count_symbol == "P") ? NumberType.Prime :
                (unique_letter_count_symbol == "PC") ? NumberType.PureComposite :
                (unique_letter_count_symbol == "AC") ? NumberType.AdditiveComposite :
                (unique_letter_count_symbol == "C") ? NumberType.Composite :
                (unique_letter_count_symbol == "") ? NumberType.None :
                                                     NumberType.Any;
            string value_symbol = FindByNumbersValueNumberTypeLabel.Enabled ? FindByNumbersValueNumberTypeLabel.Text : "";
            NumberType value_number_type =
                (value_symbol == "PP") ? NumberType.PurePrime :
                (value_symbol == "AP") ? NumberType.AdditivePrime :
                (value_symbol == "P") ? NumberType.Prime :
                (value_symbol == "PC") ? NumberType.PureComposite :
                (value_symbol == "AC") ? NumberType.AdditiveComposite :
                (value_symbol == "C") ? NumberType.Composite :
                (value_symbol == "") ? NumberType.None :
                                       NumberType.Any;
            string value_digit_sum_symbol = FindByNumbersValueDigitSumNumberTypeLabel.Enabled ? FindByNumbersValueDigitSumNumberTypeLabel.Text : "";
            NumberType value_digit_sum_number_type =
                (value_digit_sum_symbol == "PP") ? NumberType.PurePrime :
                (value_digit_sum_symbol == "AP") ? NumberType.AdditivePrime :
                (value_digit_sum_symbol == "P") ? NumberType.Prime :
                (value_digit_sum_symbol == "PC") ? NumberType.PureComposite :
                (value_digit_sum_symbol == "AC") ? NumberType.AdditiveComposite :
                (value_digit_sum_symbol == "C") ? NumberType.Composite :
                (value_digit_sum_symbol == "") ? NumberType.None :
                                                 NumberType.Any;
            string value_digital_root_symbol = FindByNumbersValueDigitalRootNumberTypeLabel.Enabled ? FindByNumbersValueDigitalRootNumberTypeLabel.Text : "";
            NumberType value_digital_root_number_type =
                (value_digital_root_symbol == "PP") ? NumberType.PurePrime :
                (value_digital_root_symbol == "AP") ? NumberType.AdditivePrime :
                (value_digital_root_symbol == "P") ? NumberType.Prime :
                (value_digital_root_symbol == "PC") ? NumberType.PureComposite :
                (value_digital_root_symbol == "AC") ? NumberType.AdditiveComposite :
                (value_digital_root_symbol == "C") ? NumberType.Composite :
                (value_digital_root_symbol == "") ? NumberType.None :
                                                    NumberType.Any;

            // 2. numbers
            int number = (int)FindByNumbersNumberNumericUpDown.Value;
            int chapter_count = (int)FindByNumbersChaptersNumericUpDown.Value;
            int verse_count = (int)FindByNumbersVersesNumericUpDown.Value;
            int word_count = (int)FindByNumbersWordsNumericUpDown.Value;
            int letter_count = (int)FindByNumbersLettersNumericUpDown.Value;
            int unique_letter_count = (int)FindByNumbersUniqueLettersNumericUpDown.Value;
            long value = (long)FindByNumbersValueNumericUpDown.Value;
            int value_digit_sum = (int)FindByNumbersValueDigitSumNumericUpDown.Value;
            int value_digital_root = (int)FindByNumbersValueDigitalRootNumericUpDown.Value;

            // 3. comparison operators = ≠ < ≤ > ≥ X %
            string number_operator_symbol = FindByNumbersNumberComparisonOperatorLabel.Text;
            ComparisonOperator number_comparison_operator =
                (number_operator_symbol == "=") ? ComparisonOperator.Equal :
                (number_operator_symbol == "≠") ? ComparisonOperator.NotEqual :
                (number_operator_symbol == "<") ? ComparisonOperator.LessThan :
                (number_operator_symbol == "≤") ? ComparisonOperator.LessThanOrEqual :
                (number_operator_symbol == ">") ? ComparisonOperator.GreaterThan :
                (number_operator_symbol == "≥") ? ComparisonOperator.GreaterThanOrEqual :
                (number_operator_symbol == "X") ? ComparisonOperator.MultipleOf :
                (number_operator_symbol == "%") ? ComparisonOperator.DivisibleByWithRemainder :
                                                  ComparisonOperator.Reserved;
            string chapter_count_operator_symbol = FindByNumbersChaptersComparisonOperatorLabel.Text;
            ComparisonOperator chapter_count_comparison_operator =
                (chapter_count_operator_symbol == "=") ? ComparisonOperator.Equal :
                (chapter_count_operator_symbol == "≠") ? ComparisonOperator.NotEqual :
                (chapter_count_operator_symbol == "<") ? ComparisonOperator.LessThan :
                (chapter_count_operator_symbol == "≤") ? ComparisonOperator.LessThanOrEqual :
                (chapter_count_operator_symbol == ">") ? ComparisonOperator.GreaterThan :
                (chapter_count_operator_symbol == "≥") ? ComparisonOperator.GreaterThanOrEqual :
                (chapter_count_operator_symbol == "X") ? ComparisonOperator.MultipleOf :
                (chapter_count_operator_symbol == "%") ? ComparisonOperator.DivisibleByWithRemainder :
                                                         ComparisonOperator.Reserved;
            string verse_count_operator_symbol = FindByNumbersVersesComparisonOperatorLabel.Text;
            ComparisonOperator verse_count_comparison_operator =
                (verse_count_operator_symbol == "=") ? ComparisonOperator.Equal :
                (verse_count_operator_symbol == "≠") ? ComparisonOperator.NotEqual :
                (verse_count_operator_symbol == "<") ? ComparisonOperator.LessThan :
                (verse_count_operator_symbol == "≤") ? ComparisonOperator.LessThanOrEqual :
                (verse_count_operator_symbol == ">") ? ComparisonOperator.GreaterThan :
                (verse_count_operator_symbol == "≥") ? ComparisonOperator.GreaterThanOrEqual :
                (verse_count_operator_symbol == "X") ? ComparisonOperator.MultipleOf :
                (verse_count_operator_symbol == "%") ? ComparisonOperator.DivisibleByWithRemainder :
                                                       ComparisonOperator.Reserved;
            string word_count_operator_symbol = FindByNumbersWordsComparisonOperatorLabel.Text;
            ComparisonOperator word_count_comparison_operator =
                (word_count_operator_symbol == "=") ? ComparisonOperator.Equal :
                (word_count_operator_symbol == "≠") ? ComparisonOperator.NotEqual :
                (word_count_operator_symbol == "<") ? ComparisonOperator.LessThan :
                (word_count_operator_symbol == "≤") ? ComparisonOperator.LessThanOrEqual :
                (word_count_operator_symbol == ">") ? ComparisonOperator.GreaterThan :
                (word_count_operator_symbol == "≥") ? ComparisonOperator.GreaterThanOrEqual :
                (word_count_operator_symbol == "X") ? ComparisonOperator.MultipleOf :
                (word_count_operator_symbol == "%") ? ComparisonOperator.DivisibleByWithRemainder :
                                                      ComparisonOperator.Reserved;
            string letter_count_operator_symbol = FindByNumbersLettersComparisonOperatorLabel.Text;
            ComparisonOperator letter_count_comparison_operator =
                (letter_count_operator_symbol == "=") ? ComparisonOperator.Equal :
                (letter_count_operator_symbol == "≠") ? ComparisonOperator.NotEqual :
                (letter_count_operator_symbol == "<") ? ComparisonOperator.LessThan :
                (letter_count_operator_symbol == "≤") ? ComparisonOperator.LessThanOrEqual :
                (letter_count_operator_symbol == ">") ? ComparisonOperator.GreaterThan :
                (letter_count_operator_symbol == "≥") ? ComparisonOperator.GreaterThanOrEqual :
                (letter_count_operator_symbol == "X") ? ComparisonOperator.MultipleOf :
                (letter_count_operator_symbol == "%") ? ComparisonOperator.DivisibleByWithRemainder :
                                                        ComparisonOperator.Reserved;
            string unique_letter_count_operator_symbol = FindByNumbersUniqueLettersComparisonOperatorLabel.Text;
            ComparisonOperator unique_letter_count_comparison_operator =
                (unique_letter_count_operator_symbol == "=") ? ComparisonOperator.Equal :
                (unique_letter_count_operator_symbol == "≠") ? ComparisonOperator.NotEqual :
                (unique_letter_count_operator_symbol == "<") ? ComparisonOperator.LessThan :
                (unique_letter_count_operator_symbol == "≤") ? ComparisonOperator.LessThanOrEqual :
                (unique_letter_count_operator_symbol == ">") ? ComparisonOperator.GreaterThan :
                (unique_letter_count_operator_symbol == "≥") ? ComparisonOperator.GreaterThanOrEqual :
                (unique_letter_count_operator_symbol == "X") ? ComparisonOperator.MultipleOf :
                (unique_letter_count_operator_symbol == "%") ? ComparisonOperator.DivisibleByWithRemainder :
                                                               ComparisonOperator.Reserved;
            string value_operator_symbol = FindByNumbersValueComparisonOperatorLabel.Text;
            ComparisonOperator value_comparison_operator =
                (value_operator_symbol == "=") ? ComparisonOperator.Equal :
                (value_operator_symbol == "≠") ? ComparisonOperator.NotEqual :
                (value_operator_symbol == "<") ? ComparisonOperator.LessThan :
                (value_operator_symbol == "≤") ? ComparisonOperator.LessThanOrEqual :
                (value_operator_symbol == ">") ? ComparisonOperator.GreaterThan :
                (value_operator_symbol == "≥") ? ComparisonOperator.GreaterThanOrEqual :
                (value_operator_symbol == "X") ? ComparisonOperator.MultipleOf :
                (value_operator_symbol == "%") ? ComparisonOperator.DivisibleByWithRemainder :
                                                 ComparisonOperator.Reserved;
            string value_digit_sum_operator_symbol = FindByNumbersValueDigitSumComparisonOperatorLabel.Text;
            ComparisonOperator value_digit_sum_comparison_operator =
                (value_digit_sum_operator_symbol == "=") ? ComparisonOperator.Equal :
                (value_digit_sum_operator_symbol == "≠") ? ComparisonOperator.NotEqual :
                (value_digit_sum_operator_symbol == "<") ? ComparisonOperator.LessThan :
                (value_digit_sum_operator_symbol == "≤") ? ComparisonOperator.LessThanOrEqual :
                (value_digit_sum_operator_symbol == ">") ? ComparisonOperator.GreaterThan :
                (value_digit_sum_operator_symbol == "≥") ? ComparisonOperator.GreaterThanOrEqual :
                (value_digit_sum_operator_symbol == "X") ? ComparisonOperator.MultipleOf :
                (value_digit_sum_operator_symbol == "%") ? ComparisonOperator.DivisibleByWithRemainder :
                                                     ComparisonOperator.Reserved;
            string value_digital_root_operator_symbol = FindByNumbersValueDigitalRootComparisonOperatorLabel.Text;
            ComparisonOperator value_digital_root_comparison_operator =
                (value_digital_root_operator_symbol == "=") ? ComparisonOperator.Equal :
                (value_digital_root_operator_symbol == "≠") ? ComparisonOperator.NotEqual :
                (value_digital_root_operator_symbol == "<") ? ComparisonOperator.LessThan :
                (value_digital_root_operator_symbol == "≤") ? ComparisonOperator.LessThanOrEqual :
                (value_digital_root_operator_symbol == ">") ? ComparisonOperator.GreaterThan :
                (value_digital_root_operator_symbol == "≥") ? ComparisonOperator.GreaterThanOrEqual :
                (value_digital_root_operator_symbol == "X") ? ComparisonOperator.MultipleOf :
                (value_digital_root_operator_symbol == "%") ? ComparisonOperator.DivisibleByWithRemainder :
                                                        ComparisonOperator.Reserved;

            // 4. remainders for % comparison operator
            int number_remainder = -1;
            if (number_comparison_operator == ComparisonOperator.DivisibleByWithRemainder)
            {
                try
                {
                    number_remainder = int.Parse(FindByNumbersNumberNumberTypeLabel.Text);
                }
                catch
                {
                    // keep as 0
                }
            }
            int chapter_count_remainder = -1;
            if (chapter_count_comparison_operator == ComparisonOperator.DivisibleByWithRemainder)
            {
                try
                {
                    chapter_count_remainder = int.Parse(FindByNumbersChaptersNumberTypeLabel.Text);
                }
                catch
                {
                    // keep as -1
                }
            }
            int verse_count_remainder = -1;
            if (verse_count_comparison_operator == ComparisonOperator.DivisibleByWithRemainder)
            {
                try
                {
                    verse_count_remainder = int.Parse(FindByNumbersVersesNumberTypeLabel.Text);
                }
                catch
                {
                    // keep as -1
                }
            }
            int word_count_remainder = -1;
            if (word_count_comparison_operator == ComparisonOperator.DivisibleByWithRemainder)
            {
                try
                {
                    word_count_remainder = int.Parse(FindByNumbersWordsNumberTypeLabel.Text);
                }
                catch
                {
                    // keep as -1
                }
            }
            int letter_count_remainder = -1;
            if (letter_count_comparison_operator == ComparisonOperator.DivisibleByWithRemainder)
            {
                try
                {
                    letter_count_remainder = int.Parse(FindByNumbersLettersNumberTypeLabel.Text);
                }
                catch
                {
                    // keep as -1
                }
            }
            int unique_letter_count_remainder = -1;
            if (unique_letter_count_comparison_operator == ComparisonOperator.DivisibleByWithRemainder)
            {
                try
                {
                    unique_letter_count_remainder = int.Parse(FindByNumbersUniqueLettersNumberTypeLabel.Text);
                }
                catch
                {
                    // keep as -1
                }
            }
            int value_remainder = -1;
            if (value_comparison_operator == ComparisonOperator.DivisibleByWithRemainder)
            {
                try
                {
                    value_remainder = int.Parse(FindByNumbersValueNumberTypeLabel.Text);
                }
                catch
                {
                    // keep as -1
                }
            }
            int value_digit_sum_remainder = -1;
            if (value_digit_sum_comparison_operator == ComparisonOperator.DivisibleByWithRemainder)
            {
                try
                {
                    value_digit_sum_remainder = int.Parse(FindByNumbersValueDigitSumNumberTypeLabel.Text);
                }
                catch
                {
                    // keep as -1
                }
            }
            int value_digital_root_remainder = -1;
            if (value_digital_root_comparison_operator == ComparisonOperator.DivisibleByWithRemainder)
            {
                try
                {
                    value_digital_root_remainder = int.Parse(FindByNumbersValueDigitalRootNumberTypeLabel.Text);
                }
                catch
                {
                    // keep as -1
                }
            }


            string text = null;
            text += "number" + number_operator_symbol + ((number > 0) ? number.ToString() : ((number_number_type != NumberType.None) ? FindByNumbersNumberNumberTypeLabel.Text : "*")) + " ";

            if (
                (m_numbers_result_type == NumbersResultType.ChapterRanges) ||
                (m_numbers_result_type == NumbersResultType.ChapterSets)
               )
            {
                text += "chapters" + chapter_count_operator_symbol + ((chapter_count > 0) ? chapter_count.ToString() : ((chapter_count_number_type != NumberType.None) ? FindByNumbersChaptersNumberTypeLabel.Text : "*")) + " ";
            }

            if (
                (m_numbers_result_type == NumbersResultType.Chapters) ||
                (m_numbers_result_type == NumbersResultType.ChapterRanges) ||
                (m_numbers_result_type == NumbersResultType.ChapterSets) ||
                (m_numbers_result_type == NumbersResultType.VerseRanges) ||
                (m_numbers_result_type == NumbersResultType.VerseSets)
               )
            {
                text += "verses" + verse_count_operator_symbol + ((verse_count > 0) ? verse_count.ToString() : ((verse_count_number_type != NumberType.None) ? FindByNumbersVersesNumberTypeLabel.Text : "*")) + " ";
            }

            if (
                (m_numbers_result_type == NumbersResultType.Chapters) ||
                (m_numbers_result_type == NumbersResultType.ChapterRanges) ||
                (m_numbers_result_type == NumbersResultType.ChapterSets) ||
                (m_numbers_result_type == NumbersResultType.Verses) ||
                (m_numbers_result_type == NumbersResultType.VerseRanges) ||
                (m_numbers_result_type == NumbersResultType.VerseSets) ||
                (m_numbers_result_type == NumbersResultType.Sentences) ||
                (m_numbers_result_type == NumbersResultType.WordRanges) ||
                (m_numbers_result_type == NumbersResultType.WordSets)
               )
            {
                text += "words" + word_count_operator_symbol + ((word_count > 0) ? word_count.ToString() : ((word_count_number_type != NumberType.None) ? FindByNumbersWordsNumberTypeLabel.Text : "*")) + " ";
            }

            text += "letters" + letter_count_operator_symbol + ((letter_count > 0) ? letter_count.ToString() : ((letter_count_number_type != NumberType.None) ? FindByNumbersLettersNumberTypeLabel.Text : "*")) + " ";
            text += "unique" + unique_letter_count_operator_symbol + ((unique_letter_count > 0) ? unique_letter_count.ToString() : ((unique_letter_count_number_type != NumberType.None) ? FindByNumbersUniqueLettersNumberTypeLabel.Text : "*")) + " ";
            text += "value" + value_operator_symbol + ((value > 0) ? value.ToString() : ((value_number_type != NumberType.None) ? FindByNumbersValueNumberTypeLabel.Text : "*")) + " ";
            text += "digit_sum" + value_digit_sum_operator_symbol + ((value_digit_sum > 0) ? value_digit_sum.ToString() : ((value_digit_sum_number_type != NumberType.None) ? FindByNumbersValueDigitSumNumberTypeLabel.Text : "*")) + " ";
            text += "digital_root" + value_digital_root_operator_symbol + ((value_digital_root > 0) ? value_digital_root.ToString() : ((value_digital_root_number_type != NumberType.None) ? FindByNumbersValueDigitalRootNumberTypeLabel.Text : "*")) + "";

            NumberQuery query = new NumberQuery();
            query.Number = number;
            query.ChapterCount = chapter_count;
            query.VerseCount = verse_count;
            query.WordCount = word_count;
            query.LetterCount = letter_count;
            query.UniqueLetterCount = unique_letter_count;
            query.Value = value;
            query.ValueDigitSum = value_digit_sum;
            query.ValueDigitalRoot = value_digital_root;
            query.NumberRemainder = number_remainder;
            query.ChapterCountRemainder = chapter_count_remainder;
            query.VerseCountRemainder = verse_count_remainder;
            query.WordCountRemainder = word_count_remainder;
            query.LetterCountRemainder = letter_count_remainder;
            query.UniqueLetterCountRemainder = unique_letter_count_remainder;
            query.ValueRemainder = value_remainder;
            query.ValueDigitSumRemainder = value_digit_sum_remainder;
            query.ValueDigitalRootRemainder = value_digital_root_remainder;
            query.NumberNumberType = number_number_type;
            query.ChapterCountNumberType = chapter_count_number_type;
            query.VerseCountNumberType = verse_count_number_type;
            query.WordCountNumberType = word_count_number_type;
            query.LetterCountNumberType = letter_count_number_type;
            query.UniqueLetterCountNumberType = unique_letter_count_number_type;
            query.ValueNumberType = value_number_type;
            query.ValueDigitSumNumberType = value_digit_sum_number_type;
            query.ValueDigitalRootNumberType = value_digital_root_number_type;
            query.NumberComparisonOperator = number_comparison_operator;
            query.ChapterCountComparisonOperator = chapter_count_comparison_operator;
            query.VerseCountComparisonOperator = verse_count_comparison_operator;
            query.WordCountComparisonOperator = word_count_comparison_operator;
            query.LetterCountComparisonOperator = letter_count_comparison_operator;
            query.UniqueLetterCountComparisonOperator = unique_letter_count_comparison_operator;
            query.ValueComparisonOperator = value_comparison_operator;
            query.ValueDigitSumComparisonOperator = value_digit_sum_comparison_operator;
            query.ValueDigitalRootComparisonOperator = value_digital_root_comparison_operator;

            if (query.IsValid(m_numbers_result_type))
            {
                int match_count = -1;
                switch (m_numbers_result_type)
                {
                    case NumbersResultType.Words:
                        {
                            match_count = m_client.FindWords(query);
                            if (m_client.FoundWords != null)
                            {
                                if (m_client.FoundVerses != null)
                                {
                                    m_find_result_header = match_count + ((match_count == 1) ? " word" : " words") + " in " + m_client.FoundVerses.Count + ((m_client.FoundVerses.Count == 1) ? " verse" : " verses") + " with " + text + " in " + m_client.SearchScope.ToString();
                                    DisplayFoundVerses(true, true);
                                }
                            }
                        }
                        break;
                    case NumbersResultType.WordRanges:
                        {
                            match_count = m_client.FindWordRanges(query);
                            if (m_client.FoundWordRanges != null)
                            {
                                if (m_client.FoundVerses != null)
                                {
                                    m_find_result_header = match_count + ((match_count == 1) ? " word range" : " word ranges") + " in " + m_client.FoundVerses.Count + ((m_client.FoundVerses.Count == 1) ? " verse" : " verses") + " with " + text + " in " + m_client.SearchScope.ToString();
                                    DisplayFoundVerses(true, true);
                                }
                            }
                        }
                        break;
                    case NumbersResultType.WordSets:
                        {
                            match_count = m_client.FindWordSets(query);
                            if (m_client.FoundWordSets != null)
                            {
                                if (m_client.FoundVerses != null)
                                {
                                    m_find_result_header = match_count + ((match_count == 1) ? " word set" : " word sets") + " in " + m_client.FoundVerses.Count + ((m_client.FoundVerses.Count == 1) ? " verse" : " verses") + " with " + text + " in " + m_client.SearchScope.ToString();
                                    DisplayFoundVerses(true, true); //??? Don't display result, show quickly in .txt file
                                }
                            }
                        }
                        break;
                    case NumbersResultType.Sentences:
                        {
                            match_count = m_client.FindSentences(query);
                            if (m_client.FoundSentences != null)
                            {
                                m_find_result_header = match_count + ((match_count == 1) ? " sentence" : " sentences") + " with " + text + " in " + m_client.SearchScope.ToString();
                                DisplayFoundVerses(true, true);
                            }
                        }
                        break;
                    case NumbersResultType.Verses:
                        {
                            match_count = m_client.FindVerses(query);
                            if (m_client.FoundVerses != null)
                            {
                                m_find_result_header = match_count + ((match_count == 1) ? " verse" : " verses") + " with " + text + " in " + m_client.SearchScope.ToString();
                                DisplayFoundVerses(true, true);
                            }
                        }
                        break;
                    case NumbersResultType.VerseRanges:
                        {
                            match_count = m_client.FindVerseRanges(query);
                            if (m_client.FoundVerseRanges != null)
                            {
                                m_find_result_header = match_count + ((match_count == 1) ? " verse range" : " verse ranges") + " with " + text + " in " + m_client.SearchScope.ToString();
                                DisplayFoundVerseRanges(true, true);
                            }
                        }
                        break;
                    case NumbersResultType.VerseSets:
                        {
                            match_count = m_client.FindVerseSets(query);
                            if (m_client.FoundVerseSets != null)
                            {
                                m_find_result_header = match_count + ((match_count == 1) ? " verse set" : " verse sets") + " with " + text + " in " + m_client.SearchScope.ToString();
                                DisplayFoundVerseSets(true, true);
                            }
                        }
                        break;
                    case NumbersResultType.Chapters:
                        {
                            match_count = m_client.FindChapters(query);
                            if (m_client.FoundChapters != null)
                            {
                                m_find_result_header = match_count + ((match_count == 1) ? " chapter" : " chapters") + " with " + text + " in " + m_client.SearchScope.ToString();
                                DisplayFoundChapters(true, true);
                            }
                        }
                        break;
                    case NumbersResultType.ChapterRanges:
                        {
                            match_count = m_client.FindChapterRanges(query);
                            if (m_client.FoundChapterRanges != null)
                            {
                                m_find_result_header = match_count + ((match_count == 1) ? " chapter range" : " chapter ranges") + " with " + text + " in " + m_client.SearchScope.ToString();
                                DisplayFoundChapterRanges(true, true);
                            }
                        }
                        break;
                    case NumbersResultType.ChapterSets:
                        {
                            match_count = m_client.FindChapterSets(query);
                            if (m_client.FoundChapterSets != null)
                            {
                                m_find_result_header = match_count + ((match_count == 1) ? " chapter set" : " chapter sets") + " with " + text + " in " + m_client.SearchScope.ToString();
                                DisplayFoundChapterSets(true, true);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 20. Search By Revelation
    ///////////////////////////////////////////////////////////////////////////////
    private RevelationPlace m_find_by_revelation_place = RevelationPlace.Makkah;
    private void FindByRevelationPlaceControls_Enter(object sender, EventArgs e)
    {
        this.AcceptButton = FindByRevelationPlaceButton;

        FindByTextButton.Enabled = false;
        FindBySimilarityButton.Enabled = false;
        FindByNumbersButton.Enabled = false;
        FindByRevelationPlaceButton.Enabled = true;
        FindByProstrationTypeButton.Enabled = false;
        FindByFrequencyButton.Enabled = false;

        AutoCompleteHeaderLabel.Visible = false;
        AutoCompleteListBox.Visible = false;

        ResetFindByTextResultTypeLabels();
        ResetFindBySimilarityResultTypeLabels();
        ResetFindByNumbersResultTypeLabels();
        ResetFindByFrequencyResultTypeLabels();
    }
    private void FindByRevelationPlaceRadioButton_CheckedChanged(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            //FindByRevelation();
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void FindByRevelationPlaceButton_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            FindByRevelation();
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void FindByRevelation()
    {
        m_search_type = SearchType.Revelation;

        if (m_client != null)
        {
            ClearFindMatches();

            if (!FindByRevelationPlaceMakkahRadioButton.Checked && !FindByRevelationPlaceMedinaRadioButton.Checked)
            {
                m_client.FoundChapters = new List<Chapter>(); // empty result
            }
            else
            {
                if (FindByRevelationPlaceMakkahRadioButton.Checked && !FindByRevelationPlaceMedinaRadioButton.Checked)
                {
                    m_find_by_revelation_place = RevelationPlace.Makkah;
                }
                else if (!FindByRevelationPlaceMakkahRadioButton.Checked && FindByRevelationPlaceMedinaRadioButton.Checked)
                {
                    m_find_by_revelation_place = RevelationPlace.Medina;
                }
                else if (FindByRevelationPlaceMakkahRadioButton.Checked && FindByRevelationPlaceMedinaRadioButton.Checked)
                {
                    m_find_by_revelation_place = RevelationPlace.Both;
                }
                m_client.FindChapters(m_find_by_revelation_place);
            }

            if (m_client.FoundChapters != null)
            {
                m_client.FoundVerses = new List<Verse>();
                if (m_client.FoundVerses != null)
                {
                    foreach (Chapter chapter in m_client.FoundChapters)
                    {
                        m_client.FoundVerses.AddRange(chapter.Verses);
                    }

                    int chapter_count = m_client.FoundChapters.Count;
                    m_find_result_header = chapter_count + ((chapter_count == 1) ? " chapter" : " chapters") + " revealed in " + m_find_by_revelation_place.ToString() + " in " + m_client.SearchScope.ToString();

                    DisplayFoundChapters(true, true);
                }
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 21. Search By Prostration
    ///////////////////////////////////////////////////////////////////////////////
    private ProstrationType m_find_by_prostration_type = ProstrationType.None;
    private void FindByProstrationTypeControls_Enter(object sender, EventArgs e)
    {
        this.AcceptButton = FindByProstrationTypeButton;

        FindByTextButton.Enabled = false;
        FindBySimilarityButton.Enabled = false;
        FindByNumbersButton.Enabled = false;
        FindByRevelationPlaceButton.Enabled = false;
        FindByProstrationTypeButton.Enabled = true;
        FindByFrequencyButton.Enabled = false;

        AutoCompleteHeaderLabel.Visible = false;
        AutoCompleteListBox.Visible = false;

        ResetFindByTextResultTypeLabels();
        ResetFindBySimilarityResultTypeLabels();
        ResetFindByNumbersResultTypeLabels();
        ResetFindByFrequencyResultTypeLabels();
    }
    private void FindByProstrationTypeRadioButton_CheckedChanged(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            //FindByProstration();
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void FindByProstrationTypeButton_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            FindByProstration();
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void FindByProstration()
    {
        m_search_type = SearchType.Prostration;

        if (m_client != null)
        {
            ClearFindMatches();

            if (!FindByProstrationTypeObligatoryCheckBox.Checked && !FindByProstrationTypeRecommendedCheckBox.Checked)
            {
                m_find_by_prostration_type = ProstrationType.None;
            }
            else if (FindByProstrationTypeObligatoryCheckBox.Checked && !FindByProstrationTypeRecommendedCheckBox.Checked)
            {
                m_find_by_prostration_type = ProstrationType.Obligatory;
            }
            else if (!FindByProstrationTypeObligatoryCheckBox.Checked && FindByProstrationTypeRecommendedCheckBox.Checked)
            {
                m_find_by_prostration_type = ProstrationType.Recommended;
            }
            else if (FindByProstrationTypeObligatoryCheckBox.Checked && FindByProstrationTypeRecommendedCheckBox.Checked)
            {
                m_find_by_prostration_type = ProstrationType.Obligatory | ProstrationType.Recommended;
            }

            m_client.FindVerses(m_find_by_prostration_type);
            if (m_client.FoundVerses != null)
            {
                int verse_count = m_client.FoundVerses.Count;
                m_find_result_header = verse_count + ((verse_count == 1) ? " verse" : " verses") + " with " + m_find_by_prostration_type.ToString() + " prostrations" + " in " + m_client.SearchScope.ToString();
                DisplayFoundVerses(true, true);
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 22. Search By Frequency
    ///////////////////////////////////////////////////////////////////////////////
    private FrequencyResultType m_frequency_result_type = FrequencyResultType.Chapters;
    private void FindByFrequencyResultTypeWordsLabel_Click(object sender, EventArgs e)
    {
        ResetFindByFrequencyResultTypeLabels();
        m_frequency_result_type = FrequencyResultType.Words;
        FindByFrequencyControls_Enter(null, null);
    }
    private void FindByFrequencyResultTypeSentencesLabel_Click(object sender, EventArgs e)
    {
        ResetFindByFrequencyResultTypeLabels();
        m_frequency_result_type = FrequencyResultType.Sentences;
        FindByFrequencyControls_Enter(null, null);
    }
    private void FindByFrequencyResultTypeVersesLabel_Click(object sender, EventArgs e)
    {
        ResetFindByFrequencyResultTypeLabels();
        m_frequency_result_type = FrequencyResultType.Verses;
        FindByFrequencyControls_Enter(null, null);
    }
    private void FindByFrequencyResultTypeChaptersLabel_Click(object sender, EventArgs e)
    {
        ResetFindByFrequencyResultTypeLabels();
        m_frequency_result_type = FrequencyResultType.Chapters;
        FindByFrequencyControls_Enter(null, null);
    }
    private FrequencySearchType m_frequency_search_type = FrequencySearchType.DuplicateLetters;
    private void FindByFrequencySearchTypeDuplicateLettersLabel_Click(object sender, EventArgs e)
    {
        ResetFindByFrequencyResultTypeLabels();
        m_frequency_search_type = FrequencySearchType.DuplicateLetters;
        FindByFrequencyControls_Enter(null, null);

        BuildLetterFrequencies();
        DisplayLetterFrequencies();
    }
    private void FindByFrequencySearchTypeUniqueLettersLabel_Click(object sender, EventArgs e)
    {
        ResetFindByFrequencyResultTypeLabels();
        m_frequency_search_type = FrequencySearchType.UniqueLetters;
        FindByFrequencyControls_Enter(null, null);

        BuildLetterFrequencies();
        DisplayLetterFrequencies();
    }

    private string m_phrase_text = "";
    private bool m_find_by_phrase = false;
    private void FindByFrequencyPhraseCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_find_by_phrase = !m_find_by_phrase;

        ResetFindByFrequencyResultTypeLabels();
        m_frequency_result_type = (m_find_by_phrase) ? FrequencyResultType.Sentences : FrequencyResultType.Chapters;
        FindByFrequencyControls_Enter(null, null);

        int shift = 46;
        if (m_find_by_phrase)
        {
            FindByFrequencyPhraseTextBox.TextChanged -= new EventHandler(FindByFrequencyPhraseTextBox_TextChanged);
            FindByFrequencyPhraseTextBox.Text = m_phrase_text;
            FindByFrequencyPhraseTextBox.TextChanged += new EventHandler(FindByFrequencyPhraseTextBox_TextChanged);

            FindByFrequencySumLabel.Top += shift;
            FindByFrequencySumComparisonOperatorLabel.Top += shift;
            FindByFrequencySumNumericUpDown.Top += shift;
            FindByFrequencySumNumberTypeLabel.Top += shift;
            LetterFrequencyListView.Top += shift;
            LetterFrequencyListView.Height -= shift;

            FindByFrequencySumLabel.BackColor = Color.Pink;
            LetterFrequencyListView.BackColor = Color.Pink;
            LetterFrequencyListView.FullRowSelect = false;
            LetterFrequencyPanel.BackColor = Color.Pink;
            LetterFrequencyPanel.Refresh();
        }
        else
        {
            FindByFrequencyPhraseTextBox.TextChanged -= new EventHandler(FindByFrequencyPhraseTextBox_TextChanged);
            FindByFrequencyPhraseTextBox.Text = "";
            FindByFrequencyPhraseTextBox.TextChanged += new EventHandler(FindByFrequencyPhraseTextBox_TextChanged);

            FindByFrequencySumLabel.Top -= shift;
            FindByFrequencySumComparisonOperatorLabel.Top -= shift;
            FindByFrequencySumNumericUpDown.Top -= shift;
            FindByFrequencySumNumberTypeLabel.Top -= shift;
            LetterFrequencyListView.Top -= shift;
            LetterFrequencyListView.Height += shift;

            FindByFrequencySumLabel.BackColor = Color.LightSteelBlue;
            LetterFrequencyListView.BackColor = Color.LightSteelBlue;
            LetterFrequencyListView.FullRowSelect = true;
            LetterFrequencyPanel.BackColor = Color.LightSteelBlue;
            LetterFrequencyPanel.Refresh();
        }

        FindByFrequencyLinkLabel.Visible = m_find_by_phrase;
        UpdateFindByFrequencyButtonToolTip();
        FindByFrequencyButton.Enabled = ((m_find_by_phrase) && (m_phrase_text.Length > 0))
                                        ||
                                        ((!m_find_by_phrase) && (LetterFrequencyListView.SelectedItems.Count > 0));
    }
    private void ResetFindByFrequencyResultTypeLabels()
    {
        FindByFrequencyResultTypeWordsLabel.BackColor = Color.DarkGray;
        FindByFrequencyResultTypeWordsLabel.BorderStyle = BorderStyle.None;

        FindByFrequencyResultTypeSentencesLabel.BackColor = Color.DarkGray;
        FindByFrequencyResultTypeSentencesLabel.BorderStyle = BorderStyle.None;

        FindByFrequencyResultTypeVersesLabel.BackColor = Color.DarkGray;
        FindByFrequencyResultTypeVersesLabel.BorderStyle = BorderStyle.None;

        FindByFrequencyResultTypeChaptersLabel.BackColor = Color.DarkGray;
        FindByFrequencyResultTypeChaptersLabel.BorderStyle = BorderStyle.None;

        FindByFrequencySearchTypeDuplicateLettersLabel.BackColor = Color.DarkGray;
        FindByFrequencySearchTypeDuplicateLettersLabel.BorderStyle = BorderStyle.None;

        FindByFrequencySearchTypeUniqueLettersLabel.BackColor = Color.DarkGray;
        FindByFrequencySearchTypeUniqueLettersLabel.BorderStyle = BorderStyle.None;
    }
    private void FindByFrequencyControl_EnabledChanged(object sender, EventArgs e)
    {
        Control control = sender as Control;
        if (control != null)
        {
            control.BackColor = (control.Enabled) ? SystemColors.Window : Color.LightGray;
        }
    }
    private void FindByFrequencyComparisonOperatorLabel_Click(object sender, EventArgs e)
    {
        UpdateSumNumberTypeLabelTag();

        Control control = sender as Control;
        if (control != null)
        {
            if (UpdateComparisonOperator(control))
            {
                if (control == FindByFrequencySumComparisonOperatorLabel)
                {
                    if (FindByFrequencySumComparisonOperatorLabel.Text == "%")
                    {
                        int remainder = -1;
                        FindByFrequencySumNumberTypeLabel.Tag = remainder;
                        FindByFrequencySumNumberTypeLabel.Text = remainder.ToString();
                        FindByFrequencySumNumberTypeLabel.ForeColor = Color.Black;
                        FindByFrequencySumNumberTypeLabel.Enabled = true;
                        ToolTip.SetToolTip(FindByFrequencySumNumberTypeLabel, "remainder");
                    }
                    else
                    {
                        FindByFrequencySumNumberTypeLabel.Text = "";
                        ToolTip.SetToolTip(FindByFrequencySumNumberTypeLabel, null);
                    }
                }

                FindByFrequencyControls_Enter(null, null);
            }
        }
    }
    private void FindByFrequencyNumberTypeLabel_Click(object sender, EventArgs e)
    {
        UpdateSumNumberTypeLabelTag();

        Control control = sender as Control;
        if (control != null)
        {
            if (UpdateNumberType(control))
            {
                if (control == FindByFrequencySumNumberTypeLabel)
                {
                    FindByFrequencySumComparisonOperatorLabel.Enabled = (control.Text.Length == 0);
                    FindByFrequencySumNumericUpDown.Enabled = (control.Text == "");
                    if (control.Text.Length > 0)
                    {
                        FindByFrequencySumComparisonOperatorLabel.Text = "=";
                        FindByFrequencySumNumericUpDown.Value = 0;
                    }
                    else
                    {
                        FindByFrequencySumNumericUpDown.Focus();
                    }
                }
                else
                {
                    // do nothing
                }

                FindByFrequencyControls_Enter(null, null);
            }
        }
    }
    private void FindByFrequencyControls_Enter(object sender, EventArgs e)
    {
        this.AcceptButton = FindByFrequencyButton;

        FindByTextButton.Enabled = false;
        FindBySimilarityButton.Enabled = false;
        FindByNumbersButton.Enabled = false;
        FindByRevelationPlaceButton.Enabled = false;
        FindByProstrationTypeButton.Enabled = false;
        FindByFrequencyButton.Enabled = ((m_find_by_phrase) && (m_phrase_text.Length > 0))
                                        ||
                                        ((!m_find_by_phrase) && (LetterFrequencyListView.SelectedItems.Count > 0));

        AutoCompleteHeaderLabel.Visible = false;
        AutoCompleteListBox.Visible = false;

        ResetFindByTextResultTypeLabels();
        ResetFindBySimilarityResultTypeLabels();
        ResetFindByNumbersResultTypeLabels();
        ResetFindByFrequencyResultTypeLabels();

        switch (m_frequency_result_type)
        {
            case FrequencyResultType.Words:
                {
                    FindByFrequencyResultTypeWordsLabel.BackColor = Color.SteelBlue;
                    FindByFrequencyResultTypeWordsLabel.BorderStyle = BorderStyle.Fixed3D;
                    ToolTip.SetToolTip(FindByFrequencyButton, "Find words with selected letters\r\nwith required letter frequency sum");
                    ToolTip.SetToolTip(FindByFrequencyPhraseTextBox, "phrase to find its letter frequency sum in words");
                }
                break;
            case FrequencyResultType.Sentences:
                {
                    FindByFrequencyResultTypeSentencesLabel.BackColor = Color.SteelBlue;
                    FindByFrequencyResultTypeSentencesLabel.BorderStyle = BorderStyle.Fixed3D;
                    ToolTip.SetToolTip(FindByFrequencyButton, "Find sentence with selected letters\r\nwith required letter frequency sum");
                    ToolTip.SetToolTip(FindByFrequencyPhraseTextBox, "phrase to find its letter frequency sum in sentences");
                }
                break;
            case FrequencyResultType.Verses:
                {
                    FindByFrequencyResultTypeVersesLabel.BackColor = Color.SteelBlue;
                    FindByFrequencyResultTypeVersesLabel.BorderStyle = BorderStyle.Fixed3D;
                    ToolTip.SetToolTip(FindByFrequencyButton, "Find verses with selected letters\r\nwith required letter frequency sum");
                    ToolTip.SetToolTip(FindByFrequencyPhraseTextBox, "phrase to find its letter frequency sum in verses");
                }
                break;
            case FrequencyResultType.Chapters:
                {
                    FindByFrequencyResultTypeChaptersLabel.BackColor = Color.SteelBlue;
                    FindByFrequencyResultTypeChaptersLabel.BorderStyle = BorderStyle.Fixed3D;
                    ToolTip.SetToolTip(FindByFrequencyButton, "Find chapters with selected letters\r\nwith required letter frequency sum");
                    ToolTip.SetToolTip(FindByFrequencyPhraseTextBox, "phrase to find its letter frequency sum in chapters");
                }
                break;
        }

        switch (m_frequency_search_type)
        {
            case FrequencySearchType.DuplicateLetters:
                {
                    FindByFrequencySearchTypeDuplicateLettersLabel.BackColor = Color.PaleVioletRed;
                    FindByFrequencySearchTypeDuplicateLettersLabel.BorderStyle = BorderStyle.Fixed3D;
                }
                break;
            case FrequencySearchType.UniqueLetters:
                {
                    FindByFrequencySearchTypeUniqueLettersLabel.BackColor = Color.PaleVioletRed;
                    FindByFrequencySearchTypeUniqueLettersLabel.BorderStyle = BorderStyle.Fixed3D;
                }
                break;
        }
    }
    private void FindByFrequencySumNumericUpDown_Leave(object sender, EventArgs e)
    {
        NumericUpDown control = sender as NumericUpDown;
        if (control != null)
        {
            if (String.IsNullOrEmpty(control.Text))
            {
                control.Value = 0;
                control.Refresh();
            }
        }

        UpdateSumNumberTypeLabelTag();
    }
    private void FindByFrequencyPhraseLabel_Click(object sender, EventArgs e)
    {
        LinkLabel_Click(sender, null);
    }
    private void FindByFrequencyPhraseTextBox_TextChanged(object sender, EventArgs e)
    {
        m_phrase_text = FindByFrequencyPhraseTextBox.Text;
        FindByFrequencyButton.Enabled = ((m_find_by_phrase) && (m_phrase_text.Length > 0))
                                        ||
                                        ((!m_find_by_phrase) && (LetterFrequencyListView.SelectedItems.Count > 0));

        BuildLetterFrequencies();
        DisplayLetterFrequencies();
    }

    // simulate SelectionChanged
    private string m_current_phrase = "";
    private void RebuildLetterFrequencies()
    {
        if (FindByFrequencyPhraseTextBox.SelectedText.Length == 0)
        {
            m_current_phrase = FindByFrequencyPhraseTextBox.Text;
        }

        if (m_current_phrase != FindByFrequencyPhraseTextBox.SelectedText)
        {
            BuildLetterFrequencies();
            DisplayLetterFrequencies();
        }
    }
    private void FindByFrequencyPhraseTextBox_KeyUp(object sender, KeyEventArgs e)
    {
        RebuildLetterFrequencies();
    }
    private void FindByFrequencyPhraseTextBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (MouseButtons == MouseButtons.Left)
        {
            RebuildLetterFrequencies();
        }
    }
    private void FindByFrequencyPhraseTextBox_MouseUp(object sender, MouseEventArgs e)
    {
        RebuildLetterFrequencies();
    }
    private void FindByNumbersNumberNumberTypeLabel_MouseEnter(object sender, EventArgs e)
    {

    }
    private void FindByNumbersNumberNumberTypeLabel_MouseLeave(object sender, EventArgs e)
    {

    }

    private void FindByFrequencySumNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
        UpdateSumNumberTypeLabelTag();

        //// don't auto-find as user may not have finished setting all parameters yet
        //if (sender == FindByFrequencySumNumericUpDown)
        //{
        //    this.Cursor = Cursors.WaitCursor;
        //    try
        //    {
        //        FindByFrequencySum();
        //    }
        //    finally
        //    {
        //        this.Cursor = Cursors.Default;
        //    }
        //}
    }
    private void UpdateSumNumberTypeLabelTag()
    {
        FindByFrequencySumNumberTypeLabel.Tag = (int)FindByFrequencySumNumericUpDown.Value;

        int number;
        if (int.TryParse(FindByFrequencySumNumberTypeLabel.Text, out number))
        {
            if (number >= (int)FindByFrequencySumNumericUpDown.Value)
            {
                number = (int)FindByFrequencySumNumericUpDown.Value - 1;
                if (number < 0) number = 0;
                FindByFrequencySumNumberTypeLabel.Text = number.ToString();
            }
        }
    }
    private void UpdateFindByFrequencyButtonToolTip()
    {
        string tooltip = ToolTip.GetToolTip(FindByFrequencyButton);
        if (tooltip != null)
        {
            if (m_find_by_phrase)
            {
                tooltip = tooltip.Replace("selected", "phrase");
            }
            else
            {
                tooltip = tooltip.Replace("phrase", "selected");
            }
        }
        ToolTip.SetToolTip(FindByFrequencyButton, tooltip);
    }
    private void FindByFrequencyButton_EnabledChanged(object sender, EventArgs e)
    {
        UpdateFindByFrequencyButtonToolTip();
    }
    private void FindByFrequencyButton_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            FindByFrequency();
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void FindByFrequency()
    {
        m_search_type = SearchType.Frequency;

        if (m_client != null)
        {
            ClearFindMatches();

            // 1. number types
            string sum_number_type_symbol = FindByFrequencySumNumberTypeLabel.Enabled ? FindByFrequencySumNumberTypeLabel.Text : "";
            NumberType sum_number_type =
                (sum_number_type_symbol == "PP") ? NumberType.PurePrime :
                (sum_number_type_symbol == "AP") ? NumberType.AdditivePrime :
                (sum_number_type_symbol == "P") ? NumberType.Prime :
                (sum_number_type_symbol == "PC") ? NumberType.PureComposite :
                (sum_number_type_symbol == "AC") ? NumberType.AdditiveComposite :
                (sum_number_type_symbol == "C") ? NumberType.Composite :
                (sum_number_type_symbol == "") ? NumberType.None :
                                             NumberType.Any;

            // 2. numbers
            int sum = (int)FindByFrequencySumNumericUpDown.Value;

            // 3. comparison operators = ≠ < ≤ > ≥ X %
            string sum_comparison_operator_symbol = FindByFrequencySumComparisonOperatorLabel.Text;
            ComparisonOperator sum_comparison_operator =
                (sum_comparison_operator_symbol == "=") ? ComparisonOperator.Equal :
                (sum_comparison_operator_symbol == "≠") ? ComparisonOperator.NotEqual :
                (sum_comparison_operator_symbol == "<") ? ComparisonOperator.LessThan :
                (sum_comparison_operator_symbol == "≤") ? ComparisonOperator.LessThanOrEqual :
                (sum_comparison_operator_symbol == ">") ? ComparisonOperator.GreaterThan :
                (sum_comparison_operator_symbol == "≥") ? ComparisonOperator.GreaterThanOrEqual :
                (sum_comparison_operator_symbol == "X") ? ComparisonOperator.MultipleOf :
                (sum_comparison_operator_symbol == "%") ? ComparisonOperator.DivisibleByWithRemainder :
                                                          ComparisonOperator.Reserved;

            // 4. remainders for % comparison operator
            int sum_remainder = -1;
            if (sum_comparison_operator == ComparisonOperator.DivisibleByWithRemainder)
            {
                try
                {
                    sum_remainder = int.Parse(FindByFrequencySumNumberTypeLabel.Text);
                }
                catch
                {
                    // keep as -1
                }
            }

            string phrase = "";
            if (FindByFrequencyPhraseTextBox.Text.Length > 0)
            {
                if (FindByFrequencyPhraseTextBox.SelectedText.Length > 0)
                {
                    phrase = FindByFrequencyPhraseTextBox.SelectedText.Trim();
                }
                else
                {
                    phrase = FindByFrequencyPhraseTextBox.Text.Trim();
                }
            }
            else
            {
                if (LetterFrequencyListView.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem item in LetterFrequencyListView.SelectedItems)
                    {
                        phrase += item.SubItems[1].Text;
                    }
                }
            }

            if (!String.IsNullOrEmpty(phrase))
            {
                if (phrase.IsArabic())
                {
                    string text = null;
                    text += phrase + " letters" + sum_comparison_operator_symbol + ((sum > 0) ? sum.ToString() : ((sum_number_type != NumberType.None) ? FindByFrequencySumNumberTypeLabel.Text : "*"))
                         + ((m_frequency_search_type == FrequencySearchType.DuplicateLetters) ? " with " : "without ") + "duplicates";

                    switch (m_frequency_result_type)
                    {
                        case FrequencyResultType.Words:
                            {
                                int match_count = m_client.FindWords(phrase, sum, sum_number_type, sum_comparison_operator, sum_remainder, m_frequency_search_type);
                                if (m_client.FoundWords != null)
                                {
                                    m_find_result_header = match_count + ((match_count == 1) ? " word" : " words") + " with " + text + " in " + m_client.SearchScope.ToString();
                                    DisplayFoundVerses(true, true);
                                }
                            }
                            break;
                        case FrequencyResultType.Sentences:
                            {
                                int match_count = m_client.FindSentences(phrase, sum, sum_number_type, sum_comparison_operator, sum_remainder, m_frequency_search_type);
                                if (m_client.FoundSentences != null)
                                {
                                    m_find_result_header = match_count + ((match_count == 1) ? " sentence" : " sentences") + " with " + text + " in " + m_client.SearchScope.ToString();
                                    DisplayFoundVerses(true, true);
                                }
                            }
                            break;
                        case FrequencyResultType.Verses:
                            {
                                int match_count = m_client.FindVerses(phrase, sum, sum_number_type, sum_comparison_operator, sum_remainder, m_frequency_search_type);
                                if (m_client.FoundVerses != null)
                                {
                                    m_find_result_header = match_count + ((match_count == 1) ? " verse" : " verses") + " with " + text + " in " + m_client.SearchScope.ToString();
                                    DisplayFoundVerses(true, true);
                                }
                            }
                            break;
                        case FrequencyResultType.Chapters:
                            {
                                int match_count = m_client.FindChapters(phrase, sum, sum_number_type, sum_comparison_operator, sum_remainder, m_frequency_search_type);
                                if (m_client.FoundChapters != null)
                                {
                                    m_find_result_header = match_count + ((match_count == 1) ? " chapter" : " chapters") + " with " + text + " in " + m_client.SearchScope.ToString();
                                    DisplayFoundChapters(true, true);
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 23. Display Search Results
    ///////////////////////////////////////////////////////////////////////////////
    // F3 and Shift+F3 Goto next/previous matches
    private struct FindMatch
    {
        public int Start;
        public int Length;
    }
    private List<FindMatch> m_find_matches = null;
    private void BuildFindMatch(int start, int length)
    {
        // build text_selections list for F3 and Shift+F3
        if (m_find_matches != null)
        {
            FindMatch find_match = new FindMatch();
            find_match.Start = start;
            find_match.Length = length;
            m_find_matches.Add(find_match);
        }
    }
    private int m_find_match_index = -1;
    private void GotoPreviousFindMatch()
    {
        if (m_find_matches != null)
        {
            m_find_match_index = -1;
            for (int i = 0; i < m_find_matches.Count; i++)
            {
                if (m_find_matches[i].Start > SearchResultTextBox.SelectionStart)
                {
                    m_find_match_index = i - 1;
                    break;
                }
            }
        }
    }
    private void GotoNextFindMatch()
    {
        if (m_find_matches != null)
        {
            m_find_match_index = m_find_matches.Count;
            for (int i = m_find_matches.Count - 1; i >= 0; i--)
            {
                if (m_find_matches[i].Start < SearchResultTextBox.SelectionStart)
                {
                    m_find_match_index = i + 1;
                    break;
                }
            }
        }
    }
    private void SelectNextFindMatch()
    {
        if (m_found_verses_displayed)
        {
            if (m_find_matches != null)
            {
                if (m_find_matches.Count > 0)
                {
                    // find the index prior to the current cursor postion
                    GotoPreviousFindMatch();
                    m_find_match_index++;

                    // round robin
                    if (m_find_match_index == m_find_matches.Count)
                    {
                        m_find_match_index = 0;
                    }

                    // find next match
                    if ((m_find_match_index >= 0) && (m_find_match_index < m_find_matches.Count))
                    {
                        int start = m_find_matches[m_find_match_index].Start;
                        int length = m_find_matches[m_find_match_index].Length;
                        if ((start >= 0) && (start < SearchResultTextBox.Text.Length))
                        {
                            SearchResultTextBox.Select(start, length);
                            SearchResultTextBox.SelectionColor = Color.Red;
                        }
                    }
                }
            }
        }
        UpdateFindMatchCaption();
    }
    private void SelectPreviousFindMatch()
    {
        if (m_found_verses_displayed)
        {
            if (m_find_matches != null)
            {
                if (m_find_matches.Count > 0)
                {
                    // find the index after the current cursor postion
                    GotoNextFindMatch();
                    m_find_match_index--;

                    // round robin
                    if (m_find_match_index < 0)
                    {
                        m_find_match_index = m_find_matches.Count - 1;
                    }

                    // find previous match
                    if ((m_find_match_index >= 0) && (m_find_match_index < m_find_matches.Count))
                    {
                        int start = m_find_matches[m_find_match_index].Start;
                        int length = m_find_matches[m_find_match_index].Length;
                        if ((start >= 0) && (start < SearchResultTextBox.Text.Length))
                        {
                            SearchResultTextBox.Select(start, length);
                            SearchResultTextBox.SelectionColor = Color.Red;
                        }
                    }
                }
            }
        }
        UpdateFindMatchCaption();
    }
    private void UpdateFindMatchCaption()
    {
        string caption = this.Text;
        int pos = caption.IndexOf(CAPTION_SEPARATOR);
        if (pos > -1)
        {
            caption = caption.Substring(0, pos);
        }

        if (m_found_verses_displayed)
        {
            if (m_find_matches != null)
            {
                caption += CAPTION_SEPARATOR + " Match " + ((m_find_match_index + 1) + "/" + m_find_matches.Count);
            }
        }
        else
        {
            //caption += CAPTION_SEPARATOR;
        }

        this.Text = caption;
    }

    private string m_find_result_header = null;
    private void UpdateHeaderLabel()
    {
        if (m_client != null)
        {
            if (m_found_verses_displayed)
            {
                int count = 0;
                if ((m_client.FoundSentences != null) && (m_client.FoundSentences.Count > 0))
                {
                    count = m_client.FoundSentences.Count;
                }
                else if ((m_client.FoundPhrases != null) && (m_client.FoundPhrases.Count > 0))
                {
                    count = GetPhraseCount(m_client.FoundPhrases);
                }
                else if ((m_client.FoundChapterRanges != null) && (m_client.FoundChapterRanges.Count > 0))
                {
                    count = m_client.FoundChapterRanges.Count;
                }
                else if ((m_client.FoundChapters != null) && (m_client.FoundChapters.Count > 0))
                {
                    count = m_client.FoundChapters.Count;
                }
                else if ((m_client.FoundVerseRanges != null) && (m_client.FoundVerseRanges.Count > 0))
                {
                    count = m_client.FoundVerseRanges.Count;
                }
                else if ((m_client.FoundVerses != null) && (m_client.FoundVerses.Count > 0))
                // leave till end in case of nested searches
                {
                    count = m_client.FoundVerses.Count;
                }
                else
                {
                    count = 0;
                }
                HeaderLabel.ForeColor = GetNumberTypeColor(count);
                HeaderLabel.Text = m_find_result_header;
                HeaderLabel.Refresh();
            }
            else
            {
                HeaderLabel.ForeColor = SystemColors.WindowText;
                if (m_client.Selection != null)
                {
                    if (m_client.Selection.Verses != null)
                    {
                        if (m_client.Selection.Verses.Count > 0)
                        {
                            Verse verse = GetVerse(CurrentVerseIndex);
                            if (verse != null)
                            {
                                if (verse.Chapter != null)
                                {
                                    string display_text = " " + verse.Chapter.TransliteratedName + "/" + verse.Chapter.EnglishName + " " + verse.Chapter.Number;
                                    HeaderLabel.Text = verse.Chapter.Name + " "
                                          + "   ءاية " + verse.NumberInChapter
                                          + "   منزل " + ((verse.Station != null) ? verse.Station.Number : -1)
                                          + "   جزء " + ((verse.Part != null) ? verse.Part.Number : -1)
                                          + "   حزب " + ((verse.Group != null) ? verse.Group.Number : -1)
                                          + "   ربع " + ((verse.Quarter != null) ? verse.Quarter.Number : -1)
                                          + "   ركوع " + ((verse.Bowing != null) ? verse.Bowing.Number : -1)
                                          + "   صفحة " + ((verse.Page != null) ? verse.Page.Number : -1)
                                          + display_text;
                                    HeaderLabel.Refresh();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void SearchScopeLabel_Click(object sender, EventArgs e)
    {
        NextSearchScope();
        UpdateSearchScope();
        FindByTextTextBox_TextChanged(null, null);
    }
    private void NextSearchScope()
    {
        if (m_client != null)
        {
            if (ModifierKeys != Keys.Shift)
            {
                if (m_client.SearchScope == SearchScope.Book)
                {
                    m_client.SearchScope = SearchScope.Selection;
                }
                else if (m_client.SearchScope == SearchScope.Selection)
                {
                    m_client.SearchScope = SearchScope.Result;
                }
                else if (m_client.SearchScope == SearchScope.Result)
                {
                    m_client.SearchScope = SearchScope.Book;
                }
            }
            else
            {
                if (m_client.SearchScope == SearchScope.Result)
                {
                    m_client.SearchScope = SearchScope.Selection;
                }
                else if (m_client.SearchScope == SearchScope.Selection)
                {
                    m_client.SearchScope = SearchScope.Book;
                }
                else if (m_client.SearchScope == SearchScope.Book)
                {
                    m_client.SearchScope = SearchScope.Result;
                }
            }
        }
    }
    private void UpdateSearchScope()
    {
        if (m_client != null)
        {
            if (m_client.SearchScope == SearchScope.Book)
            {
                SearchScopeLabel.Text = "Entire Book";
                this.ToolTip.SetToolTip(SearchScopeLabel, null);
            }
            else if (m_client.SearchScope == SearchScope.Selection)
            {
                SearchScopeLabel.Text = "Selection";
                this.ToolTip.SetToolTip(SearchScopeLabel, GetSearchScope());
            }
            else if (m_client.SearchScope == SearchScope.Result)
            {
                SearchScopeLabel.Text = "Search Result";
                this.ToolTip.SetToolTip(SearchScopeLabel, null);
            }
        }
    }
    private string GetSummarizedSearchScope()
    {
        string result = GetSearchScope();
        if (result != null)
        {
            // trim if too long
            if (result.Length > SELECTON_SCOPE_TEXT_MAX_LENGTH)
            {
                result = result.Substring(0, SELECTON_SCOPE_TEXT_MAX_LENGTH) + " ...";
            }
        }
        return result;
    }
    private string GetSearchScope()
    {
        string result = null;
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                if (m_client.Book.Verses != null)
                {
                    if (m_client.Selection != null)
                    {
                        if (m_client.Selection.Indexes != null)
                        {
                            if ((m_client.Selection.Scope == SelectionScope.Word) || (m_client.Selection.Scope == SelectionScope.Letter))
                            {
                                int verse_number = (int)VerseNumericUpDown.Value;
                                result = "Verse" + " " + m_client.Book.Verses[verse_number - 1].Address;
                            }
                            else // if scope is Chapter, Page, Station, Part, Group, Quarter, Bowing, Verse
                            {
                                StringBuilder str = new StringBuilder();

                                List<int> selection_indexes = new List<int>();
                                if (m_client.Selection.Scope == SelectionScope.Chapter)
                                {
                                    foreach (Chapter chapter in m_client.Selection.Chapters)
                                    {
                                        selection_indexes.Add(chapter.Number - 1);
                                    }
                                }
                                else
                                {
                                    selection_indexes = m_client.Selection.Indexes;
                                }

                                if (Numbers.AreConsecutive(selection_indexes))
                                {
                                    if (m_client.Selection.Indexes.Count > 1)
                                    {
                                        int first_index = m_client.Selection.Indexes[0];
                                        int last_index = m_client.Selection.Indexes[m_client.Selection.Indexes.Count - 1];

                                        if (m_client.Selection.Scope == SelectionScope.Verse)
                                        {
                                            str.Append(m_client.Book.Verses[first_index].Address + " - ");
                                            str.Append(m_client.Book.Verses[last_index].Address);
                                        }
                                        else if (m_client.Selection.Scope == SelectionScope.Chapter)
                                        {
                                            int from_chapter_number = -1;
                                            int to_chapter_number = -1;
                                            int from_compilation_order = first_index + 1;
                                            int to_compilation_order = last_index + 1;
                                            if (m_client.Book.Chapters != null)
                                            {
                                                foreach (Chapter chapter in m_client.Book.Chapters)
                                                {
                                                    if (chapter.Number == from_compilation_order)
                                                    {
                                                        from_chapter_number = chapter.Number;
                                                        break;
                                                    }
                                                }
                                                foreach (Chapter chapter in m_client.Book.Chapters)
                                                {
                                                    if (chapter.Number == to_compilation_order)
                                                    {
                                                        to_chapter_number = chapter.Number;
                                                        break;
                                                    }
                                                }
                                                str.Append(from_chapter_number.ToString() + " - ");
                                                str.Append(to_chapter_number.ToString());
                                            }
                                        }
                                        else
                                        {
                                            str.Append((first_index + 1).ToString() + "-");
                                            str.Append((last_index + 1).ToString());
                                        }
                                    }
                                    else if (m_client.Selection.Indexes.Count == 1)
                                    {
                                        int index = m_client.Selection.Indexes[0];
                                        if (m_client.Selection.Scope == SelectionScope.Verse)
                                        {
                                            str.Append(m_client.Book.Verses[index].Address);
                                        }
                                        else if (m_client.Selection.Scope == SelectionScope.Chapter)
                                        {
                                            int chapter_number = 0;
                                            int compilation_order = index + 1;
                                            if (m_client.Book.Chapters != null)
                                            {
                                                foreach (Chapter chapter in m_client.Book.Chapters)
                                                {
                                                    if (chapter.Number == compilation_order)
                                                    {
                                                        chapter_number = chapter.Number;
                                                        break;
                                                    }
                                                }
                                                str.Append(chapter_number.ToString());
                                            }
                                        }
                                        else
                                        {
                                            str.Append((index + 1).ToString());
                                        }
                                    }
                                    else
                                    {
                                        // do nothing
                                    }
                                }
                                else
                                {
                                    if (m_client.Selection.Indexes.Count > 0)
                                    {
                                        foreach (int index in m_client.Selection.Indexes)
                                        {
                                            if (m_client.Selection.Scope == SelectionScope.Verse)
                                            {
                                                str.Append(m_client.Book.Verses[index].Address + " ");
                                            }
                                            else if (m_client.Selection.Scope == SelectionScope.Chapter)
                                            {
                                                int chapter_number = 0;
                                                int compilation_order = index + 1;
                                                if (m_client.Book.Chapters != null)
                                                {
                                                    foreach (Chapter chapter in m_client.Book.Chapters)
                                                    {
                                                        if (chapter.Number == compilation_order)
                                                        {
                                                            chapter_number = chapter.Number;
                                                            break;
                                                        }
                                                    }
                                                    str.Append(chapter_number.ToString() + " ");
                                                }
                                            }
                                            else
                                            {
                                                str.Append((index + 1).ToString() + " ");
                                            }
                                        }
                                        if (str.Length > 1)
                                        {
                                            str.Remove(str.Length - 1, 1);
                                        }
                                    }

                                    if (m_client.Selection.Scope == SelectionScope.Verse)
                                    {
                                    }
                                    else if (m_client.Selection.Scope == SelectionScope.Chapter)
                                    {
                                    }
                                    else
                                    {
                                    }
                                }

                                if (m_client.Selection.Indexes.Count == 1)
                                {
                                    result = m_client.Selection.Scope.ToString() + " " + str.ToString();
                                }
                                else if (m_client.Selection.Indexes.Count > 1)
                                {
                                    result = m_client.Selection.Scope.ToString() + "s" + " " + str.ToString();
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }

    private RichTextBoxEx m_active_textbox = null;
    private bool m_found_verses_displayed = false;
    private void SwitchToMainTextBox()
    {
        if (m_active_textbox != null)
        {
            if (m_found_verses_displayed)
            {
                m_found_verses_displayed = false;
                PopulateChaptersListBox();
            }

            // in all cases
            SearchResultTextBox.Visible = false;
            SearchResultTextBox.SendToBack();
            MainTextBox.Visible = true;
            MainTextBox.BringToFront();
            m_active_textbox = MainTextBox;

            UpdateWordWrapLabel(m_active_textbox.WordWrap);

            GoldenRatioScopeLabel.Visible = true;
            GoldenRatioOrderLabel.Visible = true;
            GoldenRatioScopeLabel.Refresh();
            GoldenRatioOrderLabel.Refresh();

            this.Text = Application.ProductName + " | " + GetSummarizedSearchScope();
        }
    }
    private void SwitchToSearchResultTextBox()
    {
        if (m_active_textbox != null)
        {
            // allow subsequent Finds to update chapter list, and search history too
            //if (!m_found_verses_displayed)
            //{
            m_found_verses_displayed = true;
            PopulateChaptersListBox();
            //}

            // in all cases
            MainTextBox.Visible = false;
            MainTextBox.SendToBack();
            SearchResultTextBox.Visible = true;
            SearchResultTextBox.BringToFront();
            m_active_textbox = SearchResultTextBox;

            UpdateWordWrapLabel(m_active_textbox.WordWrap);

            GoldenRatioScopeLabel.Visible = false;
            GoldenRatioOrderLabel.Visible = false;
            GoldenRatioScopeLabel.Refresh();
            GoldenRatioOrderLabel.Refresh();

            this.Text = Application.ProductName + " | " + GetSummarizedSearchScope();
            UpdateFindMatchCaption();
        }
    }

    private int[] m_matches_per_chapter = null;
    private void DisplayFoundVerses(bool add_to_history, bool colorize_chapters_by_matches)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_client != null)
            {
                if (m_client.FoundVerses != null)
                {
                    TranslationTextBox.Text = null;
                    ZoomInLabel.Enabled = (m_text_zoom_factor <= (m_max_zoom_factor - m_zoom_factor_increment + m_error_margin));
                    ZoomOutLabel.Enabled = (m_text_zoom_factor >= (m_min_zoom_factor + m_zoom_factor_increment - m_error_margin));

                    if (colorize_chapters_by_matches)
                    {
                        m_matches_per_chapter = new int[m_client.Book.Chapters.Count];
                        if ((m_client.FoundPhrases != null) && (m_client.FoundPhrases.Count > 0))
                        {
                            foreach (Phrase phrase in m_client.FoundPhrases)
                            {
                                if (phrase != null)
                                {
                                    if (phrase.Verse != null)
                                    {
                                        if (phrase.Verse.Chapter != null)
                                        {
                                            m_matches_per_chapter[phrase.Verse.Chapter.Number - 1]++;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (Verse verse in m_client.FoundVerses)
                            {
                                if (verse != null)
                                {
                                    if (verse.Chapter != null)
                                    {
                                        m_matches_per_chapter[verse.Chapter.Number - 1]++;
                                    }
                                }
                            }
                        }

                        SwitchToSearchResultTextBox();
                    }

                    //SearchResultTextBox.TextChanged -= new EventHandler(MainTextBox_TextChanged);
                    SearchResultTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
                    SearchResultTextBox.BeginUpdate();

                    StringBuilder str = new StringBuilder();
                    foreach (Verse verse in m_client.FoundVerses)
                    {
                        if (verse != null)
                        {
                            str.Append(verse.ArabicAddress + "\t" + verse.Text + "\n");
                        }
                    }
                    if (str.Length > 1)
                    {
                        str.Remove(str.Length - 1, 1);
                    }
                    m_current_text = str.ToString();

                    m_selection_mode = true;
                    UpdateHeaderLabel();
                    SearchResultTextBox.Text = m_current_text;

                    CalculateCurrentValue();

                    BuildLetterFrequencies();
                    DisplayLetterFrequencies();

                    if (m_client.FoundPhrases != null)
                    {
                        ColorizePhrases();
                        BuildFindMatches();
                    }

                    m_current_found_verse_index = 0;
                    DisplayCurrentPositions();

                    DisplayTranslations(m_client.FoundVerses);

                    if (add_to_history)
                    {
                        AddSearchHistoryItem();
                    }

                    m_current_found_verse_index = 0;
                    RealignFoundMatchedToStart();

                    BookmarkTextBox.Enabled = false;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            SearchResultTextBox.EndUpdate();
            SearchResultTextBox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
            //SearchResultTextBox.TextChanged += new EventHandler(MainTextBox_TextChanged);
            this.Cursor = Cursors.Default;
        }
    }
    private void DisplayFoundVerseRanges(bool add_to_history, bool colorize_chapters_by_matches)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_client != null)
            {
                if (m_client.FoundVerseRanges != null)
                {
                    TranslationTextBox.Text = null;
                    ZoomInLabel.Enabled = (m_text_zoom_factor <= (m_max_zoom_factor - m_zoom_factor_increment + m_error_margin));
                    ZoomOutLabel.Enabled = (m_text_zoom_factor >= (m_min_zoom_factor + m_zoom_factor_increment - m_error_margin));

                    if (colorize_chapters_by_matches)
                    {
                        m_matches_per_chapter = new int[m_client.Book.Chapters.Count];
                        foreach (List<Verse> range in m_client.FoundVerseRanges)
                        {
                            foreach (Verse verse in range)
                            {
                                if (verse != null)
                                {
                                    if (verse.Chapter != null)
                                    {
                                        m_matches_per_chapter[verse.Chapter.Number - 1]++;
                                    }
                                }
                            }
                        }

                        SwitchToSearchResultTextBox();
                    }

                    //SearchResultTextBox.TextChanged -= new EventHandler(MainTextBox_TextChanged);
                    SearchResultTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
                    SearchResultTextBox.BeginUpdate();

                    StringBuilder str = new StringBuilder();
                    foreach (List<Verse> range in m_client.FoundVerseRanges)
                    {
                        foreach (Verse verse in range)
                        {
                            if (verse != null)
                            {
                                str.Append(verse.ArabicAddress + "\t" + verse.Text + "\n");
                            }
                        }
                    }
                    if (str.Length > 1)
                    {
                        str.Remove(str.Length - 1, 1);
                    }
                    m_current_text = str.ToString();

                    m_selection_mode = true;
                    UpdateHeaderLabel();
                    SearchResultTextBox.Text = m_current_text;

                    CalculateCurrentValue();

                    BuildLetterFrequencies();
                    DisplayLetterFrequencies();

                    ColorizeVerseRanges();

                    m_current_found_verse_index = 0;
                    DisplayCurrentPositions();

                    List<Verse> verses = new List<Verse>();
                    foreach (List<Verse> range in m_client.FoundVerseRanges)
                    {
                        verses.AddRange(range);
                    }
                    DisplayTranslations(verses);

                    if (add_to_history)
                    {
                        AddSearchHistoryItem();
                    }

                    m_current_found_verse_index = 0;
                    RealignFoundMatchedToStart();

                    BookmarkTextBox.Enabled = false;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            SearchResultTextBox.EndUpdate();
            SearchResultTextBox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
            //SearchResultTextBox.TextChanged += new EventHandler(MainTextBox_TextChanged);
            this.Cursor = Cursors.Default;
        }
    }
    private void DisplayFoundVerseSetsTOOSLOW(bool add_to_history, bool colorize_chapters_by_matches)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_client != null)
            {
                if (m_client.FoundVerseSets != null)
                {
                    TranslationTextBox.Text = null;
                    ZoomInLabel.Enabled = (m_text_zoom_factor <= (m_max_zoom_factor - m_zoom_factor_increment + m_error_margin));
                    ZoomOutLabel.Enabled = (m_text_zoom_factor >= (m_min_zoom_factor + m_zoom_factor_increment - m_error_margin));

                    if (colorize_chapters_by_matches)
                    {
                        m_matches_per_chapter = new int[m_client.Book.Chapters.Count];
                        foreach (List<Verse> set in m_client.FoundVerseSets)
                        {
                            foreach (Verse verse in set)
                            {
                                if (verse != null)
                                {
                                    if (verse.Chapter != null)
                                    {
                                        m_matches_per_chapter[verse.Chapter.Number - 1]++;
                                    }
                                }
                            }
                        }

                        SwitchToSearchResultTextBox();
                    }

                    //SearchResultTextBox.TextChanged -= new EventHandler(MainTextBox_TextChanged);
                    SearchResultTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
                    SearchResultTextBox.BeginUpdate();

                    StringBuilder str = new StringBuilder();
                    foreach (List<Verse> set in m_client.FoundVerseSets)
                    {
                        foreach (Verse verse in set)
                        {
                            if (verse != null)
                            {
                                str.Append(verse.ArabicAddress + "\t" + verse.Text + "\n");
                            }
                        }
                    }
                    if (str.Length > 1)
                    {
                        str.Remove(str.Length - 1, 1);
                    }
                    m_current_text = str.ToString();

                    m_selection_mode = true;
                    UpdateHeaderLabel();
                    SearchResultTextBox.Text = m_current_text;

                    CalculateCurrentValue();

                    BuildLetterFrequencies();
                    DisplayLetterFrequencies();

                    ColorizeVerseSets();

                    m_current_found_verse_index = 0;
                    DisplayCurrentPositions();

                    List<Verse> verses = new List<Verse>();
                    foreach (List<Verse> set in m_client.FoundVerseSets)
                    {
                        verses.AddRange(set);
                    }
                    DisplayTranslations(verses);

                    if (add_to_history)
                    {
                        AddSearchHistoryItem();
                    }

                    m_current_found_verse_index = 0;
                    RealignFoundMatchedToStart();

                    BookmarkTextBox.Enabled = false;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            SearchResultTextBox.EndUpdate();
            SearchResultTextBox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
            //SearchResultTextBox.TextChanged += new EventHandler(MainTextBox_TextChanged);
            this.Cursor = Cursors.Default;
        }
    }
    private void DisplayFoundVerseSets(bool add_to_history, bool colorize_chapters_by_matches)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_client != null)
            {
                if (m_client.FoundVerseSets != null)
                {
                    if (m_client.FoundVerseSets.Count > 0)
                    {
                        StringBuilder str = new StringBuilder();
                        foreach (List<Verse> set in m_client.FoundVerseSets)
                        {
                            foreach (Verse verse in set)
                            {
                                if (verse != null)
                                {
                                    str.Append(verse.Address + ", ");
                                }
                            }
                            if (set.Count > 0)
                            {
                                str.Remove(str.Length - 2, 2);
                            }
                            str.AppendLine();
                        }

                        string filename = "FoundVerseSets" + Globals.OUTPUT_FILE_EXT;
                        string path = Globals.STATISTICS_FOLDER + "/" + filename;
                        PublicStorage.SaveText(path, str.ToString());
                        PublicStorage.DisplayFile(path);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void DisplayFoundChapters(bool add_to_history, bool colorize_chapters_by_matches)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_client != null)
            {
                if (m_client.FoundChapters != null)
                {
                    TranslationTextBox.Text = null;
                    ZoomInLabel.Enabled = (m_text_zoom_factor <= (m_max_zoom_factor - m_zoom_factor_increment + m_error_margin));
                    ZoomOutLabel.Enabled = (m_text_zoom_factor >= (m_min_zoom_factor + m_zoom_factor_increment - m_error_margin));

                    if (colorize_chapters_by_matches)
                    {
                        m_matches_per_chapter = new int[m_client.Book.Chapters.Count];
                        foreach (Chapter chapter in m_client.FoundChapters)
                        {
                            if (chapter != null)
                            {
                                m_matches_per_chapter[chapter.Number - 1]++;
                            }
                        }

                        SwitchToSearchResultTextBox();
                    }

                    ChaptersListBox.SelectedIndexChanged -= new EventHandler(ChaptersListBox_SelectedIndexChanged);
                    if (m_client.FoundChapters.Count > 0)
                    {
                        ChaptersListBox.SelectedIndices.Clear();
                        foreach (Chapter chapter in m_client.FoundChapters)
                        {
                            if (((chapter.Number - 1) >= 0) && ((chapter.Number - 1) < ChaptersListBox.Items.Count))
                            {
                                ChaptersListBox.SelectedIndices.Add(chapter.Number - 1);
                            }
                        }
                    }
                    else
                    {
                        ChaptersListBox.SelectedIndices.Clear();
                    }
                    ChaptersListBox.SelectedIndexChanged += new EventHandler(ChaptersListBox_SelectedIndexChanged);
                    UpdateSelection();

                    //SearchResultTextBox.TextChanged -= new EventHandler(MainTextBox_TextChanged);
                    SearchResultTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
                    SearchResultTextBox.BeginUpdate();

                    StringBuilder str = new StringBuilder();
                    foreach (Chapter chapter in m_client.FoundChapters)
                    {
                        foreach (Verse verse in chapter.Verses)
                        {
                            if (verse != null)
                            {
                                str.Append(verse.ArabicAddress + "\t" + verse.Text + "\n");
                            }
                        }
                    }
                    if (str.Length > 1)
                    {
                        str.Remove(str.Length - 1, 1);
                    }
                    m_current_text = str.ToString();

                    m_selection_mode = true;
                    UpdateHeaderLabel();
                    SearchResultTextBox.Text = m_current_text;

                    CalculateCurrentValue();

                    BuildLetterFrequencies();
                    DisplayLetterFrequencies();

                    //ColorizeChapters(); // TOO SLOW

                    m_current_found_verse_index = 0;
                    DisplayCurrentPositions();

                    List<Verse> verses = new List<Verse>();
                    foreach (Chapter chapter in m_client.FoundChapters)
                    {
                        verses.AddRange(chapter.Verses);
                    }
                    DisplayTranslations(verses);

                    if (add_to_history)
                    {
                        AddSearchHistoryItem();
                    }

                    m_current_found_verse_index = 0;
                    RealignFoundMatchedToStart();

                    BookmarkTextBox.Enabled = false;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            SearchResultTextBox.EndUpdate();
            SearchResultTextBox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
            //SearchResultTextBox.TextChanged += new EventHandler(MainTextBox_TextChanged);
            this.Cursor = Cursors.Default;
        }
    }
    private void DisplayFoundChapterRanges(bool add_to_history, bool colorize_chapters_by_matches)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_client != null)
            {
                if (m_client.FoundChapterRanges != null)
                {
                    TranslationTextBox.Text = null;
                    ZoomInLabel.Enabled = (m_text_zoom_factor <= (m_max_zoom_factor - m_zoom_factor_increment + m_error_margin));
                    ZoomOutLabel.Enabled = (m_text_zoom_factor >= (m_min_zoom_factor + m_zoom_factor_increment - m_error_margin));

                    if (colorize_chapters_by_matches)
                    {
                        m_matches_per_chapter = new int[m_client.Book.Chapters.Count];
                        foreach (List<Chapter> range in m_client.FoundChapterRanges)
                        {
                            foreach (Chapter chapter in range)
                            {
                                if (chapter != null)
                                {
                                    m_matches_per_chapter[chapter.Number - 1]++;
                                }
                            }
                        }

                        SwitchToSearchResultTextBox();
                    }

                    ChaptersListBox.SelectedIndexChanged -= new EventHandler(ChaptersListBox_SelectedIndexChanged);
                    if (m_client.FoundChapterRanges.Count > 0)
                    {
                        ChaptersListBox.SelectedIndices.Clear();
                        foreach (List<Chapter> range in m_client.FoundChapterRanges)
                        {
                            foreach (Chapter chapter in range)
                            {
                                if (((chapter.Number - 1) >= 0) && ((chapter.Number - 1) < ChaptersListBox.Items.Count))
                                {
                                    ChaptersListBox.SelectedIndices.Add(chapter.Number - 1);
                                }
                            }
                        }
                    }
                    else
                    {
                        ChaptersListBox.SelectedIndices.Clear();
                    }
                    ChaptersListBox.SelectedIndexChanged += new EventHandler(ChaptersListBox_SelectedIndexChanged);
                    UpdateSelection();

                    //SearchResultTextBox.TextChanged -= new EventHandler(MainTextBox_TextChanged);
                    SearchResultTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
                    SearchResultTextBox.BeginUpdate();

                    StringBuilder str = new StringBuilder();
                    foreach (List<Chapter> range in m_client.FoundChapterRanges)
                    {
                        foreach (Chapter chapter in range)
                        {
                            foreach (Verse verse in chapter.Verses)
                            {
                                if (verse != null)
                                {
                                    str.Append(verse.ArabicAddress + "\t" + verse.Text + "\n");
                                }
                            }
                        }
                    }
                    m_current_text = str.ToString();

                    m_selection_mode = true;
                    UpdateHeaderLabel();
                    SearchResultTextBox.Text = m_current_text;

                    CalculateCurrentValue();

                    BuildLetterFrequencies();
                    DisplayLetterFrequencies();

                    //ColorizeChapterRanges(); // TOO SLOW

                    m_current_found_verse_index = 0;
                    DisplayCurrentPositions();

                    List<Verse> verses = new List<Verse>();
                    foreach (List<Chapter> range in m_client.FoundChapterRanges)
                    {
                        foreach (Chapter chapter in range)
                        {
                            verses.AddRange(chapter.Verses);
                        }
                    }
                    DisplayTranslations(verses);

                    if (add_to_history)
                    {
                        AddSearchHistoryItem();
                    }

                    m_current_found_verse_index = 0;
                    RealignFoundMatchedToStart();

                    BookmarkTextBox.Enabled = false;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            SearchResultTextBox.EndUpdate();
            SearchResultTextBox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
            //SearchResultTextBox.TextChanged += new EventHandler(MainTextBox_TextChanged);
            this.Cursor = Cursors.Default;
        }
    }
    private void DisplayFoundChapterSetsTOOSLOW(bool add_to_history, bool colorize_chapters_by_matches)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_client != null)
            {
                if (m_client.FoundChapterSets != null)
                {
                    TranslationTextBox.Text = null;
                    ZoomInLabel.Enabled = (m_text_zoom_factor <= (m_max_zoom_factor - m_zoom_factor_increment + m_error_margin));
                    ZoomOutLabel.Enabled = (m_text_zoom_factor >= (m_min_zoom_factor + m_zoom_factor_increment - m_error_margin));

                    if (colorize_chapters_by_matches)
                    {
                        m_matches_per_chapter = new int[m_client.Book.Chapters.Count];
                        foreach (List<Chapter> set in m_client.FoundChapterSets)
                        {
                            foreach (Chapter chapter in set)
                            {
                                if (chapter != null)
                                {
                                    m_matches_per_chapter[chapter.Number - 1]++;
                                }
                            }
                        }

                        SwitchToSearchResultTextBox();
                    }

                    ChaptersListBox.SelectedIndexChanged -= new EventHandler(ChaptersListBox_SelectedIndexChanged);
                    if (m_client.FoundChapterSets.Count > 0)
                    {
                        ChaptersListBox.SelectedIndices.Clear();
                        foreach (List<Chapter> set in m_client.FoundChapterSets)
                        {
                            foreach (Chapter chapter in set)
                            {
                                if (((chapter.Number - 1) >= 0) && ((chapter.Number - 1) < ChaptersListBox.Items.Count))
                                {
                                    ChaptersListBox.SelectedIndices.Add(chapter.Number - 1);
                                }
                            }
                        }
                    }
                    else
                    {
                        ChaptersListBox.SelectedIndices.Clear();
                    }
                    ChaptersListBox.SelectedIndexChanged += new EventHandler(ChaptersListBox_SelectedIndexChanged);
                    UpdateSelection();

                    //SearchResultTextBox.TextChanged -= new EventHandler(MainTextBox_TextChanged);
                    SearchResultTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
                    SearchResultTextBox.BeginUpdate();

                    StringBuilder str = new StringBuilder();
                    foreach (List<Chapter> set in m_client.FoundChapterSets)
                    {
                        foreach (Chapter chapter in set)
                        {
                            foreach (Verse verse in chapter.Verses)
                            {
                                if (verse != null)
                                {
                                    str.Append(verse.ArabicAddress + "\t" + verse.Text + "\n");
                                }
                            }
                        }
                    }
                    m_current_text = str.ToString();

                    m_selection_mode = true;
                    UpdateHeaderLabel();
                    SearchResultTextBox.Text = m_current_text;

                    CalculateCurrentValue();

                    BuildLetterFrequencies();
                    DisplayLetterFrequencies();

                    //ColorizeChapterSets(); // TOO SLOW

                    m_current_found_verse_index = 0;
                    DisplayCurrentPositions();

                    List<Verse> verses = new List<Verse>();
                    foreach (List<Chapter> set in m_client.FoundChapterSets)
                    {
                        foreach (Chapter chapter in set)
                        {
                            verses.AddRange(chapter.Verses);
                        }
                    }
                    DisplayTranslations(verses);

                    if (add_to_history)
                    {
                        AddSearchHistoryItem();
                    }

                    m_current_found_verse_index = 0;
                    RealignFoundMatchedToStart();

                    BookmarkTextBox.Enabled = false;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            SearchResultTextBox.EndUpdate();
            SearchResultTextBox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
            //SearchResultTextBox.TextChanged += new EventHandler(MainTextBox_TextChanged);
            this.Cursor = Cursors.Default;
        }
    }
    private void DisplayFoundChapterSets(bool add_to_history, bool colorize_chapters_by_matches)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_client != null)
            {
                if (m_client.FoundChapterSets != null)
                {
                    if (m_client.FoundChapterSets.Count > 0)
                    {
                        StringBuilder str = new StringBuilder();
                        foreach (List<Chapter> set in m_client.FoundChapterSets)
                        {
                            foreach (Chapter chapter in set)
                            {
                                if (chapter != null)
                                {
                                    str.Append(chapter.Number + "." + chapter.Verses.Count + ", ");
                                }
                            }
                            if (set.Count > 0)
                            {
                                str.Remove(str.Length - 2, 2);
                            }
                            str.AppendLine();
                        }

                        string filename = "FoundChapterSets" + Globals.OUTPUT_FILE_EXT;
                        string path = Globals.STATISTICS_FOLDER + "/" + filename;
                        PublicStorage.SaveText(path, str.ToString());
                        PublicStorage.DisplayFile(path);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }

    private void BuildFindMatches()
    {
        if (m_client != null)
        {
            if (m_client.FoundPhrases != null)
            {
                foreach (Phrase phrase in m_client.FoundPhrases)
                {
                    if (phrase != null)
                    {
                        if (phrase.Verse != null)
                        {
                            int start = GetPhrasePositionInRichTextBox(phrase);
                            if ((start >= 0) && (start < SearchResultTextBox.Text.Length))
                            {
                                if (phrase.Text != null)
                                {
                                    int length = phrase.Text.Length;
                                    BuildFindMatch(start, length);
                                }
                            }
                        }
                    }
                }
            }
        }
        UpdateFindMatchCaption();
    }
    private void ColorizePhrases()
    {
        if (m_client != null)
        {
            if (m_client.FoundPhrases != null)
            {
                foreach (Phrase phrase in m_client.FoundPhrases)
                {
                    if (phrase != null)
                    {
                        if (phrase.Verse != null)
                        {
                            int start = GetPhrasePositionInRichTextBox(phrase);
                            if ((start >= 0) && (start < SearchResultTextBox.Text.Length))
                            {
                                if (phrase.Text != null)
                                {
                                    int length = phrase.Text.Length;
                                    SearchResultTextBox.Select(start, length);
                                    SearchResultTextBox.SelectionColor = Color.Red;
                                }
                            }
                        }
                    }
                }

                UpdateFindMatchCaption();
            }
        }
    }
    private void ColorizeVerseRanges()
    {
        if (m_client != null)
        {
            if (m_client.FoundVerseRanges != null)
            {
                if (m_client.FoundVerseRanges.Count > 0)
                {
                    bool colorize = true; // colorize ranges alternatively

                    int line_index = 0;
                    foreach (List<Verse> range in m_client.FoundVerseRanges)
                    {
                        colorize = !colorize; // alternate colorization of ranges

                        int start = SearchResultTextBox.GetLinePosition(line_index);
                        int length = 0;
                        foreach (Verse verse in range)
                        {
                            length += SearchResultTextBox.Lines[line_index].Length + 1; // "\n"
                            line_index++;
                        }
                        SearchResultTextBox.Select(start, length);
                        SearchResultTextBox.SelectionColor = colorize ? Color.Blue : Color.Navy;
                    }
                }
            }
        }

        //FIX to reset SelectionColor
        SearchResultTextBox.Select(0, 1);
        SearchResultTextBox.SelectionColor = Color.Navy;
        SearchResultTextBox.Select(0, 0);
        SearchResultTextBox.SelectionColor = Color.Navy;
    }
    private void ColorizeVerseSets()
    {
        if (m_client != null)
        {
            if (m_client.FoundVerseSets != null)
            {
                if (m_client.FoundVerseSets.Count > 0)
                {
                    bool colorize = true; // colorize sets alternatively

                    int line_index = 0;
                    foreach (List<Verse> set in m_client.FoundVerseSets)
                    {
                        colorize = !colorize; // alternate colorization of sets

                        int start = SearchResultTextBox.GetLinePosition(line_index);
                        int length = 0;
                        foreach (Verse verse in set)
                        {
                            length += SearchResultTextBox.Lines[line_index].Length + 1; // "\n"
                            line_index++;
                        }
                        SearchResultTextBox.Select(start, length);
                        SearchResultTextBox.SelectionColor = colorize ? Color.Blue : Color.Navy;
                    }
                }
            }
        }

        //FIX to reset SelectionColor
        SearchResultTextBox.Select(0, 1);
        SearchResultTextBox.SelectionColor = Color.Navy;
        SearchResultTextBox.Select(0, 0);
        SearchResultTextBox.SelectionColor = Color.Navy;
    }
    private void ColorizeChapters()
    {
        if (m_client != null)
        {
            if (m_client.FoundChapters != null)
            {
                if (m_client.FoundChapters.Count > 0)
                {
                    bool colorize = true; // colorize chapters alternatively

                    int line_index = 0;
                    foreach (Chapter chapter in m_client.FoundChapters)
                    {
                        if (chapter != null)
                        {
                            colorize = !colorize; // alternate colorization of chapters

                            int start = SearchResultTextBox.GetLinePosition(line_index);
                            int length = 0;
                            foreach (Verse verse in chapter.Verses)
                            {
                                length += SearchResultTextBox.Lines[line_index].Length + 1; // "\n"
                                line_index++;
                            }
                            SearchResultTextBox.Select(start, length);
                            SearchResultTextBox.SelectionColor = colorize ? Color.Blue : Color.Navy;
                        }
                    }
                }
            }
        }

        //FIX to reset SelectionColor
        SearchResultTextBox.Select(0, 1);
        SearchResultTextBox.SelectionColor = Color.Navy;
        SearchResultTextBox.Select(0, 0);
        SearchResultTextBox.SelectionColor = Color.Navy;
    }
    private void ColorizeChapterRanges()
    {
        if (m_client != null)
        {
            if (m_client.FoundChapterRanges != null)
            {
                if (m_client.FoundChapterRanges.Count > 0)
                {
                    bool colorize = true; // colorize ranges alternatively

                    int line_index = 0;
                    foreach (List<Chapter> range in m_client.FoundChapterRanges)
                    {
                        if (range != null)
                        {
                            colorize = !colorize; // alternate colorization of ranges

                            int start = SearchResultTextBox.GetLinePosition(line_index);
                            int length = 0;
                            foreach (Chapter chapter in range)
                            {
                                foreach (Verse verse in chapter.Verses)
                                {
                                    length += SearchResultTextBox.Lines[line_index].Length + 1; // "\n"
                                    line_index++;
                                }
                            }
                            SearchResultTextBox.Select(start, length);
                            SearchResultTextBox.SelectionColor = colorize ? Color.Blue : Color.Navy;
                        }
                    }
                }
            }
        }

        //FIX to reset SelectionColor
        SearchResultTextBox.Select(0, 1);
        SearchResultTextBox.SelectionColor = Color.Navy;
        SearchResultTextBox.Select(0, 0);
        SearchResultTextBox.SelectionColor = Color.Navy;
    }
    private void ColorizeChapterSets()
    {
        if (m_client != null)
        {
            if (m_client.FoundChapterSets != null)
            {
                if (m_client.FoundChapterSets.Count > 0)
                {
                    bool colorize = true; // colorize sets alternatively

                    int line_index = 0;
                    foreach (List<Chapter> set in m_client.FoundChapterSets)
                    {
                        if (set != null)
                        {
                            colorize = !colorize; // alternate colorization of sets

                            int start = SearchResultTextBox.GetLinePosition(line_index);
                            int length = 0;
                            foreach (Chapter chapter in set)
                            {
                                foreach (Verse verse in chapter.Verses)
                                {
                                    length += SearchResultTextBox.Lines[line_index].Length + 1; // "\n"
                                    line_index++;
                                }
                            }
                            SearchResultTextBox.Select(start, length);
                            SearchResultTextBox.SelectionColor = colorize ? Color.Blue : Color.Navy;
                        }
                    }
                }
            }
        }

        //FIX to reset SelectionColor
        SearchResultTextBox.Select(0, 1);
        SearchResultTextBox.SelectionColor = Color.Navy;
        SearchResultTextBox.Select(0, 0);
        SearchResultTextBox.SelectionColor = Color.Navy;
    }

    private int GetPhrasePositionInRichTextBox(Phrase phrase)
    {
        if (phrase == null) return 0;

        if (m_client != null)
        {
            if (m_client.FoundVerses != null)
            {
                int char_index = 0;
                foreach (Verse verse in m_client.FoundVerses)
                {
                    if (verse != null)
                    {
                        if (phrase.Verse.Number == verse.Number)
                        {
                            return (char_index + verse.Address.Length + 1 + phrase.Position);
                        }
                        char_index += GetVerseDisplayLength(verse);
                    }
                }
            }
        }
        return -1;
    }
    private void RealignFoundMatchedToStart()
    {
        if (m_client != null)
        {
            SearchResultTextBox.ClearHighlight();

            if (m_found_verses_displayed)
            {
                List<Verse> displayed_verses = new List<Verse>();
                if (m_client.FoundVerses != null)
                {
                    displayed_verses.AddRange(m_client.FoundVerses);
                }
                else if (m_client.FoundChapters != null)
                {
                    foreach (Chapter chapter in m_client.FoundChapters)
                    {
                        displayed_verses.AddRange(chapter.Verses);
                    }
                }
                else if (m_client.FoundVerseRanges != null)
                {
                    foreach (List<Verse> range in m_client.FoundVerseRanges)
                    {
                        displayed_verses.AddRange(range);
                    }
                }
                else if (m_client.FoundChapterRanges != null)
                {
                    foreach (List<Chapter> range in m_client.FoundChapterRanges)
                    {
                        foreach (Chapter chapter in range)
                        {
                            displayed_verses.AddRange(chapter.Verses);
                        }
                    }
                }

                int start = 0;
                // scroll to beginning to show complete verse address because in Arabic, pos=0 is after the first number :(
                if (m_client.FoundVerses != null)
                {
                    if (m_client.FoundVerses.Count > 0)
                    {
                        Verse verse = m_client.FoundVerses[0];
                        if (verse != null)
                        {
                            if (verse.Chapter != null)
                            {
                                if (verse.Chapter != null)
                                {
                                    start = verse.Chapter.Number.ToString().Length;
                                }
                            }
                        }
                    }

                    // re-align to text start
                    if ((start >= 0) && (start < SearchResultTextBox.Text.Length))
                    {
                        SearchResultTextBox.ScrollToCaret();    // must be called first
                        SearchResultTextBox.Select(start, 0);   // must be called second
                    }
                }

                // prepare for Backspace
                SelectionHistoryBackwardButton.Focus();
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 24. User History
    ///////////////////////////////////////////////////////////////////////////////
    private void AddSearchHistoryItem()
    {
        if (m_client != null)
        {
            if (m_client.FoundVerses != null)
            {
                if (m_client.FoundVerses.Count > 0)
                {
                    SearchHistoryItem item = new SearchHistoryItem();
                    item.SearchType = m_search_type;
                    item.NumbersResultType = m_numbers_result_type;
                    item.Text = (m_search_type == SearchType.Numbers) ? null : FindByTextTextBox.Text;
                    item.LanguageType = m_language_type;

                    if (TranslatorComboBox.SelectedItem != null)
                    {
                        item.Translation = TranslatorComboBox.SelectedItem.ToString();
                    }

                    item.Verses = new List<Verse>(m_client.FoundVerses);
                    if (m_client.FoundPhrases == null)
                    {
                        item.Phrases = null;
                    }
                    else
                    {
                        item.Phrases = new List<Phrase>(m_client.FoundPhrases);
                    }
                    item.Header = m_find_result_header;
                    m_client.AddHistoryItem(item);
                    UpdateSelectionHistoryButtons();
                }
            }
        }
    }
    private void AddSelectionHistoryItem()
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                if (m_client.Selection != null)
                {
                    SelectionHistoryItem item = new SelectionHistoryItem(m_client.Selection.Book, m_client.Selection.Scope, m_client.Selection.Indexes);
                    if (item != null)
                    {
                        m_client.AddHistoryItem(item);
                        UpdateSelectionHistoryButtons();
                    }
                }
            }
        }
    }
    private void SelectionHistoryBackwardButton_Click(object sender, EventArgs e)
    {
        if (m_client != null)
        {
            object item = m_client.GotoPreviousHistoryItem();
            if (item != null)
            {
                if (item is SearchHistoryItem)
                {
                    if (m_find_matches != null)
                    {
                        m_find_matches.Clear(); // to reset Matched count
                    }
                }
                else if (item is SelectionHistoryItem)
                {
                    UpdateChaptersListBox();
                }

                DisplaySelectionHistoryItem(item);
                UpdateSelectionHistoryButtons();
            }
        }
        this.AcceptButton = null;
    }
    private void SelectionHistoryForwardButton_Click(object sender, EventArgs e)
    {
        if (m_client != null)
        {
            object item = m_client.GotoNextHistoryItem();
            if (item != null)
            {
                if (item is SearchHistoryItem)
                {
                    if (m_find_matches != null)
                    {
                        m_find_matches.Clear(); // to reset Matched count
                    }
                }
                else if (item is SelectionHistoryItem)
                {
                    UpdateChaptersListBox();
                }

                DisplaySelectionHistoryItem(item);
                UpdateSelectionHistoryButtons();
            }
        }
        this.AcceptButton = null;
    }
    private void SelectionHistoryDeleteLabel_Click(object sender, EventArgs e)
    {
        if (m_client != null)
        {
            PlayerStopLabel_Click(null, null);

            m_client.DeleteCurrentHistoryItem();
            if (m_client.HistoryItems.Count == 0) // no item to display
            {
                DisplaySelection(false);
            }
            else // there is an item to display
            {
                object item = m_client.CurrentHistoryItem;
                if (item != null)
                {
                    UpdateChaptersListBox();

                    DisplaySelectionHistoryItem(item);
                }
            }

            UpdateSelectionHistoryButtons();
        }
    }
    private void SelectionHistoryClearLabel_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(
            "Delete all search history?",
            Application.ProductName,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question) == DialogResult.Yes)
        {
            if (m_client != null)
            {
                PlayerStopLabel_Click(null, null);

                m_client.ClearHistoryItems();
                DisplaySelection(false);

                FindByTextTextBox.Text = null;
                AutoCompleteHeaderLabel.Visible = false;
                AutoCompleteListBox.Visible = false;

                UpdateChaptersListBox();

                UpdateSelectionHistoryButtons();
            }
        }
    }
    private void DisplaySelectionHistoryItem(object item)
    {
        if (m_client != null)
        {
            PlayerStopLabel_Click(null, null);

            if (item != null)
            {
                SearchHistoryItem search_history_item = item as SearchHistoryItem;
                if (search_history_item != null)
                {
                    FindByTextTextBox.Text = search_history_item.Text;
                    m_find_result_header = search_history_item.Header;
                    m_language_type = search_history_item.LanguageType;

                    TranslatorComboBox.SelectedItem = search_history_item.Translation;

                    m_client.FilterChapters = null;
                    if (search_history_item.Phrases != null)
                    {
                        m_client.FoundPhrases = new List<Phrase>(search_history_item.Phrases);
                    }
                    else
                    {
                        m_client.FoundPhrases = null;
                    }
                    m_client.FoundVerses = new List<Verse>(search_history_item.Verses);

                    if (!String.IsNullOrEmpty(search_history_item.Text))
                    {
                        FindByTextTextBox.SelectionStart = search_history_item.Text.Length;
                        DisplayFoundVerses(false, true);
                    }
                    else
                    {
                        // no NumberQuery is saved so we cannot colorize ranges
                        // so just display all verses and let user re-run search if they need colorization
                        //switch (search_history_item.NumbersResultType)
                        //{
                        //    case NumbersResultType.Words:
                        //    case NumbersResultType.WordRanges:
                        //    case NumbersResultType.Verses:
                        //        DisplayFoundVerses(false);
                        //        break;
                        //    case NumbersResultType.VerseRanges:
                        //        DisplayFoundVerseRanges(false);
                        //        break;
                        //    case NumbersResultType.Chapters:
                        //        DisplayFoundChapters(false);
                        //        break;
                        //    case NumbersResultType.ChapterRanges:
                        //        DisplayFoundChapterRanges(false);
                        //        break;
                        //}

                        // for now use:
                        DisplayFoundVerses(false, true);
                    }
                }
                else
                {
                    SelectionHistoryItem selection_history_item = item as SelectionHistoryItem;
                    if (selection_history_item != null)
                    {
                        m_client.Selection = new Selection(selection_history_item.Book, selection_history_item.Scope, selection_history_item.Indexes);
                        DisplaySelection(false);
                    }
                }
            }
        }
    }
    private void UpdateSelectionHistoryButtons()
    {
        if (m_client != null)
        {
            if (m_client.HistoryItems != null)
            {
                SelectionHistoryBackwardButton.Enabled = (m_client.HistoryItems.Count > 0) && (m_client.HistoryItemIndex > 0);
                SelectionHistoryForwardButton.Enabled = (m_client.HistoryItems.Count >= 0) && (m_client.HistoryItemIndex < m_client.HistoryItems.Count - 1);
                SelectionHistoryDeleteLabel.Enabled = (m_client.HistoryItems.Count > 0);
                SelectionHistoryClearLabel.Enabled = (m_client.HistoryItems.Count > 0);
                SelectionHistoryClearLabel.BackColor = (m_client.HistoryItems.Count > 0) ? Color.LightCoral : SystemColors.ControlLight;
                SelectionHistoryCounterLabel.Text = (m_client.HistoryItemIndex + 1).ToString() + " / " + m_client.HistoryItems.Count.ToString();

                if (m_client.HistoryItems.Count == 0)
                {
                    SearchResultTextBox.Text = "";
                    m_find_result_header = "";
                    UpdateHeaderLabel();
                }
            }
        }
    }
    private void SelectionHistoryCounterLabel_Click(object sender, EventArgs e)
    {
        if (m_client != null)
        {
            if (m_client.HistoryItems != null)
            {
                if (m_client.HistoryItems.Count > 0)
                {
                    DisplaySelectionHistoryItem(m_client.CurrentHistoryItem);
                }
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 25. Value Systems
    ///////////////////////////////////////////////////////////////////////////////
    private void LoadNumerologySystem(string numerology_system_name)
    {
        if (m_client != null)
        {
            PlayerStopLabel_Click(null, null);

            m_client.LoadNumerologySystem(numerology_system_name);
            CalculateCurrentValue();

            BuildLetterFrequencies();
            DisplayLetterFrequencies();
        }
    }
    private void UpdateNumerologySystemControls()
    {
        if (m_client != null)
        {
            if (m_client.NumerologySystem != null)
            {
                try
                {
                    TextModeComboBox.SelectedIndexChanged -= new EventHandler(TextModeComboBox_SelectedIndexChanged);
                    NumerologySystemComboBox.SelectedIndexChanged -= new EventHandler(NumerologySystemComboBox_SelectedIndexChanged);

                    TextModeComboBox.SelectedItem = m_client.NumerologySystem.TextMode;
                    PopulateNumerologySystemComboBox();
                    NumerologySystemComboBox.SelectedItem = m_client.NumerologySystem.LetterOrder + "_" + m_client.NumerologySystem.LetterValue;

                    UpdateKeyboard(m_client.NumerologySystem.TextMode);
                }
                finally
                {
                    TextModeComboBox.SelectedIndexChanged += new EventHandler(TextModeComboBox_SelectedIndexChanged);
                    NumerologySystemComboBox.SelectedIndexChanged += new EventHandler(NumerologySystemComboBox_SelectedIndexChanged);
                }
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 26. Value Calculations
    ///////////////////////////////////////////////////////////////////////////////
    private void PopulateTextModeComboBox()
    {
        try
        {
            TextModeComboBox.SelectedIndexChanged -= new EventHandler(TextModeComboBox_SelectedIndexChanged);
            if (m_client != null)
            {
                if (m_client.LoadedNumerologySystems != null)
                {
                    TextModeComboBox.BeginUpdate();

                    TextModeComboBox.Items.Clear();
                    foreach (NumerologySystem numerology_system in m_client.LoadedNumerologySystems.Values)
                    {
                        string[] parts = numerology_system.Name.Split('_');
                        if (parts != null)
                        {
                            if (parts.Length == 3)
                            {
                                string text_mode = parts[0];
                                if (!TextModeComboBox.Items.Contains(text_mode))
                                {
                                    TextModeComboBox.Items.Add(text_mode);
                                }
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            TextModeComboBox.EndUpdate();
            TextModeComboBox.SelectedIndexChanged += new EventHandler(TextModeComboBox_SelectedIndexChanged);
        }
    }
    private void PopulateNumerologySystemComboBox()
    {
        try
        {
            NumerologySystemComboBox.SelectedIndexChanged -= new EventHandler(NumerologySystemComboBox_SelectedIndexChanged);
            if (m_client != null)
            {
                if (m_client.LoadedNumerologySystems != null)
                {
                    NumerologySystemComboBox.BeginUpdate();

                    if (TextModeComboBox.SelectedItem != null)
                    {
                        string text_mode = TextModeComboBox.SelectedItem.ToString();

                        NumerologySystemComboBox.Items.Clear();
                        foreach (NumerologySystem numerology_system in m_client.LoadedNumerologySystems.Values)
                        {
                            string[] parts = numerology_system.Name.Split('_');
                            if (parts != null)
                            {
                                if (parts.Length == 3)
                                {
                                    if (parts[0] == text_mode)
                                    {
                                        string valuation_system = parts[1] + "_" + parts[2];
                                        if (!NumerologySystemComboBox.Items.Contains(valuation_system))
                                        {
                                            NumerologySystemComboBox.Items.Add(valuation_system);
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
            NumerologySystemComboBox.EndUpdate();
            NumerologySystemComboBox.SelectedIndexChanged += new EventHandler(NumerologySystemComboBox_SelectedIndexChanged);
        }
    }
    private void TextModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_client != null)
            {
                if (m_client.NumerologySystem != null)
                {
                    string backup_valuation_system = null;
                    if (NumerologySystemComboBox.SelectedItem != null)
                    {
                        backup_valuation_system = NumerologySystemComboBox.SelectedItem.ToString();
                    }

                    PopulateNumerologySystemComboBox();
                    if (NumerologySystemComboBox.Items.Count > 0)
                    {
                        if (backup_valuation_system != null)
                        {
                            if (NumerologySystemComboBox.Items.Contains(backup_valuation_system))
                            {
                                NumerologySystemComboBox.SelectedItem = backup_valuation_system;
                            }
                            else
                            {
                                NumerologySystemComboBox.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            NumerologySystemComboBox.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        NumerologySystemComboBox.SelectedIndex = -1;
                    }

                    NumerologySystemComboBox.SelectedItem = m_client.NumerologySystem.LetterOrder + "_" + m_client.NumerologySystem.LetterValue;

                    UpdateKeyboard(m_client.NumerologySystem.TextMode);

                    UpdateTextModeOptions();
                    BuildSimplifiedBookDisplaySelection();
                }
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void UpdateTextModeOptions()
    {
        if (TextModeComboBox.SelectedItem != null)
        {
            try
            {
                WithBismAllahCheckBox.CheckedChanged -= new EventHandler(WithBismAllahCheckBox_CheckedChanged);
                WawAsWordCheckBox.CheckedChanged -= new EventHandler(WawAsWordCheckBox_CheckedChanged);
                ShaddaAsLetterCheckBox.CheckedChanged -= new EventHandler(ShaddaAsLetterCheckBox_CheckedChanged);

                string text_mode = TextModeComboBox.SelectedItem.ToString();
                if (text_mode == "Original")
                {
                    m_with_bism_Allah = true;
                    m_waw_as_word = false;
                    m_shadda_as_letter = false;
                }
                WithBismAllahCheckBox.Checked = m_with_bism_Allah;
                WawAsWordCheckBox.Checked = m_waw_as_word;
                ShaddaAsLetterCheckBox.Checked = m_shadda_as_letter;

                WithBismAllahCheckBox.Enabled = (text_mode != "Original");
                WawAsWordCheckBox.Enabled = (text_mode != "Original");
                ShaddaAsLetterCheckBox.Enabled = (text_mode != "Original");

                WithBismAllahCheckBox.Refresh();
                WawAsWordCheckBox.Refresh();
                ShaddaAsLetterCheckBox.Refresh();
            }
            finally
            {
                WithBismAllahCheckBox.CheckedChanged += new EventHandler(WithBismAllahCheckBox_CheckedChanged);
                WawAsWordCheckBox.CheckedChanged += new EventHandler(WawAsWordCheckBox_CheckedChanged);
                ShaddaAsLetterCheckBox.CheckedChanged += new EventHandler(ShaddaAsLetterCheckBox_CheckedChanged);
            }
        }
    }
    private void TextModeComboBox_DropDown(object sender, EventArgs e)
    {
        TextModeComboBox.DropDownHeight = StatisticsGroupBox.Height - TextModeComboBox.Top - TextModeComboBox.Height - 1;
    }
    private bool m_with_bism_Allah = true;
    private void WithBismAllahCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_with_bism_Allah = WithBismAllahCheckBox.Checked;
        BuildSimplifiedBookDisplaySelection();
    }
    private bool m_waw_as_word = false;
    private void WawAsWordCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_waw_as_word = WawAsWordCheckBox.Checked;
        BuildSimplifiedBookDisplaySelection();
    }
    private bool m_shadda_as_letter = false;
    private void ShaddaAsLetterCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_shadda_as_letter = ShaddaAsLetterCheckBox.Checked;
        BuildSimplifiedBookDisplaySelection();
    }
    private void BuildSimplifiedBookDisplaySelection()
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (TextModeComboBox.SelectedItem != null)
            {
                string text_mode = TextModeComboBox.SelectedItem.ToString();

                if (m_client != null)
                {
                    if (m_client.Book != null)
                    {
                        // ALWAYS rebuild book to allow user to edit SimplificationRules file in Notepad and update text on the fly
                        //if ((m_client.Book.TextMode != text_mode) ||
                        //    (m_client.Book.WithBismAllah != m_with_bism_Allah) ||
                        //    (m_client.Book.WawAsWord != m_waw_as_word) ||
                        //    (m_client.Book.ShaddaAsLetter != m_shadda_as_letter)
                        //   )
                        {
                            m_client.BuildSimplifiedBook(text_mode, m_with_bism_Allah, m_waw_as_word, m_shadda_as_letter);

                            bool backup_found_verses_displayed = m_found_verses_displayed;

                            // re-display text in new text_mode
                            DisplaySelection(false);

                            // re-display search result if that was shown when text_mode was changed
                            if (backup_found_verses_displayed)
                            {
                                DisplayFoundVerses(false, false);
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
    private void NumerologySystemComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            NumerologySystemComboBox.SelectedIndexChanged -= new EventHandler(NumerologySystemComboBox_SelectedIndexChanged);
            NumerologySystemComboBox.SelectedIndexChanged -= new EventHandler(NumerologySystemComboBox_SelectedIndexChanged);

            if (m_client != null)
            {
                if (m_client.NumerologySystem != null)
                {
                    if (TextModeComboBox.SelectedItem != null)
                    {
                        if (NumerologySystemComboBox.SelectedItem != null)
                        {
                            string text_mode = TextModeComboBox.SelectedItem.ToString();
                            string valuation_system = NumerologySystemComboBox.SelectedItem.ToString();
                            string numerology_system_name = text_mode + "_" + valuation_system;
                            LoadNumerologySystem(numerology_system_name);
                        }
                    }
                }
            }
        }
        finally
        {
            NumerologySystemComboBox.SelectedIndexChanged += new EventHandler(NumerologySystemComboBox_SelectedIndexChanged);
            this.Cursor = Cursors.Default;
        }
    }
    private void NumerologySystemComboBox_DropDown(object sender, EventArgs e)
    {
        NumerologySystemComboBox.DropDownHeight = StatisticsGroupBox.Height - NumerologySystemComboBox.Top - NumerologySystemComboBox.Height - 1;
    }

    private void CalculateCurrentValue()
    {
        if (m_active_textbox != null)
        {
            if (m_client != null)
            {
                CalculateCurrentText();
                if (!String.IsNullOrEmpty(m_current_text))
                {
                    if (m_selection_mode)
                    {
                        if (m_found_verses_displayed)
                        {
                            if (m_client.FoundVerses != null)
                            {
                                CalculateAndDisplayCounts(m_client.FoundVerses);
                                CalculateValueAndDisplayFactors(m_client.FoundVerses);
                            }
                        }
                        else
                        {
                            if (m_client.Selection != null)
                            {
                                if (m_client.Selection.Verses != null)
                                {
                                    CalculateAndDisplayCounts(m_client.Selection.Verses);
                                    CalculateValueAndDisplayFactors(m_client.Selection.Verses);
                                }
                            }
                        }
                    }
                    else // cursor inside line OR some text is highlighted
                    {
                        if (m_active_textbox.SelectedText.Length == 0) // cursor inside line
                        {
                            if (m_readonly_translation)
                            {
                                Verse verse = GetVerse(CurrentVerseIndex);
                                if (verse != null)
                                {
                                    CalculateAndDisplayCounts(verse);
                                    CalculateValueAndDisplayFactors(verse);
                                }
                            }
                            else // edit mode so user can paste any text they like to calculate its value
                            {
                                string user_text = m_current_text;
                                CalculateValueAndDisplayFactors(user_text);
                            }
                        }
                        else // some text is selected
                        {
                            if (m_readonly_translation)
                            {
                                CalculateSelectedTextValue();
                            }
                            else // edit mode so user can paste any text they like to calculate its value
                            {
                                string user_text = m_current_text;
                                CalculateValueAndDisplayFactors(user_text);
                            }
                        }
                    }
                }
            }
        }
    }
    private void CalculateCurrentText()
    {
        if (m_active_textbox != null)
        {
            if (m_selection_mode)
            {
                m_current_text = m_active_textbox.Text;
            }
            else
            {
                if (m_active_textbox.SelectedText.Length == 0) // get text at current line
                {
                    Verse verse = GetVerse(CurrentVerseIndex);
                    if (verse != null)
                    {
                        m_current_text = verse.Text;
                    }
                    else
                    {
                        m_current_text = "";
                    }
                }
                else // get current selected text
                {
                    m_current_text = m_active_textbox.SelectedText;
                }
            }

            if (!String.IsNullOrEmpty(m_current_text))
            {
                m_current_text = RemoveVerseAddresses(m_current_text);
                m_current_text = RemoveVerseEndMarks(m_current_text);
                m_current_text = m_current_text.Trim();
                m_current_text = m_current_text.Replace("\n", "\r\n");
            }
        }
    }
    private void CalculateSelectedTextValue()
    {
        if (m_active_textbox != null)
        {
            if (m_client != null)
            {
                string selected_text = m_active_textbox.SelectedText;
                int first_char = m_active_textbox.SelectionStart;
                int last_char = first_char + m_active_textbox.SelectionLength - 1;

                // skip any \n at beginning of selected text
                // skip any Endmark at beginning of selected text
                while (
                        (selected_text.Length > 0) &&
                        (
                          (selected_text[0] == '\n') ||
                          (selected_text[0] == '\r') ||
                          (selected_text[0] == '\t') ||
                          (selected_text[0] == '_') ||
                          (selected_text[0] == ' ') ||
                          (selected_text[0] == Verse.OPEN_BRACKET[0]) ||
                          (selected_text[0] == Verse.CLOSE_BRACKET[0]) ||
                          Constants.INDIAN_DIGITS.Contains(selected_text[0]) ||
                          Constants.STOPMARKS.Contains(selected_text[0]) ||
                          Constants.QURANMARKS.Contains(selected_text[0])
                        )
                      )
                {
                    selected_text = selected_text.Remove(0, 1);
                    first_char++;
                }

                // skip any \n at end of selected text
                // skip any Endmark at end of selected text
                while (
                        (selected_text.Length > 0) &&
                        (
                          (selected_text[selected_text.Length - 1] == '\n') ||
                          (selected_text[selected_text.Length - 1] == '\r') ||
                          (selected_text[selected_text.Length - 1] == '\t') ||
                          (selected_text[selected_text.Length - 1] == '_') ||
                          (selected_text[selected_text.Length - 1] == ' ') ||
                          (selected_text[selected_text.Length - 1] == Verse.OPEN_BRACKET[0]) ||
                          (selected_text[selected_text.Length - 1] == Verse.CLOSE_BRACKET[0]) ||
                          (selected_text[selected_text.Length - 1] == ' ') ||
                          Constants.INDIAN_DIGITS.Contains(selected_text[selected_text.Length - 1]) ||
                          Constants.STOPMARKS.Contains(selected_text[selected_text.Length - 1]) ||
                          Constants.QURANMARKS.Contains(selected_text[selected_text.Length - 1])
                        )
                      )
                {
                    selected_text = selected_text.Remove(selected_text.Length - 1);
                    last_char--;
                }

                List<Verse> highlighted_verses = new List<Verse>();
                Verse first_verse = GetVerseAtChar(first_char);
                if (first_verse != null)
                {
                    Verse last_verse = GetVerseAtChar(last_char);
                    if (last_verse != null)
                    {
                        List<Verse> verses = null;
                        if (m_found_verses_displayed)
                        {
                            verses = m_client.FoundVerses;
                        }
                        else
                        {
                            if (m_client.Selection != null)
                            {
                                verses = m_client.Selection.Verses;
                            }
                        }

                        if (verses != null)
                        {
                            int first_verse_index = GetVerseIndex(first_verse);
                            int last_verse_index = GetVerseIndex(last_verse);
                            for (int i = first_verse_index; i <= last_verse_index; i++)
                            {
                                highlighted_verses.Add(verses[i]);
                            }
                            //??? TODO in Research/Ultimate Edition add harakaat too
                            Letter letter1 = GetLetterAtChar(first_char);
                            if (letter1 != null)
                            {
                                int first_verse_letter_index = letter1.NumberInVerse - 1;

                                Letter letter2 = GetLetterAtChar(last_char);
                                if (letter2 != null)
                                {
                                    int last_verse_letter_index = letter2.NumberInVerse - 1;

                                    // calculate and display verse_number_sum, word_number_sum, letter_number_sum
                                    CalculateAndDisplayCounts(highlighted_verses, first_verse_letter_index, last_verse_letter_index);

                                    // calculate Letters value
                                    CalculateValueAndDisplayFactors(highlighted_verses, first_verse_letter_index, last_verse_letter_index);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private int m_user_text_selection_start = 0;
    private int m_user_text_selection_length = 0;
    private void CalculateUserTextValue(Point location)
    {
        bool selection_changed = false;
        if (m_user_text_selection_length != UserTextTextBox.SelectedText.Length)
        {
            m_user_text_selection_length = UserTextTextBox.SelectedText.Length;
            selection_changed = true;
        }
        else if (m_user_text_selection_start != UserTextTextBox.SelectionStart)
        {
            m_user_text_selection_start = UserTextTextBox.SelectionStart;
            selection_changed = true;
        }

        if (selection_changed)
        {
            ////////////////////////////////////////////////////
            // overwrite m_current_text to show LetterStatistics
            ////////////////////////////////////////////////////
            if (UserTextTextBox.SelectionLength > 0)
            {
                // selected text only
                m_current_text = UserTextTextBox.SelectedText;
            }
            else
            {
                if ((location.X == 0) && (location.Y == 0))
                {
                    // all text
                    m_current_text = UserTextTextBox.Text;
                }
                else
                {
                    // current line text
                    int char_index = UserTextTextBox.GetCharIndexFromPosition(location);
                    int line_index = UserTextTextBox.GetLineFromCharIndex(char_index);
                    if ((line_index >= 0) && (line_index < UserTextTextBox.Lines.Length))
                    {
                        m_current_text = UserTextTextBox.Lines[line_index].ToString();
                    }
                    else
                    {
                        m_current_text = "";
                    }
                }
            }
            m_current_text = m_current_text.Trim();

            // calculate Letters value
            CalculateValueAndDisplayFactors(m_current_text);

            // calculate and display verse_number_sum, word_number_sum, letter_number_sum
            CalculateAndDisplayCounts(m_current_text);

            BuildLetterFrequencies();
            DisplayLetterFrequencies();
        }
    }
    private void UserTextTextBox_KeyUp(object sender, KeyEventArgs e)
    {
        int char_index = UserTextTextBox.GetFirstCharIndexOfCurrentLine();
        if (char_index >= 0)
        {
            Point caret_position = UserTextTextBox.GetPositionFromCharIndex(char_index);
            CalculateUserTextValue(caret_position);
        }
    }
    bool m_mouse_down = false;
    private void UserTextTextBox_MouseDown(object sender, MouseEventArgs e)
    {
        m_mouse_down = true;
    }
    private void UserTextTextBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (m_mouse_down)
        {
            CalculateUserTextValue(e.Location);
        }
    }
    private void UserTextTextBox_MouseUp(object sender, MouseEventArgs e)
    {
        m_mouse_down = false;
        CalculateUserTextValue(e.Location);
    }
    private void UserTextTextBox_MouseEnter(object sender, EventArgs e)
    {
        CalculateUserTextValue(new Point(0, 0));
    }
    private void UserTextTextBox_Enter(object sender, EventArgs e)
    {
        CalculateUserTextValue(new Point(0, 0));
    }
    private void UserTextTextBox_TextChanged(object sender, EventArgs e)
    {
    }
    private string RemoveVerseAddresses(string text)
    {
        if (string.IsNullOrEmpty(text)) return null;

        string[] lines = text.Split('\n');
        StringBuilder str = new StringBuilder();
        foreach (string line in lines)
        {
            if (line.Length > 0)
            {
                string[] line_parts = line.Split('\t'); // (TAB delimited)
                if (line_parts.Length > 1) // has address
                {
                    str.Append(line_parts[1] + "\n");  // remove verse address
                }
                else if (line_parts.Length > 0)
                {
                    str.Append(line_parts[0] + "\n");  // leave it as it is
                }
            }
        }
        if (str.Length > 1)
        {
            str.Remove(str.Length - 1, 1);
        }
        return str.ToString();
    }
    private string RemoveVerseEndMarks(string text)
    {
        // RTL script misplaces brackets
        return text; // do nothing for now

        //if (string.IsNullOrEmpty(text)) return null;
        //while (text.Contains(Verse.OPEN_BRACKET) || text.Contains(Verse.CLOSE_BRACKET))
        //{
        //    int start = text.IndexOf(Verse.OPEN_BRACKET);
        //    int end = text.IndexOf(Verse.CLOSE_BRACKET);
        //    if ((start >= 0) && (end >= 0))
        //    {
        //        if (start < end)
        //        {
        //            text = text.Remove(start, (end - start) + 1); // remove space after it
        //        }
        //        else // Arabic script misplaces brackets
        //        {
        //            text = text.Remove(end, (start - end) + 1); // remove space after it
        //        }
        //    }
        //}
        //return text;
    }

    // used for non-Quran text
    private void CalculateValueAndDisplayFactors(string user_text)
    {
        if (m_client != null)
        {
            long value = m_client.CalculateValue(user_text);
            FactorizeValue(value, "Text", false);
        }
    }
    // used for Quran text only
    private void CalculateValueAndDisplayFactors(Verse verse)
    {
        if (m_client != null)
        {
            long value = m_client.CalculateValue(verse);
            FactorizeValue(value, "Value", false);
        }
    }
    private void CalculateValueAndDisplayFactors(List<Verse> verses)
    {
        if (m_client != null)
        {
            long value = m_client.CalculateValue(verses);
            FactorizeValue(value, "Value", false);
        }
    }
    private void CalculateValueAndDisplayFactors(Chapter chapter)
    {
        if (m_client != null)
        {
            long value = m_client.CalculateValue(chapter);
            FactorizeValue(value, "Value", false);
        }
    }
    private void CalculateValueAndDisplayFactors(List<Verse> verses, int letter_index_in_verse1, int letter_index_in_verse2)
    {
        if (m_client != null)
        {
            long value = m_client.CalculateValue(verses, letter_index_in_verse1, letter_index_in_verse2);
            FactorizeValue(value, "Value", false);
        }
    }

    private void ValueTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            if (e.KeyCode == Keys.A)
            {
                if (sender is TextBoxBase)
                {
                    (sender as TextBoxBase).SelectAll();
                }
            }
        }
        else if (e.KeyCode == Keys.Enter)
        {
            CalculateExpression();
        }
        else
        {
            ValueTextBox.ForeColor = Color.DarkGray;
        }
    }
    private void CalculateExpression()
    {
        if (m_client != null)
        {
            string expression = ValueTextBox.Text;

            long value = 0L;
            if (long.TryParse(expression, out value))
            {
                m_double_value = (double)value;
                FactorizeValue(value, "Number", true);
            }
            else if (expression.IsArabic())
            {
                m_double_value = m_client.CalculateValue(expression);
                value = (long)Math.Round(m_double_value);
                FactorizeValue(value, "Text" + expression, true); // user_text
            }
            else
            {
                m_double_value = CalculateValue(expression, m_radix);
                value = (long)Math.Round(m_double_value);
                FactorizeValue(value, "Expression", true);
            }

            // if result has fraction, display it as is
            // PrimeFactorsTextBox_DoubleClick will toggle it back to long
            if (m_double_value != value)
            {
                PrimeFactorsTextBox.Text = m_double_value.ToString();
            }
        }
    }
    private double m_double_value = 0.0D;
    private bool m_double_value_displayed = false;
    private void PrimeFactorsTextBox_DoubleClick(object sender, EventArgs e)
    {
        // toggle double_value <--> prime factors
        if (m_double_value_displayed)
        {
            try
            {
                // display prime factors
                m_double_value = double.Parse(PrimeFactorsTextBox.Text);
                long value = (long)Math.Round(m_double_value);
                FactorizeValue(value, "Expression", true);
                m_double_value_displayed = false;
            }
            catch
            {
                // silence error and do nothing
            }
        }
        else // m_double_value_displayed == false
        {
            if (ValueLabel.Text == "Formula")
            {
                // display double_value
                PrimeFactorsTextBox.Text = m_double_value.ToString();
                m_double_value_displayed = true;
            }
            else
            {
                // do nothing
            }
        }
    }
    private double CalculateValue(string expression, long radix)
    {
        double result = 0D;
        if (m_client != null)
        {
            try
            {
                result = Radix.Decode(expression, radix);
                this.ToolTip.SetToolTip(this.ValueTextBox, result.ToString());
            }
            catch // if expression
            {
                string text = CalculateExpression(expression, radix);
                this.ToolTip.SetToolTip(this.ValueTextBox, text); // display the decimal expansion

                try
                {
                    result = double.Parse(text);
                }
                catch
                {
                    result = m_client.CalculateValue(expression);
                }
            }
        }
        return result;
    }
    private string CalculateExpression(string expression, long radix)
    {
        try
        {
            return Evaluator.Evaluate(expression, radix);
        }
        catch
        {
            return expression;
        }
    }
    private void FactorizeValue(long value, string caption, bool overwrite)
    {
        try
        {
            m_double_value_displayed = false;

            if (caption.StartsWith("Text")) // user_text
            {
                ValueLabel.Text = "Text";
                ValueLabel.Refresh();

                this.ToolTip.SetToolTip(this.ValueTextBox, "Value of  " + caption.Substring(4));
            }
            else
            {
                ValueLabel.Text = caption;
                ValueLabel.Refresh();

                if (caption == "Value")
                {
                    this.ToolTip.SetToolTip(this.ValueTextBox, "Selection value  القيمة حسب نظام الترقيم الحالي");
                }
                else if (caption == "Number")
                {
                    this.ToolTip.SetToolTip(this.ValueTextBox, "User number");
                }
                else if (caption == "Expression")
                {
                    this.ToolTip.SetToolTip(this.ValueTextBox, "Math expression");
                    m_double_value_displayed = true;
                }
                else
                {
                    this.ToolTip.SetToolTip(this.ValueTextBox, caption);
                }
            }

            // if there is a math expression, add to it, don't overwrite it
            if (!overwrite &&
                 (
                   (ValueTextBox.Text.EndsWith("+")) ||
                   (ValueTextBox.Text.EndsWith("-")) ||
                   (ValueTextBox.Text.EndsWith("*")) ||
                   (ValueTextBox.Text.EndsWith("/")) ||
                   (ValueTextBox.Text.EndsWith("^")) ||
                   (ValueTextBox.Text.EndsWith("%"))
                 )
               )
            {
                ValueLabel.Text = "Expression";
                ValueTextBox.Text += Radix.Encode(value, m_radix);

                // focus so user can continue with +, -, *, /, ^, %, Enter
                ValueTextBox.SelectionLength = 0;
                ValueTextBox.SelectionStart = ValueTextBox.Text.Length;
                ValueTextBox.Focus();
            }
            else
            {
                ValueTextBox.Text = Radix.Encode(value, m_radix);
                ValueTextBox.ForeColor = GetNumberTypeColor(ValueTextBox.Text, m_radix);
                ValueTextBox.SelectionStart = ValueTextBox.Text.Length;
                ValueTextBox.SelectionLength = 0;
                ValueTextBox.Refresh();

                DecimalValueTextBox.Text = value.ToString();
                DecimalValueTextBox.Visible = (m_radix != DEFAULT_RADIX);
                DecimalValueTextBox.ForeColor = GetNumberTypeColor(value);
                DecimalValueTextBox.Refresh();

                DigitSumTextBox.Text = Numbers.DigitSum(ValueTextBox.Text).ToString();
                DigitSumTextBox.ForeColor = GetNumberTypeColor(DigitSumTextBox.Text, m_radix);
                DigitSumTextBox.Refresh();

                DigitalRootTextBox.Text = Numbers.DigitalRoot(ValueTextBox.Text).ToString();
                DigitalRootTextBox.ForeColor = GetNumberTypeColor(DigitalRootTextBox.Text, m_radix);
                DigitalRootTextBox.Refresh();

                PrimeFactorsTextBox.Text = Numbers.FactorizeToString(value);
                PrimeFactorsTextBox.BackColor = (Numbers.Compare(value, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
                PrimeFactorsTextBox.Refresh();

                int nth_prime_index = -1;
                int nth_additive_prime_index = -1;
                int nth_pure_prime_index = -1;
                if (Numbers.IsPrime(value))
                {
                    nth_prime_index = Numbers.IndexOfPrime(value);
                    nth_additive_prime_index = Numbers.IndexOfAdditivePrime(value);
                    nth_pure_prime_index = Numbers.IndexOfPurePrime(value);
                    NthPrimeLabel.Text = "P";
                    NthPrimeLabel.Refresh();
                    NthAdditivePrimeLabel.Text = "AP";
                    NthAdditivePrimeLabel.Refresh();
                    NthPurePrimeLabel.Text = "PP";
                    NthPurePrimeLabel.Refresh();

                    NthPrimeTextBox.BackColor = Color.FromArgb(240, 255, 240);
                    NthAdditivePrimeTextBox.BackColor = Color.FromArgb(224, 224, 255);
                    NthPurePrimeTextBox.BackColor = Color.FromArgb(255, 224, 255);

                    ToolTip.SetToolTip(NthPrimeTextBox, "Find prime by index");
                    ToolTip.SetToolTip(NthAdditivePrimeTextBox, "Find additive prime by index");
                    ToolTip.SetToolTip(NthPurePrimeTextBox, "Find pure prime by index");
                }
                else
                {
                    nth_prime_index = Numbers.IndexOfComposite(value) + 1;
                    if (nth_prime_index == 0)
                    {
                        nth_prime_index = -1;
                    }
                    nth_additive_prime_index = Numbers.IndexOfAdditiveComposite(value) + 1;
                    if (nth_additive_prime_index == 0)
                    {
                        nth_additive_prime_index = -1;
                    }
                    nth_pure_prime_index = Numbers.IndexOfPureComposite(value) + 1;
                    if (nth_pure_prime_index == 0)
                    {
                        nth_pure_prime_index = -1;
                    }
                    NthPrimeLabel.Text = "C";
                    NthPrimeLabel.Refresh();
                    NthAdditivePrimeLabel.Text = "AC";
                    NthAdditivePrimeLabel.Refresh();
                    NthPurePrimeLabel.Text = "PC";
                    NthPurePrimeLabel.Refresh();

                    NthPrimeTextBox.BackColor = Color.FromArgb(208, 208, 208);
                    NthAdditivePrimeTextBox.BackColor = Color.FromArgb(208, 192, 192);
                    NthPurePrimeTextBox.BackColor = Color.FromArgb(240, 204, 204);

                    ToolTip.SetToolTip(NthPrimeTextBox, "Find composite by index");
                    ToolTip.SetToolTip(NthAdditivePrimeTextBox, "Find additive composite by index");
                    ToolTip.SetToolTip(NthPurePrimeTextBox, "Find pure composite by index");
                }

                NthPrimeTextBox.ForeColor = GetNumberTypeColor(nth_prime_index);
                NthPrimeTextBox.Text = nth_prime_index.ToString();
                NthPrimeTextBox.Refresh();

                NthAdditivePrimeTextBox.ForeColor = GetNumberTypeColor(nth_additive_prime_index);
                NthAdditivePrimeTextBox.Text = nth_additive_prime_index.ToString();
                NthAdditivePrimeTextBox.Refresh();

                NthPurePrimeTextBox.ForeColor = GetNumberTypeColor(nth_pure_prime_index);
                NthPurePrimeTextBox.Text = nth_pure_prime_index.ToString();
                NthPurePrimeTextBox.Refresh();
            }

            //// ??? TODO: hangs if comes from Position
            //ValueTextBox.SelectionLength = 0;
            //ValueTextBox.SelectionStart = ValueTextBox.Text.Length;
            //ValueTextBox.Focus();
        }
        catch //(Exception ex)
        {
            //MessageBox.Show(ex.Message, Application.ProductName);
        }
    }
    private void NthPrimeTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            try
            {
                long number = -1L;
                int nth_index = int.Parse(NthPrimeTextBox.Text);
                if (nth_index < 0)
                {
                    number = -1L;
                }
                else
                {
                    NthPrimeTextBox.ForeColor = GetNumberTypeColor(nth_index);
                    if (NthPrimeLabel.Text == "P")
                    {
                        number = Numbers.Primes[nth_index];
                    }
                    else //if (NthPrimeLabel.Text == "C Index")
                    {
                        if (nth_index == 0)
                        {
                            number = 0L;
                        }
                        else
                        {
                            number = Numbers.Composites[nth_index - 1];
                        }
                    }
                }
                FactorizeValue(number, NthPrimeLabel.Text, true);
            }
            catch
            {
                FactorizeValue(0L, "Error", true);
            }
        }
    }
    private void NthAdditivePrimeTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            try
            {
                long number = -1L;
                int nth_index = int.Parse(NthAdditivePrimeTextBox.Text);
                if (nth_index < 0)
                {
                    number = -1L;
                }
                else
                {
                    NthAdditivePrimeTextBox.ForeColor = GetNumberTypeColor(nth_index);
                    if (NthAdditivePrimeLabel.Text == "AP")
                    {
                        number = Numbers.AdditivePrimes[nth_index];
                    }
                    else //if (NthAdditivePrimeLabel.Text == "AC Index")
                    {
                        if (nth_index == 0)
                        {
                            number = 0L;
                        }
                        else
                        {
                            number = Numbers.AdditiveComposites[nth_index - 1];
                        }
                    }
                }
                FactorizeValue(number, NthAdditivePrimeLabel.Text, true);
            }
            catch
            {
                FactorizeValue(0L, "Error", true);
            }
        }
    }
    private void NthPurePrimeTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            try
            {
                long number = -1L;
                int nth_index = int.Parse(NthPurePrimeTextBox.Text);
                if (nth_index < 0)
                {
                    number = -1L;
                }
                else
                {
                    NthPurePrimeTextBox.ForeColor = GetNumberTypeColor(nth_index);
                    if (NthPurePrimeLabel.Text == "PP")
                    {
                        number = Numbers.PurePrimes[nth_index];
                    }
                    else //if (NthPurePrimeLabel.Text == "PC Index")
                    {
                        if (nth_index == 0)
                        {
                            number = 0L;
                        }
                        else
                        {
                            number = Numbers.PureComposites[nth_index - 1];
                        }
                    }
                }
                FactorizeValue(number, NthPurePrimeLabel.Text, true);
            }
            catch
            {
                FactorizeValue(0L, "Error", true);
            }
        }
    }
    private Color GetNumberTypeColor(long number)
    {
        return GetNumberTypeColor(number.ToString(), 10);
    }
    private Color GetNumberTypeColor(string value, long radix)
    {
        // if negative number, remove -ve sign
        if (value.StartsWith("-")) value = value.Remove(0, 1);

        if (Numbers.IsUnit(value, radix))
        {
            return Color.FromArgb(180, 0, 208);
        }
        else if (Numbers.IsPurePrime(value, radix))
        {
            return Color.DarkViolet;
        }
        else if (Numbers.IsAdditivePrime(value, radix))
        {
            return Color.Blue;
        }
        else if (Numbers.IsPrime(value, radix))
        {
            return Color.Green;
        }
        else if (Numbers.IsPureComposite(value, radix))
        {
            return Color.OrangeRed;
        }
        else if (Numbers.IsAdditiveComposite(value, radix))
        {
            return Color.Brown;
        }
        else if (Numbers.IsComposite(value, radix))
        {
            return Color.Black;
        }
        else
        {
            return Color.Black;
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 27. Value Display
    ///////////////////////////////////////////////////////////////////////////////
    private int m_radix = DEFAULT_RADIX;
    private void RadixValueLabel_Click(object sender, EventArgs e)
    {
        try
        {
            // get values in current radix
            int chapter_count = (int)Radix.Decode(ChaptersTextBox.Text, m_radix);
            int verse_count = (int)Radix.Decode(VersesTextBox.Text, m_radix);
            int word_count = (int)Radix.Decode(WordsTextBox.Text, m_radix);
            int letter_count = (int)Radix.Decode(LettersTextBox.Text, m_radix);
            ////int chapter_number_sum = (int)Radix.Decode(ChapterNumberSumTextBox.Text.Split()[1], m_radix);
            ////int verse_number_sum = (int)Radix.Decode(VerseNumberSumTextBox.Text.Split()[1], m_radix);
            ////int word_number_sum = (int)Radix.Decode(WordNumberSumTextBox.Text.Split()[1], m_radix);
            ////int letter_number_sum = (int)Radix.Decode(LetterNumberSumTextBox.Text.Split()[1], m_radix);
            //int chapter_number_sum = int.Parse(ChapterNumberSumTextBox.Text.Split()[1]);
            //int verse_number_sum = int.Parse(VerseNumberSumTextBox.Text.Split()[1]);
            //int word_number_sum = int.Parse(WordNumberSumTextBox.Text.Split()[1]);
            //int letter_number_sum = int.Parse(LetterNumberSumTextBox.Text.Split()[1]);
            long value = Radix.Decode(ValueTextBox.Text, m_radix);

            // toggle radix
            if (m_radix == DEFAULT_RADIX)
            {
                m_radix = RADIX_NINTEEN;
            }
            else
            {
                m_radix = DEFAULT_RADIX;
            }
            RadixValueLabel.Text = m_radix.ToString();

            // display values in nex radix
            //DisplayCounts(chapter_count, verse_count, word_count, letter_count, chapter_number_sum, verse_number_sum, word_number_sum, letter_number_sum);
            DisplayCounts(chapter_count, verse_count, word_count, letter_count, -1, -1, -1, -1); // -1 means don't change what is displayed
            FactorizeValue(value, "Value", true);
        }
        catch
        {
            // log exception
        }
    }
    private void RadixValueUpLabel_Click(object sender, EventArgs e)
    {
        try
        {
            // get values in current radix
            int chapter_count = (int)Radix.Decode(ChaptersTextBox.Text, m_radix);
            int verse_count = (int)Radix.Decode(VersesTextBox.Text, m_radix);
            int word_count = (int)Radix.Decode(WordsTextBox.Text, m_radix);
            int letter_count = (int)Radix.Decode(LettersTextBox.Text, m_radix);
            ////int chapter_number_sum = (int)Radix.Decode(ChapterNumberSumTextBox.Text.Split()[1], m_radix);
            ////int verse_number_sum = (int)Radix.Decode(VerseNumberSumTextBox.Text.Split()[1], m_radix);
            ////int word_number_sum = (int)Radix.Decode(WordNumberSumTextBox.Text.Split()[1], m_radix);
            ////int letter_number_sum = (int)Radix.Decode(LetterNumberSumTextBox.Text.Split()[1], m_radix);
            //int chapter_number_sum = int.Parse(ChapterNumberSumTextBox.Text.Split()[1]);
            //int verse_number_sum = int.Parse(VerseNumberSumTextBox.Text.Split()[1]);
            //int word_number_sum = int.Parse(WordNumberSumTextBox.Text.Split()[1]);
            //int letter_number_sum = int.Parse(LetterNumberSumTextBox.Text.Split()[1]);
            long value = Radix.Decode(ValueTextBox.Text, m_radix);

            // increment radix
            m_radix++;
            if (m_radix > Radix.DIGITS.Length) m_radix = 2;
            RadixValueLabel.Text = m_radix.ToString();

            // display values in nex radix
            //DisplayCounts(chapter_count, verse_count, word_count, letter_count, chapter_number_sum, verse_number_sum, word_number_sum, letter_number_sum);
            DisplayCounts(chapter_count, verse_count, word_count, letter_count, -1, -1, -1, -1); // -1 means don't change what is displayed
            FactorizeValue(value, "Value", true);
        }
        catch
        {
            // log exception
        }
    }
    private void RadixValueDownLabel_Click(object sender, EventArgs e)
    {
        try
        {
            // get values in current radix
            int chapter_count = (int)Radix.Decode(ChaptersTextBox.Text, m_radix);
            int verse_count = (int)Radix.Decode(VersesTextBox.Text, m_radix);
            int word_count = (int)Radix.Decode(WordsTextBox.Text, m_radix);
            int letter_count = (int)Radix.Decode(LettersTextBox.Text, m_radix);
            ////int chapter_number_sum = (int)Radix.Decode(ChapterNumberSumTextBox.Text.Split()[1], m_radix);
            ////int verse_number_sum = (int)Radix.Decode(VerseNumberSumTextBox.Text.Split()[1], m_radix);
            ////int word_number_sum = (int)Radix.Decode(WordNumberSumTextBox.Text.Split()[1], m_radix);
            ////int letter_number_sum = (int)Radix.Decode(LetterNumberSumTextBox.Text.Split()[1], m_radix);
            //int chapter_number_sum = int.Parse(ChapterNumberSumTextBox.Text.Split()[1]);
            //int verse_number_sum = int.Parse(VerseNumberSumTextBox.Text.Split()[1]);
            //int word_number_sum = int.Parse(WordNumberSumTextBox.Text.Split()[1]);
            //int letter_number_sum = int.Parse(LetterNumberSumTextBox.Text.Split()[1]);
            long value = Radix.Decode(ValueTextBox.Text, m_radix);

            // increment radix
            m_radix--;
            if (m_radix < 2) m_radix = Radix.DIGITS.Length;
            RadixValueLabel.Text = m_radix.ToString();

            // display values in nex radix
            //DisplayCounts(chapter_count, verse_count, word_count, letter_count, chapter_number_sum, verse_number_sum, word_number_sum, letter_number_sum);
            DisplayCounts(chapter_count, verse_count, word_count, letter_count, -1, -1, -1, -1); // -1 means don't change what is displayed
            FactorizeValue(value, "Value", true);
        }
        catch
        {
            // log exception
        }
    }
    private int m_divisor = DEFAULT_DIVISOR;
    private void DivisorValueLabel_Click(object sender, EventArgs e)
    {
        m_divisor = DEFAULT_DIVISOR;
        DivisorValueLabel.Text = m_divisor.ToString();
        UpdateBackColorsByDivisor();
    }
    private void DivisorValueUpLabel_Click(object sender, EventArgs e)
    {
        m_divisor++;
        if (m_divisor > MAX_DIVISOR) m_divisor = MIN_DIVISOR;
        DivisorValueLabel.Text = m_divisor.ToString();
        UpdateBackColorsByDivisor();
    }
    private void DivisorValueDownLabel_Click(object sender, EventArgs e)
    {
        m_divisor--;
        if (m_divisor < MIN_DIVISOR) m_divisor = MAX_DIVISOR;
        DivisorValueLabel.Text = m_divisor.ToString();
        UpdateBackColorsByDivisor();
    }
    private void UpdateBackColorsByDivisor()
    {
        try
        {
            long value = long.Parse(ValueTextBox.Text);
            PrimeFactorsTextBox.BackColor = (Numbers.Compare(value, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
            PrimeFactorsTextBox.Refresh();

            int chapter_count = int.Parse(ChaptersTextBox.Text);
            ChaptersTextBox.BackColor = (Numbers.Compare(chapter_count, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
            ChaptersTextBox.Refresh();

            int verse_count = int.Parse(VersesTextBox.Text);
            VersesTextBox.BackColor = (Numbers.Compare(verse_count, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
            VersesTextBox.Refresh();

            int word_count = int.Parse(WordsTextBox.Text);
            WordsTextBox.BackColor = (Numbers.Compare(word_count, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
            WordsTextBox.Refresh();

            int letter_count = int.Parse(LettersTextBox.Text);
            LettersTextBox.BackColor = (Numbers.Compare(letter_count, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
            LettersTextBox.Refresh();

            string chapter_number_sum_text = ChapterNumberSumTextBox.Text.Substring(2);
            int chapter_number_sum = int.Parse(chapter_number_sum_text);
            ChapterNumberSumTextBox.BackColor = (Numbers.Compare(chapter_number_sum, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
            ChapterNumberSumTextBox.Refresh();

            string verse_number_sum_text = VerseNumberSumTextBox.Text.Substring(2);
            int verse_number_sum = int.Parse(verse_number_sum_text);
            VerseNumberSumTextBox.BackColor = (Numbers.Compare(verse_number_sum, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
            VerseNumberSumTextBox.Refresh();

            string word_number_sum_text = WordNumberSumTextBox.Text.Substring(2);
            int word_number_sum = int.Parse(word_number_sum_text);
            WordNumberSumTextBox.BackColor = (Numbers.Compare(word_number_sum, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
            WordNumberSumTextBox.Refresh();

            string letter_number_sum_text = LetterNumberSumTextBox.Text.Substring(2);
            int letter_number_sum = int.Parse(letter_number_sum_text);
            LetterNumberSumTextBox.BackColor = (Numbers.Compare(letter_number_sum, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
            LetterNumberSumTextBox.Refresh();

            int sum = int.Parse(LetterFrequencySumLabel.Text);
            LetterFrequencyPrimeFactorsTextBox.BackColor = (Numbers.Compare(sum, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
            LetterFrequencyPrimeFactorsTextBox.Refresh();
        }
        catch
        {
            // ignore
        }
    }
    private void CalculateAndDisplayCounts(string user_text)
    {
        if (!String.IsNullOrEmpty(user_text))
        {
            if (m_client != null)
            {
                ChapterNumberSumTextBox.Text = "";
                VerseNumberSumTextBox.Text = "";
                WordNumberSumTextBox.Text = "";
                LetterNumberSumTextBox.Text = "";

                if (!user_text.IsArabic())  // eg English
                {
                    user_text = user_text.ToUpper();
                }

                // in all cases
                if (m_client.NumerologySystem != null)
                {
                    // simplify all text_modes
                    user_text = user_text.SimplifyTo(m_client.NumerologySystem.TextMode);
                    user_text = user_text.Replace("_", "");
                    user_text = user_text.Replace("\t", "");
                    while (user_text.Contains("  "))
                    {
                        user_text = user_text.Replace("  ", " ");
                    }
                    user_text = user_text.Replace("\r\n", "\n");

                    int chapter_count = 1;
                    int verse_count = 1;
                    int word_count = 1;
                    int letter_count = 0;
                    foreach (char c in user_text)
                    {
                        if (c == '\n')
                        {
                            verse_count++;
                            if (letter_count > 0)
                            {
                                word_count++;
                            }
                        }
                        else if (c == ' ')
                        {
                            word_count++;
                        }
                        else
                        {
                            letter_count++;
                        }
                    }
                    DisplayCounts(chapter_count, verse_count, word_count, letter_count, -1, -1, -1, -1); // -1 means don't change what is displayed
                }
            }
        }
        else
        {
            DisplayCounts(0, 0, 0, 0, -1, -1, -1, -1); // -1 means don't change what is displayed
        }
    }
    private void CalculateAndDisplayCounts(Verse verse)
    {
        if (verse != null)
        {
            int chapter_count = 1;
            int verse_count = 1;
            int word_count = verse.Words.Count;
            int letter_count = verse.LetterCount;
            int chapter_number_sum = verse.Chapter.Number;
            int verse_number_sum = verse.NumberInChapter;
            int word_number_sum = 0;
            int letter_number_sum = 0;

            if (verse.Words != null)
            {
                foreach (Word word in verse.Words)
                {
                    word_number_sum += word.NumberInVerse;
                    if ((word.Letters != null) && (word.Letters.Count > 0))
                    {
                        foreach (Letter letter in word.Letters)
                        {
                            letter_number_sum += letter.NumberInWord;
                        }
                    }
                }
            }
            DisplayCounts(chapter_count, verse_count, word_count, letter_count, chapter_number_sum, verse_number_sum, word_number_sum, letter_number_sum);
        }
    }
    private void CalculateAndDisplayCounts(List<Verse> verses)
    {
        if (verses != null)
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    List<Chapter> chapters = m_client.Book.GetChapters(verses);
                    if (chapters != null)
                    {
                        int chapter_count = chapters.Count;
                        int verse_count = verses.Count;
                        int word_count = 0;
                        int letter_count = 0;
                        int chapter_number_sum = 0;
                        int verse_number_sum = 0;
                        int word_number_sum = 0;
                        int letter_number_sum = 0;
                        foreach (Chapter chapter in chapters)
                        {
                            if (chapter != null)
                            {
                                chapter_number_sum += chapter.Number;
                            }
                        }

                        foreach (Verse verse in verses)
                        {
                            word_count += verse.Words.Count;
                            letter_count += verse.LetterCount;

                            verse_number_sum += verse.NumberInChapter;
                            if (verse.Words != null)
                            {
                                foreach (Word word in verse.Words)
                                {
                                    word_number_sum += word.NumberInVerse;
                                    if ((word.Letters != null) && (word.Letters.Count > 0))
                                    {
                                        foreach (Letter letter in word.Letters)
                                        {
                                            letter_number_sum += letter.NumberInWord;
                                        }
                                    }
                                }
                            }
                        }
                        DisplayCounts(chapter_count, verse_count, word_count, letter_count, chapter_number_sum, verse_number_sum, word_number_sum, letter_number_sum);

                        DisplayTranslations(verses); // display translations for selected verses
                    }
                }
            }
        }
    }
    private void CalculateAndDisplayCounts(List<Verse> verses, int letter_index_in_verse1, int letter_index_in_verse2)
    {
        if (verses != null)
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    List<Chapter> chapters = m_client.Book.GetChapters(verses);
                    if (chapters != null)
                    {
                        int chapter_count = chapters.Count;
                        int verse_count = verses.Count;
                        int word_count = 0;
                        int letter_count = 0;
                        int chapter_number_sum = 0;
                        int verse_number_sum = 0;
                        int word_number_sum = 0;
                        int letter_number_sum = 0;
                        foreach (Chapter chapter in chapters)
                        {
                            if (chapter != null)
                            {
                                chapter_number_sum += chapter.Number;
                            }
                        }

                        ///////////////////////////
                        // Middle Verse Part (verse1, letter_index_in_verse1, letter_index_in_verse2);
                        ///////////////////////////
                        if (verses.Count == 1)
                        {
                            Verse verse1 = verses[0];
                            if (verse1 != null)
                            {
                                verse_number_sum += verse1.NumberInChapter;

                                if (verse1.Words != null)
                                {
                                    foreach (Word word in verse1.Words)
                                    {
                                        if (word != null)
                                        {
                                            if ((word.Letters != null) && (word.Letters.Count > 0))
                                            {
                                                if ((word.Letters[word.Letters.Count - 1].NumberInVerse - 1) < letter_index_in_verse1) continue;
                                                if ((word.Letters[0].NumberInVerse - 1) > letter_index_in_verse2) break;
                                                word_count++;
                                                word_number_sum += word.NumberInVerse;

                                                foreach (Letter letter in word.Letters)
                                                {
                                                    if (letter != null)
                                                    {
                                                        if ((letter.NumberInVerse - 1) < letter_index_in_verse1) continue;
                                                        if ((letter.NumberInVerse - 1) > letter_index_in_verse2) break;
                                                        letter_count++;
                                                        letter_number_sum += letter.NumberInWord;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (verses.Count == 2)
                        {
                            ///////////////////////////
                            // End Verse Part (verse1, letter_index_in_verse1);
                            ///////////////////////////
                            Verse verse1 = verses[0];
                            if (verse1 != null)
                            {
                                verse_number_sum += verse1.NumberInChapter;

                                if (verse1.Words != null)
                                {
                                    foreach (Word word in verse1.Words)
                                    {
                                        if (word != null)
                                        {
                                            if ((word.Letters != null) && (word.Letters.Count > 0))
                                            {
                                                if ((word.Letters[word.Letters.Count - 1].NumberInVerse - 1) < letter_index_in_verse1) continue;
                                                word_count++;
                                                word_number_sum += word.NumberInVerse;

                                                foreach (Letter letter in word.Letters)
                                                {
                                                    if (letter != null)
                                                    {
                                                        if ((letter.NumberInVerse - 1) < letter_index_in_verse1) continue;
                                                        letter_count++;
                                                        letter_number_sum += letter.NumberInWord;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            ///////////////////////////
                            // Beginning Verse Part (verse2, letter_index_in_verse2);
                            ///////////////////////////
                            Verse verse2 = verses[1];
                            if (verse2 != null)
                            {
                                verse_number_sum += verse2.NumberInChapter;

                                if (verse2.Words != null)
                                {
                                    foreach (Word word in verse2.Words)
                                    {
                                        if (word != null)
                                        {
                                            if ((word.Letters != null) && (word.Letters.Count > 0))
                                            {
                                                if ((word.Letters[0].NumberInVerse - 1) > letter_index_in_verse2) break;
                                                word_count++;
                                                word_number_sum += word.NumberInVerse;

                                                foreach (Letter letter in word.Letters)
                                                {
                                                    if (letter != null)
                                                    {
                                                        if ((letter.NumberInVerse - 1) > letter_index_in_verse2) break;
                                                        letter_count++;
                                                        letter_number_sum += letter.NumberInWord;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (verses.Count > 2)
                        {
                            ///////////////////////////
                            // End Verse Part (verse1, letter_index_in_verse1);
                            ///////////////////////////
                            Verse verse1 = verses[0];
                            if (verse1 != null)
                            {
                                verse_number_sum += verse1.NumberInChapter;

                                if (verse1.Words != null)
                                {
                                    foreach (Word word in verse1.Words)
                                    {
                                        if (word != null)
                                        {
                                            if ((word.Letters != null) && (word.Letters.Count > 0))
                                            {
                                                if ((word.Letters[word.Letters.Count - 1].NumberInVerse - 1) < letter_index_in_verse1) continue;
                                                word_count++;
                                                word_number_sum += word.NumberInVerse;

                                                foreach (Letter letter in word.Letters)
                                                {
                                                    if (letter != null)
                                                    {
                                                        if ((letter.NumberInVerse - 1) < letter_index_in_verse1) continue;
                                                        letter_count++;
                                                        letter_number_sum += letter.NumberInWord;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            ///////////////////////////
                            // Middle Verses
                            ///////////////////////////
                            for (int i = 1; i < verses.Count - 1; i++)
                            {
                                Verse verse = verses[i];
                                if (verse != null)
                                {
                                    verse_number_sum += verse.NumberInChapter;

                                    if (verse.Words != null)
                                    {
                                        foreach (Word word in verse.Words)
                                        {
                                            word_count++;
                                            word_number_sum += word.NumberInVerse;

                                            if (word != null)
                                            {
                                                if ((word.Letters != null) && (word.Letters.Count > 0))
                                                {
                                                    foreach (Letter letter in word.Letters)
                                                    {
                                                        if (letter != null)
                                                        {
                                                            letter_count++;
                                                            letter_number_sum += letter.NumberInWord;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            ///////////////////////////
                            // Beginning Verse Part (verse2, letter_index_in_verse2);
                            ///////////////////////////
                            Verse verse2 = verses[verses.Count - 1];
                            if (verse2 != null)
                            {
                                verse_number_sum += verse2.NumberInChapter;

                                if (verse2.Words != null)
                                {
                                    foreach (Word word in verse2.Words)
                                    {
                                        if (word != null)
                                        {
                                            if ((word.Letters != null) && (word.Letters.Count > 0))
                                            {
                                                if ((word.Letters[0].NumberInVerse - 1) > letter_index_in_verse2) break;
                                                word_count++;
                                                word_number_sum += word.NumberInVerse;

                                                foreach (Letter letter in word.Letters)
                                                {
                                                    if (letter != null)
                                                    {
                                                        if ((letter.NumberInVerse - 1) > letter_index_in_verse2) break;
                                                        letter_count++;
                                                        letter_number_sum += letter.NumberInWord;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else // verses.Count == 0
                        {
                            // do nothing
                        }
                        DisplayCounts(chapter_count, verse_count, word_count, letter_count, chapter_number_sum, verse_number_sum, word_number_sum, letter_number_sum);

                        DisplayTranslations(verses); // display translations for selected verses
                    }
                }
            }
        }
    }
    private void DisplayCounts(int chapter_count, int verse_count, int word_count, int letter_count)
    {
        ChaptersTextBox.Text = Radix.Encode(chapter_count, m_radix);
        ChaptersTextBox.ForeColor = GetNumberTypeColor(ChaptersTextBox.Text, m_radix);
        ChaptersTextBox.BackColor = (Numbers.Compare(chapter_count, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
        ChaptersTextBox.Refresh();

        DecimalChaptersTextBox.Text = chapter_count.ToString();
        DecimalChaptersTextBox.ForeColor = GetNumberTypeColor(chapter_count);
        DecimalChaptersTextBox.Visible = (m_radix != DEFAULT_RADIX);
        DecimalChaptersTextBox.Refresh();

        VersesTextBox.Text = Radix.Encode(verse_count, m_radix);
        VersesTextBox.ForeColor = GetNumberTypeColor(VersesTextBox.Text, m_radix);
        VersesTextBox.BackColor = (Numbers.Compare(verse_count, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
        VersesTextBox.Refresh();

        DecimalVersesTextBox.Text = verse_count.ToString();
        DecimalVersesTextBox.ForeColor = GetNumberTypeColor(verse_count);
        DecimalVersesTextBox.Visible = (m_radix != DEFAULT_RADIX);
        DecimalVersesTextBox.Refresh();

        WordsTextBox.Text = Radix.Encode(word_count, m_radix);
        WordsTextBox.ForeColor = GetNumberTypeColor(WordsTextBox.Text, m_radix);
        WordsTextBox.BackColor = (Numbers.Compare(word_count, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
        WordsTextBox.Refresh();

        DecimalWordsTextBox.Text = word_count.ToString();
        DecimalWordsTextBox.ForeColor = GetNumberTypeColor(word_count);
        DecimalWordsTextBox.Visible = (m_radix != DEFAULT_RADIX);
        DecimalWordsTextBox.Refresh();

        LettersTextBox.Text = Radix.Encode(letter_count, m_radix);
        LettersTextBox.ForeColor = GetNumberTypeColor(LettersTextBox.Text, m_radix);
        LettersTextBox.BackColor = (Numbers.Compare(letter_count, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
        LettersTextBox.Refresh();

        DecimalLettersTextBox.Text = letter_count.ToString();
        DecimalLettersTextBox.ForeColor = GetNumberTypeColor(letter_count);
        DecimalLettersTextBox.Visible = (m_radix != DEFAULT_RADIX);
        DecimalLettersTextBox.Refresh();
    }
    private void DisplayCounts(int chapter_count, int verse_count, int word_count, int letter_count, int chapter_number_sum, int verse_number_sum, int word_number_sum, int letter_number_sum)
    {
        DisplayCounts(chapter_count, verse_count, word_count, letter_count);

        if (chapter_number_sum != -1)
        {
            //ChapterNumberSumTextBox.Text = SUM_SYMBOL + Radix.Encode(chapter_number_sum, m_radix);
            //ChapterNumberSumTextBox.ForeColor = GetNumberTypeColor(ChapterNumberSumTextBox.Text.Split()[1], m_radix);
            ChapterNumberSumTextBox.Text = SUM_SYMBOL + chapter_number_sum.ToString();
            ChapterNumberSumTextBox.ForeColor = GetNumberTypeColor(chapter_number_sum);
            ChapterNumberSumTextBox.BackColor = (Numbers.Compare(chapter_number_sum, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
            ChapterNumberSumTextBox.Refresh();
        }

        if (verse_number_sum != -1)
        {
            //VerseNumberSumTextBox.Text = SUM_SYMBOL + Radix.Encode(verse_number_sum, m_radix);
            //VerseNumberSumTextBox.ForeColor = GetNumberTypeColor(VerseNumberSumTextBox.Text.Split()[1], m_radix);
            VerseNumberSumTextBox.Text = SUM_SYMBOL + verse_number_sum.ToString();
            VerseNumberSumTextBox.ForeColor = GetNumberTypeColor(verse_number_sum);
            VerseNumberSumTextBox.BackColor = (Numbers.Compare(verse_number_sum, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
            VerseNumberSumTextBox.Refresh();
        }

        if (word_number_sum != -1)
        {
            //WordNumberSumTextBox.Text = SUM_SYMBOL + Radix.Encode(word_number_sum, m_radix);
            //WordNumberSumTextBox.ForeColor = GetNumberTypeColor(WordNumberSumTextBox.Text.Split()[1], m_radix);
            WordNumberSumTextBox.Text = SUM_SYMBOL + word_number_sum.ToString();
            WordNumberSumTextBox.ForeColor = GetNumberTypeColor(word_number_sum);
            WordNumberSumTextBox.BackColor = (Numbers.Compare(word_number_sum, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
            WordNumberSumTextBox.Refresh();
        }

        if (letter_number_sum != -1)
        {
            //LetterNumberSumTextBox.Text = SUM_SYMBOL + Radix.Encode(letter_number_sum, m_radix);
            //LetterNumberSumTextBox.ForeColor = GetNumberTypeColor(LetterNumberSumTextBox.Text.Split()[1], m_radix);
            LetterNumberSumTextBox.Text = SUM_SYMBOL + letter_number_sum.ToString();
            LetterNumberSumTextBox.ForeColor = GetNumberTypeColor(letter_number_sum);
            LetterNumberSumTextBox.BackColor = (Numbers.Compare(letter_number_sum, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
            LetterNumberSumTextBox.Refresh();
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 30. Statistics
    ///////////////////////////////////////////////////////////////////////////////
    private List<char> m_selected_letters = new List<char>();
    private void LetterFrequencyListView_ColumnClick(object sender, ColumnClickEventArgs e)
    {
        if (sender is ListView)
        {
            ListView listview = sender as ListView;
            try
            {
                if (m_client != null)
                {
                    m_client.SortLetterStatistics((StatisticSortMethod)e.Column);
                    DisplayLetterFrequencies();

                    // display sort marker
                    string sort_marker = (LetterStatistic.SortOrder == StatisticSortOrder.Ascending) ? "▼" : "▲";
                    // empty all sort markers
                    foreach (ColumnHeader column in listview.Columns)
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
                    // mark clicked column
                    listview.Columns[e.Column].Text = listview.Columns[e.Column].Text.Replace("  ", " " + sort_marker);
                }
            }
            catch
            {
                // log exception
            }
        }
    }
    private void LetterFrequencyListView_SelectedIndexChanged(object sender, EventArgs e)
    {
        // only update m_selected_letters if user manually select items
        // otherwise we would lose items in they don't appear in a selection
        // and would give wrong results for subsequent selections with these items 
        if (LetterFrequencyListView.Focused)
        {
            m_selected_letters.Clear();
            if (LetterFrequencyListView.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in LetterFrequencyListView.SelectedItems)
                {
                    m_selected_letters.Add(item.SubItems[1].Text[0]);
                }
            }
        }

        DisplayLetterFrequenciesTotals();

        FindByFrequencyButton.Enabled = ((m_find_by_phrase) && (m_phrase_text.Length > 0))
                                        ||
                                        ((!m_find_by_phrase) && (LetterFrequencyListView.SelectedItems.Count > 0));
    }
    private void BuildLetterFrequencies()
    {
        if (m_client != null)
        {
            if (
                 (m_text_display_mode == TextDisplayMode.None) ||
                 (m_text_display_mode == TextDisplayMode.TranslationOnly)
               )
            {
                if (!m_found_verses_displayed)
                {
                    m_current_text = m_client.Selection.Text;
                }
            }

            if (!String.IsNullOrEmpty(m_current_text))
            {
                if (FindByFrequencyPhraseTextBox.SelectedText.Length > 0)
                {
                    m_current_phrase = FindByFrequencyPhraseTextBox.SelectedText;
                }
                else
                {
                    m_current_phrase = FindByFrequencyPhraseTextBox.Text;
                }

                if (m_current_phrase.Length > 0)
                {
                    m_client.BuildLetterStatistics(m_current_text, m_current_phrase, m_frequency_search_type);
                }
                else
                {
                    m_client.BuildLetterStatistics(m_current_text);
                }
            }
            else
            {
                m_client.LetterStatistics.Clear();
            }
        }
    }
    private void DisplayLetterFrequencies()
    {
        if (m_client != null)
        {
            if (m_client.LetterStatistics != null)
            {
                LetterFrequencyListView.Items.Clear();
                if (m_client.LetterStatistics.Count > 0)
                {
                    List<int> selected_indexes = new List<int>();
                    for (int i = 0; i < m_client.LetterStatistics.Count; i++)
                    {
                        string[] item_parts = new string[3];
                        item_parts[0] = m_client.LetterStatistics[i].Order.ToString();
                        item_parts[1] = m_client.LetterStatistics[i].Letter.ToString();
                        item_parts[2] = m_client.LetterStatistics[i].Frequency.ToString();
                        LetterFrequencyListView.Items.Add(new ListViewItem(item_parts, i));

                        // re-select user items if any were selected for previous selection
                        if (m_selected_letters.Contains(m_client.LetterStatistics[i].Letter))
                        {
                            selected_indexes.Add(i);
                        }
                    }
                    // must be done after adding all items
                    foreach (int index in selected_indexes)
                    {
                        LetterFrequencyListView.SelectedIndices.Add(index);
                        LetterFrequencyListView.EnsureVisible(index);
                    }
                }

                DisplayLetterFrequenciesTotals();

                // reset sort-markers
                foreach (ColumnHeader column in LetterFrequencyListView.Columns)
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
                LetterFrequencyListView.Columns[0].Text = LetterFrequencyListView.Columns[0].Text.Replace("  ", " ▲");
                LetterFrequencyListView.Refresh();
            }
        }
    }
    private void DisplayLetterFrequenciesTotals()
    {
        try
        {
            int count = 0;
            long sum = 0L;
            if (LetterFrequencyListView.SelectedItems.Count > 0)
            {
                count = LetterFrequencyListView.SelectedItems.Count;
                foreach (ListViewItem item in LetterFrequencyListView.SelectedItems)
                {
                    sum += long.Parse(item.SubItems[2].Text);
                }
            }
            else
            {
                count = LetterFrequencyListView.Items.Count;
                foreach (ListViewItem item in LetterFrequencyListView.Items)
                {
                    sum += long.Parse(item.SubItems[2].Text);
                }
            }

            LetterFrequencyCountLabel.Text = count.ToString();
            LetterFrequencyCountLabel.ForeColor = GetNumberTypeColor(count);
            LetterFrequencyCountLabel.Refresh();

            LetterFrequencySumLabel.Text = sum.ToString();
            LetterFrequencySumLabel.ForeColor = GetNumberTypeColor(sum);
            LetterFrequencySumLabel.Refresh();

            LetterFrequencyPrimeFactorsTextBox.Text = Numbers.FactorizeToString(sum);
            LetterFrequencyPrimeFactorsTextBox.BackColor = (Numbers.Compare(sum, m_divisor, ComparisonOperator.MultipleOf, -1)) ? Color.FromArgb(192, 255, 192) : SystemColors.ControlLight;
            LetterFrequencyPrimeFactorsTextBox.Refresh();

            LetterFrequencyDigitSumTextBox.Text = Numbers.DigitSum(LetterFrequencySumLabel.Text).ToString();
            LetterFrequencyDigitSumTextBox.ForeColor = GetNumberTypeColor(LetterFrequencyDigitSumTextBox.Text, m_radix);
            LetterFrequencyDigitSumTextBox.Refresh();

            LetterFrequencyDigitalRootTextBox.Text = Numbers.DigitalRoot(LetterFrequencySumLabel.Text).ToString();
            LetterFrequencyDigitalRootTextBox.ForeColor = GetNumberTypeColor(LetterFrequencyDigitalRootTextBox.Text, m_radix);
            LetterFrequencyDigitalRootTextBox.Refresh();

            int nth_prime_index = -1;
            int nth_additive_prime_index = -1;
            int nth_pure_prime_index = -1;
            if (Numbers.IsPrime(sum))
            {
                nth_prime_index = Numbers.IndexOfPrime(sum);
                nth_additive_prime_index = Numbers.IndexOfAdditivePrime(sum);
                nth_pure_prime_index = Numbers.IndexOfPurePrime(sum);

                LetterFrequencyNthPrimeLabel.Text = "P";
                LetterFrequencyNthPrimeLabel.Refresh();
                LetterFrequencyNthAdditivePrimeLabel.Text = "AP";
                LetterFrequencyNthAdditivePrimeLabel.Refresh();
                LetterFrequencyNthPurePrimeLabel.Text = "PP";
                LetterFrequencyNthPurePrimeLabel.Refresh();

                LetterFrequencyNthPrimeTextBox.BackColor = Color.FromArgb(240, 255, 240);
                LetterFrequencyNthAdditivePrimeTextBox.BackColor = Color.FromArgb(224, 224, 255);
                LetterFrequencyNthPurePrimeTextBox.BackColor = Color.FromArgb(255, 224, 255);

                ToolTip.SetToolTip(LetterFrequencyNthPrimeTextBox, "Prime index");
                ToolTip.SetToolTip(LetterFrequencyNthAdditivePrimeTextBox, "Additive prime index");
                ToolTip.SetToolTip(LetterFrequencyNthPurePrimeTextBox, "Pure prime index");
            }
            else
            {
                nth_prime_index = Numbers.IndexOfComposite(sum) + 1;
                if (nth_prime_index == 0)
                {
                    nth_prime_index = -1;
                }
                nth_additive_prime_index = Numbers.IndexOfAdditiveComposite(sum) + 1;
                if (nth_additive_prime_index == 0)
                {
                    nth_additive_prime_index = -1;
                }
                nth_pure_prime_index = Numbers.IndexOfPureComposite(sum) + 1;
                if (nth_pure_prime_index == 0)
                {
                    nth_pure_prime_index = -1;
                }

                LetterFrequencyNthPrimeLabel.Text = "C";
                LetterFrequencyNthPrimeLabel.Refresh();
                LetterFrequencyNthAdditivePrimeLabel.Text = "AC";
                LetterFrequencyNthAdditivePrimeLabel.Refresh();
                LetterFrequencyNthPurePrimeLabel.Text = "PC";
                LetterFrequencyNthPurePrimeLabel.Refresh();

                LetterFrequencyNthPrimeTextBox.BackColor = Color.FromArgb(208, 208, 208);
                LetterFrequencyNthAdditivePrimeTextBox.BackColor = Color.FromArgb(208, 192, 192);
                LetterFrequencyNthPurePrimeTextBox.BackColor = Color.FromArgb(240, 204, 204);

                ToolTip.SetToolTip(LetterFrequencyNthPrimeTextBox, "Composite index");
                ToolTip.SetToolTip(LetterFrequencyNthAdditivePrimeTextBox, "Additive composite index");
                ToolTip.SetToolTip(LetterFrequencyNthPurePrimeTextBox, "Pure composite index");
            }

            LetterFrequencyNthPrimeTextBox.ForeColor = GetNumberTypeColor(nth_prime_index);
            LetterFrequencyNthPrimeTextBox.Text = nth_prime_index.ToString();
            LetterFrequencyNthPrimeTextBox.Refresh();

            LetterFrequencyNthAdditivePrimeTextBox.ForeColor = GetNumberTypeColor(nth_additive_prime_index);
            LetterFrequencyNthAdditivePrimeTextBox.Text = nth_additive_prime_index.ToString();
            LetterFrequencyNthAdditivePrimeTextBox.Refresh();

            LetterFrequencyNthPurePrimeTextBox.ForeColor = GetNumberTypeColor(nth_pure_prime_index);
            LetterFrequencyNthPurePrimeTextBox.Text = nth_pure_prime_index.ToString();
            LetterFrequencyNthPurePrimeTextBox.Refresh();
        }
        catch
        {
            // log exception
        }
    }

    private void LetterFrequencyInspectLabel_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            InspectLetterStatistics();
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void InspectLetterStatistics()
    {
        if (m_client != null)
        {
            string text = m_current_text;
            if (!String.IsNullOrEmpty(text))
            {
                if (!String.IsNullOrEmpty(m_current_phrase))
                {
                    string filename = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + "_" + ".txt";
                    if (m_client.NumerologySystem != null)
                    {
                        filename = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + "_" + m_client.NumerologySystem.Name + ".txt";
                    }

                    m_client.SaveLetterStatistics(filename, text, m_current_phrase);
                }
                else
                {
                    string filename = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + "_" + ".txt";
                    if (m_client.NumerologySystem != null)
                    {
                        filename = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + "_" + m_client.NumerologySystem.Name + ".txt";
                    }

                    m_client.SaveLetterStatistics(filename, text);
                }
            }
        }
    }
    private void InspectValueCalculationsLabel_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            InspectValueCalculations();
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void InspectValueCalculations()
    {
        if (m_client != null)
        {
            if (m_client.NumerologySystem != null)
            {
                string filename = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + "_" + m_client.NumerologySystem.Name + ".txt";

                StringBuilder str = new StringBuilder();
                str.AppendLine(m_current_text);
                str.AppendLine("----------------------------------------");
                str.AppendLine("Verses\t=\t" + VersesTextBox.Text);
                str.AppendLine("Words\t=\t" + WordsTextBox.Text);
                str.AppendLine("Letters\t=\t" + LettersTextBox.Text);
                str.AppendLine("Value\t=\t" + ValueTextBox.Text + ((m_radix == DEFAULT_RADIX) ? "" : " in base " + m_radix.ToString()));
                str.AppendLine("----------------------------------------");

                m_client.SaveValueCalculations(filename, str.ToString());
            }
        }
    }

    private void StatisticsControls_Enter(object sender, EventArgs e)
    {
        SearchGroupBox_Leave(null, null);
        this.AcceptButton = null;
    }
    private void StatisticsControls_Click(object sender, EventArgs e)
    {
        // Ctrl+Click factorizes number
        if (ModifierKeys == Keys.Control)
        {
            if (sender is Label)
            {
                FactorizeNumber(sender as Label);
            }
            else if (sender is TextBox)
            {
                FactorizeNumber(sender as TextBox);
            }
        }
    }
    private void FactorizeNumber(Label control)
    {
        if (control != null)
        {
            long value = 0L;
            try
            {
                string text = control.Text;
                if (!String.IsNullOrEmpty(text))
                {
                    value = long.Parse(text);
                }
            }
            catch
            {
                value = -1L; // error
            }
            FactorizeValue(value, "Number", false);
        }
    }
    private void FactorizeNumber(TextBox control)
    {
        if (control != null)
        {
            long value = 0L;
            try
            {
                string text = control.Text;
                if (!String.IsNullOrEmpty(text))
                {
                    if (
                        (control != ValueTextBox)
                        &&
                        (control != PrimeFactorsTextBox)
                       )
                    {
                        if (control.Name.StartsWith("LetterFrequency"))
                        {
                            value = long.Parse(text);
                        }
                        else if (control.Name.StartsWith("Decimal"))
                        {
                            value = Radix.Decode(text, 10L);
                        }
                        else if (text.StartsWith(SUM_SYMBOL))
                        {
                            text = text.Substring(SUM_SYMBOL.Length, text.Length - SUM_SYMBOL.Length);
                            value = Radix.Decode(text, 10L);
                        }
                        else
                        {
                            value = Radix.Decode(text, m_radix);
                        }
                    }
                }
            }
            catch
            {
                value = -1L; // error
            }
            FactorizeValue(value, "Number", false);
        }
    }
    private void StatusControls_Enter(object sender, EventArgs e)
    {
        SearchGroupBox_Leave(null, null);
        this.AcceptButton = null;
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region 31. Help
    ///////////////////////////////////////////////////////////////////////////////
    private int m_help_message_index = 0;
    private void HelpMessageLabel_Click(object sender, EventArgs e)
    {
        if (m_client != null)
        {
            if (m_client.HelpMessages != null)
            {
                int maximum = m_client.HelpMessages.Count - 1;
                if (ModifierKeys == Keys.Shift)
                {
                    m_help_message_index--;
                    if (m_help_message_index < 0) m_help_message_index = maximum;
                }
                else
                {
                    m_help_message_index++;
                    if (m_help_message_index > maximum) m_help_message_index = 0;
                }

                if (m_client.HelpMessages.Count > m_help_message_index)
                {
                    HelpMessageLabel.Text = m_client.HelpMessages[m_help_message_index];
                }
            }
        }
    }
    private void LinkLabel_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            Control control = (sender as Control);
            if (control != null)
            {
                if (control.Tag != null)
                {
                    if (!String.IsNullOrEmpty(control.Tag.ToString()))
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(control.Tag.ToString());
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
                    }
                }
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void PrimalogyLabel_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            string filename = "Primalogy.pdf";
            string path = Application.StartupPath + "/" + Globals.HELP_FOLDER + "/" + filename;
            if (!File.Exists(path))
            {
                DownloadFile("http://heliwave.com/" + filename, path);
            }
            if (File.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
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
    private void PrimalogyARLabel_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            string filename = "Primalogy_AR.pdf";
            string path = Application.StartupPath + "/" + Globals.HELP_FOLDER + "/" + filename;
            if (!File.Exists(path))
            {
                DownloadFile("http://heliwave.com/" + filename, path);
            }
            if (File.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
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
    private void EncryptedQuranLinkLabel_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            string filename = "EncryptedQuran.pdf";
            string path = Application.StartupPath + "/" + Globals.HELP_FOLDER + "/" + filename;
            if (!File.Exists(path))
            {
                DownloadFile("http://heliwave.com/" + filename, path);
            }
            if (File.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
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
    private void VersionLabel_Click(object sender, EventArgs e)
    {
        if (m_about_box != null)
        {
            m_about_box.ShowDialog();
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
}
