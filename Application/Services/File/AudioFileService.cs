using Application.Abstractions.Interfaces.Repository.File;
using Application.Abstractions.Interfaces.Services.File;
using Application.Response;
using Entities.Models.File;
using FileSignatures;
using Microsoft.AspNetCore.Http;

using TagLib;

namespace Application.Services.File;

public class AudioFileService(
    IAudioFileRepository repository,
    IFileStorageService fileStorageService,
    IFileStorageService fileStorage,
    IFileFormatInspector inspector
) : FileService<AudioFile>(repository, fileStorage, inspector), IAudioFileService
{

    public async Task<AudioFile> UploadFileAsync(IFormFile file)
    {
        await ValidateFileType(file, [typeof(Mp3), typeof(Flac)]);
        string url = await fileStorageService.SaveAudioFileAsync(file);
        AudioFile fileModel = new()
        {
            ItemName = file.FileName,
            Url = url
        };

        return await CreateAsync(fileModel);
    }

    public async Task<TimeSpan> GetDurationAsync(IFormFile file)
    {
        string? tempFilePath = null;
        try
        {
            byte[] songBytes;
            using (MemoryStream ms = new())
            {
                await file.CopyToAsync(ms);
                songBytes = ms.ToArray();
            }

            // Создаем временный файл с правильным расширением из имени исходного файла
            string extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(extension))
            {
                // Если расширение не указано, пытаемся определить по содержимому
                extension = ".mp3"; // По умолчанию MP3, так как это наиболее распространенный формат
            }
            
            tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{extension}");
            await System.IO.File.WriteAllBytesAsync(tempFilePath, songBytes);
            
            using (var tagFile = TagLib.File.Create(tempFilePath))
            {
                return tagFile.Properties.Duration;
            }
        }
        catch (Exception ex)
        {
            throw ResponseFactory.Create<InternalServerErrorResponse>([$"Error while processing audio file : {ex.Message}"]);
        }
        finally
        {
            try
            {
                if (tempFilePath != null && System.IO.File.Exists(tempFilePath))
                    System.IO.File.Delete(tempFilePath);
            }
            catch
            {
                // ignored
            }
        }
    }

    public async Task<FileStream?> GetMusicStreamAsync(int fileId, TimeSpan? startPosition, TimeSpan? length)
    {
        AudioFile file = await GetByIdValidatedAsync(fileId);

        try
        {
            if (!System.IO.File.Exists(file.Url))
            {
                throw ResponseFactory.Create<NotFoundResponse>([$"Audio file with ID {fileId} not found"]);
            }

            FileStream fileStream = new(
                file.Url,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read, // Позволяет другим процессам читать файл
                bufferSize: 4096, // Стандартный размер буфера
                useAsync: true // Асинхронный режим
            );

            if (startPosition.HasValue)
            {
                // Convert TimeSpan to byte position using TagLib
                long startByte = await ConvertTimeSpanToBytePositionAsync(file.Url, startPosition.Value);

                if (startByte < 0 || startByte >= fileStream.Length)
                {
                    fileStream.Dispose();
                    throw ResponseFactory.Create<BadRequestResponse>([$"Invalid start position: {startPosition.Value}"]);
                }

                // Установка позиции в потоке
                fileStream.Seek(startByte, SeekOrigin.Begin);

                // Note: length parameter is available but not used for stream limiting
                // To implement length limiting, we would need to wrap the stream
            }

            return fileStream;
        }
        catch (IOException ex)
        {
            throw ResponseFactory.Create<InternalServerErrorResponse>([$"IO error while accessing file: {ex.Message}"]);
        }
        catch (Application.Response.Response ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ResponseFactory.Create<InternalServerErrorResponse>([$"Unexpected error in file service: {ex.Message}"]);
        }
    }

    private async Task<long> ConvertTimeSpanToBytePositionAsync(string filePath, TimeSpan position)
    {
        try
        {
            using (var tagFile = TagLib.File.Create(filePath))
            {
                var totalTime = tagFile.Properties.Duration;
                var fileInfo = new System.IO.FileInfo(filePath);
                var fileLength = fileInfo.Length;

                if (position >= totalTime)
                {
                    return fileLength;
                }

                // Calculate byte position based on time position
                // This is an approximation - for more accurate results, we'd need to parse the audio file format
                double positionRatio = position.TotalSeconds / totalTime.TotalSeconds;
                long estimatedBytePosition = (long)(fileLength * positionRatio);

                return estimatedBytePosition;
            }
        }
        catch (Exception)
        {
            throw ResponseFactory.Create<InternalServerErrorResponse>(["Error while calculating audio file position"]);
        }
    }
}