using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss1Health : MonoBehaviour
{

    [SerializeField] int startingHealth = 100;
    [SerializeField] int currentHealth;
    [SerializeField] int damageAmount;
    [SerializeField] Slider healthSlider;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
        healthSlider.value = currentHealth;

    }


     public void TakeDamage(){

        if(currentHealth > 0) {


            currentHealth -= damageAmount;
            healthSlider.value = currentHealth;
        }
        if(currentHealth <= 0) {

            print("DEFEATED BOSS");
            this.enabled = false;
        }

        // Debug.Log("Current Health: " + currentHealth);


    }

}
