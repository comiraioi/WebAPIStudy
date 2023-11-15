using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {

        #region Upload

        // POST: https://localhost:7202/api/Images/Upload
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto request)
        {
            return Ok();
        }

        // 파일 유효성 검사
        private void ValidateFileUpload(ImageUploadRequestDto request)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", "png" };    // 확장자
            {
            if(allowedExtensions.Contains(Path.GetExtension(request.File.FileName)) == false)
            {

            }
        }

        #endregion
    }
}
