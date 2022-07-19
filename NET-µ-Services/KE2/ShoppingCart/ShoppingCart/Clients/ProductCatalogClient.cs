namespace ShoppingCart.Clients;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using ShoppingCart;
using ShoppingCart.Models;

public interface IProductCatalogClient
{
    Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productCatalogueIds);
}

public class ProductCatalogClient : IProductCatalogClient
{
    private static readonly string GetProductPathTemplate = "?productIds=[{0}]";
    private static readonly string ProductCatalogueBaseUrl = @"https://git.io/JeHiE";
    private readonly HttpClient client;

    public ProductCatalogClient(HttpClient client)
    {
        client.BaseAddress = new Uri(ProductCatalogueBaseUrl);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        this.client = client;
    }

    public async Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productCatalogIds)
    {
        using var response = await RequestProductFromProductCatalogue(productCatalogIds);
        return await ConvertToShoppingCartItems(response);
    }

    private static async Task<IEnumerable<ShoppingCartItem>> ConvertToShoppingCartItems(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        var products = await
          JsonSerializer.DeserializeAsync<List<ProductCatalogProduct>>(
            await response.Content.ReadAsStreamAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
          ?? new();
        return products
          .Select(p =>
            new ShoppingCartItem(
              p.ProductId,
              p.ProductName,
              p.ProductDescription,
              p.Price
          ));
    }

    private async Task<HttpResponseMessage> RequestProductFromProductCatalogue(int[] productCatalogueIds)
    {
        var productsResource = string.Format(GetProductPathTemplate, string.Join(",", productCatalogueIds));
        return await this.client.GetAsync(productsResource);
    }

    private record ProductCatalogProduct(int ProductId, string ProductName, string ProductDescription, Money Price);
}