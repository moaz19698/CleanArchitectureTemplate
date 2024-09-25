using Common.CustomTypes;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Common.CustomTypes.Extensions
{

    public static class FileStringListExtension
    {
        public static List<IFormFile> ToFormFiles(this List<FileString> stringFile)
        {
            List<IFormFile> formFiles = [];

            foreach (var fileString in stringFile)
            {
                formFiles.Add(fileString.ToFormFile());
            }
            return formFiles;
        }


    }
}
