using Sochs.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sochs
{
  public partial class MainDisplay : Form
  {
    public MainDisplay()
    {
      InitializeComponent();

      Configure();
    }

    private void Configure()
    {
      this.SuspendLayout();

      // TODO: Configure screen to fit the TV display

      this.ResumeLayout(false);
    }

    private void btnAlice_Click(object sender, EventArgs e)
    {
      var childControl = new ChildHelper(Children.Alice);
      childControl.Show(this);
    }

    private void btnClara_Click(object sender, EventArgs e)
    {
      var childControl = new ChildHelper(Children.Clara);
      childControl.Show(this);
    }
  }
}
