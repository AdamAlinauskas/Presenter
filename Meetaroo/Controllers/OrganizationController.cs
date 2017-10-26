using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using System;
using Migrator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon;
using System.IO;
using Amazon.S3.Model;

namespace Meetaroo.Controllers
{

    public class OrganizationController : Controller{

        public  IActionResult UploadPresentation(){
            
            return View();
        }

        [HttpPost]
        public  async Task<IActionResult> UploadPresentation(IFormFile file){    
            var client = new AmazonS3Client("AKIAJNOS24TJ3PWZHKEQ", "+d+qIQ5Uv8dfFTdsdvBd0Hp0Exm5QY2YH1ZL8903", RegionEndpoint.USWest2);
            var transfer = new Amazon.S3.Transfer.TransferUtility(client);

            var request  = new TransferUtilityUploadRequest();
            request.BucketName = "sakjfkls-test-bucket";
            request.InputStream = new MemoryStream();
            request.Key = Guid.NewGuid().ToString();
            await file.CopyToAsync(request.InputStream);

            request.CannedACL = S3CannedACL.AuthenticatedRead;

            await transfer.UploadAsync(request);
            
            return RedirectToAction("UploadPresentation");
        }

        public IActionResult PresentationFiles(){
            var client = new AmazonS3Client("AKIAJNOS24TJ3PWZHKEQ", "+d+qIQ5Uv8dfFTdsdvBd0Hp0Exm5QY2YH1ZL8903", RegionEndpoint.USWest2);
            
            var url = client.GetPreSignedURL(new GetPreSignedUrlRequest{
                BucketName = "sakjfkls-test-bucket",
                Key ="99961adc-3977-4a9c-98de-3c2b1db5cfee", //"db029c0b-8c7d-47d1-b50a-75ddefef414d",
                Expires = DateTime.Now.AddDays(5),
                Verb=  HttpVerb.GET

            });
            
            var dto = new PresentationFilesDto{PresentationUrl = url};
            return View(dto);
        }
//Presentation File
    //https://sakjfkls-test-bucket.s3-us-west-2.amazonaws.com/db029c0b-8c7d-47d1-b50a-75ddefef414d?X-Amz-Expires=432000&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAJNOS24TJ3PWZHKEQ/20171026/us-west-2/s3/aws4_request&X-Amz-Date=20171026T012042Z&X-Amz-SignedHeaders=host&X-Amz-Signature=4525e4c24e2c45a39bf7523033c6d4101055e1f6c643db2c5871bd9ea9e5f6ab    
    }

    public class PresentationFilesDto{
        public string PresentationUrl{get;set;}
    }


}
   
