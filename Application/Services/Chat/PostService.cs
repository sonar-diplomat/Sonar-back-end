using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Extensions;
using Entities.Models.Access;
using Entities.Models.Chat;

namespace Application.Services.Chat;

public class PostService(IPostRepository repository, IVisibilityStateService visibilityStateService)
    : GenericService<Post>(repository), IPostService
{
    public async Task UpdateVisibilityStatusAsync(int trackId, int newVisibilityStatusId)
    {
        Post post = await repository.Include(a => a.VisibilityState).GetByIdValidatedAsync(trackId);
        post.VisibilityState.StatusId = newVisibilityStatusId;
        await repository.UpdateAsync(post);
    }

    public async Task CreateAsync(int artistId, PostDTO dto)
    {
        Post post = new()
        {
            Title = dto.Title,
            TextContent = dto.TextContent ?? string.Empty,
            ArtistId = artistId,
            VisibilityStateId = (await visibilityStateService.CreateDefaultAsync(dto.SetPublicOn)).Id,
            Files = dto.Attachments
        };
        await CreateAsync(post);
    }

    public async Task UpdateAsync(int postId, PostDTO dto)
    {
        Post post = await GetByIdValidatedAsync(postId);
        post.Title = dto.Title;
        post.TextContent = dto.TextContent ?? post.TextContent;
        if (dto.SetPublicOn != null)
        {
            VisibilityState visibilityState =
                await visibilityStateService.GetByIdValidatedAsync(post.VisibilityStateId);
            visibilityState.SetPublicOn = dto.SetPublicOn.Value;
            await visibilityStateService.UpdateAsync(visibilityState);
        }

        post.Files = dto.Attachments ?? post.Files;
        await UpdateAsync(post);
    }
}