using FPTStore.DataAccess.Repository.IRepository;
using FPTStore.Models.ViewModels;
using FPTStore.Models;
using FPTStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FPTStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /**
        * @DESC: Display company page
        * @METHOD: GET
        * @PARAM
        * @RETURN: ViewResult
        *
        */
        [HttpGet]
        public IActionResult Index()
        {
            List<Company> objComppanyList = _unitOfWork.CompanyRepository.GetAll().ToList();

            return View(objComppanyList);
        }
        /**
         * @DESC: Return the update or insert company view
         * @METHOD: GET
         * @PARAM: int
         * @RETURN: ViewResult
         *
         */
        [HttpGet]
        public IActionResult Upsert(int? id)
        {

            if (id == null | id == 0)
            {
                // update
                return View(new Company());
            }
            else
            {
                // insert
                Company companyObj = _unitOfWork.CompanyRepository.Get(u => u.CompanyId == id);
                return View(companyObj);
            }
        }

        /**
         * @DESC: Create new company or edit company
         * @METHOD: POST
         * @PARAM: Company
         * @RETURN: ViewResult
         *
         */
        [HttpPost]
        public IActionResult Upsert(Company companyObj)
        {
          
            if (ModelState.IsValid)
            {
                
                // check whether action is add or update
                if (companyObj.CompanyId == 0)
                {
                    _unitOfWork.CompanyRepository.Add(companyObj);
                    TempData["success"] = "Create company successfully";
                }
                else
                {
                    _unitOfWork.CompanyRepository.Update(companyObj);
                    TempData["success"] = "Edit company successfully";
                }
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            return View(companyObj);
            
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
            List<Company> objComppanyList = _unitOfWork.CompanyRepository.GetAll().ToList();
            return Json(new { data = objComppanyList });
        }
        /**
         * @DESC: API to delete company
         * @METHOD: DELETE
         * @PARAM: int
         * @RETURN: JSON
         *
         */
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyToBeDeleted = _unitOfWork.CompanyRepository.Get(u => u.CompanyId == id);
            if (companyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            
            _unitOfWork.CompanyRepository.Remove(companyToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });

        }
        #endregion

    }
}
