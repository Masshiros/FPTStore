using FPTStore.Models.ViewModels;
using FPTStore.DataAccess.Data;
using FPTStore.DataAccess.Repository.IRepository;
using FPTStore.Models.ViewModels;
using FPTStore.Models;
using FPTStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTStore.DataAccess.Repository;

namespace FPTStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext db,UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        /**
        * @DESC: Display user page
        * @METHOD: GET
        * @PARAM
        * @RETURN: ViewResult
        *
        */
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        /**
       * @DESC: Display role management page
       * @METHOD: GET
       * @PARAM
       * @RETURN: ViewResult
       *
       */
        public IActionResult RoleManagement(string userId)
        {
            string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;
            RoleManagementVM RoleVM = new RoleManagementVM()
            {
                ApplicationUser = _db.ApplicationUsers.Include(u=>u.Company).FirstOrDefault(u => u.Id == userId),
                RoleList = _db.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _db.Companies.Select(i => new SelectListItem
                {
                    Text = i.CompanyName,
                    Value = i.CompanyId.ToString()
                }),
            };

            RoleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u=>u.Id==RoleId).Name;
                
            return View(RoleVM);
        }
        /**
      * @DESC: Change role of user
      * @METHOD: POST
      * @PARAM: RoleManagementVM
      * @RETURN: ViewResult
      *
      */
        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagementVM.ApplicationUser.Id).RoleId;
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;
            if (!(roleManagementVM.ApplicationUser.Role == oldRole))
            {
                // a role was updated
                ApplicationUser applicationUser =
                    _db.ApplicationUsers.FirstOrDefault(u => u.Id == roleManagementVM.ApplicationUser.Id);
                if (roleManagementVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
                }

                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }

                _db.SaveChanges();
                _userManager.RemoveFromRoleAsync(applicationUser,oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser,roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();

            }

            return RedirectToAction("Index");
        }

        #region API CALLS
        /**
         * @DESC: API to fetch all company
         * @METHOD: POST
         * @PARAM: 
         * @RETURN: JSON
         *
         */
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _db.ApplicationUsers.Include(u=>u.Company).ToList();
            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in objUserList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                
                if (user.Company == null)
                {
                    user.Company = new Company() { CompanyName = "" };
                }
            }
            return Json(new { data = objUserList });
        }
        /**
         * @DESC: Lock & unlock user
         * @METHOD: DELETE
         * @PARAM: string
         * @RETURN: JSON
         *
         */
        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {

            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _db.ApplicationUsers.Update(objFromDb);
            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Successful" });
        }
        #endregion

    }
}
