using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JAPI.Repo.Extensions
{
    /// <summary>
    /// Description Attribute will translate to the querystring Key passed to the request
    /// </summary>
    public enum RequestParamKey
    {
        [Description("limit")]
        Limit,
        [Description("showHiddenItems")]
        ShowHiddenItems,
        [Description("type")]
        ResourceType,
        [Description("folderURI")]
        FolderURI,
        [Description("q")]
        Query,
        [Description("accessType")]
        AccessType,
        [Description("recursive")]
        Recursive,
        [Description("dependsOn")]
        dependsOn,
        [Description("sortBy")]
        sortBy,
        [Description("offset")]
        offset,
        [Description("forceFullPage")]
        forceFullPage,
        [Description("forceTotalCount")]
        ForceTotalCount,
        [Description("includeParents")]
        IncludeParents,
        [Description("rootTenantId")]
        rootTenantId,
        [Description("maxdepth")]
        MaxDepth,
        [Description("j_username")]
        UserName,
        [Description("j_password")]
        Password,
    }

    public static class RequestParamExtensions
    {
        public static Dictionary<RequestParamKey, string> GetBaseParams()
            => new Dictionary<RequestParamKey, string>();


        public static string GetEnumDescriptor<T>(this T rpKey) where T : IConvertible
            => rpKey.GetType().GetMember(rpKey.ToString()).FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()?.Description
               ?? rpKey.ToString();
    }
}
