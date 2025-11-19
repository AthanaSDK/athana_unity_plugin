using Athana.Api;
using System.Collections.Generic;
using System.Text;

public static class AthanaUtils
{
    public static string preloadAds2String(Dictionary<AthanaInterface.AdType, string> dictionary)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        foreach (KeyValuePair<AthanaInterface.AdType, string> entry in dictionary)
        {
            sb.Append($"\"{entry.Key}\":\"{entry.Value}\", ");
        }
        if (dictionary.Count > 0)
        {
            sb.Length -= 2;
        }
        sb.Append("}");
        return sb.ToString();
    }
}