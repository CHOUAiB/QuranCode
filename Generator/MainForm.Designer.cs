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
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.GenerateButton = new System.Windows.Forms.Button();
            this.ListView = new System.Windows.Forms.ListView();
            this.SentenceColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ValueColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.WordColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AddPositionsAndDistancesCheckBox = new System.Windows.Forms.CheckBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.ValidWordCountLabel = new System.Windows.Forms.Label();
            this.NumberTypeLabel = new System.Windows.Forms.Label();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ConcatenationDirectionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // StatusStrip
            // 
            this.StatusStrip.Location = new System.Drawing.Point(0, 290);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(518, 22);
            this.StatusStrip.TabIndex = 2;
            this.StatusStrip.Text = "StatusStrip";
            // 
            // GenerateButton
            // 
            this.GenerateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.GenerateButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.GenerateButton.Image = ((System.Drawing.Image)(resources.GetObject("GenerateButton.Image")));
            this.GenerateButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GenerateButton.Location = new System.Drawing.Point(213, 291);
            this.GenerateButton.Name = "GenerateButton";
            this.GenerateButton.Size = new System.Drawing.Size(117, 21);
            this.GenerateButton.TabIndex = 33;
            this.GenerateButton.Text = "&Generate Words";
            this.GenerateButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.GenerateButton.UseVisualStyleBackColor = true;
            this.GenerateButton.Click += new System.EventHandler(this.GenerateButton_Click);
            // 
            // ListView
            // 
            this.ListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.SentenceColumnHeader,
            this.ValueColumnHeader,
            this.WordColumnHeader});
            this.ListView.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListView.Location = new System.Drawing.Point(0, 0);
            this.ListView.Name = "ListView";
            this.ListView.Size = new System.Drawing.Size(518, 290);
            this.ListView.TabIndex = 1;
            this.ListView.UseCompatibleStateImageBehavior = false;
            this.ListView.View = System.Windows.Forms.View.Details;
            // 
            // SentenceColumnHeader
            // 
            this.SentenceColumnHeader.Text = "Sentence";
            this.SentenceColumnHeader.Width = 297;
            // 
            // ValueColumnHeader
            // 
            this.ValueColumnHeader.Text = "Value";
            this.ValueColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ValueColumnHeader.Width = 90;
            // 
            // WordColumnHeader
            // 
            this.WordColumnHeader.Text = "Word";
            this.WordColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.WordColumnHeader.Width = 110;
            // 
            // AddPositionsAndDistancesCheckBox
            // 
            this.AddPositionsAndDistancesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddPositionsAndDistancesCheckBox.AutoSize = true;
            this.AddPositionsAndDistancesCheckBox.Location = new System.Drawing.Point(3, 294);
            this.AddPositionsAndDistancesCheckBox.Name = "AddPositionsAndDistancesCheckBox";
            this.AddPositionsAndDistancesCheckBox.Size = new System.Drawing.Size(146, 17);
            this.AddPositionsAndDistancesCheckBox.TabIndex = 11;
            this.AddPositionsAndDistancesCheckBox.Text = "&Add positions && distances";
            this.AddPositionsAndDistancesCheckBox.UseVisualStyleBackColor = true;
            this.AddPositionsAndDistancesCheckBox.CheckedChanged += new System.EventHandler(this.AddPositionsAndDistancesCheckBox_CheckedChanged);
            // 
            // SaveButton
            // 
            this.SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SaveButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.Image")));
            this.SaveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SaveButton.Location = new System.Drawing.Point(434, 291);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(62, 21);
            this.SaveButton.TabIndex = 55;
            this.SaveButton.Text = "&Save";
            this.SaveButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // ValidWordCountLabel
            // 
            this.ValidWordCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ValidWordCountLabel.ForeColor = System.Drawing.Color.Firebrick;
            this.ValidWordCountLabel.Location = new System.Drawing.Point(336, 295);
            this.ValidWordCountLabel.Name = "ValidWordCountLabel";
            this.ValidWordCountLabel.Size = new System.Drawing.Size(91, 13);
            this.ValidWordCountLabel.TabIndex = 44;
            this.ValidWordCountLabel.Text = "0 valid words";
            this.ValidWordCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NumberTypeLabel
            // 
            this.NumberTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NumberTypeLabel.BackColor = System.Drawing.Color.Silver;
            this.NumberTypeLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.NumberTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumberTypeLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.NumberTypeLabel.Location = new System.Drawing.Point(186, 293);
            this.NumberTypeLabel.Name = "NumberTypeLabel";
            this.NumberTypeLabel.Size = new System.Drawing.Size(24, 17);
            this.NumberTypeLabel.TabIndex = 22;
            this.NumberTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.NumberTypeLabel.Click += new System.EventHandler(this.NumberTypeLabel_Click);
            // 
            // ConcatenationDirectionLabel
            // 
            this.ConcatenationDirectionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ConcatenationDirectionLabel.BackColor = System.Drawing.Color.Silver;
            this.ConcatenationDirectionLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ConcatenationDirectionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConcatenationDirectionLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.ConcatenationDirectionLabel.Location = new System.Drawing.Point(160, 293);
            this.ConcatenationDirectionLabel.Name = "ConcatenationDirectionLabel";
            this.ConcatenationDirectionLabel.Size = new System.Drawing.Size(24, 17);
            this.ConcatenationDirectionLabel.TabIndex = 56;
            this.ConcatenationDirectionLabel.Text = "<--";
            this.ConcatenationDirectionLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ToolTip.SetToolTip(this.ConcatenationDirectionLabel, "concatenate letter values right to left");
            this.ConcatenationDirectionLabel.Click += new System.EventHandler(this.ConcatenationDirectionLabel_Click);
            // 
            // MainForm
            // 
            this.AcceptButton = this.GenerateButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 312);
            this.Controls.Add(this.ConcatenationDirectionLabel);
            this.Controls.Add(this.NumberTypeLabel);
            this.Controls.Add(this.ValidWordCountLabel);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.AddPositionsAndDistancesCheckBox);
            this.Controls.Add(this.GenerateButton);
            this.Controls.Add(this.ListView);
            this.Controls.Add(this.StatusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Generator";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.StatusStrip StatusStrip;
    private System.Windows.Forms.Button GenerateButton;
    private System.Windows.Forms.ListView ListView;
    private System.Windows.Forms.ColumnHeader SentenceColumnHeader;
    private System.Windows.Forms.ColumnHeader WordColumnHeader;
    private System.Windows.Forms.ColumnHeader ValueColumnHeader;
    private System.Windows.Forms.CheckBox AddPositionsAndDistancesCheckBox;
    private System.Windows.Forms.Button SaveButton;
    private System.Windows.Forms.Label ValidWordCountLabel;
    private System.Windows.Forms.Label NumberTypeLabel;
    private System.Windows.Forms.ToolTip ToolTip;
    private System.Windows.Forms.Label ConcatenationDirectionLabel;
}
