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

            //New node if accepted. If not, null
            NodeInfo lNode = NodeManager.Instance.TryRouteKeyword(input);
            bool lKeywordAccepted = lNode != null;

            if (lKeywordAccepted)
                lWaitingPrefix = Format(lNode.WarpingText, input);
            else
                lNode = NodeManager.Instance.Current;

            if (lKeywordAccepted && !lNode.IsWarpInstant)
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
            
            return Format(lKeywordAccepted ? lNode.AccessText : lNode.KeywordFailText, input);
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
    }
}