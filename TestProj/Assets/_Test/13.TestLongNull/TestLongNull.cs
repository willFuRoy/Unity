using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLongNull : MonoBehaviour {
    
    public static long a { set; get; }

	void Start () {
        a = -1;
        Debug.LogError(TestLongNull2.a.ToString());
        Debug.LogError(a.ToString());
    }

}
