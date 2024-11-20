using UnityEngine;
using UnityEngine.UI;

public class SlideUI : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _backButton;

    private bool _isInFirst = true;


    private void Start()
    {
        _nextButton.onClick.AddListener(NextAnimation);
        _backButton.onClick.AddListener(BackAnimation);
    }

    private void NextAnimation()
    {
        if(!_isInFirst)
        _animator.SetTrigger("Next");
        _isInFirst = true;
    }

    private void BackAnimation()
    {
        if(_isInFirst)
        _animator.SetTrigger("Back");
        _isInFirst = false;
    }
}
