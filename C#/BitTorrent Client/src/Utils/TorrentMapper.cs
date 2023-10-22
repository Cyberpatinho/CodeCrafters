using codecrafters_bittorrent.src.Attributes;
using codecrafters_bittorrent.src.Objects;
using codecrafters_bittorrent.src.Utils;
using System.Reflection;

namespace codecrafters_bittorrent.Utils
{
    public class TorrentMapper
    {
        public static T Map<T>(Dictionary<string, DecodedObject> data) where T : new()
        {
            T result = new();
            Type type = typeof(T);
            
            foreach (var propertyInfo in type.GetProperties())
            {
                var nameAttribute = propertyInfo.GetCustomAttribute<NameAttribute>();
                if (nameAttribute?.Name != null && data.TryGetValue(nameAttribute.Name, out var decodedObject))
                {
                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        string stringValue = Helper.ToUTF8((byte[])decodedObject.Value);
                        propertyInfo.SetValue(result, stringValue);   
                    }
                    else if (propertyInfo.PropertyType == typeof(long))
                    {
                        if (decodedObject.Value is long longValue)
                            propertyInfo.SetValue(result, longValue);
                    }
                    else if (propertyInfo.PropertyType == typeof(Torrent.TorrentInfo))
                    {
                        if (decodedObject.Value is Dictionary<string, DecodedObject> subDict)
                            propertyInfo.SetValue(result, Map<Torrent.TorrentInfo>(subDict));
                        
                    }

                }

            }

            return result;
        }

    }
}
