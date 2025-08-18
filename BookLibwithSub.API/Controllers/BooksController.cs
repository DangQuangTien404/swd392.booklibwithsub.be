using BookLibwithSub.Repo.Entities;
using BookLibwithSub.Repo.Interfaces;
using BookLibwithSub.Service.Constants;
using BookLibwithSub.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibwithSub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _repository;

        public BooksController(IBookRepository repository)
        {
            _repository = repository;
        }

        private static BookResponse ToResponse(Book b) =>
            new(b.BookID, b.Title, b.AuthorName, b.ISBN, b.Publisher, b.PublishedYear, b.TotalCopies, b.AvailableCopies);

        [HttpGet("by-title")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<BookResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByTitle([FromQuery] string title)
        {

            var (items, _) = await _repository.SearchAsync(title, 1, 100);
            var matches = items.Where(b => string.Equals(b.Title, title, StringComparison.OrdinalIgnoreCase))
                               .Select(ToResponse);
            return Ok(matches);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var book = await _repository.GetByIdAsync(id);
            if (book == null) return NotFound();
            return Ok(ToResponse(book));
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(typeof(BookResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateBookRequest req)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = new Book
            {
                Title = req.Title,
                AuthorName = req.AuthorName,
                ISBN = req.Isbn,
                Publisher = req.Publisher,
                PublishedYear = req.PublishedYear,
                TotalCopies = req.TotalCopies,
                AvailableCopies = req.AvailableCopies
            };

            try
            {
                var created = await _repository.AddAsync(entity);
                return CreatedAtAction(nameof(Get), new { id = created.BookID }, ToResponse(created));
            }
            catch (Exception ex)
            {
                return Problem(title: "Failed to create book", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookRequest req)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Title = req.Title;
            existing.AuthorName = req.AuthorName;
            existing.ISBN = req.Isbn;
            existing.Publisher = req.Publisher;
            existing.PublishedYear = req.PublishedYear;
            existing.TotalCopies = req.TotalCopies;
            existing.AvailableCopies = req.AvailableCopies;

            try
            {
                await _repository.UpdateAsync(existing);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(title: "Failed to update book", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
