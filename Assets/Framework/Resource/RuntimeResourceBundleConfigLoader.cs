using UnityEngine;
using System.Collections.Generic;

public static class RuntimeResourceBundleConfigLoader
{
    public static List<KeyValuePair<string, int>> LoadResourceBundleConfig(string configName)
    {
        var r = new List<KeyValuePair<string, int>>();

        string path = Application.streamingAssetsPath + "/" + configName.ToLower() + ".txt";
        System.IO.StreamReader s = new System.IO.StreamReader(path);

        while(!s.EndOfStream)
        {
            var line = s.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var k = line.Split('\t');
            if (k.Length != 2)
                break;

            var p = new KeyValuePair<string, int>(k[0], int.Parse(k[1]));
            r.Add(p);
        }

        s.Close();
        s.Dispose();
        return r;
    }
}