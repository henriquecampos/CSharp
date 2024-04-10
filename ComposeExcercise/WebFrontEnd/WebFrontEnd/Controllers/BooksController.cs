using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using static System.Net.Http.HttpClient;
using WebFrontEnd.Models;

namespace WebFrontEnd.Controllers
{
    public class BooksController : Controller
    {
        HttpClient Client = new();
        private readonly string _baseUri;

        private readonly ILogger<BooksController> _logger;


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public BooksController(ILogger<BooksController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _baseUri = configuration.GetValue<string>("Settings:ApiUrl")!;
        }

        public async Task<IActionResult> Index(List<BookDTO>? booksDto)
        {
            Client.DefaultRequestHeaders.Accept.Clear();
            var books = await GetAlltBooksAsync();

            foreach (Book book in books ?? Enumerable.Empty<Book>())
            {
                booksDto.Add(
                    new BookDTO
                    {
                        Id = book.ID,
                        Title = book.Title,
                        Category = book.Category,
                        Language = book.Language,
                        Weight = book.Weight,
                        EditionDate = book.EditionDate,
                        Publisher = book.Publisher,
                        PaperbackPages = book.PaperbackPages,
                        ISBN_10 = book.ISBN_10,
                        ISBN_13 = book.ISBN_13,
                        Dimensions = book.Dimensions,
                        Edition = book.Edition
                    }
                );
            }

            return View(booksDto);
        }

        // List all books: api/Books
        public async Task<List<Book>> GetAlltBooksAsync()
        {
            using (var client = new System.Net.Http.HttpClient())
            { 
            // Call *mywebapi*, and display its response in the page
            var request = new System.Net.Http.HttpRequestMessage();
            // webapi is the container name
            request.RequestUri = new Uri($"{_baseUri}/api/Books");
            var response = await client.SendAsync(request);
            await using Stream stream = await response.Content.ReadAsStreamAsync();
            var books = await JsonSerializer.DeserializeAsync<List<Book>>(stream);
            return books;
            }
        }
    }
}
