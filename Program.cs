using static Http;

var filename = @"H:\inpath\nagap_glacier_photos.csv";
var outpath = @"H:\outpath";
var lines = await File.ReadAllLinesAsync(filename);
var cnt = 0;
foreach (var line in lines)
{
    if (cnt == 0) { cnt++; continue; }
    var split = line.Split(',');
    var outfile = Path.Combine(outpath, split[8]);
    if (File.Exists(outfile)) continue;
    Console.Write("Downloading..." + split[8]);
    var resp = await Call(HttpVerbs.GET, "https://arcticdata.io/metacat/d1/mn/v2/object/" + split[10]);
    await File.WriteAllBytesAsync(outfile, resp.Response);
    Console.WriteLine("...Done");
}
public static class Http
{
    public static async Task<ApiResponse> Call(HttpVerbs verb, string api)
    {
        using HttpClient client = new();
        client.BaseAddress = new Uri(api);
        HttpResponseMessage r = new();
        r = verb switch
        {
            HttpVerbs.GET => await client.GetAsync((string?)null),
            _ => throw new ArgumentException("not supported - " + verb, nameof(verb)),
        };

        return new ApiResponse
        {
            StatusCode = (int)r.StatusCode,
            Response = await r.Content.ReadAsByteArrayAsync()
        };
    }

    public class ApiResponse
    {
        public required int StatusCode { get; set; }
        public required byte[] Response { get; set; }
    }


    //
    // Summary:
    //     Enumerates the HTTP verbs.
    [Flags]
    public enum HttpVerbs
    {
        //
        // Summary:
        //     Retrieves the information or entity that is identified by the URI of the request.
        GET = 1,
        //
        // Summary:
        //     Posts a new entity as an addition to a URI.
        POST = 2,
        //
        // Summary:
        //     Replaces an entity that is identified by a URI.
        PUT = 4,
        //
        // Summary:
        //     Requests that a specified URI be deleted.
        DELETE = 8,
        //
        // Summary:
        //     Retrieves the message headers for the information or entity that is identified
        //     by the URI of the request.
        HEAD = 0x10,
        //
        // Summary:
        //     Requests that a set of changes described in the request entity be applied to
        //     the resource identified by the Request- URI.
        PATCH = 0x20,
        //
        // Summary:
        //     Represents a request for information about the communication options available
        //     on the request/response chain identified by the Request-URI.
        OPTIONS = 0x40
    }
}