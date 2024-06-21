using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication12.Models;

namespace WebApplication12.Controllers
{
    public class DbViewController : Controller
    {
        EFContext _context;
        public DbViewController(EFContext context)
        {
            _context = context;
        }
        public IActionResult Items()
        {
            List<Item> items = new List<Item>();
            items = _context.Items.ToList();
            return View("Items", model: items);
        }
        public IActionResult DisableItem(int id)
        {
            Item? item = _context.Items.Where(x => x.id == id).FirstOrDefault();
            if (item != null && !item.isDeleted)
            {
                item.isDeleted = true;
                _context.SaveChanges();
                return Redirect("Items");
            }
            else
            {
                return StatusCode(401, "Object is already disabled or do not exist");
            }
        }
        public IActionResult OrderList()
        {
            List<Order> orders = _context.Orders.Include(x=>x.Items).ToList();
            return View("Orders",orders);
        }
        public IActionResult CreateOrder() 
        {
            _context.Orders.Add(new Order());
            _context.SaveChanges();
            return Redirect("OrderList");
        }
        public IActionResult AddItemToOrder(int OrderId,int ItemId)
        {
            Order order = _context.Orders.Where(x=>x.Id ==  OrderId).Include(x=>x.Items).FirstOrDefault();
            Item item = _context.Items.Where(x=>x.id ==  ItemId).FirstOrDefault();
            if (order is null || item is null)
                return StatusCode(401, "Order or Item do not exist");
            order.Items.Add(item);
            _context.SaveChanges();
            return Redirect("OrderList");
        }
        public IActionResult RemoveFromOrder(int OrderId, int ItemId)
        {
            Order order = _context.Orders.Where(x => x.Id == OrderId).Include(x => x.Items).FirstOrDefault();
            Item item = _context.Orders.Where(x => x.Id == OrderId).Include(x => x.Items).FirstOrDefault().Items.Where(x=>x.id == ItemId).FirstOrDefault();
            if (order is null || item is null)
                return StatusCode(401, "Order or Item do not exist");
            order.Items.Remove(item);
            _context.SaveChanges();

            return Redirect("OrderList");

        }
    }
}
