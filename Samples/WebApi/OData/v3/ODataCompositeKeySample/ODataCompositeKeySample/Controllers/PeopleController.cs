using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using Microsoft.Data.OData.Query;
using ODataCompositeKeySample.Extensions;
using ODataCompositeKeySample.Models;

namespace ODataCompositeKeySample.Controllers
{
    [ModelValidationFilter]
    public class PeopleController : ODataController
    {
        private PeopleRepository _repo = new PeopleRepository();

        public IEnumerable<Person> Get()
        {
            return _repo.Get();
        }

        public Person Get([FromODataUri] string firstName, [FromODataUri] string lastName)
        {
            Person person = _repo.Get(firstName, lastName);
            if (person == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return person;
        }

        public HttpResponseMessage PutPerson([FromODataUri] string firstName, [FromODataUri] string lastName, Person person)
        {
            _repo.UpdateOrAdd(person);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [AcceptVerbs("PATCH", "MERGE")]
        public HttpResponseMessage PatchPerson([FromODataUri] string firstName, [FromODataUri] string lastName, Delta<Person> delta)
        {
            var person = _repo.Get(firstName, lastName);
            if (person == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            delta.Patch(person);

            person.FirstName = firstName;
            person.LastName = lastName;
            _repo.UpdateOrAdd(person);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public HttpResponseMessage PostPerson(Person person)
        {
            _repo.UpdateOrAdd(person);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, person);

            var path = Request.GetODataPath();
            string key = string.Format(
                "{0}={1},{2}={3}",
                "FirstName", ODataUriUtils.ConvertToUriLiteral(person.FirstName, Microsoft.Data.OData.ODataVersion.V3),
                "LastName", ODataUriUtils.ConvertToUriLiteral(person.LastName, Microsoft.Data.OData.ODataVersion.V3));

            response.Headers.Location = new Uri(
                Url.ODataLink(
                    new EntitySetPathSegment(path.EntitySet.Name),
                    new KeyValuePathSegment(key)));

            return response;
        }

        public HttpResponseMessage DeletePerson([FromODataUri] string firstName, [FromODataUri] string lastName)
        {
            var person = _repo.Remove(firstName, lastName);
            if (person == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, person);
        }
    }
}
