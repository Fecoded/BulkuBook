using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

       

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _context.ApplicationUsers.Include(x => x.Company).ToList();
            var userRole = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();
            foreach(var user in userList)
            {
                var roleId = userRole.FirstOrDefault(x => x.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(x => x.Id == roleId).Name;
                if(user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnLock([FromBody] string id)
        {
            var result = _context.ApplicationUsers.FirstOrDefault(x => x.Id == id);

            if (result == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if(result.LockoutEnd != null && result.LockoutEnd > DateTime.Now)
            {
                // Unlock User
                result.LockoutEnd = DateTime.Now;
            }
            else
            {
                // Lock User
                result.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _context.SaveChanges();
            return Json(new { success = true, message = "Operation Successful" });
        }



        #endregion
    }
}
