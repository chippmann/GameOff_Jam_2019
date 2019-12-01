using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Planty.Data;
using Planty.Serialization;

namespace Planty.Networking {
    public class RestClient : Node2D {
        private HTTPRequest httpRequest;
        public const string CredentialsFilePath = "res://Credentials/tweetsApiCredentials.json";

        private RestResponse restResponse;

        public override void _Ready() {
            base._Ready();
            httpRequest = new HTTPRequest();
            AddChild(httpRequest);
            httpRequest.Connect("request_completed", this, nameof(OnRequestCompleted));
        }

        public async Task<RestResponse> MakeGetRequest(string url, List<string> headers, string eTag = "") {
            headers.AddRange(GetBasicAuthHeaders());
            headers.Add($"Etag: {eTag}");
            httpRequest.Request(url, headers.ToArray());
            await ToSignal(httpRequest, "request_completed");
            return restResponse;
        }

        public async Task<T> MakeGetRequest<T>(string url, List<string> headers, string eTag = "") {
            headers.AddRange(GetBasicAuthHeaders());
            headers.Add($"Etag: {eTag}");
            httpRequest.Request(url, headers.ToArray());
            await ToSignal(httpRequest, "request_completed");
            return Serializer.Deserialize<T>(restResponse.body);
        }

        private List<string> GetBasicAuthHeaders() {
            var file = new File();
            file.Open(CredentialsFilePath, (int) File.ModeFlags.Read);
            var json = file.GetAsText();
            var credentials = Serializer.Deserialize<Credentials>(json);
            var encoded = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(credentials.user + ":" + credentials.password));
            var basicAuthHeaders = new List<string> {"Authorization: Basic " + encoded};
            return basicAuthHeaders;
        }

        private void OnRequestCompleted(HTTPRequest.Result result, int responseCode, string[] headers, byte[] body) {
            restResponse = new RestResponse {
                result = result,
                responseCode = responseCode,
                headers = headers,
                body = Encoding.UTF8.GetString(body)
            };
        }
    }
}