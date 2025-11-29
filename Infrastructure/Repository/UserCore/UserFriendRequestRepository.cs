using Application.Abstractions.Interfaces.Repository.UserCore;
using Entities.Models.UserCore;
using Infrastructure.Data;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.UserCore;

public class UserFriendRequestRepository(SonarContext context)
    : GenericRepository<UserFriendRequest>(context), IUserFriendRequestRepository;