using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace Interaction
{
    public class InteractableControl : MonoBehaviour
    {
        [SerializeField]
        Camera playerCamera;

        [SerializeField]
        TextMeshProUGUI interactText;

        [SerializeField]
        Canvas canvas;

        [SerializeField]
        public float interactDistance = 3f;

        public Interactable currentTargetedInteraction;

        [SerializeField] InputActionReference input_action;

        public void Start()
        {
            interactText.text = "Test";

            input_action.action.canceled+= context => 
            {
                Debug.Log("Released");
                if (currentTargetedInteraction != null)
                    currentTargetedInteraction.Release();
            };

        }

        public void Update()
        {
            UpdateCurrentInteratable();

            UpdateInteractText();
        }

        void UpdateCurrentInteratable()
        {
            var ray = playerCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));

            Physics.Raycast(ray, out var hit, interactDistance);

			// Make sure it's an object the player can pickup
			Interactable interactable = hit.collider?.GetComponent<Interactable>();
			
			if (interactable != null && interactable.RequiresForklift() == false)
			{
			    currentTargetedInteraction = hit.collider?.GetComponent<Interactable>();
			}
			else
			{
				currentTargetedInteraction = null;
			}
        }
        void UpdateInteractText()
        {
            if (currentTargetedInteraction == null)
            {
                interactText.text = string.Empty;
                return;
            }
            interactText.text = currentTargetedInteraction.MessageInteract;
        }
        
        public void OnInteract()
        {
            if (currentTargetedInteraction != null)
                currentTargetedInteraction.Interact(this);
            else
                GetComponent<PickupController>().CheckDropInput();
        }
    }
}
