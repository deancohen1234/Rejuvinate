using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public delegate void CheckpointEvent(Checkpoint checkpoint);
    public CheckpointEvent m_OnCheckpointActivated;

    public Checkpoint[] m_AllCheckpoints;
    public PlayerEssenceController m_EssenceController;

    private Checkpoint m_ActiveCheckpoint = null;

    // Start is called before the first frame update
    void Start()
    {
        //subscribe manager functions to events
        m_OnCheckpointActivated += OnCheckpointActivated;
        m_EssenceController.m_OnPlayerDeath += OnPlayerDeath;

        SetupCheckpoints();
    }

    private void SetupCheckpoints()
    {
        //set so whenever a checkpoint is activated the manager is notified
        for (int i = 0; i < m_AllCheckpoints.Length; i++)
        {
            m_AllCheckpoints[i].Initialize(m_OnCheckpointActivated);
        }
    }

    private void OnCheckpointActivated(Checkpoint checkpoint)
    {
        Debug.Log("Checkpoint activated + " + checkpoint.m_Order);

        //set active checkpoint
        m_ActiveCheckpoint = checkpoint;
    }

    private void OnPlayerDeath()
    {
        if (m_ActiveCheckpoint == null)
        {
            Debug.Log("Player does not have active checkpoint");
            return;
        }

        m_EssenceController.gameObject.transform.position = m_ActiveCheckpoint.GetPosition();
        m_EssenceController.RestoreAllEssence();
    }
}
