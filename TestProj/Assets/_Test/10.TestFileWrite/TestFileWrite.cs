using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestFileWrite : MonoBehaviour {

    byte[] data;

    void Start () {
        DateTime time = DateTime.UtcNow;
        data = new byte[1000000000];
        for (int i = 0; i < 1000000000; i++)
        {
            data[i] = (byte)i;
        }
        using (FileStream fs = new FileStream("Assets/../a.txt", FileMode.Create))
        {
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(data);
                bw.Flush();
                bw.Close();
            }
            fs.Close();
        }
        TimeSpan ts = DateTime.UtcNow - time;
        Debug.LogError(ts.Milliseconds);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
