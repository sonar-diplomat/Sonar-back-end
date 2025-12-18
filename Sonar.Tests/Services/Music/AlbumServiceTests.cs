using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs.Music;
using Application.Response;
using Application.Services.Music;
using Entities.Models.File;
using Entities.Models.Music;
using FluentAssertions;
using Infrastructure.Repository.Music;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Sonar.Tests.Services.Music;

public class AlbumServiceTests : MusicServiceTestsBase
{
    private readonly Mock<IAlbumRepository> _albumRepositoryMock;
    private readonly Mock<ITrackRepository> _trackRepositoryMock;
    private readonly Mock<IImageFileService> _imageFileServiceMock;
    private readonly Mock<IAudioFileService> _audioFileServiceMock;
    private readonly Mock<IVisibilityStateService> _visibilityStateServiceMock;
    private readonly Mock<IArtistService> _artistServiceMock;
    private readonly Mock<IAlbumArtistService> _albumArtistServiceMock;
    private readonly Mock<ITrackArtistService> _trackArtistServiceMock;
    private readonly Mock<ITrackService> _trackServiceMock;
    private readonly MoodTagRepository _moodTagRepository;
    private readonly AlbumMoodTagRepository _albumMoodTagRepository;
    private readonly TrackMoodTagRepository _trackMoodTagRepository;
    private readonly AlbumService _service;

    public AlbumServiceTests()
    {
        _albumRepositoryMock = new Mock<IAlbumRepository>();
        _trackRepositoryMock = new Mock<ITrackRepository>();
        _imageFileServiceMock = new Mock<IImageFileService>();
        _audioFileServiceMock = new Mock<IAudioFileService>();
        _visibilityStateServiceMock = new Mock<IVisibilityStateService>();
        _artistServiceMock = new Mock<IArtistService>();
        _albumArtistServiceMock = new Mock<IAlbumArtistService>();
        _trackArtistServiceMock = new Mock<ITrackArtistService>();
        _trackServiceMock = new Mock<ITrackService>();
        _moodTagRepository = new MoodTagRepository(Context);
        _albumMoodTagRepository = new AlbumMoodTagRepository(Context);
        _trackMoodTagRepository = new TrackMoodTagRepository(Context);

        SetupMocks();

        var libraryServiceMock = new Mock<ILibraryService>();
        var folderServiceMock = new Mock<IFolderService>();

        _service = new AlbumService(
            _albumRepositoryMock.Object,
            _trackRepositoryMock.Object,
            _visibilityStateServiceMock.Object,
            _imageFileServiceMock.Object,
            _audioFileServiceMock.Object,
            _albumArtistServiceMock.Object,
            _trackArtistServiceMock.Object,
            _artistServiceMock.Object,
            libraryServiceMock.Object,
            folderServiceMock.Object,
            _trackServiceMock.Object,
            _moodTagRepository,
            _albumMoodTagRepository,
            _trackMoodTagRepository
        );
    }

    private void SetupMocks()
    {
        _albumRepositoryMock
            .Setup(x => x.Query())
            .Returns(() => Context.Set<Album>().AsQueryable());

        _trackRepositoryMock
            .Setup(x => x.Query())
            .Returns(() => Context.Set<Track>().AsQueryable());

        _imageFileServiceMock
            .Setup(x => x.UploadFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync((IFormFile file) =>
            {
                var nextId = Context.ImageFiles.Count() + Context.AudioFiles.Count() + 10000;
                return CreateImageFile(nextId, itemName: file.FileName, url: $"https://example.com/{file.FileName}");
            });

        _audioFileServiceMock
            .Setup(x => x.UploadFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync((IFormFile file) =>
            {
                var nextId = Context.ImageFiles.Count() + Context.AudioFiles.Count() + 20000;
                return CreateAudioFile(nextId, itemName: file.FileName, url: $"https://example.com/{file.FileName}");
            });

        _audioFileServiceMock
            .Setup(x => x.GetDurationAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync(TimeSpan.FromSeconds(180));

        _visibilityStateServiceMock
            .Setup(x => x.CreateDefaultAsync(It.IsAny<DateTime?>()))
            .ReturnsAsync(() => CreateVisibilityState());

        _albumRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Album>()))
            .ReturnsAsync((Album album) =>
            {
                Context.Albums.Add(album);
                Context.SaveChanges();
                Context.ChangeTracker.Clear();
                return album;
            });

        _trackRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Track>()))
            .ReturnsAsync((Track track) =>
            {
                Context.Tracks.Add(track);
                Context.SaveChanges();
                Context.ChangeTracker.Clear();
                return track;
            });
    }

    #region UploadAsync Tests

    [Fact]
    public async Task UploadAsync_WithGenreAndMoodTags_CreatesAlbumWithGenreAndMoodTags()
    {
        var distributor = CreateDistributor(1);
        var genre = GetGenre(1);
        var moodTags = GetMoodTags(1, 2);
        var dto = CreateUploadAlbumDTO(genre.Id, moodTags.Select(mt => mt.Id).ToArray());

        var result = await _service.UploadAsync(dto, distributor.Id);

        result.Should().NotBeNull();
        result.GenreId.Should().Be(genre.Id);

        var albumMoodTags = Context.AlbumMoodTags
            .Where(amt => amt.AlbumId == result.Id)
            .ToList();

        albumMoodTags.Should().HaveCount(2);
        albumMoodTags.Select(amt => amt.MoodTagId).Should().BeEquivalentTo(moodTags.Select(mt => mt.Id));
    }

    [Fact]
    public async Task UploadAsync_WithoutGenreAndMoodTags_CreatesAlbumWithoutGenreAndMoodTags()
    {
        var distributor = CreateDistributor(1);
        var dto = CreateUploadAlbumDTO(null, null);

        var result = await _service.UploadAsync(dto, distributor.Id);

        result.Should().NotBeNull();
        result.GenreId.Should().BeNull();

        var albumMoodTags = Context.AlbumMoodTags
            .Where(amt => amt.AlbumId == result.Id)
            .ToList();

        albumMoodTags.Should().BeEmpty();
    }

    [Fact]
    public async Task UploadAsync_WithMoreThanThreeMoodTags_ThrowsBadRequest()
    {
        var distributor = CreateDistributor(1);
        var dto = CreateUploadAlbumDTO(1, new[] { 1, 2, 3, 4 });

        var exception = await Assert.ThrowsAsync<BadRequestResponse>(
            () => _service.UploadAsync(dto, distributor.Id));

        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task UploadAsync_WithInvalidMoodTagIds_ThrowsBadRequest()
    {
        var distributor = CreateDistributor(1);
        var dto = CreateUploadAlbumDTO(1, new[] { 1, 999 });

        var exception = await Assert.ThrowsAsync<BadRequestResponse>(
            () => _service.UploadAsync(dto, distributor.Id));

        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task UploadAsync_WithMaximumThreeMoodTags_Succeeds()
    {
        var distributor = CreateDistributor(1);
        var moodTags = GetMoodTags(1, 2, 3);
        var dto = CreateUploadAlbumDTO(1, moodTags.Select(mt => mt.Id).ToArray());

        var result = await _service.UploadAsync(dto, distributor.Id);

        result.Should().NotBeNull();

        var albumMoodTags = Context.AlbumMoodTags
            .Where(amt => amt.AlbumId == result.Id)
            .ToList();

        albumMoodTags.Should().HaveCount(3);
    }

    #endregion

    #region CreateTrackAsync Tests

    [Fact]
    public async Task CreateTrackAsync_WithGenreAndMoodTagsInDto_UsesDtoValues()
    {
        var album = CreateAlbum(1, 1, new[] { 1, 2 });
        var genre = GetGenre(2);
        var moodTags = GetMoodTags(3, 4);
        var dto = CreateUploadTrackDTO(genre.Id, moodTags.Select(mt => mt.Id).ToArray());

        SetupAlbumRepositoryMocks(album);

        var result = await _service.CreateTrackAsync(album.Id, dto);

        result.Should().NotBeNull();
        result.GenreId.Should().Be(genre.Id);

        var trackMoodTags = Context.TrackMoodTags
            .Where(tmt => tmt.TrackId == result.Id)
            .ToList();

        trackMoodTags.Should().HaveCount(2);
        trackMoodTags.Select(tmt => tmt.MoodTagId).Should().BeEquivalentTo(moodTags.Select(mt => mt.Id));
    }

    [Fact]
    public async Task CreateTrackAsync_WithoutGenreAndMoodTagsInDto_InheritsFromAlbum()
    {
        var genre = GetGenre(1);
        var moodTags = GetMoodTags(1, 2);
        var album = CreateAlbum(1, genre.Id, moodTags.Select(mt => mt.Id).ToArray());
        var dto = CreateUploadTrackDTO(0, null);

        SetupAlbumRepositoryMocks(album);
        SetupTrackRepositoryMocks();

        var result = await _service.CreateTrackAsync(album.Id, dto);

        result.Should().NotBeNull();
        result.GenreId.Should().Be(genre.Id);

        var trackMoodTags = Context.TrackMoodTags
            .Where(tmt => tmt.TrackId == result.Id)
            .ToList();

        trackMoodTags.Should().HaveCount(2);
        trackMoodTags.Select(tmt => tmt.MoodTagId).Should().BeEquivalentTo(moodTags.Select(mt => mt.Id));
    }

    [Fact]
    public async Task CreateTrackAsync_WithoutGenreInDtoAndAlbum_ThrowsBadRequest()
    {
        var album = CreateAlbum(1, null, null);
        var dto = CreateUploadTrackDTO(0, null);

        SetupAlbumRepositoryMocks(album);
        SetupTrackRepositoryMocks();

        var exception = await Assert.ThrowsAsync<BadRequestResponse>(
            () => _service.CreateTrackAsync(album.Id, dto));

        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateTrackAsync_WithMoreThanThreeMoodTags_ThrowsBadRequest()
    {
        var album = CreateAlbum(1, 1, null);
        var dto = CreateUploadTrackDTO(1, new[] { 1, 2, 3, 4 });

        SetupAlbumRepositoryMocks(album);
        SetupTrackRepositoryMocks();

        var exception = await Assert.ThrowsAsync<BadRequestResponse>(
            () => _service.CreateTrackAsync(album.Id, dto));

        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateTrackAsync_WithInvalidMoodTagIds_ThrowsBadRequest()
    {
        var album = CreateAlbum(1, 1, null);
        var dto = CreateUploadTrackDTO(1, new[] { 1, 999 });

        SetupAlbumRepositoryMocks(album);
        SetupTrackRepositoryMocks();

        var exception = await Assert.ThrowsAsync<BadRequestResponse>(
            () => _service.CreateTrackAsync(album.Id, dto));

        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateTrackAsync_WithEmptyMoodTagIdsInDto_UsesAlbumMoodTags()
    {
        var genre = GetGenre(1);
        var moodTags = GetMoodTags(1, 2);
        var album = CreateAlbum(1, genre.Id, moodTags.Select(mt => mt.Id).ToArray());
        var dto = CreateUploadTrackDTO(0, Array.Empty<int>());

        SetupAlbumRepositoryMocks(album);
        SetupTrackRepositoryMocks();

        var result = await _service.CreateTrackAsync(album.Id, dto);

        result.Should().NotBeNull();

        var trackMoodTags = Context.TrackMoodTags
            .Where(tmt => tmt.TrackId == result.Id)
            .ToList();

        trackMoodTags.Should().HaveCount(2);
    }

    #endregion

    #region GetAlbumTracksAsync Tests

    [Fact]
    public async Task GetAlbumTracksAsync_ReturnsTracksWithGenreAndMoodTags()
    {
        var genre = GetGenre(1);
        var moodTags = GetMoodTags(1, 2);
        var album = CreateAlbum(1, genre.Id, moodTags.Select(mt => mt.Id).ToArray());
        var track = CreateTrackInAlbum(1, album.Id, genre.Id, moodTags.Select(mt => mt.Id).ToArray());

        SetupAlbumRepositoryMocks(album);
        SetupTrackRepositoryMocks();

        var result = await _service.GetAlbumTracksAsync(album.Id, null);

        result.Should().NotBeNullOrEmpty();
        var trackDto = result.First();
        trackDto.Genre.Should().NotBeNull();
        trackDto.Genre.Id.Should().Be(genre.Id);
        trackDto.MoodTags.Should().HaveCount(2);
    }

    #endregion

    #region Helper Methods

    private void SetupAlbumRepositoryMocks(Album album)
    {
        Context.ChangeTracker.Clear();

        _albumRepositoryMock
            .Setup(x => x.Query())
            .Returns(() => Context.Set<Album>().AsQueryable());

        _albumRepositoryMock
            .Setup(x => x.GetTracksFromAlbumAsync(It.IsAny<int>()))
            .ReturnsAsync((int albumId) =>
            {
                Context.ChangeTracker.Clear();
                var albumTracks = Context.Albums
                    .Where(a => a.Id == albumId)
                    .SelectMany(a => a.Tracks)
                    .Select(t => t.Id)
                    .ToList();

                return Context.Tracks
                    .Where(t => albumTracks.Contains(t.Id))
                    .Include(t => t.Genre)
                    .Include(t => t.TrackMoodTags)
                    .ThenInclude(tmt => tmt.MoodTag)
                    .Include(t => t.VisibilityState)
                    .ThenInclude(vs => vs.Status)
                    .ToList();
            });
    }

    private void SetupTrackRepositoryMocks()
    {
        _trackRepositoryMock
            .Setup(x => x.Query())
            .Returns(() => Context.Set<Track>().AsQueryable());
    }

    private UploadAlbumDTO CreateUploadAlbumDTO(int? genreId, int[]? moodTagIds)
    {
        var coverFile = new Mock<IFormFile>();
        coverFile.Setup(f => f.FileName).Returns("cover.jpg");
        coverFile.Setup(f => f.Length).Returns(1024);

        return new UploadAlbumDTO
        {
            Name = "Test Album",
            Cover = coverFile.Object,
            Authors = new[] { "Test Artist" },
            GenreId = genreId,
            MoodTagIds = moodTagIds
        };
    }

    private UploadTrackDTO CreateUploadTrackDTO(int genreId, int[]? moodTagIds)
    {
        var audioFile = new Mock<IFormFile>();
        audioFile.Setup(f => f.FileName).Returns("track.mp3");
        audioFile.Setup(f => f.Length).Returns(2048);

        return new UploadTrackDTO
        {
            Title = "Test Track",
            IsExplicit = false,
            DrivingDisturbingNoises = false,
            LowQualityAudioFile = audioFile.Object,
            MediumQualityAudioFile = null,
            HighQualityAudioFile = null,
            GenreId = genreId,
            MoodTagIds = moodTagIds
        };
    }

    private Album CreateAlbum(int id, int? genreId, int[]? moodTagIds)
    {
        var visibilityState = CreateVisibilityState(id);
        var distributor = CreateDistributor(1);
        var imageFile = CreateImageFile(id * 1000);

        var album = new Album
        {
            Id = id,
            Name = $"Album {id}",
            GenreId = genreId,
            CoverId = imageFile.Id,
            DistributorId = distributor.Id,
            VisibilityStateId = visibilityState.Id
        };

        Context.Albums.Add(album);
        Context.SaveChanges();
        Context.ChangeTracker.Clear();

        if (moodTagIds != null && moodTagIds.Any())
        {
            CreateAlbumMoodTags(album.Id, moodTagIds);
        }

        return album;
    }

    private void CreateAlbumMoodTags(int albumId, int[] moodTagIds)
    {
        Context.ChangeTracker.Clear();
        foreach (var moodTagId in moodTagIds)
        {
            var albumMoodTag = new AlbumMoodTag
            {
                AlbumId = albumId,
                MoodTagId = moodTagId
            };
            Context.AlbumMoodTags.Add(albumMoodTag);
        }
        Context.SaveChanges();
        Context.ChangeTracker.Clear();
    }

    private Track CreateTrackInAlbum(int id, int albumId, int genreId, int[] moodTagIds)
    {
        var album = Context.Albums.First(a => a.Id == albumId);
        var visibilityState = CreateVisibilityState(id + 1000);
        var audioFile = CreateAudioFile(id * 1000 + 1);

        var track = new Track
        {
            Id = id,
            Title = $"Track {id}",
            GenreId = genreId,
            IsExplicit = false,
            DrivingDisturbingNoises = false,
            CoverId = album.CoverId,
            LowQualityAudioFileId = audioFile.Id,
            VisibilityStateId = visibilityState.Id,
            Duration = TimeSpan.FromSeconds(180)
        };

        Context.Tracks.Add(track);
        album.Tracks.Add(track);
        Context.SaveChanges();
        Context.ChangeTracker.Clear();

        if (moodTagIds.Any())
        {
            CreateTrackMoodTags(track.Id, moodTagIds);
        }

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

