namespace Dungens.Dragon.Repository.Interfaces
{
    public interface IPersistence
    {
        public bool SaveData(string data, string filePath);
        public T RetrieveData<T>(string filePath);
        public T RetrieveDataById<T>(string filePath, string Id);
        bool RemoveDataById<T>(string filePath, string Id);
    }
}
