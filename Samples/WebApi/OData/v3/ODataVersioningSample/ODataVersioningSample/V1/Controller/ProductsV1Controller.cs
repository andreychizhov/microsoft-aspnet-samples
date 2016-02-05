using AutoMapper;
using AutoMapper.QueryableExtensions;
using ODataVersioningSample.Models;
using ODataVersioningSample.V1.ViewModels;
using V2VM = ODataVersioningSample.V2.ViewModels;
using ODataVersioningSample.V2.Controller;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using ODataVersioningSample.V2.Models;

namespace ODataVersioningSample.V1.Controller
{
    public class ProductsV1Controller : EntitySetController<Product, int>
    {
        private ProductRepository _repository = new ProductRepository(new DbProductsContext());

        public override IQueryable<Product> Get()
        {
            return _repository.Get().Project().To<Product>();
        }

        public override HttpResponseMessage Get([FromODataUri] int key)
        {
            var v2Product = _repository.GetByID((long)key, Request);

            return Request.CreateResponse(
                HttpStatusCode.Created,
                Mapper.Map<Product>(v2Product));
        }

        protected override Product CreateEntity(Product entity)
        {
            var v2Product = _repository.Create(Mapper.Map<V2VM.Product>(entity));
            return Mapper.Map<Product>(v2Product);
        }

        protected override Product PatchEntity(int key, Delta<Product> patch)
        {
            Delta<V2VM.Product> v2Patch = new Delta<V2VM.Product>();
            foreach (string name in patch.GetChangedPropertyNames())
            {
                object value;
                if (patch.TryGetPropertyValue(name, out value))
                {
                    v2Patch.TrySetPropertyValue(name, value);
                }
            }
            var v2Product = _repository.Patch((long)key, v2Patch, Request);
            return Mapper.Map<Product>(v2Product);
        }

        protected override Product UpdateEntity(int key, Product update)
        {
            var v2Product = _repository.Update((long)key, Mapper.Map<V2VM.Product>(update), Request);
            return Mapper.Map<Product>(v2Product);
        }

        public override void Delete([FromODataUri] int key)
        {
            _repository.Delete((long)key, Request);
        }

        protected override int GetKey(Product entity)
        {
            return entity.ID;
        }
    }
}