using Polly;
using ShoppingCart.ShoppingCart;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.Scan(selector =>
    selector.FromAssemblyOf<Program>()
        .AddClasses(c => c.Where(t => t != typeof(ProductCatalogClient) && t.GetMethods().All(m => m.Name != "<Clone>$")))
        .AsImplementedInterfaces()
);
builder.Services.AddHttpClient<IProductCatalogClient, ProductCatalogClient>()
    .AddTransientHttpErrorPolicy(p =>
        p.WaitAndRetryAsync(
            3,
            attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt))
        ));

var app = builder.Build();
app.UseHttpsRedirection();
app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
