using NaughtyAttributes;
using UnityEngine;

namespace Root
{
    public class NodeManager : Singleton<NodeManager>
    {
        [SerializeField] private NodeInfo startNode;
        [SerializeField] private SynonymDatabaseInfo synonymDatabase;
        [ShowNativeProperty] public NodeInfo Current { get; private set; }

        private Canvas gameCanvas;

        protected override void Awake()
        {
            base.Awake();
            Current = startNode;
            gameCanvas = FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
        }

        public NodeInfo TryRouteKeyword(string keyword)
        {
            foreach (Route lRoute in Current.Routes)
            {
                if (synonymDatabase.IsSynonymOf(keyword, lRoute.accessKeyword))
                {
                    if (lRoute.to.IsPositional)
                        Current = lRoute.to;

                    if (lRoute.to.WindowToOpenPrefab != null)
                    {
                        Instantiate(lRoute.to.WindowToOpenPrefab, gameCanvas.transform.position,
                                Quaternion.identity, gameCanvas.transform);
                    }

                    return lRoute.to;
                }
            }

            return null;
        }
    }
}