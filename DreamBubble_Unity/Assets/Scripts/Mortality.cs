using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mortality : MonoBehaviour
{
    private List<Shield> m_Shields = new List<Shield>();

    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.CompareTag("InstantDeath"))
        {
            Die();
        }
        else if(other.gameObject.CompareTag("Damage"))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        if(m_Shields.Count == 0)
        {
            Die();
            return;
        }

        //TODO: tell shield to take damage here. Shiled will remove itself from list if it is destroyed.
    }

    public void Die()
    {
        StartCoroutine(Co_Die());
    }

    private IEnumerator Co_Die()
    {
        yield return new WaitForSeconds(0.2f);

        Globals.ClearDreamItems();
        

        SceneManager.LoadScene("House");
    }
}
