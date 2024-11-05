using System.Drawing;
using System.Drawing.Imaging;
using static Http;

var filename = @"H:\inpath\nagap_glacier_photos.csv";
var outpath = @"H:\outpath";
var checkpath = @"H:\jpg";

var lines = await File.ReadAllLinesAsync(filename);
var cnt = 0;
foreach (var line in lines)
{
    if (cnt == 0) { cnt++; continue; }
    var split = line.Split(',');
    if (Path.GetExtension(split[8]) != ".tif") continue;
    var exist = Path.Combine(checkpath, Path.GetFileNameWithoutExtension(split[8]) + ".jpg");
    if (File.Exists(exist)) { Console.WriteLine("continuing"); continue; }
    var outfile = Path.Combine(outpath, split[8]);
    Console.Write("Downloading..." + split[8]);
    var resp = await Call(HttpVerbs.GET, "https://arcticdata.io/metacat/d1/mn/v2/object/" + split[10]);
    await File.WriteAllBytesAsync(outfile, resp.Response);
    Console.Write("...Converting..." + split[8]);
    Converter.ConvertTiffToJpeg(outfile);
    Console.Write("...Deleting..." + split[8]);
    if (File.Exists(outfile))
    {
        File.Delete(outfile);
    }
    Console.WriteLine("...Done");
    GC.Collect();
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

internal static class Converter
{
    const string OUTPATH = @"H:\jpg";
    public static string[] ConvertTiffToJpeg(string fileName)
    {
        using var imageFile = Image.FromFile(fileName);
        var frameDimensions = new FrameDimension(imageFile.FrameDimensionsList[0]);
        var frameNum = imageFile.GetFrameCount(frameDimensions);         // Gets the number of pages from the tiff image (if multipage) 
        var jpegPaths = new string[frameNum];

        for (var frame = 0; frame < frameNum; frame++)
        {
            // Selects one frame at a time and save as jpeg. 
            imageFile.SelectActiveFrame(frameDimensions, frame);
            using var bmp = new Bitmap(imageFile);
            jpegPaths[frame] = String.Format("{0}\\{1}.jpg", OUTPATH, Path.GetFileNameWithoutExtension(fileName));
            bmp.Save(jpegPaths[frame], ImageFormat.Jpeg);
        }

        return jpegPaths;
    }
}