using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Extensions;
using Entities.Models.Chat;
using Entities.Models.Music;
using ISettingsService = Application.Abstractions.Interfaces.Services.ISettingsService;
using IUserPrivacySettingsService = Application.Abstractions.Interfaces.Services.IUserPrivacySettingsService;

namespace Application.Services.Music;

public class ArtistService(
    IArtistRepository repository,
    IPostService postService,
    ISettingsService settingsService,
    IUserPrivacySettingsService userPrivacySettingsService)
    : GenericService<Artist>(repository), IArtistService
{
    public async Task<Artist?> GetByNameAsync(string name)
    {
        return await repository.GetByNameAsync(name);
    }

    public async Task<IEnumerable<Artist>> SearchArtistsAsync(string searchTerm)
    {
        return await repository.SearchByNameAsync(searchTerm);
    }

    public async Task UpdateNameAsync(int artistId, string newArtistName)
    {
        Artist artist = await GetByIdValidatedAsync(artistId);
        artist.ArtistName = newArtistName;
        await repository.UpdateAsync(artist);
    }

    public async Task DeleteArtistAsync(int artistId)
    {
        Artist artist = await GetByIdValidatedAsync(artistId);
        foreach (Post post in artist.Posts) await postService.DeleteAsync(post);
        await repository.RemoveAsync(artist);
    }

    public async Task CreatePostAsync(int artistId, PostDTO dto)
    {
        await GetByIdValidatedAsync(artistId);
        await postService.CreateAsync(artistId, dto);
    }

    public async Task UpdatePostAsync(int postId, int artistId, PostDTO dto)
    {
        await GetByIdValidatedAsync(artistId);
        await postService.UpdateAsync(postId, dto);
    }

    public async Task<Artist> RegisterArtistAsync(int userId, string artistName)
    {
        Artist artist = new()
        {
            Id = userId,
            UserId = userId,
            ArtistName = artistName
        };
        Artist createdArtist = await repository.AddAsync(artist);

        var settings = await settingsService.GetByIdValidatedFullAsync(userId);
        if (settings?.UserPrivacy != null)
        {
            var privacySettings = await userPrivacySettingsService.GetByIdAsync(settings.UserPrivacy.Id);
            if (privacySettings != null)
            {
                privacySettings.WhichCanMessageId = 2; // friends
                await userPrivacySettingsService.UpdateAsync(privacySettings);
            }
        }

        return createdArtist;
    }
}