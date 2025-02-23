using AutoMapper;
using AWSS3.Models;
using AWSS3.Services;
using Ecommerce.API.Helper;
using Ecommerce.Core.DTOs;
using Ecommerce.Core.Entities.Product;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Sharing;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IStorageService _storageService;
        private readonly IConfiguration _configuration;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper, IStorageService storageService, IConfiguration configuration) : base(unitOfWork, mapper)
        {
            _storageService = storageService;
            _configuration = configuration;
        }

        #region Get All Products
        [HttpGet("getAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll([FromQuery] ProductParams productParams)
        {
            try
            {
                IReadOnlyList<Product>  products = await _unitOfWork.ProductRepository.GetAllAsync(productParams);

                if (products is null) return NotFound(new APIResponse(404, "There is no products!"));

                var totalCount = await _unitOfWork.ProductRepository.CountAsync();

                return Ok(new Pagination<Product>(productParams.PageNumber, productParams.PageSize, totalCount, products));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse(400, ex.Message));
            }
        }
        #endregion

        #region Get By Id
        [HttpGet("getById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest(new APIResponse(400, "ID must be greater than 0!"));

            try
            {
                Product product = await _unitOfWork.ProductRepository.GetByIdAsync(id, x => x.Category, x => x.Photos);

                if (product is null) return NotFound(new APIResponse(404, $"There is no product with ID: {id}"));

                //IEnumerable<Photo> photos = await _unitOfWork.PhotoRepository.GetPhotoByProductId(id);

                //if (photos == null)
                //{
                //    return NotFound(new APIResponse(404, "Photo not found"));
                //}

                //foreach (var photo in photos)
                //{
                //    var s3Object = new S3Object()
                //    {
                //        BucketName = "pyramakerz-task",
                //        Name = photo.ImageName
                //    };

                //    var awsCredentials = new AwsCredentials()
                //    {
                //        AwsKey = _configuration["AWSConfiguration:AWSAccessKey"],
                //        AwsSecretKey = _configuration["AWSConfiguration:AWSSecretKey"]
                //    };

                //    var result = await _storageService.GetPreSignedUrlAsync(s3Object, awsCredentials);

                //    if (result.StatusCode != 200)
                //    {
                //        return BadRequest(new APIResponse(result.StatusCode, result.Message));
                //    }

                //    photo.ImageUrl = result.PreSignedUrl;
                //}

                //ProductDto model = new()
                //{
                //    Name = product.Name,
                //    Description = product.Description,
                //    CategoryName = product.Category.Name,
                //};

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse(400, ex.Message));
            }
        }
        #endregion

        #region Create Product
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]ProductDto model)
        {
            if (!ModelState.IsValid) return BadRequest();
            try
            {
                Product product = new()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    CategoryId = 1
                };

                await _unitOfWork.ProductRepository.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();

                foreach (var photo in model.Photos)
                {
                    await using var memoryStream = new MemoryStream();
                    await photo.CopyToAsync(memoryStream);

                    var fileExt = Path.GetExtension(photo.FileName);
                    var fileName = $"{Guid.NewGuid()}.{fileExt}";

                    var s3Object = new S3Object()
                    {
                        BucketName = "pyramakerz-task",
                        Name = fileName,
                        InputStream = memoryStream
                    };
                    var awsCredentials = new AwsCredentials()
                    {
                        AwsKey = _configuration["AWSConfiguration:AWSAccessKey"],
                        AwsSecretKey = _configuration["AWSConfiguration:AWSSecretKey"]
                    };

                    
                    var result = await _storageService.UploadFileAsync(s3Object, awsCredentials);

                    if (result.StatusCode != 200)
                    {
                        return BadRequest(result.Message);
                    }

                    Photo photoObj = new()
                    {
                        ImageName = fileName,
                        ProductId = product.Id,
                    };

                    var urlResult = await _storageService.GetPreSignedUrlAsync(s3Object, awsCredentials);

                    if (urlResult.StatusCode != 200)
                    {
                        return BadRequest(new APIResponse(urlResult.StatusCode, urlResult.Message));
                    }

                    photoObj.ImageUrl = urlResult.PreSignedUrl;

                    await _unitOfWork.PhotoRepository.AddAsync(photoObj);
                }

                await _unitOfWork.SaveChangesAsync();

                return Created("", new APIResponse(201, "Product created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse(400, ex.Message));
            }
        }
        #endregion

        #region Delete Product
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            Product product = await _unitOfWork.ProductRepository.GetByIdAsync(id);

            if (product is null) return NotFound(new APIResponse(404, $"There is no product with ID: {id}"));

            try
            {
                IEnumerable<Photo> photos = await _unitOfWork.PhotoRepository.GetPhotoByProductId(id);

                if (photos == null)
                {
                    return NotFound(new APIResponse(404, "Photo not found"));
                }

                foreach (var photo in photos)
                {

                    var s3Object = new S3Object()
                    {
                        BucketName = "pyramakerz-task",
                        Name = photo.ImageName
                    };

                    var awsCredentials = new AwsCredentials()
                    {
                        AwsKey = _configuration["AWSConfiguration:AWSAccessKey"],
                        AwsSecretKey = _configuration["AWSConfiguration:AWSSecretKey"]
                    };

                    var result = await _storageService.DeleteFileAsync(s3Object, awsCredentials);

                    if (result.StatusCode != 200)
                    {
                        return BadRequest(result.Message);
                    }

                    await _unitOfWork.PhotoRepository.DeleteAsync(photo.Id);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse(400, ex.Message));
            }

            await _unitOfWork.ProductRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new APIResponse(200, "Product deleted successfully."));
        }
        #endregion
    }
}
