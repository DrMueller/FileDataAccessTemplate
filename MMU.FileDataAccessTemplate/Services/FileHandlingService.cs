using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace $safeprojectname$.Services
{
    public class FileHandlingService<TModel>
    {
        private readonly DirectoryInfo _directoryInfo;

        public FileHandlingService()
        {
            _directoryInfo = new DirectoryInfo(GetModelPath());
            if (!_directoryInfo.Exists)
            {
                _directoryInfo.Create();
            }
        }

        #region Public/Internal Methods

        internal void DeleteFile(string fileName)
        {
            var fullFileName = GetFullFileName(fileName);
            File.Delete(fullFileName);
        }

        internal bool FileExists(string fileName)
        {
            var fullFileName = GetFullFileName(fileName);
            return File.Exists(fullFileName);
        }

        internal IReadOnlyCollection<TModel> LoadAll()
        {
            var result = GetAllFileNames().Select(LoadFromFile).ToList();
            return result.AsReadOnly();
        }

        internal TModel LoadFromFile(string fileName)
        {
            var fullFileName = GetFullFileName(fileName);
            var str = File.ReadAllText(fullFileName);
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<TModel>(str);
            return result;
        }

        internal void SaveToFile(TModel model, string fileName)
        {
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            var fullFileName = GetFullFileName(fileName);
            File.WriteAllText(fullFileName, str);
        }

        #endregion

        #region Private Methods

        private IEnumerable<string> GetAllFileNames()
        {
            var allFiles = _directoryInfo.GetFiles();
            return allFiles.Select(f => f.Name).ToList();
        }

        private string GetFullFileName(string fileName)
        {
            var result = Path.Combine(_directoryInfo.FullName, fileName);
            return result;
        }

        private static string GetModelPath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string result = Uri.UnescapeDataString(uri.Path);
            result = Path.GetDirectoryName(result);
            result = Path.Combine(result, typeof(TModel).Name);
            return result;
        }

        #endregion
    }
}
