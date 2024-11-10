using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SwitchLanguage))]
public class SwitchLanguageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // �f�t�H���g�̃C���X�y�N�^��`��

        SwitchLanguage switchLanguage = (SwitchLanguage)target;

        // ���{��e�L�X�g
        EditorGUILayout.LabelField("���{��e�L�X�g", EditorStyles.boldLabel);
        switchLanguage.texts[0] = EditorGUILayout.TextField("Text [0]", switchLanguage.texts[0]);
        switchLanguage.fontSize[0] = EditorGUILayout.IntField("Font Size [0]", switchLanguage.fontSize[0]);

        // �p��e�L�X�g
        EditorGUILayout.LabelField("�p��e�L�X�g", EditorStyles.boldLabel);
        switchLanguage.texts[1] = EditorGUILayout.TextField("Text [1]", switchLanguage.texts[1]);
        switchLanguage.fontSize[1] = EditorGUILayout.IntField("Font Size [1]", switchLanguage.fontSize[1]);

        // ���̑��̗v�f
        if (switchLanguage.texts.Length > 2)
        {
            for (int i = 2; i < switchLanguage.texts.Length; i++)
            {
                EditorGUILayout.LabelField("�e�L�X�g " + i, EditorStyles.boldLabel);
                switchLanguage.texts[i] = EditorGUILayout.TextField("Text [" + i + "]", switchLanguage.texts[i]);
                switchLanguage.fontSize[i] = EditorGUILayout.IntField("Font Size [" + i + "]", switchLanguage.fontSize[i]);
            }
        }

        // �ύX��ۑ�
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
