namespace Sochs
{
  partial class MainDisplay
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
      this.btnAlice = new System.Windows.Forms.Button();
      this.btnClara = new System.Windows.Forms.Button();
      this.lblTimeOfDay = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // btnAlice
      // 
      this.btnAlice.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnAlice.Location = new System.Drawing.Point(213, 237);
      this.btnAlice.Name = "btnAlice";
      this.btnAlice.Size = new System.Drawing.Size(321, 103);
      this.btnAlice.TabIndex = 0;
      this.btnAlice.Text = "Alice";
      this.btnAlice.UseVisualStyleBackColor = true;
      this.btnAlice.Click += new System.EventHandler(this.btnAlice_Click);
      // 
      // btnClara
      // 
      this.btnClara.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClara.Location = new System.Drawing.Point(213, 483);
      this.btnClara.Name = "btnClara";
      this.btnClara.Size = new System.Drawing.Size(321, 103);
      this.btnClara.TabIndex = 1;
      this.btnClara.Text = "Clara";
      this.btnClara.UseVisualStyleBackColor = true;
      this.btnClara.Click += new System.EventHandler(this.btnClara_Click);
      // 
      // lblTimeOfDay
      // 
      this.lblTimeOfDay.AutoSize = true;
      this.lblTimeOfDay.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblTimeOfDay.Location = new System.Drawing.Point(12, 9);
      this.lblTimeOfDay.Name = "lblTimeOfDay";
      this.lblTimeOfDay.Size = new System.Drawing.Size(106, 37);
      this.lblTimeOfDay.TabIndex = 2;
      this.lblTimeOfDay.Text = "label1";
      // 
      // MainDisplay
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Window;
      this.ClientSize = new System.Drawing.Size(733, 1100);
      this.Controls.Add(this.lblTimeOfDay);
      this.Controls.Add(this.btnClara);
      this.Controls.Add(this.btnAlice);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MainDisplay";
      this.ShowIcon = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnAlice;
    private System.Windows.Forms.Button btnClara;
    private System.Windows.Forms.Label lblTimeOfDay;
  }
}