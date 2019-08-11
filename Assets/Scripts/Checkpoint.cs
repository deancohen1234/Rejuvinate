using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    //order in which checkpoints are obtaining
    //after obtaining a checkpoint you cannot go backwards and activate previous checkpoint
    //NOTE "I will deal with ordering later" (Dean)

    public ParticleSystem m_CheckpointParticles;
    public uint m_Order;
    public string m_PlayerLayerName = "Player";

    private CheckpointManager.CheckpointEvent m_MainCheckpointEvent;
    private Vector3 m_Position;

    public void Initialize(CheckpointManager.CheckpointEvent checkpointEvent)
    {
        m_MainCheckpointEvent = checkpointEvent;

        m_Position = transform.position;

        if (m_CheckpointParticles)
        {
            m_CheckpointParticles.Stop();
        }
    }

    public Vector3 GetPosition()
    {
        return m_Position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (m_MainCheckpointEvent != null)
            {
                m_MainCheckpointEvent(this);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == m_PlayerLayerName)
        {
            //invoke checkpoint for manager
            m_MainCheckpointEvent(this);

            other.gameObject.GetComponent<PlayerEssenceController>().RestoreAllEssence();
            OnCheckpointActivated();
        }
    }

    private void OnCheckpointActivated()
    {
        if (m_CheckpointParticles == null) { return; }

        m_CheckpointParticles.Emit(20);
        m_CheckpointParticles.Play();
    }


}
