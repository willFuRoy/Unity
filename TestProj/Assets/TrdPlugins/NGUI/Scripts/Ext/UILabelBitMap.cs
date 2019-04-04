using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UILabelBitMap : MonoBehaviour {
    public UIFont bitMapFont {
        get { return mbitMapFont; }
    }

    [SerializeField]
	UIFont mbitMapFont;
    [SerializeField]
    GameObject mEmationPrefab;

	UILabel mLbl;
    List<EmotionItem> mItems = new List<EmotionItem>();


    public static UIFont gEmotionFont;
    public static GameObject gEmotionPrefab;

    public static System.Func<GameObject, Transform, EmotionItem> onCheckOutEvts;
    public static System.Action<GameObject, GameObject> onCheckInEvts;
    void Awake()
	{
        if (mbitMapFont == null) {
            mbitMapFont = gEmotionFont;
        }
        if (mEmationPrefab == null) {
            mEmationPrefab = gEmotionPrefab;
        }

		mLbl = GetComponent<UILabel> ();
	}

    private void OnDestroy() {
        Clear();
    }

    public void SetSpriteData(BMSymbol symbol, Vector3 position, int index)
	{
		if (!Application.isPlaying)
			return;

        //bool isNew;
        //EmotionItem item = UIService.CheckOut<EmotionItem>(mEmationPrefab, transform, out isNew);
        EmotionItem item = onCheckOutEvts(mEmationPrefab, transform);
        if (item != null) {
            item.SetEmotion(symbol.spriteName);
            item.emotion.depth = mLbl.depth + 2;
            item.transform.localPosition = position;
            item.gameObject.SetActive(true);

            mItems.Add(item);
        }
    }

    public void Clear() {
        for (int i = 0, max = mItems.Count; i < max; ++i) {
            var item = mItems[i];
            if (item != null) {
                //UIService.CheckIn(mEmationPrefab, item.gameObject);
                onCheckInEvts(mEmationPrefab, item.gameObject);
            }
        }
        mItems.Clear();
    }
		
}
