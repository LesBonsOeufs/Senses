using System;
using UnityEngine;

namespace Root
{
    [CreateAssetMenu]
    public class NodeInfo : ScriptableObject
    {
        [field: SerializeField] public Route[] Routes { get; private set; }

        [field: SerializeField, Tooltip("Text to show while warping to this node")] 
        public string WarpingText { get; private set; }

        [field: SerializeField, Tooltip("Text to show when arriving to this node")] 
        public string WelcomeText { get; private set; }

        [field: SerializeField, Tooltip("Text to show if keyword has no related routes")]
        public string KeywordFailText { get; private set; }
    }

    [Serializable]
    public class Route
    {
        public NodeInfo to;
        public string accessKeyword;
    }
}