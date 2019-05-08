using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TestEnum
{
    test1,
    test2,
}

public class TestEnumNull : MonoBehaviour {

    public TestEnum e { get; private set; }
    // Use this for initialization
    void Start () {
        switch (e)
        {
            case TestEnum.test1:
                break;
            default:
                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
