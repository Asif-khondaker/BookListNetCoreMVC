using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookListNetCoreMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookListNetCoreMVC.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _db;
        public BookController(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public Book Book { set; get; }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Upsert(int? id)
        
         {
            Book = new Book();
            if(id == null)
            {
                //Create
                return View(Book);
            }
            //update
            Book = _db.Books.FirstOrDefault(b => b.Id == id);
            if(Book == null)
            {
                return NotFound();
            }
            return View(Book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                //Create
                if (Book.Id == 0)
                {
                    _db.Books.Add(Book);
                }
                //Update
                else
                {
                    _db.Books.Update(Book);
                }
                _db.SaveChanges();
                return RedirectToAction("index");
            }
            return View(Book);
        }

        #region API call
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.Books.ToListAsync() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var bookFromDb = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (bookFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _db.Books.Remove(bookFromDb);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Delete succesfull" });
        }
        #endregion
    }
}