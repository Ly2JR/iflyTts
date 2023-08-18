using iflyTts;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

const string HOST_URL = "https://tts-api.xfyun.cn/v2/tts";
const string HEADERS = "host date request-line";
const string APP_ID = "";
const string API_SECRET = "";
const string API_KEY = "";
const string TEXT = "Hello,World";

var jsonOption = new JsonSerializerOptions()
{
    PropertyNamingPolicy = new LowerCaseNamingPolicy(),//.NET 8 Support JsonNamingPolicy.SnakeCaseLower
};

var url=AuthUrl(HOST_URL, API_KEY, API_SECRET);
Console.WriteLine(url);
var uri = new Uri(url);

var sendText = new SendTest(APP_ID, TEXT);
var sendByte = JsonSerializer.SerializeToUtf8Bytes(sendText,jsonOption);

using ClientWebSocket ws = new();
await ws.ConnectAsync(uri, default);

Console.WriteLine($"发送数据:{TEXT}");
await ws.SendAsync(sendByte, WebSocketMessageType.Binary, true, default);

//接受数据
var sb = new StringBuilder();
var bytes = new byte[1024];
var result = await ws.ReceiveAsync(bytes, default);
var res = Encoding.UTF8.GetString(bytes, 0, result.Count);
sb.Append(res);
while (!result.EndOfMessage)
{
    result = await ws.ReceiveAsync(bytes, default);
    res = Encoding.UTF8.GetString(bytes, 0, result.Count);
    sb.Append(res);
}
Console.WriteLine($"响应数据:{sb}");


var receiceData = JsonSerializer.Deserialize<ReceiveData>(sb.ToString(),jsonOption);
if (receiceData.Code == 0 && receiceData.Data != null)
{
    var audioBase64String = receiceData.Data.Audio;
    var encoderBytes = Convert.FromBase64String(audioBase64String!);

    var path = GetCurrentProjectPath();
    var savePath = $"{path}/files/{DateTime.Now.Ticks}.pcm";
    using (FileStream outStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
    {
        await outStream.WriteAsync(encoderBytes);
    }
}

await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", default);

string AuthUrl(string hostUrl, string apiKey, string apiSecret)
{
    Uri uri = new Uri(hostUrl);
    //签名时间,RFC1123格式(Thu, 01 Aug 2019 01:53:21 GMT)。
    var date = DateTime.Now.ToUniversalTime().ToString("r");
    //参与签名的字段host,date,
    var signatureOrigin = $"host: {uri.Host}\ndate: {date}\nGET {uri.LocalPath} HTTP/1.1"; 
    //签名结果
    var signature = "";
    var secretByte = Encoding.UTF8.GetBytes(apiSecret);
    using (HMACSHA256 hmac = new HMACSHA256(secretByte))
    {
        var signatureByte = Encoding.UTF8.GetBytes(signatureOrigin);
        byte[] hashValue = hmac.ComputeHash(signatureByte);
        signature=Convert.ToBase64String(hashValue);
    }
    //构建请求参数
    var key = $"api_key=\"{apiKey}\", algorithm=\"hmac-sha256\", headers=\"{HEADERS}\", signature=\"{signature}\"";
    var keyBytes= Encoding.UTF8.GetBytes(key);
    var authorization = Convert.ToBase64String(keyBytes);
    return $"wss://{uri.Host}{uri.LocalPath}?authorization={authorization}&date={date}&host={uri.Host}";
}

string GetCurrentProjectPath()
{
    var runEnvironment = Environment.Version;
    return Environment.CurrentDirectory.Replace(@$"\bin\Debug\net{runEnvironment.Major}.{runEnvironment.Minor}", "");
}