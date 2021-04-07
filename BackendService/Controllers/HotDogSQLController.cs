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
    public class HotDogSQLController : TableController<HotDogSQL>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            Context context = new Context();
            DomainManager = new EntityDomainManager<HotDogSQL>(context, Request);
        }

        public IQueryable<HotDogSQL> GetAllHotDogs()
        {
            return Query();
        }

        public SingleResult<HotDogSQL> GetHotDog(string id)
        {
            return Lookup(id);
        }

        public Task<HotDogSQL> PatchHotDog(string id, Delta<HotDogSQL> patch)
        {
            return UpdateAsync(id, patch);
        }

        public async Task<IHttpActionResult> PostHotDog(HotDogSQL item)
        {
            HotDogSQL current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        public Task DeleteHotDog(string id)
        {
            return DeleteAsync(id);
        }
    }
}