using System.Threading;
using TMPro;
using UnityEngine;

namespace Root
{
    public class Interpreter : Singleton<Interpreter>
    {
        [SerializeField] private SynonymDatabaseInfo testDataInfo;

        [SerializeField] private TextMeshProUGUI interpreterTmp;

        public async Awaitable<string> Execute(string input)
        {
            CancellationTokenSource lWaitingTextCTS = new();
            CancellationTokenSource lLinkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
                lWaitingTextCTS.Token,
                destroyCancellationToken);

            string lWaitingPrefix = "";

            bool lConnect;
            if (testDataInfo.ContainsSynonymOf(input, "connect"))
            {
                lConnect = true;
                lWaitingPrefix = "Connecting";
            }
            else
                lConnect = false;

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
            
            return lConnect ? "Connected" : "No internet connection";
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
    }
}