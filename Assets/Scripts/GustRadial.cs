using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GustRadial : MonoBehaviour
{
    public PlayerController m_PlayerController;
    public float m_LerpSpeed = 5.0f;

    private SpriteRenderer m_SpriteRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (m_PlayerController == null)
        {
            Debug.LogError("No Player Controller Assigned for Gust Radial!");
            return;
        }

        float rx = Input.GetAxis("RStickHorizontal");
        float ry = Input.GetAxis("RStickVertical");

        if (transform.parent.GetComponent<PlayerController>().IsStickOnOuterRim(new Vector2(rx, ry)))
        {
            Color color = m_SpriteRenderer.color;
            color.a = 1;
            m_SpriteRenderer.color = color;
        }
        else
        {
            Color color = m_SpriteRenderer.color;
            color.a = 0;
            m_SpriteRenderer.color = color;
        }

        float angle = Mathf.Atan2(-ry, rx); //in radians
        Quaternion destinationRotation = Quaternion.Euler(new Vector3(0, 0, (Mathf.Rad2Deg * (angle - Mathf.PI / 2))));
        Quaternion desiredQuaternion = Quaternion.Lerp(transform.localRotation, destinationRotation, Time.unscaledDeltaTime * m_LerpSpeed);

        transform.localRotation = desiredQuaternion;
    }
}
