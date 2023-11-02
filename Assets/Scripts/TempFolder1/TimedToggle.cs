using System.Collections;
using UnityEngine;

/// <summary>
/// A time-based item toggler.
/// </summary>
public class TimedToggle : ATriggerToggle
{
    [SerializeField]
    private bool _useUnscaledTime = false;
    [SerializeField]
    private bool _resetStateUponReactivation = false;
    [SerializeField]
    private float _duration;

    private bool _state;
    private Coroutine _coroutine;



    private void OnEnable()
    {
        if (_resetStateUponReactivation)
        {
            _state = _enabledOnStart;
        }

        _coroutine = StartCoroutine(IEFlip());
    }

    private void OnDisable() => StopCoroutine(_coroutine);

    private IEnumerator IEFlip()
    {
        yield return _useUnscaledTime ? new WaitForSeconds(_duration) : new WaitForSecondsRealtime(_duration);

        _state = !_state; // flippy dippy doo

        FireEvent(_state);

        yield return IEFlip(); // loop IEnum
    }
}
