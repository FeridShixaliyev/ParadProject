
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parad.Areas.Manage.ViewModels;
using Parad.DAL;
using Parad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parad.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin,Moderator")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _sql;
        private readonly IWebHostEnvironment _env;

        public UserController(AppDbContext sql,IWebHostEnvironment env,UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _sql = sql;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<AppUser> users = await _userManager.Users.ToListAsync();
            return View(users);
        }
        public async Task<IActionResult> Block(string? id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _userManager.SetLockoutEnabledAsync(user,true);
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index", "User");
        }
        public async Task<IActionResult> Delete(string? id)
        {
            AppUser user =await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _userManager.DeleteAsync(user);
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index","User");
        }
        
        public async Task<IActionResult> Role(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            RoleVM roleVM = new RoleVM()
            {
                User = user,
                Roles = await _userManager.GetRolesAsync(user)
            };
            ViewBag.userRoles = await _roleManager.Roles.ToListAsync();
            
            return View(roleVM);
        }
        [HttpPost]
        public async Task<IActionResult> Role(RoleVM roleObject,string id)
        {
            AppUser user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == roleObject.User.Id);
            if (user == null) return NotFound();
            roleObject.User = user;
            var userRole= await _userManager.GetRolesAsync(user);
            foreach (var newRole in roleObject.Roles)
            {
                foreach (var item in userRole)
                {
                    if (newRole != item)
                    {
                        await _userManager.AddToRoleAsync(user,newRole);
                        await _userManager.RemoveFromRolesAsync(user,userRole);
                    }
                    else
                    {
                        ViewBag.userRoles = await _roleManager.Roles.ToListAsync();
                        ModelState.AddModelError("", "The user already has the role you selected.");
                        return View(new RoleVM() { 
                            User=roleObject.User,
                            Roles=roleObject.Roles
                        });
                    }
                }
            }

            await _sql.SaveChangesAsync();

            return RedirectToAction("Index","User");
        }
    }
}
