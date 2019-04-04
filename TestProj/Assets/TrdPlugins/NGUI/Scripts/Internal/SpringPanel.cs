//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Similar to SpringPosition, but also moves the panel's clipping. Works in local coordinates.
/// </summary>

[RequireComponent(typeof(UIPanel))]
[AddComponentMenu("NGUI/Internal/Spring Panel")]
public class SpringPanel : MonoBehaviour
{
	static public SpringPanel current;

	/// <summary>
	/// Target position to spring the panel to.
	/// </summary>

	public Vector3 target = Vector3.zero;

	/// <summary>
	/// Strength of the spring. The higher the value, the faster the movement.
	/// </summary>

	public float strength = 10f;

	public delegate void OnFinished ();

	/// <summary>
	/// Delegate function to call when the operation finishes.
	/// </summary>

	public OnFinished onFinished;

	Vector3 mLastPos = Vector3.zero;
	public Vector3 lastPos {
		set { mLastPos = value; }
		get{ return mLastPos; }
	}

	UIPanel mPanel;
	Transform mTrans;
	UIScrollView mDrag;

	/// <summary>
	/// Cache the transform.
	/// </summary>

	void Start ()
	{
		mPanel = GetComponent<UIPanel>();
		mDrag = GetComponent<UIScrollView>();
		mTrans = transform;
	}

	/// <summary>
	/// Advance toward the target position.
	/// </summary>

	void Update ()
	{
	    AdvanceTowardsPosition();
	}

    /// <summary>
    /// Advance toward the target position.
	/// </summary>

    Vector3 before;
    Vector3 after;
	protected virtual void AdvanceTowardsPosition ()
	{
		float delta = RealTime.deltaTime;

		bool trigger = false;
		before = mTrans.localPosition;
		after = NGUIMath.SpringLerp(mTrans.localPosition, target, strength, delta);

		if ((after - target).sqrMagnitude < 0.01f)
		{
			//By Zsy : 增加一个弹回原来位置的回调
			if ((lastPos - target).sqrMagnitude < 0.01f) {
				if(mDrag.onMoveBackToLastPosition != null)
					mDrag.onMoveBackToLastPosition ();
			}

            lastPos = target;
			after = target;
			enabled = false;
			trigger = true;
		}
		mTrans.localPosition = after;

		//Vector3 offset = after - before;
		Vector2 cr = mPanel.clipOffset;
        cr.x -= (after - before).x;
        cr.y -= (after - before).y;
		mPanel.clipOffset = cr;

        if (mDrag != null) mDrag.UpdateScrollbars(false);

		if (trigger && onFinished != null)
		{
			current = this;
			onFinished();
			current = null;
		}
    }

	/// <summary>
	/// Start the tweening process.
	/// </summary>

	static public SpringPanel Begin (GameObject go, Vector3 pos, float strength, SpringPanel sp = null)
	{
		if(sp == null)
		{
			sp = go.GetComponent<SpringPanel>();
			if (sp == null) sp = go.AddComponent<SpringPanel>();
		}

        if(sp.lastPos == Vector3.zero)
		    sp.lastPos = pos;

		sp.target = pos;
		sp.strength = strength;
		sp.onFinished = null;
		sp.enabled = true;
		return sp;
	}
		
}
