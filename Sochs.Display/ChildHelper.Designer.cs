namespace Sochs.Display
{
  partial class ChildHelper
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
      this.lblChildName = new System.Windows.Forms.Label();
      this.button1 = new System.Windows.Forms.Button();
      this.lblTimeOfDay = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // lblChildName
      // 
      this.lblChildName.AutoSize = true;
      this.lblChildName.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblChildName.Location = new System.Drawing.Point(615, 131);
      this.lblChildName.Name = "lblChildName";
      this.lblChildName.Size = new System.Drawing.Size(106, 37);
      this.lblChildName.TabIndex = 0;
      this.lblChildName.Text = "label1";
      // 
      // button1
      // 
      this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.button1.Location = new System.Drawing.Point(12, 134);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(100, 34);
      this.button1.TabIndex = 1;
      this.button1.Text = "<< Back";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // lblTimeOfDay
      // 
      this.lblTimeOfDay.AutoSize = true;
      this.lblTimeOfDay.Location = new System.Drawing.Point(320, 296);
      this.lblTimeOfDay.Name = "lblTimeOfDay";
      this.lblTimeOfDay.Size = new System.Drawing.Size(35, 13);
      this.lblTimeOfDay.TabIndex = 2;
      this.lblTimeOfDay.Text = "label1";
      // 
      // ChildHelper
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Window;
      this.ClientSize = new System.Drawing.Size(733, 1100);
      this.Controls.Add(this.lblTimeOfDay);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.lblChildName);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ChildHelper";
      this.ShowIcon = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Load += new System.EventHandler(this.ChildHelper_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblChildName;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Label lblTimeOfDay;
  }
}