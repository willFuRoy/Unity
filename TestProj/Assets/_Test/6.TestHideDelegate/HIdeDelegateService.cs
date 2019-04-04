using UnityEngine;
using System.Collections;
using System;

public class HIdeDelegateService : TestSingleton<HIdeDelegateService>
{
    public delegate void HideDelegate();
    public static HideDelegate evt;

    [ContextMenu("ActiveEvt")]
    public void ActiveEvt()
    {
        if (evt != null)
            evt.Invoke();
    }
}
