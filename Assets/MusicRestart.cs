using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicRestart : MonoBehaviour
{
    [SerializeField] SerializedInterface<IVacuumable> vacuumable;
}
[System.Serializable]
public class SerializedInterface<TInterface> : ISerializationCallbackReceiver
{
    [SerializeField] UnityEngine.Object reference;
    UnityEngine.Object _oldReference;
    public void OnAfterDeserialize() { }

    public void OnBeforeSerialize()
    {
        if(reference == default)
        {
            return;
        }
        if (reference is GameObject gameObject
            && gameObject.TryGetComponent(out TInterface newReference))
        {
            reference = newReference as UnityEngine.Object;
        }
        if (reference is TInterface)
        {
            _oldReference = reference;
        }
        else
        {
            reference = _oldReference;
        }
    }
}
