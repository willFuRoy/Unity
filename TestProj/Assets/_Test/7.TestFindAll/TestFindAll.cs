using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestFind
{
    public int a = 1;
}

public class TestFindAll : MonoBehaviour {

    List<TestFind> list = new List<TestFind>();
    private void Start()
    {
        List<TestFind> al = list.FindAll(x => x.a == 2);
        Debug.Log(al.Count);
    }
}
