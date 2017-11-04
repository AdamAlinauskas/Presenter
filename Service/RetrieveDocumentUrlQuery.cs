using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using DataAccess;
using Dto;

namespace Service{
    public interface IRetrieveDocumentUrlQuery{
        Task<ViewDocumentDto> FetchAsync(long id);
    }
    
    public class RetrieveDocumentUrlQuery : IRetrieveDocumentUrlQuery{
        private readonly IDocumentRepository documentRepository;

        public RetrieveDocumentUrlQuery(IDocumentRepository documentRepository)
        {
            this.documentRepository = documentRepository;
        }

        public async Task<ViewDocumentDto> FetchAsync(long id){
            var key = await documentRepository.GetAwsKeyFor(id);
            var client = new AmazonS3Client("AKIAJNOS24TJ3PWZHKEQ", "+d+qIQ5Uv8dfFTdsdvBd0Hp0Exm5QY2YH1ZL8903", RegionEndpoint.USWest2);
            
            var url = client.GetPreSignedURL(new GetPreSignedUrlRequest{
                BucketName = "sakjfkls-test-bucket",
                Key =key, //"db029c0b-8c7d-47d1-b50a-75ddefef414d",
                Expires = DateTime.Now.AddDays(1),
                Verb=  HttpVerb.GET
            });

            return new ViewDocumentDto{Url = url};
        }
    }
}