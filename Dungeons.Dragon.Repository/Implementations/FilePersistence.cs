using Dungens.Dragon.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection; // Fixes CS0103: Adds the required using directive for Newtonsoft.Json

namespace Dungens.Dragon.Repository.Implementations
{
    public class FilePersistence : IPersistence
    {
        private readonly ILogger<FilePersistence> _logger;
        private string baseFilePath;
        public FilePersistence(ILogger<FilePersistence> logger)
        {
            _logger = logger;
            baseFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameData");

            if (!Directory.Exists(baseFilePath))
            {
                Directory.CreateDirectory(baseFilePath);
                _logger.LogInformation("Created GameData folder at: {Path}", baseFilePath);
            }
        }
        public bool SaveData(string data, string filePath)
        {
            try
            {
                _logger.LogInformation($"Saving data to {filePath}");
                var directoryPath = Path.Combine(baseFilePath, filePath);

                File.WriteAllText(directoryPath, data);
                _logger.LogInformation($"Data saved successfully to {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }
        public T RetrieveData<T>(string filePath)
        {
            try
            {
                _logger.LogInformation($"Retrieving data from {filePath}");
                var directoryPath = Path.Combine(baseFilePath, filePath);
                if (!File.Exists(directoryPath))
                {
                    _logger.LogWarning($"File not found at path: {directoryPath}");
                    throw new FileNotFoundException($"The file at path {filePath} was not found.");
                }
                var data = File.ReadAllText(directoryPath);
                _logger.LogInformation($"Data retrieved successfully from {filePath}");
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().Name);
                throw;
            }

        }

        public T? RetrieveDataById<T>(string filePath, string id)
        {
            try
            {
                _logger.LogInformation($"Retrieving data by ID from {filePath}");
                var directoryPath = Path.Combine(baseFilePath, filePath);
                if (!File.Exists(directoryPath))
                {
                    _logger.LogWarning($"File not found at path: {directoryPath}");
                    throw new FileNotFoundException($"The file at path {directoryPath} was not found.");
                }

                var data = File.ReadAllText(directoryPath);
                var token = JToken.Parse(data);
                _logger.LogInformation($"Data parsed successfully from {filePath}");
                List<JObject> list;
                if (token != null && token is JArray jArray)
                {
                    _logger.LogInformation($"JSON array found in {filePath}");
                    list = jArray?.Select(x => x as JObject).Where(x => x != null).ToList();
                }
                else if (token is JObject jObject)
                {
                    _logger.LogInformation($"Single JSON object found in {filePath}");
                    list = new List<JObject> { jObject };
                }
                else
                {
                    _logger.LogError("Invalid JSON format.");
                    throw new JsonException("Invalid JSON format.");
                }

                var match = list?.FirstOrDefault(item => item["Id"] != null && item["Id"].ToString() == id);
                _logger.LogInformation($"Data retrieval by ID completed from {filePath}");
                return match != null ? match.ToObject<T>() : default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }
        public bool RemoveDataById<T>(string filePath, string id)
        {
            try
            {
                _logger.LogInformation($"Removing data by ID from {filePath}");
                var directoryPath = Path.Combine(baseFilePath, filePath);
                if (!File.Exists(directoryPath))
                {
                    _logger.LogWarning($"File not found at path: {directoryPath}");
                    throw new FileNotFoundException($"The file at path {directoryPath} was not found.");
                }
                var data = File.ReadAllText(directoryPath);
                var token = JToken.Parse(data);
                _logger.LogInformation($"Data parsed successfully from {filePath}");
                List<JObject> list;
                if (token != null && token is JArray jArray)
                {
                    _logger.LogInformation($"JSON array found in {filePath}");
                    list = jArray.Select(x => x as JObject).Where(x => x != null).ToList();
                }
                else if (token is JObject jObject)
                {
                    _logger.LogInformation($"Single JSON object found in {filePath}");
                    list = new List<JObject> { jObject };
                }
                else
                {
                    _logger.LogError("Invalid JSON format.");
                    throw new JsonException("Invalid JSON format.");
                }

                var match = list.FirstOrDefault(item => item["Id"] != null && item["Id"].ToString() == id);
                if (match != null)
                {
                    _logger.LogInformation($"Match found. Removing data with ID: {id} from {filePath}");
                    list.Remove(match);
                    var jsonToSave = list.Count > 0 ? JArray.FromObject(list).ToString() : "[]";
                    File.WriteAllText(directoryPath, jsonToSave);
                    _logger.LogInformation($"Data with ID: {id} removed successfully from {filePath}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

    }
}
