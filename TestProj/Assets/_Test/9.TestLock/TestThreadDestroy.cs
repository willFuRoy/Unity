using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TestThreadDestroy : MonoBehaviour {

    public TestThreadDestroy2 thread2;

    protected virtual void Awake()
    {
        Destroy(thread2.gameObject);
        Thread a = new Thread(ThreadA);
        a.Start();
        Thread b = new Thread(ThreadB);
        b.Start();
    }

    protected virtual void Start () {

	}

    void ThreadA()
    {
    }

    void ThreadB()
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 0)
                Thread.Sleep(1000);
            else
                thread2.Start();
        }
    }
}
