using Com.Ctrip.Framework.Apollo.ConfigAdapter;

YamlConfigAdapter.Register();

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddApollo(false);

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.Run(context =>
{
    context.Response.StatusCode = 404;

    var key = context.Request.Query["key"];
    if (string.IsNullOrWhiteSpace(key)) return Task.CompletedTask;

    var value = context.RequestServices.GetRequiredService<IConfiguration>()[key];
    if (value != null) context.Response.StatusCode = 200;

    context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";

    return context.Response.WriteAsync(value ?? "undefined");
});

app.Run();
