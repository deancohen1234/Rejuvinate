using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WindGustAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        //on creation play animation
        GetComponent<Animator>().SetTrigger("StartGust");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnAnimationFinished()
    {
        Destroy(gameObject);
    }
}
