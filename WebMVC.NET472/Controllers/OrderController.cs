using BikeDistributor.Domain;
using BikeDistributor.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using WebMVC.NET472.Views.Order.OrderViewModel;

namespace WebMVC.NET472.Controllers
{
    public class OrderController : Controller
    {
		UnitOfWork UOW = new UnitOfWork();

		public OrderController()
		{
			var order = new Order(1, "Mountain Bike Shop");
			order.AddLine(new OrderLine(new Bike("Chopper", "1972", Bike.TwoThousand), 10));
			order.AddLine(new OrderLine(new Bike("Mountain Bike", "1999", Bike.OneThousand), 5));
			UOW.OrderService().Add(order);

			order = new Order(2, "Valley Bike Shop");
			order.AddLine(new OrderLine(new Bike("Specialized", "Venge Elite", Bike.TwoThousand), 3));
			order.AddLine(new OrderLine(new Bike("Alchemy", "Arktos", Bike.FourThousand), 2));
			UOW.OrderService().Add(order);

			order = new Order(3, "Rock Bike Shop");
			order.AddLine(new OrderLine(new Bike("Alchemy", "Arktos", Bike.FourThousand), 5));
			order.AddLine(new OrderLine(new Bike("Bianchi", "Infinito CV", Bike.TwoThousand), 1));
			order.AddLine(new OrderLine(new Bike("Giant", "Defy 1", Bike.OneThousand), 2));
			UOW.OrderService().Add(order);
		}

		// GET: Order
		public async Task<ActionResult> Index(int? orderId, bool? showReceipt)
        {
			ViewBag.ShowProducts = false;

			var viewModel = new OrderIndexData();

			viewModel.Orders = await UOW.OrderService().GetAll().ToListAsync();

			if (orderId != null)
			{
				Order order = UOW.OrderService().FindById(orderId.Value);

				viewModel.Lines = order.Lines;

				ViewData["OrderId"] = orderId.Value;

				if (showReceipt != null)
				{
					ViewBag.ShowReceipt = showReceipt.Value;
					viewModel.Receipt = UOW.OrderService().HtmlReceipt(order.OrderId);
				}
				else
				{
					ViewBag.ShowProducts = true;
				}
			}

			return View(viewModel);
		}

		// GET: Order/Details/5
		public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Order/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Order/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Order/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Order/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Order/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Order/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
