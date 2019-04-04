using UnityEngine;
using System.Collections;

public class HideDelegate : MonoBehaviour {

	// Use this for initialization
	void Start () {
        HIdeDelegateService.evt += Evt;

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void Evt()
    {
        Debug.Log("HideDelegate");
    }
}
