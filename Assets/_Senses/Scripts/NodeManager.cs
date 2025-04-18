using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Root
{
    public class NodeManager : Singleton<NodeManager>
    {
        [SerializeField] private NodeInfo startNode;
        [SerializeField] private SynonymDatabaseInfo synonymDatabase;
        [SerializeField] private NodeEventRoutes eventRoutes;
        [SerializeField] private MailHandler mailHandler;

        [ShowNativeProperty] public NodeInfo Current 
        {
            get => _current;
            private set
            {
                if (_current == value)
                    return;

                previousNode = _current;
                _current = value;
            }
        }
        private NodeInfo _current;

        private NodeInfo previousNode;
        private Canvas gameCanvas;

        protected override void Awake()
        {
            base.Awake();
            Current = startNode;

            //Use first active Overlay Canvas
            gameCanvas = FindObjectsByType<Canvas>(FindObjectsSortMode.None)
                .Where(canvas => canvas.renderMode == RenderMode.ScreenSpaceOverlay).First();
        }

        public EResult TryRouteKeyword(string keyword, out NodeInfo node)
        {
            //Routes array made of current node's routes + mail route (mail system accessible everywhere)
            List<Route> lRoutes = new(Current.Routes)
            {
                new Route { accessKeyword = mailHandler.AccessKey, to = mailHandler.MailNode }
            };

            foreach (Route lRoute in lRoutes)
            {
                if (synonymDatabase.IsSynonymOf(keyword, lRoute.accessKeyword))
                {
                    node = lRoute.isBack ? previousNode : lRoute.to;

                    if (node.WindowToOpenPrefab != null)
                    {
                        Instantiate(node.WindowToOpenPrefab, gameCanvas.transform.position,
                                Quaternion.identity, gameCanvas.transform);
                    }

                    if (node.IsPositional)
                    {
                        Current = node;
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