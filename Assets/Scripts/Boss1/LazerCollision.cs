using UnityEngine;
public class LazerCollision : MonoBehaviour
{
    public int energyAmount;
    public int virusAmount;


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {
            PlayerRef.PlayerManager.EnergyManager.Battery += energyAmount * Time.fixedDeltaTime;
            PlayerRef.PlayerManager.EnergyManager.Virus += virusAmount * Time.fixedDeltaTime;
        }
    }
}
