using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGoal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        

        Debug.Log("You Win!");

        SceneManager.LoadScene("TitleScreen");
    }
}
