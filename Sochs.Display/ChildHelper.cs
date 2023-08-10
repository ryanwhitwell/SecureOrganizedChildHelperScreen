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
  public partial class ChildHelper : Form
  {
    private readonly string _childName;
    
    public ChildHelper(string childName)
    {
      if(string.IsNullOrWhiteSpace(childName)) { throw new ArgumentNullException(nameof(childName)); }

      _childName = childName;
      
      InitializeComponent();
    }

    private void ChildHelper_Load(object sender, EventArgs e)
    {
      lblChildName.Text = _childName;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
