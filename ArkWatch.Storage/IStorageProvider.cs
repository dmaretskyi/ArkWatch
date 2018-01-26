namespace ArkWatch.Storage
{
    public interface IStorageProvider
    {
        StorageData LoadData();

        void SaveData(StorageData data);
    }
}