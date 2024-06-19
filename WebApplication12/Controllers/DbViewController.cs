using Microsoft.AspNetCore.Mvc;
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
                return StatusCode(401, "Object is already Disabled or do not exist");
            }
        }
    }
}
