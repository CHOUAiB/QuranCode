namespace InitialLetters
{
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
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.TypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UniqueLettersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UniqueWordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AllWordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.ListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.LettersTextBox = new System.Windows.Forms.TextBox();
            this.ResultPanel = new System.Windows.Forms.Panel();
            this.ResultLabel = new System.Windows.Forms.Label();
            this.ElapsedTimeLabel = new System.Windows.Forms.Label();
            this.InitialLettersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuStrip.SuspendLayout();
            this.ResultPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TypeToolStripMenuItem,
            this.RunToolStripMenuItem,
            this.ViewToolStripMenuItem,
            this.HelpToolStripMenuItem});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(236, 24);
            this.MenuStrip.TabIndex = 0;
            this.MenuStrip.Text = "MenuStrip";
            // 
            // TypeToolStripMenuItem
            // 
            this.TypeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UniqueLettersToolStripMenuItem,
            this.UniqueWordsToolStripMenuItem,
            this.AllWordsToolStripMenuItem});
            this.TypeToolStripMenuItem.Name = "TypeToolStripMenuItem";
            this.TypeToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.TypeToolStripMenuItem.Text = "&Type";
            // 
            // UniqueLettersToolStripMenuItem
            // 
            this.UniqueLettersToolStripMenuItem.Name = "UniqueLettersToolStripMenuItem";
            this.UniqueLettersToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.UniqueLettersToolStripMenuItem.Text = "Unique &Letters";
            this.UniqueLettersToolStripMenuItem.Click += new System.EventHandler(this.TypeUniqueLettersToolStripMenuItem_Click);
            // 
            // UniqueWordsToolStripMenuItem
            // 
            this.UniqueWordsToolStripMenuItem.Name = "UniqueWordsToolStripMenuItem";
            this.UniqueWordsToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.UniqueWordsToolStripMenuItem.Text = "Unique &Words";
            this.UniqueWordsToolStripMenuItem.Click += new System.EventHandler(this.TypeUniqueWordsToolStripMenuItem_Click);
            // 
            // AllWordsToolStripMenuItem
            // 
            this.AllWordsToolStripMenuItem.Name = "AllWordsToolStripMenuItem";
            this.AllWordsToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.AllWordsToolStripMenuItem.Text = "&All Words";
            this.AllWordsToolStripMenuItem.Click += new System.EventHandler(this.TypeAllWordsToolStripMenuItem_Click);
            // 
            // RunToolStripMenuItem
            // 
            this.RunToolStripMenuItem.Name = "RunToolStripMenuItem";
            this.RunToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.RunToolStripMenuItem.Text = "&Run";
            this.RunToolStripMenuItem.Click += new System.EventHandler(this.RunToolStripMenuItem_Click);
            // 
            // ViewToolStripMenuItem
            // 
            this.ViewToolStripMenuItem.Enabled = false;
            this.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem";
            this.ViewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.ViewToolStripMenuItem.Text = "&View";
            this.ViewToolStripMenuItem.Click += new System.EventHandler(this.ViewToolStripMenuItem_Click);
            // 
            // HelpToolStripMenuItem
            // 
            this.HelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.InitialLettersToolStripMenuItem,
            this.toolStripMenuItem1,
            this.AboutToolStripMenuItem});
            this.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            this.HelpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.HelpToolStripMenuItem.Text = "&Help";
            // 
            // AboutToolStripMenuItem
            // 
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            this.AboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.AboutToolStripMenuItem.Text = "&About...";
            this.AboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // Timer
            // 
            this.Timer.Interval = 1000;
            this.Timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // ListView
            // 
            this.ListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.ListView.Enabled = false;
            this.ListView.Font = new System.Drawing.Font("Lucida Console", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListView.Location = new System.Drawing.Point(0, 24);
            this.ListView.Name = "ListView";
            this.ListView.Size = new System.Drawing.Size(236, 349);
            this.ListView.TabIndex = 1;
            this.ListView.TabStop = false;
            this.ListView.UseCompatibleStateImageBehavior = false;
            this.ListView.View = System.Windows.Forms.View.Details;
            this.ListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListView_SortColumnClick);
            this.ListView.SelectedIndexChanged += new System.EventHandler(this.ListView_SelectedIndexChanged);
            this.ListView.Resize += new System.EventHandler(this.ListView_Resize);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 265;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ProgressBar.Location = new System.Drawing.Point(0, 418);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(236, 12);
            this.ProgressBar.Step = 1;
            this.ProgressBar.TabIndex = 0;
            // 
            // LettersTextBox
            // 
            this.LettersTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.LettersTextBox.Enabled = false;
            this.LettersTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LettersTextBox.Location = new System.Drawing.Point(0, 395);
            this.LettersTextBox.Name = "LettersTextBox";
            this.LettersTextBox.Size = new System.Drawing.Size(236, 23);
            this.LettersTextBox.TabIndex = 2;
            this.LettersTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LettersTextBox_KeyPress);
            // 
            // ResultPanel
            // 
            this.ResultPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResultPanel.Controls.Add(this.ResultLabel);
            this.ResultPanel.Controls.Add(this.ElapsedTimeLabel);
            this.ResultPanel.Location = new System.Drawing.Point(0, 373);
            this.ResultPanel.Name = "ResultPanel";
            this.ResultPanel.Size = new System.Drawing.Size(236, 21);
            this.ResultPanel.TabIndex = 20;
            // 
            // ResultLabel
            // 
            this.ResultLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ResultLabel.AutoSize = true;
            this.ResultLabel.Location = new System.Drawing.Point(1, 4);
            this.ResultLabel.Name = "ResultLabel";
            this.ResultLabel.Size = new System.Drawing.Size(89, 13);
            this.ResultLabel.TabIndex = 0;
            this.ResultLabel.Text = "12345 sentences";
            // 
            // ElapsedTimeLabel
            // 
            this.ElapsedTimeLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ElapsedTimeLabel.AutoSize = true;
            this.ElapsedTimeLabel.Location = new System.Drawing.Point(186, 4);
            this.ElapsedTimeLabel.Name = "ElapsedTimeLabel";
            this.ElapsedTimeLabel.Size = new System.Drawing.Size(49, 13);
            this.ElapsedTimeLabel.TabIndex = 0;
            this.ElapsedTimeLabel.Text = "00:00:00";
            // 
            // InitialLettersToolStripMenuItem
            // 
            this.InitialLettersToolStripMenuItem.Name = "InitialLettersToolStripMenuItem";
            this.InitialLettersToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.InitialLettersToolStripMenuItem.Text = "&Initial Letters";
            this.InitialLettersToolStripMenuItem.Click += new System.EventHandler(this.InitialLettersToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(236, 430);
            this.Controls.Add(this.ResultPanel);
            this.Controls.Add(this.LettersTextBox);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.ListView);
            this.Controls.Add(this.MenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuStrip;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Initial Letters";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ResultPanel.ResumeLayout(false);
            this.ResultPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem HelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
        private System.Windows.Forms.Timer Timer;
        private System.Windows.Forms.ToolStripMenuItem TypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UniqueLettersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UniqueWordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AllWordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RunToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewToolStripMenuItem;
        private System.Windows.Forms.ListView ListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.TextBox LettersTextBox;
        private System.Windows.Forms.Panel ResultPanel;
        private System.Windows.Forms.Label ResultLabel;
        private System.Windows.Forms.Label ElapsedTimeLabel;
        private System.Windows.Forms.ToolStripMenuItem InitialLettersToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    }
}

