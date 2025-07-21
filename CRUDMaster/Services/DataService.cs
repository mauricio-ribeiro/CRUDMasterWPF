using Newtonsoft.Json;
using System.IO;

namespace CRUDMaster.Services
{
    public class DataService
    {
        private readonly string _dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");

        public DataService()
        {
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }
        }

        public void Salvar<T>(string fileName, List<T> dados)
        {
            var filePath = Path.Combine(_dataDirectory, fileName);
            var json = JsonConvert.SerializeObject(dados, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public List<T> Carregar<T>(string fileName)
        {
            var filePath = Path.Combine(_dataDirectory, fileName);
            if (!File.Exists(filePath)) return null;

            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<T>>(json);
        }
    }
}