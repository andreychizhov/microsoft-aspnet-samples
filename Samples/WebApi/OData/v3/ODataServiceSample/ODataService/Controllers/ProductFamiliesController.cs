using Microsoft.Data.OData;
using ODataService.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;

namespace ODataService.Controllers
{
    /// <summary>
    /// This controller is responsible for the ProductFamilies entity set.
    /// 
    /// <remarks>
    /// In this example we leverage the EntitySetController<TEntity,TKey> class that
    /// provides basic plumbing for implementing an OData EntitySet.
    /// 
    /// This class overrides all the method needed to provide full support for 
    /// the OData operations currently supported by Web API.
    /// </remarks>
    /// </summary>
    public class ProductFamiliesController : EntitySetController<ProductFamily, int>
    {
        ProductsContext _db = new ProductsContext();

        /// <summary>
        /// Support for querying ProductFamilies
        /// </summary>
        public override IQueryable<ProductFamily> Get()
        {
            // if you need to secure this data, one approach would be
            // to apply a where clause before returning. This way any $filter etc, 
            // will be applied only after $filter
            return _db.ProductFamilies;
        }

        /// <summary>
        /// Support for getting a ProductFamily by key
        /// </summary>
        protected override ProductFamily GetEntityByKey(int key)
        {
           return _db.ProductFamilies.SingleOrDefault(f => f.ID == key);
        }

        /// <summary>
        /// Support for creating a ProductFamily
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected override ProductFamily CreateEntity(ProductFamily entity)
        {
            _db.ProductFamilies.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        /// <summary>
        /// Support for deleting a ProductFamily
        /// </summary>
        /// <param name="key"></param>
        public override void Delete([FromODataUri]int key)
        {
            ProductFamily toDelete = _db.ProductFamilies.FirstOrDefault(f => f.ID == key);
            if (toDelete == null)
            {
                throw ODataErrors.EntityNotFound(Request);
            }
            foreach (var product in toDelete.Products.ToList())
            {
                product.Family = null;
            }

            _db.ProductFamilies.Remove(toDelete);
            _db.SaveChanges();
        }

        /// <summary>
        /// Support for patching a ProductFamily
        /// </summary>
        protected override ProductFamily PatchEntity(int key, Delta<ProductFamily> patch)
        {
            ProductFamily toUpdate = _db.ProductFamilies.FirstOrDefault(f => f.ID == key);
            if (toUpdate == null)
            {
                throw ODataErrors.EntityNotFound(Request);
            }
            patch.Patch(toUpdate);
            _db.SaveChanges();
            return toUpdate;
        }

        /// <summary>
        /// Support for replacing a ProductFamily
        /// </summary>
        protected override ProductFamily UpdateEntity(int key, ProductFamily update)
        {
            if (key != update.ID)
            {
                throw new HttpResponseException(Request.CreateODataErrorResponse(HttpStatusCode.BadRequest, 
                new ODataError()
                {
                    Message = "The supplied key and the ProductFamily being patched do not match."
                }));
            }

            if (!_db.ProductFamilies.Any(p => p.ID == key))
            {
                throw ODataErrors.EntityNotFound(Request);
            }
            update.ID = key; // ignore the key in the entity use the key in the URL.

            _db.ProductFamilies.Attach(update);
            _db.Entry(update).State = EntityState.Modified;
            _db.SaveChanges();
            return update;
        }

        /// <summary>
        /// Support for /ProductFamilies(1)/Products
        /// </summary>
        [Queryable]
        public IQueryable<Product> GetProducts([FromODataUri] int key)
        {
            return _db.ProductFamilies.Where(pf => pf.ID == key).SelectMany(pf => pf.Products);
        }

        /// <summary>
        /// Support for POST /ProductFamiles(1)/Products
        /// </summary>
        public HttpResponseMessage PostProducts([FromODataUri] int key, Product product)
        {
            var family = _db.ProductFamilies.Find(key);
            if (family == null)
            {
                throw Request.EntityNotFound();
            }

            family.Products.Add(product);
            _db.SaveChanges();

            var response = Request.CreateResponse(
                HttpStatusCode.Created,
                product);
            response.Headers.Location = new Uri(Url.ODataLink(
                new EntitySetPathSegment("Products"),
                new KeyValuePathSegment(product.ID.ToString())));
            return response;
        }

        /// <summary>
        /// Support for /ProductFamilies(1)/Supplier
        /// </summary>
        public Supplier GetSupplier([FromODataUri] int key)
        {
            return _db.ProductFamilies.Where(pf => pf.ID == key).Select(pf => pf.Supplier).SingleOrDefault();
        }

        /// <summary>
        /// Support ProductFamily.Products.Add(Product) and ProductFamily.Supplier = Supplier
        /// </summary>
        public override void CreateLink([FromODataUri] int key, string navigationProperty, [FromBody] Uri link)
        {
            ProductFamily family = _db.ProductFamilies.SingleOrDefault(p => p.ID == key);
            if (family == null)
            {
                throw ODataErrors.EntityNotFound(Request);
            }
            switch (navigationProperty)
            {
                case "Products":
                    int productId = Request.GetKeyValue<int>(link);
                    Product product = _db.Products.SingleOrDefault(p => p.ID == productId);
                    if (product == null)
                    {
                        throw ODataErrors.EntityNotFound(Request);
                    }
                    product.Family = family;
                    break;

                case "Supplier":
                    int supplierId = Request.GetKeyValue<int>(link);
                    Supplier supplier = _db.Suppliers.SingleOrDefault(s => s.ID == supplierId);
                    if (supplier == null)
                    {
                        throw ODataErrors.EntityNotFound(Request);
                    }
                    family.Supplier = supplier;
                    break;

                default:
                    base.CreateLink(key, navigationProperty, link);
                    break;
            }
            _db.SaveChanges();
        }

        /// <summary>
        /// Support for ProductFamily.Supplier = null
        /// which uses this Url shape:
        ///     DELETE ~/ProductFamilies(id)/$links/Supplier
        ///     headers
        ///     
        ///     [link]
        /// </summary>
        public override void DeleteLink([FromODataUri] int key, string navigationProperty, [FromBody] Uri link)
        {
            ProductFamily family = _db.ProductFamilies.SingleOrDefault(p => p.ID == key);
            if (family == null)
            {
                throw ODataErrors.EntityNotFound(Request);
            }

            switch (navigationProperty)
            {
                case "Supplier":
                    family.Supplier = null;
                    break;

                default:
                    base.DeleteLink(key, navigationProperty, link);
                    break;

            }
            _db.SaveChanges();
        }

        /// <summary>
        /// Support for ProductFamily.Products.Delete(Product)
        /// 
        /// which uses this URL shape:
        ///     DELETE ~/ProductFamilies(id)/$links/Products(relatedId)
        /// </summary>
        public override void DeleteLink([FromODataUri] int key, string relatedKey, string navigationProperty)
        {
            ProductFamily family = _db.ProductFamilies.SingleOrDefault(p => p.ID == key);
            if (family == null)
            {
                throw ODataErrors.EntityNotFound(Request);
            }

            switch (navigationProperty)
            {
                case "Products":
                    int productId = Convert.ToInt32(relatedKey);
                    Product product = _db.Products.SingleOrDefault(p => p.ID == productId);
                    if (product == null)
                    {
                        throw ODataErrors.EntityNotFound(Request);
                    }
                    product.Family = null;
                    break;


                default:
                    base.DeleteLink(key, relatedKey, navigationProperty);
                    break;
            }
            _db.SaveChanges();
        }

        /// <summary>
        /// Support for /ProductFamilies(1)/CreateProduct
        /// </summary>
        /// <param name="key">Bound key</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        public int CreateProduct([FromODataUri] int key, ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateODataErrorResponse(HttpStatusCode.BadRequest,
                    new ODataError()
                    {
                        Message = ODataHelper.GetModelStateErrorInformation(ModelState)
                    }));
            }
        
            ProductFamily productFamily = _db.ProductFamilies.SingleOrDefault(p => p.ID == key);
            string productName = parameters["Name"].ToString();

            Product product = new Product
            {
                Name = productName,
                Family = productFamily,
                ReleaseDate = DateTime.Now,
                SupportedUntil = DateTime.Now.AddYears(10)
            };
            _db.Products.Add(product);
            _db.SaveChanges();

            return product.ID;
        }

        /// <summary>
        /// Required override to help the base class build self/edit/id links.
        /// </summary>
        protected override int GetKey(ProductFamily entity)
        {
            return entity.ID;
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
