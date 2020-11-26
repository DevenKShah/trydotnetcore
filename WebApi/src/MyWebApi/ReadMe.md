# How to do things in WebApi

>AspNetCore .Net 5

> Resource https://dotnetdetail.net/asp-net-core-3-0-web-api-versioning-best-practices/

## DOCUMENTATION

### ENABLE SWAGGER DOCUMENTATION

Add package `Swashbuckle.AspNetCore` to the api project.


In `Startup.ConfigureServices` add:

```c#
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyWebApi", Version = "v1" });
});
```

In `Startup.Configure` add:

```c#
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyWebApi v1"));
```

### ADD VERSION TO PATH

Add package `Microsoft.AspNetCore.Mvc.Versioning` to the api project.

In `Startup.ConfigureServices` add following before adding swaggerGen:

```c#
services.AddApiVersioning(o => 
{
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1, 0);
});
```

In `Startup.ConfigureServices` add another `SwaggerDoc`. Also create and add filters required for path replacement:

```c#
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyWebApi", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "MyWebApi", Version = "v1" });
    c.ResolveConflictingActions(a => a.First()); //If multiple actions found list the first one
    c.OperationFilter<RemoveVersionFromParameter>();
    c.DocumentFilter<ReplaceVersionWithExactValuePath>();
});
```

In `Startup.Configure` add another `SwaggerEndPoint`:

```c#
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyWebApi v1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "MyWebApi v2");
});
```

Decorate controller with following attributes:

```c#
[ApiVersion("1")]
[Route("/api/v{version:apiVersion}/[controller]")]
[ApiExplorerSettings(GroupName = "v1")]
public class WeatherForecastController : ControllerBase
```

#### To add new version within the same controller

- Decorate action with:

```c#
    [MapToApiVersion("2")]
    [ApiExplorerSettings(GroupName = "v2")]
    [HttpGet]
    public IEnumerable<WeatherForecast> GetV2()
```

- Decorate controller with:

```c#
[ApiController]
[ApiVersion("1")]
[ApiVersion("2")]
[Route("/api/v{version:apiVersion}/[controller]")]
public class WeatherForecastController : ControllerBase
```

#### To add version in the headers

- Add `HeaderApiVersionReader` to `ApiVersioning` in `Startup.ConfigureServices`

```c#
services.AddApiVersioning(o => 
{
    o.AssumeDefaultVersionWhenUnspecified = true; //defaults to DefaultApiVersion
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.ApiVersionReader = new HeaderApiVersionReader("X-Version");
    o.ReportApiVersions = true; //returns the supported versions in response header
});
```

- Controller's `Route` can be simplified as such `[Route("api/[Controller]")]`

- To add the header on swagger page create and add following `SwaggerOperationVersionHeaderFilter` filter in `Startup.ConfigureServices` :

```c#
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyWebApi", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "MyWebApi", Version = "v2" });
    c.SwaggerDoc("v3", new OpenApiInfo { Title = "MyWebApi", Version = "v3" });
    c.ResolveConflictingActions(a => a.First());
    c.OperationFilter<RemoveVersionFromParameter>();
    c.DocumentFilter<ReplaceVersionWithExactValuePath>();
    c.OperationFilter<SwaggerOperationVersionHeaderFilter>(); //Adds header on swagger page
});
```
