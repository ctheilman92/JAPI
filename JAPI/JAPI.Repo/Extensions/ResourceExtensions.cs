using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAPI.Repo.Extensions
{
    public static class ResourceExtensions
    {
        public static T ConvertBaseResourceType<T>(this Resource resource) where T : Resource
        {
            var serializeParent = JsonConvert.SerializeObject(resource);
            return JsonConvert.DeserializeObject<T>(serializeParent);
        }

        public static ResourceLookup<T> ConvertResourceLookupBaseType<T>(this ResourceLookup<Resource> resourceLookup) where T : Resource
        {
            var serializeParent = JsonConvert.SerializeObject(resourceLookup);
            return JsonConvert.DeserializeObject<ResourceLookup<T>>(serializeParent);
        }
    }
}
