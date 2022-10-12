// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class TraceHttpDto
{
    public string Name { get; set; }

    public int Status { get; set; }

    [JsonPropertyName("http.method")]
    public string Method { get; set; }

    [JsonPropertyName("http.url")]
    public string Url { get; set; }

    [JsonPropertyName("http.target")]
    public string Target { get; set; }

    [JsonPropertyName("http.host")]
    public string Host { get; set; }

    [JsonPropertyName("http.scheme")]
    public string Scheme { get; set; }

    [JsonPropertyName("http.status_code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("http.flavor")]
    public string Flavor { get; set; }

    [JsonPropertyName("http.user_agent")]
    public string UserAgent { get; set; }

    [JsonPropertyName("http.request_content_length")]
    public int RequestContentLength { get; set; }

    [JsonPropertyName("http.request_content_length_uncompressed")]
    public int RequestContentLengthUncompressed { get; set; }

    [JsonPropertyName("http.response_content_length")]
    public int ResponseContentLength { get; set; }

    [JsonPropertyName("http.response_content_length_uncompressed")]
    public int ResponseContentLengthUncompressed { get; set; }

    [JsonPropertyName("http.retry_count")]
    public int RetryCount { get; set; }

    [JsonPropertyName("net.peer.ip")]
    public string PeerIp { get; set; }

    [JsonPropertyName("net.peer.port")]
    public int? PeerPort { get; set; }

    public Dictionary<string, IEnumerable<string>> RequestHeaders { get; set; }

    public Dictionary<string, IEnumerable<string>> ReponseHeaders { get; set; }

    /// <summary>
    /// http client
    /// </summary>
    [JsonPropertyName("net.peer.name")]
    public string PeerName { get; set; }

    #region http server
    [JsonPropertyName("http.server_name")]
    public string ServerName { get; set; }

    [JsonPropertyName("http.route")]
    public string Route { get; set; }

    [JsonPropertyName("http.client_ip")]
    public string ClientIp { get; set; }
    #endregion
}
