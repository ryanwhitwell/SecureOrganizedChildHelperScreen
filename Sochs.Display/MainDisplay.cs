using Sochs.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sochs
{
  public partial class MainDisplay : Form, ITemporalDisplay
  {
    private TimeOfDay _timeOfDay;
    
    TimeOfDay ITemporalDisplay.TimeOfDay
    { 
      get => _timeOfDay; 
      set => _timeOfDay = value; 
    }

    public MainDisplay()
    {
      InitializeComponent();

      ConfigureDisplayForScreen();

      Task timeTask = TimeHelper.GetTimeTask(this);

      timeTask.Start();
    }

    private void ConfigureDisplayForScreen()
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

    public void SetToMorning()
    {
      lblTimeOfDay.Invoke(new MethodInvoker(() => {
        lblTimeOfDay.Text = TimeHelper.MorningDescription;
      }));
    }

    public void SetToAfternoon()
    {
      lblTimeOfDay.Invoke(new MethodInvoker(() => {
        lblTimeOfDay.Text = TimeHelper.AfternoonDescription;
      }));
    }

    public void SetToEvening()
    {
      lblTimeOfDay.Invoke(new MethodInvoker(() => {
        lblTimeOfDay.Text = TimeHelper.EveningDescription;
      }));
    }
  }
}
