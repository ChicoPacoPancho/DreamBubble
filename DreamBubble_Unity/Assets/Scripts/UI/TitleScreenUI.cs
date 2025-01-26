using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenUI : MonoBehaviour
{
    
    public Button startButton;


    void OnEnable() 
    {
        startButton.onClick.AddListener(StartGame);
    }

    void OnDisable()
    {
        startButton.onClick.RemoveListener(StartGame);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("House");
    }
}
