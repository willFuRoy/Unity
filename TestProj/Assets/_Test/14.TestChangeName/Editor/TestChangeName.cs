using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TestChangeName
{
    static string jsonPath = "Assets/AtlasOriginRes/AtlasReplace.json";
    static string originPath = "Assets/AtlasOriginRes/";
    static string[] path = { "Assets/ArtRes/Language/", "Assets/Prefab/AtlasPrefab/" };

    [MenuItem("NGUI/ChangeAtlasName")]
    public static void ChangeName()
    {
        TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonPath);
        Dictionary<string, object> json = new Dictionary<string, object>();
        if (ta != null)
        {
            json = MiniJSON.Json.Deserialize(ta.text) as Dictionary<string, object>;
        }
        else
        {
            Debug.LogError("缺少ReplaceJson");
        }
        Dictionary<string, Dictionary<string, string>> dicList = new Dictionary<string, Dictionary<string, string>>();
        for (int i = 0, maxi = path.Length; i < maxi; ++i)
        {
            if (!Directory.Exists(path[i]))
            {
                Debug.LogError(path[i] + " 路径问题");
                continue;
            }
            Dictionary<string, string> fileList = new Dictionary<string, string>();
            dicList.Add(path[i], fileList);
            DirectoryInfo dir = new DirectoryInfo(path[i]);
            FileInfo[] files = dir.GetFiles("*.prefab", SearchOption.AllDirectories);
            for (int j = 0, maxj = files.Length; j < maxj; ++j)
            {
                string basePath = "Assets" + files[j].FullName.Substring(Application.dataPath.Length);
                basePath = basePath.Replace('\\', '/');
                Object o = AssetDatabase.LoadMainAssetAtPath(basePath);
                if (o != null && PrefabUtility.GetPrefabType(o) == PrefabType.Prefab)
                {
                    Object t = (o as GameObject).GetComponent(typeof(UIAtlas));
                    if (t != null && !fileList.ContainsKey(o.name))
                    {
                        fileList.Add(o.name, files[j].FullName.Replace('\\', '/'));
                        fileList.Add(o.name + ".meta", files[j].FullName.Replace('\\', '/') + ".meta");
                    }
                }
            }
        }
        foreach (var item in dicList)
        {
            foreach (var item2 in item.Value)
            {
                string key = item2.Key.Contains(".") ? item2.Key.Substring(0, item2.Key.IndexOf('.')) : item2.Key;
                string newPath = item2.Value;
                if (json.ContainsKey(key))
                {
                    newPath = newPath.Replace(key, json[key].ToString());
                    ChangeFile(item2.Value, newPath, key, json[key].ToString());
                    File.Delete(item2.Value);
                }
            }
        }
        list.Clear();
        GetAllDirectory(originPath);
        for (int i = 0, maxi = list.Count; i < maxi; i++)
        {
            string ends = list[i].Substring(list[i].LastIndexOf('\\') + 1);
            if (json.ContainsKey(ends))
            {
                string newPath = list[i].Substring(0, list[i].LastIndexOf('\\') + 1) + json[ends].ToString();
                ChangeDiretory(list[i], newPath);
            }
        }
    }

    static void ChangeFile(string oldPath, string newPath, string oldName, string newName)
    {
        // 更新文件
        string[] lines = File.ReadAllLines(oldPath);

        using (StreamWriter sw = new StreamWriter(newPath, true, System.Text.Encoding.UTF8))
        {
            foreach (var l in lines)
            {
                if (l.Contains("  m_Name: " + oldName))
                {
                    sw.WriteLine("  m_Name: " + newName);
                }
                else
                {
                    sw.WriteLine(l);
                }
            }
        }
    }

    static List<string> list = new List<string>();

    static void GetAllDirectory(string dirs)
    {
        DirectoryInfo dir = new DirectoryInfo(dirs);
        DirectoryInfo[] fsinfos = dir.GetDirectories();
        foreach (DirectoryInfo fsinfo in fsinfos)
        {
            if (fsinfo is DirectoryInfo)
            {
                GetAllDirectory(fsinfo.FullName);
                list.Add(fsinfo.FullName);
            }
        }
    }

    static void ChangeDiretory(string srcFolderPath, string destFolderPath)
    {
        if (System.IO.Directory.Exists(srcFolderPath))
        {
            System.IO.Directory.Move(srcFolderPath, destFolderPath);
        }
    }
}
