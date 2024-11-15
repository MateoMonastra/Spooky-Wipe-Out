using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : MonoBehaviour
{
    private List<IEnumerator> _preActions;
    private IEnumerator _action;
    private List<IEnumerator> _postActions;

    public Sequence()
    {
        _preActions = new List<IEnumerator>();
        _action = null;
        _postActions = new List<IEnumerator>();
    }

    public IEnumerator Execute()
    {
        foreach (var preAction in _preActions)
        {
            yield return preAction;
        }

        yield return _action;

        foreach (var postAction in _postActions)
        {
            yield return postAction;
        }
    }

    public void AddPreAction(IEnumerator action)
    {
        _preActions.Add(action);
    }

    public void SetAction(IEnumerator action)
    {
        _action = action;
    }

    public void AddPostAction(IEnumerator action)
    {
        _postActions.Add(action);
    }

    public void RemovePreAction(IEnumerator action)
    {
        _preActions.Remove(action);
    }

    public void RemovePostAction(IEnumerator action)
    {
        _postActions.Remove(action);
    }
}
