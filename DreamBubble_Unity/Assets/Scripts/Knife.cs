using UnityEngine;

public class Knife : MonoBehaviour
{
    public float offset = 0;

    private void Start() 
    {
        GetComponent<Animator>().SetFloat("CycleOffset", offset);
    }
}
