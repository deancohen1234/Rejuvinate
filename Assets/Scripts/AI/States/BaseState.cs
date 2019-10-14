using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//eventually allow time for each switch if possible
public class BaseState : IAIState
{
    public string m_Name;

    protected System.Action<string> m_SwapStateCallback;

    public void Initialize(System.Action<string> swapStateCallback)
    {
        m_SwapStateCallback = swapStateCallback;
    }

    public virtual void OnEnter(GameObject enemy)
    {
        Debug.Log("Virtual Enter Running");
    }

    public virtual void OnUpdate(GameObject enemy)
    {
        Debug.Log("Virtual Update Running");
    }

    public virtual void OnExit(GameObject enemy)
    {
        Debug.Log("Virtual Exit Running");
    }
}

public interface IAIState
{
    void OnEnter(GameObject enemy);
    void OnUpdate(GameObject enemy);
    void OnExit(GameObject enemy);
}
