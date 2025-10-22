using Application.Abstractions.Interfaces.Repository.File;
using Entities.Models.File;
using Infrastructure.Data;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.File;

public class VideoFileRepository(SonarContext dbContext)
    : GenericRepository<VideoFile>(dbContext), IVideoFileRepository;