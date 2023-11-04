using Microsoft.EntityFrameworkCore;
using Schurko.Foundation.Patterns;
using Schurko.Foundation.Tests.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Tests.Patterns
{
    [TestClass]
    public class ScheduleTests
    {
        [TestMethod]
        public void Repository_InMemory_Test()
        {
            using (var context = new MemoryDbContext())
            {
                MemoryRepository repo = new MemoryRepository(context);
                Product p = new Product() { Id = 123, Name = "Raspberry Pi", Description = "Raspberry Pi 4 loaded with Parrot OS." };
                Product pp = new Product() { Id = 456, Name = "Pineapple IV", Description = "Pineapple IV Router by HAK5." };
                repo.Add(p);
                repo.Add(pp);
                Assert.IsTrue(repo.SaveChanges());
                var pid = repo.GetById(123);
                Assert.IsTrue(pid.Id == 123);
                var products = repo.GetAll();
                Assert.IsTrue(products != null);
            }
        }
    }

    public class MemoryDbContext : DbContext
    {
        public MemoryDbContext() { }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "InMemDb");
        }
        public DbSet<Product> Products { get; set; }
    }

    public class MemoryRepository : Repository<Product>
    {
        private readonly MemoryDbContext _dbContext;

        public MemoryRepository(MemoryDbContext ctx)
        {
            _dbContext = ctx;
        }

        public override void Add(Product entity)
        {
            _dbContext.Add(entity);
        }

        public override void Delete(Product entity)
        {
            _dbContext.Remove(entity);
        }

        public override IEnumerable<Product> GetAll()
        {
            return _dbContext.Products;
        }

        public override Product GetById(int id)
        {
            return _dbContext.Products.FirstOrDefault<Product>(cc => cc.Id == id);
        }

        public override bool SaveChanges()
        {
            _dbContext.SaveChanges();
            return true;
        }

        public override void Update(Product entity)
        {
            _dbContext.Update(entity);
        }
    }
}
