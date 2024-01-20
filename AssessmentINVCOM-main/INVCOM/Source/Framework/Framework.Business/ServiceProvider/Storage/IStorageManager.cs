using CsvHelper.Configuration;
using Framework.Business.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Business.ServiceProvider.Storage
{
    public interface IStorageManager<TConfig> : IManagerBase
    {
        Task<IEnumerable<string>> GetFiles(TConfig config);
        //Task<IEnumerable<T>> ReadFile<T>(TConfig config, string key, CsvConfiguration csvConfiguration);
        //Task<IEnumerable<T>> ReadFiles<T>(TConfig config, CsvConfiguration csvConfiguration);
        Task<Dictionary<string, string>> ReadFile(TConfig config, string fileName);
        Task<Dictionary<string, string>> ReadFiles(TConfig config);
        Task<bool> UploadFileAsync(TConfig config, string fileName, Stream fileConten);
    }
}
