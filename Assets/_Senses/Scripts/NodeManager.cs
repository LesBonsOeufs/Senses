using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Windows;

namespace Root
{
    public class NodeManager : Singleton<NodeManager>
    {
        [SerializeField] private NodeInfo startNode;
        [SerializeField] private SynonymDatabaseInfo synonymDatabase;
        [ShowNativeProperty] public NodeInfo Current { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Current = startNode;
        }

        public bool TryRouteKeyword(string keyword)
        {
            foreach (var route in Current.Routes)
            {
                if (synonymDatabase.ContainsSynonymOf(keyword, route.accessKeyword))
                {
                    Current = route.to;
                    return true;
                }
            }

            return false;
        }
    }
}