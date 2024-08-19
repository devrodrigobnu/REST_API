using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using api.Models;

namespace api.Services
{
    public class JsonFileService
    {
        private readonly string _dbContext;

        public JsonFileService(string dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            if (!File.Exists(_dbContext))
            {
                return new List<Product>();
            }

            var json = await File.ReadAllTextAsync(_dbContext);
            return JsonSerializer.Deserialize<List<Product>>(json) ?? new List<Product>();
        }

        public async Task SaveProductAsync(List<Product> products)
        {
            var json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_dbContext, json);
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var products = await GetProductsAsync();
            return products.FirstOrDefault(p => p.Id == id);
        }

        public async Task AddProductAsync(Product product)
        {
            var products = await GetProductsAsync();
            product.Id = products.Any() ? products.Max(p => p.Id) + 1 : 1;
            products.Add(product);
            await SaveProductsAsync(products);
        }

        public async Task UpdateProductAsync(Product updatedProduct)
        {
            var products = await GetProductsAsync();
            var index = products.FindIndex(p => p.Id == updatedProduct.Id);
            if (index != -1)
            {
                products[index] = updatedProduct;
                await SaveProductsAsync(products);
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var products = await GetProductsAsync();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                products.Remove(product);
                await SaveProductsAsync(products);
            }
        }

        public async Task SaveProductsAsync(List<Product> products)
        {
            var json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_dbContext, json);
        }

    }
}
