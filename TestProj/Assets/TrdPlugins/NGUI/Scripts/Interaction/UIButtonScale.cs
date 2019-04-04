//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Simple example script of how a button can be scaled visibly when the mouse hovers over it or it gets pressed.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button Scale")]
public class UIButtonScale : MonoBehaviour
{
	public Transform tweenTarget;
	public Vector3 hover = new Vector3(1.1f, 1.1f, 1.1f);
	public Vector3 pressed = new Vector3(1.05f, 1.05f, 1.05f);
	public float duration = 0.2f;

    //by fxc 2018/9/4:解决按钮缩放改变collider问题
    public bool useCustom = true;

    Vector3 mScale;
    BoxCollider mCollider;
    Vector3 mColliderSize;
	bool mStarted = false;

	void Start ()
	{
		if (!mStarted)
		{
			mStarted = true;
			if (tweenTarget == null) tweenTarget = transform;
			mScale = tweenTarget.localScale;
            //by fxc 2018/9/4:解决按钮缩放改变collider问题
            mCollider = GetComponent<BoxCollider>();
            mColliderSize = mCollider != null ? mCollider.size : Vector3.zero;
        }
	}

	void OnEnable () { if (mStarted) OnHover(UICamera.IsHighlighted(gameObject)); }

	void OnDisable ()
	{
		if (mStarted && tweenTarget != null)
		{
            if (useCustom)
            {
                TweenWidget tc = tweenTarget.GetComponent<TweenWidget>();

                if (tc != null)
                {
                    tc.value = mScale;
                    tc.enabled = false;
                }
            }
            else
            {
                TweenScale tc = tweenTarget.GetComponent<TweenScale>();

                if (tc != null)
                {
                    tc.value = mScale;
                    tc.enabled = false;
                }
            }
		}
	}

	void OnPress (bool isPressed)
	{
		if (enabled)
		{
			if (!mStarted) Start();

            //start by fxc 2018/9/4:解决按钮缩放改变collider问题
            if (useCustom)
            {
                TweenWidget.Begin(tweenTarget.gameObject, duration, isPressed ? Vector3.Scale(mScale, pressed) :
                    (UICamera.IsHighlighted(gameObject) ? Vector3.Scale(mScale, hover) : mScale), mCollider, mColliderSize, mScale).method = UITweener.Method.EaseInOut;
            }
            //end
            else
            {
                TweenScale.Begin(tweenTarget.gameObject, duration, isPressed ? Vector3.Scale(mScale, pressed) :
                    (UICamera.IsHighlighted(gameObject) ? Vector3.Scale(mScale, hover) : mScale)).method = UITweener.Method.EaseInOut;
            }
        }
	}

	void OnHover (bool isOver)
	{
        if (enabled)
        {
            if (!mStarted) Start();
            if (useCustom)
            {
                TweenWidget.Begin(tweenTarget.gameObject, duration, isOver ? Vector3.Scale(mScale, hover) :mScale, mCollider, mColliderSize, mScale).method = UITweener.Method.EaseInOut;
            }
            else
            {
                TweenScale.Begin(tweenTarget.gameObject, duration, isOver ? Vector3.Scale(mScale, hover) : mScale).method = UITweener.Method.EaseInOut;
            }
        }
    }

	void OnSelect (bool isSelected)
	{
		if (enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
			OnHover(isSelected);
	}
}
