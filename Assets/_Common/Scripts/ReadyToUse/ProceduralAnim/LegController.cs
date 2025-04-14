using UnityEngine;

public class LegController : MonoBehaviour
{
    public float maxTipDistance = 0.7f;
    [field: SerializeField] public Leg[] Legs { get; private set; }
    [SerializeField] private bool twoByTwo = true;

    //Two by two
    private bool readySwitchOrder = false;
    private bool stepOrder = true;

    //One by one
    private int currentLegIndex = 0;

    private void Update()
    {
        if (Legs.Length < 2) return;

        if (twoByTwo)
            TwoByTwo();
        else
            OneByOne();
    }

    private void TwoByTwo()
    {
        // Make sure first leg with maxTipDistance is moved in priority.
        for (int i = 0; i < Legs.Length; i++)
        {
            if (Legs[i].TipDistance > maxTipDistance)
            {
                stepOrder = i % 2 == 0;
                break;
            }
        }

        // Ordering steps
        foreach (Leg leg in Legs)
        {
            leg.Movable = stepOrder;
            stepOrder = !stepOrder;
        }

        //Below is quick & dirty
        int lIndex = stepOrder ? 0 : 1;

        if (readySwitchOrder && !Legs[lIndex].Animating)
        {
            stepOrder = !stepOrder;
            readySwitchOrder = false;
        }

        if (!readySwitchOrder && Legs[lIndex].Animating)
            readySwitchOrder = true;
    }

    private void OneByOne()
    {
        for (int i = 0; i < Legs.Length; i++)
        {
            if (i == currentLegIndex || Legs[i].TipDistance > maxTipDistance)
                Legs[i].Movable = true;
            else
                Legs[i].Movable = false;
        }

        if (readySwitchOrder && !Legs[currentLegIndex].Animating)
        {
            currentLegIndex++;
            currentLegIndex %= Legs.Length;
            readySwitchOrder = false;
        }

        if (!readySwitchOrder && Legs[currentLegIndex].Animating)
            readySwitchOrder = true;
    }
}