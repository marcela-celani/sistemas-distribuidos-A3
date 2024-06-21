namespace web_api.Model
{

    using Microsoft.OpenApi.Any;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using web_api.Model;

    public class SwaggerDefaultValues : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || context?.Type != typeof(User)) return;

            if (schema.Properties.ContainsKey("Id"))
            {
                schema.Properties["Id"].Default = new OpenApiString(string.Empty);
            }
        }
    }
}
