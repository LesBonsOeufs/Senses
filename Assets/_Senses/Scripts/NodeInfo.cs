using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    [CreateAssetMenu]
    public class NodeInfo : ScriptableObject
    {
        [field: SerializeField, Tooltip("Removes warp loading duration")]
        public bool IsWarpInstant { get; private set; } = false;

        [field: SerializeField, HideIf(nameof(IsWarpInstant)), ResizableTextArea, Tooltip("Text to show while warping to this node")] 
        public string WarpingText { get; private set; }

        [field: SerializeField, ResizableTextArea, Tooltip("Text to show when arriving to this node")] 
        public string AccessText { get; private set; }

        [field: SerializeField, Tooltip("If false, Node is not used for position: it will not change \"current node\" when accessed.")]
        public bool IsPositional { get; private set; } = true;

        [field: SerializeField, ShowIf(nameof(IsPositional))] public List<Route> Routes { get; private set; }

        [field: SerializeField, ShowIf(nameof(IsPositional)), ResizableTextArea, Tooltip("Text to show if keyword has no related routes from this node")]
        public string KeywordFailText { get; private set; }

        [field: SerializeField] public WindowOpenCloseAnim WindowToOpenPrefab { get; private set; }

        public void SetAccessText(string newText) => AccessText = newText;
    }

    [Serializable]
    public struct Route
    {
        public string accessKeyword;
        public bool isBack;
        [InfoBox("The entered node won't be used if isBack is true", EInfoBoxType.Warning)] public NodeInfo to;
    }
}