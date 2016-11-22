#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"
#r "System.Drawing
 
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.IO; 
using System.Drawing;
 
public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    var image = await req.Content.ReadAsStreamAsync();
 
    MemoryStream mem = new MemoryStream();
    image.CopyTo(mem); //make a copy since one gets destroy in the other API. Lame, I know.
    image.Position = 0;
    mem.Position = 0;
     
    string result = await CallVisionAPI(image); 
    log.Info(result); 
 
    if (String.IsNullOrEmpty(result)) {
        return req.CreateResponse(HttpStatusCode.BadRequest);
    }
     
    ImageData imageData = JsonConvert.DeserializeObject<ImageData>(result);
 
    MemoryStream outputStream = new MemoryStream();
    using(Image maybeFace = Image.FromStream(mem, true))
    {
        using (Graphics g = Graphics.FromImage(maybeFace))
        {
            Pen yellowPen = new Pen(Color.Yellow, 4);
            foreach (Face face in imageData.Faces)
            {
                var faceRectangle = face.FaceRectangle;
                g.DrawRectangle(yellowPen, 
                    faceRectangle.Left, faceRectangle.Top, 
                    faceRectangle.Width, faceRectangle.Height);
            }
        }
        maybeFace.Save(outputStream, ImageFormat.Jpeg);
    }
     
    var response = new HttpResponseMessage()
    {
        Content = new ByteArrayContent(outputStream.ToArray()),
        StatusCode = HttpStatusCode.OK,
    };
    response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
    return response;
}
  
static async Task<string> CallVisionAPI(Stream image)
{
    using (var client = new HttpClient())
    {
        var content = new StreamContent(image);
        var url = "https://api.projectoxford.ai/vision/v1.0/analyze?visualFeatures=Faces";
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("Vision_API_Subscription_Key"));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        var httpResponse = await client.PostAsync(url, content);
 
        if (httpResponse.StatusCode == HttpStatusCode.OK){
            return await httpResponse.Content.ReadAsStringAsync();
        }
    }
    return null;
}

public class ImageData {
    public List<Face> Faces { get; set; }
}
 
public class Face {
    public int Age { get; set; }
    public string Gender { get; set; }
    public FaceRectangle FaceRectangle { get; set; }
}
 
public class FaceRectangle : TableEntity {
    public string ImageFile { get; set; }
    public int Left { get; set; }
    public int Top { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}