using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AtlasInfo
{
    public bool select;
    public bool trimAlpha;
    public bool unityPacking;
    public bool forceSquareAtlas;
}

public class TestArtResTool
{

    #region 一键打包

    public class AtlasPackWin : EditorWindow
    {
        static bool beginPack;
        static int i = 0;
        static int j = 0;
        static UnityEngine.Object[] atlas;
        static Vector2 mScroll = Vector2.zero;
        static Dictionary<UnityEngine.Object, AtlasInfo> atlasInfoDic = new Dictionary<UnityEngine.Object, AtlasInfo>();

        [MenuItem("NGUI/OneKeyPack")]
        public static void Init()
        {
            AtlasPackWin myWindow = (AtlasPackWin)EditorWindow.GetWindow(typeof(AtlasPackWin), false, "AtlasPackWin", true);
            myWindow.Show(true);
            beginPack = false;
            i = 0;
            atlas = FindAllAtlas();
        }

        private void Update()
        {
            if (!beginPack) return;
            if (j >= 0)
            {
                j--; return;
            }
            if (atlas != null && atlas.Length > i)
            {
                UnityEngine.Object o = atlas[i];
                if (atlasInfoDic.ContainsKey(o))
                {
                    if(atlasInfoDic[o].select)
                    {
                        NGUISettings.unityPacking = atlasInfoDic[o].unityPacking;
                        NGUISettings.forceSquareAtlas = atlasInfoDic[o].forceSquareAtlas;
                        UIAtlasMaker.OneKeyPack(atlas[i]);
                        i++;
                        j = 20;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            else
            {
                Debug.LogError("一键打包图集完成");
                Close();
                beginPack = false;
                NGUISettings.unityPacking = true;
                NGUISettings.forceSquareAtlas = false;
                return;
            }
        }


        void OnGUI()
        {
            NGUIEditorTools.SetLabelWidth(80f);
            GUILayout.Label("图集一键打包", "LODLevelNotifyText");
            GUILayout.Space(6f);

            if (atlas == null || atlas.Length == 0)
            {
                GUILayout.Label("没有图集");
                GUILayout.BeginHorizontal();
                bool close = GUILayout.Button("关闭", "LargeButton", GUILayout.Width(120f));
                GUILayout.EndHorizontal();
                if (close) Close();
            }
            else
            {
                mScroll = GUILayout.BeginScrollView(mScroll);
                for (int i = 0, maxi = atlas.Length; i < maxi; i++)
                {
                    DrawObject(atlas[i], i);
                }
                GUILayout.EndScrollView();
                GUILayout.BeginHorizontal();
                GUI.backgroundColor = Color.white;
                bool selectAll = GUILayout.Button("全部选中", "LargeButton", GUILayout.Width(120f));
                if (selectAll) SetAllAtlaState(true);
                bool cancelAll = GUILayout.Button("全部取消", "LargeButton", GUILayout.Width(120f));
                if (cancelAll) SetAllAtlaState(false);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                bool goPack = GUILayout.Button("开始打包", "LargeButton", GUILayout.Width(500f));
                if (goPack) beginPack = true;
                GUILayout.EndHorizontal();
            }
        }

        static UnityEngine.Object[] FindAllAtlas(UnityEngine.Object[] select = null)
        {
            atlasInfoDic.Clear();
            DirectoryInfo dir = new DirectoryInfo("Assets/ArtRes/Language/");
            FileInfo[] files = dir.GetFiles("*.prefab", SearchOption.AllDirectories);
            List<UnityEngine.Object> atlas = new List<UnityEngine.Object>();
            for (int j = 0, maxj = files.Length; j < maxj; ++j)
            {
                string basePath = "Assets" + files[j].FullName.Substring(Application.dataPath.Length);
                basePath = basePath.Replace('\\', '/');
                UnityEngine.Object o = AssetDatabase.LoadMainAssetAtPath(basePath);
                if (o != null && PrefabUtility.GetPrefabType(o) == PrefabType.Prefab)
                {
                    UnityEngine.Object t = (o as GameObject).GetComponent(typeof(UIAtlas));
                    if (t != null && !atlas.Contains(t))
                    {
                        atlas.Add(t);
                        AtlasInfo ai = new AtlasInfo();
                        ai.select = false;
                        ai.trimAlpha = true;
                        ai.unityPacking = true;
                        ai.forceSquareAtlas = false;
                        atlasInfoDic.Add(t, ai);
                    }
                }
            }
            return atlas.ToArray();
        }

        static void SetAllAtlaState(bool state)
        {
            foreach (var item in atlasInfoDic)
            {
                item.Value.select = state;
            }
        }

        void DrawObject(UnityEngine.Object obj, int index)
        {
            if (obj == null || !atlasInfoDic.ContainsKey(obj)) return;
            Component comp = obj as Component;

            GUILayout.BeginHorizontal();
            {
                if(index % 2 == 0)
                    GUI.backgroundColor = Color.red;
                else
                    GUI.backgroundColor = Color.green;

                string path = AssetDatabase.GetAssetPath(obj);

                atlasInfoDic[obj].select = EditorGUILayout.Toggle("Select", atlasInfoDic[obj].select, GUILayout.Width(100f));
                if (string.IsNullOrEmpty(path))
                {
                    path = "[Embedded]";
                    GUI.contentColor = new Color(0.7f, 0.7f, 0.7f);
                }
                else if (comp != null && EditorUtility.IsPersistent(comp.gameObject))
                    GUI.contentColor = new Color(0.6f, 0.8f, 1f);

                GUILayout.Button(obj.name, "AS TextArea", GUILayout.Width(160f), GUILayout.Height(20f));
                GUILayout.Button(path.Replace("Assets/", ""), "AS TextArea", GUILayout.Height(20f));
                GUI.contentColor = Color.white;

                atlasInfoDic[obj].trimAlpha = EditorGUILayout.Toggle("Trim Alpha", atlasInfoDic[obj].trimAlpha, GUILayout.Width(100f));
                atlasInfoDic[obj].unityPacking = EditorGUILayout.Toggle("Unity Packer", atlasInfoDic[obj].unityPacking, GUILayout.Width(100f));
                if (!atlasInfoDic[obj].unityPacking)
                    atlasInfoDic[obj].forceSquareAtlas = EditorGUILayout.Toggle("Force Square", atlasInfoDic[obj].forceSquareAtlas, GUILayout.Width(100f));
            }
            GUILayout.EndHorizontal();
        }
    }
    #endregion
}
