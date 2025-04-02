using System.Threading;
using TMPro;
using UnityEngine;

namespace Root
{
    public class Interpreter : Singleton<Interpreter>
    {
        [SerializeField] private TextMeshProUGUI interpreterTmp;

        public async Awaitable<string> Execute(string input)
        {
            CancellationTokenSource lWaitingTextCTS = new();
            CancellationTokenSource lLinkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
                lWaitingTextCTS.Token,
                destroyCancellationToken);

            Awaitable lWaitingText = WaitingText(lLinkedCTS.Token);

            try
            {
                await Awaitable.WaitForSecondsAsync(5f, lLinkedCTS.Token);
            }
            finally
            {
                lWaitingTextCTS.Cancel();
                await lWaitingText; //Ensure cleanup
            }
            
            return input;
        }

        public async Awaitable WaitingText(CancellationToken cts)
        {
            string[] lLoopingTexts = { ".", "..", "..." };
            int lIndex = 0;

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    interpreterTmp.text = lLoopingTexts[lIndex++];
                    lIndex %= lLoopingTexts.Length;
                    await Awaitable.WaitForSecondsAsync(0.5f, destroyCancellationToken);
                }
            }
            finally
            {
                interpreterTmp.text = "";
            }
        }
    }
}