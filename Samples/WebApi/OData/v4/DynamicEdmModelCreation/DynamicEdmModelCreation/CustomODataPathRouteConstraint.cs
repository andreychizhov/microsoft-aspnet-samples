using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace DynamicEdmModelCreation
{
    public class CustomODataPathRouteConstraint : ODataPathRouteConstraint
    {
        public Func<HttpRequestMessage, IEdmModel> EdmModelProvider { get; set; }

        public CustomODataPathRouteConstraint(
            IODataPathHandler pathHandler,
            Func<HttpRequestMessage, IEdmModel> modelProvider,
            string routeName,
            IEnumerable<IODataRoutingConvention> routingConventions)
            : base(pathHandler, new EdmModel(), routeName, routingConventions)
        {
            EdmModelProvider = modelProvider;
        }

        public override bool Match(
            HttpRequestMessage request,
            IHttpRoute route,
            string parameterName,
            IDictionary<string, object> values,
            HttpRouteDirection routeDirection)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (values == null)
            {
                throw new ArgumentNullException("values");
            } 
            
            if (routeDirection != HttpRouteDirection.UriResolution)
            {
                return true;
            }

            object odataPathRouteValue;
            if (!values.TryGetValue(ODataRouteConstants.ODataPath, out odataPathRouteValue))
            {
                return false;
            }

            string odataPath = odataPathRouteValue as string ?? string.Empty;

            ODataPath path;
            IEdmModel model;
            try
            {
                request.Properties[Constants.CustomODataPath] = odataPath;
                model = EdmModelProvider(request);
                odataPath = (string)request.Properties[Constants.CustomODataPath];
                path = PathHandler.Parse(model, odataPath);
            }
            catch (ODataException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            if (path == null)
            {
                return false;
            }

            HttpRequestMessageProperties odataProperties = request.ODataProperties();
            odataProperties.Model = model;
            odataProperties.PathHandler = PathHandler;
            odataProperties.Path = path;
            odataProperties.RouteName = RouteName;
            odataProperties.RoutingConventions = RoutingConventions;

            if (values.ContainsKey(ODataRouteConstants.Controller))
            {
                return true;
            }

            string controllerName = SelectControllerName(path, request);
            if (controllerName != null)
            {
                values[ODataRouteConstants.Controller] = controllerName;
            }

            return true;
        }
    }
}