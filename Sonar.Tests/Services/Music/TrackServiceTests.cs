using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs.Music;
using Application.Extensions;
using Application.Response;
using Application.Services.Music;
using Entities.Models.File;
using Entities.Models.Music;
using FluentAssertions;
using Infrastructure.Repository.Music;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Sonar.Tests.Services.Music;

public class TrackServiceTests : MusicServiceTestsBase
{
    private readonly Mock<ITrackRepository> _trackRepositoryMock;
    private readonly Mock<IAudioFileService> _audioFileServiceMock;
    private readonly Mock<IVisibilityStateService> _visibilityStateServiceMock;
    private readonly Mock<ILibraryService> _libraryServiceMock;
    private readonly Mock<IArtistService> _artistServiceMock;
    private readonly Mock<ITrackArtistService> _trackArtistServiceMock;
    private readonly MoodTagRepository _moodTagRepository;
    private readonly TrackMoodTagRepository _trackMoodTagRepository;
    private readonly TrackService _service;

    public TrackServiceTests()
    {
        _trackRepositoryMock = new Mock<ITrackRepository>();
        _audioFileServiceMock = new Mock<IAudioFileService>();
        _visibilityStateServiceMock = new Mock<IVisibilityStateService>();
        _libraryServiceMock = new Mock<ILibraryService>();
        _artistServiceMock = new Mock<IArtistService>();
        _trackArtistServiceMock = new Mock<ITrackArtistService>();
        _moodTagRepository = new MoodTagRepository(Context);
        _trackMoodTagRepository = new TrackMoodTagRepository(Context);

        SetupMocks();

        _service = new TrackService(
            _trackRepositoryMock.Object,
            _audioFileServiceMock.Object,
            _visibilityStateServiceMock.Object,
            _libraryServiceMock.Object,
            _artistServiceMock.Object,
            _trackArtistServiceMock.Object,
            _moodTagRepository,
            _trackMoodTagRepository
        );
    }

    private void SetupMocks()
    {
        _trackRepositoryMock
            .Setup(x => x.Query())
            .Returns(() => Context.Set<Track>().AsQueryable());
    }

    #region GetTrackDtoAsync Tests

    [Fact]
    public async Task GetTrackDtoAsync_WithGenreAndMoodTags_ReturnsCorrectDto()
    {
        var genre = GetGenre(1);
        var moodTags = GetMoodTags(1, 2, 3);
        var track = CreateTrackWithGenreAndMoodTags(1, genre.Id, moodTags.Select(mt => mt.Id).ToArray());

        SetupTrackRepositoryMocks();
        SetupVisibilityStateMocks();

        var result = await _service.GetTrackDtoAsync(track.Id, null);

        result.Should().NotBeNull();
        result.Genre.Should().NotBeNull();
        result.Genre.Id.Should().Be(genre.Id);
        result.Genre.Name.Should().Be(genre.Name);
        result.MoodTags.Should().HaveCount(3);
        result.MoodTags.Select(mt => mt.Id).Should().BeEquivalentTo(moodTags.Select(mt => mt.Id));
    }

    [Fact]
    public async Task GetTrackDtoAsync_WithoutMoodTags_ReturnsEmptyMoodTagsList()
    {
        var genre = GetGenre(1);
        var track = CreateTrackWithGenreAndMoodTags(2, genre.Id, Array.Empty<int>());

        SetupTrackRepositoryMocks();
        SetupVisibilityStateMocks();

        var result = await _service.GetTrackDtoAsync(track.Id, null);

        result.Should().NotBeNull();
        result.Genre.Should().NotBeNull();
        result.MoodTags.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTrackDtoAsync_WithoutGenre_ThrowsBadRequest()
    {
        var track = CreateTrack(3, 1);
        track.GenreId = 999;
        Context.Tracks.Update(track);
        Context.SaveChanges();
        Context.ChangeTracker.Clear();

        SetupTrackRepositoryMocks();
        SetupVisibilityStateMocks();

        var exception = await Assert.ThrowsAsync<BadRequestResponse>(
            () => _service.GetTrackDtoAsync(track.Id, null));

        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(400);
    }

    #endregion

    #region UpdateMoodTagsAsync Tests

    [Fact]
    public async Task UpdateMoodTagsAsync_WithValidMoodTagIds_UpdatesMoodTags()
    {
        var track = CreateTrack(1, 1);
        var moodTags = GetMoodTags(1, 2);

        await _service.UpdateMoodTagsAsync(track.Id, moodTags.Select(mt => mt.Id));

        var trackMoodTags = Context.TrackMoodTags
            .Where(tmt => tmt.TrackId == track.Id)
            .ToList();

        trackMoodTags.Should().HaveCount(2);
        trackMoodTags.Select(tmt => tmt.MoodTagId).Should().BeEquivalentTo(moodTags.Select(mt => mt.Id));
    }

    [Fact]
    public async Task UpdateMoodTagsAsync_WithMoreThanThreeMoodTags_ThrowsBadRequest()
    {
        var track = CreateTrack(1, 1);

        var exception = await Assert.ThrowsAsync<BadRequestResponse>(
            () => _service.UpdateMoodTagsAsync(track.Id, new[] { 1, 2, 3, 4 }));

        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task UpdateMoodTagsAsync_WithInvalidMoodTagIds_ThrowsBadRequest()
    {
        var track = CreateTrack(1, 1);

        var exception = await Assert.ThrowsAsync<BadRequestResponse>(
            () => _service.UpdateMoodTagsAsync(track.Id, new[] { 1, 999 }));

        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task UpdateMoodTagsAsync_WithEmptyMoodTagIds_RemovesAllMoodTags()
    {
        var track = CreateTrack(1, 1);
        var existingMoodTags = GetMoodTags(1, 2);
        CreateTrackMoodTags(track.Id, existingMoodTags.Select(mt => mt.Id).ToArray());

        await _service.UpdateMoodTagsAsync(track.Id, Array.Empty<int>());

        var trackMoodTags = Context.TrackMoodTags
            .Where(tmt => tmt.TrackId == track.Id)
            .ToList();

        trackMoodTags.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateMoodTagsAsync_ReplacesExistingMoodTags()
    {
        var track = CreateTrack(1, 1);
        CreateTrackMoodTags(track.Id, new[] { 1, 2 });
        var newMoodTags = GetMoodTags(3, 4);

        await _service.UpdateMoodTagsAsync(track.Id, newMoodTags.Select(mt => mt.Id));

        var trackMoodTags = Context.TrackMoodTags
            .Where(tmt => tmt.TrackId == track.Id)
            .ToList();

        trackMoodTags.Should().HaveCount(2);
        trackMoodTags.Select(tmt => tmt.MoodTagId).Should().BeEquivalentTo(newMoodTags.Select(mt => mt.Id));
    }

    [Fact]
    public async Task UpdateMoodTagsAsync_WithMaximumThreeMoodTags_Succeeds()
    {
        var track = CreateTrack(1, 1);
        var moodTags = GetMoodTags(1, 2, 3);

        await _service.UpdateMoodTagsAsync(track.Id, moodTags.Select(mt => mt.Id));

        var trackMoodTags = Context.TrackMoodTags
            .Where(tmt => tmt.TrackId == track.Id)
            .ToList();

        trackMoodTags.Should().HaveCount(3);
    }

    #endregion

    #region Helper Methods

    private void SetupTrackRepositoryMocks()
    {
        _trackRepositoryMock
            .Setup(x => x.Query())
            .Returns(() => Context.Set<Track>().AsQueryable());
    }

    private void SetupVisibilityStateMocks()
    {
        _visibilityStateServiceMock
            .Setup(x => x.CreateDefaultAsync(It.IsAny<DateTime?>()))
            .ReturnsAsync(() => CreateVisibilityState());
    }

    private Track CreateTrack(int id, int genreId)
    {
        var visibilityState = CreateVisibilityState(id);
        var imageFile = CreateImageFile(id * 1000);
        var audioFile = CreateAudioFile(id * 1000 + 1);

        var track = new Track
        {
            Id = id,
            Title = $"Track {id}",
            GenreId = genreId,
            IsExplicit = false,
            DrivingDisturbingNoises = false,
            CoverId = imageFile.Id,
            LowQualityAudioFileId = audioFile.Id,
            VisibilityStateId = visibilityState.Id,
            Duration = TimeSpan.FromSeconds(180)
        };

        Context.Tracks.Add(track);
        Context.SaveChanges();
        Context.ChangeTracker.Clear();
        return track;
    }

    private Track CreateTrackWithGenreAndMoodTags(int id, int genreId, int[] moodTagIds)
    {
        var track = CreateTrack(id, genreId);
        CreateTrackMoodTags(track.Id, moodTagIds);
        return track;
    }

    private void CreateTrackMoodTags(int trackId, int[] moodTagIds)
    {
        Context.ChangeTracker.Clear();
        foreach (var moodTagId in moodTagIds)
        {
            var trackMoodTag = new TrackMoodTag
            {
                TrackId = trackId,
                MoodTagId = moodTagId
            };
            Context.TrackMoodTags.Add(trackMoodTag);
        }
        Context.SaveChanges();
        Context.ChangeTracker.Clear();
    }

    #endregion
}

