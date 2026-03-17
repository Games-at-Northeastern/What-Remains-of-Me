using UnityEngine;

public class DisableLaserCollider : MonoBehaviour
{
    public DeathLaser connectedLaser;
    public TimedToggle timerLaser;
    public AlarmLaserTrigger alarmTrigger;
    public float timeDelay = 2f; 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Disables the laser when the "Player" first enters the collision object
        if (collision.gameObject.CompareTag("Player"))
        {
            //connectedLaser.ToggleLaser();
            //timerLaser.enabled = true;
            //wee.OnAlarmStart();
            alarmTrigger.OffAlarmStart();
            Debug.Log("Disabled the laser " + alarmTrigger.name);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //Starts up the laser again after exiting the collision box 
        // The expected time can be adjustable in timeDelay
        if (collision.gameObject.CompareTag("Player"))
        {
            //Invoke(nameof(LaserToggleOn), timeDelay);
            alarmTrigger.OnAlarmStart();
            Debug.Log("Enabled the laser " + alarmTrigger.name);
        }
    }
}
