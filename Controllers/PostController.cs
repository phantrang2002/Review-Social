using Microsoft.AspNetCore.Mvc;
using ReviewSocial.Models;
using System.Security.Claims;
using ReviewSocial.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ReviewSocial.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICommentRepository _comment;
        private readonly string view = "~/Views/Admin/PostManagement/";

        public PostController(IPostRepository postRepository,
            ICategoryRepository categoryRepository,
            IHttpContextAccessor contextAccessor,
            IWebHostEnvironment webHostEnvironment,
            ICommentRepository comment)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _contextAccessor = contextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _comment = comment;
        }

        // Trang chủ danh sách bài viết của 1 category
        public IActionResult Index(int id)
        {
            var cate = _categoryRepository.GetAll();
            var post = _postRepository.GetAll();
            // lay 5 gia tri lon nhat theo column
            //dbContext.Table.OrderByDescending(x => x.Column).Take(5);
            return View(new Tuple<IEnumerable<Category>, IEnumerable<Post>>(cate, post));

        }
        public async Task<IActionResult> DetailsAsync(int id)
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToRoute("login");
            }
            var item = _postRepository.GetById(id);
            item.View += 1;
            _postRepository.Update(item);
            var cmt = _comment.GetByPost(id);
            var post = _postRepository.GetById(id);
            return View(new Tuple<Post, IEnumerable<Comment>>(post, cmt));


        }
      
        public async Task<string> UploadFile(IFormFile file)
        {

            if (file != null)
            {

                var fileName = DateTime.UtcNow.Ticks.ToString() + Path.GetExtension(Path.GetFileName(file.FileName));
                Console.WriteLine(fileName);
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

         [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _categoryRepository.GetAll();
            return View(view + "CreateAndUpdate.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Create(Post post, IFormFile thumbnailFile)
        {
            Console.WriteLine(thumbnailFile.FileName);
            var image = await UploadFile(thumbnailFile);

            var item = new Post
            {
                CreatedDate = DateTime.UtcNow,
                UserId = int.Parse(_contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid)),
                CategoryId = post.CategoryId,
                Content = post.Content ?? "",
                Title = post.Title ?? "",
                verifykey = post.verifykey ?? "",
                TotalReport = 0,
                View = 0,
                Thumbnail = image,
            };

            _postRepository.Create(item);
            return Ok(post);


            ViewBag.Categories = _categoryRepository.GetAll();
            return View(view + "CreateAndUpdate.cshtml");
        }

        
        //chatgpt
        public async Task<IActionResult> Comment(Comment comment, IFormFile file)
        {
            try
            {
                var item = new Comment();
                item.CreatedDate = DateTime.UtcNow;
                item.UserId = int.Parse(_contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid));
                item.PostsId = comment.PostsId;
                item.Content = comment.Content;

                if (file != null)
                {
                    item.simage = await UploadFile(file);
                }
                else
                {
                    item.simage = ""; 
                }

                _comment.Create(item);
                return Ok(); ;
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

                var item = _comment.GetById(id);
                if (Convert.ToInt32(HttpContext.Session.GetString("id")) != item.UserId)
                {
                    return BadRequest();
                }
                if (item == null)
                {
                    return NotFound();
                }

                _comment.Delete(item);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public IActionResult UpdateCmt(string content, int id)
        {
            try
            {
                var item = _comment.GetById(id);
                if (item.UserId != Convert.ToInt32(HttpContext.Session.GetString("id")))
                {
                    return BadRequest();
                }
                item.Content = content;
                _comment.Update(item);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}