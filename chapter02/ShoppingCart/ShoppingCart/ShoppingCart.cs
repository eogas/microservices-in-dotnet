using ShoppingCart.EventFeed;

namespace ShoppingCart.ShoppingCart;

public class ShoppingCart
{
    private readonly HashSet<ShoppingCartItem> items = new();

    public int UserId { get; }
    public IEnumerable<ShoppingCartItem> Items => this.items;

    public ShoppingCart(int userId) => this.UserId = userId;

    public void AddItems(IEnumerable<ShoppingCartItem> shoppingCartItems, IEventStore eventStore)
    {
        foreach (var item in shoppingCartItems)
        {
            if (!this.items.Add(item))
            {
                continue;
            }
            
            eventStore.Raise("ShoppingCartItemAdded", new { UserId, item });
        }
    }

    public void RemoveItems(int[] productCatalogueIds, IEventStore eventStore)
    {
        var numRemoved = this.items.RemoveWhere(i => productCatalogueIds.Contains(i.ProductCatalogueId));

        if (numRemoved == 0)
        {
            return;
        }

        eventStore.Raise("ShoppingCartItemsRemoved", new { UserId, productCatalogueIds });
    }
}
