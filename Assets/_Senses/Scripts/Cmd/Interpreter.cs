using System.Threading;
using TMPro;
using UnityEngine;

namespace Root
{
    public class Interpreter : Singleton<Interpreter>
    {
        public const string USER_INPUT_TAG = "[input]";

        [SerializeField] private TextMeshProUGUI interpreterTmp;

        public async Awaitable<SResult> Execute(string input)
        {
            CancellationTokenSource lWaitingTextCTS = new();
            CancellationTokenSource lLinkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
                lWaitingTextCTS.Token,
                destroyCancellationToken);

            SResult lResult = new();
            string lWaitingPrefix = "";
            string lOutput = "";

            switch (NodeManager.Instance.TryRouteKeyword(input, out NodeInfo lNode))
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
                    lNode = NodeManager.Instance.Current;
                    lOutput = lNode.KeywordFailText;
                    break;
            }

            lResult.output = Format(lOutput, input);
            bool lNodeReceived = lNode != null;

            if (lNodeReceived && !lNode.IsWarpInstant)
            {
                Awaitable lWaitingText = WaitingText(lLinkedCTS.Token, lWaitingPrefix);

                try
                {
                    await Awaitable.WaitForSecondsAsync(3f, lLinkedCTS.Token);
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
                    await Awaitable.WaitForSecondsAsync(0.2f, destroyCancellationToken);
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
            public string directory;
            public WindowOpenCloseAnim windowFromDirectory;
            public string output;
        }
    }
}