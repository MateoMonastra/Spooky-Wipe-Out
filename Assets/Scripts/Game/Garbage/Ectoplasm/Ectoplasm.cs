using System;
using Game;
using UnityEngine;

public class Ectoplasm : MonoBehaviour, IWasheable
{
    public Action<Ectoplasm> OnBeingDestroy;
    private void Start()
    {     
        GameManager.GetInstance().ectoplasms.Add(this);
    }
    
    public void IsBeingWashed(params object[] args)
    {
        if (transform.localScale.x > (float)args[1] || transform.localScale.y > (float)args[1] || transform.localScale.z > (float)args[1])
        {
            Vector3 newScale = transform.localScale - Vector3.one * (float)args[0] * Time.deltaTime;
            
            newScale = new Vector3(
                Mathf.Max(newScale.x, (float)args[1]),
                Mathf.Max(newScale.y, (float)args[1]),
                Mathf.Max(newScale.z, (float)args[1])
            );
            
            transform.localScale = newScale;
        }
        else
        {
            gameObject.SetActive(false);
            OnBeingDestroy?.Invoke(this);
        }
    }
}
