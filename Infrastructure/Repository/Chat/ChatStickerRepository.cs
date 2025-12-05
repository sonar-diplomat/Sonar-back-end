using Application.Abstractions.Interfaces.Repository.Chat;
using Entities.Models.Chat;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Chat;

public class ChatStickerRepository(SonarContext dbContext) : GenericRepository<ChatSticker>(dbContext), IChatStickerRepository
{
}

