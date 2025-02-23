using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using AWSS3.Models;

namespace AWSS3.Services
{
    public class StorageService : IStorageService
    {
        public async Task<S3ResponseDto> UploadFileAsync(S3Object s3Object, AwsCredentials awsCredentials)
        {
            var credentials = new BasicAWSCredentials(awsCredentials.AwsKey, awsCredentials.AwsSecretKey);

            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.EUNorth1
            };

            var response = new S3ResponseDto();

            try
            {
                var uploadRequest = new TransferUtilityUploadRequest()
                {
                    InputStream = s3Object.InputStream,
                    Key = s3Object.Name,
                    BucketName = s3Object.BucketName,
                    CannedACL = S3CannedACL.NoACL
                };

                using var client = new AmazonS3Client(credentials, config);

                var transferUtility = new TransferUtility(client);

                await transferUtility.UploadAsync(uploadRequest);

                response.StatusCode = 200;
                response.Message = $"{s3Object.Name} has been uploaded successfully";
            }
            catch (AmazonS3Exception ex)
            {
                response.StatusCode = (int)ex.StatusCode;
                response.Message = ex.Message;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<S3ResponseDto> DeleteFileAsync(S3Object s3Object, AwsCredentials awsCredentials)
        {
            var credentials = new BasicAWSCredentials(awsCredentials.AwsKey, awsCredentials.AwsSecretKey);
            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.EUNorth1
            };

            var response = new S3ResponseDto();

            try
            {
                using var client = new AmazonS3Client(credentials, config);

                var deleteRequest = new Amazon.S3.Model.DeleteObjectRequest()
                {
                    BucketName = s3Object.BucketName,
                    Key = s3Object.Name
                };

                await client.DeleteObjectAsync(deleteRequest);

                response.StatusCode = 200;
                response.Message = $"{s3Object.Name} has been deleted successfully";
            }
            catch (AmazonS3Exception ex)
            {
                response.StatusCode = (int)ex.StatusCode;
                response.Message = ex.Message;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return response;
        }


        public async Task<S3ResponseDto> GetPreSignedUrlAsync(S3Object s3Object, AwsCredentials awsCredentials)
        {
            var credentials = new BasicAWSCredentials(awsCredentials.AwsKey, awsCredentials.AwsSecretKey);
            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.EUNorth1
            };

            var response = new S3ResponseDto();

            try
            {
                using var client = new AmazonS3Client(credentials, config);

                var request = new Amazon.S3.Model.GetPreSignedUrlRequest()
                {
                    BucketName = s3Object.BucketName,
                    Key = s3Object.Name,
                    Expires = DateTime.UtcNow.AddMinutes(15)
                };

                var url = client.GetPreSignedURL(request);

                response.StatusCode = 200;
                response.Message = "Pre-signed URL generated successfully";
                response.PreSignedUrl = url;
            }
            catch (AmazonS3Exception ex)
            {
                response.StatusCode = (int)ex.StatusCode;
                response.Message = ex.Message;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }

            return response;
        } 
    }
}
