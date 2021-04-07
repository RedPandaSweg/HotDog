﻿using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using BackendService.DataObjects;
using BackendService.Models;

namespace BackendService.Controllers
{
    public class OrderController : TableController<Order>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            Context context = new Context();
            DomainManager = new EntityDomainManager<Order>(context, Request);
        }

        public IQueryable<Order> GetAllOrders()
        {
            return Query();
        }

        public SingleResult<Order> GetOrder(string id)
        {
            return Lookup(id);
        }

        public Task<Order> PatchOrder(string id, Delta<Order> patch)
        {
            return UpdateAsync(id, patch);
        }

        public async Task<IHttpActionResult> PostOrder(Order item)
        {
            Order current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        public Task DeleteOrder(string id)
        {
            return DeleteAsync(id);
        }
    }
}