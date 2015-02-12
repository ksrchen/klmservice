using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace loader
{
    class Program
    {
        private static HttpClient GetClient(string baseUrl)
        {
            
            var client = new HttpClient();

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        static void Main(string[] args)
        {

            StreamReader reader = new StreamReader(@".\resincome.txt");
            string line = string.Empty;
            int lineCount = 0;
            string[] columnHeader = null;
            List<ResIncome> resIncomes = new List<ResIncome>();
            while(true)
            {
                line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }
                lineCount++;

                if (lineCount == 1)
                {
                    columnHeader = line.Split(new char[] { '\t' });
                }
                else
                {
                    var data = line.Split(new char[] { '\t' });

                    ResIncome resIncome = new ResIncome();
                    resIncome.MLnumber = data[Array.IndexOf(columnHeader, "MLnumber")];
                    resIncome.MLSID = data[Array.IndexOf(columnHeader, "MLSID")];
                    resIncome.StreetName = data[Array.IndexOf(columnHeader, "StreetName")];
                    resIncome.StreetNumber = data[Array.IndexOf(columnHeader, "StreetNumber")];
                    resIncome.City = data[Array.IndexOf(columnHeader, "City")];
                    resIncome.State = data[Array.IndexOf(columnHeader, "State")];
                    resIncome.PostalCode = data[Array.IndexOf(columnHeader, "PostalCode")];
                    resIncome.PostalCodePlus4 = data[Array.IndexOf(columnHeader, "PostalCodePlus4")];

                    string address = string.Format("{0} {1} {2} {3}, {4}",
                        resIncome.StreetNumber,
                        resIncome.StreetName,
                        resIncome.City,
                        resIncome.State,
                        resIncome.PostalCode);

                    var client = GetClient(@"http://maps.googleapis.com");
                    string path = @"maps/api/geocode/json?sensor=true&address=" + HttpUtility.UrlEncode(address);
                    var content = client.GetAsync(path).Result.Content;
                    dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(content.ReadAsStringAsync().Result);
                    if (result.status == "OK")
                    {
                        resIncome.Latitude = result.results[0].geometry.location.lat;
                        resIncome.longitude = result.results[0].geometry.location.lng;
                        resIncomes.Add(resIncome);
                    }
                    else
                    {
                    }


                    if (resIncomes.Count >= 1000)
                    {
                        var klmServiceClient = GetClient(@"http://kmlservice.azurewebsites.net/");

                        var status = klmServiceClient.PostAsJsonAsync<List<ResIncome>>(@"api/resincome/", resIncomes).Result.Content.ReadAsStringAsync().Result;

                        resIncomes.Clear();
                        break;
                    }

                }


            }

        }
    }
}
