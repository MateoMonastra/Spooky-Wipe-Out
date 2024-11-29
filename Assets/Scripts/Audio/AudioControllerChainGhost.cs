using Ghosts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioControllerChainGhost : MonoBehaviour
{
    private ChainGhostAgent _parentAgent;
    [SerializeField] private AK.Wwise.Event chainGhostOnFlee;
    [SerializeField] private AK.Wwise.Event destroyedGhost;

    private void Awake()
    {
        // Get the parent ChainGhostAgent component
        _parentAgent = GetComponentInParent<ChainGhostAgent>();
    }

    private void OnEnable()
    {
        if (_parentAgent != null)
        {
            _parentAgent.OnFlee += Flee;
        }
    }

    private void OnDisable()
    {
        if (_parentAgent != null)
        {
            _parentAgent.OnFlee += Flee;
        }
    }

    
    private void Flee()
    {
        chainGhostOnFlee.Post(this.gameObject);
    }
        
}
    

