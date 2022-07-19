namespace ShoppingCart.Controllers;

using System.Threading.Tasks;
using ShoppingCart.Models;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart;
using ShoppingCart.Events;
using ShoppingCart.Clients;

[Route("/shoppingcart")]
public class ShoppingCartController : ControllerBase
{
    private readonly IEventStore eventStore;
    private readonly IProductCatalogClient productCatalog;
    private readonly IShoppingCartStore shoppingCartStore;

    public ShoppingCartController(
      IShoppingCartStore shoppingCartStore,
      IProductCatalogClient productCatalog,
      IEventStore eventStore)
    {
        this.shoppingCartStore = shoppingCartStore;
        this.productCatalog = productCatalog;
        this.eventStore = eventStore;
    }

    [HttpDelete("{userid:int}/items")]
    public ShoppingCart Delete(int userId, [FromBody] int[] productIds)
    {
        var shoppingCart = this.shoppingCartStore.Get(userId);
        shoppingCart.RemoveItems(productIds, this.eventStore);
        this.shoppingCartStore.Save(shoppingCart);
        return shoppingCart;
    }

    [HttpGet("{userId:int}")]
    public ShoppingCart Get(int userId) => this.shoppingCartStore.Get(userId);

    [HttpPost("{userId:int}/items")]
    public async Task<ShoppingCart> Post(int userId, [FromBody] int[] productIds)
    {
        var shoppingCart = shoppingCartStore.Get(userId);
        var shoppingCartItems = await this.productCatalog.GetShoppingCartItems(productIds);
        shoppingCart.AddItems(shoppingCartItems, eventStore);
        this.shoppingCartStore.Save(shoppingCart);

        return shoppingCart;
    }
}