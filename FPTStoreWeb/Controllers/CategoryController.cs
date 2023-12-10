using FPTStoreWeb.Data;
using FPTStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace FPTStoreWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
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
            List<Category> objCategoryList = _db.Categories.ToList();
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
            if(categoryObj.CategoryName == categoryObj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("CategoryName", "The DisplayOrder cannot exactly match the Category Name");
            }
            if (ModelState.IsValid)
            {
                _db.Categories.Add(categoryObj);
                _db.SaveChanges();
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
            Category? category = _db.Categories.Find(id);
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
                _db.Categories.Update(categoryObj);
                _db.SaveChanges();
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
            Category? category = _db.Categories.Find(id);
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
        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? categoryObj = _db.Categories.Find(id);
            if (categoryObj == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(categoryObj);
            _db.SaveChanges();
            TempData["success"] = "Delete category successfully";
            return RedirectToAction("Index");
           
       
        }
    }
}
