using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Root
{
    public class MailManager : Singleton<MailManager>
    {
        [SerializeField, Expandable] private NodeInfo mailNode;
        [SerializeField] private SerializedDictionary<string, NodeInfo> quickMails = new();
        [SerializeField, ReadOnly] private List<Route> runtimeAddedRoutes = new();

        public NodeInfo QuickMail(string code)
        {
            if (!quickMails.TryGetValue(code, out NodeInfo lNode))
                return null;

            Route lRoute = new Route { accessKeyword = mailNode.Routes.Count.ToString(), to = lNode };
            mailNode.Routes.Add(lRoute);
            runtimeAddedRoutes.Add(lRoute);
            return lNode;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (Route lRuntimeRoute in runtimeAddedRoutes)
                mailNode.Routes.Remove(lRuntimeRoute);
        }
    }
}