using AutoMapper;
using AutoMapper.QueryableExtensions;
using ODataVersioningSample.Models;
using ODataVersioningSample.V2.ViewModels;
using ODataVersioningSample.Extensions;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using System.Data.Entity;
using ODataVersioningSample.V2.Models;

namespace ODataVersioningSample.V2.Controller
{
    public class ProductsV2Controller : EntitySetController<Product, long>
    {
        private ProductRepository _repository = new ProductRepository(new DbProductsContext());

        public override IQueryable<Product> Get()
        {
            return _repository.Get();
        }

        public override HttpResponseMessage Get([FromODataUri] long key)
        {
            return Request.CreateResponse(
                HttpStatusCode.OK,
                _repository.GetByID(key, Request));
        }

        protected override Product CreateEntity(Product entity)
        {
            return _repository.Create(entity);
        }

        protected override Product PatchEntity(long key, Delta<Product> patch)
        {
            return _repository.Patch(key, patch, Request);
        }

        protected override Product UpdateEntity(long key, Product update)
        {
            return _repository.Update(key, update, Request);
        }

        public override void Delete([FromODataUri] long key)
        {
            _repository.Delete(key, Request);
        }

        public ProductFamily GetFamily([FromODataUri] long key)
        {
            return _repository.GetFamily(key, Request);
        }

        [AcceptVerbs("POST", "PUT")]
        public override void CreateLink([FromODataUri] long key, string navigationProperty, [FromBody] Uri link)
        {
            _repository.CreateLink(key, navigationProperty, link, Request);
        }

        protected override long GetKey(Product entity)
        {
            return entity.ID;
        }
    }
}