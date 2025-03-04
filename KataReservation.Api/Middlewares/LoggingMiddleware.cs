using System.Diagnostics;
using Microsoft.IO;

namespace KataReservation.Api.Middlewares;

public class LoggingMiddleware(RequestDelegate requestDelegate, ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<LoggingMiddleware>();
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();

    public async Task Invoke(HttpContext context)
    {
        await LogRequest(context);
        await LogResponse(context);
    }

    private async Task LogRequest(HttpContext context)
    {
        context.Request.EnableBuffering();
        await using var requestStream = _recyclableMemoryStreamManager.GetStream();
        await context.Request.Body.CopyToAsync(requestStream);

        _logger.LogInformation("Endpoint: {Endpoint} - RequestedJSONMessage: {RequestedJSONMessage}",
            context.GetEndpoint()?.DisplayName, ReadStreamInChunks(requestStream));

        context.Request.Body.Position = 0;
    }
    private async Task LogResponse(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        await using var responseBody = _recyclableMemoryStreamManager.GetStream();
        context.Response.Body = responseBody;

        var s = new Stopwatch();
        s.Start();
        await requestDelegate(context);
        s.Stop();

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        _logger.LogInformation("ResponseDelay: {ResponseDelay} - ResponseJSONMessage: {ResponseJSONMessage}", s.Elapsed, text);

        await responseBody.CopyToAsync(originalBodyStream);
    }

    private static string ReadStreamInChunks(Stream stream)
    {
        const int readChunkBufferLength = 4096;
        stream.Seek(0, SeekOrigin.Begin);
        using var textWriter = new StringWriter();
        using var reader = new StreamReader(stream);
        var readChunk = new char[readChunkBufferLength];
        int readChunkLength;

        do
        {
            readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
            textWriter.Write(readChunk, 0, readChunkLength);
        } while (readChunkLength > 0);

        return textWriter.ToString();
    }
}