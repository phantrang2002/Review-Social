using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReviewSocial.Models;
using ReviewSocial.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ReviewSocial.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostRepository _post;
        private readonly ICategoryRepository _cate;

        public HomeController(ILogger<HomeController> logger, IPostRepository post, ICategoryRepository cate)
        {
            _logger = logger;
            _post = post;
            _cate = cate;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var cate = _cate.GetAll();
            var post = _post.GetAll();
            return View(new Tuple<IEnumerable<Category>, IEnumerable<Post>>(cate, post));
        }

        [HttpGet]
        public IActionResult Search(string name, int? category)
        {
            IEnumerable<Category> cate = _cate.GetAll();
            IEnumerable<Post> post = null;

            if (category.HasValue)
            {
                post = _post.GetByCategory(category.Value);
            }
            else if (!string.IsNullOrWhiteSpace(name))
            {
                post = _post.GetByTitle(name);
            }
            else
            {
                post = _post.GetAll();
            }

            return View(new Tuple<IEnumerable<Category>, IEnumerable<Post>>(cate, post));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<string> UploadFile(IFormFile file)
        {
            if (file != null)
            {
                var fileName = DateTime.UtcNow.Ticks.ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine("wwwroot", "img", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return $"img/{fileName}";
            }
            else
            {
                return "";
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Post post, IFormFile file)
        {
            try
            {
                if (HttpContext.Session.GetString("id") != null)
                {
                    var imagePath = "";
                    if (file != null)
                    {
                        imagePath = await UploadFile(file);
                    }

                    // Kiểm tra nếu tệp không phải là hình ảnh
                    if (file != null && !IsImageFile(file))
                    {
                        return BadRequest("Chỉ cho phép tải lên các tệp hình ảnh.");
                    }

                    var now = DateTime.Now;
                    var item = new Post
                    {
                        CategoryId = post.CategoryId,
                        Content = post.Content ?? "",
                        CreatedDate = now,
                        Thumbnail = imagePath,
                        Title = post.Title ?? "",
                        verifykey = post.verifykey ?? "",
                        UserId = Convert.ToInt32(HttpContext.Session.GetString("id")),
                        TotalReport = 0,
                        View = 0
                    };

                    _post.Create(item);

                    return Ok();
                }
                else
                {
                    return Forbid();
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                var item = _post.GetById(id);
                if (item == null)
                {
                    return NotFound();
                }

                // Kiểm tra quyền của người dùng
                if (Convert.ToInt32(HttpContext.Session.GetString("id")) != item.UserId)
                {
                    return BadRequest("Bạn không có quyền xóa bài viết này.");
                }

                _post.Delete(item);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCmt(Post post, IFormFile file)
        {
            try
            {
                var item = _post.GetById(post.Id);
                if (item == null)
                {
                    return NotFound();
                }

                // Kiểm tra quyền của người dùng
                if (item.UserId != Convert.ToInt32(HttpContext.Session.GetString("id")))
                {
                    return BadRequest("Bạn không có quyền chỉnh sửa bài viết này.");
                }

                var imagePath = "";
                if (file != null)
                {
                    imagePath = await UploadFile(file);
                }

                item.CategoryId = post.CategoryId;
                item.Content = post.Content ?? "";
                if (!string.IsNullOrEmpty(imagePath))
                {
                    item.Thumbnail = imagePath;
                }
                item.Title = post.Title ?? "";

                _post.Update(item);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        // Phương thức hỗ trợ kiểm tra tệp có phải là hình ảnh không
        private bool IsImageFile(IFormFile file)
        {
            if (file.ContentType.ToLower() == "image/jpg" || 
                file.ContentType.ToLower() == "image/jpeg" || 
                file.ContentType.ToLower() == "image/png")
            {
                return true;
            }

            return false;
        }

        // [HttpGet]
        // public IActionResult SearchByCategory()
        // {
        //     var categories = _cate.GetAll(); // Lấy danh sách tất cả các danh mục
        //     return View(categories);
        // }

        // public IActionResult Search()
        // {
        //     // Lấy tất cả các bài viết
        //     var allPosts = _context.Posts.ToList();
        //     return View(allPosts);
        // }

 

        // [HttpGet]  
        // public IActionResult SearchByCategory(int categoryId)
        // {
        //     var categories = _cate.GetAll(); 
        //     var posts = _post.GetByCategory(categoryId); 

        //     ViewBag.CategoryId = categoryId; 
        //     ViewBag.Categories = categories; 

        //     return View(posts);
        // }

        [HttpGet]
        public IActionResult AllPost()
        {
            var allPosts = _post.GetAll(); // Lấy tất cả các bài viết
            return View(allPosts);
        }



    }
}
