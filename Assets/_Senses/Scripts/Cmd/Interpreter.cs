using NaughtyAttributes;
using System.Threading;
using TMPro;
using UnityEngine;

namespace Root
{
    public class Interpreter : Singleton<Interpreter>
    {
        public const string USER_INPUT_TAG = "[input]";

        [SerializeField] private TextMeshProUGUI interpreterTmp;
        [Foldout("Animation"), SerializeField] private float waitingDuration = 2f;
        [Foldout("Animation"), SerializeField] private float waiterCharUpdDuration = 0.2f;

        public async Awaitable<SResult> Execute(string input)
        {
            CancellationTokenSource lWaitingTextCTS = new();
            CancellationTokenSource lLinkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
                lWaitingTextCTS.Token,
                destroyCancellationToken);

            SResult lResult = new();
            string lWaitingPrefix = "";
            string lOutput = "";
            lResult.type = NodeManager.Instance.TryRouteKeyword(input, out NodeInfo lNode);

            switch (lResult.type)
            {
                case NodeManager.EResult.POSITIONAL:
                    lResult.directory = lNode.Directory;
                    lResult.windowFromDirectory = lNode.WindowFromDirectory;
                    goto case NodeManager.EResult.NON_POSITIONAL;
                case NodeManager.EResult.NON_POSITIONAL:
                    lOutput = lNode.AccessText;
                    lWaitingPrefix = Format(lNode.WarpingText, input);
                    break;
                case NodeManager.EResult.EVENT:
                    lOutput = "Event sent successfully!";
                    break;
                case NodeManager.EResult.MAIL:
                    lOutput = $"QuickMail received: {lNode.name}";
                    break;
                case NodeManager.EResult.FAIL:
                    lOutput = NodeManager.Instance.Current.KeywordFailText;
                    break;
            }

            lResult.output = Format(lOutput, input);
            bool lNodeReceived = lNode != null;

            if (lNodeReceived && !lNode.IsWarpInstant)
            {
                Awaitable lWaitingText = WaitingText(lLinkedCTS.Token, lWaitingPrefix);

                try
                {
                    await Awaitable.WaitForSecondsAsync(waitingDuration, lLinkedCTS.Token);
                }
                finally
                {
                    lWaitingTextCTS.Cancel();
                    await lWaitingText; //Ensure cleanup
                }
            }

            return lResult;
        }

        private async Awaitable WaitingText(CancellationToken cts, string prefix = "")
        {
            string[] lLoopingTexts = { ".", "..", "..." };
            int lIndex = 0;

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    interpreterTmp.text = $"{prefix}{lLoopingTexts[lIndex++]}";
                    lIndex %= lLoopingTexts.Length;
                    await Awaitable.WaitForSecondsAsync(waiterCharUpdDuration, destroyCancellationToken);
                }
            }
            finally
            {
                interpreterTmp.text = "";
            }
        }

        private string Format(string text, string userInput)
        {
            return text.Replace(USER_INPUT_TAG, userInput);
        }

        public struct SResult
        {
            public NodeManager.EResult type;
            public string directory;
            public WindowOpenCloseAnim windowFromDirectory;
            public string output;
        }
    }
}