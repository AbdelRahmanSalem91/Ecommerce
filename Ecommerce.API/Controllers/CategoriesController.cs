using AutoMapper;
using Ecommerce.API.Helper;
using Ecommerce.Core.DTOs;
using Ecommerce.Core.Entities.Product;
using Ecommerce.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    public class CategoriesController : BaseController
    {
        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        #region Get All
        [HttpGet("getAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                IReadOnlyList<Category> categories = await _unitOfWork.CategoryRepository.GetAllAsync();

                if (categories is null) return NotFound(new APIResponse(404, "There is no categories!" ));

                return Ok(categories);
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
            if (id <= 0) return BadRequest(new APIResponse(400, "ID must be greater than 0!" ));

            try
            {
                Category category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

                CategoryDto model = _mapper.Map<CategoryDto>(category);

                if (category is null) return NotFound(new APIResponse(404, $"There is no categroy with ID: {id}"));

                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse(400, ex.Message));
            }
        }
        #endregion

        #region Create Category
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CategoryDto model)
       {
            if (!ModelState.IsValid) return BadRequest();
            try
            {
                Category category = _mapper.Map<Category>(model);

                await _unitOfWork.CategoryRepository.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();

                return Created("", new APIResponse(201, "Category created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse(400, ex.Message));
            }
        }
        #endregion

        #region Update Category
        [HttpPut("update")]
        public async Task<IActionResult> Update(CategoryUpdateDto model)
        {
            if (!ModelState.IsValid) return BadRequest();

            try
            {
                Category category = _mapper.Map<Category>(model);

                await _unitOfWork.CategoryRepository.UpdateAsync(category);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new APIResponse(200, "Category updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse(400, ex.Message));
            }
        }
        #endregion

        #region Delete Category
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid) return BadRequest();

            try
            {
                await _unitOfWork.CategoryRepository.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new APIResponse(200, "Category deleted successfully."));

            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse(400, ex.Message));
            }
        }
        #endregion
    }
}
