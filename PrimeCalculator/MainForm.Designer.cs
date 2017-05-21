partial class MainForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ElapsedTimeLabel = new System.Windows.Forms.Label();
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.ElapsedTimeValueLabel = new System.Windows.Forms.Label();
            this.MilliSecondsLabel = new System.Windows.Forms.Label();
            this.ProgressValueLabel = new System.Windows.Forms.Label();
            this.OutputTextBox = new System.Windows.Forms.TextBox();
            this.MainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.WebsiteLabel = new System.Windows.Forms.Label();
            this.NotifyIconContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.DigitsLabel = new System.Windows.Forms.Label();
            this.HistoryBackwardLabel = new System.Windows.Forms.Button();
            this.HistoryForewardLabel = new System.Windows.Forms.Button();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.MultithreadingCheckBox = new System.Windows.Forms.CheckBox();
            this.SumOfDivisorsTextBox = new System.Windows.Forms.TextBox();
            this.DisplayAbundantNumbersLable = new System.Windows.Forms.Label();
            this.DisplayPerfectNumbersLable = new System.Windows.Forms.Label();
            this.DisplayDeficientNumbersLable = new System.Windows.Forms.Label();
            this.NumberKindIndexTextBox = new System.Windows.Forms.TextBox();
            this.SumOfProperDivisorsTextBox = new System.Windows.Forms.TextBox();
            this.PCIndexChainLabel = new System.Windows.Forms.Label();
            this.HistoryClearLabel = new System.Windows.Forms.Label();
            this.HistoryDeleteLabel = new System.Windows.Forms.Label();
            this.ValuePanel = new System.Windows.Forms.Panel();
            this.SquareDiffTextBox = new System.Windows.Forms.TextBox();
            this.SquareSumTextBox = new System.Windows.Forms.TextBox();
            this.IndexChainLengthTextBox = new System.Windows.Forms.TextBox();
            this.NthNonAdditiveNumberTextBox = new System.Windows.Forms.TextBox();
            this.CPIndexChainL2RTextBox = new System.Windows.Forms.TextBox();
            this.PCIndexChainR2LTextBox = new System.Windows.Forms.TextBox();
            this.CPIndexChainR2LTextBox = new System.Windows.Forms.TextBox();
            this.PCIndexChainL2RTextBox = new System.Windows.Forms.TextBox();
            this.ValueTextBox = new System.Windows.Forms.TextBox();
            this.DigitalRootTextBox = new System.Windows.Forms.TextBox();
            this.DigitSumTextBox = new System.Windows.Forms.TextBox();
            this.NthAdditiveNumberTextBox = new System.Windows.Forms.TextBox();
            this.NthNumberTextBox = new System.Windows.Forms.TextBox();
            this.AddToSeparatorLine2Panel = new System.Windows.Forms.Panel();
            this.PrimeFactorsTextBox = new System.Windows.Forms.TextBox();
            this.NotifyIconContextMenuStrip.SuspendLayout();
            this.ValuePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ElapsedTimeLabel
            // 
            this.ElapsedTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ElapsedTimeLabel.BackColor = System.Drawing.SystemColors.Info;
            this.ElapsedTimeLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ElapsedTimeLabel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.ElapsedTimeLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.ElapsedTimeLabel.Location = new System.Drawing.Point(57, 322);
            this.ElapsedTimeLabel.Name = "ElapsedTimeLabel";
            this.ElapsedTimeLabel.Size = new System.Drawing.Size(180, 16);
            this.ElapsedTimeLabel.TabIndex = 26;
            this.ElapsedTimeLabel.Text = "Elapsed Time";
            this.ElapsedTimeLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ElapsedTimeLabel.Visible = false;
            this.ElapsedTimeLabel.Click += new System.EventHandler(this.ElapsedTimeLabel_Click);
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ProgressLabel.BackColor = System.Drawing.SystemColors.Info;
            this.ProgressLabel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.ProgressLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.ProgressLabel.Location = new System.Drawing.Point(0, 322);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(56, 16);
            this.ProgressLabel.TabIndex = 25;
            this.ProgressLabel.Text = "Progress";
            this.ProgressLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ProgressLabel.Visible = false;
            // 
            // ElapsedTimeValueLabel
            // 
            this.ElapsedTimeValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ElapsedTimeValueLabel.BackColor = System.Drawing.SystemColors.ControlText;
            this.ElapsedTimeValueLabel.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.ElapsedTimeValueLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.ElapsedTimeValueLabel.Location = new System.Drawing.Point(57, 336);
            this.ElapsedTimeValueLabel.Name = "ElapsedTimeValueLabel";
            this.ElapsedTimeValueLabel.Size = new System.Drawing.Size(180, 17);
            this.ElapsedTimeValueLabel.TabIndex = 28;
            this.ElapsedTimeValueLabel.Text = "00:00:00";
            this.ElapsedTimeValueLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ElapsedTimeValueLabel.Visible = false;
            // 
            // MilliSecondsLabel
            // 
            this.MilliSecondsLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.MilliSecondsLabel.BackColor = System.Drawing.SystemColors.ControlText;
            this.MilliSecondsLabel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.MilliSecondsLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.MilliSecondsLabel.Location = new System.Drawing.Point(180, 339);
            this.MilliSecondsLabel.Name = "MilliSecondsLabel";
            this.MilliSecondsLabel.Size = new System.Drawing.Size(37, 12);
            this.MilliSecondsLabel.TabIndex = 29;
            this.MilliSecondsLabel.Text = "000";
            this.MilliSecondsLabel.Visible = false;
            // 
            // ProgressValueLabel
            // 
            this.ProgressValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ProgressValueLabel.BackColor = System.Drawing.SystemColors.ControlText;
            this.ProgressValueLabel.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.ProgressValueLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.ProgressValueLabel.Location = new System.Drawing.Point(0, 336);
            this.ProgressValueLabel.Name = "ProgressValueLabel";
            this.ProgressValueLabel.Size = new System.Drawing.Size(56, 17);
            this.ProgressValueLabel.TabIndex = 27;
            this.ProgressValueLabel.Text = "0%";
            this.ProgressValueLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ProgressValueLabel.Visible = false;
            // 
            // OutputTextBox
            // 
            this.OutputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.OutputTextBox.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputTextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.OutputTextBox.Location = new System.Drawing.Point(0, 145);
            this.OutputTextBox.Multiline = true;
            this.OutputTextBox.Name = "OutputTextBox";
            this.OutputTextBox.ReadOnly = true;
            this.OutputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.OutputTextBox.Size = new System.Drawing.Size(235, 213);
            this.OutputTextBox.TabIndex = 24;
            this.OutputTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ToolTip.SetToolTip(this.OutputTextBox, "Factorization result");
            this.OutputTextBox.WordWrap = false;
            this.OutputTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBar.Location = new System.Drawing.Point(0, 357);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(240, 6);
            this.ProgressBar.TabIndex = 0;
            this.ProgressBar.Visible = false;
            // 
            // WebsiteLabel
            // 
            this.WebsiteLabel.BackColor = System.Drawing.SystemColors.Control;
            this.WebsiteLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.WebsiteLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.WebsiteLabel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.WebsiteLabel.ForeColor = System.Drawing.Color.Purple;
            this.WebsiteLabel.Location = new System.Drawing.Point(0, 364);
            this.WebsiteLabel.Name = "WebsiteLabel";
            this.WebsiteLabel.Size = new System.Drawing.Size(240, 16);
            this.WebsiteLabel.TabIndex = 31;
            this.WebsiteLabel.Tag = "http://heliwave.com";
            this.WebsiteLabel.Text = "heliwave.com     ";
            this.WebsiteLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ToolTip.SetToolTip(this.WebsiteLabel, "Ali Adams");
            this.WebsiteLabel.Click += new System.EventHandler(this.LinkLabel_Click);
            // 
            // NotifyIconContextMenuStrip
            // 
            this.NotifyIconContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AboutToolStripMenuItem,
            this.ExitToolStripMenuItem});
            this.NotifyIconContextMenuStrip.Name = "NotifyIconContextMenuStrip";
            this.NotifyIconContextMenuStrip.Size = new System.Drawing.Size(108, 48);
            // 
            // AboutToolStripMenuItem
            // 
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            this.AboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.AboutToolStripMenuItem.Text = "About";
            this.AboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.ExitToolStripMenuItem.Text = "Exit";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // NotifyIcon
            // 
            this.NotifyIcon.ContextMenuStrip = this.NotifyIconContextMenuStrip;
            this.NotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotifyIcon.Icon")));
            this.NotifyIcon.Text = "Prime Calculator";
            this.NotifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseClick);
            this.NotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
            // 
            // DigitsLabel
            // 
            this.DigitsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DigitsLabel.BackColor = System.Drawing.SystemColors.Control;
            this.DigitsLabel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.DigitsLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.DigitsLabel.Location = new System.Drawing.Point(176, 2);
            this.DigitsLabel.Name = "DigitsLabel";
            this.DigitsLabel.Size = new System.Drawing.Size(38, 18);
            this.DigitsLabel.TabIndex = 0;
            this.DigitsLabel.Text = "digits";
            this.DigitsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.DigitsLabel, "digits");
            this.DigitsLabel.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            // 
            // HistoryBackwardLabel
            // 
            this.HistoryBackwardLabel.BackColor = System.Drawing.SystemColors.Control;
            this.HistoryBackwardLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.HistoryBackwardLabel.Enabled = false;
            this.HistoryBackwardLabel.FlatAppearance.BorderSize = 0;
            this.HistoryBackwardLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.HistoryBackwardLabel.Image = ((System.Drawing.Image)(resources.GetObject("HistoryBackwardLabel.Image")));
            this.HistoryBackwardLabel.Location = new System.Drawing.Point(2, 2);
            this.HistoryBackwardLabel.Name = "HistoryBackwardLabel";
            this.HistoryBackwardLabel.Size = new System.Drawing.Size(10, 18);
            this.HistoryBackwardLabel.TabIndex = 1;
            this.ToolTip.SetToolTip(this.HistoryBackwardLabel, "Back");
            this.HistoryBackwardLabel.UseVisualStyleBackColor = false;
            this.HistoryBackwardLabel.Click += new System.EventHandler(this.HistoryBackwardLabel_Click);
            // 
            // HistoryForewardLabel
            // 
            this.HistoryForewardLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.HistoryForewardLabel.BackColor = System.Drawing.SystemColors.Control;
            this.HistoryForewardLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.HistoryForewardLabel.Enabled = false;
            this.HistoryForewardLabel.FlatAppearance.BorderSize = 0;
            this.HistoryForewardLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.HistoryForewardLabel.Image = ((System.Drawing.Image)(resources.GetObject("HistoryForewardLabel.Image")));
            this.HistoryForewardLabel.Location = new System.Drawing.Point(160, 2);
            this.HistoryForewardLabel.Name = "HistoryForewardLabel";
            this.HistoryForewardLabel.Size = new System.Drawing.Size(14, 18);
            this.HistoryForewardLabel.TabIndex = 3;
            this.ToolTip.SetToolTip(this.HistoryForewardLabel, "Forward");
            this.HistoryForewardLabel.UseVisualStyleBackColor = false;
            this.HistoryForewardLabel.Click += new System.EventHandler(this.HistoryForewardLabel_Click);
            // 
            // VersionLabel
            // 
            this.VersionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.VersionLabel.BackColor = System.Drawing.SystemColors.Control;
            this.VersionLabel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.VersionLabel.ForeColor = System.Drawing.Color.Purple;
            this.VersionLabel.Location = new System.Drawing.Point(177, 363);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(63, 17);
            this.VersionLabel.TabIndex = 32;
            this.VersionLabel.Tag = "http://qurancode.com";
            this.VersionLabel.Text = "v6.19.114";
            this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.VersionLabel, "QuranCode");
            this.VersionLabel.Click += new System.EventHandler(this.LinkLabel_Click);
            // 
            // ToolTip
            // 
            this.ToolTip.AutomaticDelay = 100;
            this.ToolTip.AutoPopDelay = 5000;
            this.ToolTip.InitialDelay = 40;
            this.ToolTip.ReshowDelay = 20;
            // 
            // MultithreadingCheckBox
            // 
            this.MultithreadingCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MultithreadingCheckBox.AutoSize = true;
            this.MultithreadingCheckBox.Checked = true;
            this.MultithreadingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MultithreadingCheckBox.ForeColor = System.Drawing.Color.Purple;
            this.MultithreadingCheckBox.Location = new System.Drawing.Point(3, 364);
            this.MultithreadingCheckBox.Name = "MultithreadingCheckBox";
            this.MultithreadingCheckBox.Size = new System.Drawing.Size(15, 14);
            this.MultithreadingCheckBox.TabIndex = 30;
            this.ToolTip.SetToolTip(this.MultithreadingCheckBox, "Multi-threading");
            this.MultithreadingCheckBox.UseVisualStyleBackColor = true;
            this.MultithreadingCheckBox.CheckedChanged += new System.EventHandler(this.MultithreadingCheckBox_CheckedChanged);
            // 
            // SumOfDivisorsTextBox
            // 
            this.SumOfDivisorsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SumOfDivisorsTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.SumOfDivisorsTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SumOfDivisorsTextBox.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.SumOfDivisorsTextBox.Location = new System.Drawing.Point(108, 102);
            this.SumOfDivisorsTextBox.Name = "SumOfDivisorsTextBox";
            this.SumOfDivisorsTextBox.ReadOnly = true;
            this.SumOfDivisorsTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SumOfDivisorsTextBox.Size = new System.Drawing.Size(52, 20);
            this.SumOfDivisorsTextBox.TabIndex = 12;
            this.SumOfDivisorsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ToolTip.SetToolTip(this.SumOfDivisorsTextBox, "Sum of divisors");
            this.SumOfDivisorsTextBox.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            this.SumOfDivisorsTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // DisplayAbundantNumbersLable
            // 
            this.DisplayAbundantNumbersLable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.DisplayAbundantNumbersLable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DisplayAbundantNumbersLable.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DisplayAbundantNumbersLable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DisplayAbundantNumbersLable.ForeColor = System.Drawing.SystemColors.Window;
            this.DisplayAbundantNumbersLable.Location = new System.Drawing.Point(2, 115);
            this.DisplayAbundantNumbersLable.Name = "DisplayAbundantNumbersLable";
            this.DisplayAbundantNumbersLable.Size = new System.Drawing.Size(5, 5);
            this.DisplayAbundantNumbersLable.TabIndex = 17;
            this.DisplayAbundantNumbersLable.Tag = "";
            this.DisplayAbundantNumbersLable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ToolTip.SetToolTip(this.DisplayAbundantNumbersLable, "Display abundant numbers");
            this.DisplayAbundantNumbersLable.Click += new System.EventHandler(this.DisplayAbundantNumbersLable_Click);
            // 
            // DisplayPerfectNumbersLable
            // 
            this.DisplayPerfectNumbersLable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.DisplayPerfectNumbersLable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DisplayPerfectNumbersLable.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DisplayPerfectNumbersLable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DisplayPerfectNumbersLable.ForeColor = System.Drawing.SystemColors.Window;
            this.DisplayPerfectNumbersLable.Location = new System.Drawing.Point(2, 109);
            this.DisplayPerfectNumbersLable.Name = "DisplayPerfectNumbersLable";
            this.DisplayPerfectNumbersLable.Size = new System.Drawing.Size(5, 5);
            this.DisplayPerfectNumbersLable.TabIndex = 16;
            this.DisplayPerfectNumbersLable.Tag = "";
            this.DisplayPerfectNumbersLable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ToolTip.SetToolTip(this.DisplayPerfectNumbersLable, "Display perfect numbers");
            this.DisplayPerfectNumbersLable.Click += new System.EventHandler(this.DisplayPerfectNumbersLable_Click);
            // 
            // DisplayDeficientNumbersLable
            // 
            this.DisplayDeficientNumbersLable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.DisplayDeficientNumbersLable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DisplayDeficientNumbersLable.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DisplayDeficientNumbersLable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DisplayDeficientNumbersLable.ForeColor = System.Drawing.SystemColors.Window;
            this.DisplayDeficientNumbersLable.Location = new System.Drawing.Point(2, 103);
            this.DisplayDeficientNumbersLable.Name = "DisplayDeficientNumbersLable";
            this.DisplayDeficientNumbersLable.Size = new System.Drawing.Size(5, 5);
            this.DisplayDeficientNumbersLable.TabIndex = 15;
            this.DisplayDeficientNumbersLable.Tag = "";
            this.DisplayDeficientNumbersLable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ToolTip.SetToolTip(this.DisplayDeficientNumbersLable, "Display deficient numbers");
            this.DisplayDeficientNumbersLable.Click += new System.EventHandler(this.DisplayDeficientNumbersLable_Click);
            // 
            // NumberKindIndexTextBox
            // 
            this.NumberKindIndexTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NumberKindIndexTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.NumberKindIndexTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumberKindIndexTextBox.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.NumberKindIndexTextBox.Location = new System.Drawing.Point(6, 102);
            this.NumberKindIndexTextBox.Name = "NumberKindIndexTextBox";
            this.NumberKindIndexTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.NumberKindIndexTextBox.Size = new System.Drawing.Size(52, 20);
            this.NumberKindIndexTextBox.TabIndex = 10;
            this.NumberKindIndexTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ToolTip.SetToolTip(this.NumberKindIndexTextBox, "Deficient number index");
            this.NumberKindIndexTextBox.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            this.NumberKindIndexTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NumberKindIndexTextBox_KeyDown);
            this.NumberKindIndexTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // SumOfProperDivisorsTextBox
            // 
            this.SumOfProperDivisorsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SumOfProperDivisorsTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.SumOfProperDivisorsTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SumOfProperDivisorsTextBox.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.SumOfProperDivisorsTextBox.Location = new System.Drawing.Point(57, 102);
            this.SumOfProperDivisorsTextBox.Name = "SumOfProperDivisorsTextBox";
            this.SumOfProperDivisorsTextBox.ReadOnly = true;
            this.SumOfProperDivisorsTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SumOfProperDivisorsTextBox.Size = new System.Drawing.Size(52, 20);
            this.SumOfProperDivisorsTextBox.TabIndex = 11;
            this.SumOfProperDivisorsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ToolTip.SetToolTip(this.SumOfProperDivisorsTextBox, "Sum of proper divisors");
            this.SumOfProperDivisorsTextBox.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            this.SumOfProperDivisorsTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // PCIndexChainLabel
            // 
            this.PCIndexChainLabel.BackColor = System.Drawing.Color.Wheat;
            this.PCIndexChainLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PCIndexChainLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PCIndexChainLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PCIndexChainLabel.ForeColor = System.Drawing.SystemColors.Window;
            this.PCIndexChainLabel.Location = new System.Drawing.Point(2, 122);
            this.PCIndexChainLabel.Name = "PCIndexChainLabel";
            this.PCIndexChainLabel.Size = new System.Drawing.Size(5, 20);
            this.PCIndexChainLabel.TabIndex = 23;
            this.PCIndexChainLabel.Tag = "http://eng.bu.ac.th/bucroccs/index.php/research/14-people-detail/19-dr-waleed-s-m" +
    "ohammed";
            this.PCIndexChainLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ToolTip.SetToolTip(this.PCIndexChainLabel, "©2016 Dr Waleed S. Mohammed");
            this.PCIndexChainLabel.Click += new System.EventHandler(this.LinkLabel_Click);
            // 
            // HistoryClearLabel
            // 
            this.HistoryClearLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.HistoryClearLabel.BackColor = System.Drawing.Color.LightCoral;
            this.HistoryClearLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.HistoryClearLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HistoryClearLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.HistoryClearLabel.Location = new System.Drawing.Point(232, 3);
            this.HistoryClearLabel.Name = "HistoryClearLabel";
            this.HistoryClearLabel.Size = new System.Drawing.Size(6, 19);
            this.HistoryClearLabel.TabIndex = 229;
            this.HistoryClearLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.HistoryClearLabel, "Clear");
            this.HistoryClearLabel.Click += new System.EventHandler(this.HistoryClearLabel_Click);
            // 
            // HistoryDeleteLabel
            // 
            this.HistoryDeleteLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.HistoryDeleteLabel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.HistoryDeleteLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.HistoryDeleteLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HistoryDeleteLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.HistoryDeleteLabel.Image = ((System.Drawing.Image)(resources.GetObject("HistoryDeleteLabel.Image")));
            this.HistoryDeleteLabel.Location = new System.Drawing.Point(214, 3);
            this.HistoryDeleteLabel.Name = "HistoryDeleteLabel";
            this.HistoryDeleteLabel.Size = new System.Drawing.Size(18, 19);
            this.HistoryDeleteLabel.TabIndex = 228;
            this.HistoryDeleteLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.HistoryDeleteLabel, "Delete");
            this.HistoryDeleteLabel.Click += new System.EventHandler(this.HistoryDeleteLabel_Click);
            // 
            // ValuePanel
            // 
            this.ValuePanel.Controls.Add(this.HistoryClearLabel);
            this.ValuePanel.Controls.Add(this.HistoryDeleteLabel);
            this.ValuePanel.Controls.Add(this.PCIndexChainLabel);
            this.ValuePanel.Controls.Add(this.SumOfDivisorsTextBox);
            this.ValuePanel.Controls.Add(this.SquareDiffTextBox);
            this.ValuePanel.Controls.Add(this.SquareSumTextBox);
            this.ValuePanel.Controls.Add(this.IndexChainLengthTextBox);
            this.ValuePanel.Controls.Add(this.NthNonAdditiveNumberTextBox);
            this.ValuePanel.Controls.Add(this.DigitsLabel);
            this.ValuePanel.Controls.Add(this.CPIndexChainL2RTextBox);
            this.ValuePanel.Controls.Add(this.DisplayAbundantNumbersLable);
            this.ValuePanel.Controls.Add(this.DisplayPerfectNumbersLable);
            this.ValuePanel.Controls.Add(this.PCIndexChainR2LTextBox);
            this.ValuePanel.Controls.Add(this.HistoryForewardLabel);
            this.ValuePanel.Controls.Add(this.HistoryBackwardLabel);
            this.ValuePanel.Controls.Add(this.CPIndexChainR2LTextBox);
            this.ValuePanel.Controls.Add(this.PCIndexChainL2RTextBox);
            this.ValuePanel.Controls.Add(this.DisplayDeficientNumbersLable);
            this.ValuePanel.Controls.Add(this.ValueTextBox);
            this.ValuePanel.Controls.Add(this.NumberKindIndexTextBox);
            this.ValuePanel.Controls.Add(this.SumOfProperDivisorsTextBox);
            this.ValuePanel.Controls.Add(this.DigitalRootTextBox);
            this.ValuePanel.Controls.Add(this.DigitSumTextBox);
            this.ValuePanel.Controls.Add(this.NthAdditiveNumberTextBox);
            this.ValuePanel.Controls.Add(this.NthNumberTextBox);
            this.ValuePanel.Controls.Add(this.AddToSeparatorLine2Panel);
            this.ValuePanel.Controls.Add(this.PrimeFactorsTextBox);
            this.ValuePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ValuePanel.Location = new System.Drawing.Point(0, 0);
            this.ValuePanel.Name = "ValuePanel";
            this.ValuePanel.Size = new System.Drawing.Size(240, 146);
            this.ValuePanel.TabIndex = 12;
            // 
            // SquareDiffTextBox
            // 
            this.SquareDiffTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SquareDiffTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.SquareDiffTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SquareDiffTextBox.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.SquareDiffTextBox.Location = new System.Drawing.Point(1, 62);
            this.SquareDiffTextBox.Name = "SquareDiffTextBox";
            this.SquareDiffTextBox.ReadOnly = true;
            this.SquareDiffTextBox.Size = new System.Drawing.Size(237, 20);
            this.SquareDiffTextBox.TabIndex = 6;
            this.SquareDiffTextBox.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            this.SquareDiffTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // SquareSumTextBox
            // 
            this.SquareSumTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SquareSumTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.SquareSumTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SquareSumTextBox.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.SquareSumTextBox.Location = new System.Drawing.Point(1, 42);
            this.SquareSumTextBox.Name = "SquareSumTextBox";
            this.SquareSumTextBox.ReadOnly = true;
            this.SquareSumTextBox.Size = new System.Drawing.Size(237, 20);
            this.SquareSumTextBox.TabIndex = 5;
            this.SquareSumTextBox.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            this.SquareSumTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // IndexChainLengthTextBox
            // 
            this.IndexChainLengthTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IndexChainLengthTextBox.BackColor = System.Drawing.SystemColors.Info;
            this.IndexChainLengthTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.IndexChainLengthTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IndexChainLengthTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.IndexChainLengthTextBox.Location = new System.Drawing.Point(210, 122);
            this.IndexChainLengthTextBox.Name = "IndexChainLengthTextBox";
            this.IndexChainLengthTextBox.ReadOnly = true;
            this.IndexChainLengthTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.IndexChainLengthTextBox.Size = new System.Drawing.Size(28, 20);
            this.IndexChainLengthTextBox.TabIndex = 22;
            this.IndexChainLengthTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.IndexChainLengthTextBox.Click += new System.EventHandler(this.IndexChainLengthTextBox_Click);
            this.IndexChainLengthTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // NthNonAdditiveNumberTextBox
            // 
            this.NthNonAdditiveNumberTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NthNonAdditiveNumberTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.NthNonAdditiveNumberTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NthNonAdditiveNumberTextBox.Location = new System.Drawing.Point(159, 82);
            this.NthNonAdditiveNumberTextBox.Name = "NthNonAdditiveNumberTextBox";
            this.NthNonAdditiveNumberTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.NthNonAdditiveNumberTextBox.Size = new System.Drawing.Size(79, 20);
            this.NthNonAdditiveNumberTextBox.TabIndex = 9;
            this.NthNonAdditiveNumberTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.NthNonAdditiveNumberTextBox.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            this.NthNonAdditiveNumberTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NthNonAdditiveNumberTextBox_KeyDown);
            this.NthNonAdditiveNumberTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // CPIndexChainL2RTextBox
            // 
            this.CPIndexChainL2RTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CPIndexChainL2RTextBox.BackColor = System.Drawing.SystemColors.Info;
            this.CPIndexChainL2RTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CPIndexChainL2RTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.CPIndexChainL2RTextBox.Location = new System.Drawing.Point(108, 122);
            this.CPIndexChainL2RTextBox.Name = "CPIndexChainL2RTextBox";
            this.CPIndexChainL2RTextBox.ReadOnly = true;
            this.CPIndexChainL2RTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CPIndexChainL2RTextBox.Size = new System.Drawing.Size(52, 20);
            this.CPIndexChainL2RTextBox.TabIndex = 20;
            this.CPIndexChainL2RTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.CPIndexChainL2RTextBox.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            this.CPIndexChainL2RTextBox.TextChanged += new System.EventHandler(this.CPIndexChainL2RTextBox_TextChanged);
            this.CPIndexChainL2RTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // PCIndexChainR2LTextBox
            // 
            this.PCIndexChainR2LTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PCIndexChainR2LTextBox.BackColor = System.Drawing.SystemColors.Info;
            this.PCIndexChainR2LTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PCIndexChainR2LTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.PCIndexChainR2LTextBox.Location = new System.Drawing.Point(57, 122);
            this.PCIndexChainR2LTextBox.Name = "PCIndexChainR2LTextBox";
            this.PCIndexChainR2LTextBox.ReadOnly = true;
            this.PCIndexChainR2LTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PCIndexChainR2LTextBox.Size = new System.Drawing.Size(52, 20);
            this.PCIndexChainR2LTextBox.TabIndex = 19;
            this.PCIndexChainR2LTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.PCIndexChainR2LTextBox.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            this.PCIndexChainR2LTextBox.TextChanged += new System.EventHandler(this.PCIndexChainR2LTextBox_TextChanged);
            this.PCIndexChainR2LTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // CPIndexChainR2LTextBox
            // 
            this.CPIndexChainR2LTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CPIndexChainR2LTextBox.BackColor = System.Drawing.SystemColors.Info;
            this.CPIndexChainR2LTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CPIndexChainR2LTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.CPIndexChainR2LTextBox.Location = new System.Drawing.Point(159, 122);
            this.CPIndexChainR2LTextBox.Name = "CPIndexChainR2LTextBox";
            this.CPIndexChainR2LTextBox.ReadOnly = true;
            this.CPIndexChainR2LTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CPIndexChainR2LTextBox.Size = new System.Drawing.Size(52, 20);
            this.CPIndexChainR2LTextBox.TabIndex = 21;
            this.CPIndexChainR2LTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.CPIndexChainR2LTextBox.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            this.CPIndexChainR2LTextBox.TextChanged += new System.EventHandler(this.CPIndexChainR2LTextBox_TextChanged);
            this.CPIndexChainR2LTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // PCIndexChainL2RTextBox
            // 
            this.PCIndexChainL2RTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PCIndexChainL2RTextBox.BackColor = System.Drawing.SystemColors.Info;
            this.PCIndexChainL2RTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PCIndexChainL2RTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.PCIndexChainL2RTextBox.Location = new System.Drawing.Point(6, 122);
            this.PCIndexChainL2RTextBox.Name = "PCIndexChainL2RTextBox";
            this.PCIndexChainL2RTextBox.ReadOnly = true;
            this.PCIndexChainL2RTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PCIndexChainL2RTextBox.Size = new System.Drawing.Size(52, 20);
            this.PCIndexChainL2RTextBox.TabIndex = 18;
            this.PCIndexChainL2RTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.PCIndexChainL2RTextBox.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            this.PCIndexChainL2RTextBox.TextChanged += new System.EventHandler(this.PCIndexChainL2RTextBox_TextChanged);
            this.PCIndexChainL2RTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // ValueTextBox
            // 
            this.ValueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ValueTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.ValueTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ValueTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.ValueTextBox.Location = new System.Drawing.Point(16, 3);
            this.ValueTextBox.Name = "ValueTextBox";
            this.ValueTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ValueTextBox.Size = new System.Drawing.Size(144, 20);
            this.ValueTextBox.TabIndex = 2;
            this.ValueTextBox.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            this.ValueTextBox.TextChanged += new System.EventHandler(this.ValueTextBox_TextChanged);
            this.ValueTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ValueTextBox_KeyDown);
            this.ValueTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // DigitalRootTextBox
            // 
            this.DigitalRootTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DigitalRootTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.DigitalRootTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DigitalRootTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.DigitalRootTextBox.Location = new System.Drawing.Point(210, 102);
            this.DigitalRootTextBox.Name = "DigitalRootTextBox";
            this.DigitalRootTextBox.ReadOnly = true;
            this.DigitalRootTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.DigitalRootTextBox.Size = new System.Drawing.Size(28, 20);
            this.DigitalRootTextBox.TabIndex = 14;
            this.DigitalRootTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.DigitalRootTextBox.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            this.DigitalRootTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // DigitSumTextBox
            // 
            this.DigitSumTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DigitSumTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.DigitSumTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DigitSumTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.DigitSumTextBox.Location = new System.Drawing.Point(159, 102);
            this.DigitSumTextBox.Name = "DigitSumTextBox";
            this.DigitSumTextBox.ReadOnly = true;
            this.DigitSumTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.DigitSumTextBox.Size = new System.Drawing.Size(52, 20);
            this.DigitSumTextBox.TabIndex = 13;
            this.DigitSumTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.DigitSumTextBox.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            this.DigitSumTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // NthAdditiveNumberTextBox
            // 
            this.NthAdditiveNumberTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NthAdditiveNumberTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.NthAdditiveNumberTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NthAdditiveNumberTextBox.Location = new System.Drawing.Point(80, 82);
            this.NthAdditiveNumberTextBox.Name = "NthAdditiveNumberTextBox";
            this.NthAdditiveNumberTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.NthAdditiveNumberTextBox.Size = new System.Drawing.Size(79, 20);
            this.NthAdditiveNumberTextBox.TabIndex = 8;
            this.NthAdditiveNumberTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.NthAdditiveNumberTextBox.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            this.NthAdditiveNumberTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NthAdditiveNumberTextBox_KeyDown);
            this.NthAdditiveNumberTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // NthNumberTextBox
            // 
            this.NthNumberTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NthNumberTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.NthNumberTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NthNumberTextBox.Location = new System.Drawing.Point(1, 82);
            this.NthNumberTextBox.Name = "NthNumberTextBox";
            this.NthNumberTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.NthNumberTextBox.Size = new System.Drawing.Size(79, 20);
            this.NthNumberTextBox.TabIndex = 7;
            this.NthNumberTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.NthNumberTextBox.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            this.NthNumberTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NthNumberTextBox_KeyDown);
            this.NthNumberTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // AddToSeparatorLine2Panel
            // 
            this.AddToSeparatorLine2Panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AddToSeparatorLine2Panel.BackColor = System.Drawing.SystemColors.Window;
            this.AddToSeparatorLine2Panel.Location = new System.Drawing.Point(1, 142);
            this.AddToSeparatorLine2Panel.Name = "AddToSeparatorLine2Panel";
            this.AddToSeparatorLine2Panel.Size = new System.Drawing.Size(239, 1);
            this.AddToSeparatorLine2Panel.TabIndex = 227;
            // 
            // PrimeFactorsTextBox
            // 
            this.PrimeFactorsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PrimeFactorsTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.PrimeFactorsTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PrimeFactorsTextBox.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.PrimeFactorsTextBox.Location = new System.Drawing.Point(1, 22);
            this.PrimeFactorsTextBox.Name = "PrimeFactorsTextBox";
            this.PrimeFactorsTextBox.ReadOnly = true;
            this.PrimeFactorsTextBox.Size = new System.Drawing.Size(237, 20);
            this.PrimeFactorsTextBox.TabIndex = 4;
            this.PrimeFactorsTextBox.Click += new System.EventHandler(this.TextBoxLabelControls_CtrlClick);
            this.PrimeFactorsTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FixMicrosoft);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 380);
            this.Controls.Add(this.ValuePanel);
            this.Controls.Add(this.MultithreadingCheckBox);
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.WebsiteLabel);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.ProgressValueLabel);
            this.Controls.Add(this.MilliSecondsLabel);
            this.Controls.Add(this.ElapsedTimeValueLabel);
            this.Controls.Add(this.ProgressLabel);
            this.Controls.Add(this.ElapsedTimeLabel);
            this.Controls.Add(this.OutputTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Menu = this.MainMenu;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Prime Calculator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.NotifyIconContextMenuStrip.ResumeLayout(false);
            this.ValuePanel.ResumeLayout(false);
            this.ValuePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label ElapsedTimeLabel;
    private System.Windows.Forms.Label ProgressLabel;
    private System.Windows.Forms.Label ElapsedTimeValueLabel;
    private System.Windows.Forms.Label MilliSecondsLabel;
    private System.Windows.Forms.Label ProgressValueLabel;
    private System.Windows.Forms.TextBox OutputTextBox;
    private System.Windows.Forms.MainMenu MainMenu;
    private System.Windows.Forms.ProgressBar ProgressBar;
    private System.Windows.Forms.Label WebsiteLabel;
    private System.Windows.Forms.ContextMenuStrip NotifyIconContextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
    private System.Windows.Forms.NotifyIcon NotifyIcon;
    private System.Windows.Forms.Label DigitsLabel;
    private System.Windows.Forms.Button HistoryBackwardLabel;
    private System.Windows.Forms.Button HistoryForewardLabel;
    private System.Windows.Forms.Label VersionLabel;
    private System.Windows.Forms.ToolTip ToolTip;
    private System.Windows.Forms.CheckBox MultithreadingCheckBox;
    private System.Windows.Forms.Panel ValuePanel;
    private System.Windows.Forms.TextBox SumOfDivisorsTextBox;
    private System.Windows.Forms.TextBox SquareDiffTextBox;
    private System.Windows.Forms.TextBox SquareSumTextBox;
    private System.Windows.Forms.TextBox NthNonAdditiveNumberTextBox;
    private System.Windows.Forms.Label DisplayAbundantNumbersLable;
    private System.Windows.Forms.Label DisplayPerfectNumbersLable;
    private System.Windows.Forms.Label DisplayDeficientNumbersLable;
    private System.Windows.Forms.TextBox ValueTextBox;
    private System.Windows.Forms.TextBox NumberKindIndexTextBox;
    private System.Windows.Forms.TextBox SumOfProperDivisorsTextBox;
    private System.Windows.Forms.Label PCIndexChainLabel;
    private System.Windows.Forms.TextBox IndexChainLengthTextBox;
    private System.Windows.Forms.TextBox CPIndexChainL2RTextBox;
    private System.Windows.Forms.TextBox PCIndexChainR2LTextBox;
    private System.Windows.Forms.TextBox CPIndexChainR2LTextBox;
    private System.Windows.Forms.TextBox PCIndexChainL2RTextBox;
    private System.Windows.Forms.TextBox DigitalRootTextBox;
    private System.Windows.Forms.TextBox DigitSumTextBox;
    private System.Windows.Forms.TextBox NthAdditiveNumberTextBox;
    private System.Windows.Forms.TextBox NthNumberTextBox;
    private System.Windows.Forms.Panel AddToSeparatorLine2Panel;
    private System.Windows.Forms.TextBox PrimeFactorsTextBox;
    private System.Windows.Forms.Label HistoryClearLabel;
    private System.Windows.Forms.Label HistoryDeleteLabel;
}
