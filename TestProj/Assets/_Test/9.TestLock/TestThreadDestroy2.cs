using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestThreadDestroy2 : TestThreadDestroy
{
    public bool a = true;

    protected override void Awake()
    {

    }

    protected override void Start () {
        base.Start();
        a = false;
    }	
}
