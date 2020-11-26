# How to do things in WebApi

>AspNetCore .Net 5

## DOCUMENTATION

### ENABLE SWAGGER DOCUMENTATION

Add package `Swashbuckle.AspNetCore` to the api project.


In startup `ConfigureServices` add:

```c#
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyWebApi", Version = "v1" });
});
```

In startup `Configure` add:

```c#
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyWebApi v1"));
```

### ADD VERSION TO PATH

Add package `Microsoft.AspNetCore.Mvc.Versioning` to the api project.

In startup `ConfigureServices` add following before adding swaggerGen:

```c#
services.AddApiVersioning(o => 
{
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1, 0);
});
```

In startup `ConfigureServices` add another `SwaggerDoc`. Also create and add filters required for path replacement:

```c#
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyWebApi", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "MyWebApi", Version = "v1" });
    c.ResolveConflictingActions(a => a.First());
    c.OperationFilter<RemoveVersionFromParameter>();
    c.DocumentFilter<ReplaceVersionWithExactValuePath>();
});
```

In startup `Configure` add another `SwaggerEndPoint`:

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
```
