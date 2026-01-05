using System.Text.Json;
using System.Text.Json.Serialization;

namespace VietLab.Middleware;

/// <summary>
/// Middleware để convert OData response sang camelCase
/// </summary>
public class CamelCaseODataMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JsonSerializerOptions _jsonOptions;

    public CamelCaseODataMiddleware(RequestDelegate next)
    {
        _next = next;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Chỉ xử lý OData endpoints
        if (context.Request.Path.StartsWithSegments("/odata"))
        {
            var originalBodyStream = context.Response.Body;

            try
            {
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    await _next(context);

                    // Chỉ xử lý nếu response là JSON và thành công
                    if (context.Response.ContentType?.Contains("application/json") == true 
                        && context.Response.StatusCode >= 200 
                        && context.Response.StatusCode < 300)
                    {
                        responseBody.Seek(0, SeekOrigin.Begin);
                        var responseText = await new StreamReader(responseBody).ReadToEndAsync();

                        if (!string.IsNullOrEmpty(responseText) && responseText.Trim().StartsWith("{"))
                        {
                            try
                            {
                                // Parse và re-serialize với camelCase
                                using (var jsonDoc = JsonDocument.Parse(responseText))
                                {
                                    var convertedJson = JsonSerializer.Serialize(jsonDoc, _jsonOptions);
                                    var bytes = System.Text.Encoding.UTF8.GetBytes(convertedJson);
                                    
                                    // Reset response body
                                    context.Response.Body = originalBodyStream;
                                    context.Response.ContentLength = bytes.Length;
                                    
                                    await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                                    await context.Response.Body.FlushAsync();
                                    return;
                                }
                            }
                            catch
                            {
                                // Nếu không parse được, giữ nguyên response
                                context.Response.Body = originalBodyStream;
                                responseBody.Seek(0, SeekOrigin.Begin);
                                await responseBody.CopyToAsync(originalBodyStream);
                                return;
                            }
                        }
                    }
                    
                    // Không phải JSON hoặc lỗi, copy trực tiếp
                    context.Response.Body = originalBodyStream;
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception)
            {
                // Nếu có lỗi, đảm bảo response body được restore
                context.Response.Body = originalBodyStream;
                throw;
            }
        }
        else
        {
            await _next(context);
        }
    }
}

