using UnityEngine;

/// <summary>
/// Represents a horizontal sliding door for the Vox bottom puzzle entrance.
/// Requires an Animator with a bool parameter named "Opening".
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class VoxDoor : MonoBehaviour
{
    private Animator _anim;
    private Collider2D _col;
    
    private void Awake() 
    {
        _anim = GetComponent<Animator>();
        _col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        _anim.SetBool("Opening", false);
        _col.enabled = true;
    }

    /// <summary>
    /// Open this door and disable its collider.
    /// </summary>
    public void Open()
    {
        _anim.SetBool("Opening", true);
        _col.enabled = false;
    }
}