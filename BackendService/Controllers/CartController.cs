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
    public class CartController : TableController<Cart>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            Context context = new Context();
            DomainManager = new EntityDomainManager<Cart>(context, Request);
        }

        public IQueryable<Cart> GetAllCarts()
        {
            return Query();
        }

        public SingleResult<Cart> GetCart(string id)
        {
            return Lookup(id);
        }

        public Task<Cart> PatchCart(string id, Delta<Cart> patch)
        {
            return UpdateAsync(id, patch);
        }

        public async Task<IHttpActionResult> PostCart(Cart item)
        {
            Cart current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        public Task DeleteCart(string id)
        {
            return DeleteAsync(id);
        }
    }
}