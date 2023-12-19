using FPTStore.DataAccess.Repository.IRepository;
using FPTStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using FPTStore.Models.ViewModels;
using FPTStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Stripe;
using Stripe.Checkout;

namespace FPTStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /**
         * @DESC: Return Order Page
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
        * @DESC: Return Order Detail Page
        * @METHOD: GET
        * @PARAM: int
        * @RETURN: ViewResult
        *
        */
        public IActionResult Details(int orderId)
        {
             OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeaderRepository.Get(u => u.OrderHeaderId == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetailRepository.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")

            };
            return View(OrderVM);
        }
        /**
        * @DESC: Update Order  Detail Page
        * @METHOD: Post
        * @PARAM: OrderVM (has been binding)
        * @RETURN: ViewResult
        *
        */
        [HttpPost]
        [Authorize(Roles=SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
           var orderHeaderFromDb = _unitOfWork.OrderHeaderRepository.Get(u=>u.OrderHeaderId == OrderVM.OrderHeader.OrderHeaderId);
           orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
           orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
           orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
           orderHeaderFromDb.City = OrderVM.OrderHeader.City;
           orderHeaderFromDb.State = OrderVM.OrderHeader.State;
           orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
           if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
           {
               orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
           }
           if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
           {
               orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
           }
           _unitOfWork.OrderHeaderRepository.Update(orderHeaderFromDb);
           _unitOfWork.Save();

           TempData["Success"] = "Order Details Updated Successfully.";


           return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.OrderHeaderId });
           
        }
        /**
         * @DESC: Update OrderStatus from Approved to Processing
         * @METHOD: Post
         * @PARAM: OrderVM (has been binding)
         * @RETURN: ViewResult
         *
         */
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeaderRepository.UpdateStatus(OrderVM.OrderHeader.OrderHeaderId,SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.OrderHeaderId });
        }
        /**
          * @DESC: Update OrderStatus from Processing to Shipped
          * @METHOD: Post
          * @PARAM: OrderVM (has been binding)
          * @RETURN: ViewResult
          *
          */
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrderHeaderRepository.Get(u => u.OrderHeaderId == OrderVM.OrderHeader.OrderHeaderId);
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            // if this is company , they will have 30 days to pay
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            _unitOfWork.OrderHeaderRepository.Update(orderHeader);
            _unitOfWork.Save();
            TempData["success"] = "Order Shipped Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.OrderHeaderId });
        }
        /**
          * @DESC: Cancel order 
          * @METHOD: Post
          * @PARAM: OrderVM (has been binding)
          * @RETURN: ViewResult
          *
          */
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeaderRepository.Get(u => u.OrderHeaderId == OrderVM.OrderHeader.OrderHeaderId);
            // if this is customer - and their payment is approved, we will refund
            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                _unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.OrderHeaderId,SD.StatusCancelled,SD.StatusRefunded);
            }
            // this is a company order - just set order and payment status to Cancel
            else
            {
                _unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.OrderHeaderId, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["success"] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.OrderHeaderId });
        }
        /**
          * @DESC: Only for company - pay for their approved order
          * @METHOD: Post
          * @PARAM: OrderVM (has been binding)
          * @RETURN: ViewResult
          *
          */
        [ActionName("Details")]
        [HttpPost]
        public IActionResult Details_PAY_NOW()
        {
            OrderVM.OrderHeader = _unitOfWork.OrderHeaderRepository
                .Get(u => u.OrderHeaderId == OrderVM.OrderHeader.OrderHeaderId, includeProperties: "ApplicationUser");
            OrderVM.OrderDetail = _unitOfWork.OrderDetailRepository
                .GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.OrderHeaderId, includeProperties: "Product");
            //stripe logic
            var domain = "https://localhost:2607/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.OrderHeaderId}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.OrderHeaderId}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in OrderVM.OrderDetail)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.ProductTitle
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }


            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(OrderVM.OrderHeader.OrderHeaderId, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
           
        }
        /**
          * @DESC: Return payment confirmation page
          * @METHOD: Get
          * @PARAM: int
          * @RETURN: ViewResult
          *
          */
        public IActionResult PaymentConfirmation(int orderHeaderId)
        {

            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.Get(u => u.OrderHeaderId == orderHeaderId);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                //this is an order by company

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }


            }


            return View(orderHeaderId);
        }

        #region API CALLS
        /**
         * @DESC: API to fetch all order
         * @METHOD: POST
         * @PARAM: string
         * @RETURN: JSON
         *
         */
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaders;
            // if user role are admin and employee, get all order
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                objOrderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                // if just a normal customer or company , get only their order
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId= claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                objOrderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(u => u.ApplicationUserId == userId,includeProperties: "ApplicationUser");
            }
            switch (status)
            {
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment || u.PaymentStatus == SD.PaymentStatusPending);
                    break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }
            return Json(new { data = objOrderHeaders});
        }
       
        #endregion
    }
}
