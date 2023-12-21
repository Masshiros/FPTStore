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
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(IUnitOfWork unitOfWork,UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
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
   
            RoleManagementVM RoleVM = new RoleManagementVM()
            {
                ApplicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId,includeProperties:"Company"),
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _unitOfWork.CompanyRepository.GetAll().Select(i => new SelectListItem
                {
                    Text = i.CompanyName,
                    Value = i.CompanyId.ToString()
                }),
            };

            RoleVM.ApplicationUser.Role =
                _userManager.GetRolesAsync(_unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId)).GetAwaiter().GetResult().FirstOrDefault();
                
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
            
            string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUserRepository.Get(u => u.Id == roleManagementVM.ApplicationUser.Id)).GetAwaiter().GetResult().FirstOrDefault();
            ApplicationUser applicationUser =
                _unitOfWork.ApplicationUserRepository.Get(u => u.Id == roleManagementVM.ApplicationUser.Id);
            if (!(roleManagementVM.ApplicationUser.Role == oldRole))
            {
                // a role was updated
                
                if (roleManagementVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
                }

                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _unitOfWork.ApplicationUserRepository.Update(applicationUser);
                _unitOfWork.Save();
                _userManager.RemoveFromRoleAsync(applicationUser,oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser,roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();

            }
            else
            {
                // role company still remain but they change their company
                if (oldRole == SD.Role_Company && applicationUser.CompanyId != roleManagementVM.ApplicationUser.CompanyId)
                {
                    applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
                    _unitOfWork.ApplicationUserRepository.Update(applicationUser);
                    _unitOfWork.Save();
                }
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
            List<ApplicationUser> objUserList = _unitOfWork.ApplicationUserRepository.GetAll( includeProperties: "Company").ToList();
            foreach (var user in objUserList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
                
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

            var objFromDb = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == id);
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

            _unitOfWork.ApplicationUserRepository.Update(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Operation Successful" });
        }
        #endregion

    }
}
