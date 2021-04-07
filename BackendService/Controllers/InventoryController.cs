using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using BackendService.DataObjects;
using BackendService.Models;

namespace BackendService.Controllers
{
    public class InventoryController : TableController<Inventory>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            Context context = new Context();
            DomainManager = new EntityDomainManager<Inventory>(context, Request);
        }

        public IQueryable<Inventory> GetAllInventories()
        {
            return Query();
        }

        public SingleResult<Inventory> GetInventory(string id)
        {
            return Lookup(id);
        }

        public Task<Inventory> PatchInventory(string id, Delta<Inventory> patch)
        {
            return UpdateAsync(id, patch);
        }

        public async Task<IHttpActionResult> PostInventory(Inventory item)
        {
            Inventory current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        public Task DeleteInventory(string id)
        {
            return DeleteAsync(id);
        }
    }
}