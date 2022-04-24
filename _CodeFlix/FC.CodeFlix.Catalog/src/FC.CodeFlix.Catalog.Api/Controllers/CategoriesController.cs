using FC.CodeFlix.Catalog.Api.ApiModels.Category;
using FC.CodeFlix.Catalog.Api.ApiModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;
using FC.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;
using FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
using FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.CodeFlix.Catalog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoriesController(IMediator mediator) => this._mediator = mediator;

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CategoryModelOutput>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryInput input, CancellationToken cancellationToken)
        {
            var output = await this._mediator.Send(input, cancellationToken);

            return CreatedAtAction(nameof(Create), new { output.Id }, new ApiResponse<CategoryModelOutput>(output));
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryModelOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryApiInput apiInput, [FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var input = new UpdateCategoryInput(id, apiInput.Name, apiInput.Description, apiInput.IsActive);
            var output = await this._mediator.Send(input, cancellationToken);

            return Ok(new ApiResponse<CategoryModelOutput>(output));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            await this._mediator.Send(new DeleteCategoryInput(id), cancellationToken);

            return NoContent();
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryModelOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var output = await this._mediator.Send(new GetCategoryInput(id), cancellationToken);

            return Ok(new ApiResponse<CategoryModelOutput>(output));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseList<CategoryModelOutput>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List(
            CancellationToken cancellationToken, 
            [FromQuery] int? page = null, 
            [FromQuery(Name = "per_page")] int? perPage = null,
            [FromQuery] string? search = null, 
            [FromQuery] string? sort = null, 
            [FromQuery] SearchOrder? dir = null)
        {
            var input = new ListCategoriesInput();
            input.SetValues(page, perPage, search, sort, dir);
            var output = await this._mediator.Send(input, cancellationToken);

            return Ok(new ApiResponseList<CategoryModelOutput>(output));
        }
    }
}