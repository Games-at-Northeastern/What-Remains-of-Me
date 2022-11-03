using UnityEngine;

namespace SmartScriptableObjects
{
	/// <summary>
	/// Base class for ScriptableObjects that need a description field in the inspector.
	/// </summary>
	public class DescriptionBaseSO : ScriptableObject
	{
		[TextArea, SerializeField] private string description;
	}
}
