using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour
{
    public float m_Smoothing = 1.0f;

    private Transform m_Camera;

    private Vector3 m_PreviousCamPos;

    private void Awake()
    {
        m_Camera = Camera.main.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_PreviousCamPos = m_Camera.position;
    }

    // Update is called once per frame
    void Update()
    {
        float parallaxX = (m_PreviousCamPos.x - m_Camera.position.x) * transform.position.z * -1; //-1 i think because it is moving in opposite direction as cam
        float parallaxY = (m_PreviousCamPos.y - m_Camera.position.y) * transform.position.z * -1; //-1 i think because it is moving in opposite direction as cam

        float backgroundTargetPosX = transform.position.x + parallaxX;
        float backgroundTargetPosY = transform.position.y + parallaxY;

        Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgroundTargetPosY, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, backgroundTargetPos, Time.deltaTime * m_Smoothing);

        m_PreviousCamPos = m_Camera.position;
    }
}
