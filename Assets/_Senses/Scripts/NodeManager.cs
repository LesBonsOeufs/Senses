using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class NodeManager : Singleton<NodeManager>
    {
        [SerializeField] private NodeInfo startNode;
        [SerializeField] private SynonymDatabaseInfo synonymDatabase;
        [SerializeField] private NodeEventRoutes eventRoutes;
        [SerializeField] private MailHandler mailHandler;

        [ShowNativeProperty] public NodeInfo Current { get; private set; }

        private Canvas gameCanvas;

        protected override void Awake()
        {
            base.Awake();
            Current = startNode;
            gameCanvas = FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
        }

        public EResult TryRouteKeyword(string keyword, out NodeInfo node)
        {
            foreach (Route lRoute in Current.Routes)
            {
                if (synonymDatabase.IsSynonymOf(keyword, lRoute.accessKeyword))
                {
                    node = lRoute.to;

                    if (lRoute.to.WindowToOpenPrefab != null)
                    {
                        Instantiate(lRoute.to.WindowToOpenPrefab, gameCanvas.transform.position,
                                Quaternion.identity, gameCanvas.transform);
                    }

                    if (lRoute.to.IsPositional)
                    {
                        Current = lRoute.to;
                        return EResult.POSITIONAL;
                    }

                    return EResult.NON_POSITIONAL;
                }
            }

            node = null;
            List<EventRoute> lEventRoutes = eventRoutes.GetFor(Current);

            if (lEventRoutes != null)
            {
                foreach (EventRoute lEventRoute in lEventRoutes)
                {
                    if (synonymDatabase.IsSynonymOf(keyword, lEventRoute.accessKeyword))
                    {
                        lEventRoute.to?.Invoke();
                        return EResult.EVENT;
                    }
                }
            }

            node = mailHandler.QuickMail(keyword);

            if (node != null)
                return EResult.MAIL;

            return EResult.FAIL;
        }

        public enum EResult
        {
            POSITIONAL,
            NON_POSITIONAL,
            EVENT,
            MAIL,
            FAIL
        }
    }
}