using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;

namespace VietLab.Filters;

/// <summary>
/// Schema Filter để convert tất cả property names sang camelCase trong Swagger schema
/// </summary>
public class CamelCaseSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null || schema.Properties.Count == 0)
            return;

        // Tạo dictionary mới với keys đã convert sang camelCase
        var newProperties = new Dictionary<string, OpenApiSchema>();
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        foreach (var property in schema.Properties)
        {
            // Convert property name sang camelCase
            var camelCaseName = jsonOptions.PropertyNamingPolicy?.ConvertName(property.Key) ?? property.Key;
            newProperties[camelCaseName] = property.Value;
        }

        // Thay thế properties cũ bằng properties mới
        schema.Properties = newProperties;

        // Cập nhật Required properties list
        if (schema.Required != null && schema.Required.Count > 0)
        {
            var newRequired = new HashSet<string>();
            foreach (var required in schema.Required)
            {
                var camelCaseName = jsonOptions.PropertyNamingPolicy?.ConvertName(required) ?? required;
                newRequired.Add(camelCaseName);
            }
            schema.Required = newRequired;
        }
    }
}

