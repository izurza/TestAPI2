using System.Text.Json;
using System.Configuration;
using System.Net;

namespace BlazorAppTest.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    public TokenService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    public Token? GetToken()
    {
        string jsonCred = "";
        using (StreamReader sr = File.OpenText(_configuration["Credentials:location"])) 
        {
            jsonCred = sr.ReadToEnd();
        }

        return JsonSerializer.Deserialize<Token>(jsonCred);

    }
    public string GetAccessToken()
    {
        Token? token = GetToken();
        return token.token_type + " " + token.access_token;
    }
    public async void RenewToken()
    {
        var response = await _httpClient.PostAsJsonAsync(_configuration["Credentials:url"],GetCredential());
        var result = await response.Content.ReadAsStringAsync();
        Console.WriteLine(result);
        string path = _configuration["Credentials:location"];
        if (!File.Exists(path))
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.Write(result);
            }
        }
        else
        {
            File.Delete(path);
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.Write(result);
            }
        }
        Console.WriteLine("Token renovado");

    }

    public Credential GetCredential()
    {
        return new Credential()
        {
            client_id = _configuration["Credentials:client_id"],
            client_secret = _configuration["Credentials:client_secret"],
            audience = _configuration["Credentials:audience"],
            grant_type = _configuration["Credentials:grant_type"]
        };
       
    }

    public class Token
    {
        public string access_token { get; set; }
        public string scope { get; set; }
        public long expires_in { get; set; }
        public string token_type { get; set; }

    }

    public class Credential
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string audience { get; set; }
        public string grant_type { get; set; }

    }

}
