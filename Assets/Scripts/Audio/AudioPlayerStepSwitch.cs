using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerStepSwitch : MonoBehaviour
{
    [SerializeField] GameObject Player;
    
    
    // Start is called before the first frame update
    void Start()
    {
        setRotationAsCamera();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = Player.transform.position;
    }

    void setRotationAsCamera()
    {
        gameObject.transform.Rotate(0, -45, 0);
    }
}
