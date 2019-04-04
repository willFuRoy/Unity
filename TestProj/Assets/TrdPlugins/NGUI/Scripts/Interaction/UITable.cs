//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// All children added to the game object with this script will be arranged into a table
/// with rows and columns automatically adjusting their size to fit their content
/// (think "table" tag in HTML).
/// </summary>

[AddComponentMenu("NGUI/Interaction/Table")]
public class UITable : UIWidgetContainer
{
	public delegate void OnReposition ();

	public enum Direction
	{
		Down,
		Up,
	}

	public enum Sorting
	{
		None,
		Alphabetic,
		Horizontal,
		Vertical,
		Custom,
	}

	/// <summary>
	/// How many columns there will be before a new line is started. 0 means unlimited.
	/// </summary>

	public int columns = 0;

	/// <summary>
	/// Which way the new lines will be added.
	/// </summary>

	public Direction direction = Direction.Down;

	/// <summary>
	/// How to sort the grid's elements.
	/// </summary>

	public Sorting sorting = Sorting.None;

	/// <summary>
	/// Final pivot point for the table itself.
	/// </summary>

	public UIWidget.Pivot pivot = UIWidget.Pivot.TopLeft;

	/// <summary>
	/// Final pivot point for the table's content.
	/// </summary>

	public UIWidget.Pivot cellAlignment = UIWidget.Pivot.TopLeft;

	/// <summary>
	/// Whether inactive children will be discarded from the table's calculations.
	/// </summary>

	public bool hideInactive = true;

	/// <summary>
	/// Whether the parent container will be notified of the table's changes.
	/// </summary>

	public bool keepWithinPanel = false;

	/// <summary>
	/// Padding around each entry, in pixels.
	/// </summary>

	public Vector2 padding = Vector2.zero;

	/// <summary>
	/// Delegate function that will be called when the table repositions its content.
	/// </summary>

	public OnReposition onReposition;

	/// <summary>
	/// Custom sort delegate, used when the sorting method is set to 'custom'.
	/// </summary>

	public System.Comparison<Transform> onCustomSort;

	protected UIPanel mPanel;
	protected bool mInitDone = false;
	protected bool mReposition = false;

	/// <summary>
	/// Reposition the children on the next Update().
	/// </summary>

	public bool repositionNow { set { if (value) { mReposition = true; waitframes = 0; enabled = true; } } }

    // llm 调整位置时，使用指定的窗体计算大小，可以替换到下面的 pcx, pcy
    public bool useUITag = false;

    //By Zsy : 分别用于固定横向和纵向bounds的图片
    public UISprite pcy;
    public UISprite pcx;
    //public float boundY = 0;
    //public float boundX = 0;

    /// <summary>
    /// Get the current list of the grid's children.
    /// </summary>
    public List<Transform> GetChildList ()
	{
		Transform myTrans = transform;
		List<Transform> list = new List<Transform>();

		for (int i = 0; i < myTrans.childCount; ++i)
		{
			Transform t = myTrans.GetChild(i);
			if (!hideInactive || (t && NGUITools.GetActive(t.gameObject)))
				list.Add(t);
		}

		// Sort the list using the desired sorting logic
		if (sorting != Sorting.None)
		{
			if (sorting == Sorting.Alphabetic) list.Sort(UIGrid.SortByName);
			else if (sorting == Sorting.Horizontal) list.Sort(UIGrid.SortHorizontal);
			else if (sorting == Sorting.Vertical) list.Sort(UIGrid.SortVertical);
			else if (onCustomSort != null) list.Sort(onCustomSort);
			else Sort(list);
		}
		return list;
	}

	/// <summary>
	/// Want your own custom sorting logic? Override this function.
	/// </summary>

	protected virtual void Sort (List<Transform> list) { list.Sort(UIGrid.SortByName); }

	/// <summary>
	/// Position the grid's contents when the script starts.
	/// </summary>

	protected virtual void Start ()
	{
		Init();
		Reposition();
        //StartCoroutine(ReShowPanel());
	}

//    IEnumerator ReShowPanel()
//    {
//        yield return null;
//        mPanel.RebuildAllDrawCalls();
//        enabled = false;
//    }

	/// <summary>
	/// Find the necessary components.
	/// </summary>

	protected virtual void Init ()
	{
		mInitDone = true;
		mPanel = NGUITools.FindInParents<UIPanel>(gameObject);
	}

	/// <summary>
	/// Is it time to reposition? Do so now.
	/// </summary>

	int waitframes = -99;
	protected virtual void LateUpdate ()
	{
        if (mReposition) {
            Reposition();
        }

        if (waitframes >= 2) {
            mReposition = false;
            enabled = false;
            return;
        }
        waitframes++;
    }

	/// <summary>
	/// Reposition the content on inspector validation.
	/// </summary>

	void OnValidate () { if (!Application.isPlaying && NGUITools.GetActive(this)) Reposition(); }

    // llm 2018-1-16 使用指定控件进行Bounds计算
    Bounds CalculateBounds(Transform trans, bool considerInactive) {
        if (useUITag) {
            UIRepositionTag tag = trans.GetComponentInChildren<UIRepositionTag>();
            if (tag != null) {
                return NGUIMath.CalculateRelativeWidgetBounds(tag.transform, tag.transform, considerInactive, false);
            }
        }

        return NGUIMath.CalculateRelativeWidgetBounds(trans, considerInactive);
    }

    // llm 2018-1-16 使用指定控件进行Bounds计算
    Bounds CalculateAllBounds(Transform trans) {
        if (useUITag) {
            // llm 使用指定的 UI进行 Bounds 计算
            UIRepositionTag[] tags = trans.GetComponentsInChildren<UIRepositionTag>(true);
            Bounds r = new Bounds();
            r.min = Vector3.one * float.MaxValue;
            r.max = Vector3.one * float.MinValue;
            for (int i = 0, max = tags.Length; i < max; ++i) {
                UIRepositionTag tag = tags[i];
                Bounds b = NGUIMath.CalculateRelativeWidgetBounds(trans, tag.transform, false, false);
                r.Encapsulate(b);
            }

            return r;
        }
        
        return NGUIMath.CalculateRelativeWidgetBounds(trans);
    }

    /// <summary>
    /// Positions the grid items, taking their own size into consideration.
    /// </summary>

    protected void RepositionVariableSize (List<Transform> children)
	{
		float xOffset = 0;
		float yOffset = 0;

		int cols = columns > 0 ? children.Count / columns + 1 : 1;
		int rows = columns > 0 ? columns : children.Count;

		Bounds[,] bounds = new Bounds[cols, rows];
		Bounds[] boundsRows = new Bounds[rows];
		Bounds[] boundsCols = new Bounds[cols];

		int x = 0;
		int y = 0;

		for (int i = 0, imax = children.Count; i < imax; ++i)
		{
			Transform t = children[i];

            // llm 可以使用指定的窗体计算 bounds
            //Bounds b = NGUIMath.CalculateRelativeWidgetBounds(t, !hideInactive);
            Bounds b = CalculateBounds(t, !hideInactive);

			//By Zsy : 这里只实现了纵向,因为横向暂时没有用到,如果以后需要请自行补充,方法与纵向类似
            if (pcy != null)
            {
				//By Zsy : b是获取的总AABB包围盒，这里直接用pcy的包围盒代替总bounds在Y方向值以固定Y坐标
                Bounds b1 = NGUIMath.CalculateRelativeWidgetBounds(pcy.cachedTransform, !hideInactive);
                Vector3 ex = b.extents;
                ex.y = b1.extents.y;
                b.extents = ex;
                ex = b.center;
                ex.y = b1.center.y;
                b.center = ex;
                b.max = b.center + b.extents;
                b.min = b.center - b.extents;
                b.size = b.extents * 2;
            }

            Vector3 scale = t.localScale;
			b.min = Vector3.Scale(b.min, scale);
			b.max = Vector3.Scale(b.max, scale);
			bounds[y, x] = b;

			boundsRows[x].Encapsulate(b);
			boundsCols[y].Encapsulate(b);

			if (++x >= columns && columns > 0)
			{
				x = 0;
				++y;
			}
		}

		x = 0;
		y = 0;

		Vector2 po = NGUIMath.GetPivotOffset(cellAlignment);

		for (int i = 0, imax = children.Count; i < imax; ++i)
		{
			Transform t = children[i];
			Bounds b = bounds[y, x];
			Bounds br = boundsRows[x];
			Bounds bc = boundsCols[y];

			Vector3 pos = t.localPosition;
			pos.x = xOffset + b.extents.x - b.center.x;
			pos.x -= Mathf.Lerp(0f, b.max.x - b.min.x - br.max.x + br.min.x, po.x) - padding.x;


			if (direction == Direction.Down)
			{
				pos.y = -yOffset - b.extents.y - b.center.y;
				pos.y += Mathf.Lerp(b.max.y - b.min.y - bc.max.y + bc.min.y, 0f, po.y) - padding.y;
			}
			else
			{
				pos.y = yOffset + b.extents.y - b.center.y;
				pos.y -= Mathf.Lerp(0f, b.max.y - b.min.y - bc.max.y + bc.min.y, po.y) - padding.y;
			}

			xOffset += br.size.x + padding.x * 2f;

			t.localPosition = pos;

			if (++x >= columns && columns > 0)
			{
				x = 0;
				++y;

				xOffset = 0f;
				yOffset += bc.size.y + padding.y * 2f;
			}
		}

		// Apply the origin offset
		if (pivot != UIWidget.Pivot.TopLeft)
		{
			po = NGUIMath.GetPivotOffset(pivot);

			float fx, fy;

            // llm 2018-1-16 是否使用 UIRepositionTag 进行
            //Bounds b = NGUIMath.CalculateRelativeWidgetBounds(transform);
            Bounds b = CalculateAllBounds(transform);

            fx = Mathf.Lerp(0f, b.size.x, po.x);
			fy = Mathf.Lerp(-b.size.y, 0f, po.y);

			Transform myTrans = transform;

			for (int i = 0; i < myTrans.childCount; ++i)
			{
				Transform t = myTrans.GetChild(i);
				SpringPosition sp = t.GetComponent<SpringPosition>();

				if (sp != null)
				{
					sp.target.x -= fx;
					sp.target.y -= fy;
				}
				else
				{
					Vector3 pos = t.localPosition;
					pos.x -= fx;
					pos.y -= fy;
					t.localPosition = pos;
				}
			}
		}
	}

	/// <summary>
	/// Recalculate the position of all elements within the table, sorting them alphabetically if necessary.
	/// </summary>

	[ContextMenu("Execute")]
	public virtual void Reposition ()
	{
		if (Application.isPlaying && !mInitDone && NGUITools.GetActive(this)) Init();

		Transform myTrans = transform;
		List<Transform> ch = GetChildList();
		if (ch.Count > 0) RepositionVariableSize(ch);

		if (keepWithinPanel && mPanel != null)
		{
			mPanel.ConstrainTargetToBounds(myTrans, true);
			UIScrollView sv = mPanel.GetComponent<UIScrollView>();
			if (sv != null) sv.UpdateScrollbars(true);
		}

		if (onReposition != null)
			onReposition();
	}

    [ContextMenu("ExecuteAll")]
    public void RepositionAll() {
        UITable[] tbs = GetComponentsInChildren<UITable>();
        for (int i = tbs.Length - 1; i >= 0; --i) {
            var t = tbs[i];
            if (t != this) {
                t.Reposition();
            }
        }
        Reposition();
    }

	public void RepositionLater()
	{
		mReposition = true;
		waitframes = 0;
		enabled = true;
	}
}
