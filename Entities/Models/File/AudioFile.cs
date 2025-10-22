namespace Entities.Models.File;

using FileModel = File;

public class AudioFile : FileModel
{
    public TimeSpan Duration { get; set; }
}