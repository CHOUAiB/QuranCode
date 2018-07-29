﻿partial class MainForm
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
            this.ValueCombinationDirectionLabel = new System.Windows.Forms.Label();
            this.AddDistancesToPreviousCheckBox = new System.Windows.Forms.CheckBox();
            this.AddDistancesToNextCheckBox = new System.Windows.Forms.CheckBox();
            this.ValueInterlaceLabel = new System.Windows.Forms.Label();
            this.AutoGenerateButton = new System.Windows.Forms.Button();
            this.AddVerseAndWordValuesCheckBox = new System.Windows.Forms.CheckBox();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.NumerologySystemComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // GenerateButton
            // 
            this.GenerateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.GenerateButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.GenerateButton.Image = ((System.Drawing.Image)(resources.GetObject("GenerateButton.Image")));
            this.GenerateButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GenerateButton.Location = new System.Drawing.Point(384, 307);
            this.GenerateButton.Name = "GenerateButton";
            this.GenerateButton.Size = new System.Drawing.Size(110, 21);
            this.GenerateButton.TabIndex = 19;
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
            this.ListView.Size = new System.Drawing.Size(625, 291);
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
            this.SentenceColumnHeader.Width = 345;
            // 
            // ValueColumnHeader
            // 
            this.ValueColumnHeader.Text = "Value  ";
            this.ValueColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ValueColumnHeader.Width = 94;
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
            this.AddPositionsCheckBox.Location = new System.Drawing.Point(4, 316);
            this.AddPositionsCheckBox.Name = "AddPositionsCheckBox";
            this.AddPositionsCheckBox.Size = new System.Drawing.Size(130, 17);
            this.AddPositionsCheckBox.TabIndex = 4;
            this.AddPositionsCheckBox.Text = "Add positions to value";
            this.ToolTip.SetToolTip(this.AddPositionsCheckBox, "Add letter, word and verse positions to each letter value");
            this.AddPositionsCheckBox.UseVisualStyleBackColor = true;
            this.AddPositionsCheckBox.CheckedChanged += new System.EventHandler(this.AddPositionsCheckBox_CheckedChanged);
            // 
            // SaveButton
            // 
            this.SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SaveButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.Image")));
            this.SaveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SaveButton.Location = new System.Drawing.Point(562, 307);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(59, 21);
            this.SaveButton.TabIndex = 88;
            this.SaveButton.Text = "&Save";
            this.SaveButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // WordCountLabel
            // 
            this.WordCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.WordCountLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.WordCountLabel.Location = new System.Drawing.Point(495, 311);
            this.WordCountLabel.Name = "WordCountLabel";
            this.WordCountLabel.Size = new System.Drawing.Size(70, 13);
            this.WordCountLabel.TabIndex = 23;
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
            this.NumberTypeLabel.ForeColor = System.Drawing.Color.Green;
            this.NumberTypeLabel.Location = new System.Drawing.Point(357, 309);
            this.NumberTypeLabel.Name = "NumberTypeLabel";
            this.NumberTypeLabel.Size = new System.Drawing.Size(25, 17);
            this.NumberTypeLabel.TabIndex = 12;
            this.NumberTypeLabel.Text = "P";
            this.NumberTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.NumberTypeLabel, "use prime concatenated letter values only");
            this.NumberTypeLabel.Click += new System.EventHandler(this.NumberTypeLabel_Click);
            // 
            // ValueCombinationDirectionLabel
            // 
            this.ValueCombinationDirectionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ValueCombinationDirectionLabel.BackColor = System.Drawing.Color.Silver;
            this.ValueCombinationDirectionLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ValueCombinationDirectionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ValueCombinationDirectionLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.ValueCombinationDirectionLabel.Location = new System.Drawing.Point(331, 309);
            this.ValueCombinationDirectionLabel.Name = "ValueCombinationDirectionLabel";
            this.ValueCombinationDirectionLabel.Size = new System.Drawing.Size(25, 17);
            this.ValueCombinationDirectionLabel.TabIndex = 11;
            this.ValueCombinationDirectionLabel.Text = "←";
            this.ValueCombinationDirectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.ValueCombinationDirectionLabel, "concatenate letter values right to left: BBBAAA");
            this.ValueCombinationDirectionLabel.Click += new System.EventHandler(this.ValueCombinationDirectionLabel_Click);
            // 
            // AddDistancesToPreviousCheckBox
            // 
            this.AddDistancesToPreviousCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddDistancesToPreviousCheckBox.AutoSize = true;
            this.AddDistancesToPreviousCheckBox.Location = new System.Drawing.Point(152, 302);
            this.AddDistancesToPreviousCheckBox.Name = "AddDistancesToPreviousCheckBox";
            this.AddDistancesToPreviousCheckBox.Size = new System.Drawing.Size(149, 17);
            this.AddDistancesToPreviousCheckBox.TabIndex = 5;
            this.AddDistancesToPreviousCheckBox.Text = "Add ← distances to value";
            this.ToolTip.SetToolTip(this.AddDistancesToPreviousCheckBox, "Add letter and word distances to each letter value\r\nto the previous same letter a" +
        "nd word");
            this.AddDistancesToPreviousCheckBox.UseVisualStyleBackColor = true;
            this.AddDistancesToPreviousCheckBox.CheckedChanged += new System.EventHandler(this.AddDistancesToPreviousCheckBox_CheckedChanged);
            // 
            // AddDistancesToNextCheckBox
            // 
            this.AddDistancesToNextCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddDistancesToNextCheckBox.AutoSize = true;
            this.AddDistancesToNextCheckBox.Location = new System.Drawing.Point(152, 316);
            this.AddDistancesToNextCheckBox.Name = "AddDistancesToNextCheckBox";
            this.AddDistancesToNextCheckBox.Size = new System.Drawing.Size(149, 17);
            this.AddDistancesToNextCheckBox.TabIndex = 6;
            this.AddDistancesToNextCheckBox.Text = "Add → distances to value";
            this.ToolTip.SetToolTip(this.AddDistancesToNextCheckBox, "Add letter and word distances to each letter value\r\nto the next same letter and w" +
        "ord\r\n");
            this.AddDistancesToNextCheckBox.UseVisualStyleBackColor = true;
            this.AddDistancesToNextCheckBox.CheckedChanged += new System.EventHandler(this.AddDistancesToNextCheckBox_CheckedChanged);
            // 
            // ValueInterlaceLabel
            // 
            this.ValueInterlaceLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ValueInterlaceLabel.BackColor = System.Drawing.Color.Silver;
            this.ValueInterlaceLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ValueInterlaceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ValueInterlaceLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.ValueInterlaceLabel.Location = new System.Drawing.Point(305, 309);
            this.ValueInterlaceLabel.Name = "ValueInterlaceLabel";
            this.ValueInterlaceLabel.Size = new System.Drawing.Size(25, 17);
            this.ValueInterlaceLabel.TabIndex = 9;
            this.ValueInterlaceLabel.Text = "- -";
            this.ValueInterlaceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.ValueInterlaceLabel, "interlace letter values");
            this.ValueInterlaceLabel.Click += new System.EventHandler(this.ValueInterlaceLabel_Click);
            // 
            // AutoGenerateButton
            // 
            this.AutoGenerateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AutoGenerateButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AutoGenerateButton.Image = ((System.Drawing.Image)(resources.GetObject("AutoGenerateButton.Image")));
            this.AutoGenerateButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.AutoGenerateButton.Location = new System.Drawing.Point(376, 1);
            this.AutoGenerateButton.Name = "AutoGenerateButton";
            this.AutoGenerateButton.Size = new System.Drawing.Size(25, 23);
            this.AutoGenerateButton.TabIndex = 89;
            this.AutoGenerateButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ToolTip.SetToolTip(this.AutoGenerateButton, resources.GetString("AutoGenerateButton.ToolTip"));
            this.AutoGenerateButton.UseVisualStyleBackColor = true;
            this.AutoGenerateButton.Click += new System.EventHandler(this.AutoGenerateButton_Click);
            // 
            // AddVerseAndWordValuesCheckBox
            // 
            this.AddVerseAndWordValuesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddVerseAndWordValuesCheckBox.AutoSize = true;
            this.AddVerseAndWordValuesCheckBox.Location = new System.Drawing.Point(4, 302);
            this.AddVerseAndWordValuesCheckBox.Name = "AddVerseAndWordValuesCheckBox";
            this.AddVerseAndWordValuesCheckBox.Size = new System.Drawing.Size(143, 17);
            this.AddVerseAndWordValuesCheckBox.TabIndex = 3;
            this.AddVerseAndWordValuesCheckBox.Text = "Add verse && word values";
            this.ToolTip.SetToolTip(this.AddVerseAndWordValuesCheckBox, "Add letter\'s verse and word values to each letter value");
            this.AddVerseAndWordValuesCheckBox.UseVisualStyleBackColor = true;
            this.AddVerseAndWordValuesCheckBox.CheckedChanged += new System.EventHandler(this.AddVerseAndWordValuesCheckBox_CheckedChanged);
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBar.Location = new System.Drawing.Point(-1, 291);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(626, 11);
            this.ProgressBar.TabIndex = 2;
            // 
            // NumerologySystemComboBox
            // 
            this.NumerologySystemComboBox.BackColor = System.Drawing.Color.MistyRose;
            this.NumerologySystemComboBox.DropDownHeight = 1024;
            this.NumerologySystemComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NumerologySystemComboBox.FormattingEnabled = true;
            this.NumerologySystemComboBox.IntegralHeight = false;
            this.NumerologySystemComboBox.Location = new System.Drawing.Point(165, 2);
            this.NumerologySystemComboBox.Name = "NumerologySystemComboBox";
            this.NumerologySystemComboBox.Size = new System.Drawing.Size(212, 21);
            this.NumerologySystemComboBox.TabIndex = 0;
            this.NumerologySystemComboBox.SelectedIndexChanged += new System.EventHandler(this.NumerologySystemComboBox_SelectedIndexChanged);
            this.NumerologySystemComboBox.MouseHover += new System.EventHandler(this.NumerologySystemComboBox_MouseHover);
            // 
            // MainForm
            // 
            this.AcceptButton = this.GenerateButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 331);
            this.Controls.Add(this.ValueInterlaceLabel);
            this.Controls.Add(this.AddDistancesToNextCheckBox);
            this.Controls.Add(this.AddPositionsCheckBox);
            this.Controls.Add(this.AddDistancesToPreviousCheckBox);
            this.Controls.Add(this.NumberTypeLabel);
            this.Controls.Add(this.ValueCombinationDirectionLabel);
            this.Controls.Add(this.AddVerseAndWordValuesCheckBox);
            this.Controls.Add(this.AutoGenerateButton);
            this.Controls.Add(this.NumerologySystemComboBox);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.GenerateButton);
            this.Controls.Add(this.ListView);
            this.Controls.Add(this.WordCountLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(640, 1600);
            this.MinimumSize = new System.Drawing.Size(640, 369);
            this.Name = "MainForm";
            this.Text = "Generator";
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
    private System.Windows.Forms.Label ValueCombinationDirectionLabel;
    private System.Windows.Forms.ColumnHeader IdColumnHeader;
    private System.Windows.Forms.CheckBox AddDistancesToPreviousCheckBox;
    private System.Windows.Forms.ProgressBar ProgressBar;
    private System.Windows.Forms.CheckBox AddDistancesToNextCheckBox;
    private System.Windows.Forms.ComboBox NumerologySystemComboBox;
    private System.Windows.Forms.Label ValueInterlaceLabel;
    private System.Windows.Forms.Button AutoGenerateButton;
    private System.Windows.Forms.CheckBox AddVerseAndWordValuesCheckBox;
}
