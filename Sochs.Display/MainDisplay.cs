using Sochs.Core;
using Sochs.Core.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sochs.Display
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
    }

    private void ConfigureDisplayForScreen()
    {
      this.SuspendLayout();

      // TODO: Configure screen to fit the TV display

      this.ResumeLayout(false);

    }

    private void BtnAlice_Click(object sender, EventArgs e)
    {
      var childControl = new ChildHelper(Children.Alice);
      childControl.Show(this);
    }

    private void BtnClara_Click(object sender, EventArgs e)
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

    protected override void OnLoad(EventArgs e)
    {
      // do stuff before Load-event is raised
      base.OnLoad(e);
      // do stuff after Load-event was raised
      Task.Run(() => TimeHelper.UpdateTimeOfDay(this));
    }
  }
}
