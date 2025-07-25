using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class GhostRest : MonoBehaviour, IRest
{
    public UnityEvent<bool> OnTired;
    
    private NavMeshAgent _agent;
    public bool isRested;

    [SerializeField] private float restDuration = 3f;

    private Coroutine _resting;

    public UnityEvent<bool> SetRestState { get; set; } = new UnityEvent<bool>();

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        Debug.Log("enter rest state");

        if(_resting != null)
        {
            StopCoroutine(_resting);
        }
        
        _resting = StartCoroutine(Rest());
    }

    private IEnumerator Rest()
    {
        _agent.isStopped = true;
        OnTired?.Invoke(true);
        float timer = 0.0f;

        while(!isRested && timer < restDuration) 
        {
            timer += Time.deltaTime;
            yield return null;
        }

        _agent.isStopped = false;
        OnTired?.Invoke(false);

        SetRestState.Invoke(true);
        isRested = true;

        Debug.Log("finish rest");
    }

    private void OnDisable()
    {
        if(_resting != null)
            StopCoroutine(_resting);
    }
}
