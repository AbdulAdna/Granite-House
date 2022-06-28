using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Extensions;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public ShoppingCartViewModel shoppingCartVM { get; set; }

        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            shoppingCartVM = new ShoppingCartViewModel()
            {
                Products = new List<Products>()
            };
        }

        //Get Index shopping Cart
        public async Task<IActionResult> Index()
        {
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            if (lstShoppingCart!= null && lstShoppingCart.Count > 0)
            {
                foreach (int cartItem in lstShoppingCart)
                {
                    Products prod = _db.Products.Include(p=>p.SpecialTags).Include(p => p.ProductTypes).Where(p => p.Id == cartItem).FirstOrDefault();
                    shoppingCartVM.Products.Add(prod);
                }
            }

            return View(shoppingCartVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            List<int> lstCardItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            shoppingCartVM.Appointments.AppointmentDate = shoppingCartVM.Appointments.AppointmentDate.AddHours(shoppingCartVM.Appointments.ApponntmentTime.Hour)
                                                                                                     .AddMinutes(shoppingCartVM.Appointments.ApponntmentTime.Minute);
            Appointments appointments = shoppingCartVM.Appointments;
            _db.Appointments.Add(appointments);
            _db.SaveChanges();

            int appointmentId = appointments.Id;

            foreach (int productId in lstCardItems)
            {
                ProductsSelectedForAppointment productsSelectedForAppointment = new ProductsSelectedForAppointment()
                {
                    AppointmentId = appointmentId,
                    ProductId = productId
                };
                _db.ProductsSelectedForAppointments.Add(productsSelectedForAppointment);
                
            }
            _db.SaveChanges();

            lstCardItems = new List<int>();
            HttpContext.Session.Set("ssShoppingCart", lstCardItems);
            
            return RedirectToAction("AppointmentConfirmation", "ShoppingCart" , new { id = appointmentId});
        }

        public IActionResult Remove(int id)
        {
            List<int> lstCardItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            if (lstCardItems.Count > 0)
            {
                if (lstCardItems.Contains(id))
                {
                    lstCardItems.Remove(id);                 
                }

                HttpContext.Session.Set("ssShoppingCart", lstCardItems);
            }

           
            return RedirectToAction(nameof(Index));
        }

        //Get Action Method
        public IActionResult AppointmentConfirmation(int id)
        {
            shoppingCartVM.Appointments = _db.Appointments.Where(a => a.Id == id).FirstOrDefault();
            List<ProductsSelectedForAppointment> objProdList = _db.ProductsSelectedForAppointments.Where(p => p.AppointmentId == id).ToList();

            foreach (ProductsSelectedForAppointment prodAptObj in objProdList)
            {
                shoppingCartVM.Products.Add(_db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags).Where(p => p.Id == prodAptObj.ProductId).FirstOrDefault());
            }


            return View(shoppingCartVM);
        }

    }
}