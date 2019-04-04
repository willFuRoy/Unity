using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TestLock : MonoBehaviour {

    private static Object lockA = new Object();

    int count = 0;

    [ContextMenu("Go")]
	void Start () {
        Thread a = new Thread(ThreadA);
        a.Start();
        Thread b = new Thread(ThreadB);
        b.Start();
    }

    void ThreadA()
    {
        for (int i = 0; i < 5; i++)
        {
            TestThread("ThreadA");
        }
    }

    void ThreadB()
    {
        for (int i = 0; i < 5; i++)
        {
            TestThread("ThreadB");
        }
    }

    void TestThread(string threadName)
    {
        lock(lockA)
        {
            Debug.Log(threadName + count);
            count += 20;
        }
    }    
}
