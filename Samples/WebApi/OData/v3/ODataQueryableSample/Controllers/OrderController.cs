﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Query;
using System.Web.Http.OData.Query.Validators;
using Microsoft.Data.OData;
using Microsoft.Data.OData.Query.SemanticAst;
using ODataQueryableSample.Models;

namespace ODataQueryableSample.Controllers
{
    /// <summary>
    /// This sample order controller demonstrates how to create an action which supports
    /// OData style queries using by accessing the query directly before applying it.
    /// This allows for inspection and manipulation of the query before it is being
    /// applied.
    /// </summary>
    public class OrderController : ApiController
    {
        private static List<Order> OrderList = new List<Order>
        {  
            new Order{ Id = 11, Name = "Order1", Quantity = 1 }, 
            new Order{ Id = 33, Name = "Order3", Quantity = 3 }, 
            new Order{ Id = 4, Name = "Order4", Quantity = 100 }, 
            new Order { Id = 22, Name = "Order2", Quantity = 2 }, 
            new Order { Id = 3, Name = "Order0", Quantity = 0 },
        };

        // Note this can be done through Queryable attribute as well
        public IQueryable<Order> Get(ODataQueryOptions queryOptions)
        {
            // Register a custom FilterByValidator to disallow custom logic in the filter query
            if (queryOptions.Filter != null)
            {
                queryOptions.Filter.Validator = new RestrictiveFilterByQueryValidator();
            }

            // Validate the query, we only allow order by Id property and 
            // we only allow maximum Top query value to be 9
            ODataValidationSettings settings = new ODataValidationSettings(){ MaxTop = 9 };
            settings.AllowedOrderByProperties.Add("Id");
            queryOptions.Validate(settings);

            // Apply the query
            return queryOptions.ApplyTo(OrderList.AsQueryable()) as IQueryable<Order>;
        }

        private class RestrictiveFilterByQueryValidator : FilterQueryValidator
        {   
            public override void ValidateSingleValuePropertyAccessNode(SingleValuePropertyAccessNode propertyAccessNode, ODataValidationSettings settings)
            {
                // Validate if we are accessing some sensitive property of Order, such as Quantity
                if (propertyAccessNode.Property.Name == "Quantity")
                {
                    throw new ODataException("Filter with Quantity is not allowed.");
                }

                base.ValidateSingleValuePropertyAccessNode(propertyAccessNode, settings);
            }
        }
    }
}
