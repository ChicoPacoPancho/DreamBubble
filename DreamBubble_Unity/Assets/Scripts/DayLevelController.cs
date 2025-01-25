using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayLevelController : MonoBehaviour
{
    public int dreamItemsPerDay = 3;

    void Start()
    {
        Globals.OnDreamItemsChanged += OnDreamItemsChanged;
    }

    private void OnDreamItemsChanged()
    {
        if(Globals.dreamItems.Count == dreamItemsPerDay)
        {
            GoToSleep();
        }
    }

    void Update()
    {
        //debug stuff. Delete later
        if(Input.GetKeyDown(KeyCode.RightCurlyBracket))
        {
            GoToSleep();
        }
    }


    private void GoToSleep()
    {
        //load Dreamscape scene
        SceneManager.LoadScene("Dreamscape");
    }
}
