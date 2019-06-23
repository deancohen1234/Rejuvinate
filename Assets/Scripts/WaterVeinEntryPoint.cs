using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaterVeinEntryPoint : MonoBehaviour
{
    public WaterVein m_ParentVein;

    private Transform m_EnteringObject;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Trigger");
        //collider.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        collider.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        //collider.gameObject.GetComponent<PlayerController>().enabled = false;
        m_EnteringObject = collider.transform;

        m_ParentVein.EnterVein(collider.transform);
    }
}
