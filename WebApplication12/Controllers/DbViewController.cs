using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication12.Models;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        public IActionResult OrderList()
        {
            List<Order> orders = _context.Orders
                .Include(x=>x.Items)
                .Include(x=>x.User)
                .ToList();
            return View("Orders",orders);
        }
        [Authorize]
        public IActionResult CreateOrder() 
        {
            int UserId = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimTypes.Sid).FirstOrDefault()?.Value);
            _context.Orders.Add(new Order() { DateTime = DateTime.Now, User = _context.Users.Where(x => x.Id == UserId).FirstOrDefault() }) ;
            _context.SaveChanges();
            return Redirect("OrderList");
        }
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        public IActionResult RemoveOrder(int OrderId) 
        {

            Order order = _context.Orders.Where(x => x.Id == OrderId).FirstOrDefault();
            if(order is null)
            {
                return StatusCode(401,"such order do not exist");
            }
            _context.Orders.Remove(order);
            _context.SaveChanges();
            return Redirect("OrderList");
        }
        public IActionResult RegisterView() 
        {
            return View("Register");

        }
        public IActionResult LoginView()
        {
            return View("Login");
        }
        public IActionResult Register(string email, string password)
        {
            User user = new User();
            user.Email = email;
            user.Password = password;
            _context.Users.Add(user);
            _context.SaveChanges();
            return Redirect("LoginView");
        }
        public IActionResult Login(string email,string password)
        {
            User user = _context.Users.Where(x => x.Password == password && x.Email == email).FirstOrDefault();
            if (user is not null)
            {
                List<Claim> claims = new List<Claim>() { new Claim(ClaimTypes.Email, email.ToLower()), new Claim(ClaimTypes.Sid, user.Id.ToString()) };
                ClaimsIdentity identity = new ClaimsIdentity(claims, "Cookies");
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                return Redirect("OrderList");
            }
            else
            {
                return StatusCode(401);
            }
            
        }
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Items");
        }
    }
}
