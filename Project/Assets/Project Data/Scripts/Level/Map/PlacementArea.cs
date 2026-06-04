using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    public class PlacementArea : MonoBehaviour, Interactable
    {
        public string MessageInteract => "Place In Area";
        [SerializeField]
        private GameObject cubePrefab;
        [SerializeField]
        int num_boxes = 0;
        class ObjectPosition
        {
            public Vector3 position;
            public bool taken;
        }
		
		[SerializeField]
		private bool requiresForklift = false;

        //defining an array the has the positions that each object will take
        List<ObjectPosition> positions = new List<ObjectPosition>();

        //used to get the index of a place in the positions list
        List<GameObject> objects = new List<GameObject>();

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            for (int i = 0; i < 27; i++)
            {
                positions.Add(new ObjectPosition());

                positions[i].position = new Vector3(i % 3, i / 9, (i / 3) % 3);
                positions[i].taken = false;
                objects.Add(Instantiate(cubePrefab));
                objects[i].transform.position = positionInArea(i, objects[i]);
                objects[i].SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {
           
            //for (int i = 0; i < num_boxes; i++)
            //{
            //    objects[i].SetActive(true);
            //}
            //
            //for (int i = 0; i < num_boxes; i++)
            //{
            //    objects[i].SetActive(false);
            //}

        }

        public virtual void Interact(InteractableControl interactableControl)
        {
            var pickupController = interactableControl.GetComponent<PickupController>();
            
            

            if (pickupController.HasPickupable)
            {
                Debug.Log("Placed");
                pickupController.currentPickupable.Destroy(pickupController);
                num_boxes += 1;
                objects[num_boxes -1].SetActive(true);

            }
            else
            {
                //Debug.Log("Taken");
                //num_boxes -= 1;
                //objects[num_boxes - 1].SetActive(true);
            }
       
        }

        //temporary fix, will probably add function for removing objects from
        //  the area manually and for when they fall off later when we can discuss
        //  how we want to implement it
        private Vector3 positionInArea(int idx, GameObject current_obj)
        {
            //      position in the area     placement object's position   total scale of the object in its heirarchy   the height needed to get the object out of the floor
            return positions[idx].position + gameObject.transform.position - (gameObject.transform.lossyScale / 2) + new Vector3(0, current_obj.transform.localScale.y / 2, 0);
        }

        public virtual void Release() { }

		public bool RequiresForklift()
		{
			return requiresForklift;
		}
    }
}
