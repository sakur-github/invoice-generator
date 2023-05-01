using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceGenerator.Helpers
{
    internal class GoogleApiHelper
    {
        public static GoogleApiHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GoogleApiHelper();
                return _instance;
            }
        }

        private static GoogleApiHelper? _instance;
        private HttpClient client;
        
        public GoogleApiHelper()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://fonts.google.com/download");
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36");
        }

        public async Task<HttpResponseMessage> GetResponseAsync(string endpoint)
        {
            return await client.GetAsync(endpoint);
        }
    }
}
