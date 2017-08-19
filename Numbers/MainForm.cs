using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.IO;

public partial class MainForm : Form
{
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

    private static int ROWS = 57;
    private static int COLS = 10;
    private TextBox[,] controls = new TextBox[ROWS, COLS];

    public MainForm()
    {
        InitializeComponent();

        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < COLS; j++)
            {
                TextBox control = new TextBox();
                if (control != null)
                {
                    control.Width = 60;
                    control.Height = 23;
                    control.Top = 20 + i * 23;
                    control.Left = 8 + j * 62;
                    control.TextAlign = HorizontalAlignment.Center;
                    control.Font = new Font("Arial", 12);
                    control.MaxLength = 7;
                    MainPanel.Controls.Add(control);

                    control.KeyPress += FixMicrosoft;
                    if (j < (COLS - 1)) control.KeyDown += TextBox_KeyDown;
                    if (j == 0) control.TextChanged += TextBox_TextChanged;

                    switch (j)
                    {
                        case 0:
                            {
                                control.BackColor = SystemColors.Info;
                            }
                            break;
                        case 1:
                        case 2:
                        case 3:
                            {
                                control.BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[3];
                                control.BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[3];
                                control.BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[3];
                            }
                            break;
                        case 4:
                        case 5:
                        case 6:
                            {
                                control.BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[6];
                                control.BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[6];
                                control.BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[6];
                            }
                            break;
                        case 7:
                            {
                                control.BackColor = Numbers.NUMBER_KIND_COLORS[0];
                            }
                            break;
                        case 8:
                            {
                                control.BackColor = Numbers.NUMBER_KIND_COLORS[2];
                            }
                            break;
                        case 9:
                            {
                                control.BackColor = Color.Silver;
                            }
                            break;
                        default:
                            {
                                control.BackColor = Color.Black;
                            }
                            break;
                    }

                    controls[i, j] = control;
                }
            }
        }

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
        // remove icon from tray
        if (NotifyIcon != null)
        {
            NotifyIcon.Visible = false;
            NotifyIcon.Dispose();
        }

        SaveSettings();
    }

    private void IncrementIndex(object sender)
    {
        Point point = GetControlLocation(sender);
        TextBox index_control = controls[point.X, 0];
        if (index_control != null)
        {
            int index = 0;
            if (int.TryParse(index_control.Text, out index))
            {
                if (index < int.MaxValue) index++;
                index_control.Text = index.ToString();
            }
        }
    }
    private void DecrementIndex(object sender)
    {
        Point point = GetControlLocation(sender);
        TextBox index_control = controls[point.X, 0];
        if (index_control != null)
        {
            int index = 0;
            if (int.TryParse(index_control.Text, out index))
            {
                if (index > 0) index--;
                index_control.Text = index.ToString();
            }
        }
    }
    private Point GetControlLocation(object sender)
    {
        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < COLS; j++)
            {
                if (sender == controls[i, j])
                {
                    return new Point(i, j);
                }
            }
        }
        return new Point(-1, -1);
    }
    private void TextBox_TextChanged(object sender, EventArgs e)
    {
        Point point = GetControlLocation(sender);
        if (point.Y == 0)
        {
            Control control = (sender as TextBox);
            if (control != null)
            {
                control.Text = control.Text.Replace(" ", "");

                int index = 0;
                if (int.TryParse(control.Text, out index))
                {
                    index--;
                    if (index >= 0)
                    {
                        this.Cursor = Cursors.WaitCursor;
                        try
                        {
                            long p = Numbers.Primes[index];
                            long ap = Numbers.AdditivePrimes[index];
                            long xp = Numbers.NonAdditivePrimes[index];
                            long c = Numbers.Composites[index];
                            long ac = Numbers.AdditiveComposites[index];
                            long xc = Numbers.NonAdditiveComposites[index];
                            controls[point.X, 1].Text = p.ToString();
                            controls[point.X, 2].Text = ap.ToString();
                            controls[point.X, 3].Text = xp.ToString();
                            controls[point.X, 4].Text = c.ToString();
                            controls[point.X, 5].Text = ac.ToString();
                            controls[point.X, 6].Text = xc.ToString();
                            controls[point.X, 1].ForeColor = Numbers.GetNumberTypeColor(p);
                            controls[point.X, 2].ForeColor = Numbers.GetNumberTypeColor(ap);
                            controls[point.X, 3].ForeColor = Numbers.GetNumberTypeColor(xp);
                            controls[point.X, 4].ForeColor = Numbers.GetNumberTypeColor(c);
                            controls[point.X, 5].ForeColor = Numbers.GetNumberTypeColor(ac);
                            controls[point.X, 6].ForeColor = Numbers.GetNumberTypeColor(xc);
                        }
                        catch
                        {
                            controls[point.X, 1].Text = "";
                            controls[point.X, 2].Text = "";
                            controls[point.X, 3].Text = "";
                            controls[point.X, 4].Text = "";
                            controls[point.X, 5].Text = "";
                            controls[point.X, 6].Text = "";
                        }
                        finally
                        {
                            this.Cursor = Cursors.Default;
                        }

                        this.Cursor = Cursors.WaitCursor;
                        try
                        {
                            long df = Numbers.Deficients[index];
                            long ab = Numbers.Abundants[index];
                            long diff = ab - df;
                            controls[point.X, 7].Text = df.ToString();
                            controls[point.X, 8].Text = ab.ToString();
                            controls[point.X, 9].Text = diff.ToString();
                            controls[point.X, 7].ForeColor = Numbers.GetNumberTypeColor(df);
                            controls[point.X, 8].ForeColor = Numbers.GetNumberTypeColor(ab);
                            controls[point.X, 9].ForeColor = Numbers.GetNumberTypeColor(diff);
                        }
                        catch
                        {
                            controls[point.X, 7].Text = "";
                            controls[point.X, 8].Text = "";
                            controls[point.X, 9].Text = "";
                        }
                        finally
                        {
                            this.Cursor = Cursors.Default;
                        }
                    }
                    else
                    {
                        controls[point.X, 1].Text = "";
                        controls[point.X, 2].Text = "";
                        controls[point.X, 3].Text = "";
                        controls[point.X, 4].Text = "";
                        controls[point.X, 5].Text = "";
                        controls[point.X, 6].Text = "";
                        controls[point.X, 7].Text = "";
                        controls[point.X, 8].Text = "";
                        controls[point.X, 9].Text = "";
                    }
                }
                else
                {
                    controls[point.X, 1].Text = "";
                    controls[point.X, 2].Text = "";
                    controls[point.X, 3].Text = "";
                    controls[point.X, 4].Text = "";
                    controls[point.X, 5].Text = "";
                    controls[point.X, 6].Text = "";
                    controls[point.X, 7].Text = "";
                    controls[point.X, 8].Text = "";
                    controls[point.X, 9].Text = "";
                }
            }
        }
    }
    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up)
        {
            IncrementIndex(sender);
        }
        else if (e.KeyCode == Keys.Down)
        {
            DecrementIndex(sender);
        }
        else if (e.KeyCode == Keys.Enter)
        {
            Point point = GetControlLocation(sender);
            if (point.Y > 0)
            {
                Control index_control = controls[point.X, 0] as TextBox;
                if (index_control != null)
                {
                    long value = 0L;
                    if (long.TryParse((sender as TextBox).Text, out value))
                    {
                        int index = 0;
                        switch (point.Y)
                        {
                            case 1:
                                index = Numbers.PrimeIndexOf(value) + 1;
                                break;
                            case 2:
                                index = Numbers.AdditivePrimeIndexOf(value) + 1;
                                break;
                            case 3:
                                index = Numbers.NonAdditivePrimeIndexOf(value) + 1;
                                break;
                            case 4:
                                index = Numbers.CompositeIndexOf(value) + 1;
                                break;
                            case 5:
                                index = Numbers.AdditiveCompositeIndexOf(value) + 1;
                                break;
                            case 6:
                                index = Numbers.NonAdditiveCompositeIndexOf(value) + 1;
                                break;
                            case 7:
                                index = Numbers.DeficientIndexOf(value) + 1;
                                break;
                            case 8:
                                index = Numbers.AbundantIndexOf(value) + 1;
                                break;
                            case 9:
                                index = -1;
                                break;
                        }
                        index_control.Text = index.ToString();
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
