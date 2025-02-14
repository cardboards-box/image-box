
namespace ImageBox.Core.FileCache.Sources;

internal class HttpFileSource(
    IServiceConfig _config,
    IApiService _api,
    IJsonService _json) : IFileSourceService
{
    public async Task<FileResult?> Fetch(FileFetchProperties properties)
    {
        if (!properties.Source.Type.HasFlag(IOPathType.Http) ||
            !ValidateRequest(properties.Source.OSSafe))
            return null;

        var url = properties.Source.OSSafe;
        var userAgent = properties.UserAgent ?? _config.Requests.UserAgent;
        var accept = properties.Accepts ?? "*/*";

        var bob = _api.Create(url, _json, "GET");
        bob.Accept(accept)
            .UserAgent(userAgent)
            .Message(c => _config.Requests.Configure?.Invoke(c));

        var req = await bob.Result() ?? throw new NullReferenceException($"Request returned null for: {url}");
        req.EnsureSuccessStatusCode();

        var headers = req.Content.Headers;
        var path = headers?.ContentDisposition?.FileName ?? headers?.ContentDisposition?.Parameters?.FirstOrDefault()?.Value ?? "";
        var type = headers?.ContentType?.ToString() ?? "";

        return new(await req.Content.ReadAsStreamAsync(), path, type, properties.ShouldCache);
    }

    public bool ValidateRequest(string path)
    {
        if (!_config.Requests.AllowNetworkRequests) return false;

        if (_config.Requests.AllowedDomains is null ||
            _config.Requests.AllowedDomains.Length == 0) return true;
        
        foreach(var domain in _config.Requests.AllowedDomains)
        {
            var expression = new Regex(domain);
            if (expression.IsMatch(path)) return true;
        }

        return false;
    }
}
