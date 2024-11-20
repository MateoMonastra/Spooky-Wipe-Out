using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fsm_Mk2;
using Player.FSM;

public class AudioSteps : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject Player;
    [SerializeField] private PlayerAgent _playerAgent;
    [SerializeField] private InputReader _inputReader;

    public Vector2 direction;

    void Start()
    {
        _inputReader.OnMove += setWalkingSwitch;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = Player.transform.position;
    }

    public void setWalkingSwitch(Vector2 _direction)
    {
        direction = _direction;
        if (_direction == Vector2.zero)
        {
            AkSoundEngine.SetSwitch("isWalking", "No", gameObject);

        }
        else
        {
            AkSoundEngine.SetSwitch("isWalking", "Yes", gameObject);
        }
    }
}
