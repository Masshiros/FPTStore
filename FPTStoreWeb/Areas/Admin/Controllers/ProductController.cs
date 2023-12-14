using FPTStore.DataAccess.Data;
using FPTStore.DataAccess.Repository;
using FPTStore.DataAccess.Repository.IRepository;
using FPTStore.Models;
using FPTStore.Models.ViewModels;
using FPTStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;

namespace FPTStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
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
            List<Product> objProductList = _unitOfWork.ProductRepository.GetAll(includeProperties:"Category").ToList();
            
            return View(objProductList);
        }
        /**
         * @DESC: Return the update or insert product view
         * @METHOD: GET
         * @PARAM: int
         * @RETURN: ViewResult
         *
         */
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(u =>
                    new SelectListItem
                    {
                        Text = u.CategoryName,
                        Value = u.CategoryId.ToString()
                    }),
                Product = new Product()
            };
            if (id == null | id == 0)
            {
                // update
                return View(productVM);
            }
            else
            {
                // insert
                productVM.Product = _unitOfWork.ProductRepository.Get(u => u.ProductId == id);
                return View(productVM);
            }
        }

        /**
         * @DESC: Create new product or edit product
         * @METHOD: POST
         * @PARAM: ProductVM , IFormFile?
         * @RETURN: ViewResult
         *
         */
        [HttpPost]
        public IActionResult Upsert(ProductVM productObj, IFormFile? file)
        {
            /*if (productObj.ProductTitle == productObj..ToString())
            {
                ModelState.AddModelError("CategoryName", "The DisplayOrder cannot exactly match the Category Name");
            }*/
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product\");
                    if (!string.IsNullOrEmpty(productObj.Product.ImageUrl))
                    {
                        // delete the old image 
                        var oldImagePath = Path.Combine(wwwRootPath,productObj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productObj.Product.ImageUrl = @"\images\product\" + fileName;
                }

                // check whether action is add or update
                if (productObj.Product.ProductId == 0)
                {
                    _unitOfWork.ProductRepository.Add(productObj.Product);
                    TempData["success"] = "Create product successfully";
                }
                else
                {
                    _unitOfWork.ProductRepository.Update(productObj.Product);
                    TempData["success"] = "Edit product successfully";
                }
                _unitOfWork.Save();
               
                return RedirectToAction("Index");
            }
            else
            {
                productObj.CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(u =>
                    new SelectListItem
                    {
                        Text = u.CategoryName,
                        Value = u.CategoryId.ToString()
                    });
                TempData["error"] = "Some Error Happen";
                return View(productObj);
            }
        }
        #region API CALLS
            /**
             * @DESC: API to fetch all product
             * @METHOD: POST
             * @PARAM: 
             * @RETURN: JSON
             *
             */
            [HttpGet]
            public IActionResult GetAll()
            {
                List<Product> objProductList = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category").ToList();
                return Json(new { data = objProductList });
            }
            /**
             * @DESC: API to delete product
             * @METHOD: DELETE
             * @PARAM: int
             * @RETURN: JSON
             *
             */
        [HttpDelete]
            public IActionResult Delete(int? id)
            {
                var productToBeDeleted = _unitOfWork.ProductRepository.Get(u =>u.ProductId == id);
                if (productToBeDeleted == null)
                {
                    return Json(new { success = false, message = "Error while deleting" });
                }
                // delete the old image 
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                _unitOfWork.ProductRepository.Remove(productToBeDeleted);
                _unitOfWork.Save();
              
                return Json(new { success = true, message = "Delete Successful" });

        }
            #endregion
    }
}
