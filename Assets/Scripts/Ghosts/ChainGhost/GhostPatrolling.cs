using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Events;

public class GhostPatrolling : MonoBehaviour
{
    [SerializeField] private List<Transform> _waypoints;
    [SerializeField] private float _patrolingSpeed = 3f;

    public UnityEvent<bool> stopTired;

    private NavMeshAgent _agent;

    private int _currentWaypointIndex = 0;
    private float _patrolingSpeedDeltaTimed;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {
        _patrolingSpeedDeltaTimed = _patrolingSpeed * Time.deltaTime;

        stopTired?.Invoke(false);

        Walk();
    }

    private void Walk()
    {
        if (_agent == null)
        {
            return;
        }

        if(_waypoints.Count == 0)
        {
            return;
        }

        _agent.speed = _patrolingSpeedDeltaTimed;

        float distanceToWaypoint = Vector3.Distance(_waypoints[_currentWaypointIndex].position.IgnoreY(), transform.position.IgnoreY());

        if(distanceToWaypoint <= 2)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Count;
        }

        _agent.SetDestination(_waypoints[_currentWaypointIndex].position);
    }
}
