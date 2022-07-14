﻿using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;
using GasStationBot.Domain.Extensions;
using GasStationBot.TelegramBot.Commands;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace GasStationBot.TelegramBot.CommandHandlers
{
    public class AddGasStationSelectFuelCommandHandler : BaseTelegramCommandHandler<AddGasStationSelectFuelCommand>
    {

        private readonly ITelegramUserStateService _userStateService;

        public AddGasStationSelectFuelCommandHandler(ITelegramBotClient botClient,
                                                     ITelegramUserStateService userStateService) : base(botClient)
        {
            _userStateService = userStateService;
        }

        protected override string Message => GetCustomMessage();

        protected override IReplyMarkup Keyboard => GetCustomKeyboard();

        protected override async Task<UserState> HandleCommand()
        {
            if (CheckBackCommand())
            {
                return UserState.None;
            }

            var tempData = _userStateService.GetUserTempData(Command.UserId);
            if (tempData.SelectedGasStation == null)
            {
                await SendMessage(Command.UserId, "Помилка! АЗС не знайдена, спробуйте обрати знов.");
                return UserState.AddGasStation_SelectGasStations;
            }

            if (Command.UserMessage == "Підтвердити" )
            {
                if(tempData.SelectedFuels != null && tempData.SelectedFuels.Any())
                {
                    return Command.NextState!.Value;
                }
                else
                {
                    await SendMessage(Command.UserId, "Помилка! Паливо не додано.");
                    return Command.UserState;
                }
            }

            if (Command.UserMessage == "Скинути обране")
            {
                if (tempData.SelectedFuels != null && tempData.SelectedFuels.Any())
                {
                    ClearSelectedFuel();
                    return Command.UserState;
                }
                else
                {
                    await SendMessage(Command.UserId, "Помилка! Паливо не додано.");
                    return Command.UserState;
                }
                
            }

            if (CheckNewFuel(out Fuel fuel))
            {
                if(tempData.SelectedFuels == null)
                {
                    tempData.SelectedFuels = new List<Fuel>();
                }

                tempData.SelectedFuels.Add(fuel);
                _userStateService.SetUserTempData(Command.UserId, tempData);
                return Command.UserState;
            }

            await SendMessage(Command.UserId, "Пальне не знайдено або вже додано, спробуйте ще.");
            await SendMessage(Command.UserId, Message, Keyboard);
            return Command.UserState;
        }

        private void ClearSelectedFuel()
        {
            throw new NotImplementedException();
        }

        private bool CheckNewFuel(out Fuel fuel)
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);
            var selectedFuel = tempData.SelectedGasStation.Fuels.SingleOrDefault(f => f.FuelType.GetDescription() == Command.UserMessage);
            if(selectedFuel == null)
            {
                fuel = null;
                return false;
            }

            fuel = selectedFuel;

            if (tempData.SelectedFuels == null)
            {
                return true;
            }

            return !tempData.SelectedFuels.Any(f => f.FuelType == selectedFuel.FuelType);
        }

        private string GetCustomMessage()
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);
            var sb = new StringBuilder();
            sb.AppendLine("Обране АЗС: ");
            sb.AppendLine($"Адреса: {tempData.SelectedGasStation.Address}");

            sb.AppendLine($"Доступне Пальне: ");
            foreach (var fuel in tempData.SelectedGasStation!.Fuels)
            {
                if (!tempData.SelectedFuels.Any(f => f.FuelType == fuel.FuelType))
                {
                    sb.Append($"{fuel.FuelType.GetDescription()}  ");
                    //availableFuel.Add(fuel);
                }
            }

            sb.AppendLine();
            sb.AppendLine($"Обране Пальне: ");
            foreach (var fuel in tempData.SelectedFuels)
            {
                sb.Append($"{fuel.FuelType.GetDescription()}  ");
            }

            sb.AppendLine();
            sb.AppendLine("Оберіть пальне (на клавіатурі), або натисніть \"Підтвердити\"");

            return sb.ToString();
        }

        private IReplyMarkup GetCustomKeyboard()
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);

            var keyboard = new List<KeyboardButton[]>();
            keyboard.Add(new KeyboardButton[] { "До головної" });

            if(tempData.SelectedFuels != null || tempData.SelectedFuels.Any())
            {
                keyboard.Add(new KeyboardButton[] { "Підтвердити", "Скинути обране" });
            }

            if (tempData.SelectedGasStation == null)
            {
                if(tempData.SelectedFuels == null)
                {
                    tempData.SelectedFuels = new List<Fuel>();
                }

                var availableFuel = new List<Fuel>();
                foreach (var fuel in tempData.SelectedGasStation!.Fuels)
                {
                    if(!tempData.SelectedFuels.Any(f => f.FuelType == fuel.FuelType))
                    {
                        availableFuel.Add(fuel);
                    }
                }

                for (int i = 0; i < availableFuel.Count(); i += 2)
                {
                    var subKeyboard = new KeyboardButton[2];
                    var subList = availableFuel.GetRange(i, Math.Min(2, availableFuel.Count() - i));
                    for (int j = 0; j < subList.Count; j++)
                    {
                        subKeyboard[j] = new KeyboardButton(subList[j].FuelType.GetDescription());
                    }

                    keyboard.Add(subKeyboard);
                }
            }
            //var gsService = _gasStationsServices.SingleOrDefault(u => u.GasStationName == tempData.ProviderName);
            //if (gsService != null)
            //{
            //    //TODO: Fix Issue with .Result
            //    var citiesTask = gsService.GetGasStationsWithoutAdditionalData();
            //    citiesTask.Wait();
            //    var cities = citiesTask.Result.Select(gs => gs.City).Distinct().ToList();

            //    for (int i = 0; i < cities.Count(); i += 3)
            //    {
            //        var subKeyboard = new KeyboardButton[3];
            //        var subList = cities.GetRange(i, Math.Min(3, cities.Count() - i));
            //        for (int j = 0; j < subList.Count; j++)
            //        {
            //            subKeyboard[j] = new KeyboardButton(subList[j]);
            //        }

            //        keyboard.Add(subKeyboard);
            //    }
            //}


            return new ReplyKeyboardMarkup(keyboard);
        }
    }
}