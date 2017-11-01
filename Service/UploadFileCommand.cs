
using System;
using System.IO;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using DataAccess;

namespace Service{
    
    public interface IUploadFileCommand{
        void Execute(Stream fileStream, string fileName);
    }
    
    public class UploadFileCommand : IUploadFileCommand{
        private readonly IFileRepository fileRepository;

        public UploadFileCommand(IFileRepository fileRepository){
            this.fileRepository = fileRepository;
        }

        public async void Execute(Stream fileStream, string fileName)
        {
            await UploadFile(fileName, fileStream);
        }

        private static async System.Threading.Tasks.Task<string> UploadFile(string fileName,Stream fileStream)
        {
            var extension = Path.GetExtension(fileName);

            var client = new AmazonS3Client("AKIAJNOS24TJ3PWZHKEQ", "+d+qIQ5Uv8dfFTdsdvBd0Hp0Exm5QY2YH1ZL8903", RegionEndpoint.USWest2);
            var transfer = new Amazon.S3.Transfer.TransferUtility(client);

            var request = new TransferUtilityUploadRequest();
            request.BucketName = "sakjfkls-test-bucket";
            request.InputStream = new MemoryStream();
            request.Key = Guid.NewGuid().ToString() + extension;
            request.InputStream = fileStream;

            request.CannedACL = S3CannedACL.AuthenticatedRead;

            await transfer.UploadAsync(request);
            return request.Key;
        }
    }
}