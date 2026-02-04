using UnityEngine;
public class LazerCollision : MonoBehaviour
{
    public int energyAmount;
    public int virusAmount;


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {
            PlayerManager.Instance.EnergyManager.Battery += energyAmount * Time.fixedDeltaTime;
            PlayerManager.Instance.EnergyManager.Virus += virusAmount * Time.fixedDeltaTime;
        }
    }
}
