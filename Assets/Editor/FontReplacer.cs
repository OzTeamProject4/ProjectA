using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class FontReplacer : EditorWindow
{
    private TMP_FontAsset _targetFont;
    private int _replacedCount = 0;

    [MenuItem("Tools/Font Replacer")]
    public static void ShowWindow()
    {
        GetWindow<FontReplacer>("Font Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("TMP 폰트 일괄 교체", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        _targetFont = (TMP_FontAsset)EditorGUILayout.ObjectField(
            "교체할 폰트", _targetFont, typeof(TMP_FontAsset), false);

        EditorGUILayout.Space();

        if (_targetFont == null)
        {
            EditorGUILayout.HelpBox("교체할 Font Asset을 선택해주세요.", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("씬 내 텍스트 교체 (선택 오브젝트)"))
            ReplaceInScene();

        if (GUILayout.Button("프리팹 텍스트 교체 (KKH)"))
            ReplaceInPrefabs();

        if (GUILayout.Button("씬(선택) + 프리팹(KKH) 교체"))
        {
            ReplaceInScene();
            ReplaceInPrefabs();
        }
    }

    // =========================================================================
    // 씬 내 교체 (Hierarchy 에서 선택한 오브젝트 및 그 자식만 대상)
    // =========================================================================
    private void ReplaceInScene()
    {
        _replacedCount = 0;

        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("알림", "Hierarchy에서 교체할 오브젝트를 선택해주세요.", "확인");
            return;
        }

        HashSet<TextMeshProUGUI> targets = new HashSet<TextMeshProUGUI>();

        foreach (GameObject selectedObject in selectedObjects)
        {
            if (null == selectedObject)
            {
                continue;
            }

            TextMeshProUGUI[] texts = selectedObject.GetComponentsInChildren<TextMeshProUGUI>(true);

            foreach (TextMeshProUGUI tmp in texts)
            {
                targets.Add(tmp);
            }
        }

        foreach (TextMeshProUGUI tmp in targets)
        {
            Undo.RecordObject(tmp, "Replace Font");
            tmp.font = _targetFont;
            EditorUtility.SetDirty(tmp);
            _replacedCount++;
        }

        Debug.Log($"[FontReplacer] 씬 내 텍스트 {_replacedCount}개 교체 완료");
        EditorUtility.DisplayDialog("완료", $"씬 내 텍스트 {_replacedCount}개 교체 완료", "확인");
    }

    // =========================================================================
    // 프리팹 전체 교체
    // =========================================================================
    private void ReplaceInPrefabs()
    {
        _replacedCount = 0;

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Devs/KKH" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = PrefabUtility.LoadPrefabContents(path);

            TextMeshProUGUI[] texts = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);
            bool changed = false;

            foreach (TextMeshProUGUI tmp in texts)
            {
                tmp.font = _targetFont;
                changed = true;
                _replacedCount++;
            }

            if (changed)
            {
                PrefabUtility.SaveAsPrefabAsset(prefab, path);
                Debug.Log($"[FontReplacer] 프리팹 교체: {path}");
            }

            PrefabUtility.UnloadPrefabContents(prefab);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[FontReplacer] 프리팹 텍스트 {_replacedCount}개 교체 완료");
        EditorUtility.DisplayDialog("완료", $"프리팹 텍스트 {_replacedCount}개 교체 완료", "확인");
    }
}
