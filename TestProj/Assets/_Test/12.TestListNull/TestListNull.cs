using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestListNull : MonoBehaviour {

	// Use this for initialization
	void Start () {
        List<object> list = new List<object>();
        list.Add("a");
        list.Add("b");
        list.Add(null);
        list.Add("c");
        for (int i = 0, maxi = list.Count; i < maxi; ++i)
        {
            Debug.LogError(list[i].ToString());
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
