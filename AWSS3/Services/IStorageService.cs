
using AWSS3.Models;

namespace AWSS3.Services
{
    public interface IStorageService
    {
        Task<S3ResponseDto> UploadFileAsync(S3Object s3Object, AwsCredentials awsCredentials);
        Task<S3ResponseDto> DeleteFileAsync(S3Object s3Object, AwsCredentials awsCredentials);
        Task<S3ResponseDto> GetPreSignedUrlAsync(S3Object s3Object, AwsCredentials awsCredentials);
    }
}
