using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechStore.Data.Repositories;
using TechStore.Interfaces;
using TechStore.Models;
using Microsoft.AspNetCore.Http;

namespace TechStore.Services
{
    public class ProductsService : Service<Product, Guid>, IProductsService, IProductQueries, IProductLifecycle
    {
        private readonly IProductsRepository _repository;

        public ProductsService(IProductsRepository repository) : base(repository, repository, repository)
        {
            _repository = repository;
        }

        public async Task<Product?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _repository.GetByIdWithDetailsAsync(id);
        }

    }
}
