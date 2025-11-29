using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.Services;
using Entities.Models.UserCore;

public class UserFriendRequestService(IUserFriendRequestRepository repository)
    : GenericService<UserFriendRequest>(repository), IUserFriendRequestService;