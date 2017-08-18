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
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.ProgressValueLabel = new System.Windows.Forms.Label();
            this.MainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.WebsiteLabel = new System.Windows.Forms.Label();
            this.NotifyIconContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.VersionLabel = new System.Windows.Forms.Label();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.PLabel = new System.Windows.Forms.Label();
            this.APLabel = new System.Windows.Forms.Label();
            this.XPLabel = new System.Windows.Forms.Label();
            this.XCLabel = new System.Windows.Forms.Label();
            this.ACLabel = new System.Windows.Forms.Label();
            this.CLabel = new System.Windows.Forms.Label();
            this.ABLabel = new System.Windows.Forms.Label();
            this.DFLabel = new System.Windows.Forms.Label();
            this.IndexLabel = new System.Windows.Forms.Label();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.MainTabPage = new System.Windows.Forms.TabPage();
            this.NotifyIconContextMenuStrip.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.MainTabControl.SuspendLayout();
            this.MainTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ProgressLabel.BackColor = System.Drawing.SystemColors.Info;
            this.ProgressLabel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.ProgressLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.ProgressLabel.Location = new System.Drawing.Point(0, 314);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(56, 16);
            this.ProgressLabel.TabIndex = 25;
            this.ProgressLabel.Text = "Progress";
            this.ProgressLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ProgressLabel.Visible = false;
            // 
            // ProgressValueLabel
            // 
            this.ProgressValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ProgressValueLabel.BackColor = System.Drawing.SystemColors.ControlText;
            this.ProgressValueLabel.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.ProgressValueLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.ProgressValueLabel.Location = new System.Drawing.Point(0, 328);
            this.ProgressValueLabel.Name = "ProgressValueLabel";
            this.ProgressValueLabel.Size = new System.Drawing.Size(56, 17);
            this.ProgressValueLabel.TabIndex = 27;
            this.ProgressValueLabel.Text = "0%";
            this.ProgressValueLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ProgressValueLabel.Visible = false;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBar.Location = new System.Drawing.Point(0, 349);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(504, 6);
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
            this.WebsiteLabel.Location = new System.Drawing.Point(0, 356);
            this.WebsiteLabel.Name = "WebsiteLabel";
            this.WebsiteLabel.Size = new System.Drawing.Size(584, 16);
            this.WebsiteLabel.TabIndex = 999;
            this.WebsiteLabel.Tag = "http://qurancode.com";
            this.WebsiteLabel.Text = "©2017  Ali Adams                         www.heliwave.com                        " +
    "     www.qurancode.com                                           ";
            this.WebsiteLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ToolTip.SetToolTip(this.WebsiteLabel, "Quran 89:3 \"By the composites and the primes\".");
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
            // VersionLabel
            // 
            this.VersionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.VersionLabel.BackColor = System.Drawing.SystemColors.Control;
            this.VersionLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.VersionLabel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.VersionLabel.ForeColor = System.Drawing.Color.Purple;
            this.VersionLabel.Location = new System.Drawing.Point(521, 354);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(63, 17);
            this.VersionLabel.TabIndex = 32;
            this.VersionLabel.Tag = "http://heliwave.com/114.txt";
            this.VersionLabel.Text = "v6.19.114";
            this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.VersionLabel, "114 Amazing Numbers");
            this.VersionLabel.Click += new System.EventHandler(this.LinkLabel_Click);
            // 
            // ToolTip
            // 
            this.ToolTip.AutomaticDelay = 100;
            this.ToolTip.AutoPopDelay = 10000;
            this.ToolTip.InitialDelay = 40;
            this.ToolTip.ReshowDelay = 20;
            // 
            // PLabel
            // 
            this.PLabel.BackColor = System.Drawing.SystemColors.Control;
            this.PLabel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.PLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.PLabel.Location = new System.Drawing.Point(88, 1);
            this.PLabel.Name = "PLabel";
            this.PLabel.Size = new System.Drawing.Size(30, 16);
            this.PLabel.TabIndex = 15;
            this.PLabel.Text = "P";
            this.PLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.PLabel, "Prime Number = Divisible by self only");
            // 
            // APLabel
            // 
            this.APLabel.BackColor = System.Drawing.SystemColors.Control;
            this.APLabel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.APLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.APLabel.Location = new System.Drawing.Point(148, 1);
            this.APLabel.Name = "APLabel";
            this.APLabel.Size = new System.Drawing.Size(30, 16);
            this.APLabel.TabIndex = 17;
            this.APLabel.Text = "AP";
            this.APLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.APLabel, "Additive Prime = Prime with prime digit sum");
            // 
            // XPLabel
            // 
            this.XPLabel.BackColor = System.Drawing.SystemColors.Control;
            this.XPLabel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.XPLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.XPLabel.Location = new System.Drawing.Point(210, 1);
            this.XPLabel.Name = "XPLabel";
            this.XPLabel.Size = new System.Drawing.Size(30, 16);
            this.XPLabel.TabIndex = 19;
            this.XPLabel.Text = "XP";
            this.XPLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.XPLabel, "Non-additive Prime = Prime with non-prime digit sum");
            // 
            // XCLabel
            // 
            this.XCLabel.BackColor = System.Drawing.SystemColors.Control;
            this.XCLabel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.XCLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.XCLabel.Location = new System.Drawing.Point(399, 1);
            this.XCLabel.Name = "XCLabel";
            this.XCLabel.Size = new System.Drawing.Size(30, 16);
            this.XCLabel.TabIndex = 25;
            this.XCLabel.Text = "XC";
            this.XCLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.XCLabel, "Non-additive Composite = Composite with non-composite digit sum");
            // 
            // ACLabel
            // 
            this.ACLabel.BackColor = System.Drawing.SystemColors.Control;
            this.ACLabel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.ACLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.ACLabel.Location = new System.Drawing.Point(337, 1);
            this.ACLabel.Name = "ACLabel";
            this.ACLabel.Size = new System.Drawing.Size(30, 16);
            this.ACLabel.TabIndex = 23;
            this.ACLabel.Text = "AC";
            this.ACLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.ACLabel, "Additive Composite = Composite with composite digit sum");
            // 
            // CLabel
            // 
            this.CLabel.BackColor = System.Drawing.SystemColors.Control;
            this.CLabel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.CLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.CLabel.Location = new System.Drawing.Point(272, 1);
            this.CLabel.Name = "CLabel";
            this.CLabel.Size = new System.Drawing.Size(30, 16);
            this.CLabel.TabIndex = 21;
            this.CLabel.Text = "C";
            this.CLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.CLabel, "Composite Number = Divisible by self and others");
            // 
            // ABLabel
            // 
            this.ABLabel.BackColor = System.Drawing.SystemColors.Control;
            this.ABLabel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.ABLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.ABLabel.Location = new System.Drawing.Point(516, 1);
            this.ABLabel.Name = "ABLabel";
            this.ABLabel.Size = new System.Drawing.Size(30, 16);
            this.ABLabel.TabIndex = 31;
            this.ABLabel.Text = "AB";
            this.ABLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.ABLabel, "Abundant Number > Sum of proper divisors");
            // 
            // DFLabel
            // 
            this.DFLabel.BackColor = System.Drawing.SystemColors.Control;
            this.DFLabel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.DFLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.DFLabel.Location = new System.Drawing.Point(458, 1);
            this.DFLabel.Name = "DFLabel";
            this.DFLabel.Size = new System.Drawing.Size(30, 16);
            this.DFLabel.TabIndex = 30;
            this.DFLabel.Text = "DF";
            this.DFLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.DFLabel, "Deficient Number < Sum of proper divisors");
            // 
            // IndexLabel
            // 
            this.IndexLabel.BackColor = System.Drawing.SystemColors.Control;
            this.IndexLabel.Font = new System.Drawing.Font("Tahoma", 10F);
            this.IndexLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.IndexLabel.Location = new System.Drawing.Point(27, 1);
            this.IndexLabel.Name = "IndexLabel";
            this.IndexLabel.Size = new System.Drawing.Size(30, 16);
            this.IndexLabel.TabIndex = 27;
            this.IndexLabel.Text = "#";
            this.IndexLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.IndexLabel, "Index");
            // 
            // MainPanel
            // 
            this.MainPanel.BackColor = System.Drawing.SystemColors.Control;
            this.MainPanel.Controls.Add(this.ABLabel);
            this.MainPanel.Controls.Add(this.DFLabel);
            this.MainPanel.Controls.Add(this.XCLabel);
            this.MainPanel.Controls.Add(this.ACLabel);
            this.MainPanel.Controls.Add(this.CLabel);
            this.MainPanel.Controls.Add(this.XPLabel);
            this.MainPanel.Controls.Add(this.APLabel);
            this.MainPanel.Controls.Add(this.PLabel);
            this.MainPanel.Controls.Add(this.IndexLabel);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(3, 3);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(570, 324);
            this.MainPanel.TabIndex = 33;
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.MainTabPage);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Location = new System.Drawing.Point(0, 0);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(584, 356);
            this.MainTabControl.TabIndex = 1;
            // 
            // MainTabPage
            // 
            this.MainTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.MainTabPage.Controls.Add(this.MainPanel);
            this.MainTabPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainTabPage.Location = new System.Drawing.Point(4, 22);
            this.MainTabPage.Name = "MainTabPage";
            this.MainTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.MainTabPage.Size = new System.Drawing.Size(576, 330);
            this.MainTabPage.TabIndex = 1;
            this.MainTabPage.Text = "D i s c o v e r   h i d d e n   c o n n e c t i o n s   b e t w e e n   p r i m e" +
    " s ,  c o m p o s i t e s  a n d   t h e i r   i n d i c e s .";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 372);
            this.Controls.Add(this.MainTabControl);
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.WebsiteLabel);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.ProgressValueLabel);
            this.Controls.Add(this.ProgressLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(600, 410);
            this.Menu = this.MainMenu;
            this.MinimumSize = new System.Drawing.Size(600, 410);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Numbers are the DNA that controls the materialization of light to form our Univer" +
    "se.";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.NotifyIconContextMenuStrip.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
            this.MainTabControl.ResumeLayout(false);
            this.MainTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label ProgressLabel;
    private System.Windows.Forms.Label ProgressValueLabel;
    private System.Windows.Forms.MainMenu MainMenu;
    private System.Windows.Forms.ProgressBar ProgressBar;
    private System.Windows.Forms.Label WebsiteLabel;
    private System.Windows.Forms.ContextMenuStrip NotifyIconContextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
    private System.Windows.Forms.NotifyIcon NotifyIcon;
    private System.Windows.Forms.Label VersionLabel;
    private System.Windows.Forms.ToolTip ToolTip;
    private System.Windows.Forms.Panel MainPanel;
    private System.Windows.Forms.Label XCLabel;
    private System.Windows.Forms.Label ACLabel;
    private System.Windows.Forms.Label CLabel;
    private System.Windows.Forms.Label XPLabel;
    private System.Windows.Forms.Label APLabel;
    private System.Windows.Forms.Label PLabel;
    private System.Windows.Forms.Label IndexLabel;
    private System.Windows.Forms.Label ABLabel;
    private System.Windows.Forms.Label DFLabel;
    private System.Windows.Forms.TabControl MainTabControl;
    private System.Windows.Forms.TabPage MainTabPage;
}
