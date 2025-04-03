using System.Threading;
using TMPro;
using UnityEngine;

namespace Root
{
    public class Interpreter : Singleton<Interpreter>
    {
        public const string USER_INPUT_TAG = "[input]";

        [SerializeField] private TextMeshProUGUI interpreterTmp;

        public async Awaitable<string> Execute(string input)
        {
            CancellationTokenSource lWaitingTextCTS = new();
            CancellationTokenSource lLinkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
                lWaitingTextCTS.Token,
                destroyCancellationToken);

            string lWaitingPrefix = "";

            bool lKeywordAccepted = NodeManager.Instance.TryRouteKeyword(input);
            //If accepted, new node. If not, same node.
            NodeInfo lNode = NodeManager.Instance.Current;

            if (lKeywordAccepted)
                lWaitingPrefix = Format(lNode.WarpingText, input);

            Awaitable lWaitingText = WaitingText(lLinkedCTS.Token, lWaitingPrefix);

            try
            {
                await Awaitable.WaitForSecondsAsync(5f, lLinkedCTS.Token);
            }
            finally
            {
                lWaitingTextCTS.Cancel();
                await lWaitingText; //Ensure cleanup
            }
            
            return Format(lKeywordAccepted ? lNode.WelcomeText : lNode.KeywordFailText, input);
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
            return text.Replace(USER_INPUT_TAG, "<color=green>" + userInput + "</color>");
        }
    }
}