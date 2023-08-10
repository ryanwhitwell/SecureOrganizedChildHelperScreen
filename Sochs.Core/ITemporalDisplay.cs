using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sochs.Core
{
  public interface ITemporalDisplay
  {
    void SetToMorning();
    void SetToAfternoon();
    void SetToEvening();
    TimeOfDay TimeOfDay { get; set; }
  }
}
