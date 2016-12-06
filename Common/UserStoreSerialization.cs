// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Xml;

namespace SEL
{
    public class UserStoreSerialization<T>
    {
        public static readonly IsolatedStorageFile _store = IsolatedStorageFile.GetUserStoreForApplication();
        private static readonly XmlWriterSettings _writerSettings = new XmlWriterSettings
                                                               {
                                                                   NamespaceHandling = NamespaceHandling.OmitDuplicates,
                                                                   Indent = true,
                                                                   CloseOutput = true,
                                                                   NewLineHandling = NewLineHandling.Replace, 
                                                                   NewLineChars = "\n"
                                                               };

        private readonly string _name;

        public UserStoreSerialization(string name)
        {
            _name = name;
        }

        public void Serialize(T obj)
        {
            Serialize(obj, _name);
        }

        public T Deserialize(bool deleteOnFail = false)
        {
            return Deserialize(_name, deleteOnFail);
        }

        /// <summary>
        /// Serialize state for persistence in isolated storage.
        /// </summary>
        public static void Serialize(T obj, string name)
        {
            var serializer = new DataContractSerializer(typeof (T));
            using (var xmlWriter = XmlWriter.Create(_store.OpenFile(name, FileMode.Create), _writerSettings))
            {
                serializer.WriteObject(xmlWriter, obj);
            }
        }

        /// <summary>
        /// Deserialize state from persistence in isolated storage.
        /// </summary>
        public static T Deserialize(string name, bool deleteOnFail = false)
        {
            var serializer = new DataContractSerializer(typeof (T));

            T deserialized = default(T);

            try
            {
                using (var stream = _store.OpenFile(name, FileMode.Open))
                {
                    deserialized = (T) serializer.ReadObject(stream);
                }
            }
            catch (IsolatedStorageException)
            {
                if (deleteOnFail)
                {
                    _store.DeleteFileIfExists(name);
                }
                // There is no file. Will start with a clean slate.
            }
            catch (SerializationException)
            {
                if (deleteOnFail)
                {
                    _store.DeleteFileIfExists(name);
                }
            }

            return deserialized;
        }

        public void Delete()
        {
            _store.DeleteFileIfExists(_name);
        }

        public bool Exists()
        {
            return _store.FileExists(_name);
        }
    }

    public static class UserStoreSerialization
    {
        private static readonly IsolatedStorageFile _store = IsolatedStorageFile.GetUserStoreForApplication();

        public static void Delete(string name)
        {
            _store.DeleteFileIfExists(name);
        }

        public static bool Exists(string name)
        {
            return _store.FileExists(name);
        }

        public static Stream GetStream(string name)
        {
            return _store.OpenFile(name, FileMode.Open);
        }
    }

    public static class IsolatedStorageFileExtensions
    {
        public static void DeleteFileIfExists(this IsolatedStorageFile @this, string name)
        {
            if (@this.FileExists(name))
            {
                @this.DeleteFile(name);
            }   
        }
    }
}