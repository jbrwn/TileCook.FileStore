using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

public class FileTileInfoContractResolver : DefaultContractResolver
{
    private Dictionary<string, string> PropertyMappings;

    public FileTileInfoContractResolver()
    {
        PropertyMappings = new Dictionary<string, string> 
        {
            {"Name", "name"},
            {"Description", "description"},
            {"Scheme", "scheme"},
            {"Format", "format"},
            {"MinZoom", "minzoom"},
            {"MaxZoom", "maxzoom"},
            {"Bounds", "bounds"},
            {"VectorLayers", "vector_layers"},
            {"Fields","fields"},
            {"Center","center"}
        };
    }

    protected override string ResolvePropertyName(string propertyName)
    {
        string name = null;
        if (!PropertyMappings.TryGetValue(propertyName, out name))
        {
            name = base.ResolvePropertyName(propertyName);
        }
        return name;
    }
}