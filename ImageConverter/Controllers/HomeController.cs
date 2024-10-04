using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

public class HomeController : Controller 
{
    private const long MaxFileSize = 10 * 1024 * 1024;
    private static readonly string[] PermittedExtensions = { ".heic", ".heif", ".jpg", ".jpeg", ".png" };


    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file != null)
        {
            if (file.Length > MaxFileSize)
            {
                ViewBag.Message = "���� ������� �������. ������������ ������ - 10MB";
                return View();
            }

            if (!file.ContentType.Contains("image"))
            {
                ViewBag.Message = "�������� ��� �����. ����������, ��������� �����������";
            }


            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!PermittedExtensions.Contains(fileExtension))
            {
                ViewBag.Message = "������������ ������ �����. ����������, ��������� HEIF, JPG ��� PNG";
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", file.FileName);


            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                using (var image = new MagickImage(filePath))
                {
                    var outputFormat = "jpg"; // ����� �������� �� "png"
                    var outputFilePath = Path.ChangeExtension(filePath, outputFormat);

                    image.Write(outputFilePath);
                }

                ViewBag.Message = "���� ������� �������� � �������������!";
            }
            catch
            {
                ViewBag.Message = "��������� ������ ��� �������� �����. ���������� �����.";
            }

        }
        return View();
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
