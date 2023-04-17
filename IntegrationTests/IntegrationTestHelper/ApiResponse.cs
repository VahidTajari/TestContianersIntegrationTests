namespace IntegrationTests.IntegrationTestHelper;
public sealed class ApiResponse
{
    public ApiResponse(string path, string method, int responseStatusCode, string responseJson)
    {
        Path = path;
        Method = method;
        ResponseStatusCode = responseStatusCode;
        ResponseJson = responseJson;
    }

    public string Path { get; set; }
    public string Method { get; set; }
    public int ResponseStatusCode { get; set; }
    public string ResponseJson { get; set; }
}
