using UnityEngine;
using System.Collections;

public class TestCoroutine : MonoBehaviour {

    UnityEngine.Coroutine co;

	// Use this for initialization
	IEnumerator Start () {
        co = StartCoroutine(TestCo());
        yield return new WaitForSeconds(2.0f);
        if (co != null)
            Debug.Log(1);
        else
            Debug.Log(2);
	}
	
    IEnumerator TestCo()
    {
        int i = 50;
        while(i > 0)
        {
            i--;
            yield return 1;
            Debug.LogError(i);
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
