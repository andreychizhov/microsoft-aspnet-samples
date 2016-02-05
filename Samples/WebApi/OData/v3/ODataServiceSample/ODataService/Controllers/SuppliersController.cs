using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using ODataService.Models;
using ODataService.Extensions;

namespace ODataService.Controllers
{
    /// <summary>
    /// This controller implements support for Suppliers EntitySet.
    /// It does not implement everything, it only supports Query, Get by Key and Create, 
    /// by handling these requests:
    /// 
    /// GET /Suppliers
    /// GET /Suppliers(key)
    /// GET /Suppliers?$filter=..&$orderby=..&$top=..&$skip=..
    /// POST /Suppliers
    /// </summary>
    public class SuppliersController : ODataController
    {
        ProductsContext _db = new ProductsContext();

        public IQueryable<Supplier> Get()
        {
            return _db.Suppliers;
        }

        public HttpResponseMessage Get([FromODataUri] int key)
        {
            Supplier supplier = _db.Suppliers.SingleOrDefault(s => s.ID == key);
            if (supplier == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, supplier);
            }
        }

        public HttpResponseMessage Post(Supplier supplier)
        {
            supplier.ProductFamilies = null;

            Supplier addedSupplier = _db.Suppliers.Add(supplier);
            _db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Created, addedSupplier);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
