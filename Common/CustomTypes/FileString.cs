using Microsoft.AspNetCore.Http;

namespace Common.CustomTypes
{
    public class FileString
    {
        #region Constructor
        public FileString(string base64, string name, string extension)
        {
            Base64 = base64;
            Name = name;
            Extension = extension;
        }
        #endregion

        #region Properties
        public string Base64 { get; private set; }
        public string Name { get; private set; }
        public string Extension { get; private set; }
        public string FileName => Name + "." + Extension;
        #endregion

        #region Methods
        public IFormFile ToFormFile()
        {
            byte[] bytes = Convert.FromBase64String(Base64);
            MemoryStream stream = new(bytes);

            return new FormFile(stream, 0, bytes.Length, Name, FileName);
        }
        #endregion
    }
}
