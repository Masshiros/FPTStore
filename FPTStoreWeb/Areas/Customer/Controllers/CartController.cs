using FPTStore.DataAccess.Repository.IRepository;
using FPTStore.Models;
using FPTStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FPTStore.Utility;
using Stripe.Checkout;

namespace FPTStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
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
            // get user id
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId,
                    includeProperties: "Product"),
                OrderHeader = new()

            };
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.UserName;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.UserStreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.UserCity;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.UserState;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.UserPostalCode;
            return View(ShoppingCartVM);
        }
        /**
       * @DESC: Add orderHeader to Db
       * @METHOD: POST
       * @PARAM:
       * @RETURN: ViewResult
       *
       */
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            // get user id
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(
                u => u.ApplicationUserId == userId,
                includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // regular customer - need to make payment
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending; 
            }
            else
            {
                // company user
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            _unitOfWork.OrderHeaderRepository.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.OrderHeaderId,
                    Price = cart.Price,
                    Count = cart.Count,
                };
                _unitOfWork.OrderDetailRepository.Add(orderDetail);
                _unitOfWork.Save();
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // regular customer - need to make payment
                // stripe logic
                var domain = "https://localhost:2607/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain+ $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.OrderHeaderId}",
                    CancelUrl = domain+ "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };
                foreach (var item in ShoppingCartVM.ShoppingCartList)
                {
                    var SessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.ProductTitle
                            },
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(SessionLineItem);
                }
                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.OrderHeaderId,session.Id,session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location",session.Url);
                return new StatusCodeResult(303);
            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.OrderHeaderId });
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader =
                _unitOfWork.OrderHeaderRepository.Get(u => u.OrderHeaderId == id, includeProperties: "ApplicationUser");
            // check this is not a company user
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                // get stripe session
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                // check session status - "paid": update orderHeader table
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(id,session.Id,session.PaymentIntentId);
                    _unitOfWork.OrderHeaderRepository.UpdateStatus(id,SD.StatusApproved,SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
                    HttpContext.Session.Clear();
            }
            // remove user's cart
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCartRepository.GetAll(u=>u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCartRepository.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);
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
                OrderHeader = new()

            };
            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImageRepository.GetAll();
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.ProductId).ToList();
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
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

                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCartRepository
                    .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
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
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCartRepository
                .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
            _unitOfWork.Save();
            return Json(new { success = true, data = cartFromDb });

        }



        #endregion
    }
}
