using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public Camera m_StableCamera;

    public float m_PerlinScale = 5f;
    public float m_MaxAngle = 10f;
    [Range(.01f, .99f)]
    public float m_FalloffSpeed = .5f;

    private float m_Trauma = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddTrauma(.1f); //trauma between 0 and 1
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            AddTrauma(.5f); //trauma between 0 and 1
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            AddTrauma(.9f); //trauma between 0 and 1
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            TestPerlinNoise();
        }

        AddCameraShake(); //always updates shake

    }

    private void TestPerlinNoise()
    {
        float perlinValue = Mathf.PerlinNoise((1 / 128 * m_PerlinScale) + Time.time, (1 / 128 * m_PerlinScale) + Time.time);

        Debug.Log((perlinValue * 2) - 1);
    }

    private void AddCameraShake()
    {
        float shake = m_Trauma;
        /*
        float x = m_MaxAngle * shake * Random.Range(-1, 1);
        float y = m_MaxAngle * shake * Random.Range(-1, 1);
        float z = m_MaxAngle * shake * Random.Range(-1, 1);*/

        float perlinValueX = Mathf.PerlinNoise((1 / 128 * m_PerlinScale) + Time.time, (1 / 128 * m_PerlinScale) + Time.time);
        float perlinValueY = Mathf.PerlinNoise((1 / 128 * m_PerlinScale) + Time.time, (1 / 128 * m_PerlinScale) + Time.time + 1);
        float perlinValueZ = Mathf.PerlinNoise((1 / 128 * m_PerlinScale) + Time.time, (1 / 128 * m_PerlinScale) + Time.time + 2);

        float x = m_MaxAngle * shake * ((perlinValueX * 2) - 1); //puts perlin between -1, 1
        float y = m_MaxAngle * shake * ((perlinValueY * 2) - 1); //puts perlin between -1, 1
        float z = m_MaxAngle * shake * ((perlinValueZ * 2) - 1); //puts perlin between -1, 1
        //Quaternion q = Quaternion.Euler(x, y, transform.rotation.eulerAngles.z);
        Vector3 v = new Vector3(x, y, 0);

        //GetComponent<Camera>().transform.rotation = q * m_StableCamera.transform.rotation; //potentially could just be a vector not a camera
        GetComponent<Camera>().transform.position = m_StableCamera.transform.position + v;

        if (m_Trauma > 0)
        {
            m_Trauma *= m_FalloffSpeed; //to not make the falloff for trauma be linear
            //m_Trauma -= 0.05f; //linear

            if (m_Trauma <= 0)
            {
                m_Trauma = 0;
            }
        }

    }

    public void AddTrauma(float value)
    {
        if (m_Trauma + value > 1)
        {
            return;
        }
        m_Trauma += value;
    }
}
