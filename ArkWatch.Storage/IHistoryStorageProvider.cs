namespace ArkWatch.Storage
{
    public interface IHistoryStorageProvider
    {
        HistoryDataCollection LoadData();

        void SaveData(HistoryDataCollection data);
    }
}