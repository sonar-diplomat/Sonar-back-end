using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Services.Chat;
using Application.DTOs.Chat;
using Application.Extensions;
using Entities.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Chat;

public class ChatStickerService(IChatStickerRepository repository) : GenericService<ChatSticker>(repository), IChatStickerService
{
    public new async Task<IEnumerable<ChatStickerDTO>> GetAllAsync()
    {
        var stickers = await repository
            .SnInclude(s => s.Category)
            .ToListAsync();

        return stickers.Select(s => new ChatStickerDTO
        {
            Id = s.Id,
            Name = s.Name,
            ImageFileId = s.ImageFileId,
            CategoryId = s.CategoryId,
            CategoryName = s.Category?.Name
        });
    }
}