using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parad.DAL;
using Parad.Models;
using Parad.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Parad.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _sql;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(AppDbContext sql, UserManager<AppUser> userManager)
        {
            _sql = sql;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            IQueryable<Image> images =  _sql.Images.AsQueryable();
            ViewBag.ImageCount =  images.Count();
            HomeVM homeVM = new HomeVM
            {

                AppUser = await _userManager.Users.ToListAsync(),
                Sliders=await _sql.Sliders.ToListAsync(),
                Images = await _sql.Images.Take(20).Include(i => i.User).OrderByDescending(i=>i.DownloadDate).ToListAsync(),
                Likes = await _sql.Likes.ToListAsync(),
                Categories = await _sql.Categories.Take(9).ToListAsync(),
            };
            return View(homeVM);
        }
        [Authorize]
        public async Task<IActionResult> About()
        {
            ViewBag.AppUser = await _userManager.FindByNameAsync(User.Identity.Name);
            return View();
        }
        public IActionResult LoadMore(int skip)
        {
            return PartialView("_ImagePartial",_sql.Images
                                                            .OrderByDescending(i=>i.Id)
                                                            .Skip(skip).Take(20)
                                                            .Include(i => i.User));
        }
    }
}
