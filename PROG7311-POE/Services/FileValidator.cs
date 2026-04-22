using Microsoft.AspNetCore.Http;

namespace PROG7311_POE.Services
{
    public static class FileValidator
    {
        public static bool IsValidPdf(IFormFile file, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (file == null)
            {
                errorMessage = "No file provided.";
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".pdf")
            {
                errorMessage = "Only PDF files are allowed.";
                return false;
            }

            if (file.Length == 0)
            {
                errorMessage = "File is empty.";
                return false;
            }

            return true;
        }
    }
}