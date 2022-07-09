using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources.NetStandard;

namespace ResourceLocalizer
{
    public class ResourceManagement : IDisposable
    {
        private string _resxFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ResourceFiles", "Language.resx");
        private readonly string _resxFileBase = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ResourceFiles", "Language.resx");

        private bool _disposed = false;

        /// <summary>
        /// Creating resource (.resx) file on given directory.
        /// </summary>
        private void CreateResxFile()
        {
            var resourceWriter = new ResXResourceWriter(_resxFile);
            resourceWriter.Generate();
            resourceWriter.Close();
        }

        /// <summary>
        /// Gets resource key from given culture's resource file.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public string GetString(string key, CultureInfo culture)
        {
            ResxFileNameReplacer(culture.TwoLetterISOLanguageName);

            using var reader = new ResXResourceReader(_resxFile);
            var val = reader.Cast<DictionaryEntry>().ToList().FirstOrDefault(x => x.Key.ToString() == key).Value;

            return val?.ToString() ?? null;
        }

        /// <summary>
        /// Get resources key - values of given culture's resource file.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public List<DictionaryEntry> GetResources(CultureInfo culture)
        {
            ResxFileNameReplacer(culture.TwoLetterISOLanguageName);

            using var reader = new ResXResourceReader(_resxFile);
            var resx = reader.Cast<DictionaryEntry>().ToList();

            return resx;
        }

        /// <summary>
        /// Adding or updating resource file for given key and culture.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="culture"></param>
        public void AddOrUpdateResource(string key, string value, CultureInfo culture)
        {
            List<DictionaryEntry> resx;
            ResxFileNameReplacer(culture.TwoLetterISOLanguageName);

            using (var reader = new ResXResourceReader(_resxFile))
            {
                resx = reader.Cast<DictionaryEntry>().ToList();
                if (!resx.Any(x => x.Key.ToString().Equals(key)))
                    resx.Add(new DictionaryEntry(key, value));
                else
                {
                    var existingResource = resx.FirstOrDefault(x => x.Key.ToString().Equals(key));
                    var modifiedResx = new DictionaryEntry(key, value);
                    resx.Remove(existingResource);
                    resx.Add(modifiedResx);
                }
            }

            UpdateResourceFile(resx);
        }

        /// <summary>
        /// Removing resource key for given culture.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="culture"></param>
        public void RemoveResource(string key, CultureInfo culture)
        {
            List<DictionaryEntry> resx;
            ResxFileNameReplacer(culture.TwoLetterISOLanguageName);

            using (var reader = new ResXResourceReader(_resxFile))
            {
                resx = reader.Cast<DictionaryEntry>().ToList();
                if (!resx.Any(x => x.Key.ToString().Equals(key)))
                    return;

                var existingResource = resx.FirstOrDefault(x => x.Key.ToString().Equals(key));
                resx.Remove(existingResource);
            }

            UpdateResourceFile(resx);
        }

        /// <summary>
        /// Removing keys from resource files.
        /// You can add other cultures to remove too.
        /// </summary>
        /// <param name="key"></param>
        public void RemoveResource(string key)
        {
            RemoveResource(key, CultureInfo.CreateSpecificCulture("tr-TR"));
            RemoveResource(key, CultureInfo.CreateSpecificCulture("en-US"));
        }

        /// <summary>
        /// Updates resource file.
        /// </summary>
        /// <param name="dicEntries"></param>
        private void UpdateResourceFile(List<DictionaryEntry> dicEntries)
        {
            using var resourceWriter = new ResXResourceWriter(_resxFile);
            dicEntries.ForEach(r => resourceWriter.AddResource(r.Key.ToString(), r.Value.ToString()));

            resourceWriter.Generate();
            resourceWriter.Close();
        }

        /// <summary>
        /// Check and replace resx file with given language.
        /// Craete if not exists
        /// </summary>
        /// <param name="lang"></param>
        private void ResxFileNameReplacer(string lang)
        {
            _resxFile = !lang.Equals("tr") 
                ? _resxFileBase.Replace(_resxFileBase.Split(".").Last(), lang + ".resx") 
                : _resxFileBase;

            if (!File.Exists(_resxFile)) CreateResxFile();
        }
        
        /// <summary>
        /// Disposing object and garbage collector finalizing.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
                _disposed = true;
        }
    }
}