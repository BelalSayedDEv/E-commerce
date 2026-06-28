using E_Commerce.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinqMastryController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public LinqMastryController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("linq-test")]
        public IActionResult TestLinq()
        {
            //IQueryable<Product> query = context.Products;

            //Console.WriteLine("BASE");
            //Console.WriteLine(query.ToQueryString());

            //query = query.Where(p => p.Price > 100);

            //Console.WriteLine("WHERE");
            //Console.WriteLine(query.ToQueryString());

            //var projection = query.Select(p => new
            //{
            //    p.Id,
            //    p.Name
            //});

            //Console.WriteLine("SELECT");
            //Console.WriteLine(projection.ToQueryString());

            //var ordered = projection.OrderBy(p => p.Name);

            //Console.WriteLine("ORDER");
            //Console.WriteLine(ordered.ToQueryString());


            //var query = context.Products
            //    .GroupBy(p => p.CategoryID)
            //    .Select(g => new { CategoryId = g.Key, count = g.Count() });

            //Console.WriteLine("Group By");
            //Console.WriteLine(query.ToQueryString());


            //// Approach A: Navigation property

            //var query = context.Products.Select(p => new { Name = p.Name, Category = p.Category.Name });
            //Console.WriteLine("Navigation property");
            //Console.WriteLine(query.ToQueryString());

            //// Approach B: Explicit Join

            //query = context.Products.Join(context.Categories, p => p.CategoryID, c => c.Id, (p, c) => new { Name = p.Name, Category = c.Name });
            //Console.WriteLine("Explicit Join");
            //Console.WriteLine(query.ToQueryString());

            //var query = context.Users.SelectMany(u => u.Orders).SelectMany(o => o.Items);
            //Console.WriteLine(query.ToQueryString());

            //var query = context.Products.OrderBy(p => p.CategoryID).ThenByDescending(p => p.Price);
            //Console.WriteLine(query.ToQueryString());

            //var Status = new[] { "Pending", "Processing" };


            //var query = context.Orders.Where(o => Status.Contains(o.Status));
            //Console.WriteLine(query.ToQueryString());

            //var query4=  context.Orders.Where(p => p.Status == "Pending" || p.Status == "Processing");

            //var query1 = context.Products.GroupBy(p => p.Category.Name)
            //    .Select(g => new
            //    {
            //        CategoryName = g.Key,
            //        ProductCount = g.Count(),
            //        AvaregePrice = g.Average(p => p.Price),
            //        CheapestProduct = g.OrderBy(p => p.Price).Select(p => p.Name).FirstOrDefault(),
            //        MostExpensiveProduct = g.OrderByDescending(p => p.Price).Select(p => p.Name).FirstOrDefault()

            //    });

            //var query1 = context.Products.Select(p => new
            //{
            //    CategoryName = p.Category.Name,
            //    ProductName = p.Name,
            //    TotalSoldQuantity = p.OrderItems.Sum(Order => Order.Quantity),
            //    TotalRevenue =
            //    p.OrderItems.Sum(oi => oi.Price * oi.Quantity),
            //    NumberOfOrdersThatIncludedThisProduct = p.OrderItems.Count()

            //});

            //var query1 = context.OrderItems.GroupBy(o => o.Product.Name).Select(g => new
            //{
            //    Name = g.Key,
            //    TotalQuantitySold = g.Sum(p => p.Quantity),
            //    TotalRevenue = g.Sum(o => o.Price * o.Quantity),
            //    NumberOfUniqueBuyers = g.Select(o => o.Order.UserId).Distinct().Count()
            //}).OrderByDescending(o => o.TotalRevenue);

            var query1 = context.Products.Include(p => p.Comments);

            Console.WriteLine(query1.ToQueryString());

            var query2 = context.Products.Include(p => p.Comments).AsSplitQuery();

            Console.WriteLine(query2.ToQueryString());

            return Ok();
        }
    }
}
