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
        m_ParentVein.m_VeinMovementCompleted += ObjectExitedVein; 
    }

    //when player has finished vein movement
    private void ObjectExitedVein()
    {
        if (m_EnteringObject)
        {
            m_EnteringObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

            //get direction between last two points in curve to use as launch velocity
            BezierCurve veinCurve = m_ParentVein.GetCurve();
            Vector2 velocityDirection = veinCurve.GetPoint(veinCurve.GetPointArrayLength() - 1, null) - veinCurve.GetPoint(veinCurve.GetPointArrayLength() - 2, null);

            Debug.Log(velocityDirection);
            m_EnteringObject.GetComponent<Rigidbody2D>().velocity = velocityDirection.normalized * 20.0f;
        }
    }
}
