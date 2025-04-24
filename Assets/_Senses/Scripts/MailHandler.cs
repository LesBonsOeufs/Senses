using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class MailHandler : MonoBehaviour
    {
        [field: SerializeField] public string AccessKey { get; private set; } = "mail";
        [field: SerializeField, Expandable] public NodeInfo MailNode { get; private set; }
        [SerializeField] private SerializedDictionary<string, NodeInfo> quickMails = new();
        [SerializeField, ReadOnly] private List<Route> runtimeAddedRoutes = new();

        private string baseAccessText;

        private void Awake()
        {
            baseAccessText = MailNode.AccessText;
            MailNode.SetAccessText(GetDynamicAccessText() + baseAccessText);
        }

        public NodeInfo QuickMail(string code)
        {
            if (!quickMails.TryGetValue(code, out NodeInfo lNode))
                return null;

            Route lRoute = new Route { accessKeyword = MailNode.Routes.Count.ToString(), to = lNode };
            MailNode.Routes.Add(lRoute);
            runtimeAddedRoutes.Add(lRoute);

            MailNode.SetAccessText(GetDynamicAccessText() + baseAccessText);
            return lNode;
        }

        private string GetDynamicAccessText()
        {
            //-1 for accounting the isBack
            string lReturnedText = $"You have {MailNode.Routes.Count - 1} mails.\n";

            foreach (Route lRoute in MailNode.Routes)
            {
                if (lRoute.isPrevious)
                    continue;

                lReturnedText += $"{lRoute.accessKeyword}->{lRoute.to.AccessText.Split("\n")[0]}\n";
            }

            return lReturnedText;
        }

        private void OnDestroy()
        {
            foreach (Route lRuntimeRoute in runtimeAddedRoutes)
                MailNode.Routes.Remove(lRuntimeRoute);

            MailNode.SetAccessText(baseAccessText);
        }
    }
}