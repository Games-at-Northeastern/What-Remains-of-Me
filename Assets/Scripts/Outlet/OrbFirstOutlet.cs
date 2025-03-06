using UnityEngine;

public class OrbFirstOutlet : AControllable
{


    [SerializeField] private Animator[] coverables;
    [SerializeField] private Animator[] unCoverables;
    [SerializeField] private bool Activated = false;

    private void Update()
    {
        // slider.value = GetVirus() / 100f;

        if (GetEnergy() >= 50f)
        {
            GetComponent<SpriteRenderer>().color = Color.gray;
            for (int i = 0; i < unCoverables.Length; i++)
            {
                unCoverables[i].SetBool("Shielded", false);
                unCoverables[i].SetBool("Activate", true);
                unCoverables[i].GetComponentInParent<BoxCollider2D>().enabled = true;
            }
            for (int i = 0; i < coverables.Length; i++)
            {
                coverables[i].SetBool("Shielded", true);
                coverables[i].SetBool("Activate", true);
                coverables[i].GetComponentInParent<BoxCollider2D>().enabled = false;
            }
            Activated = true;
        }
    }
}
