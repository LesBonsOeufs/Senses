using NaughtyAttributes;
using UnityEngine;

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

        public NodeInfo TryRouteKeyword(string keyword)
        {
            foreach (Route lRoute in Current.Routes)
            {
                if (synonymDatabase.ContainsSynonymOf(keyword, lRoute.accessKeyword))
                {
                    Current = lRoute.to;
                    return Current;
                }
            }

            return null;
        }
    }
}