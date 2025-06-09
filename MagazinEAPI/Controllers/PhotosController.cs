using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagazinEAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotoController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private const string BucketName = "photos";

        public PhotoController(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] PhotoUploadRequest request)
        {
            var file = request.File;
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!await _s3Client.DoesS3BucketExistAsync(BucketName))
                await _s3Client.PutBucketAsync(BucketName);

            using var stream = file.OpenReadStream();
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = file.FileName,
                BucketName = BucketName,
                ContentType = file.ContentType
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

            var fileUrl = $"http://localhost:9000/{BucketName}/{file.FileName}";
            return Ok(new { Url = fileUrl });
        }


        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetPhoto(string fileName)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = BucketName,
                    Key = fileName
                };

                var response = await _s3Client.GetObjectAsync(request);

                var memoryStream = new MemoryStream();
                await response.ResponseStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                return File(memoryStream, response.Headers.ContentType ?? "application/octet-stream", fileName);
            }
            catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound("File not found");
            }
        }




        [HttpGet("list")]
        public async Task<IActionResult> ListPhotos()
        {
            var request = new ListObjectsV2Request
            {
                BucketName = BucketName
            };

            var result = await _s3Client.ListObjectsV2Async(request);
            var files = result.S3Objects.Select(o => o.Key).ToList();
            return Ok(files);
        }
    }
    public class PhotoUploadRequest
    {
        public IFormFile File { get; set; }
    }

}
