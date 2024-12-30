using in2Task.Data;
using in2Task.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace in2Task.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

         public IActionResult Index()
        {
            return View();
            
        }


        private List<User> LoadUsers()
        {
            // Ucitaavanje korisnika 
            var users = _context.Users.OrderByDescending(x => x.Id).ToList();
            return users; 
        }
        public IActionResult Privacy()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var users = LoadUsers(); 
            return View(users);
        }
        
        
        public IActionResult AddingNewUser(string username,string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Spremanje korisničkog ID-a u sesiju
                HttpContext.Session.SetInt32("UserId", user.Id);

                // Preusmjeravanje na početnu stranicu ili dashboard
                return RedirectToAction("Privacy", "Home");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddingNewUser(User user)
        {
            if  (string.IsNullOrEmpty(user.Username) ||
                 string.IsNullOrEmpty(user.Password) ||
                 string.IsNullOrEmpty(user.Email))
            {
                ViewBag.ErrorMessage = "Sva polja moraju biti popunjena."; 
                return View(user); 
            }
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Korisničko ime je već zauzeto."; 
                return View(user);
            }


            _context.Users.Add(user);
            _context.SaveChanges();

            
            return RedirectToAction("Privacy");
        }
        public IActionResult Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(p=> p.Id == id);
            if(user !=null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Privacy");
        }
        public IActionResult Edit(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        
        [HttpPost]
        public IActionResult Edit(User model)
        {
            var user = _context.Users.Find(model.Id);
                if(user is not null)
            {
                user.Username = model.Username;
                user.Password = model.Password;
                user.Email = model.Email;

                _context.SaveChanges();
            }
            return RedirectToAction("Privacy"); 
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Provjera da li postoji korisnik s unesenim podacima
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Spremanje korisničkog ID-a u sesiju
                HttpContext.Session.SetInt32("UserId", user.Id);

                // Preusmjeravanje na početnu stranicu ili dashboard
                return RedirectToAction("Privacy", "Home");
            }

            ViewBag.ErrorMessage = "Pogrešno korisničko ime ili lozinka";
            return View();
        }

        //public IActionResult Logout()
        //{
        //    HttpContext.Session.Clear(); // Brisanje sesije
        //    return RedirectToAction("Login"); // Povratak na login formu
        //}

        public IActionResult BlogPost() 
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Ako korisnik nije prijavljen, preusmjerite ga na stranicu za prijavu
                return RedirectToAction("Login", "Home");
            }
            var posts = _context.BlogPosts
            .Include(bp => bp.User)
            .Include(bp => bp.Comments)
            .ThenInclude(c => c.User)
            .OrderByDescending(bp => bp.CreatedAt)
            .ToList();

            return View(posts);
        }
        public IActionResult AddBlogPost()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBlogPost(BlogPost blogPost)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (string.IsNullOrEmpty(blogPost.Title) || string.IsNullOrEmpty(blogPost.Content))
            {
                ViewBag.ErrorMessage = "Naslov i sadržaj su obavezni.";
                return View(blogPost);
            }

            blogPost.CreatedAt = DateTime.Now;
            blogPost.UpdatedAt = DateTime.Now;

            blogPost.UserId = userId.Value;

            _context.BlogPosts.Add(blogPost);
            _context.SaveChanges();

            return RedirectToAction("BlogPost");
        }

        public IActionResult DeleteConfirmed(int id)
        {
            var blogPost = _context.BlogPosts.FirstOrDefault(bp => bp.Id == id);
            if (blogPost != null)
            {
                _context.BlogPosts.Remove(blogPost);
                _context.SaveChanges();
            }
            return RedirectToAction("BlogPost");
        }

        
        public IActionResult EditBlogPost(int id)
        {

            var blog = _context.BlogPosts.FirstOrDefault(u => u.Id == id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }
        [HttpPost]
        public IActionResult EditBlogPost(BlogPost model)
        {
            var blog = _context.BlogPosts.Find(model.Id);
            if (blog is not null)
            {
                blog.Title = model.Title;
                blog.Content = model.Content;
                blog.CreatedAt = DateTime.Now;
                blog.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
            }
            return RedirectToAction("BlogPost");
        }

        [HttpPost]
        public IActionResult AddComment(int postId, string content)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (string.IsNullOrEmpty(content))
            {
                TempData["ErrorMessage"] = "Sadržaj komentara ne smije biti prazan.";
                return RedirectToAction("BlogPost", new { id = postId }); 
            }

            var blogPost = _context.BlogPosts.FirstOrDefault(bp => bp.Id == postId);
            if (blogPost == null)
            {
                TempData["ErrorMessage"] = "Post ne postoji.";
                return RedirectToAction("BlogPost"); 
            }

            // Dodajte novi komentar
            var comment = new Comment
            {
                Content = content,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = userId.Value,
                BlogPostId = postId
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("BlogPost", new { id = postId }); // Preusmjeravanje natrag na post
        }

        public IActionResult Search(string search)
        {
            // Dohvati sve postove iz baze s povezanim korisnicima i komentarima
            var allPostsInDb = _context.BlogPosts
                .Include(bp => bp.User)       // UVitaj korisnike
                .Include(bp => bp.Comments)  // -||-
                .ToList();

            
            if (!string.IsNullOrEmpty(search))
            {
                allPostsInDb = allPostsInDb
                    .Where(p => p.Title.Contains(search, StringComparison.OrdinalIgnoreCase)) // Case-insensitive(ZNACI DA .NET I.net ce nam dati isto rezultat pretrazivanja dok bez toga ako upisuemo pretrazivanje ce biti samo prihvaceno tocno u svaki znak bilo to veliko ili malo slovo)
                    .ToList();
            }

            ViewBag.SearchText = search;

            return View("BlogPost", allPostsInDb);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
