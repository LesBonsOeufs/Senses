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
                if (synonymDatabase.IsSynonymOf(keyword, lRoute.accessKeyword))
                {
                    if (lRoute.to.IsPositional)
                        Current = lRoute.to;

                    return lRoute.to;
                }
            }

            return null;
        }
    }
}