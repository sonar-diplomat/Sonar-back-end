using Application.Abstractions.Interfaces.Repository.File;
using Application.Abstractions.Interfaces.Services.File;
using Application.Response;
using Entities.Models.File;
using FileSignatures;
using Microsoft.AspNetCore.Http;
using NAudio.Wave;

namespace Application.Services.File;

public class AudioFileService(
    IAudioFileRepository repository,
    IFileStorageService fileStorageService,
    IFileStorageService fileStorage,
    IFileFormatInspector inspector
) : FileService<AudioFile>(repository, fileStorage, inspector), IAudioFileService
{
    //public async Task<FileStream?> GetMusicStreamAsync(int fileId, long? startByte, long? length)
    //{
    //    return new FileStream("C:\\Users\\timpf\\Music\\Нова папка\\Infected Mushroom - Guitarmass.aac", FileMode.Open,
    //        FileAccess.Read);
    //}

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
        string tempFilePath = Path.GetTempFileName();
        try
        {
            TimeSpan duration;
            byte[] songBytes;
            using (MemoryStream ms = new())
            {
                await file.CopyToAsync(ms);
                songBytes = ms.ToArray();
            }

            await System.IO.File.WriteAllBytesAsync(tempFilePath, songBytes);
            await using (AudioFileReader reader = new(tempFilePath))
            {
                duration = reader.TotalTime;
            }

            return duration;
        }
        catch (Exception)
        {
            throw ResponseFactory.Create<InternalServerErrorResponse>(["Error while processing audio file"]);
        }
        finally
        {
            try
            {
                if (System.IO.File.Exists(tempFilePath))
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
                // Convert TimeSpan to byte position using NAudio
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
        catch (Exception ex)
        {
            throw ResponseFactory.Create<InternalServerErrorResponse>([$"Unexpected error in file service: {ex.Message}"]);
        }
    }

    private async Task<long> ConvertTimeSpanToBytePositionAsync(string filePath, TimeSpan position)
    {
        try
        {
            await using (AudioFileReader reader = new(filePath))
            {
                if (position >= reader.TotalTime)
                {
                    return reader.Length;
                }

                // Calculate byte position based on time position
                // This is an approximation - for more accurate results, we'd need to parse the audio file format
                double positionRatio = position.TotalSeconds / reader.TotalTime.TotalSeconds;
                long estimatedBytePosition = (long)(reader.Length * positionRatio);

                return estimatedBytePosition;
            }
        }
        catch (Exception)
        {
            throw ResponseFactory.Create<InternalServerErrorResponse>(["Error while calculating audio file position"]);
        }
    }
}