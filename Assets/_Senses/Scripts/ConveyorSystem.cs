using NaughtyAttributes;
using UnityEngine;

namespace Root
{
    public class ConveyorSystem : MonoBehaviour
    {
        [SerializeField] Blocker blocker1;
        [SerializeField] Blocker blocker2;

        [SerializeField] private ConveyorBelt beltMain;
        [SerializeField] private ConveyorBelt belt1;
        [SerializeField] private ConveyorBelt belt2;
        [SerializeField] private ConveyorBelt belt3;

        private bool allowUsage = false;

        private void Start()
        {
            blocker1.Close();
            blocker2.Close();
            belt1.enabled = false;
            belt2.enabled = false;
            belt3.enabled = false;
        }

        public void OpenWay(int way)
        {
            if (!allowUsage)
                return;

            switch (way)
            {
                case 0:
                    blocker1.Close();
                    blocker2.Close();
                    beltMain.enabled = true;
                    belt1.enabled = true;
                    break;
                case 1:
                    blocker1.Open();
                    blocker2.Close();
                    beltMain.enabled = true;
                    belt2.enabled = true;
                    break;
                case 2:
                    blocker1.Open();
                    blocker2.Open();
                    beltMain.enabled = true;
                    belt3.enabled = true;
                    break;
            }

            allowUsage = false;
        }

        [Button]
        public void ResetWay()
        {
            beltMain.enabled = false;
            blocker1.Close();
            blocker2.Close();
            allowUsage = true;
        }
    }
}