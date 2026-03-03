using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Contracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet()]
    [ProducesResponseType(typeof(ApiPagedSuccessResponse<BookResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiPagedSuccessResponse<BookResponse>>> Get(int page = 1, int pageSize = 10)
    {
        var (items, totalCount) = await _bookService.GetPagedAsync(page, pageSize);

        var result = new ApiPagedSuccessResponse<BookResponse>(
            StatusCodes.Status200OK,
            "Livros listados com sucesso.",
            page,
            pageSize,
            totalCount,
            items.Select(MapToResponse).ToArray());

        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiSuccessResponse<BookResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiSuccessResponse<BookResponse>>> GetById(long id)
    {
        var book = await _bookService.GetByIdAsync(id);
        return Ok(new ApiSuccessResponse<BookResponse>(
            StatusCodes.Status200OK,
            "Livro obtido com sucesso.",
            MapToResponse(book)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiSuccessResponse<BookResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiSuccessResponse<BookResponse>>> Create([FromBody] UpsertBookRequest request)
    {
        var createdBook = await _bookService.CreateAsync(request.Title, request.Author, request.Description);
        var response = MapToResponse(createdBook);
        var payload = new ApiSuccessResponse<BookResponse>(
            StatusCodes.Status201Created,
            "Livro criado com sucesso.",
            response);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, payload);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ApiSuccessResponse<BookResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiSuccessResponse<BookResponse>>> Update(long id, [FromBody] UpsertBookRequest request)
    {
        var updatedBook = await _bookService.UpdateAsync(id, request.Title, request.Author, request.Description);
        return Ok(new ApiSuccessResponse<BookResponse>(
            StatusCodes.Status200OK,
            "Livro atualizado com sucesso.",
            MapToResponse(updatedBook)));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(long id)
    {
        await _bookService.DeleteAsync(id);
        return NoContent();
    }

    private static BookResponse MapToResponse(Domain.Entities.Book book)
        => new(book.Id, book.Title, book.Author, book.Description, book.CreatedDate, book.LastUpdatedDate);

    public record UpsertBookRequest(string Title, string Author, string Description);

    public record BookResponse(long Id, string Title, string Author, string Description, DateTime CreatedDate, DateTime? LastUpdatedDate);
}