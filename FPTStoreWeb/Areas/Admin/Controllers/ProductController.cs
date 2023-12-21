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
                // insert
                return View(productVM);
            }
            else
            {
                // update
                productVM.Product = _unitOfWork.ProductRepository.Get(u => u.ProductId == id,includeProperties:"ProductImages");
                return View(productVM);
            }
        }

        /**
         * @DESC: Create new product or edit product
         * @METHOD: POST
         * @PARAM: ProductVM , List<IFormFile>?
         * @RETURN: ViewResult
         *
         */
        [HttpPost]
        public IActionResult Upsert(ProductVM productObj, List<IFormFile>? files)
        {
            /*if (productObj.ProductTitle == productObj..ToString())
            {
                ModelState.AddModelError("CategoryName", "The DisplayOrder cannot exactly match the Category Name");
            }*/
            if (ModelState.IsValid)
            {
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
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-"+ productObj.Product.ProductId;
                        string finalPath = Path.Combine(wwwRootPath,productPath);
                        // check if the finalPath exist 
                        if (!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productObj.Product.ProductId,
                        };
                        if (productObj.Product.ProductImages == null)
                        {
                            productObj.Product.ProductImages = new List<ProductImage>();
                        }
                        productObj.Product.ProductImages.Add(productImage);
                    }
                    _unitOfWork.ProductRepository.Update(productObj.Product);
                    _unitOfWork.Save();

                   
                }

                
               
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

        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _unitOfWork.ProductImageRepository.Get(u => u.ImageId == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,imageToBeDeleted.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.ProductImageRepository.Remove(imageToBeDeleted);
                _unitOfWork.Save();
            TempData["success"] = "Delete Successfully";
            }

            return RedirectToAction(nameof(Upsert), new { id = productId });
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
            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);
            // check if the finalPath exist 
            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(finalPath);
            }
            _unitOfWork.ProductRepository.Remove(productToBeDeleted);
            _unitOfWork.Save();
              
                return Json(new { success = true, message = "Delete Successful" });

            }
            #endregion
    }
}
