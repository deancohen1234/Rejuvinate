using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour
{
    public Transform[] m_Backgrounds;
    private float[] parallaxScales;
    public float smoothing = 1.0f;

    private Transform cam;
    private Vector3 previousCamPos;

    private void Awake()
    {
        cam = Camera.main.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        previousCamPos = cam.position;

        parallaxScales = new float[m_Backgrounds.Length];
        for (int i = 0; i < m_Backgrounds.Length; i++)
        {
            parallaxScales[i] = m_Backgrounds[i].position.z * -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_Backgrounds.Length; i++)
        {
            float parallaxX = (previousCamPos.x - cam.position.x) * parallaxScales[i];
            float parallaxY = (previousCamPos.y - cam.position.y) * parallaxScales[i];

            float backgroundTargetPosX = m_Backgrounds[i].position.x + parallaxX;
            float backgroundTargetPosY = m_Backgrounds[i].position.y + parallaxY;

            m_Backgrounds[i].position = Vector3.Lerp(m_Backgrounds[i].position, new Vector3(backgroundTargetPosX, backgroundTargetPosY, m_Backgrounds[i].position.z), Time.deltaTime * smoothing);
        }

        previousCamPos = cam.position;
    }
}
