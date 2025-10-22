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
    public async Task<FileStream?> GetMusicStreamAsync(int fileId, long? startByte, long? length)
    {
        return new FileStream("C:\\Users\\timpf\\Music\\Нова папка\\Infected Mushroom - Guitarmass.aac", FileMode.Open,
            FileAccess.Read);
    }

    public async Task<AudioFile> UploadFileAsync(IFormFile file)
    {
        await ValidateFileType(file, [typeof(Mp3), typeof(Flac)]);
        string url = await fileStorageService.SaveAudioFileAsync(file);
        AudioFile fileModel = new()
        {
            ItemName = file.FileName,
            Duration = await GetDurationAsync(file),
            Url = url
        };

        return await CreateAsync(fileModel);
    }

    private async Task<TimeSpan> GetDurationAsync(IFormFile file)
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

    //public async Task<FileStream?> GetMusicStreamAsync(int fileId, long? startByte, long? length)
    //{
    //    FileModel file = await GetByIdValidatedAsync(fileId);

    //    try
    //    {
    //        if (!System.IO.File.Exists(file.Url))
    //        {
    //            throw AppExceptionFactory.Create<Exception.FileNotFoundException>();
    //        }

    //        FileStream fileStream = new FileStream(
    //            file.Url,
    //            FileMode.Open,
    //            FileAccess.Read,
    //            FileShare.Read, // Позволяет другим процессам читать файл
    //            bufferSize: 4096, // Стандартный размер буфера
    //            useAsync: true // Асинхронный режим
    //        );
    //        if (startByte.HasValue && length.HasValue)
    //        {

    //            if (startByte.Value < 0 || startByte.Value >= fileStream.Length)
    //            {
    //                fileStream.Dispose();
    //                throw new ArgumentException();
    //            }

    //            if (length.Value <= 0 || startByte.Value + length.Value > fileStream.Length)
    //            {
    //                fileStream.Dispose();
    //                throw new ArgumentException();
    //            }

    //            // Установка позиции в потоке
    //            fileStream.Seek(startByte.Value, SeekOrigin.Begin);
    //        }
    //        return fileStream;
    //    }
    //    catch (IOException ex)
    //    {
    //        throw new IOException($"IO error while accessing file: {ex.Message}");
    //    }
    //    catch (System.Exception ex)
    //    {
    //        throw new System.Exception($"Unexpected error in file service: {ex.Message}");
    //    }
    //}
}