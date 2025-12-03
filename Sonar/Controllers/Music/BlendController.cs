using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Music;

[Route("api/[controller]")]
[ApiController]
public class BlendController(
    UserManager<User> userManager,
    IShareService shareService,
    ICollectionService<Blend> collectionService
) : CollectionController<Blend>(userManager, collectionService, shareService)
{
}