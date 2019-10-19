using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public delegate void CheckpointEvent(Checkpoint checkpoint);
    public CheckpointEvent m_OnCheckpointActivated;

    public Checkpoint[] m_AllCheckpoints;
    public PlayerController m_Player;

    private Checkpoint m_ActiveCheckpoint = null;

    // Start is called before the first frame update
    void Start()
    {
        SetupCheckpoints();
    }

    private void OnEnable()
    {
        //subscribe manager functions to events
        m_OnCheckpointActivated += OnCheckpointActivated;
        m_Player.GetComponent<HealthComponent>().m_OnPlayerRespawn += OnPlayerRespawn;
    }

    private void OnDisable()
    {
        //unsubscribe manager functions to events

        //TODO m_Player  line throws error on closing game

        //m_OnCheckpointActivated -= OnCheckpointActivated;
        //m_Player.GetComponent<HealthComponent>().m_OnPlayerRespawn -= OnPlayerRespawn;
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
        Debug.Log("Checkpoint activated: " + checkpoint.m_Order);

        //set active checkpoint
        m_ActiveCheckpoint = checkpoint;
        m_Player.GetEssenceController().RestoreAllEssence();
    }

    private void OnPlayerRespawn()
    {
        if (m_ActiveCheckpoint == null)
        {
            Debug.Log("Player does not have active checkpoint");
            return;
        }

        m_Player.gameObject.transform.position = m_ActiveCheckpoint.GetPosition();
        m_Player.GetEssenceController().RestoreAllEssence();
    }
}
