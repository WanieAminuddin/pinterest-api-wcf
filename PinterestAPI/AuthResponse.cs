using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PinterestAPI
{
    public class AuthResponse
    {
        private string access_token;
        public string Access_token
        {
            get
            {
                return access_token;
            }
            set { access_token = value; }
        }
        public string refresh_token { get; set; }
        public string clientId { get; set; }
        public string secret { get; set; }

        public string state { get; set; } 

        

        public static AuthResponse get(string response)
        {
            AuthResponse result = JsonConvert.DeserializeObject<AuthResponse>(response);
            return result;
        }

        public static AuthResponse Exchange(string authCode, string clientid, string secret, string redirectUri)
        {

            var request = (HttpWebRequest)WebRequest.Create("https://api.pinterest.com/v1/oauth/token");

            string postData = string.Format("code={0}&redirect_uri={1}&client_id={2}&client_secret={3}&scope=&grant_type=authorization_code", authCode, redirectUri, clientid, secret);
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var x = AuthResponse.get(responseString);

            x.clientId = clientid;
            x.secret = secret;

            return x;
        }

        public static Uri GetAutenticationURI(string clientId, string redirectUri, string state)
        {
            if (string.IsNullOrEmpty(redirectUri))
            {
                redirectUri = string.Format("https://localhost/pinauth/auth/pin/callback/");
            }
            string oauth = string.Format("https://api.pinterest.com/oauth/?response_type=code&redirect_uri={0}&client_id={1}&scope=read_public,write_public&state={2}", redirectUri, clientId, state);
            return new Uri(oauth);
        }

        public static string APIWebRequest(string method, string url, string postData)
        {
            Uri uri = new Uri(url);           

            HttpWebRequest webRequest = null;

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method;
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            webRequest.AllowWriteStreamBuffering = true;

            webRequest.PreAuthenticate = true;
            webRequest.ServicePoint.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

            if (postData != null)
            {
                byte[] fileToSend = Encoding.UTF8.GetBytes(postData);
                webRequest.ContentLength = fileToSend.Length;

                Stream reqStream = webRequest.GetRequestStream();

                reqStream.Write(fileToSend, 0, fileToSend.Length);
                reqStream.Close();
            }

            var returned = WebResponseGet(webRequest);
            return returned;

        }
        public static string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }

            return responseData;
        }
    }
}
