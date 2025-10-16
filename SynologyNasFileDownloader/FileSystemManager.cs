using System.IO.Compression;

namespace SynologyNas
{
    public class FileSystemManager
    {
        public async Task EnsureDirectoryExistsAsync(string localSavePath)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (!Directory.Exists(localSavePath))
                    {
                        Directory.CreateDirectory(localSavePath);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Путь к папке указан не верно, укажите корректный путь! {ex.Message}");
                }
            });
        }

        public async Task<string> SaveFileFromStreamAsync(Stream inputStream, string localSavePath, string fileName)
        {
            return await Task.Run(() =>
            {
                var filePath = Path.Combine(localSavePath, fileName);
                using (var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    inputStream.CopyTo(outputStream);
                }
                return filePath;
            });
        }

        public async Task<List<string>> ExtractArchiveFromStreamAsync(Stream inputStream, string localSavePath)
        {
            return await Task.Run(() =>
            {
                var extractedFiles = new List<string>();
                using (var archive = new ZipArchive(inputStream, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        string destinationPath = Path.Combine(localSavePath, entry.FullName);
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
                        entry.ExtractToFile(destinationPath, overwrite: true);
                        extractedFiles.Add(destinationPath);
                    }
                }
                return extractedFiles;
            });
        }
    }
}