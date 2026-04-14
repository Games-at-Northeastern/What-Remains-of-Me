using System.Collections;
using UnityEngine;
/// <summary>
///     A time-based item toggler.
/// </summary>
public class TimedToggle : AEventToggle
{
    [SerializeField]
    private bool _useUnscaledTime;
    [SerializeField]
    private bool _resetStateUponReactivation;
    [SerializeField]
    private float _duration;
    [SerializeField]
    private bool delay;
    [SerializeField]
    private float delayAmount;
    private Coroutine _coroutine;
    private bool _state;

    private bool delayComplete;


    private void OnEnable()
    {
        if (!delay) {
            _state = _enabledOnStart;
            _coroutine = StartCoroutine(IEFlip());
        } else {
            StartCoroutine(StartDelay());
        }
    }

    private void OnDisable()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
    }

    private IEnumerator IEFlip()
    {
        yield return _useUnscaledTime ? new WaitForSeconds(_duration) : new WaitForSecondsRealtime(_duration);

        _state = !_state; // flippy dippy doo

        FireEvent(_state);

        yield return IEFlip(); // loop IEnum
    }

    private IEnumerator StartDelay()
    {
        yield return _useUnscaledTime ? new WaitForSeconds(delayAmount) : new WaitForSecondsRealtime(delayAmount);

        _state = !_state; // flippy dippy doo

        delayComplete = true;
        _state = !_state;
        FireEvent(_state);

        yield return IEFlip();
    }
}
