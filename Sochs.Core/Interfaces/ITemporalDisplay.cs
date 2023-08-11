namespace Sochs.Core.Interfaces
{
  public interface ITemporalDisplay
  {
    void SetToMorning();
    void SetToAfternoon();
    void SetToEvening();
    TimeOfDay TimeOfDay { get; set; }
  }
}
