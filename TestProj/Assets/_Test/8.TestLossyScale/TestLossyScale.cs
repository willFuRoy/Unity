using UnityEngine;
using System.Collections;

public class TestLossyScale : MonoBehaviour {

    [ContextMenu("ShowLossyScale")]
    void ShowLossyScale()
    {
        Transform[] tss = GetComponentsInChildren<Transform>();
        for (int i = 0, maxi = tss.Length; i < maxi; ++i)
        {
            Debug.Log(tss[i].name + " :: " + tss[i].lossyScale);
        }
    }
}
