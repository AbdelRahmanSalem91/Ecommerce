using AutoMapper;
using Ecommerce.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    public class BugController : BaseController
    {
        public BugController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        #region Not Found
        [HttpGet("not-found")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNotFound()
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(100);

            if (category is null) return NotFound();

            return Ok(category);
        }
        #endregion

        #region Server Error
        [HttpGet("server-error")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetServerError()
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(100);

            category.Name = "";

            return Ok(category);
        }
        #endregion

        #region Bad Request with Parameter
        [HttpGet("bad-request/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBadRequest(int id)
        {
            return Ok();
        }
        #endregion

        #region Bad Request
        [HttpGet("bad-request")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBadRequest()
        {
            return BadRequest();
        }
        #endregion
    }
}
