using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using POSAccounting.Models;

namespace POSAccounting.Server
{
    public class Service 
    {
        private static string BaseURI { get { return "http://localhost:31205/Api/"; } }
        public static string Login { get { return "Account/Login"; } }
        public static string ClinetAppInit { get { return "ClinetAppInit/Index"; } }
        public static string GetUserActions { get { return "User/GetUserActions"; } }

        public static async Task<ResponseM<T>> Post<T,K>(string path, K model ) where T : class 
        {
            ResponseM<T> result = new ResponseM<T>();
            try
            {
                // Posting.  
                using (var client = new HttpClient())
                {
                    // Setting Base address.  
                    client.BaseAddress = new Uri(BaseURI);

                    // Setting content type.  
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Setting timeout.  
                    client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

                    // Initialization.  
                    HttpResponseMessage response = new HttpResponseMessage();

                    // HTTP POST  
                    response = await client.PostAsJsonAsync(path, model).ConfigureAwait(false);
                    result.Code = response.StatusCode;
                    JObject j = response.Content.ReadAsAsync<JObject>().Result;
                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {
                        // Reading Response.                       
                        result.Model = JsonConvert.DeserializeObject<T>(j["Model"].ToString());
                        // Releasing.  
                    }
                    else
                    {
                        result.ErrorResponseM = JsonConvert.DeserializeObject<ErrorResponseM>(j.ToString());
                    }
                    response.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public static async Task<ResponseM<T>> Get<T>(string path, IEnumerable<string> query = null) where T : class
        {
            ResponseM<T> result = new ResponseM<T>();

            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseURI);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));


                    HttpResponseMessage response = await client.GetAsync(path);
                    result.Code = response.StatusCode;
                    JObject j = response.Content.ReadAsAsync<JObject>().Result;
                    // Verification
                    if (response.IsSuccessStatusCode)
                    {
                        // Reading Response.
                        result.Model = JsonConvert.DeserializeObject<T>(j["Model"].ToString());
                        // Releasing.
                    }
                    else
                    {
                        result.ErrorResponseM = JsonConvert.DeserializeObject<ErrorResponseM>(j.ToString());
                    }
                    response.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
            
        }

        public static async Task<T> Delete<T>(string path, int id) where T : new()
        {
            T responseM = new T();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseURI);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

                    HttpResponseMessage response = await client.DeleteAsync(path + $"{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        responseM = await response.Content.ReadAsAsync<T>();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return responseM;

        }
    }
}
