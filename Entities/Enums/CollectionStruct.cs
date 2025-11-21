using Entities.Models.Music;

namespace Entities.Enums;

public struct CollectionStruct
{
    public const string Album = "album";
    public const string Playlist = "playlist";
    public const string Blend = "blend";
    
    public static Type? IsValid(string collectionType)
    {
        return collectionType switch
        {
            Album => typeof(Album),
            Playlist => typeof(Playlist),
            Blend => typeof(Blend),
            _ => null
        };
    }
}