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

        private static double? convert2Double(string val)
        {
            double? dValue = null;
            double temp = 0;
            if (!string.IsNullOrWhiteSpace(val))
            {
                if (double.TryParse(val, out temp))
                {
                    dValue = temp;
                }
            }
            return dValue;
        }

        private static Dictionary<string, string> LoadCityAbbreviation()
        {
            var result = new Dictionary<string, string>();
            StreamReader reader = new StreamReader(@".\CityAreaAbbList.txt");
            string line = string.Empty;
            while (true)
            {
                line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }

                var parts = line.Split(new char[] { ',' });

                result[parts[1]] = parts[0];
            }
            return result;

        }

        static void Main(string[] args)
        {
            loadAttachment();

        }

        static void loadAttachment()
        {
            StreamReader reader = new StreamReader(@".\attachments.txt");
            string line = string.Empty;
            int lineCount = 0;
            string[] columnHeader = null;
            List<attachment> items = new List<attachment>();
            while (true)
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

                    var item = new attachment();
                    item.Board = data[Array.IndexOf(columnHeader, "Board")];
                    item.ClassID = data[Array.IndexOf(columnHeader, "ClassID")];
                    item.ClassKey = data[Array.IndexOf(columnHeader, "ClassKey")];
                    item.ClassSourceKey = data[Array.IndexOf(columnHeader, "ClassSourceKey")];
                    item.FileExtension = data[Array.IndexOf(columnHeader, "FileExtension")];
                    item.IsDeleted = data[Array.IndexOf(columnHeader, "IsDeleted")];
                  //  item.MediaDescription = data[Array.IndexOf(columnHeader, "MediaDescription")];
                    item.MediaKey = data[Array.IndexOf(columnHeader, "MediaKey")];
                    item.MediaURL = data[Array.IndexOf(columnHeader, "MediaURL")];
                    item.MediaType = data[Array.IndexOf(columnHeader, "MediaType")];
                    item.MLS_ID = data[Array.IndexOf(columnHeader, "MLS_ID")];
                    item.OfficeCode = data[Array.IndexOf(columnHeader, "OfficeCode")];
                    item.SourceKey = data[Array.IndexOf(columnHeader, "SourceKey")];
                    item.TimestampModified = data[Array.IndexOf(columnHeader, "TimestampModified")];
                    item.TimestampUploaded = data[Array.IndexOf(columnHeader, "TimestampUploaded")];

                    items.Add(item);
                }

                if (items.Count >= 100)
                {
                    var klmServiceClient = GetClient(@"http://kmlservice.azurewebsites.net/");
                    var status = klmServiceClient.PostAsJsonAsync<List<attachment>>(@"api/attachments/", items).Result.Content.ReadAsStringAsync().Result;
                    items.Clear();
                }
            }
            if (items.Count > 0)
            {
                var klmServiceClient = GetClient(@"http://kmlservice.azurewebsites.net/");
                var status = klmServiceClient.PostAsJsonAsync<List<attachment>>(@"api/attachments/", items).Result.Content.ReadAsStringAsync().Result;
                items.Clear();
            }
        }

        static void loadProperty()
        {
            var CityMap = LoadCityAbbreviation();

            StreamReader reader = new StreamReader(@".\listings.txt");
            StreamWriter writer = new StreamWriter(@"failed.txt");
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
                    resIncome.ListingKey =data[Array.IndexOf(columnHeader, "ListingKey")];
                    resIncome.MLnumber = data[Array.IndexOf(columnHeader, "MLnumber")];
                    resIncome.MLSID = data[Array.IndexOf(columnHeader, "MLSID")];
                    resIncome.StreetName = data[Array.IndexOf(columnHeader, "StreetName")];
                    resIncome.StreetNumber = data[Array.IndexOf(columnHeader, "StreetNumber")];
                    resIncome.City = data[Array.IndexOf(columnHeader, "City")];
                    resIncome.State = data[Array.IndexOf(columnHeader, "State")];
                    resIncome.PostalCode = data[Array.IndexOf(columnHeader, "PostalCode")];
                    resIncome.PostalCodePlus4 = data[Array.IndexOf(columnHeader, "PostalCodePlus4")];


                    resIncome.Acres = convert2Double(data[Array.IndexOf(columnHeader, "Acres")]);
                    resIncome.Floor = data[Array.IndexOf(columnHeader, "Floor")];

                    resIncome.GrossMultiplier = convert2Double(data[Array.IndexOf(columnHeader, "GrossMultiplier")]);
                    resIncome.GrossOperatingIncome = convert2Double(data[Array.IndexOf(columnHeader, "GrossOperatingIncome")]);
                    resIncome.ListPrice = convert2Double(data[Array.IndexOf(columnHeader, "ListPrice")]);
                    resIncome.LotSquareFootage = convert2Double(data[Array.IndexOf(columnHeader, "LotSquareFootage")]);
                    resIncome.MonthlyGrossIncome = convert2Double(data[Array.IndexOf(columnHeader, "MonthlyGrossIncome")]);

                    resIncome.NetOperatingIncome = convert2Double(data[Array.IndexOf(columnHeader, "NetOperatingIncome")]);
                    resIncome.NumberElectricMeters = convert2Double(data[Array.IndexOf(columnHeader, "NumberElectricMeters")]);
                    resIncome.NumberGarageSpaces = convert2Double(data[Array.IndexOf(columnHeader, "NumberGarageSpaces")]);
                    resIncome.NumberGasMeters = convert2Double(data[Array.IndexOf(columnHeader, "NumberGasMeters")]);
                    resIncome.NumberUnits = convert2Double(data[Array.IndexOf(columnHeader, "NumberUnits")]);

                    resIncome.NumberWaterMeters = convert2Double(data[Array.IndexOf(columnHeader, "NumberWaterMeters")]);
                    resIncome.OperatingExpense = convert2Double(data[Array.IndexOf(columnHeader, "OperatingExpense")]);

                    resIncome.Parking = data[Array.IndexOf(columnHeader, "Parking")];
                    resIncome.ParkingSpacesTotal = convert2Double(data[Array.IndexOf(columnHeader, "ParkingSpacesTotal")]);

                    resIncome.Pool = data[Array.IndexOf(columnHeader, "Pool")];
                    resIncome.PreviousPrice = convert2Double(data[Array.IndexOf(columnHeader, "PreviousPrice")]);
                    resIncome.PricePerUnit = convert2Double(data[Array.IndexOf(columnHeader, "PricePerUnit")]);
                    resIncome.PropertyDescription = data[Array.IndexOf(columnHeader, "PropertyDescription")];


                    resIncome.SA_FirstName = data[Array.IndexOf(columnHeader, "SA_FirstName")];
                    resIncome.SA_LastName = data[Array.IndexOf(columnHeader, "SA_LastName")];
                    resIncome.SA_PublicID = data[Array.IndexOf(columnHeader, "SA_PublicID")];
                    resIncome.SA_StateLicenseNumber =convert2Double( data[Array.IndexOf(columnHeader, "SA_StateLicenseNumber")]);
                    resIncome.SellingPrice = convert2Double(data[Array.IndexOf(columnHeader, "SellingPrice")]);
                    resIncome.TotalExpenses = convert2Double(data[Array.IndexOf(columnHeader, "TotalExpenses")]);


                    string address = string.Format("{0} {1} {2} {3}, {4}",
                        resIncome.StreetNumber,
                        resIncome.StreetName,
                        CityMap.ContainsKey(resIncome.City)?CityMap[resIncome.City]:resIncome.City,
                        resIncome.State,
                        resIncome.PostalCode);

                    int tries =0;
                    bool resolved = false;
                    while (tries++ < 3)
                    {
                        dynamic result = GeoCode(address);
                        if (result.status == "OK")
                        {
                            resIncome.Latitude = result.results[0].geometry.location.lat;
                            resIncome.Longitude = result.results[0].geometry.location.lng;
                            resIncomes.Add(resIncome);
                            resolved = true;
                            break;
                        }else
                        {
                            System.Threading.Thread.Sleep(500);
                        }
                    }

                    if (!resolved)
                    {
                        Console.WriteLine("failed to convert" + resIncome.ListingKey);
                        writer.WriteLine(line);
                    }

                    if (resIncomes.Count >= 100)
                    {
                        var klmServiceClient = GetClient(@"http://kmlservice.azurewebsites.net/");
                        var status = klmServiceClient.PostAsJsonAsync<List<ResIncome>>(@"api/resincome/", resIncomes).Result.Content.ReadAsStringAsync().Result;
                        resIncomes.Clear();                        
                    }

                }


            }

            if (resIncomes.Count > 0)
            {
                var klmServiceClient = GetClient(@"http://kmlservice.azurewebsites.net/");

                var status = klmServiceClient.PostAsJsonAsync<List<ResIncome>>(@"api/resincome/", resIncomes).Result.Content.ReadAsStringAsync().Result;
            }

        }

        private static dynamic GeoCode(string address)
        {
            var client = GetClient(@"http://maps.googleapis.com");
            string path = @"maps/api/geocode/json?sensor=true&address=" + HttpUtility.UrlEncode(address);
            var content = client.GetAsync(path).Result.Content;
            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(content.ReadAsStringAsync().Result);
            return result;
        }
    }
}
