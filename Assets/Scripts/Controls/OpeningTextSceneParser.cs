using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UIElements;
public class OpeningTextSceneParser : MonoBehaviour
{
    public InputActionReference binding;

    public UnityEvent listener;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnEnable() => binding.action.Enable();

    void Update()
    {
        binding.action.performed += ctx => Activate();
    }

    private void Activate()
    {
        listener.Invoke();
    }
}
