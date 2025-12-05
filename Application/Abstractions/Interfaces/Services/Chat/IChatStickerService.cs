using Application.DTOs.Chat;
using Entities.Models.Chat;

namespace Application.Abstractions.Interfaces.Services.Chat;

public interface IChatStickerService : IGenericService<ChatSticker>
{
    new Task<IEnumerable<ChatStickerDTO>> GetAllAsync();
}

