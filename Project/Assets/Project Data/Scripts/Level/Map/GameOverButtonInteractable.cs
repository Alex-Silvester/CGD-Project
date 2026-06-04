using UnityEngine;
using Interaction;

public class GameOverButtonInteractable : MonoBehaviour, Interactable
{
	public delegate void OnPressed();
	public static event OnPressed onPressed;
	
    public virtual string MessageInteract => "Press <sprite name=\"Xbox_X\"> to self-destruct warehouse";

	[SerializeField]
	private bool requiresForklift = false;

    public void Interact(InteractableControl interactableControl)
    {
		// Let other scripts know
        if (onPressed != null)
			onPressed();
    }

    public virtual void Release() { }
	
	public bool RequiresForklift()
	{
		return requiresForklift;
	}
}