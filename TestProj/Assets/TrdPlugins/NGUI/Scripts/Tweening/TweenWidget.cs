using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//by fxc 2018/9/4:最好不要使用此脚本，在设计期把结构最好，尽量删除此脚本

/// <summary>
/// Tween the widget's size.
/// </summary>
/// 
[RequireComponent(typeof(UIWidget))]
[AddComponentMenu("NGUI/Tween/Tween Widget")]
public class TweenWidget : UITweener
{
    public Vector3 from = Vector3.one;
    public Vector3 to = Vector3.one;
    public bool updateTable = false;
    public bool updateChildTable = true;
    public UITable specialTable;
    public BoxCollider mCollider;
    public Vector3 colliderSize;
    public Vector3 originScale;

    Transform mTrans;
    UITable mTable;
    List<UITable> mTables;

    public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

    public Vector3 value { get { return cachedTransform.localScale; } set { cachedTransform.localScale = value; } }

    [System.Obsolete("Use 'value' instead")]
    public Vector3 scale { get { return this.value; } set { this.value = value; } }

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished)
    {
        value = from * (1f - factor) + to * factor;
        if (mCollider != null) mCollider.size = new Vector3(originScale.x * colliderSize.x / value.x, originScale.y * colliderSize.y / value.y, colliderSize.z);
        if (updateTable)
        {
            if (mTables == null)
            {
                mTables = new List<UITable>();
                if (updateChildTable)
                {
                    mTables.AddRange(GetComponentsInChildren<UITable>());
                }

                UITable t = NGUITools.FindInParents<UITable>(gameObject);
                if (t != null)
                {
                    mTables.Add(t);
                }

                if (mTables.Count == 0)
                {
                    updateTable = false;
                    return;
                }
            }
            for (int i = 0, max = mTables.Count; i < max; ++i)
            {
                mTables[i].repositionNow = true;
            }
        }
    }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>

    static public TweenWidget Begin(GameObject go, float duration, Vector3 scale, BoxCollider collider, Vector3 colliderSize, Vector3 originScale)
    {
        TweenWidget comp = UITweener.Begin<TweenWidget>(go, duration);
        comp.from = comp.value;
        comp.to = scale;
        comp.mCollider = collider;
        comp.colliderSize = colliderSize;
        comp.originScale = originScale;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    [ContextMenu("Set 'From' to current value")]
    public override void SetStartToCurrentValue() { from = value; }

    [ContextMenu("Set 'To' to current value")]
    public override void SetEndToCurrentValue() { to = value; }

    [ContextMenu("Assume value of 'From'")]
    void SetCurrentValueToStart() { value = from; }

    [ContextMenu("Assume value of 'To'")]
    void SetCurrentValueToEnd() { value = to; }

}
