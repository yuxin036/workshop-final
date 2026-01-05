using BookSystem.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace BookSystem.Controllers
{
    [Route("api/bookmaintain")]
    [ApiController]
    public class BookMaintainController : ControllerBase
    {
        
        [HttpGet]
        [Route("testconnection")]
        public IActionResult TestConnection()
        {
            BookService bookService = new BookService();
            string result = bookService.TestDbConnection();
            return Ok(result);
        }

        [HttpPost]
        [Route("addbook")]
        public IActionResult AddBook(Book book)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    BookService bookService = new BookService();
                    bookService.AddBook(book);
                    return Ok(
                        new ApiResult<string>()
                        {
                            Data = string.Empty,
                            Status = true,
                            Message = string.Empty
                        });
                }
                else
                {
                    return BadRequest(ModelState);
                }

            }
            catch (Exception)
            {
                return Problem(); 
            }
        }
        [HttpPost()]
        [Route("querybook")]
        public IActionResult QueryBook([FromBody]BookQueryArg arg)
        {
            try
            {
                BookService bookService = new BookService();

                return Ok(bookService.QueryBook(arg));
            }
            catch (Exception)
            {
                return Problem();
            }
        }

        [HttpPost()]
        [HttpGet("loadbook/{bookId}")]
        public IActionResult GetBookById([FromRoute] int bookId)
        {
            try
            {
                BookService bookService = new BookService();
                var book = bookService.GetBookById(bookId);
                ApiResult<Book> result = new ApiResult<Book>
                {
                    Data = book,
                    Status = true,
                    Message = string.Empty
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        //TODO:UpdateBook()
        [HttpPost("updatebook")]
        public IActionResult UpdateBook([FromBody] Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BookService bookService = new BookService();
                    bookService.UpdateBook(book);
                    return Ok(new ApiResult<string>()
                    {
                        Data = string.Empty,
                        Status = true,
                        Message = string.Empty
                    });
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }



        [HttpDelete("deletebook/{bookId}")]
        public IActionResult DeleteBookById([FromRoute] int bookId)
        {
            try
            {
                BookService bookService = new BookService();
                
                // Simple check: for this example, we assume a book with a keeper cannot be deleted.
                var book = bookService.GetBookById(bookId);
                if (book != null && !string.IsNullOrEmpty(book.BookKeeperId))
                {
                    return Ok(new ApiResult<string>
                    {
                        Data = string.Empty,
                        Status = false,
                        Message = "該書已借出，不可刪除。"
                    });
                }

                bookService.DeleteBookById(bookId);

                return Ok(new ApiResult<string>
                {
                    Data = string.Empty,
                    Status = true,
                    Message = string.Empty
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        //TODO:booklendrecord
        [HttpPost("querylendrecord")]
        public IActionResult QueryLendRecord([FromBody] int bookId)
        {
            try
            {
                BookService bookService = new BookService();
                var records = bookService.GetLendRecordByBookId(bookId);
                var result = new ApiResult<List<BookLendRecord>>()
                {
                    Data = records,
                    Status = true,
                    Message = string.Empty
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                return Problem(ex.Message);
            }
        }
    }
}
