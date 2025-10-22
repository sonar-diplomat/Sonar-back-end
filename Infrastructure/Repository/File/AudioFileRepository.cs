using Application.Abstractions.Interfaces.Repository.File;
using Entities.Models.File;
using Infrastructure.Data;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.File;

public class AudioFileRepository(SonarContext dbContext)
    : GenericRepository<AudioFile>(dbContext), IAudioFileRepository
{
}