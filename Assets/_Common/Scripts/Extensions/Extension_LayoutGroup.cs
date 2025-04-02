using UnityEngine;
using UnityEngine.UI;

public static class Extension_LayoutGroup
{
    public static void RefreshLayoutGroupsImmediateAndRecursive(this GameObject root)
    {
        var lComponentsInChildren = root.GetComponentsInChildren<LayoutGroup>(true);

        foreach (var layoutGroup in lComponentsInChildren)
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());

        var parent = root.GetComponent<LayoutGroup>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent.GetComponent<RectTransform>());
    }
}