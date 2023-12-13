using FPTStore.DataAccess.Data;
using FPTStore.DataAccess.Repository.IRepository;
using FPTStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FPTStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /**
         * @DESC: Display product page
         * @METHOD: GET
         * @PARAM
         * @RETURN: ViewResult
         *
         */
        [HttpGet]
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.ProductRepository.GetAll().ToList();
            
            return View(objProductList);
        }
        /**
         * @DESC: Return the create product view
         * @METHOD: GET
         * @PARAM
         * @RETURN: ViewResult
         *
         */
        [HttpGet]
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(u =>
                new SelectListItem
                {
                    Text = u.CategoryName,
                    Value = u.CategoryId.ToString()
                });
            ViewBag.CategoryList = CategoryList;
            return View();
        }
        /**
         * @DESC: Create new product
         * @METHOD: POST
         * @PARAM: Product
         * @RETURN: ViewResult
         *
         */
        [HttpPost]
        public IActionResult Create(Product productObj)
        {
            /*if (productObj.ProductTitle == productObj..ToString())
            {
                ModelState.AddModelError("CategoryName", "The DisplayOrder cannot exactly match the Category Name");
            }*/
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Add(productObj);
                _unitOfWork.Save();
                TempData["success"] = "Create product successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        /**
         * @DESC: Return the edit product view
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
            Product? product = _unitOfWork.ProductRepository.Get(u => u.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        /**
        * @DESC: Edit product
        * @METHOD: POST
        * @PARAM: Product
        * @RETURN: ViewResult
        *
        */
        [HttpPost]
        public IActionResult Edit(Product productObj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Update(productObj);
                _unitOfWork.Save();
                TempData["success"] = "Edit product successfully";
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
            Product? product = _unitOfWork.ProductRepository.Get(u => u.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        /**
        * @DESC: Delete product
        * @METHOD: POST
        * @PARAM: int
        * @RETURN: ViewResult
        *
        */
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product? product = _unitOfWork.ProductRepository.Get(u => u.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            _unitOfWork.ProductRepository.Remove(product);
            _unitOfWork.Save();
            TempData["success"] = "Delete product successfully";
            return RedirectToAction("Index");


        }
    }
}
