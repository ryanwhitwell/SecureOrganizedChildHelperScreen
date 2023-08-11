using Sochs.Core;
using Sochs.Core.Interfaces;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sochs.Display
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

      var mainDisplay = this.Owner as ITemporalDisplay;

      var timeOfDay = mainDisplay.TimeOfDay;

      switch(timeOfDay)
      {
        case TimeOfDay.Morning:
          lblTimeOfDay.Text = $"I'm going to load {_childName}'s morning stuff";
          break;
        case TimeOfDay.Afternoon:
          lblTimeOfDay.Text = $"I'm going to load {_childName}'s afternoon stuff";
          break;
        case TimeOfDay.Evening:
          lblTimeOfDay.Text = $"I'm going to load {_childName}'s evening stuff";
          break;
        default:
          throw new InvalidEnumArgumentException(nameof(timeOfDay));
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
