//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(UIPlayTween))]
public class UIPlayTweenEditor : Editor
{
	enum ResetOnPlay
	{
		ContinueFromCurrent,
		RestartTween,
		RestartIfNotPlaying,
	}

	enum SelectedObject
	{
		KeepCurrent,
		SetToNothing,
	}

    bool expanded = false;
    void DrawCustomField() {
        expanded = EditorGUILayout.Foldout(expanded, "Custom Tweens");
        if (expanded) {
            // draw count and elements
            UIPlayTween tw = target as UIPlayTween;

            EditorGUI.indentLevel++;
            int cnt = EditorGUILayout.IntField("Count", tw.customTweens != null ? tw.customTweens.Length : 0);
            EditorGUI.indentLevel--;

            if (cnt != tw.customTweens.Length) {
                Array.Resize<GameObject>(ref tw.customTweens, cnt);
            }
            if (tw != null) {
                for (int i = 0; i < tw.customTweens.Length; ++i) {
                    EditorGUI.indentLevel++;
                    tw.customTweens[i] = EditorGUILayout.ObjectField("Element " + i, tw.customTweens[i], typeof(GameObject), true) as GameObject;
                    EditorGUI.indentLevel--;
                }
            }
        }
    }

    public override void OnInspectorGUI ()
	{
		NGUIEditorTools.SetLabelWidth(120f);
		UIPlayTween tw = target as UIPlayTween;
		GUILayout.Space(6f);

		GUI.changed = false;
		GameObject tt = (GameObject)EditorGUILayout.ObjectField("Tween Target", tw.tweenTarget, typeof(GameObject), true);
		bool inc = EditorGUILayout.Toggle("Include Children", tw.includeChildren);
        DrawCustomField();
		int group = EditorGUILayout.IntField("Tween Group", tw.tweenGroup, GUILayout.Width(160f));

		AnimationOrTween.Trigger trigger = (AnimationOrTween.Trigger)EditorGUILayout.EnumPopup("Trigger condition", tw.trigger);
		AnimationOrTween.Direction dir = (AnimationOrTween.Direction)EditorGUILayout.EnumPopup("Play direction", tw.playDirection);
		AnimationOrTween.EnableCondition enab = (AnimationOrTween.EnableCondition)EditorGUILayout.EnumPopup("If target is disabled", tw.ifDisabledOnPlay);
		ResetOnPlay rs = tw.resetOnPlay ? ResetOnPlay.RestartTween : (tw.resetIfDisabled ? ResetOnPlay.RestartIfNotPlaying : ResetOnPlay.ContinueFromCurrent);
		ResetOnPlay reset = (ResetOnPlay)EditorGUILayout.EnumPopup("On activation", rs);
		AnimationOrTween.DisableCondition dis = (AnimationOrTween.DisableCondition)EditorGUILayout.EnumPopup("When finished", tw.disableWhenFinished);

		if (GUI.changed)
		{
			NGUIEditorTools.RegisterUndo("Tween Change", tw);
			tw.tweenTarget = tt;
			tw.tweenGroup = group;
			tw.includeChildren = inc;
			tw.trigger = trigger;
			tw.playDirection = dir;
			tw.ifDisabledOnPlay = enab;
			tw.resetOnPlay = (reset == ResetOnPlay.RestartTween);
			tw.resetIfDisabled = (reset == ResetOnPlay.RestartIfNotPlaying);
			tw.disableWhenFinished = dis;
			NGUITools.SetDirty(tw);
		}

		NGUIEditorTools.SetLabelWidth(80f);
		NGUIEditorTools.DrawEvents("On Finished", tw, tw.onFinished);
	}
}
