using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayLevelController : MonoBehaviour
{
    public int dreamItemsPerDay = 3;

    public static Action OnDayCompleted;

    [SerializeField] private Animator characterAnimator;

    void Start()
    {
        Globals.OnDreamItemsChanged += OnDreamItemsChanged;
    }

    private void OnDreamItemsChanged()
    {
        if(Globals.dreamItems.Count == dreamItemsPerDay)
        {
            OnDayCompleted?.Invoke();

            StartCoroutine(Co_EndDay());
        }
    }

    private IEnumerator Co_EndDay()
    {
        yield return new WaitForSeconds(1f);

        characterAnimator?.SetTrigger("GoToSleep");

        //wait for 6 seconds
        yield return new WaitForSeconds(2.2f);

        //load Dreamscape scene
        SceneManager.LoadScene("Dreamscape");
    }
}
