namespace GasStationBot.Domain.Entities
{
    public enum UserState
    {
        None, //Base Menu
        StartMenu, //Start 
        About, //About 
        ShowGasStation,

        //Add GasStation
        AddGasStation_SelectProvider,
        AddGasStation_SelectCity,
        AddGasStation_SelectGasStations,
        AddGasStation_SelectFuel,
        AddGasStation_ConfirmGasStation,

        //Remove GasStation
        RemoveGasStation_ListGasStations,
        RemoveGasStation_RemoveGasStations,
    }
}
