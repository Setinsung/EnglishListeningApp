using System.Net;

namespace Commons.Helpers;

public static class HttpClientExtensions
{
    /// <summary>
    /// 将 HttpResponseMessage 的内容保存到文件中。
    /// </summary>
    /// <param name="respMsg">HttpResponseMessage 对象。</param>
    /// <param name="file">保存的文件路径。</param>
    /// <param name="cancellationToken">取消操作的 CancellationToken。</param>
    /// <returns>表示异步保存操作的任务。</returns>
    public static async Task SaveToFileAsync(this HttpResponseMessage respMsg, string file, CancellationToken cancellationToken = default)
    {
        if (respMsg.IsSuccessStatusCode == false) throw new ArgumentException($"StatusCode of HttpResponseMessage is {respMsg.StatusCode}", nameof(respMsg));
        using FileStream fs = new(file, FileMode.Create);
        await respMsg.Content.CopyToAsync(fs, cancellationToken);
    }

    /// <summary>
    /// 通过 HttpClient 下载文件然后保存到本地。
    /// </summary>
    /// <param name="httpClient">HttpClient 对象。</param>
    /// <param name="url">文件的 URL。</param>
    /// <param name="localFile">本地保存的文件路径。</param>
    /// <param name="cancellationToken">取消操作的 CancellationToken。</param>
    /// <returns>表示异步下载操作的任务，任务返回下载的文件的 HttpStatusCode。</returns>
    public static async Task<HttpStatusCode> DownloadFileAsync(this HttpClient httpClient, Uri url, string localFile, CancellationToken cancellationToken = default)
    {
        var resp = await httpClient.GetAsync(url, cancellationToken);
        if (resp.IsSuccessStatusCode)
        {
            await SaveToFileAsync(resp, localFile, cancellationToken);
        }
        return resp.StatusCode;
    }

    /// <summary>
    /// 通过 HttpClient 获取 JSON 数据并反序列化为指定类型。
    /// </summary>
    /// <typeparam name="T">要反序列化的类型。</typeparam>
    /// <param name="client">HttpClient 对象。</param>
    /// <param name="url">请求URL。</param>
    /// <param name="cancellationToken">取消操作的 CancellationToken。</param>
    /// <returns>异步获取 JSON 数据并反序列化后返回。</returns>
    public static async Task<T?> GetJsonAsync<T>(this HttpClient client, Uri url, CancellationToken cancellationToken = default)
    {
        string json = await client.GetStringAsync(url, cancellationToken);
        return json.ParseJson<T>();
    }
}
