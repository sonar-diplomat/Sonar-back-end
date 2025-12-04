using Application.Response;
using Entities.Models.Access;
using Entities.Models.Music;

namespace Application;

/// <summary>
/// Static class for validating VisibilityState across the application.
/// Contains all visibility validation logic centralized in one place.
/// </summary>
public static class VisibilityStateValidator
{
    private const int VisibleStatusId = 1;
    private const int UnlistedStatusId = 2;
    private const int RestrictedStatusId = 3;
    private const int HiddenStatusId = 4;

    /// <summary>
    /// Checks if user is an author of the entity.
    /// </summary>
    private static bool IsUserAuthor(int? userId, IEnumerable<int>? authorIds)
    {
        return userId.HasValue && authorIds != null && authorIds.Contains(userId.Value);
    }

    /// <summary>
    /// Applies standard visibility filter to queryable tracks.
    /// Filters out Hidden entities, entities not yet public, and keeps only Visible or Restricted.
    /// </summary>
    private static IQueryable<Track> ApplyStandardVisibilityFilter(IQueryable<Track> query)
    {
        return query.Where(t =>
           t.VisibilityState.StatusId != HiddenStatusId ||
            t.VisibilityState.SetPublicOn <= DateTime.UtcNow
        );
    }

    /// <summary>
    /// Applies standard visibility filter to queryable albums.
    /// Filters out Hidden entities, entities not yet public, and keeps only Visible or Restricted.
    /// </summary>
    private static IQueryable<Album> ApplyStandardVisibilityFilter(IQueryable<Album> query)
    {
        return query.Where(a =>
            a.VisibilityState.StatusId != HiddenStatusId ||
            a.VisibilityState.SetPublicOn <= DateTime.UtcNow
        );
    }

    /// <summary>
    /// Applies standard visibility filter to queryable playlists.
    /// Filters out Hidden entities, entities not yet public, and keeps only Visible or Restricted.
    /// </summary>
    private static IQueryable<Playlist> ApplyStandardVisibilityFilter(IQueryable<Playlist> query)
    {
        return query.Where(p =>
            p.VisibilityState.StatusId != HiddenStatusId ||
            p.VisibilityState.SetPublicOn <= DateTime.UtcNow
        );
    }



    /// <summary>
    /// Checks if the entity is accessible (not Hidden and already public).
    /// If the user is an author, visibility state is ignored.
    /// If entityTypeName and entityId are provided, throws NotFoundResponse instead of returning false.
    /// </summary>
    /// <param name="visibilityState">The visibility state to check.</param>
    /// <param name="userId">ID of the user requesting the entity. If user is author, visibility is ignored.</param>
    /// <param name="authorIds">Collection of author user IDs. If userId is in this collection, visibility is ignored.</param>
    /// <param name="entityTypeName">Optional. Type name of the entity for error messages (e.g., "Track", "Album"). If provided with entityId, throws exception instead of returning false.</param>
    /// <param name="entityId">Optional. ID of the entity for error messages. If provided with entityTypeName, throws exception instead of returning false.</param>
    /// <returns>True if entity is accessible or user is author, false otherwise (or throws exception if entityTypeName and entityId provided).</returns>
    /// <exception cref="ResponseException">Throws NotFoundResponse if entity is Hidden or not yet public (unless user is author) and entityTypeName/entityId are provided.</exception>
    public static bool IsAccessible(VisibilityState? visibilityState, int? userId, IEnumerable<int>? authorIds, string? entityTypeName = null, int? entityId = null)
    {
        if (visibilityState == null)
        {
            if (entityTypeName != null && entityId.HasValue)
                throw ResponseFactory.Create<NotFoundResponse>([$"{entityTypeName} with ID {entityId.Value} not found"]);
            return false;
        }

        // If user is an author, ignore visibility state
        if (IsUserAuthor(userId, authorIds))
            return true;

        if (visibilityState.StatusId == VisibleStatusId)
            return true;

        // Check if content is Hidden - return 404 as if entity doesn't exist
        if (visibilityState.StatusId == HiddenStatusId)
        {
            if (entityTypeName != null && entityId.HasValue)
                throw ResponseFactory.Create<NotFoundResponse>([$"{entityTypeName} with ID {entityId.Value} not found"]);
            return false;
        }

        // Check if content is not yet public
        if (visibilityState.SetPublicOn > DateTime.UtcNow)
        {
            if (entityTypeName != null && entityId.HasValue)
                throw ResponseFactory.Create<NotFoundResponse>([$"{entityTypeName} with ID {entityId.Value} not found"]);
            return false;
        }

        // Visible, Unlisted, and Restricted are accessible
        return true;
    }

    /// <summary>
    /// Checks if the entity should be included in search results.
    /// </summary>
    /// <param name="visibilityState">The visibility state to check.</param>
    /// <returns>True if entity should appear in search, false otherwise.</returns>
    public static bool ShouldAppearInSearch(VisibilityState? visibilityState)
    {
        if (visibilityState == null)
            return false;

        // Hidden and Unlisted should not appear in search
        if (visibilityState.StatusId is HiddenStatusId or UnlistedStatusId)
            return false;

        // Check if content is not yet public
        if (visibilityState.SetPublicOn > DateTime.UtcNow)
            return false;

        // Visible and Restricted can appear in search
        return visibilityState.StatusId is VisibleStatusId or RestrictedStatusId;
    }

    /// <summary>
    /// Filters tracks by visibility state for search queries.
    /// </summary>
    /// <param name="query">Queryable tracks to filter.</param>
    /// <param name="userId">Optional user ID for author check.</param>
    /// <returns>Filtered queryable tracks.</returns>
    public static IQueryable<Track> FilterTracksByVisibility(IQueryable<Track> query, int? userId)
    {
        if (!userId.HasValue)
        {
            return ApplyStandardVisibilityFilter(query);
        }

        int userIdValue = userId.Value;
        return query.Where(t =>
            t.TrackArtists.Any(ta => ta.Artist != null && ta.Artist.UserId == userIdValue) ||
            (t.VisibilityState.StatusId != HiddenStatusId &&
             t.VisibilityState.SetPublicOn <= DateTime.UtcNow &&
             (t.VisibilityState.StatusId == VisibleStatusId || t.VisibilityState.StatusId == RestrictedStatusId))
        );
    }

    /// <summary>
    /// Filters albums by visibility state for search queries, considering authorship.
    /// </summary>
    /// <param name="query">Queryable albums to filter.</param>
    /// <param name="userId">Optional user ID for author check.</param>
    /// <returns>Filtered queryable albums.</returns>
    public static IQueryable<Album> FilterAlbumsByVisibility(IQueryable<Album> query, int? userId)
    {
        if (!userId.HasValue)
        {
            return ApplyStandardVisibilityFilter(query);
        }

        int userIdValue = userId.Value;
        return query.Where(a =>
            a.AlbumArtists.Any(aa => aa.Artist != null && aa.Artist.UserId == userIdValue) ||
            (a.VisibilityState.StatusId != HiddenStatusId &&
             a.VisibilityState.SetPublicOn <= DateTime.UtcNow &&
             (a.VisibilityState.StatusId == VisibleStatusId || a.VisibilityState.StatusId == RestrictedStatusId))
        );
    }

    /// <summary>
    /// Filters playlists by visibility state for search queries, considering authorship.
    /// </summary>
    /// <param name="query">Queryable playlists to filter.</param>
    /// <param name="userId">Optional user ID for author check.</param>
    /// <returns>Filtered queryable playlists.</returns>
    public static IQueryable<Playlist> FilterPlaylistsByVisibility(IQueryable<Playlist> query, int? userId)
    {
        if (!userId.HasValue)
        {
            return ApplyStandardVisibilityFilter(query);
        }

        int userIdValue = userId.Value;
        return query.Where(p =>
            p.CreatorId == userIdValue ||
            (p.VisibilityState.StatusId != HiddenStatusId &&
             p.VisibilityState.SetPublicOn <= DateTime.UtcNow &&
             (p.VisibilityState.StatusId == VisibleStatusId || p.VisibilityState.StatusId == RestrictedStatusId))
        );
    }

}


