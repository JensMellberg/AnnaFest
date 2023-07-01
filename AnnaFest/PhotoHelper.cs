namespace AnnaFest
{
    public class PhotoRepository
    {
        private const string InfoFileName = "Info.txt";
        private const string FolderName = "pictures";
        private static PhotoRepository instance;
        public static PhotoRepository Instance => instance ?? (instance = new PhotoRepository());

        private bool isInitialized = false;
        private IList<PhotoModel> photos;
        private DateTime? lastShownDateTime = null;
        private Guid lastShownId;

        public static string RelativePath(PhotoModel model) => $"/{FolderName}/{model.FileName}";

        public bool AddFile(string rootPath, IFormFile formFile, string description, string user)
        {
            this.Initialize(rootPath);
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
                    User = user
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

        public IList<PhotoModel> GetAllPhotos(string rootPath)
        {
            this.Initialize(rootPath);
            return this.photos;
        }

        public PhotoModel GetCurrentPhoto(string rootPath)
        {
            this.Initialize(rootPath);
            if (this.photos.Count == 0)
            {
                return null;
            }

            if (this.lastShownDateTime == null)
            {
                var photo = this.photos[0];
                SetLastShownPhoto(photo);
                return photo;
            }

            var lastShownPhoto = this.photos.FirstOrDefault(x => x.Id == this.lastShownId);
            if (lastShownPhoto == null)
            {
                var photo = this.photos[0];
                SetLastShownPhoto(photo);
                return photo;
            }

            var currentIndex = this.photos.IndexOf(lastShownPhoto);
            if (this.lastShownDateTime.Value.AddSeconds(10) < DateTime.Now)
            {
                var photo = currentIndex < this.photos.Count - 1 ? this.photos[currentIndex + 1] : this.photos[0];
                SetLastShownPhoto(photo);
                return photo;
            }
            else
            {
                return lastShownPhoto;
            }
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
            File.WriteAllLines(Path.Combine(rootPath, FolderName, InfoFileName), this.photos.Select(x => x.ToString()));
        }

        private void SetLastShownPhoto(PhotoModel photo)
        {
            if (photo != null)
            {
                this.lastShownDateTime = DateTime.Now;
                this.lastShownId = photo.Id;
            }
        }

        private void Initialize(string rootPath)
        {
            if (this.isInitialized)
            {
                return;
            }

            this.isInitialized = true;
            this.photos = new List<PhotoModel>();
            var infoFilePath = Path.Combine(rootPath, FolderName, InfoFileName);

            if (!File.Exists(infoFilePath))
            {
                File.Create(infoFilePath);
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
                        photos.Add(new PhotoModel
                        {
                            FileName = tokens[0],
                            Description = tokens[1],
                            Id = Guid.Parse(tokens[2]),
                            User = tokens[3]
                        });
                    } catch { }
                }
            }
            catch { }
        }
    }

    public class PhotoModel
    {
        public string FileName { get; set; }

        public string Description { get; set; }

        public Guid Id { get; set; }

        public string User { get; set; }

        public override string ToString() => FileName + "¤" + Description + "¤" + Id + "¤" + User;

        public PhotoJsonModel GetJsonModel()
        {
            return new PhotoJsonModel
            {
                description = this.Description,
                fileName = PhotoRepository.RelativePath(this),
                id = this.Id.ToString(),
                user = User
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
