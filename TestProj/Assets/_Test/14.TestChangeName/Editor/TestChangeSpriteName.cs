using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TestChangeSpriteName
{
    static string nameDicPath = "Assets/Replace/";
    static string originPath = "Assets/AtlasOriginRes/";
    static string prefabPath = "Assets/Prefab/UIPrefab";

    static Dictionary<string, string> nameDic = new Dictionary<string, string>();

    [MenuItem("NGUI/ChangeSpriteName")]
    public static void ChangeSpriteName()
    {
        GetNameDic();
        ChangeOriginSpriteName();
        ChangePrefabSpriteName();
    }

    static void GetNameDic()
    {
        nameDic.Clear();
        if (!Directory.Exists(nameDicPath))
        {
            Debug.LogError(nameDicPath + " 路径问题");
            return;
        }
        DirectoryInfo dir = new DirectoryInfo(nameDicPath);
        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0, maxi = files.Length; i < maxi; ++i)
        {
            if (files[i].FullName.Contains(".meta")) continue;
            string name = files[i].Name.Substring(0, files[i].Name.IndexOf('.'));
            string[] a = name.Split('@');
            if (a.Length == 2)
            {
                nameDic.Add(a[1], a[0]);
            }
        }
    }

    static void ChangeOriginSpriteName()
    {
        if (!Directory.Exists(originPath))
        {
            Debug.LogError(originPath + " 路径问题");
            return;
        }
        DirectoryInfo dir = new DirectoryInfo(originPath);
        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0, maxi = files.Length; i < maxi; ++i)
        {
            string name = files[i].Name.Substring(0, files[i].Name.IndexOf('.'));
            if (nameDic.ContainsKey(name))
            {
                string newPath = files[i].FullName.Replace(name, nameDic[name]);
                if (File.Exists(files[i].FullName))
                {
                    File.Move(files[i].FullName, newPath);
                }
            }
        }
    }

    static void ChangePrefabSpriteName()
    {
        if (!Directory.Exists(prefabPath))
        {
            Debug.LogError(prefabPath + " 路径问题");
            return;
        }
        DirectoryInfo dir = new DirectoryInfo(prefabPath);
        FileInfo[] files = dir.GetFiles("*.prefab", SearchOption.AllDirectories);
        for (int i = 0, maxi = files.Length; i < maxi; ++i)
        {
            ChangeFile(files[i].FullName);
        }
    }

    static void ChangeFile(string oldPath)
    {
        // 更新文件
        string[] lines = File.ReadAllLines(oldPath);

        using (StreamWriter sw = new StreamWriter(oldPath))
        {
            foreach (var l in lines)
            {
                if (l.Contains("  mSpriteName: "))
                {
                    string oldName = l.Substring("  mSpriteName: ".Length);
                    if (nameDic.ContainsKey(oldName))
                    {
                        string newLine = l.Replace(oldName, nameDic[oldName]);
                        sw.WriteLine(newLine);
                    }
                    else
                    {
                        sw.WriteLine(l);
                    }
                }
                else
                {
                    sw.WriteLine(l);
                }
            }
        }
    }
}
