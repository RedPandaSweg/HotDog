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
    public class IngredientController : TableController<Ingredient>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            Context context = new Context();
            DomainManager = new EntityDomainManager<Ingredient>(context, Request);
        }

        public IQueryable<Ingredient> GetAllIngredients()
        {
            return Query();
        }

        public SingleResult<Ingredient> GetIngredient(string id)
        {
            return Lookup(id);
        }

        public Task<Ingredient> PatchIngredient(string id, Delta<Ingredient> patch)
        {
            return UpdateAsync(id, patch);
        }

        public async Task<IHttpActionResult> PostIngredient(Ingredient item)
        {
            Ingredient current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        public Task DeleteIngredient(string id)
        {
            return DeleteAsync(id);
        }
    }
}