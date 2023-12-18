using FPTStore.DataAccess.Repository.IRepository;
using FPTStore.Models;
using FPTStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FPTStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /**
        * @DESC: Display cart page
        * @METHOD: GET
        * @PARAM: 
        * @RETURN: ViewResult
        *
        */
        public IActionResult Index()
        {
            
            return View();
        }

        /**
        * @DESC: Display summary page
        * @METHOD: GET
        * @PARAM:
        * @RETURN: ViewResult
        *
        */
        public IActionResult Summary()
        {
            return View();
        }
       
        #region HelperMethod
        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
        #endregion

        #region API
        /**
       * @DESC: API to get cart data
       * @METHOD: GET
       * @PARAM:
       * @RETURN: Json
       *
       */
        public IActionResult GetCart()
        {
            // get user id
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId,
                    includeProperties: "Product"),

            };
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderTotal += (cart.Price * cart.Count);
            }
            return Json(new{data=ShoppingCartVM});
        }
        /**
         * @DESC: API to Add one more product in cart
         * @METHOD: GET
         * @PARAM: int
         * @RETURN: Json
         *
         */
        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.ShoppingCartId == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCartRepository.Update(cartFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, data = cartFromDb });

        }
        /**
          * @DESC:API to Minus one more product in cart
          * @METHOD: GET
          * @PARAM: int
          * @RETURN: Json
          *
          */
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.ShoppingCartId == cartId);
            if (cartFromDb.Count <= 1)
            {
                _unitOfWork.ShoppingCartRepository.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCartRepository.Update(cartFromDb);
            }
            _unitOfWork.Save();
            return Json(new { success = true, data = cartFromDb });

        }
        /**
          * @DESC: API to Remove a cart
          * @METHOD: GET
          * @PARAM: int
          * @RETURN: Json
          *
          */
        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.ShoppingCartId == cartId);
            _unitOfWork.ShoppingCartRepository.Remove(cartFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, data = cartFromDb });

        }



        #endregion
    }
}
