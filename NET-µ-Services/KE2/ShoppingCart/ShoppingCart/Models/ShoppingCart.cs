using ShoppingCart.Events;

namespace ShoppingCart.Models;

public class ShoppingCart
{
    private readonly HashSet<ShoppingCartItem> items = new();

    public ShoppingCart(int userId) => this.UserId = userId;

    public IEnumerable<ShoppingCartItem> Items => this.items;
    public int UserId { get; }

    public void AddItems(IEnumerable<ShoppingCartItem> shoppingCartItems, IEventStore eventStore)
    {
        foreach (var item in shoppingCartItems)
            if (this.items.Add(item))
                eventStore.Raise("ShoppingCartItemAdded", new { this.UserId, item });
    }

    public void RemoveItems(int[] productCatalogueIds, IEventStore eventStore) =>
      this.items.RemoveWhere(i => productCatalogueIds.Contains(i.ProductCatalogueId));
}

public record ShoppingCartItem(
  int ProductCatalogueId,
  string ProductName,
  string Description,
  Money Price)
{
    public virtual bool Equals(ShoppingCartItem? obj) =>
      obj != null && this.ProductCatalogueId.Equals(obj.ProductCatalogueId);

    public override int GetHashCode() => this.ProductCatalogueId.GetHashCode();
}

public record Money(string Currency, decimal Amount);