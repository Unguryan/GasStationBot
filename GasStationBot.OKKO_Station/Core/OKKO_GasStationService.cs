using GasStationBot.Application.Services;
using GasStationBot.Domain.Entities;
using GasStationBot.OKKO_Station.ResponseRequest_Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace GasStationBot.WOG_Station.Core
{
    public class OKKO_GasStationService : IGasStationsService
    {
        public string GasStationName => "OKKO";

        private const string Path = "OkkoStations.json";

        private object locker = new object();

        //Takes 90 seconds
        public async Task<IEnumerable<GasStation>> GetGasStations()
        {
            var httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("ua"));
            using (httpClient)
            {
                var response = await httpClient.GetAsync(OKKO_Constants.BaseUrl);
                var stationsString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseRequestBaseModel>(stationsString);

                if (result == null || result.Collection == null || !result.Collection.Any())
                {
                    return new List<GasStation>();
                }

                var res = new List<GasStation>();
                foreach (var gasStation in result.Collection)
                {
                    res.Add(ConvertTo(gasStation.Attributes));
                }

                //TODO: Store updated data in file
                SetDataToFile(res);
                return res;
            }
        }

        public async Task<IEnumerable<GasStation>> GetGasStationsWithoutAdditionalData()
        {
            //TODO: Add stored gasStation in memory, just hold in memory
            return GetDataFromFile();
        }

        private GasStation ConvertTo(ResponseRequestGasStationAttributes response)
        {
            return new GasStation()
            {
                Provider = "OKKO",
                Address = response.Adresa,
                City = response.Naselenyy_punkt,
                Fuels = GetFuels(response.Notification)
            };
        }

        private List<Fuel> GetFuels(string response)
        {
            var fuels = GetDefaultFuel();
            if (string.IsNullOrEmpty(response))
            {
                foreach (var fuel in fuels)
                {
                    if (!fuel.StateOfFuel.Any())
                    {
                        fuel.StateOfFuel.Add(FuelState.NotExist);
                    }
                }
                return fuels;
            }

            var removedTagsStr = Regex.Replace(response, "<.*?>|&.*?;", string.Empty);

            var list = new List<string>();
            if (removedTagsStr.Contains("Diesel") || removedTagsStr.Contains("ДП"))
            {
                list.Add(removedTagsStr);
            }

            if(removedTagsStr.Contains("За готівку і банківські картки доступно*:"))
            {
                var tempStr = removedTagsStr.Split("За готівку і банківські картки доступно*:");
                var splitText = tempStr[1].Split("З паливною карткою і талонами доступно:");
                ParseFuels(fuels, splitText[0], splitText[1]);
                return fuels;
            }
            
            if(removedTagsStr.Contains("З паливною карткою і талонами доступно:"))
            {
                var splitText = removedTagsStr.Split("З паливною карткою і талонами доступно:");
                ParseFuels(fuels, string.Empty, splitText[1]);
                return fuels;
            }

            foreach (var fuel in fuels)
            {
                if (!fuel.StateOfFuel.Any())
                {
                    fuel.StateOfFuel.Add(FuelState.NotExist);
                }
            }
            return fuels;
        }

        private void ParseFuels(List<Fuel> fuels, string existFuel, string talonsFuel)
        {
            //Check Exist
            FillFuel(existFuel, fuels, FuelState.Exist);
            //if (!string.IsNullOrEmpty(existFuel))
            //{
            //    foreach (var type in OKKO_Constants.FuelTypes)
            //    {
            //        if (existFuel.Contains(type.Key))
            //        {
            //            var tempFuel = fuels.FirstOrDefault(f => f.FuelType == type.Value);
            //            if (tempFuel != null)
            //            {
            //                if(!tempFuel.StateOfFuel.Any(sf => sf.Equals(FuelState.Exist)))
            //                {
            //                    tempFuel.StateOfFuel.Add(FuelState.Exist);
            //                }
            //            }
            //        }
            //    }
            //}

            //Check Talons
            FillFuel(talonsFuel, fuels, FuelState.Talons);

            //Other fill like NonExist
            foreach (var fuel in fuels)
            {
                if (!fuel.StateOfFuel.Any())
                {
                    fuel.StateOfFuel.Add(FuelState.NotExist);
                }
            }
        }

        private void FillFuel(string fuelStr, List<Fuel> fuels, FuelState fuelState)
        {
            if (!string.IsNullOrEmpty(fuelStr))
            {
                foreach (var type in OKKO_Constants.FuelTypes)
                {
                    if (fuelStr.Contains(type.Key))
                    {
                        var tempFuel = fuels.FirstOrDefault(f => f.FuelType == type.Value);
                        if (tempFuel != null)
                        {
                            if (!tempFuel.StateOfFuel.Any(sf => sf.Equals(fuelState)))
                            {
                                tempFuel.StateOfFuel.Add(fuelState);
                            }
                        }
                    }
                }
            }
        }

        //private FuelType GetFuelType(string name)
        //{
        //    //TODO: Update this, possible null error
        //    return OKKO_Constants.FuelTypes.FirstOrDefault(f => f.Key == name).Value;
        //}

        //private string GetFuelName(string fuelString)
        //{
        //    if (string.IsNullOrEmpty(fuelString))
        //    {
        //        return string.Empty;
        //    }

        //    return OKKO_Constants.Fuels.FirstOrDefault(s => fuelString.Contains(s)) ?? string.Empty;
        //}

        //private List<FuelState> GetFuelState(string fuelString)
        //{
        //    if (string.IsNullOrEmpty(fuelString))
        //    {
        //        return new List<FuelState>() { FuelState.NotExist };
        //    }

        //    var result = new List<FuelState>();
        //    foreach (var state in OKKO_Constants.FuelStates)
        //    {
        //        if (fuelString.Contains(state.Key))
        //        {
        //            result.Add(state.Value);
        //        }
        //    }

        //    return result;
        //}

        private void SetDataToFile(IEnumerable<GasStation> res)
        {
            //TODO: update locker
            lock (locker)
            {
                if (!File.Exists(Path))
                {
                    File.Create(Path).Dispose();
                };

                var json = JsonConvert.SerializeObject(res);
                File.WriteAllText(Path, json);
            }
        }

        private IEnumerable<GasStation> GetDataFromFile()
        {
            lock (locker)
            {
                if (!File.Exists(Path))
                {
                    File.Create(Path).Dispose();
                };

                var json = File.ReadAllText(Path);
                return JsonConvert.DeserializeObject<IEnumerable<GasStation>>(json) ?? new List<GasStation>();
            }
        }

        private List<Fuel> GetDefaultFuel()
        {
            return new List<Fuel>()
            {
                new Fuel()
                {
                    Name = "М95",
                    FuelType = FuelType.Petrol_95_Plus,
                    StateOfFuel = new List<FuelState>()
                },
                new Fuel()
                {
                    Name = "А95",
                    FuelType = FuelType.Petrol_95,
                    StateOfFuel = new List<FuelState>()
                },
                new Fuel()
                {
                    Name = "МДП",
                    FuelType = FuelType.Diesel_Plus,
                    StateOfFuel = new List<FuelState>()
                },
                new Fuel()
                {
                    Name = "ДП",
                    FuelType = FuelType.Diesel,
                    StateOfFuel = new List<FuelState>()
                },
                new Fuel()
                {
                    Name = "ГАЗ",
                    FuelType = FuelType.Gas,
                    StateOfFuel = new List<FuelState>()
                }
            };
            //"М95",
            //"А95",
            //"МДП",
            //"ДП",
            //"ГАЗ"
        }

    }
}
