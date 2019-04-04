using UnityEngine;
using System.Collections;

public class TestSingleton<T> : MonoBehaviour where T : MonoBehaviour {

    static T mInstance;

    public TestSingleton() : base()
    {
        mInstance = this as T;
    }

    public static T instance
    {
        get { return mInstance; }
    }
}
