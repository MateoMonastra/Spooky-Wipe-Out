using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Timer
{
    enum CountType
    {
        CountDown,
        CountUp
    };

    public class Timer : MonoBehaviour
    {
        public Action OnFinish;

        public TextMeshProUGUI textTimer;
        [SerializeField] private float startTime;
        [SerializeField] private float objectiveTime;
        [SerializeField] private CountType countType;

        private float _currentTime;
        private bool _finished;
        private const float Tolerance = 0.01f;

        private void Start()
        {
            _currentTime = startTime;
            _finished = false;
        }

        private void Update()
        {
            switch (countType)
            {
                case CountType.CountUp:
                    _currentTime += Time.deltaTime;
                    break;
                case CountType.CountDown:
                    _currentTime -= Time.deltaTime;
                    break;
            }

            textTimer.text = SetTimerText();


            if (Mathf.Abs(_currentTime - objectiveTime) < Tolerance)
            {
                _finished = true;
                OnFinish?.Invoke();
            }
        }

        public string SetTimerText()
        {
            string newText;

            int minutes = (int)_currentTime / 60;
            int seconds = (int)_currentTime % 60;

            if (seconds >= 10)
            {
                newText = minutes + ":" + seconds;
            }
            else
            {
                newText = minutes + ":0" + seconds;
            }

            return newText;
        }

        public float GetTime()
        {
            return _currentTime;
        }

        public bool GetIsFinished()
        {
            return _finished;
        }
    }
}