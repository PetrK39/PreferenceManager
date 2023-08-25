using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace PreferenceManagerLibrary.PreferenceStorage
{
    /// <summary>
    /// IPreferenceStorage implementation to store preferences in XML format
    /// </summary>
    public class XMLPreferenceStorage : IPreferenceStorage
    {
        [Serializable]
        [XmlType(TypeName = "Preference")]
        public struct SerializableKeyValue<K, V>
        {
            [XmlAttribute]
            public K Key { get; set; }
            [XmlAttribute]
            public V Value { get; set; }
            public SerializableKeyValue(K key, V value)
            {
                Key = key;
                Value = value;
            }
        }


        public event EventHandler<Exception> OnError;
        private readonly FileInfo file;

        public XMLPreferenceStorage(FileInfo file)
        {
            this.file = file;
        }
        public XMLPreferenceStorage(string filePath) : this(new FileInfo(filePath)) { }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <returns>Enumerable of KeyValuePairs with (preference key, preference value)</returns>
        public IEnumerable<KeyValuePair<string, string>> LoadPreferences()
        {
            if (!file.Exists)
            {
                OnError?.Invoke(this, new FileNotFoundException("Config file not found, loading defaults", file.FullName));
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            var xml = new XmlSerializer(typeof(List<SerializableKeyValue<string, string>>));

            using (var fileReader = file.OpenRead())
            {
                return (xml.Deserialize(fileReader) as List<SerializableKeyValue<string, string>>).Select(skv => new KeyValuePair<string, string>(skv.Key, skv.Value));
            }
        }
        public void SavePreferences(IEnumerable<KeyValuePair<string, string>> values)
        {
            file.Directory.Create();

            var serializable = values.Select(kv => new SerializableKeyValue<string, string>(kv.Key, kv.Value));

            var xml = new XmlSerializer(typeof(List<SerializableKeyValue<string, string>>));

            using (var fileWriter = file.CreateText())
            using (var writer = XmlWriter.Create(fileWriter, new XmlWriterSettings { Indent = true }))
            {
                xml.Serialize(writer, serializable.ToList());
            }
        }
    }
}