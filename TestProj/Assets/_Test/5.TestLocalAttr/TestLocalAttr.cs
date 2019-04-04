using UnityEngine;
using System.Collections;

public class TestLocalAttr : MonoBehaviour {

    private void Awake()
    {
        Debug.Log("transform.localPosition:" + transform.localPosition);
    }

    [ContextMenu("GetLocalAttr")]
    public void GetLocalAttr()
    {
        Debug.Log("transform.localPosition:" + transform.localPosition);
        Debug.Log("transform.localRotation:" + transform.localRotation);
        Debug.Log("transform.localScale:" + transform.localScale);
    }
}
