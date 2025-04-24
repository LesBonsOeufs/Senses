using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Root
{
    public class NodeEventRoutes : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<NodeInfo, List<EventRoute>> eventsPerNode;

        public List<EventRoute> GetFor(NodeInfo node)
        {
            if (eventsPerNode.TryGetValue(node, out List<EventRoute> lEventRoutes))
                return lEventRoutes;

            return null;
        }
    }

    [Serializable]
    public struct EventRoute
    {
        public string accessKeyword;
        public UnityEvent to;
    }
}