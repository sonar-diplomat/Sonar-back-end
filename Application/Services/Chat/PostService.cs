using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Repository.File;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Extensions;
using Application.Response;
using Entities.Models.Access;
using Entities.Models.Chat;
using FileModel = Entities.Models.File.File;

namespace Application.Services.Chat;

public class PostService(IPostRepository repository, IVisibilityStateService visibilityStateService, IFileRepository fileRepository)
    : GenericService<Post>(repository), IPostService
{
    public async Task UpdateVisibilityStatusAsync(int trackId, int newVisibilityStatusId)
    {
        Post post = await repository.SnInclude(a => a.VisibilityState).GetByIdValidatedAsync(trackId);
        post.VisibilityState.StatusId = newVisibilityStatusId;
        await repository.UpdateAsync(post);
    }

    public async Task CreateAsync(int artistId, PostDTO dto)
    {
        List<FileModel> files = new();
        if (dto.AttachmentIds != null)
        {
            foreach (int fileId in dto.AttachmentIds)
            {
                FileModel? file = await fileRepository.GetByIdAsync(fileId);
                if (file == null)
                    throw ResponseFactory.Create<NotFoundResponse>([$"File with ID {fileId} not found"]);
                files.Add(file);
            }
        }

        Post post = new()
        {
            Title = dto.Title,
            TextContent = dto.TextContent ?? string.Empty,
            ArtistId = artistId,
            VisibilityStateId = (await visibilityStateService.CreateDefaultAsync(dto.SetPublicOn)).Id,
            Files = files
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

        if (dto.AttachmentIds != null)
        {
            List<FileModel> files = new();
            foreach (int fileId in dto.AttachmentIds)
            {
                FileModel? file = await fileRepository.GetByIdAsync(fileId);
                if (file == null)
                    throw ResponseFactory.Create<NotFoundResponse>([$"File with ID {fileId} not found"]);
                files.Add(file);
            }
            post.Files = files;
        }
        await UpdateAsync(post);
    }
}