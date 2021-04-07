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
    public class ResupplyController : TableController<Resupply>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            Context context = new Context();
            DomainManager = new EntityDomainManager<Resupply>(context, Request);
        }

        public IQueryable<Resupply> GetAllResupplies()
        {
            return Query();
        }

        public SingleResult<Resupply> GetResupply(string id)
        {
            return Lookup(id);
        }

        public Task<Resupply> PatchResupply(string id, Delta<Resupply> patch)
        {
            return UpdateAsync(id, patch);
        }

        public async Task<IHttpActionResult> PostResupply(Resupply item)
        {
            Resupply current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        public Task DeleteResupply(string id)
        {
            return DeleteAsync(id);
        }
    }
}