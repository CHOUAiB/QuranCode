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
            this.GenerateButton = new System.Windows.Forms.Button();
            this.ListView = new System.Windows.Forms.ListView();
            this.IdColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SentenceColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ValueColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.WordColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AddPositionsCheckBox = new System.Windows.Forms.CheckBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.WordCountLabel = new System.Windows.Forms.Label();
            this.NumberTypeLabel = new System.Windows.Forms.Label();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ConcatenationDirectionLabel = new System.Windows.Forms.Label();
            this.AddDistancesCheckBox = new System.Windows.Forms.CheckBox();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // GenerateButton
            // 
            this.GenerateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.GenerateButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.GenerateButton.Image = ((System.Drawing.Image)(resources.GetObject("GenerateButton.Image")));
            this.GenerateButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GenerateButton.Location = new System.Drawing.Point(327, 306);
            this.GenerateButton.Name = "GenerateButton";
            this.GenerateButton.Size = new System.Drawing.Size(110, 21);
            this.GenerateButton.TabIndex = 13;
            this.GenerateButton.Text = "&Generate words";
            this.GenerateButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ToolTip.SetToolTip(this.GenerateButton, resources.GetString("GenerateButton.ToolTip"));
            this.GenerateButton.UseVisualStyleBackColor = true;
            this.GenerateButton.Click += new System.EventHandler(this.GenerateButton_Click);
            // 
            // ListView
            // 
            this.ListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IdColumnHeader,
            this.SentenceColumnHeader,
            this.ValueColumnHeader,
            this.WordColumnHeader});
            this.ListView.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListView.Location = new System.Drawing.Point(0, 0);
            this.ListView.Name = "ListView";
            this.ListView.Size = new System.Drawing.Size(575, 294);
            this.ListView.TabIndex = 1;
            this.ListView.UseCompatibleStateImageBehavior = false;
            this.ListView.View = System.Windows.Forms.View.Details;
            this.ListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListView_ColumnClick);
            // 
            // IdColumnHeader
            // 
            this.IdColumnHeader.Text = "# ▲";
            this.IdColumnHeader.Width = 55;
            // 
            // SentenceColumnHeader
            // 
            this.SentenceColumnHeader.Text = "Sentence  ";
            this.SentenceColumnHeader.Width = 297;
            // 
            // ValueColumnHeader
            // 
            this.ValueColumnHeader.Text = "Value  ";
            this.ValueColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ValueColumnHeader.Width = 90;
            // 
            // WordColumnHeader
            // 
            this.WordColumnHeader.Text = "Word  ";
            this.WordColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.WordColumnHeader.Width = 110;
            // 
            // AddPositionsCheckBox
            // 
            this.AddPositionsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddPositionsCheckBox.AutoSize = true;
            this.AddPositionsCheckBox.Location = new System.Drawing.Point(5, 309);
            this.AddPositionsCheckBox.Name = "AddPositionsCheckBox";
            this.AddPositionsCheckBox.Size = new System.Drawing.Size(130, 17);
            this.AddPositionsCheckBox.TabIndex = 3;
            this.AddPositionsCheckBox.Text = "Add positions to value";
            this.ToolTip.SetToolTip(this.AddPositionsCheckBox, "Add letter, word, verse and chapter positions\r\nto each letter value");
            this.AddPositionsCheckBox.UseVisualStyleBackColor = true;
            this.AddPositionsCheckBox.CheckedChanged += new System.EventHandler(this.AddPositionsCheckBox_CheckedChanged);
            // 
            // SaveButton
            // 
            this.SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SaveButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.Image")));
            this.SaveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SaveButton.Location = new System.Drawing.Point(515, 306);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(59, 21);
            this.SaveButton.TabIndex = 15;
            this.SaveButton.Text = "&Save";
            this.SaveButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // WordCountLabel
            // 
            this.WordCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.WordCountLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.WordCountLabel.Location = new System.Drawing.Point(440, 310);
            this.WordCountLabel.Name = "WordCountLabel";
            this.WordCountLabel.Size = new System.Drawing.Size(70, 13);
            this.WordCountLabel.TabIndex = 14;
            this.WordCountLabel.Text = "00000 words";
            this.WordCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ToolTip.SetToolTip(this.WordCountLabel, "Valid Quran words");
            // 
            // NumberTypeLabel
            // 
            this.NumberTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.NumberTypeLabel.BackColor = System.Drawing.Color.Silver;
            this.NumberTypeLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.NumberTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumberTypeLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.NumberTypeLabel.Location = new System.Drawing.Point(301, 308);
            this.NumberTypeLabel.Name = "NumberTypeLabel";
            this.NumberTypeLabel.Size = new System.Drawing.Size(24, 17);
            this.NumberTypeLabel.TabIndex = 12;
            this.NumberTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.NumberTypeLabel.Click += new System.EventHandler(this.NumberTypeLabel_Click);
            // 
            // ConcatenationDirectionLabel
            // 
            this.ConcatenationDirectionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ConcatenationDirectionLabel.BackColor = System.Drawing.Color.Silver;
            this.ConcatenationDirectionLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ConcatenationDirectionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConcatenationDirectionLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.ConcatenationDirectionLabel.Location = new System.Drawing.Point(275, 308);
            this.ConcatenationDirectionLabel.Name = "ConcatenationDirectionLabel";
            this.ConcatenationDirectionLabel.Size = new System.Drawing.Size(24, 17);
            this.ConcatenationDirectionLabel.TabIndex = 11;
            this.ConcatenationDirectionLabel.Text = "←";
            this.ConcatenationDirectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.ConcatenationDirectionLabel, "concatenate letter values right to left");
            this.ConcatenationDirectionLabel.Click += new System.EventHandler(this.ConcatenationDirectionLabel_Click);
            // 
            // AddDistancesCheckBox
            // 
            this.AddDistancesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddDistancesCheckBox.AutoSize = true;
            this.AddDistancesCheckBox.Location = new System.Drawing.Point(137, 309);
            this.AddDistancesCheckBox.Name = "AddDistancesCheckBox";
            this.AddDistancesCheckBox.Size = new System.Drawing.Size(134, 17);
            this.AddDistancesCheckBox.TabIndex = 4;
            this.AddDistancesCheckBox.Text = "Add distances to value";
            this.ToolTip.SetToolTip(this.AddDistancesCheckBox, "Add letter, word and verse distances\r\nto the same previous and next letter, word " +
        "and verse\r\nto each letter value");
            this.AddDistancesCheckBox.UseVisualStyleBackColor = true;
            this.AddDistancesCheckBox.CheckedChanged += new System.EventHandler(this.AddDistancesCheckBox_CheckedChanged);
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBar.Location = new System.Drawing.Point(-1, 294);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(576, 11);
            this.ProgressBar.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AcceptButton = this.GenerateButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 327);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.ConcatenationDirectionLabel);
            this.Controls.Add(this.NumberTypeLabel);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.AddDistancesCheckBox);
            this.Controls.Add(this.AddPositionsCheckBox);
            this.Controls.Add(this.GenerateButton);
            this.Controls.Add(this.ListView);
            this.Controls.Add(this.WordCountLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Book Generator";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button GenerateButton;
    private System.Windows.Forms.ListView ListView;
    private System.Windows.Forms.ColumnHeader SentenceColumnHeader;
    private System.Windows.Forms.ColumnHeader WordColumnHeader;
    private System.Windows.Forms.ColumnHeader ValueColumnHeader;
    private System.Windows.Forms.CheckBox AddPositionsCheckBox;
    private System.Windows.Forms.Button SaveButton;
    private System.Windows.Forms.Label WordCountLabel;
    private System.Windows.Forms.Label NumberTypeLabel;
    private System.Windows.Forms.ToolTip ToolTip;
    private System.Windows.Forms.Label ConcatenationDirectionLabel;
    private System.Windows.Forms.ColumnHeader IdColumnHeader;
    private System.Windows.Forms.CheckBox AddDistancesCheckBox;
    private System.Windows.Forms.ProgressBar ProgressBar;
}
