﻿namespace Firebase_Connection
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.dirNameLable = new System.Windows.Forms.Label();
            this.lastUpdatelabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Highlight;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(25, 45);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(5);
            this.label1.Size = new System.Drawing.Size(309, 42);
            this.label1.TabIndex = 0;
            this.label1.Text = "Firebase Link is Active";
            // 
            // dirNameLable
            // 
            this.dirNameLable.AutoSize = true;
            this.dirNameLable.BackColor = System.Drawing.SystemColors.Highlight;
            this.dirNameLable.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dirNameLable.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dirNameLable.Location = new System.Drawing.Point(26, 167);
            this.dirNameLable.Name = "dirNameLable";
            this.dirNameLable.Padding = new System.Windows.Forms.Padding(5);
            this.dirNameLable.Size = new System.Drawing.Size(525, 35);
            this.dirNameLable.TabIndex = 1;
            this.dirNameLable.Text = "Test filename... C:\\Users\\MBPtrader\\Documents\\FireBase";
            // 
            // lastUpdatelabel
            // 
            this.lastUpdatelabel.AutoSize = true;
            this.lastUpdatelabel.BackColor = System.Drawing.SystemColors.Highlight;
            this.lastUpdatelabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lastUpdatelabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lastUpdatelabel.Location = new System.Drawing.Point(25, 103);
            this.lastUpdatelabel.Name = "lastUpdatelabel";
            this.lastUpdatelabel.Padding = new System.Windows.Forms.Padding(5);
            this.lastUpdatelabel.Size = new System.Drawing.Size(244, 42);
            this.lastUpdatelabel.TabIndex = 2;
            this.lastUpdatelabel.Text = "January 10, 2017";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 248);
            this.Controls.Add(this.lastUpdatelabel);
            this.Controls.Add(this.dirNameLable);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Firebase Link";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label dirNameLable;
        private System.Windows.Forms.Label lastUpdatelabel;
    }
}

