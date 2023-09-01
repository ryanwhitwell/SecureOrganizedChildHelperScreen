using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Sochs.Library.Interfaces;

namespace Sochs.Library
{
  public class AudioService : IAudioService
  {
    private readonly ILogger<AudioService> _logger;

    private readonly IJSRuntime _js;


    public AudioService(ILogger<AudioService> logger, IJSRuntime js)
    {
      _ = logger ?? throw new ArgumentNullException(nameof(logger));
      _ = js ?? throw new ArgumentNullException(nameof(js));

      _logger = logger;
      _js = js;
    }

    public async Task PlayAudioFile(string filePath)
    {
      try
      {
        await _js.InvokeVoidAsync("playAudioFile", filePath);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error playing audio file at path {filePath}", filePath);
        throw;
      }
      
    }
  }
}
