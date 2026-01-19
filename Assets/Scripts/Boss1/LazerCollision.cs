using UnityEngine;
public class LazerCollision : MonoBehaviour
{
    public int energyAmount;
    public int virusAmount;


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {
            EnergyManager.Instance.Battery += energyAmount * Time.fixedDeltaTime;
            EnergyManager.Instance.Virus += virusAmount * Time.fixedDeltaTime;
        }
    }
}
