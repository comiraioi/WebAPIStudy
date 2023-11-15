using System.ComponentModel.DataAnnotations.Schema;

namespace NZWalks.API.Models.Domain
{
    public class Image
    {
        public Guid Id { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }     // 실제 파일

        public string FileName {  get; set; }   // 파일명

        public string? FileDescription { get; set; }

        public string FileExtension {  get; set; }  // 확장자

        public long FileSizeInBytes { get; set; }   // 파일 사이즈

        public string FilePath { get; set; }    // 파일 경로
    }
}
