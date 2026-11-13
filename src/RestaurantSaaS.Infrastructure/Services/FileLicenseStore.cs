using RestaurantSaaS.Application.Abstractions;

namespace RestaurantSaaS.Infrastructure.Services;

public sealed class FileLicenseStore(LicenseOptions options) : ILicenseFileStore
{
    private string FilePath => string.IsNullOrWhiteSpace(options.FilePath)
        ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RestoPOS", "license.lic")
        : options.FilePath;

    public string? Read() => File.Exists(FilePath) ? File.ReadAllText(FilePath) : null;

    public void Write(string content)
    {
        var directory = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var temp = FilePath + ".tmp";
        File.WriteAllText(temp, content);
        File.Move(temp, FilePath, overwrite: true);
    }

    // ADD inside the class, after Write():
    public void Delete()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }
    }
}
