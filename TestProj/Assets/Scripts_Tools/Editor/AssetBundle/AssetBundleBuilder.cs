using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder
{
    static string jsonPath = "Assets/_ProjConfig/assetbundle_cfg.json";
    static string buildOriginPath = Application.dataPath + "/../AssetsResources/";
    static string buildTargetPath = Application.streamingAssetsPath + "/AssetsResources/";

    static List<string> strMultUsed;                //使用过多次的依赖文件
    static List<string> strOnceUsed;                //使用过一次的依赖文件
    static Dictionary<string, int> strMultUsedDic;  
    static Dictionary<string, string> allAssets;    //所有需要设置名字的字典

    #region Main Method

    public static void BuileAssetBundle()
    {
#if UNITY_STANDALONE
        BuileWindowAssetBundle();
#elif UNITY_ANDROID
        BuileAndroidAssetBundle();
#elif UNITY_IPHONE || UNITY_IOS
        BuileIOSAssetBundle();
#endif
    }

    public static void SetTotalAssetBundleNames()
    {
        //清除所有bundleName;
        ClearAssetBundleNames();
        AssetDatabase.Refresh();

        string str;
        using (StreamReader jsonCfg = new StreamReader(jsonPath))
        {
            str = jsonCfg.ReadToEnd();
        }
        Dictionary<string, object> json = MiniJSON.Json.Deserialize(str) as Dictionary<string, object>;
        List<object> mainPath = json["mainPath"] as List<object>;
        Dictionary<string, object> lonelyExtPath = json["lonelyExtPath"] as Dictionary<string, object>;
        Dictionary<string, object> lonelyFilesPath = json["lonelyFilePath"] as Dictionary<string, object>;

        Dictionary<string, object> onePath = json["onePath"] as Dictionary<string, object>;
        Dictionary<string, object> clearPath = json["clearPath"] as Dictionary<string, object>;
        List<object> skipPath = json["skipPath"] as List<object>;
        strMultUsed = new List<string>();
        strOnceUsed = new List<string>();
        strMultUsedDic = new Dictionary<string, int>();
        allAssets = new Dictionary<string, string>();

        if (mainPath != null)
        {
            for (int i = 0, max = mainPath.Count; i < max; ++i)
            {
                GetMainBundleName(Application.dataPath + mainPath[i].ToString(), skipPath);
            }
        }
        //清除只使用了一次的依赖的bundle文件
        if (strOnceUsed != null)
        {
            for (int i = 0; i < strOnceUsed.Count; i++)
            {
                RemoveAsset(strOnceUsed[i]);
                EditorUtility.DisplayProgressBar("清理一次依赖文件", "清理一次依赖文件中....", 1f * i / strOnceUsed.Count);
            }
        }

        //获取所有需要单独打包文件下的后缀
        if (lonelyExtPath != null)
        {
            foreach (var item in lonelyExtPath)
            {
                string path = item.Key;
                List<object> extensions = item.Value as List<object>;
                GetLonelyExtBundlName(Application.dataPath + path, extensions, skipPath);
            }
        }

        //获取所有需要单独打包文件下的名字
        if (lonelyFilesPath != null)
        {
            foreach (var item in lonelyFilesPath)
            {
                string path = item.Key;
                List<object> files = item.Value as List<object>;
                GetLonelyFileBundlName(Application.dataPath + path, files);
            }
        }
    }
    #endregion

    #region Sub Method

    #if UNITY_STANDALONE
    /// <summary>
    /// 打包所有设置了 AssetBundle Name的资源
    /// </summary>
    [MenuItem("AssetBundle/Create Windows AssetBundles", false, 1)]
    [MenuItem("Assets/AssetBundle/Create Windows AssetBundles", false, 1)]
    static void BuileWindowAssetBundle()
    {
        string targetPath = Application.dataPath + "/../AssetsResources/";
        if (!Directory.Exists(targetPath))
            Directory.CreateDirectory(targetPath);

        BuildPipeline.BuildAssetBundles(targetPath, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows);

        AssetDatabase.Refresh();

        CopyFilesToStreaming();

        AssetDatabase.Refresh();

        EditorUtility.ClearProgressBar();

        UnityEngine.Debug.Log("Bundle Build Done");
    }
    #endif

    #if UNITY_ANDROID
    /// <summary>
    /// 打包所有设置了 AssetBundle Name的资源
    /// </summary>
    [MenuItem("AssetBundle/Create Android AssetBundles", false, 2)]
    [MenuItem("Assets/AssetBundle/Create Android AssetBundles", false, 2)]
    static void BuileAndroidAssetBundle()
    {
        string targetPath = Application.dataPath + "/../AssetsResources/";
        if (!Directory.Exists(targetPath))
            Directory.CreateDirectory(targetPath);

        BuildPipeline.BuildAssetBundles(targetPath, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android);
        
        DirectoryInfo directory = new DirectoryInfo(hfPath);
        if (directory.Exists)
        {
            AssetDatabase.Refresh();
            DeleteTxt(new DirectoryInfo(hfPath));
        }

        AssetDatabase.Refresh();
        
        CopyFilesToStreaming();

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();

        UnityEngine.Debug.Log("Bundle Build Done");
    }
    #endif

    #if UNITY_IPHONE || UNITY_IOS
    /// <summary>
    /// 打包所有设置了 AssetBundle Name的资源
    /// </summary>
    [MenuItem("AssetBundle/Create IOS AssetBundles", false, 3)]
    [MenuItem("Assets/AssetBundle/Create IOS AssetBundles", false, 3)]
    static void BuileIOSAssetBundle()
    {
        string targetPath = Application.dataPath + "/../AssetsResources/";
        if (!Directory.Exists(targetPath))
            Directory.CreateDirectory(targetPath);

        BuildPipeline.BuildAssetBundles(targetPath, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.iOS);

        DirectoryInfo directory = new DirectoryInfo(hfPath);
        if (directory.Exists)
        {
            AssetDatabase.Refresh();
            DeleteTxt(new DirectoryInfo(hfPath));
        }

        AssetDatabase.Refresh();

        CopyFilesToStreaming();

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();

        UnityEngine.Debug.Log("Bundle Build Done");
    }
    #endif

    [MenuItem("AssetBundle/Normal Method/Name Method/ClearAllBundleName")]
    static void ClearAssetBundleNames()
    {
        var names = AssetDatabase.GetAllAssetBundleNames();

        for (int i = 0, max = names.Length; i < max; ++i)
        {
            AssetDatabase.RemoveAssetBundleName(names[i], true);
        }
        Debug.LogError("清除完成");
    }
    static void GetMainBundleName(string path, List<object> skipPathList)
    {
        if (Directory.Exists(path))
        {
            EditorUtility.DisplayProgressBar("获取名称中", "获取名称中...", 0f);   //显示进程加载条
            DirectoryInfo dir = new DirectoryInfo(path);    //获取目录信息

            FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);  //获取所有的文件信息
            for (var i = 0; i < files.Length; ++i)
            {
                FileInfo fileInfo = files[i];

                if (i % 10 == 0)
                    EditorUtility.DisplayProgressBar("获取名称中", "获取名称中...", 1f * i / files.Length);

                //需要跳过打包的文件
                bool skip = false;
                foreach (var item in skipPathList)
                {
                    if (fileInfo.FullName.IndexOf(item.ToString()) != -1)
                    {
                        skip = true;
                        break;
                    }
                }
                if (skip)
                    continue;

                string basePath = "Assets" + fileInfo.FullName.Substring(Application.dataPath.Length);  //编辑器下路径Assets/..
                basePath = basePath.Replace('\\', '/');
                if (fileInfo.FullName.EndsWith(".mat"))
                {
                    if (CheckisStandard(basePath))
                        continue;
                }

                string assetName = fileInfo.FullName.Substring(Application.dataPath.Length + 1);  //预设的Assetbundle名字，带上一级目录名称
                assetName = assetName.Substring(0, assetName.LastIndexOf('.')); //名称要去除扩展名
                assetName = assetName.Replace('\\', '/');   //注意此处的斜线一定要改成反斜线，否则不能设置名称

                if (!allAssets.ContainsKey(basePath))
                {
                    AddAsset(basePath, assetName + ".ab");
                }

                switch (fileInfo.Name.Split('.')[1])
                {
                    case "prefab":
                    case "FBX":
                        {
                            //获得他们的所有依赖，不过AssetDatabase.GetDependencies返回的依赖是包含对象自己本身的
                            string[] dps = AssetDatabase.GetDependencies(basePath); //获取依赖的相对路径Assets/...
                            for (int j = 0; j < dps.Length; j++)
                            {
                                //要过滤掉依赖的自己本身和脚本文件，自己本身的名称已设置，而脚本不能打包,FBX文件不计算依赖
                                if (dps[j].Contains(fileInfo.FullName.Substring(path.Length).Replace('\\', '/')) || dps[j].Contains(".cs") || dps[j].EndsWith(".FBX"))
                                    continue;
                                else if (dps[j].EndsWith(".mat"))
                                {
                                    if (CheckisStandard(dps[j]))
                                        continue;
                                }
                                else
                                {
                                    //让已经使用过的依赖 单独打包
                                    if (!strMultUsed.Contains(dps[j]))
                                    {
                                        strMultUsed.Add(dps[j]);
                                        strOnceUsed.Add(dps[j]);

                                    }
                                    else
                                    {
                                        if (strOnceUsed.Contains(dps[j]))
                                            strOnceUsed.Remove(dps[j]);

                                        string ab = dps[j].Substring(dps[j].IndexOf('/') + 1);
                                        ab = ab.Substring(ab.IndexOf('/') + 1);
                                        ab = ab.Substring(0, ab.LastIndexOf('.'));

                                        AddAsset(dps[j], ab + ".ab");
                                    }
                                }
                            }
                            break;
                        }
                    case "mat":
                        {
                            string[] dps = AssetDatabase.GetDependencies(basePath);
                            for (int j = 0, maxj = dps.Length; j < maxj; ++j)
                            {
                                if (dps[j].EndsWith(".shader") || dps[j].EndsWith(".mat"))
                                    continue;

                                if (strMultUsedDic.ContainsKey(dps[j]))
                                {
                                    strMultUsedDic[dps[j]]++;
                                }
                                else
                                {
                                    strMultUsedDic.Add(dps[j], 1);
                                }
                            }
                            break;
                        }
                    default:
                        break;
                }
            }

            foreach (var item in strMultUsedDic)
            {
                if (item.Value == 1)
                    allAssets.Remove(item.Key);
            }

            EditorUtility.ClearProgressBar();
        }
    }
    static void GetLonelyExtBundlName(string path, List<object> extensionList, List<object> skipPathList)
    {
        if (!Directory.Exists(path))
        {
            UnityEngine.Debug.Log("不存在此目录：" + path);
            return;
        }

        if (Directory.Exists(path))
        {
            EditorUtility.DisplayProgressBar("获取名称中", "获取名称中....", 0.0f);

            DirectoryInfo dir = new DirectoryInfo(path);

            if (extensionList == null || extensionList.Count <= 0)
            {
                FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);

                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo fileInfo = files[i];
                    EditorUtility.DisplayProgressBar("获取名称中", "获取名称中....", 1f * i / files.Length);
                    bool skip = false;
                    foreach (var item in skipPathList)
                    {
                        if (fileInfo.FullName.IndexOf(item.ToString()) != -1)
                        {
                            skip = true;
                            break;
                        }
                    }
                    if (skip)
                        continue;
                    string basePath = "Assets" + fileInfo.FullName.Substring(Application.dataPath.Length);
                    basePath = basePath.Replace('\\', '/');
                    string assetName = fileInfo.FullName.Substring(Application.dataPath.Length + 1).Replace('\\', '/');
                    assetName = assetName.Substring(0, assetName.LastIndexOf('.'));
                    AddAsset(basePath, assetName + ".ab");
                }
            }
            else
            {
                foreach (var extension in extensionList)
                {
                    FileInfo[] files = dir.GetFiles("*" + extension.ToString(), SearchOption.AllDirectories);

                    for (int i = 0; i < files.Length; i++)
                    {
                        FileInfo fileInfo = files[i];
                        EditorUtility.DisplayProgressBar("获取名称中", "获取名称中....", 1f * i / files.Length);
                        bool skip = false;
                        foreach (var item in skipPathList)
                        {
                            if (fileInfo.FullName.IndexOf(item.ToString()) != -1)
                            {
                                skip = true;
                                break;
                            }
                        }
                        if (skip)
                            continue;
                        string basePath = "Assets" + fileInfo.FullName.Substring(Application.dataPath.Length);
                        basePath = basePath.Replace('\\', '/');
                        string assetName = fileInfo.FullName.Substring(Application.dataPath.Length + 1).Replace('\\', '/');
                        assetName = assetName.Substring(0, assetName.LastIndexOf('.'));
                        AddAsset(basePath, assetName + ".ab");
                    }
                }
            }
        }
        EditorUtility.ClearProgressBar();
    }
    static void GetLonelyFileBundlName(string path, List<object> fileList)
    {
        if (!Directory.Exists(path) || fileList == null)
        {
            UnityEngine.Debug.Log("不存在此目录：" + path);
            return;
        }

        if (Directory.Exists(path))
        {
            EditorUtility.DisplayProgressBar("获取名称中", "获取名称中....", 0.0f);
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] allFiles = dir.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < allFiles.Length; ++i)
            {
                FileInfo fileInfo = allFiles[i];
                EditorUtility.DisplayProgressBar("获取名称中", "获取名称中....", 1f * i / allFiles.Length);
                string basePath = "Assets" + fileInfo.FullName.Substring(Application.dataPath.Length);
                basePath = basePath.Replace('\\', '/');
                object o = fileList.Find(x => basePath.Contains(x.ToString()));
                if (o == null) continue;
                string assetName = fileInfo.FullName.Substring(Application.dataPath.Length + 1).Replace('\\', '/');
                assetName = assetName.Substring(0, assetName.LastIndexOf('.'));
                AddAsset(basePath, assetName + ".ab");
            }
        }
        EditorUtility.ClearProgressBar();
    }
    static void AddAsset(string path, string name)
    {
        if (!allAssets.ContainsKey(path))
            allAssets.Add(path, name);
        else
            allAssets[path] = name;
    }
    static void RemoveAsset(string path)
    {
        if (allAssets.ContainsKey(path))
            allAssets.Remove(path);
    }
    static bool CheckisStandard(string matpath)
    {
        matpath = matpath.Replace('\\', '/');
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matpath);
        if (mat == null)
        {
            UnityEngine.Debug.Log("错误mat文件：" + matpath);
            return true;
        }

        if (mat.shader.Equals(Shader.Find("Standard")))
            return true;

        return false;
    }
    static void ChangeLua2Txt(string path)
    {
        DirectoryInfo directory = new DirectoryInfo(path);
        foreach (var NextFile in directory.GetFiles("*.lua", SearchOption.AllDirectories))
        {
            var utf8WithoutBom = new System.Text.UTF8Encoding(false);
            StreamReader sr = new StreamReader(NextFile.FullName, utf8WithoutBom);
            string text = sr.ReadToEnd();
            FileStream fs = new FileStream(Path.ChangeExtension(NextFile.FullName, "txt"), FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs, utf8WithoutBom);
            sw.Write(text);

            sr.Close();

            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }    
    static void DeleteTxt(string path)
    {
        DirectoryInfo directory = new DirectoryInfo(path);
        foreach (var NextFile in directory.GetFiles("*.txt", SearchOption.AllDirectories))
        {
            NextFile.Delete();
        }
    }
    static void CopyFilesToStreaming()
    {
        EditorUtility.DisplayProgressBar("复制资源", "生成打包资源....", 0);
        if (Directory.Exists(buildTargetPath))
        {
            DirectoryInfo dic = new DirectoryInfo(buildTargetPath);
            DeleteFiles(dic);
        }
        else
            Directory.CreateDirectory(buildTargetPath);
        CopyDirectory(buildOriginPath, buildTargetPath);
    }
    static void DeleteFiles(DirectoryInfo fatherFolder)
    {
        FileInfo[] files = fatherFolder.GetFiles();
        foreach (FileInfo file in files)
        {
            try
            {
                File.Delete(file.FullName);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex);
            }
        }

        foreach (DirectoryInfo childFolder in fatherFolder.GetDirectories())
        {
            DeleteFiles(childFolder);
        }
    }
    static void CopyDirectory(string srcPath, string destPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)     //判断是否文件夹
                {
                    // SVN不能拷贝
                    if (i.Name == ".svn")
                    {
                        continue;
                    }

                    if (!Directory.Exists(destPath + "/" + i.Name))
                    {
                        Directory.CreateDirectory(destPath + "/" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                    }
                    CopyDirectory(i.FullName, destPath + "/" + i.Name);    //递归调用复制子文件夹
                }
                else
                {
                    if (!i.FullName.EndsWith(".manifest"))
                        File.Copy(i.FullName, destPath + "/" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                }
            }
        }
        catch
        {
            throw;
        }
    }
    static string SwitchPlatform()
    {
#if UNITY_ANDROID
        return "android";
#elif UNITY_IPHONE || UNITY_IOS
        return "iOS";
#elif UNITY_STANDALONE
        return "windows";
#else
        return "windows";
#endif
    }
    #endregion
}
