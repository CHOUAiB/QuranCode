using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.IO;

public partial class MainForm : Form
{
    private Factorizer m_factorizer = null;
    private Thread m_worker_thread = null;

    private void FixMicrosoft(object sender, KeyPressEventArgs e)
    {
        // stop annoying beep due to parent not having an AcceptButton
        if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Escape))
        {
            e.Handled = true;
        }
        // enable Ctrl+A to SelectAll in TextBox and RichTextBox
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

    private static Color[] NUMBER_TYPE_COLORS =
    { 
        /* NumberType.None */                   Color.Black,
        /* NumberType.Unit */                   Color.DarkViolet,
        /* NumberType.Prime */                  Color.Green,
        /* NumberType.AdditivePrime */          Color.Blue,
        /* NumberType.NonAdditivePrime */       Color.Green,
        /* NumberType.Composite */              Color.Black,
        /* NumberType.AdditiveComposite */      Color.Brown,
        /* NumberType.NonAdditiveComposite */   Color.Black,
        /* NumberType.Odd */                    Color.Black,
        /* NumberType.Even */                   Color.Black,
        /* NumberType.Square */                 Color.Black,
        /* NumberType.Cubic */                  Color.Black,
        /* NumberType.Quartic */                Color.Black,
        /* NumberType.Quintic */                Color.Black,
        /* NumberType.Sextic */                 Color.Black,
        /* NumberType.Septic */                 Color.Black,
        /* NumberType.Octic */                  Color.Black,
        /* NumberType.Nonic */                  Color.Black,
        /* NumberType.Decic */                  Color.Black,
        /* NumberType.Natural */                Color.Black
    };
    private static Color[] NUMBER_TYPE_BACKCOLORS =
    { 
        /* NumberType.None */                   Color.Black,
        /* NumberType.Unit */                   Color.DarkViolet,
        /* NumberType.Prime */                  Color.Green,
        /* NumberType.AdditivePrime */          Color.FromArgb(224, 224, 255),
        /* NumberType.NonAdditivePrime */       Color.FromArgb(240, 255, 240),
        /* NumberType.Composite */              Color.Black,
        /* NumberType.AdditiveComposite */      Color.FromArgb(224, 192, 192),
        /* NumberType.NonAdditiveComposite */   Color.FromArgb(208, 208, 208),
        /* NumberType.Odd */                    Color.Black,
        /* NumberType.Even */                   Color.Black,
        /* NumberType.Square */                 Color.Black,
        /* NumberType.Cubic */                  Color.Black,
        /* NumberType.Quartic */                Color.Black,
        /* NumberType.Quintic */                Color.Black,
        /* NumberType.Sextic */                 Color.Black,
        /* NumberType.Septic */                 Color.Black,
        /* NumberType.Octic */                  Color.Black,
        /* NumberType.Nonic */                  Color.Black,
        /* NumberType.Decic */                  Color.Black,
        /* NumberType.Natural */                Color.Black
    };

    private static Color[] NUMBER_KIND_COLORS =
    { 
        /* NumberKind.Deficient */          Color.FromArgb(255, 240, 240),
        /* NumberKind.Perfect */            Color.FromArgb(255, 204, 204),
        /* NumberKind.Abundant */           Color.FromArgb(255, 224, 224)
    };

    public const int DEFAULT_SLEEP_TIME = 40; // ms
    private int m_sleep_time = DEFAULT_SLEEP_TIME;
    public int SleepTime
    {
        get { return m_sleep_time; }
        set { m_sleep_time = value; }
    }

    public MainForm()
    {
        InitializeComponent();

        AboutToolStripMenuItem.Font = new Font(AboutToolStripMenuItem.Font, AboutToolStripMenuItem.Font.Style | FontStyle.Bold);

        m_filename = AppDomain.CurrentDomain.FriendlyName.Replace(".exe", ".ini");
        LoadSettings();
    }

    private string m_filename = null;
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
                                case "Count":
                                    {
                                        string[] sub_parts = parts[1].Split('\t');
                                        if (sub_parts.Length == 2)
                                        {
                                            int count = int.Parse(sub_parts[0]);
                                            string[] sub_sub_parts = sub_parts[1].Split(',');
                                            if (sub_sub_parts.Length == count)
                                            {
                                                foreach (string item in sub_sub_parts)
                                                {
                                                    m_history.Add(item);
                                                    m_history_index++;
                                                }
                                                ValueTextBox.Text = m_history[m_history_index];
                                            }
                                        }
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

        HistoryDeleteLabel.Enabled = (m_history.Count > 0);
        HistoryClearLabel.Enabled = (m_history.Count > 0);
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

                writer.WriteLine("[History]");
                StringBuilder str = new StringBuilder("Count=" + this.m_history.Count + "\t");
                foreach (string item in m_history)
                {
                    str.Append(item + ",");
                }
                str.Remove(str.Length - 1, 1);
                writer.WriteLine(str.ToString());
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
        m_factorizer = null;
        m_worker_thread = null;

        string version = typeof(MainForm).Assembly.GetName().Version.ToString();
        int pos = version.LastIndexOf(".");
        if (pos > -1)
        {
            VersionLabel.Text = "v" + version.Substring(0, pos);
        }

        if (this.Top < 0)
        {
            RestoreLocation();
        }
    }
    private void MainForm_Shown(object sender, EventArgs e)
    {
        NotifyIcon.Visible = true;

        EnableEntryControls();
        SetupToolTips();
    }
    private void SetupToolTips()
    {
        this.ToolTip.SetToolTip(this.PCIndexChainL2RTextBox, "Prime/composite index chain --> P=0 C=1");
        this.ToolTip.SetToolTip(this.PCIndexChainR2LTextBox, "Prime/composite index chain <-- P=0 C=1");
        this.ToolTip.SetToolTip(this.CPIndexChainL2RTextBox, "Prime/composite index chain --> P=1 C=0");
        this.ToolTip.SetToolTip(this.CPIndexChainR2LTextBox, "Prime/composite index chain <-- P=1 C=0");
        this.ToolTip.SetToolTip(this.IndexChainLengthTextBox, "Prime/composite index chain length");
        this.ToolTip.SetToolTip(this.DigitSumTextBox, "Digit sum");
        this.ToolTip.SetToolTip(this.DigitalRootTextBox, "Digital root");
        this.ToolTip.SetToolTip(this.NthNumberTextBox, "Prime index");
        this.ToolTip.SetToolTip(this.NthAdditiveNumberTextBox, "Additive prime index");
        this.ToolTip.SetToolTip(this.NthNonAdditiveNumberTextBox, "Non-additive prime index");
    }
    private void MainForm_Resize(object sender, EventArgs e)
    {
        ValueTextBox.SelectionStart = 0;
    }
    private void MainForm_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            if ((m_worker_thread != null) && (m_worker_thread.IsAlive))
            {
                if (MessageBox.Show("Stop and lose all progress?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    Cancel();
                }
            }
            else
            {
                ValueTextBox.Text = "";
                ValueTextBox.Focus();
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
        Cancel();
    }
    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        CloseApplication();
    }
    private void CloseApplication()
    {
        if (m_worker_thread != null)
        {
            m_worker_thread.Join(); // wait for workerThread to terminate
            m_worker_thread = null;
        }

        // remove icon from tray
        if (NotifyIcon != null)
        {
            NotifyIcon.Visible = false;
            NotifyIcon.Dispose();
        }

        SaveSettings();
    }

    private void ValueTextBox_TextChanged(object sender, EventArgs e)
    {
        int digits = Numbers.DigitCount(ValueTextBox.Text);
        int length = ValueTextBox.Text.Replace(" ", "").Length;
        if (digits == length)
        {
            HistoryBackwardLabel.Enabled = (digits > 0);
            HistoryForewardLabel.Enabled = (digits > 0);
        }
        DigitsLabel.Text = (digits == 0) ? "digits" : digits.ToString();

        int digit_sum = Numbers.DigitSum(ValueTextBox.Text);
        DigitSumTextBox.Text = digit_sum.ToString();
        DigitSumTextBox.ForeColor = GetNumberTypeColor(digit_sum);
        DigitSumTextBox.Refresh();

        int digital_root = Numbers.DigitalRoot(ValueTextBox.Text);
        DigitalRootTextBox.Text = digital_root.ToString();
        DigitalRootTextBox.ForeColor = GetNumberTypeColor(digital_root);
        DigitalRootTextBox.Refresh();

        ClearFactors();
        ClearProgress();
        ClearNumberAnalyses();

        this.ToolTip.SetToolTip(this.ValueTextBox, null);
    }
    private void ValueTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (ModifierKeys == (Keys.Shift | Keys.Control))
        {
            if (e.KeyCode == Keys.Delete)
            {
                HistoryClearLabel_Click(null, null);
            }
        }
        else if (ModifierKeys == Keys.Control)
        {
            if (e.KeyCode == Keys.A)
            {
                if (sender is TextBoxBase)
                {
                    (sender as TextBoxBase).SelectAll();
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                HistoryBackwardLabel_Click(null, null);
            }
            else if (e.KeyCode == Keys.Right)
            {
                HistoryForewardLabel_Click(null, null);
            }
            else if (e.KeyCode == Keys.Up)
            {
                NextPrimeNumber();
            }
            else if (e.KeyCode == Keys.Down)
            {
                PreviousPrimeNumber();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                HistoryDeleteLabel_Click(null, null);
            }
        }
        else if (e.KeyCode == Keys.Enter)
        {
            CallRun();
        }
        else
        {
            ValueTextBox.ForeColor = Color.DarkGray;
        }
    }
    private void NextPrimeNumber()
    {
        // guard against multiple runs on multiple ENTER keys
        if ((m_worker_thread != null) && (m_worker_thread.IsAlive)) return;

        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (ValueTextBox.Text.Length == 0) ValueTextBox.Text = "1";
            string number = ValueTextBox.Text;

            BeforeProcessing();

            try
            {
                m_factorizer = new Factorizer(this, "nextprime", number, m_multithreading);
                if (m_factorizer != null)
                {
                    m_worker_thread = new Thread(new ThreadStart(m_factorizer.Run));
                    m_worker_thread.Priority = ThreadPriority.Highest;
                    m_worker_thread.IsBackground = false;
                    m_worker_thread.Start();
                }
            }
            catch
            {
                Cancel();
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void PreviousPrimeNumber()
    {
        // guard against multiple runs on multiple ENTER keys
        if ((m_worker_thread != null) && (m_worker_thread.IsAlive)) return;

        this.Cursor = Cursors.WaitCursor;
        try
        {
            string number = ValueTextBox.Text + ",0"; // previousprime = nextprime(n,0), thanks to Benjamin Buhrow (bbuhrow@gmail.com) for telling me that.

            BeforeProcessing();

            try
            {
                m_factorizer = new Factorizer(this, "nextprime", number, m_multithreading);
                if (m_factorizer != null)
                {
                    m_worker_thread = new Thread(new ThreadStart(m_factorizer.Run));
                    m_worker_thread.Priority = ThreadPriority.Highest;
                    m_worker_thread.IsBackground = false;
                    m_worker_thread.Start();
                }
            }
            catch
            {
                Cancel();
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }

    private List<string> m_history = new List<string>();
    private int m_history_index = -1;
    private void HistoryForewardLabel_Click(object sender, EventArgs e)
    {
        if ((m_history_index >= 0) && (m_history_index < m_history.Count - 1))
        {
            m_history_index++;
            ValueTextBox.Text = m_history[m_history_index];
        }
    }
    private void HistoryBackwardLabel_Click(object sender, EventArgs e)
    {
        if ((m_history_index > 0) && (m_history_index < m_history.Count))
        {
            m_history_index--;
            ValueTextBox.Text = m_history[m_history_index];
        }
    }
    private void HistoryDeleteLabel_Click(object sender, EventArgs e)
    {
        if ((m_history_index >= 0) && (m_history_index < m_history.Count))
        {
            m_history.RemoveAt(m_history_index);

            if ((m_history_index >= m_history.Count))
            {
                m_history_index--;
            }
            if ((m_history_index >= 0) && (m_history_index < m_history.Count))
            {
                ValueTextBox.Text = m_history[m_history_index];
            }
            else
            {
                ValueTextBox.Text = "";
            }
        }
        else
        {
            if (m_history.Count > 0)
            {
                ValueTextBox.Text = m_history[m_history.Count - 1];
            }
            else
            {
                ValueTextBox.Text = "";
            }

            if ((m_history_index >= m_history.Count))
            {
                m_history_index--;
            }
        }
        ValueTextBox.Refresh();

        HistoryDeleteLabel.Enabled = (m_history.Count > 0);
        HistoryClearLabel.Enabled = (m_history.Count > 0);
    }
    private void HistoryClearLabel_Click(object sender, EventArgs e)
    {
        m_history.Clear();
        m_history_index = -1;
        ValueTextBox.Text = "";
        ValueTextBox.Refresh();

        HistoryDeleteLabel.Enabled = false;
        HistoryClearLabel.Enabled = false;
    }

    private double m_double_value = 0.0D;
    private double CalculateValue(string expression)
    {
        double result = 0D;
        try
        {
            result = Radix.Decode(expression, 10L);
            this.ToolTip.SetToolTip(this.ValueTextBox, result.ToString());
        }
        catch // if expression
        {
            string text = CalculateExpression(expression);
            this.ToolTip.SetToolTip(this.ValueTextBox, text); // display the decimal expansion

            try
            {
                result = double.Parse(text);
            }
            catch
            {
                result = 0L;
            }
        }
        return result;
    }
    private string CalculateExpression(string expression)
    {
        try
        {
            return Evaluator.Evaluate(expression, 10L);
        }
        catch
        {
            return expression;
        }
    }
    private void FactorizeValue(long number)
    {
        ValueTextBox.Text = Radix.Encode(number, 10L);
        ValueTextBox.ForeColor = GetNumberTypeColor(number);
        ValueTextBox.SelectionStart = ValueTextBox.Text.Length;
        ValueTextBox.SelectionLength = 0;
        ValueTextBox.Refresh();

        //string factors_str = Numbers.FactorizeToString(number);
        //PrimeFactorsTextBox.Text = factors_str;
        //PrimeFactorsTextBox.BackColor = (Numbers.Compare(number, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.ControlLight;
        //PrimeFactorsTextBox.Refresh();
        CallRun();
    }
    private void AnalyzeValue(long number)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            ValueTextBox.ForeColor = GetNumberTypeColor(number);
            ValueTextBox.Refresh();

            DigitSumTextBox.Text = Numbers.DigitSum(number).ToString();
            DigitSumTextBox.ForeColor = GetNumberTypeColor(Numbers.DigitSum(number));
            DigitSumTextBox.Refresh();

            DigitalRootTextBox.Text = Numbers.DigitalRoot(number).ToString();
            DigitalRootTextBox.ForeColor = GetNumberTypeColor(Numbers.DigitalRoot(number));
            DigitalRootTextBox.Refresh();

            PCIndexChainL2RTextBox.Text = DecimalPCIndexChainL2R(number).ToString();
            PCIndexChainL2RTextBox.ForeColor = GetNumberTypeColor(DecimalPCIndexChainL2R(number));
            PCIndexChainL2RTextBox.Refresh();

            PCIndexChainR2LTextBox.Text = DecimalPCIndexChainR2L(number).ToString();
            PCIndexChainR2LTextBox.ForeColor = GetNumberTypeColor(DecimalPCIndexChainR2L(number));
            PCIndexChainR2LTextBox.Refresh();

            CPIndexChainL2RTextBox.Text = DecimalCPIndexChainL2R(number).ToString();
            CPIndexChainL2RTextBox.ForeColor = GetNumberTypeColor(DecimalCPIndexChainL2R(number));
            CPIndexChainL2RTextBox.Refresh();

            CPIndexChainR2LTextBox.Text = DecimalCPIndexChainR2L(number).ToString();
            CPIndexChainR2LTextBox.ForeColor = GetNumberTypeColor(DecimalCPIndexChainR2L(number));
            CPIndexChainR2LTextBox.Refresh();

            IndexChainLengthTextBox.Text = IndexChainLength(number).ToString();
            //IndexChainLengthTextBox.ForeColor = GetNumberTypeColor(IndexChainLength(number));
            IndexChainLengthTextBox.Refresh();

            string squares1_str = "";
            string squares2_str = "";
            if (Numbers.IsUnit(number) || Numbers.IsPrime(number))
            {
                squares1_str = Numbers.Get4nPlus1EqualsSumOfTwoSquares(number);
                squares2_str = Numbers.Get4nPlus1EqualsDiffOfTwoSquares(number);
            }
            else //if composite
            {
                squares1_str = Numbers.Get4nPlus1EqualsDiffOfTwoSquares(number);
                squares2_str = Numbers.Get4nPlus1EqualsDiffOfTwoSquares2(number);
            }
            SquareSumTextBox.Text = squares1_str;
            SquareSumTextBox.Refresh();
            SquareDiffTextBox.Text = squares2_str;
            SquareDiffTextBox.Refresh();

            UpdateNumberKind(number);
            UpdateSumOfDivisors(number);

            if (long.TryParse(m_factorizer.Number, out number))
            {
                if (number <= 0L)
                {
                    m_index_type = IndexType.Prime;
                }
                else if (number == 1L)
                {
                    m_index_type = IndexType.Prime;
                }
                else
                {
                    if (m_factorizer.IsPrime)
                    {
                        m_index_type = IndexType.Prime;
                        int prime_index = Numbers.PrimeIndexOf(number);
                        int additive_prime_index = Numbers.AdditivePrimeIndexOf(number);
                        int non_additive_prime_index = Numbers.NonAdditivePrimeIndexOf(number);
                        NthNumberTextBox.ForeColor = GetNumberTypeColor(prime_index);
                        NthAdditiveNumberTextBox.ForeColor = GetNumberTypeColor(additive_prime_index);
                        NthNonAdditiveNumberTextBox.ForeColor = GetNumberTypeColor(non_additive_prime_index);
                        NthNumberTextBox.BackColor = (non_additive_prime_index == -1) ? NUMBER_TYPE_BACKCOLORS[(int)NumberType.AdditivePrime] : NUMBER_TYPE_BACKCOLORS[(int)NumberType.NonAdditivePrime];
                        NthAdditiveNumberTextBox.BackColor = NUMBER_TYPE_BACKCOLORS[(int)NumberType.AdditivePrime];
                        NthNonAdditiveNumberTextBox.BackColor = NUMBER_TYPE_BACKCOLORS[(int)NumberType.NonAdditivePrime];
                        NthNumberTextBox.Text = prime_index.ToString();
                        NthAdditiveNumberTextBox.Text = additive_prime_index.ToString();
                        NthNonAdditiveNumberTextBox.Text = non_additive_prime_index.ToString();
                        ToolTip.SetToolTip(NthNumberTextBox, "Prime index");
                        ToolTip.SetToolTip(NthAdditiveNumberTextBox, "Additive prime index");
                        ToolTip.SetToolTip(NthNonAdditiveNumberTextBox, "Non-additive prime index");
                    }
                    else // Composite
                    {
                        m_index_type = IndexType.Composite;
                        int composite_index = Numbers.CompositeIndexOf(number);
                        int additive_composite_index = Numbers.AdditiveCompositeIndexOf(number);
                        int non_additive_composite_index = Numbers.NonAdditiveCompositeIndexOf(number);
                        NthNumberTextBox.ForeColor = GetNumberTypeColor(composite_index);
                        NthAdditiveNumberTextBox.ForeColor = GetNumberTypeColor(additive_composite_index);
                        NthNonAdditiveNumberTextBox.ForeColor = GetNumberTypeColor(non_additive_composite_index);
                        NthNumberTextBox.BackColor = (non_additive_composite_index == -1) ? NUMBER_TYPE_BACKCOLORS[(int)NumberType.AdditiveComposite] : NUMBER_TYPE_BACKCOLORS[(int)NumberType.NonAdditiveComposite];
                        NthAdditiveNumberTextBox.BackColor = NUMBER_TYPE_BACKCOLORS[(int)NumberType.AdditiveComposite];
                        NthNonAdditiveNumberTextBox.BackColor = NUMBER_TYPE_BACKCOLORS[(int)NumberType.NonAdditiveComposite];
                        NthNumberTextBox.Text = composite_index.ToString();
                        NthAdditiveNumberTextBox.Text = additive_composite_index.ToString();
                        NthNonAdditiveNumberTextBox.Text = non_additive_composite_index.ToString();
                        ToolTip.SetToolTip(NthNumberTextBox, "Composite index");
                        ToolTip.SetToolTip(NthAdditiveNumberTextBox, "Additive composite index");
                        ToolTip.SetToolTip(NthNonAdditiveNumberTextBox, "Non-additive composite index");
                    }
                }
            }
            else  // BigIntegr so Yafu tool is used
            {
                if (m_factorizer.IsPrime)
                {
                    m_index_type = IndexType.Prime;

                    ValueTextBox.ForeColor = GetNumberTypeColor(19L);
                    if (Numbers.IsPrime(Numbers.DigitSum(m_factorizer.Number)))
                    {
                        ValueTextBox.ForeColor = GetNumberTypeColor(29L);
                    }
                    ValueTextBox.Refresh();

                    NthNumberTextBox.ForeColor = GetNumberTypeColor(-1L);
                    NthAdditiveNumberTextBox.ForeColor = GetNumberTypeColor(-1L);
                    NthNonAdditiveNumberTextBox.ForeColor = GetNumberTypeColor(-1L);
                    NthNumberTextBox.Text = "-1";
                    NthAdditiveNumberTextBox.Text = "-1";
                    NthNonAdditiveNumberTextBox.Text = "-1";
                    ToolTip.SetToolTip(NthNumberTextBox, "Prime index");
                    ToolTip.SetToolTip(NthAdditiveNumberTextBox, "Additive prime index");
                    ToolTip.SetToolTip(NthNonAdditiveNumberTextBox, "Non-additive prime index");
                }
                else // Composite BigInteger
                {
                    m_index_type = IndexType.Composite;

                    ValueTextBox.ForeColor = GetNumberTypeColor(21L);
                    if (Numbers.IsComposite(Numbers.DigitSum(m_factorizer.Number)))
                    {
                        ValueTextBox.ForeColor = GetNumberTypeColor(15L);

                        if (Numbers.IsCompositeDigits(m_factorizer.Number))
                        {
                            ValueTextBox.ForeColor = GetNumberTypeColor(4L);
                        }
                    }
                    ValueTextBox.Refresh();

                    NthNumberTextBox.ForeColor = GetNumberTypeColor(-1L);
                    NthAdditiveNumberTextBox.ForeColor = GetNumberTypeColor(-1L);
                    NthNonAdditiveNumberTextBox.ForeColor = GetNumberTypeColor(-1L);
                    NthNumberTextBox.Text = "-1";
                    NthAdditiveNumberTextBox.Text = "-1";
                    NthNonAdditiveNumberTextBox.Text = "-1";
                    ToolTip.SetToolTip(NthNumberTextBox, "Composite index");
                    ToolTip.SetToolTip(NthAdditiveNumberTextBox, "Additive composite index");
                    ToolTip.SetToolTip(NthNonAdditiveNumberTextBox, "Non-additive composite index");
                }
            }

            NthNumberTextBox.Refresh();
            NthAdditiveNumberTextBox.Refresh();
            NthNonAdditiveNumberTextBox.Refresh();
        }
        catch //(Exception ex)
        {
            //MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void UpdateNumberKind(long number)
    {
        m_number_kind = Numbers.GetNumberKind(number);
        int number_kind_index = -1;
        switch (m_number_kind)
        {
            case NumberKind.Deficient:
                {
                    number_kind_index = Numbers.DeficientIndexOf(number) + 1;
                    NumberKindIndexTextBox.BackColor = NUMBER_KIND_COLORS[0];
                }
                break;
            case NumberKind.Perfect:
                {
                    number_kind_index = Numbers.PerfectIndexOf(number) + 1;
                    NumberKindIndexTextBox.BackColor = NUMBER_KIND_COLORS[1];
                }
                break;
            case NumberKind.Abundant:
                {
                    number_kind_index = Numbers.AbundantIndexOf(number) + 1;
                    NumberKindIndexTextBox.BackColor = NUMBER_KIND_COLORS[2];
                }
                break;
            default:
                {
                    number_kind_index = -1;
                    NumberKindIndexTextBox.BackColor = SystemColors.Control;
                }
                break;
        }
        NumberKindIndexTextBox.Text = number_kind_index.ToString();
        NumberKindIndexTextBox.ForeColor = GetNumberTypeColor(number_kind_index);
        ToolTip.SetToolTip(NumberKindIndexTextBox, m_number_kind.ToString() + " number index");
        NumberKindIndexTextBox.Refresh();
    }
    private void DisplayDeficientNumbersLable_Click(object sender, EventArgs e)
    {
        if (Directory.Exists(Globals.NUMBERS_FOLDER))
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string filename = Globals.NUMBERS_FOLDER + "/" + "DeficientNumbers" + ".txt";
                using (StreamWriter writer = new StreamWriter(filename, false))
                {
                    StringBuilder str = new StringBuilder();

                    str.AppendLine("----------------------------------------");
                    str.AppendLine("Deficient numbers are those with the sum of their proper divisors less than themselves.");
                    str.AppendLine("----------------------------------------");
                    str.AppendLine("#" + "\t" + "Number");
                    str.AppendLine("----------------------------------------");

                    for (int i = 0; i < Numbers.Deficients.Count; i++)
                    {
                        str.AppendLine((i + 1).ToString() + "\t" + Numbers.Deficients[i]);
                    }
                    str.AppendLine("----------------------------------------");

                    writer.WriteLine(str.ToString());
                }

                // show file content after save
                if (File.Exists(filename))
                {
                    FileHelper.WaitForReady(filename);

                    System.Diagnostics.Process.Start("Notepad.exe", filename);
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
    private void DisplayPerfectNumbersLable_Click(object sender, EventArgs e)
    {
        if (Directory.Exists(Globals.NUMBERS_FOLDER))
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string filename = Globals.NUMBERS_FOLDER + "/" + "PerfectNumbers" + ".txt";
                if (File.Exists(filename))
                {
                    FileHelper.WaitForReady(filename);

                    System.Diagnostics.Process.Start("Notepad.exe", filename);
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
    private void DisplayAbundantNumbersLable_Click(object sender, EventArgs e)
    {
        if (Directory.Exists(Globals.NUMBERS_FOLDER))
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string filename = Globals.NUMBERS_FOLDER + "/" + "AbundantNumbers" + ".txt";
                using (StreamWriter writer = new StreamWriter(filename, false))
                {
                    StringBuilder str = new StringBuilder();

                    str.AppendLine("----------------------------------------");
                    str.AppendLine("Abundant numbers are those with the sum of their proper divisors greater than themselves.");
                    str.AppendLine("----------------------------------------");
                    str.AppendLine("#" + "\t" + "Number");
                    str.AppendLine("----------------------------------------");

                    for (int i = 0; i < Numbers.Abundants.Count; i++)
                    {
                        str.AppendLine((i + 1).ToString() + "\t" + Numbers.Abundants[i]);
                    }
                    str.AppendLine("----------------------------------------");

                    writer.WriteLine(str.ToString());
                }

                // show file content after save
                if (File.Exists(filename))
                {
                    FileHelper.WaitForReady(filename);

                    System.Diagnostics.Process.Start("Notepad.exe", filename);
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
    private void UpdateSumOfDivisors(long number)
    {
        long sum_of_divisors = Numbers.SumOfDivisors(number);
        string divisors = Numbers.GetDivisorsString(number);
        SumOfDivisorsTextBox.Text = sum_of_divisors.ToString();
        SumOfDivisorsTextBox.ForeColor = GetNumberTypeColor(sum_of_divisors);
        SumOfDivisorsTextBox.Refresh();
        ToolTip.SetToolTip(SumOfDivisorsTextBox, "Sum Of Divisors\r\n" + divisors + " = " + sum_of_divisors);

        long sum_of_proper_divisors = Numbers.SumOfProperDivisors(number);
        string proper_divisors = Numbers.GetProperDivisorsString(number);
        SumOfProperDivisorsTextBox.Text = sum_of_proper_divisors.ToString();
        SumOfProperDivisorsTextBox.ForeColor = GetNumberTypeColor(sum_of_proper_divisors);
        SumOfProperDivisorsTextBox.Refresh();
        ToolTip.SetToolTip(SumOfProperDivisorsTextBox, "Sum Of Proper Divisors\r\n" + proper_divisors + " = " + sum_of_proper_divisors);
    }

    private void ValueInspectLabel_Click(object sender, EventArgs e)
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
        long number = 0;
        StringBuilder str = new StringBuilder();
        if (long.TryParse(ValueTextBox.Text, out number))
        {
            str.AppendLine("Number\t\t=\t" + ValueTextBox.Text);

            str.AppendLine();
            str.AppendLine("Prime Factors\t=\t" + Numbers.FactorizeToString(number));
            int nth_number_index = -1;
            int nth_additive_number_index = -1;
            int nth_non_additive_number_index = -1;
            if (Numbers.IsPrime(number))
            {
                m_index_type = IndexType.Prime;
                nth_number_index = Numbers.PrimeIndexOf(number);
                nth_additive_number_index = Numbers.AdditivePrimeIndexOf(number);
                nth_non_additive_number_index = Numbers.NonAdditivePrimeIndexOf(number);
                str.AppendLine("P  Index\t=\t" + nth_number_index);
                str.AppendLine("AP Index\t=\t" + nth_additive_number_index);
                str.AppendLine("XP Index\t=\t" + nth_non_additive_number_index);
            }
            else // any other index type will be treated as IndexNumberType.Composite
            {
                m_index_type = IndexType.Composite;
                nth_number_index = Numbers.CompositeIndexOf(number);
                nth_additive_number_index = Numbers.AdditiveCompositeIndexOf(number);
                nth_non_additive_number_index = Numbers.NonAdditiveCompositeIndexOf(number);
                str.AppendLine("C  Index\t=\t" + nth_number_index);
                str.AppendLine("AC Index\t=\t" + nth_additive_number_index);
                str.AppendLine("XC Index\t=\t" + nth_non_additive_number_index);
            }

            str.AppendLine();
            string divisors = Numbers.GetDivisorsString(number);
            long sum_of_divisors = Numbers.SumOfDivisors(number);
            str.AppendLine("Sum Of Divisors\t\t=\t" + sum_of_divisors + " = " + divisors);

            string proper_divisors = Numbers.GetDivisorsString(number);
            long sum_of_proper_divisors = Numbers.SumOfProperDivisors(number);
            str.AppendLine("Sum Of Proper Divisors\t=\t" + sum_of_proper_divisors + " = " + proper_divisors);

            m_number_kind = Numbers.GetNumberKind(number);
            int number_kind_index = -1;
            switch (m_number_kind)
            {
                case NumberKind.Deficient:
                    {
                        number_kind_index = Numbers.DeficientIndexOf(number) + 1;
                    }
                    break;
                case NumberKind.Perfect:
                    {
                        number_kind_index = Numbers.PerfectIndexOf(number) + 1;
                    }
                    break;
                case NumberKind.Abundant:
                    {
                        number_kind_index = Numbers.AbundantIndexOf(number) + 1;
                    }
                    break;
                default:
                    {
                        number_kind_index = -1;
                    }
                    break;
            }
            str.AppendLine(m_number_kind.ToString() + " Index\t\t=\t" + number_kind_index);

            str.AppendLine();
            string squares1_str = "";
            string squares2_str = "";
            if (Numbers.IsUnit(number) || Numbers.IsPrime(number))
            {
                squares1_str = Numbers.Get4nPlus1EqualsSumOfTwoSquares(number);
                squares2_str = Numbers.Get4nPlus1EqualsDiffOfTwoSquares(number);
            }
            else //if composite
            {
                squares1_str = Numbers.Get4nPlus1EqualsDiffOfTwoSquares(number);
                squares2_str = Numbers.Get4nPlus1EqualsDiffOfTwoSquares2(number);
            }
            str.AppendLine("4n+1 Squares1\t\t=\t" + squares1_str);
            str.AppendLine("4n+1 Squares2\t\t=\t" + squares2_str);

            str.AppendLine();
            str.AppendLine("Left-to-right prime/composite index chain | P=0 C=1\r\n" + GetPCIndexChainL2R(number) + "\r\n" + "Chain length = " + IndexChainLength(number) + "\t\t" + BinaryPCIndexChainL2R(number) + "  =  " + DecimalPCIndexChainL2R(number));
            str.AppendLine();
            str.AppendLine("Right-to-left prime/composite index chain | P=0 C=1\r\n" + GetPCIndexChainR2L(number) + "\r\n" + "Chain length = " + IndexChainLength(number) + "\t\t" + BinaryPCIndexChainR2L(number) + "  =  " + DecimalPCIndexChainR2L(number));
            str.AppendLine();
            str.AppendLine("Left-to-right composite/prime index chain | C=0 P=1\r\n" + GetCPIndexChainL2R(number) + "\r\n" + "Chain length = " + IndexChainLength(number) + "\t\t" + BinaryCPIndexChainL2R(number) + "  =  " + DecimalCPIndexChainL2R(number));
            str.AppendLine();
            str.AppendLine("Right-to-left composite/prime index chain | C=0 P=1\r\n" + GetCPIndexChainR2L(number) + "\r\n" + "Chain length = " + IndexChainLength(number) + "\t\t" + BinaryCPIndexChainR2L(number) + "  =  " + DecimalCPIndexChainR2L(number));

            string filename = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + ".txt";
            if (Directory.Exists(Globals.NUMBERS_FOLDER))
            {
                filename = Globals.NUMBERS_FOLDER + "/" + filename;
                FileHelper.SaveText(filename, str.ToString());

                // show file content after save
                if (File.Exists(filename))
                {
                    FileHelper.WaitForReady(filename);

                    System.Diagnostics.Process.Start("Notepad.exe", filename);
                }
            }
        }
    }

    private long DecimalPCIndexChainL2R(long number)
    {
        if (number < 0L) number = -1L * number;
        if (number <= 1L) return -1L;

        StringBuilder str = new StringBuilder();
        while (number > 1L)
        {
            int index = -1;
            if ((index = Numbers.PrimeIndexOf(number)) > -1)
            {
                str.Append("0");
            }
            else if ((index = Numbers.CompositeIndexOf(number)) > -1)
            {
                str.Append("1");
            }
            else // number is too large
            {
                return 0L;
            }
            number = index;
        }
        return Convert.ToInt64(str.ToString(), 2);
    }
    private long DecimalPCIndexChainR2L(long number)
    {
        if (number < 0L) number = -1L * number;
        if (number <= 1L) return -1L;

        StringBuilder str = new StringBuilder();
        while (number > 1L)
        {
            int index = -1;
            if ((index = Numbers.PrimeIndexOf(number)) > -1)
            {
                str.Insert(0, "0");
            }
            else if ((index = Numbers.CompositeIndexOf(number)) > -1)
            {
                str.Insert(0, "1");
            }
            else // number is too large
            {
                return 0L;
            }
            number = index;
        }
        return Convert.ToInt64(str.ToString(), 2);
    }
    private long DecimalCPIndexChainL2R(long number)
    {
        if (number < 0L) number = -1L * number;
        if (number <= 1L) return -1L;

        StringBuilder str = new StringBuilder();
        while (number > 1L)
        {
            int index = -1;
            if ((index = Numbers.PrimeIndexOf(number)) > -1)
            {
                str.Append("1");
            }
            else if ((index = Numbers.CompositeIndexOf(number)) > -1)
            {
                str.Append("0");
            }
            else // number is too large
            {
                return 0L;
            }
            number = index;
        }
        return Convert.ToInt64(str.ToString(), 2);
    }
    private long DecimalCPIndexChainR2L(long number)
    {
        if (number < 0L) number = -1L * number;
        if (number <= 1L) return -1L;

        StringBuilder str = new StringBuilder();
        while (number > 1L)
        {
            int index = -1;
            if ((index = Numbers.PrimeIndexOf(number)) > -1)
            {
                str.Insert(0, "1");
            }
            else if ((index = Numbers.CompositeIndexOf(number)) > -1)
            {
                str.Insert(0, "0");
            }
            else // number is too large
            {
                return 0L;
            }
            number = index;
        }
        return Convert.ToInt64(str.ToString(), 2);
    }
    private int IndexChainLength(long number)
    {
        if (number <= 1L) return 0;

        int length = 0;
        while (number > 1L)
        {
            int index = -1;
            if ((index = Numbers.PrimeIndexOf(number)) > -1)
            {
                length++;
            }
            else if ((index = Numbers.CompositeIndexOf(number)) > -1)
            {
                length++;
            }
            else // number is too large
            {
                return 0;
            }
            number = index;
        }
        return length;
    }
    private string BinaryPCIndexChainL2R(long number)
    {
        if (number < 0L) number = -1L * number;
        if (number <= 1L) return "0";

        StringBuilder str = new StringBuilder();
        while (number > 1L)
        {
            int index = -1;
            if ((index = Numbers.PrimeIndexOf(number)) > -1)
            {
                str.Append("0");
            }
            else if ((index = Numbers.CompositeIndexOf(number)) > -1)
            {
                str.Append("1");
            }
            else // number is too large
            {
                return "";
            }
            number = index;
        }
        return str.ToString();
    }
    private string BinaryPCIndexChainR2L(long number)
    {
        if (number < 0L) number = -1L * number;
        if (number <= 1L) return "0";

        StringBuilder str = new StringBuilder();
        while (number > 1L)
        {
            int index = -1;
            if ((index = Numbers.PrimeIndexOf(number)) > -1)
            {
                str.Insert(0, "0");
            }
            else if ((index = Numbers.CompositeIndexOf(number)) > -1)
            {
                str.Insert(0, "1");
            }
            else // number is too large
            {
                return "";
            }
            number = index;
        }
        return str.ToString();
    }
    private string BinaryCPIndexChainL2R(long number)
    {
        if (number < 0L) number = -1L * number;
        if (number <= 1L) return "0";

        StringBuilder str = new StringBuilder();
        while (number > 1L)
        {
            int index = -1;
            if ((index = Numbers.PrimeIndexOf(number)) > -1)
            {
                str.Append("1");
            }
            else if ((index = Numbers.CompositeIndexOf(number)) > -1)
            {
                str.Append("0");
            }
            else // number is too large
            {
                return "";
            }
            number = index;
        }
        return str.ToString();
    }
    private string BinaryCPIndexChainR2L(long number)
    {
        if (number < 0L) number = -1L * number;
        if (number <= 1L) return "0";

        StringBuilder str = new StringBuilder();
        while (number > 1L)
        {
            int index = -1;
            if ((index = Numbers.PrimeIndexOf(number)) > -1)
            {
                str.Insert(0, "1");
            }
            else if ((index = Numbers.CompositeIndexOf(number)) > -1)
            {
                str.Insert(0, "0");
            }
            else // number is too large
            {
                return "";
            }
            number = index;
        }
        return str.ToString();
    }
    private string GetPCIndexChainL2R(long number)
    {
        if (number < 0L) number = -1L * number;
        if (number <= 1L) return "0";

        StringBuilder str = new StringBuilder();
        while (number > 1L)
        {
            if (str.Length > 0)
            {
                str.Append("-");
            }

            int index = -1;
            if ((index = Numbers.PrimeIndexOf(number)) > -1)
            {
                str.Append("P" + index.ToString());
            }
            else if ((index = Numbers.CompositeIndexOf(number)) > -1)
            {
                str.Append("C" + index.ToString());
            }
            else // number is too large
            {
                return "";
            }
            number = index;
        }
        return str.ToString();
    }
    private string GetPCIndexChainR2L(long number)
    {
        if (number < 0L) number = -1L * number;
        if (number <= 1L) return "0";

        StringBuilder str = new StringBuilder();
        while (number > 1L)
        {
            if (str.Length > 0)
            {
                str.Insert(0, "-");
            }

            int index = -1;
            if ((index = Numbers.PrimeIndexOf(number)) > -1)
            {
                str.Insert(0, "P" + index.ToString());
            }
            else if ((index = Numbers.CompositeIndexOf(number)) > -1)
            {
                str.Insert(0, "C" + index.ToString());
            }
            else // number is too large
            {
                return "";
            }
            number = index;
        }
        return str.ToString();
    }
    private string GetCPIndexChainL2R(long number)
    {
        if (number < 0L) number = -1L * number;
        if (number <= 1L) return "0";

        StringBuilder str = new StringBuilder();
        while (number > 1L)
        {
            if (str.Length > 0)
            {
                str.Append("-");
            }

            int index = -1;
            if ((index = Numbers.PrimeIndexOf(number)) > -1)
            {
                str.Append("P" + index.ToString());
            }
            else if ((index = Numbers.CompositeIndexOf(number)) > -1)
            {
                str.Append("C" + index.ToString());
            }
            else // number is too large
            {
                return "";
            }
            number = index;
        }
        return str.ToString();
    }
    private string GetCPIndexChainR2L(long number)
    {
        if (number < 0L) number = -1L * number;
        if (number <= 1L) return "0";

        StringBuilder str = new StringBuilder();
        while (number > 1L)
        {
            if (str.Length > 0)
            {
                str.Insert(0, "-");
            }

            int index = -1;
            if ((index = Numbers.PrimeIndexOf(number)) > -1)
            {
                str.Insert(0, "P" + index.ToString());
            }
            else if ((index = Numbers.CompositeIndexOf(number)) > -1)
            {
                str.Insert(0, "C" + index.ToString());
            }
            else // number is too large
            {
                return "";
            }
            number = index;
        }
        return str.ToString();
    }
    private void NumberIndexChain(long number)
    {
        string filename = "NumberIndexChain" + "_" + number + ".txt";
        StringBuilder str = new StringBuilder();
        int length = IndexChainLength(number);
        str.AppendLine(number.ToString() + "\t" + length.ToString() + "\t" + GetPCIndexChainL2R(number));
        SaveNumberIndexChain(filename, number, length, str.ToString());
    }
    private void NumbersOfIndexChainLength(int length)
    {
        string filename = "PIndexChainLength" + "_" + length + ".txt";
        StringBuilder str = new StringBuilder();
        long running_total = 0L;
        for (int i = 0; i < Numbers.Primes.Count; i++)
        {
            long number = Numbers.Primes[i];
            int len = IndexChainLength(number);
            if (len == length)
            {
                running_total += number;
                str.AppendLine(number.ToString() + "\t" + running_total.ToString() + "\t" + DecimalPCIndexChainL2R(number).ToString() + "\t" + DecimalPCIndexChainR2L(number).ToString() + "\t" + DecimalCPIndexChainL2R(number).ToString() + "\t" + DecimalCPIndexChainR2L(number).ToString() + "\t" + GetPCIndexChainL2R(number));
            }
        }
        SaveIndexChainLength(filename, NumberType.Prime, length, str.ToString());

        filename = "APIndexChainLength" + "_" + length + ".txt";
        str = new StringBuilder();
        running_total = 0L;
        for (int i = 0; i < Numbers.AdditivePrimes.Count; i++)
        {
            long number = Numbers.AdditivePrimes[i];
            int len = IndexChainLength(number);
            if (len == length)
            {
                running_total += number;
                str.AppendLine(number.ToString() + "\t" + running_total.ToString() + "\t" + DecimalPCIndexChainL2R(number).ToString() + "\t" + DecimalPCIndexChainR2L(number).ToString() + "\t" + DecimalCPIndexChainL2R(number).ToString() + "\t" + DecimalCPIndexChainR2L(number).ToString() + "\t" + GetPCIndexChainL2R(number));
            }
        }
        SaveIndexChainLength(filename, NumberType.AdditivePrime, length, str.ToString());

        filename = "XPIndexChainLength" + "_" + length + ".txt";
        str = new StringBuilder();
        running_total = 0L;
        for (int i = 0; i < Numbers.NonAdditivePrimes.Count; i++)
        {
            long number = Numbers.NonAdditivePrimes[i];
            int len = IndexChainLength(number);
            if (len == length)
            {
                running_total += number;
                str.AppendLine(number.ToString() + "\t" + running_total.ToString() + "\t" + DecimalPCIndexChainL2R(number).ToString() + "\t" + DecimalPCIndexChainR2L(number).ToString() + "\t" + DecimalCPIndexChainL2R(number).ToString() + "\t" + DecimalCPIndexChainR2L(number).ToString() + "\t" + GetPCIndexChainL2R(number));
            }
        }
        SaveIndexChainLength(filename, NumberType.NonAdditivePrime, length, str.ToString());

        filename = "CIndexChainLength" + "_" + length + ".txt";
        str = new StringBuilder();
        running_total = 0L;
        for (int i = 0; i < Numbers.Composites.Count; i++)
        {
            long number = Numbers.Composites[i];
            int len = IndexChainLength(number);
            if (len == length)
            {
                running_total += number;
                str.AppendLine(number.ToString() + "\t" + running_total.ToString() + "\t" + DecimalPCIndexChainL2R(number).ToString() + "\t" + DecimalPCIndexChainR2L(number).ToString() + "\t" + DecimalCPIndexChainL2R(number).ToString() + "\t" + DecimalCPIndexChainR2L(number).ToString() + "\t" + GetPCIndexChainL2R(number));
            }
        }
        SaveIndexChainLength(filename, NumberType.Composite, length, str.ToString());

        filename = "ACIndexChainLength" + "_" + length + ".txt";
        str = new StringBuilder();
        running_total = 0L;
        for (int i = 0; i < Numbers.AdditiveComposites.Count; i++)
        {
            long number = Numbers.AdditiveComposites[i];
            int len = IndexChainLength(number);
            if (len == length)
            {
                running_total += number;
                str.AppendLine(number.ToString() + "\t" + running_total.ToString() + "\t" + DecimalPCIndexChainL2R(number).ToString() + "\t" + DecimalPCIndexChainR2L(number).ToString() + "\t" + DecimalCPIndexChainL2R(number).ToString() + "\t" + DecimalCPIndexChainR2L(number).ToString() + "\t" + GetPCIndexChainL2R(number));
            }
        }
        SaveIndexChainLength(filename, NumberType.AdditiveComposite, length, str.ToString());

        filename = "XCIndexChainLength" + "_" + length + ".txt";
        str = new StringBuilder();
        running_total = 0L;
        for (int i = 0; i < Numbers.NonAdditiveComposites.Count; i++)
        {
            long number = Numbers.NonAdditiveComposites[i];
            int len = IndexChainLength(number);
            if (len == length)
            {
                running_total += number;
                str.AppendLine(number.ToString() + "\t" + running_total.ToString() + "\t" + DecimalPCIndexChainL2R(number).ToString() + "\t" + DecimalPCIndexChainR2L(number).ToString() + "\t" + DecimalCPIndexChainL2R(number).ToString() + "\t" + DecimalCPIndexChainR2L(number).ToString() + "\t" + GetPCIndexChainL2R(number));
            }
        }
        SaveIndexChainLength(filename, NumberType.NonAdditiveComposite, length, str.ToString());

        filename = "NIndexChainLength" + "_" + length + ".txt";
        str = new StringBuilder();
        running_total = 0L;
        for (int i = 0; i < int.MaxValue / 1024; i++)
        {
            int len = IndexChainLength(i);
            if (len == length)
            {
                long number = i;
                running_total += number;
                str.AppendLine(number.ToString() + "\t" + running_total.ToString() + "\t" + DecimalPCIndexChainL2R(number).ToString() + "\t" + DecimalPCIndexChainR2L(number).ToString() + "\t" + DecimalCPIndexChainL2R(number).ToString() + "\t" + DecimalCPIndexChainR2L(number).ToString() + "\t" + GetPCIndexChainL2R(number));
            }
        }
        SaveIndexChainLength(filename, NumberType.Natural, length, str.ToString());
    }
    public void SaveNumberIndexChain(string filename, long number, int chain_length, string text)
    {
        if (Directory.Exists(Globals.NUMBERS_FOLDER))
        {
            filename = Globals.NUMBERS_FOLDER + "/" + filename;
            try
            {
                using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                {
                    writer.WriteLine("-----------------------------------------------------");
                    writer.WriteLine(number.ToString() + " with IndexChainLength = " + chain_length.ToString());
                    writer.WriteLine("-----------------------------------------------------");
                    writer.WriteLine("Number\tTotal\tPC_L2R\tPC_R2L\tCP_L2R\tCP_R2L\tChain");
                    writer.WriteLine("-----------------------------------------------------");
                    writer.Write(text);
                    writer.WriteLine("-----------------------------------------------------");
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }

            // show file content after save
            if (File.Exists(filename))
            {
                FileHelper.WaitForReady(filename);

                System.Diagnostics.Process.Start("Notepad.exe", filename);
            }
        }
    }
    public void SaveIndexChainLength(string filename, NumberType number_type, int chain_length, string text)
    {
        if (Directory.Exists(Globals.NUMBERS_FOLDER))
        {
            filename = Globals.NUMBERS_FOLDER + "/" + filename;
            try
            {
                using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                {
                    writer.WriteLine("-----------------------------------------------------");
                    writer.WriteLine(number_type.ToString() + " numbers with IndexChainLength = " + chain_length.ToString());
                    writer.WriteLine("-----------------------------------------------------");
                    writer.WriteLine("Number\tTotal\tPC_L2R\tPC_R2L\tCP_L2R\tCP_R2L\tChain");
                    writer.WriteLine("-----------------------------------------------------");
                    writer.Write(text);
                    writer.WriteLine("-----------------------------------------------------");
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }

            // show file content after save
            if (File.Exists(filename))
            {
                FileHelper.WaitForReady(filename);

                System.Diagnostics.Process.Start("Notepad.exe", filename);
            }
        }
    }

    private IndexType m_index_type = IndexType.Prime;
    private void NthNumberTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            try
            {
                long number = -1L;
                int nth_index = int.Parse(NthNumberTextBox.Text) - 1;
                NthNumberTextBox.ForeColor = GetNumberTypeColor(nth_index);
                if (m_index_type == IndexType.Prime)
                {
                    number = Numbers.Primes[nth_index];
                    FactorizeValue(number);
                }
                else // any other index type will be treated as IndexNumberType.Composite
                {
                    number = Numbers.Composites[nth_index];
                    FactorizeValue(number);
                }
            }
            catch
            {
                FactorizeValue(0L);
            }
        }
    }
    private void NthAdditiveNumberTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            try
            {
                long number = -1L;
                int nth_index = int.Parse(NthAdditiveNumberTextBox.Text) - 1;
                NthAdditiveNumberTextBox.ForeColor = GetNumberTypeColor(nth_index);
                if (m_index_type == IndexType.Prime)
                {
                    number = Numbers.AdditivePrimes[nth_index];
                    FactorizeValue(number);
                }
                else // any other index type will be treated as IndexNumberType.Composite
                {
                    number = Numbers.AdditiveComposites[nth_index];
                    FactorizeValue(number);
                }
            }
            catch
            {
                FactorizeValue(0L);
            }
        }
    }
    private void NthNonAdditiveNumberTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            try
            {
                long number = -1L;
                int nth_index = int.Parse(NthNonAdditiveNumberTextBox.Text) - 1;
                NthNonAdditiveNumberTextBox.ForeColor = GetNumberTypeColor(nth_index);
                if (m_index_type == IndexType.Prime)
                {
                    number = Numbers.NonAdditivePrimes[nth_index];
                    FactorizeValue(number);
                }
                else // any other index type will be treated as IndexNumberType.Composite
                {
                    number = Numbers.NonAdditiveComposites[nth_index];
                    FactorizeValue(number);
                }
            }
            catch
            {
                FactorizeValue(0L);
            }
        }
    }

    private NumberKind m_number_kind = NumberKind.Deficient;
    private void NumberKindIndexTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            try
            {
                long number = -1L;
                int nth_index = int.Parse(NumberKindIndexTextBox.Text) - 1;
                switch (m_number_kind)
                {
                    case NumberKind.Deficient:
                        {
                            number = Numbers.Deficients[nth_index];
                            FactorizeValue(number);
                        }
                        break;
                    case NumberKind.Perfect:
                        {
                            number = Numbers.Perfects[nth_index];
                            FactorizeValue(number);
                        }
                        break;
                    case NumberKind.Abundant:
                        {
                            number = Numbers.Abundants[nth_index];
                            FactorizeValue(number);
                        }
                        break;
                    default:
                        {
                        }
                        break;
                }
            }
            catch
            {
                FactorizeValue(0L);
            }
        }
    }
    private Color GetNumberTypeColor(long number)
    {
        if (number <= 0L)
        {
            return Color.Gray;
        }
        else if (Numbers.IsUnit(number))
        {
            return Color.FromArgb(180, 0, 208);
        }
        else if (Numbers.IsAdditivePrime(number))
        {
            return Color.Blue;
        }
        else if (Numbers.IsPrime(number))
        {
            return Color.Green;
        }
        else if (Numbers.IsAdditiveComposite(number))
        {
            return Color.Brown;
        }
        else if (Numbers.IsComposite(number))
        {
            return Color.Black;
        }
        else
        {
            return Color.Black;
        }
    }

    private void TextBoxLabelControls_CtrlClick(object sender, EventArgs e)
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
    private void PCIndexChainL2RTextBox_TextChanged(object sender, EventArgs e)
    {
        long number = 0;
        if (long.TryParse(ValueTextBox.Text, out number))
        {
            this.ToolTip.SetToolTip(this.PCIndexChainL2RTextBox, "Left-to-right prime/composite index chain | P=0 C=1\r\n" + GetPCIndexChainL2R(number) + "\r\n" + "Chain length = " + IndexChainLength(number) + "\t\t" + BinaryPCIndexChainL2R(number) + "  =  " + DecimalPCIndexChainL2R(number));
        }
    }
    private void PCIndexChainR2LTextBox_TextChanged(object sender, EventArgs e)
    {
        long number = 0;
        if (long.TryParse(ValueTextBox.Text, out number))
        {
            this.ToolTip.SetToolTip(this.PCIndexChainR2LTextBox, "Right-to-left prime/composite index chain | P=0 C=1\r\n" + GetPCIndexChainR2L(number) + "\r\n" + "Chain length = " + IndexChainLength(number) + "\t\t" + BinaryPCIndexChainR2L(number) + "  =  " + DecimalPCIndexChainR2L(number));
        }
    }
    private void CPIndexChainL2RTextBox_TextChanged(object sender, EventArgs e)
    {
        long number = 0;
        if (long.TryParse(ValueTextBox.Text, out number))
        {
            this.ToolTip.SetToolTip(this.CPIndexChainL2RTextBox, "Left-to-right composite/prime index chain | C=0 P=1\r\n" + GetCPIndexChainL2R(number) + "\r\n" + "Chain length = " + IndexChainLength(number) + "\t\t" + BinaryCPIndexChainL2R(number) + "  =  " + DecimalCPIndexChainL2R(number));
        }
    }
    private void CPIndexChainR2LTextBox_TextChanged(object sender, EventArgs e)
    {
        long number = 0;
        if (long.TryParse(ValueTextBox.Text, out number))
        {
            this.ToolTip.SetToolTip(this.CPIndexChainR2LTextBox, "Right-to-left composite/prime index chain | C=0 P=1\r\n" + GetCPIndexChainR2L(number) + "\r\n" + "Chain length = " + IndexChainLength(number) + "\t\t" + BinaryCPIndexChainR2L(number) + "  =  " + DecimalCPIndexChainR2L(number));
        }
    }
    private void IndexChainLengthTextBox_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            TextBoxLabelControls_CtrlClick(sender, e);
        }
        else
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                int length = 0;
                if (int.TryParse(IndexChainLengthTextBox.Text, out length))
                {
                    NumbersOfIndexChainLength(length);
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
    private void FactorizeNumber(Label control)
    {
        if (control != null)
        {
            long number = 0L;
            try
            {
                string text = control.Text;
                if (!String.IsNullOrEmpty(text))
                {
                    number = Math.Abs((long)double.Parse(text));
                }

                FactorizeValue(number);
            }
            catch
            {
                //number = -1L; // error
            }
        }
    }
    private void FactorizeNumber(TextBox control)
    {
        if (control != null)
        {
            if (control != ValueTextBox)
            {
                long number = 0L;
                try
                {
                    string text = control.Text;
                    if (!String.IsNullOrEmpty(text))
                    {
                        if (control.Name.StartsWith("LetterFrequency"))
                        {
                            number = Math.Abs((long)double.Parse(text));
                        }
                        else if (control.Name.StartsWith("Decimal"))
                        {
                            number = Radix.Decode(text, 10L);
                        }
                        else if (text.StartsWith("4×")) // 4n+1
                        {
                            int start = "4×".Length;
                            int end = text.IndexOf("+");
                            text = text.Substring(start, end - start);
                            number = Radix.Decode(text, 10L);
                        }
                        else
                        {
                            number = Radix.Decode(text, 10L);
                        }
                    }

                    FactorizeValue(number);
                }
                catch
                {
                    //number = -1L; // error
                }
            }
        }
    }
    private void Control_MouseHover(object sender, EventArgs e)
    {

    }

    private void EnableEntryControls()
    {
        MultithreadingCheckBox.Enabled = true;

        ValueTextBox.Enabled = true;
        ValueTextBox.ForeColor = SystemColors.ControlText;
        ValueTextBox.Refresh();
        ValueTextBox.Focus(); // will show the InputPanel too

        m_old_progress = -1;
        ProgressLabel.Text = "Progress";
        ProgressLabel.Refresh();
    }
    private void DisableEntryControls()
    {
        MultithreadingCheckBox.Enabled = false;

        ValueTextBox.Enabled = false;
        ValueTextBox.ForeColor = SystemColors.ControlDark;
        ValueTextBox.Refresh();

        ProgressLabel.Text = "IsPrime ...";
        ProgressLabel.Refresh();
    }
    private void ClearNumberAnalyses()
    {
        NthNumberTextBox.Text = string.Empty;
        NthAdditiveNumberTextBox.Text = string.Empty;
        NthNonAdditiveNumberTextBox.Text = string.Empty;
        NthNumberTextBox.Refresh();
        NthAdditiveNumberTextBox.Refresh();
        NthNonAdditiveNumberTextBox.Refresh();

        SquareSumTextBox.Text = string.Empty;
        SquareSumTextBox.Refresh();
        SquareDiffTextBox.Text = string.Empty;
        SquareDiffTextBox.Refresh();

        NumberKindIndexTextBox.Text = string.Empty;
        NumberKindIndexTextBox.Refresh();
        SumOfProperDivisorsTextBox.Text = string.Empty;
        SumOfProperDivisorsTextBox.Refresh();
        SumOfDivisorsTextBox.Text = string.Empty;
        SumOfDivisorsTextBox.Refresh();

        PCIndexChainL2RTextBox.Text = string.Empty;
        PCIndexChainL2RTextBox.Refresh();
        PCIndexChainR2LTextBox.Text = string.Empty;
        PCIndexChainR2LTextBox.Refresh();
        CPIndexChainL2RTextBox.Text = string.Empty;
        CPIndexChainL2RTextBox.Refresh();
        CPIndexChainR2LTextBox.Text = string.Empty;
        CPIndexChainR2LTextBox.Refresh();
        IndexChainLengthTextBox.Text = string.Empty;
        IndexChainLengthTextBox.Refresh();
    }
    private void ClearFactors()
    {
        PrimeFactorsTextBox.Text = string.Empty;
        PrimeFactorsTextBox.Refresh();
        OutputTextBox.Text = string.Empty;
        OutputTextBox.Refresh();
    }
    private void ClearProgress()
    {
        ElapsedTimeValueLabel.Text = "00:00:00";
        ElapsedTimeValueLabel.Refresh();
        MilliSecondsLabel.Text = "000";
        MilliSecondsLabel.Refresh();
        ProgressBar.Value = 0;
        ProgressValueLabel.Text = ProgressBar.Value + "%";
        ProgressValueLabel.Refresh();
    }
    private void BeforeProcessing()
    {
        DisableEntryControls();
        ClearNumberAnalyses();
        ClearFactors();
        ClearProgress();
    }
    private void CallRun()
    {
        ValueTextBox.Text = ValueTextBox.Text.Replace(" ", "");

        long number = 0L;
        string expression = ValueTextBox.Text;
        if (long.TryParse(expression, out number))
        {
            m_double_value = (double)number;
        }
        else
        {
            m_double_value = CalculateValue(expression);
            number = (long)Math.Round(m_double_value);
        }

        if (m_double_value != number)
        {
            PrimeFactorsTextBox.Text = m_double_value.ToString();
        }

        Run();
    }
    private bool m_multithreading = true;
    private void MultithreadingCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_multithreading = MultithreadingCheckBox.Checked;
    }
    public void Run()
    {
        // guard against multiple runs on multiple ENTER keys
        if ((m_worker_thread != null) && (m_worker_thread.IsAlive)) return;

        this.Cursor = Cursors.WaitCursor;
        try
        {
            string arguments = ValueTextBox.Text;
            if (arguments.Length == 0) return;
            while (arguments.StartsWith("-")) arguments = arguments.Substring(1);
            if (arguments.StartsWith("0")) return;

            BeforeProcessing();
            try
            {
                if (Numbers.IsDigitsOnly(arguments))
                {
                    m_factorizer = new Factorizer(this, "factor", arguments, m_multithreading);
                }
                else
                {
                    m_factorizer = new Factorizer(this, null, arguments, m_multithreading);
                }

                if (m_factorizer != null)
                {
                    m_worker_thread = new Thread(new ThreadStart(m_factorizer.Run));
                    m_worker_thread.Priority = ThreadPriority.Highest;
                    m_worker_thread.IsBackground = false;
                    m_worker_thread.Start();
                }
            }
            catch
            {
                Cancel();
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    public void Cancel()
    {
        if ((m_worker_thread != null) && (m_worker_thread.IsAlive))
        {
            if (m_factorizer != null)
            {
                m_factorizer.Cancel = true;

                if (m_worker_thread != null)
                {
                    m_worker_thread.Join();
                    m_worker_thread = null;
                }
            }

            AfterCancelled();
        }
    }
    private void AfterCancelled()
    {
        EnableEntryControls();
        ClearProgress();
    }
    private void AfterProcessing()
    {
        string item = ValueTextBox.Text;
        if (!m_history.Contains(item))
        {
            m_history.Insert(m_history_index + 1, item);
            m_history_index++;
        }
        HistoryDeleteLabel.Enabled = (m_history.Count > 0);
        HistoryClearLabel.Enabled = (m_history.Count > 0);

        m_index_type = IndexType.Prime;

        if (m_factorizer != null)
        {
            EnableEntryControls();

            long number = 0L;
            if (long.TryParse(m_factorizer.Number, out number))
            {
                AnalyzeValue(number);
            }
        }
    }

    private enum TimeDisplayMode { Elapsed, Remaining }
    private TimeDisplayMode m_time_display_mode = TimeDisplayMode.Elapsed;
    private void ElapsedTimeLabel_Click(object sender, EventArgs e)
    {
        if ((m_worker_thread != null) && (m_worker_thread.IsAlive))
        {
            if (m_time_display_mode == TimeDisplayMode.Elapsed)
            {
                ElapsedTimeLabel.Text = "Remaining Time";
                m_time_display_mode = TimeDisplayMode.Remaining;
            }
            else
            {
                ElapsedTimeLabel.Text = "Elapsed Time";
                m_time_display_mode = TimeDisplayMode.Elapsed;
            }
        }
    }

    private int m_old_progress = -1;
    public delegate void UpdateProgressBar(int progress);
    public void UpdateProgressBarMethod(int progress)
    {
        if (!m_factorizer.Cancel)
        {
            if (progress > m_old_progress)
            {
                m_old_progress = progress;
                if (m_factorizer != null)
                {
                    if (!m_factorizer.Cancel)
                    {
                        if ((progress >= 0) && (progress <= 100))
                        {
                            ProgressBar.Value = progress;
                            ProgressValueLabel.Text = ProgressBar.Value + "%";
                            ProgressValueLabel.Refresh();

                            UpdateTimer(progress);
                        }
                    }

                    if (progress == 100) // finished naturally 
                    {
                        AfterProcessing();
                    }
                }
            }
            else // no new progress
            {
                if (m_time_display_mode == TimeDisplayMode.Elapsed)
                {
                    if (m_factorizer != null)
                    {
                        UpdateTimer(progress);
                    }
                }
                else //if (m_time_display_mode == TimeDisplayMode.Remaining)
                {
                    // don't update remaining as it goes up wrongly with time passage and no progress
                    // if progree was double not int then ok but now
                    // it keeps going up up up till it there is progress so t goes down to the real remaining time and then
                    // it keeps going up up up again and so on, like a seesaw
                }
            }
        }
    }
    private void UpdateTimer(int progress)
    {
        TimeSpan timespan = m_factorizer.Duration;
        if (m_time_display_mode == TimeDisplayMode.Remaining)
        {
            if (progress > 0)
            {
                long ticks = (long)((double)timespan.Ticks * ((double)(100 - progress) / (double)progress));
                timespan = new TimeSpan(ticks);
            }
            else
            {
                timespan = new TimeSpan(0, 0, 0);
            }
        }
        ElapsedTimeValueLabel.Text = String.Format("{0:00}:{1:00}:{2:00}", timespan.Hours, timespan.Minutes, timespan.Seconds);
        ElapsedTimeValueLabel.Refresh();
        MilliSecondsLabel.Text = String.Format("{0:000}", timespan.Milliseconds);
        MilliSecondsLabel.Refresh();
    }

    public delegate void UpdateOutputTextBox(string output);
    public void UpdateOutputTextBoxMethod(string output)
    {
        if (m_factorizer != null)
        {
            if (!m_factorizer.Cancel)
            {
                if (m_factorizer.IsPrime == false)
                {
                    ProgressLabel.Text = "Factoring ...";
                    ProgressLabel.Refresh();
                }

                OutputTextBox.Text = output;
                OutputTextBox.Refresh();
                OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
                OutputTextBox.ScrollToCaret();

                int pos = output.LastIndexOf("\r\n");
                if (pos > -1)
                {
                    string result = output.Substring(pos);
                    if ((result.Length > 0) && (result != "\r\n"))
                    {
                        pos = result.IndexOf("=");
                        if (pos > -1)
                        {
                            PrimeFactorsTextBox.Text = result.Substring(pos + 1);
                            PrimeFactorsTextBox.Refresh();
                        }
                    }
                }

                if (!output.Contains(Environment.NewLine))
                {
                    if (output.Length == 0) return;
                    while (output.StartsWith("-")) output = output.Substring(1);
                    while (output.StartsWith("0")) output = output.Substring(1);
                    if (output.Length == 0) return; // check again, yes!

                    if (Char.IsLetter(output[0])) return;
                    if (output == "1") return;

                    BeforeProcessing();

                    ValueTextBox.Text = output;
                    ValueTextBox.Refresh();
                    ValueTextBox.SelectionStart = ValueTextBox.Text.Length;

                    try
                    {
                        m_factorizer = new Factorizer(this, "factor", output, m_multithreading);
                        if (m_factorizer != null)
                        {
                            m_worker_thread = new Thread(new ThreadStart(m_factorizer.Run));
                            m_worker_thread.Priority = ThreadPriority.Highest;
                            m_worker_thread.IsBackground = false;
                            m_worker_thread.Start();
                        }
                    }
                    catch
                    {
                        Cancel();
                    }
                }
            }
        }
    }

    private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            // toggle visible
            this.Visible = true;

            // restore if minimized
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }

            // and bring to foreground
            this.Activate();
        }
    }
    private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        //if (e.Button == MouseButtons.Left)
        //{
        //    // make visible (in case it is hidden)
        //    this.Visible = true;

        //    // toggle maximized
        //    if (this.WindowState == FormWindowState.Maximized)
        //    {
        //        this.WindowState = FormWindowState.Normal;
        //    }
        //    else
        //    {
        //        this.WindowState = FormWindowState.Maximized;
        //    }

        //    // and bring to foreground
        //    this.Activate();
        //}
    }
    private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MessageBox.Show
        (
            Application.ProductName + "  v" + Application.ProductVersion + "\r\n" +
            "Copyright ©2017 Ali Adams - علي عبد الرزاق عبد الكريم القره غولي" + "\r\n" + "\r\n" +
            "Powered by Yet Another Factoring Utility\r\nBenjamin Buhrow <bbuhrow@gmail.com>" + "\r\n" + "\r\n" +
            "God >",
            "About",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1
        );
    }
    private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        CloseApplication();

        Application.Exit();
        System.Environment.Exit(0);
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
}
