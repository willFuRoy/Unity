using UnityEngine;
using System.Collections;

public class EmotionItem : MonoBehaviour {
    public UIAtlas emotionAtlas;
    public UISprite emotion;
	public UISpriteAnimation anim;

	public string Value
	{
		get { return emotion.spriteName; }
	}

    public static UIFont emotionFont;

    public static System.Action<EmotionItem> onPlayEmotionEvt;

	public void SetEmotion(string spriteName)
	{
        emotion.atlas = emotionAtlas == null ? emotionFont.atlas : emotionAtlas;
        emotion.spriteName = spriteName;
		emotion.MakePixelPerfect ();
        if (onPlayEmotionEvt != null) {
            onPlayEmotionEvt.Invoke(this);
        }
		name = "Emotion_" + spriteName;
	}
}
