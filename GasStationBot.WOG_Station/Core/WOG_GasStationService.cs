﻿using GasStationBot.Application.Services;
using GasStationBot.Domain.Models.GasStation;
using GasStationBot.WOG_Station.ResponseRequest_Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace GasStationBot.WOG_Station.Core
{
    public class WOG_GasStationService : IGasStationsService
    {

        public async Task<IEnumerable<GasStation>> GetGasStations()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("ua"));
            using (httpClient)
            {
                var response = await httpClient.GetAsync(WOG_Constants.BaseUrl);
                var stationsString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseRequestBaseModel>(stationsString);

                if (result == null)
                {
                    return new List<GasStation>();
                }

                var res = new List<GasStation>();
                foreach (var item in result.Data.Stations)
                {
                    var responseStation = await httpClient.GetAsync(item.Link);
                    var stationString = await responseStation.Content.ReadAsStringAsync();
                    var station = JsonConvert.DeserializeObject<ResponseRequestGasStation>(stationString);
                    if (station != null)
                    {
                        res.Add(ConvertTo(station));
                    }
                }

                return res;
            }
        }

        private GasStation ConvertTo(ResponseRequestGasStation response)
        {
            return new GasStation()
            {
                Provider = "WOG",
                Address = response.Name,
                City = response.City,
                Fuels = GetFuels(response)
            };
        }

        private List<Fuel> GetFuels(ResponseRequestGasStation response)
        {
            var list = new List<Fuel>();

            if (response == null || string.IsNullOrEmpty(response.WorkDescription))
            {
                return list;
            }

            var splitFuel = response.WorkDescription.Split("\n");
            foreach (var fuelString in splitFuel)
            {
                var fuelName = GetFuelName(fuelString);
                if (string.IsNullOrEmpty(fuelName))
                {
                    continue;
                };

                var fuelState = GetFuelState(fuelString);

                list.Add(new Fuel()
                {
                    Name = fuelName,
                    StateOfFuel = fuelState
                });
            }

            return list;

            //АЗК працює згідно графіку.\n
            //М95 - Пальне відсутнє.\n
            //А95 - Тільки спецтранспорт.\n
            //ДП - Готівка, банк.картки. Гаманець ПРАЙД . Талони . Паливна картка (ліміт картки).\n
            //МДП+ - Пальне відсутнє.\n

            //АЗК працює зг?дно граф?ку.
            //М95 - Гот ? вка, банк.картки.Гаманець ПРАЙД . Талони.Паливна картка(л? м?т картки).
            //А95 - Гот ? вка, банк.картки.Гаманець ПРАЙД . Талони.Паливна картка(л? м?т картки).
            //ДП - Пальне в? дсутнє.
            //МДП + -Пальне в? дсутнє.
            //ГАЗ - Гот ? вка, банк.картки.Гаманець ПРАЙД . Талони.Паливна картка(л? м?т картки).
        }

        private string GetFuelName(string fuelString)
        {
            if (string.IsNullOrEmpty(fuelString))
            {
                return string.Empty;
            }

            return WOG_Constants.Fuels.FirstOrDefault(s => fuelString.Contains(s)) ?? string.Empty;
        }

        private List<FuelState> GetFuelState(string fuelString)
        {
            if (string.IsNullOrEmpty(fuelString))
            {
                return new List<FuelState>() { FuelState.NotExist };
            }

            var result = new List<FuelState>();
            foreach (var state in WOG_Constants.FuelStates)
            {
                if (fuelString.Contains(state.Key))
                {
                    result.Add(state.Value);
                }
            }

            return result;
        }
    }
}