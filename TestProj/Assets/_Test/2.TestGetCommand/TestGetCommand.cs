using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Linq;
using System.Text;

public class TestGetCommand : MonoBehaviour {

    public Text text2;
    public Text text;
	// Use this for initialization
	void Start () {
        string[] CommandLineArgs = Environment.GetCommandLineArgs();

        string CommandLine = Environment.CommandLine;

        StringBuilder sb = new StringBuilder();

        for (int i = 0, max = CommandLineArgs.Length; i < max; i++)
        {
            sb.Append(CommandLineArgs[i] + ",");
            Debug.Log(CommandLineArgs[i]);
        }

        //获取到所有的参数
        //命令行 ：start 1.exe this is a

        text.text = sb.ToString();
        text2.text = CommandLine;

        //输出结果
        //text = 1.exe,this,is,a,
        //text2 = 1.exe this is a
    }

    // Update is called once per frame
    void Update () {
	
	}
}
