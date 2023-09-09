using System.IO.Compression;

namespace AnnaFest
{
    public class PhotoRepository
    {
        private const string InfoFileName = "Info.txt";
        private const string FolderName = "pictures";
        private static PhotoRepository instance;
        public static PhotoRepository Instance => instance ?? (instance = new PhotoRepository());

        private bool isInitialized = false;
        private bool shouldShowNew = false;
        private IList<PhotoModel> photos;
        private IList<PhotoModel> oldPhotos;
        private DateTime? lastShownDateTime = null;
        private Guid lastShownId;
        private Guid lastShownNewId;
        private object lockObject = new object();
        private Dictionary<string, string> themeByYear = new Dictionary<string, string>
        {
            { "2015", "Disney"},
            { "2016", "Årtal" },
            { "2017", "Förtrollad skog" },
            { "2018", "Barbie" },
            { "2019", "Barnkalas" },
            { "2020", "Djur" },
            { "2021", "Konst" },
            { "2022", "Glitter" }
        };

        public static string RelativePath(PhotoModel model) => $"/{FolderName}/{model.FileName}";

        public bool AddFile(string rootPath, IFormFile formFile, string description, string user)
        {
            this.Initialize(rootPath);
            lock (this.lockObject)
            {
                try
                {
                    var fileName = formFile.FileName.Replace("'", "").Replace("\"", "");
                    var file = Path.Combine(rootPath, FolderName, fileName);
                    using (var fs = new FileStream(file, FileMode.Create))
                    {
                        formFile.CopyTo(fs);
                    }

                    var photo = new PhotoModel
                    {
                        FileName = fileName,
                        Description = description,
                        Id = Guid.NewGuid(),
                        User = user,
                        IsDefault = false
                    };

                    this.photos.Add(photo);
                    File.AppendAllText(Path.Combine(rootPath, FolderName, InfoFileName), photo.ToString() + '\n');
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public void DownloadZip(string rootPath, MemoryStream memoryStream)
        {
            lock (this.lockObject)
            {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var photo in this.photos)
                        {
                            try
                            {
                                var fileName = photo.FileName;
                                var modifiedUserName = string.IsNullOrEmpty(photo.User) ? string.Empty : string.Concat(photo.User.Split(Path.GetInvalidFileNameChars())) + "-";
                                var file = Path.Combine(rootPath, FolderName, fileName);
                                var fileBytes = File.ReadAllBytes(file);
                                var fileEnding = fileName.Split('.')[1];
                                var fileBeginning = fileName.Substring(0, fileName.Length - fileEnding.Length - 1);
                                var zipArchiveEntry = archive.CreateEntry(modifiedUserName + fileBeginning + "." + fileEnding, CompressionLevel.Fastest);
                                using (var zipStream = zipArchiveEntry.Open())
                                    zipStream.Write(fileBytes, 0, fileBytes.Length);
                            } catch { }
                        }
                    }
            }
        }

        public IList<PhotoModel> GetAllPhotos(string rootPath)
        {
            this.Initialize(rootPath);
            lock (this.lockObject)
            {
                return new List<PhotoModel>(this.photos);
            }
        }

        public PhotoModel GetCurrentPhoto(string rootPath)
        {
            this.Initialize(rootPath);
            lock (this.lockObject)
            {
                if (this.photos.Count == 0 && this.oldPhotos.Count == 0)
                {
                    return null;
                }

                if (this.lastShownDateTime == null)
                {
                    var photo = this.GetRandomOldPhoto();
                    SetLastShownPhoto(photo);
                    return photo;
                }

                var lastShownPhoto = this.photos.FirstOrDefault(x => x.Id == this.lastShownId) ?? this.oldPhotos.FirstOrDefault(x => x.Id == this.lastShownId);
                if (lastShownPhoto == null)
                {
                    var photo = this.GetRandomOldPhoto();
                    SetLastShownPhoto(photo);
                    return photo;
                }

                if (this.lastShownDateTime.Value.AddSeconds(10) < DateTime.Now)
                {
                    PhotoModel photo;
                    if (this.shouldShowNew && this.photos.Count > 0)
                    {
                        var lastShownNewPhoto = this.photos.FirstOrDefault(x => x.Id == this.lastShownNewId);
                        if (lastShownNewPhoto == null)
                        {
                            photo = this.photos[0];
                        }
                        else
                        {
                            var currentIndex = this.photos.IndexOf(lastShownNewPhoto);
                            photo = currentIndex < this.photos.Count - 1 ? this.photos[currentIndex + 1] : this.photos[0];
                        }
                    }
                    else
                    {
                        photo = this.GetRandomOldPhoto();
                    }

                    this.shouldShowNew = !this.shouldShowNew;
                    SetLastShownPhoto(photo);
                    return photo;
                }
                else
                {
                    return lastShownPhoto;
                }
            }
        }

        private PhotoModel GetRandomOldPhoto()
        {
            var r = new Random();
            return this.oldPhotos[r.Next(0, this.oldPhotos.Count - 1)];
        }

        public void DeletePhoto(Guid photoId, string rootPath)
        {
            this.Initialize(rootPath);
            var target = this.photos.FirstOrDefault(x => x.Id == photoId);
            if (target == null)
            {
                return;
            }

            if (this.lastShownId == photoId)
            {
                var currentIndex = this.photos.IndexOf(target);
                var photo = currentIndex < this.photos.Count - 1 ? this.photos[currentIndex + 1] : this.photos[0];
                SetLastShownPhoto(photo);
            }

            this.photos.Remove(target);
            var filePath = Path.Combine(rootPath, FolderName, target.FileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            this.RewriteFile(rootPath);
        }

        private void RewriteFile(string rootPath)
        {
            File.WriteAllLines(Path.Combine(rootPath, FolderName, InfoFileName), this.photos.Concat(this.oldPhotos).Select(x => x.ToString()));
        }

        private void SetLastShownPhoto(PhotoModel photo)
        {
            if (photo != null)
            {
                this.lastShownDateTime = DateTime.Now;
                this.lastShownId = photo.Id;
                this.shouldShowNew = photo.IsDefault;
                if (!photo.IsDefault)
                {
                    this.lastShownNewId = photo.Id;
                }
            }
        }

        private void Initialize(string rootPath)
        {
            if (this.isInitialized)
            {
                return;
            }

            lock (this.lockObject)
            {
                this.isInitialized = true;
                this.photos = new List<PhotoModel>();
                this.oldPhotos = new List<PhotoModel>();
                var infoFilePath = Path.Combine(rootPath, FolderName, InfoFileName);

                if (!File.Exists(infoFilePath))
                {
                    var d = new DirectoryInfo(Path.Combine(rootPath, FolderName));
                    var photos = d.GetFiles().Select(x => x.Name);
                    this.oldPhotos = photos.Select(x => new PhotoModel
                    {
                        FileName = x,
                        Id = Guid.NewGuid(),
                        Description = "",
                        User = "",
                        IsDefault = false
                    }).ToList();
                    var directories = d.GetDirectories();
                    foreach (var dir in directories)
                    {
                        var files = dir.GetFiles();
                        var description = "Maskerad " + dir.Name;
                        if (this.themeByYear.TryGetValue(dir.Name, out var theme))
                        {
                            description += " " + theme;
                        }

                        foreach (var file in files)
                        {
                            this.oldPhotos.Add(new PhotoModel
                            {
                                FileName = dir.Name + "/" + file.Name,
                                Id = Guid.NewGuid(),
                                Description = description,
                                User = "",
                                IsDefault = true
                            });
                        }
                    }
                    this.RewriteFile(rootPath);
                    return;
                }

                try
                {
                    var info = File.ReadAllText(Path.Combine(rootPath, FolderName, InfoFileName)).Split("\n", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var i in info)
                    {
                        try
                        {
                            var tokens = i.Split('¤');
                            var photo = new PhotoModel
                            {
                                FileName = tokens[0],
                                Description = tokens[1],
                                Id = Guid.Parse(tokens[2]),
                                User = tokens[3],
                                IsDefault = tokens[4].Substring(0, 3) == "old"
                            };
                            if (photo.IsDefault)
                            {
                                this.oldPhotos.Add(photo);
                            }
                            else
                            {
                                photos.Add(photo);
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }
    }

    public class PhotoModel
    {
        public string FileName { get; set; }

        public string Description { get; set; }

        public Guid Id { get; set; }

        public string User { get; set; }

        public bool IsDefault { get; set; }

        public override string ToString() => FileName + "¤" + Description + "¤" + Id + "¤" + User + "¤" + (IsDefault ? "old" : "new");

        public PhotoJsonModel GetJsonModel()
        {
            return new PhotoJsonModel
            {
                description = this.Description,
                fileName = PhotoRepository.RelativePath(this),
                id = this.Id.ToString(),
                user = IsDefault ? "" : ("Maskerad 2023 Met gala" + (!string.IsNullOrEmpty(User) ?  ": " + User : ""))
            };
        }
    }

    public class PhotoJsonModel
    {
        public string fileName { get; set; }

        public string description { get; set; }

        public string id { get; set; }

        public string user { get; set; }
    }
}
