using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class GhostRest : MonoBehaviour, IRest
{
    private NavMeshAgent _agent;

    [SerializeField] private float restDuration = 3f;

    private Coroutine _resting;

    public UnityEvent<bool> SetRestState { get; set; } = new UnityEvent<bool>();

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        if(_resting != null)
        {
            StopCoroutine(_resting);
        }
        
        _resting = StartCoroutine(Rest());
    }

    private IEnumerator Rest()
    {
        _agent.isStopped = true;

        yield return new WaitForSeconds(restDuration);

        _agent.isStopped = false;

        SetRestState.Invoke(true);

        Debug.Log("finish rest");
    }

    private void OnDisable()
    {
        StopCoroutine(_resting);
    }
}
