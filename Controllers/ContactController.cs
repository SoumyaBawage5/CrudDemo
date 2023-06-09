using Microsoft.AspNetCore.Mvc;
using CrudDemo.Data;
using CrudDemo.Models;
using Microsoft.EntityFrameworkCore;
using CrudDemo.Settings;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Routing.Constraints;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using System.Runtime;
using Amazon;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.IO;
using static Amazon.S3.Util.S3EventNotification;

namespace CrudDemo.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ContactController : Controller
    {
        private readonly CrudDemoDbContext dbContext;

        private readonly IOptions<AwsConfig> awsConfig;
        private string objectKey;

        public ContactController(CrudDemoDbContext dbContext, IOptions<AwsConfig> awsConfig)
        {
            this.dbContext = dbContext;
            this.awsConfig = awsConfig;
        }
        

        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
           
            return Ok(await dbContext.Contacts.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddContacts([FromForm] ReqFromUser reqFromUser) 
        {
           if(reqFromUser.PngFile.ContentType!="image/png")
            {
                return BadRequest();
            }
            var filePath = $"{awsConfig.Value.S3BucketFolder}/{reqFromUser.PngFile.FileName}";
            var s3Client = new AmazonS3Client(awsConfig.Value.Access_Key, awsConfig.Value.Secret_Key,RegionEndpoint.GetBySystemName(awsConfig.Value.Region));
            using var stream = new MemoryStream(); 
            await reqFromUser.PngFile.CopyToAsync(stream); 
            var contact = new Contact()
            {
               
               // Id= reqFromUser.Id,
                Name = reqFromUser.Name,
                Email = reqFromUser.Email,
                Phone = reqFromUser.Phone,
                Address = reqFromUser.Address,
                filePath = filePath
            };
            PutObjectRequest request = new PutObjectRequest()
            {
                InputStream = stream,
                BucketName = awsConfig.Value.S3Bucket,
                Key = filePath
            };
            await s3Client.PutObjectAsync(request);
            


            await dbContext.Contacts.AddAsync(contact);
            await dbContext.SaveChangesAsync();

            return Ok(contact);


        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateContacts([FromForm] ReqFromUser reqFromUser, [FromRoute] Guid id)
        {

            var contacts = await dbContext.Contacts.FindAsync(id);

            if (contacts == null)
            {
                return NotFound();

            }
            if (reqFromUser.PngFile.ContentType != "image/png")
            {
                return BadRequest();
            }
            
            var filePath = $"{awsConfig.Value.S3BucketFolder}/{contacts.Id}.png";
            var s3Client = new AmazonS3Client(awsConfig.Value.Access_Key, awsConfig.Value.Secret_Key, RegionEndpoint.GetBySystemName(awsConfig.Value.Region));
            using var stream = new MemoryStream();
            await reqFromUser.PngFile.CopyToAsync(stream);
            var contact = new Contact()
            {
                Id=contacts.Id,
                Name = reqFromUser.Name,
                Email = reqFromUser.Email,
                Phone = reqFromUser.Phone,
                Address = reqFromUser.Address,
                filePath = filePath
            };
            PutObjectRequest request = new PutObjectRequest()
            {
                InputStream = stream,
                BucketName = awsConfig.Value.S3Bucket, 
                Key = filePath
            };
            await s3Client.PutObjectAsync(request);



            //dbContext.Contacts.Update(contact);
            dbContext.Entry(contacts).CurrentValues.SetValues(contact);
            await dbContext.SaveChangesAsync();

            //contact.Id = id;

            return Ok(contact);




        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetContact([FromRoute] Guid id)
        {
            var contact = await dbContext.Contacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound();

            }



            return Ok(contact);


        }

        [HttpGet]
        [Route("api/[controller]/download/image")]
        public async Task<IActionResult> DownloadImage(string bucketName, string keyName)
        {
            var credentials = new Amazon.Runtime.BasicAWSCredentials(awsConfig.Value.Access_Key, awsConfig.Value.Secret_Key); // Replace with your own AWS access key and secret key
            var awsconfig = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsConfig.Value.Region) // Replace with the appropriate region where your S3 bucket is located
            };

            using (var s3client = new AmazonS3Client(credentials, awsconfig))
            {


               
                             var transferUtility = new TransferUtility(s3client);
                             var memoryStream = new MemoryStream();
                             await transferUtility.DownloadAsync(memoryStream.ToString(), bucketName, keyName);

                             memoryStream.Position = 0;
                             return File(memoryStream, keyName);  // Modify the content type according to your image format
                         }
                     }


        /*var s3Client = new AmazonS3Client(awsConfig.Value.Access_Key, awsConfig.Value.Secret_Key, RegionEndpoint.GetBySystemName(awsConfig.Value.Region));
        // Create a TransferUtility instance to download the file
        var transferUtility = new TransferUtility(s3Client);
        // Create a MemoryStream to hold the downloaded file data
        var memoryStream = new MemoryStream(); 
        try 
        { 
            // Download the file from the S3 bucket to the MemoryStream
            await transferUtility.DownloadAsync(memoryStream.ToString(), bucketName, keyName);
            // Set the position of the MemoryStream to the beginning
            memoryStream.Position = 0;
            // Return the file as a FileStreamResult with appropriate content type and content-disposition headers
            return new FileStreamResult(memoryStream, "passport.png")
            {
                FileDownloadName = keyName
            };
        }
        catch (AmazonS3Exception ex)
        {
            // Handle any S3 exception here
            return BadRequest("Error downloading file from S3: " + ex.Message);
        } 
        finally 
        { // Dispose of the MemoryStream and TransferUtility

          memoryStream.Dispose(); 
          transferUtility.Dispose();
        }
    }

        /*[HttpGet("DownloadFile")]
        public async Task<ActionResult> DownloadFile(string NameFile)
        {
            // ... code for validation and get the file
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files",
                   NameFile);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, contentType, Path.GetFileName(filePath));
        }*/

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DelContact([FromRoute] Guid id)
        {
            var contact = await dbContext.Contacts.FindAsync(id);

            if (contact != null)
            {
                dbContext.Remove(contact);
                await dbContext.SaveChangesAsync();
                return Ok(contact);
            }
            return NotFound();


        }


        



    }
}

