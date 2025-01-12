using Microsoft.AspNetCore.Mvc;
using StealCat.Data.Entities;
using StealCatApi.Models;
using StealCatApi.Services;

namespace StealCatApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatsController : ControllerBase
    {
        private readonly CatsService _catsService;

        public CatsController(CatsService catsService)
        {
            _catsService = catsService;
        }

        [HttpPost("fetch")]
        public async Task<IActionResult> FetchCats()
        {
            try
            {
                // Call the FetchCatsAsync method from the CatService
                List<CatDto> cats = await _catsService.FetchCats();

                // Return the list of cats
                return Ok(cats);
            }
            catch (Exception ex)
            {
                // Handle errors, returning a generic server error response
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCatByIdAsync(int id)
        {
            try
            {
                DbCatsDto cat = await _catsService.GetCatByIdAsync(id);

                if (cat == null)
                {
                    return NotFound(new { error = "Cat not found" });
                }

                return Ok(cat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCatsAsync([FromQuery] string tag = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            List<DbCatsDto> cats;

            if (string.IsNullOrEmpty(tag))
            {
                // Retrieve all cats with paging
                cats = await _catsService.GetCatsWithPagingAsync(page, pageSize, null);
            }
            else
            {
                // Retrieve cats with a specific tag and paging
                cats = await _catsService.GetCatsWithPagingAsync(page, pageSize, tag);
            }

            if (cats == null || !cats.Any())
            {
                return NotFound(tag == null ? "No cats found." : "No cats found with the specified tag.");
            }

            return Ok(cats);
        }
    }
}
