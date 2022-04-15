using System.Net;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;
using App.Models.Blog;
using App.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using App.Utilities;
using App.Models.Product;
using AppMvc.Areas.Product.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AppMvc.Areas.Order.Controllers
{
    [Area("Order")]
    [Route("admin/ordermanage/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator + "," + RoleName.Editor)]
    public class OrderManageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderManageController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }
        // GET: Blog/Post
        public async Task<IActionResult> Index([FromQuery(Name = "p")] int currentPage, int pagesize)
        {
            var posts = _context.Orders
                        .Include(p => p.Owner)
                        .OrderByDescending(p => p.DateCreated);

            int totalPosts = await posts.CountAsync();
            if (pagesize <= 0) pagesize = 10;
            int countPages = (int)Math.Ceiling((double)totalPosts / pagesize);

            if (currentPage > countPages) currentPage = countPages;
            if (currentPage < 1) currentPage = 1;

            var pagingModel = new PagingModel()
            {
                countpages = countPages,
                currentpage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index", new
                {
                    p = pageNumber,
                    pagesize = pagesize
                })
            };

            ViewBag.pagingModel = pagingModel;
            ViewBag.totalPosts = totalPosts;

            ViewBag.postIndex = (currentPage - 1) * pagesize;

            var postsInPage = await posts.Skip((currentPage - 1) * pagesize)
                             .Take(pagesize)
                             .ToListAsync();

            return View(postsInPage);
        }

        // GET: Blog/Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Orders
                .Include(p => p.Owner)
                .Include(p=>p.OrderDetails)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            var list = _context.OrderDetails.Where(p=>p.OrderId == post.Id)
            .Include(p => p.Product)
            .ToList();
            ViewData["list"] = list;
            return View(post);
        }

        // GET: Blog/Post/Create

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Blog/Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Products.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            _context.Products.Remove(post);
            await _context.SaveChangesAsync();

            StatusMessage = "Bạn vừa xóa hóa đơn: " + post.Title;

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Statistics(){
            return View();
        }
        public IActionResult Revenue(){
            return View();
        }
        public JsonResult DsOrder()
        {
            try{
                // var dsOrder = (from l in _context.Orders
                //                select new
                //                {
                //                    Id = l.Id,
                //                    UserName = l.UserName,
                //                    DateCreated = l.DateCreated
                //                }).ToList();
                var posts = _context.Orders
                        .Include(p => p.Owner)
                        .OrderByDescending(p => p.DateCreated);
                var dsOrder= (from l1 in posts
                select new {
                    Id=l1.Id,
                    UserName=l1.Owner.UserName,
                    DateCreated = l1.DateCreated,
                    Price=l1.TotalPrice
                }).ToList();
                return Json(new { code = 200, dsOrder = dsOrder });
               
            }
            catch(Exception e){
                return Json(new {code=500});

            }
        }
        public JsonResult DsOrderyear(int year){
            try
            {
                
                var posts = _context.Orders
                            .Include(p => p.Owner)
                            .OrderByDescending(p => p.DateCreated);
                var dsOrder = (from p in posts
                               where p.DateCreated.Year == year
                               select new
                               {
                                   Id = p.Id,
                                   UserName = p.Owner.UserName,
                                   DateCreated = p.DateCreated,
                                   Price=p.TotalPrice
                               }).ToList();
                return Json(new { code = 200, dsOrder = dsOrder });
            }
            catch(Exception ex){
                return Json(new {code=500});

            }
        }
        public JsonResult dsOrderMonth(int year, int month){
            try
            {
                
                var posts = _context.Orders
                            .Include(p => p.Owner)
                            .OrderByDescending(p => p.DateCreated);
                var dsOrder = (from p in posts
                               where p.DateCreated.Year == year && p.DateCreated.Month == month
                               select new
                               {
                                   Id = p.Id,
                                   UserName = p.Owner.UserName,
                                   DateCreated = p.DateCreated,
                                   Price=p.TotalPrice
                               }).ToList();
                return Json(new { code = 200, dsOrder = dsOrder });
            }
            catch(Exception ex){
                return Json(new {code=500});

            }
        }
        public JsonResult dsOrderDay(int year, int month, int day){
            try
            {
                
                var posts = _context.Orders
                            .Include(p => p.Owner)
                            .OrderByDescending(p => p.DateCreated);
                var dsOrder = (from p in posts
                               where p.DateCreated.Year == year && p.DateCreated.Month == month && p.DateCreated.Day == day
                               select new
                               {
                                   Id = p.Id,
                                   UserName = p.Owner.UserName,
                                   DateCreated = p.DateCreated,
                                   Price=p.TotalPrice
                               }).ToList();
                return Json(new { code = 200, dsOrder = dsOrder });
            }
            catch(Exception ex){
                return Json(new {code=500});

            }
        }
    }
}
