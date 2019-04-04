using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
    作为副本中所有触发器相关数据模型的基类。  
*/

public class LevelTiggerBase : MonoBehaviour {
    
    //此id可重复
    public int triggerId;
    
    //触发器进入执行的事情
    public string enterCmds;
    //List<CmdBaseModel> mEnterCmds; 解析成命令集，通过命令集来完成相应操作
    
    //触发器退出执行的事情
    public string exitsCmds;
    //List<CmdBaseModel> mExitsCmds;

}
