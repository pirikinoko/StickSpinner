using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SwitchLanguage))]
public class SwitchLanguageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // デフォルトのインスペクタを描画

        SwitchLanguage switchLanguage = (SwitchLanguage)target;

        // 日本語テキスト
        EditorGUILayout.LabelField("日本語テキスト", EditorStyles.boldLabel);
        switchLanguage.texts[0] = EditorGUILayout.TextField("Text [0]", switchLanguage.texts[0]);
        switchLanguage.fontSize[0] = EditorGUILayout.IntField("Font Size [0]", switchLanguage.fontSize[0]);

        // 英語テキスト
        EditorGUILayout.LabelField("英語テキスト", EditorStyles.boldLabel);
        switchLanguage.texts[1] = EditorGUILayout.TextField("Text [1]", switchLanguage.texts[1]);
        switchLanguage.fontSize[1] = EditorGUILayout.IntField("Font Size [1]", switchLanguage.fontSize[1]);

        // その他の要素
        if (switchLanguage.texts.Length > 2)
        {
            for (int i = 2; i < switchLanguage.texts.Length; i++)
            {
                EditorGUILayout.LabelField("テキスト " + i, EditorStyles.boldLabel);
                switchLanguage.texts[i] = EditorGUILayout.TextField("Text [" + i + "]", switchLanguage.texts[i]);
                switchLanguage.fontSize[i] = EditorGUILayout.IntField("Font Size [" + i + "]", switchLanguage.fontSize[i]);
            }
        }

        // 変更を保存
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
