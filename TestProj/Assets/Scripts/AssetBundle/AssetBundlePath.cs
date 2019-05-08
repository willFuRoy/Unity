using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundlePath
{
    static AssetBundlePath mInstance = null;

    public static AssetBundlePath instance
    {
        get
        {
            if (mInstance == null)
                mInstance = new AssetBundlePath();
            return mInstance;
        }
    }

    string androidLoadStreamingPath = string.Concat(Application.dataPath, "!assets/AssetsResources/");
    string otherLoadStreamingPath = string.Concat(Application.streamingAssetsPath, "/AssetsResources/");
    /// <summary>
    /// LoadFromFile使用的streaming地址
    /// </summary>
    public string loadStreamingPath
    {
        get
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return androidLoadStreamingPath;//注意，只有安卓的路径这么奇怪，在datapath和！assets之间没有“/”
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return otherLoadStreamingPath;
                default:
                    return otherLoadStreamingPath;
            }
        }
    }

    string androidwwwStreamingPath = string.Concat(Application.streamingAssetsPath, "/");
    string otherwwwStreamingPath = string.Concat("File://", Application.streamingAssetsPath, "/");
    /// <summary>
    /// www请求的Streaming路径
    /// </summary>
    public string wwwStreamingPath
    {
        get
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return androidwwwStreamingPath;
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return otherwwwStreamingPath;
                default:
                    return otherwwwStreamingPath;
            }
        }
    }

    string phoneSandPath = string.Concat(Application.persistentDataPath, "/gowsand/");
    string windowSandPath = string.Concat(Application.dataPath, "/../StreamingAssets/");
    /// <summary>
    /// 沙盒路径
    /// </summary>
    public string sandPath
    {
        get
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                    return phoneSandPath;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return windowSandPath;
                default:
                    return windowSandPath;
            }
        }
    }
}
