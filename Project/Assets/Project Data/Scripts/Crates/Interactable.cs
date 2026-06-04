using System;

namespace Interaction
{
    public interface Interactable
    {
        public string MessageInteract { get; }
        public void Interact(InteractableControl interactableControl);
		public bool RequiresForklift();

        public void Release();
    }
}

