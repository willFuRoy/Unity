using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder
{
    const string jsonPath = "Assets/_ProjConfig/assetbundle_cfg.json";

    /// <summary>
    /// 清除所有Assetbundle名称
    /// </summary>
    static void ClearAssetBundleNames()
    {
        var names = AssetDatabase.GetAllAssetBundleNames();

        for (int i = 0, max = names.Length; i < max; ++i)
        {
            AssetDatabase.RemoveAssetBundleName(names[i], true);
        }
        Debug.LogError("清除完成");
    }

    /// <summary>
    /// 计算资源主路径
    /// </summary>
    /// <param name="path"></param>
    static void CalcMainPathAsset(string path)
    {

    }
}
