using Application.Response;
using Entities.Models.Access;

namespace Application.Extensions;

/// <summary>
/// Extension methods for checking VisibilityState before returning data to clients.
/// </summary>
public static class VisibilityStateExtensions
{
    private const int VisibleStatusId = 1;
    private const int UnlistedStatusId = 2;
    private const int RestrictedStatusId = 3;
    private const int HiddenStatusId = 4;

    /// <summary>
    /// Validates that the entity is visible to the client.
    /// Throws NotFoundResponse if entity is Hidden or not yet public.
    /// </summary>
    /// <param name="visibilityState">The visibility state to check.</param>
    /// <param name="entityTypeName">Type name of the entity for error messages (e.g., "Track", "Album").</param>
    /// <param name="entityId">ID of the entity for error messages.</param>
    /// <exception cref="ResponseException">Throws NotFoundResponse if entity is Hidden or not yet public.</exception>
    public static void ValidateVisibility(this VisibilityState visibilityState, string entityTypeName, int entityId)
    {
        if (visibilityState == null)
            throw ResponseFactory.Create<NotFoundResponse>([$"{entityTypeName} with ID {entityId} not found"]);

        // Check if content is Hidden - return 404 as if entity doesn't exist
        if (visibilityState.StatusId == HiddenStatusId)
            throw ResponseFactory.Create<NotFoundResponse>([$"{entityTypeName} with ID {entityId} not found"]);

        // Check if content is not yet public (SetPublicOn is in the future)
        if (visibilityState.SetPublicOn > DateTime.UtcNow)
            throw ResponseFactory.Create<NotFoundResponse>([$"{entityTypeName} with ID {entityId} not found"]);

        // Visible, Unlisted, and Restricted are accessible (Restricted may have limited functionality on client side)
        // The actual restrictions for Restricted status are handled on the client side
    }

    /// <summary>
    /// Checks if the entity should be included in search results.
    /// </summary>
    /// <param name="visibilityState">The visibility state to check.</param>
    /// <returns>True if entity should appear in search, false otherwise.</returns>
    public static bool ShouldAppearInSearch(this VisibilityState? visibilityState)
    {
        if (visibilityState == null)
            return false;

        // Hidden and Unlisted should not appear in search
        if (visibilityState.StatusId == HiddenStatusId || visibilityState.StatusId == UnlistedStatusId)
            return false;

        // Check if content is not yet public
        if (visibilityState.SetPublicOn > DateTime.UtcNow)
            return false;

        // Visible and Restricted can appear in search
        return visibilityState.StatusId == VisibleStatusId || visibilityState.StatusId == RestrictedStatusId;
    }

    /// <summary>
    /// Checks if the entity is accessible (not Hidden and already public).
    /// </summary>
    /// <param name="visibilityState">The visibility state to check.</param>
    /// <returns>True if entity is accessible, false otherwise.</returns>
    public static bool IsAccessible(this VisibilityState? visibilityState)
    {
        if (visibilityState == null)
            return false;

        // Hidden is never accessible
        if (visibilityState.StatusId == HiddenStatusId)
            return false;

        // Check if content is not yet public
        if (visibilityState.SetPublicOn > DateTime.UtcNow)
            return false;

        // Visible, Unlisted, and Restricted are accessible
        return true;
    }
}

