using FPTStore.DataAccess.Data;
using FPTStore.DataAccess.Repository.IRepository;
using FPTStore.Models;
using FPTStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPTStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /**
         * @DESC: Display category page
         * @METHOD: GET
         * @PARAM
         * @RETURN: ViewResult
         *
         */
        [HttpGet]
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.CategoryRepository.GetAll().ToList();
            return View(objCategoryList);
        }
        /**
         * @DESC: Return the create category view
         * @METHOD: GET
         * @PARAM
         * @RETURN: ViewResult
         *
         */
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        /**
         * @DESC: Create new category
         * @METHOD: POST
         * @PARAM: Category
         * @RETURN: ViewResult
         *
         */
        [HttpPost]
        public IActionResult Create(Category categoryObj)
        {
            if (categoryObj.CategoryName == categoryObj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("CategoryName", "The DisplayOrder cannot exactly match the Category Name");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepository.Add(categoryObj);
                _unitOfWork.Save();
                TempData["success"] = "Create category successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        /**
         * @DESC: Return the edit category view
         * @METHOD: GET
         * @PARAM: int
         * @RETURN: ViewResult
         *
         */
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _unitOfWork.CategoryRepository.Get(u => u.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        /**
        * @DESC: Edit category
        * @METHOD: POST
        * @PARAM: Category
        * @RETURN: ViewResult
        *
        */
        [HttpPost]
        public IActionResult Edit(Category categoryObj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepository.Update(categoryObj);
                _unitOfWork.Save();
                TempData["success"] = "Edit category successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        /**
         * @DESC: Find the delete object
         * @METHOD: GET
         * @PARAM: int
         * @RETURN: ViewResult
         *
         */
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _unitOfWork.CategoryRepository.Get(u => u.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        /**
        * @DESC: Delete category
        * @METHOD: POST
        * @PARAM: int
        * @RETURN: ViewResult
        *
        */
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? categoryObj = _unitOfWork.CategoryRepository.Get(u => u.CategoryId == id);
            if (categoryObj == null)
            {
                return NotFound();
            }
            _unitOfWork.CategoryRepository.Remove(categoryObj);
            _unitOfWork.Save();
            TempData["success"] = "Delete category successfully";
            return RedirectToAction("Index");


        }
    }
}
