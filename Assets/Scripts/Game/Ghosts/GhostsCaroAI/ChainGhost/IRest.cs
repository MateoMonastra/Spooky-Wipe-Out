using UnityEngine.Events;

public interface IRest
{
    public UnityEvent<bool> SetRestState { get; set; }
}
