using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

//Used to sort a list of gameObjects based on their distance to a transform
public class DistanceCompare : IComparer<GameObject>
{
    private Transform positionTransform;

    public DistanceCompare(Transform transform)
    {
        positionTransform = transform;

    }

    // Compares by Height, Length, and Width.
    public int Compare(GameObject x, GameObject y)
    {
        Vector3 posA = x.transform.position;
        Vector3 posB = y.transform.position;

        return Vector3.Distance(posA, positionTransform.position).CompareTo(Vector3.Distance(posB, positionTransform.position));
    }
}


public class CratePickUp : MonoBehaviour
{
    public enum PickUpDirection {
        None,
        Right,
        Left,
        Forward
    }

    [SerializeField] GameObject PickupLocation;
    [SerializeField] Vector3 pickupPositionOffset;


    [SerializeField] List<GameObject> pickupList = new();

    [SerializeField] List<GameObject> heldObjects = new();

    [SerializeField] int maxObjects;

    [SerializeField] bool forkLiftSelected;

    [SerializeField] bool holdingForklift;

    [SerializeField] bool liftFull;

    [SerializeField] TMP_Text interactionUIText;

    [SerializeField] Transform leftPickUpOffset;

    [SerializeField] Transform rightPickUpOffset;

    [SerializeField] Transform forwardPickUpOffset;


    public UnityEvent onGrabbed = new UnityEvent();
    public UnityEvent onDropped = new UnityEvent();

    private DistanceCompare distanceCompare;

    public int heldObjectsCount
    {
        get { return heldObjects.Count; }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Awake()
    {
        forwardPickUpOffset.GetComponent<BoxCollider>().enabled = false;
        leftPickUpOffset.GetComponent<BoxCollider>().enabled = false;
        rightPickUpOffset.GetComponent<BoxCollider>().enabled = false;

        distanceCompare = new DistanceCompare(gameObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
        liftFull = heldObjects.Count == maxObjects;

        if (pickupList.Count == 0)
        {
            forkLiftSelected = false;
            return;
        }

        if (pickupList[0].tag == "Player")
        {
            forkLiftSelected = true;
        }
    }

    private void LateUpdate()
    {
        foreach (GameObject obj in heldObjects)
        {
            if (obj == null)
            {
                heldObjects.Remove(obj);
            }
        }

        foreach (GameObject obj in pickupList)
        {
            if (obj == null)
            {
                pickupList.Remove(obj);
            }
        }

        updatePositions();
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the other object is a box and the current player isn't holding a forklift
        if (other.tag == "Float" && !holdingForklift && other.GetComponent<ICollectable>().CanCollect)
        {
            interactionUIText.gameObject.SetActive(true);
            if (heldObjects.Count > 0 && !heldObjects.Contains(other.gameObject))
                interactionUIText.text = "<sprite name=\"Xbox_Y\"> to pick up\n<sprite name=\"Xbox_X\"> to drop";
            else if(!heldObjects.Contains(other.gameObject))
                interactionUIText.text = "<sprite name=\"Xbox_Y\"> to pick up";

            pickupList.Add(other.gameObject);
        }

        //if the other object is a player and the current player isn't holding any boxes
        if(other.tag == "Player" && heldObjects.Count == 0)
        {
            interactionUIText.gameObject.SetActive(true);
            interactionUIText.text = "<sprite name=\"Xbox_Y\"> to pick up";

            pickupList.Add(other.gameObject);
        }

        //sort the objects in the pickupList by their distance to the lift (close to far)
        pickupList.Sort(distanceCompare);
    }

    private void OnTriggerExit(Collider other)
    {
        pickupList.Remove(other.gameObject);

        if(!holdingForklift && heldObjects.Count == 0)
        {
            interactionUIText.gameObject.SetActive(false);
        }

        if (heldObjects.Count > 0)
            interactionUIText.text = "<sprite name=\"Xbox_X\"> to drop";
        if (holdingForklift)
            interactionUIText.text = "<sprite name=\"Xbox_X\"> to drop";
    }

    public void PickUpSelected()
    {
        foreach (GameObject obj in pickupList)
        {
            if (obj == null)
            {
                pickupList.Remove(obj);
            }
        }

        if (pickupList.Count == 0)
            return;

        if (forkLiftSelected || holdingForklift || !pickupList[0].GetComponent<ICollectable>().CanCollect)
            return;

        if (!liftFull && pickupList.Count > 0)
        {
            heldObjects.Add(pickupList[0]);          

            var heldCount = heldObjects.Count - 1;

            SetPositionInParent(heldObjects[heldCount].gameObject.transform, heldCount);

            if (heldObjects[heldCount].TryGetComponent<PhysicsPickup>(out var pickup))
            {
                pickup.OnGrabbed.Invoke();
                onGrabbed?.Invoke();
                pickupList.RemoveAt(0);
            }
        }

        if (heldObjects.Count > 0)
            interactionUIText.text = "<sprite name=\"Xbox_X\"> to drop";
        if (holdingForklift)
            interactionUIText.text = "<sprite name=\"Xbox_X\"> to drop";
    }

    public void PickUpSelectedForklift()
    {
        if(!forkLiftSelected || heldObjects.Count > 0)
            return;

        var Angle = CalculateAngleOfPickup(pickupList[0]);

        pickupList[0].GetComponent<Rigidbody>().useGravity = false;
        heldObjects.Add(pickupList[0]);
        pickupList.Remove(pickupList[0]);
        SetForkliftPostitionInParent(Angle);

        holdingForklift = true;

        onGrabbed?.Invoke();
    }

    public void DropHeld()
    {
        foreach (GameObject obj in heldObjects)
        {
            if (obj == null)
            {
                heldObjects.Remove(obj);
            }
        }

        var heldCount = heldObjects.Count - (heldObjects.Count == 0 ? 0 : 1);

        if (heldObjects.Count == 0)
        {
            Debug.LogWarning("No objects to drop");
            //turn off lights when there are no crates
            onDropped?.Invoke();
            return; 
        }

        if (heldObjects[heldCount].TryGetComponent<PhysicsPickup>(out var pickup))
        {
            pickup.OnDropped.Invoke();

            onDropped?.Invoke();
        }

        if (heldObjects[0].tag == "Player")
        {
            Debug.LogWarning("Dropping player");
            holdingForklift = false;
            heldObjects[0].GetComponent<BoxCollider>().enabled = true;
            heldObjects[0].GetComponent<DrivingController>().togglePlayerLifted();

            onDropped?.Invoke();
        }


        heldObjects[heldCount].GetComponent<Rigidbody>().isKinematic = false;

        forwardPickUpOffset.GetComponent<BoxCollider>().enabled = false;
        leftPickUpOffset.GetComponent<BoxCollider>().enabled = false;
        rightPickUpOffset.GetComponent<BoxCollider>().enabled = false;

        UnsetPositionInParent(heldObjects[heldCount].gameObject.transform, heldCount);
        heldObjects.Remove(heldObjects[heldCount]);

        if(pickupList.Count > 0)
        {
            interactionUIText.text = "<sprite name=\"Xbox_Y\"> to pick up";
        }
        else if(pickupList.Count > 0 && heldObjects.Count > 0)
        {
            interactionUIText.text = "<sprite name=\"Xbox_Y\"> to pick up\n<sprite name=\"Xbox_X\"> to drop";
        }
        else if(heldObjects.Count > 0)
        {
            interactionUIText.text = "<sprite name=\"Xbox_X\"> to drop";
        }
        else
        {
            interactionUIText.text = "";
        }
        
        //Swap Feature (if we have something in our pickup radius and we are holding something, drop what we are holding and pick up the new object) To be added if we feel its needed
        //else if (pickupList.Count > 0)
        //{
        //    if (heldObjects.Count == 0)
        //        return;
        //    else if (heldObjects.Count > 0)
        //    {
        //        if (heldObjects[0].TryGetComponent<PhysicsPickup>(out var pickup))
        //        {
        //            Debug.Log("Invoking Drop");
        //            pickup.OnDropped.Invoke();
        //            onDropped?.Invoke();
        //        }
        //        var heldCount = heldObjects.Count - 1;
        //        UnsetPositionInParent(heldObjects[heldCount].gameObject.transform, heldCount);
        //        heldObjects.Remove(heldObjects[heldCount]);
        //        heldCount--;
        //        PickUpSelected();
        //    }
        //}
    }

    private void updatePositions()
    {
        if (heldObjects.Count == 0) return;
        if (heldObjects[0].tag == "Player") return;

        for(int i = 0; i < heldObjects.Count; i++)
        {
            SetPositionInParent(heldObjects[i].transform, i);
        }
    }

    public void SetPositionInParent(Transform newPosition, int heldcount)
    {
            pickupPositionOffset = new Vector3(0, heldObjects[0].transform.lossyScale.y * heldcount, 0);

            newPosition.parent = PickupLocation.transform;
            newPosition.transform.position = PickupLocation.transform.position + pickupPositionOffset;
            newPosition.transform.rotation = PickupLocation.transform.rotation;
            newPosition.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void UnsetPositionInParent(Transform newPosition, int heldcount)
    {
        if (heldObjects[0].gameObject.tag == "Player")
        {
            heldObjects[0].GetComponent<Rigidbody>().useGravity = true;
            heldObjects[0].GetComponent<DrivingController>().togglePlayerLifted();
        }
        newPosition.parent = null;
        newPosition.GetComponent<Rigidbody>().isKinematic = false;

        newPosition.GetComponent<Collider>().enabled = true;
    }

    public PickUpDirection CalculateAngleOfPickup(GameObject otherGameObject)
    {
        float sign = Mathf.Sign(Vector3.Dot(transform.forward, otherGameObject.transform.right));
        float angle = Mathf.Acos(Vector3.Dot(transform.forward, otherGameObject.transform.forward)) * 180f/Mathf.PI * sign;

        if((-45 <= angle && angle <= 45))
        {
            return PickUpDirection.Forward;
        }

        if(45 < angle && angle <= 135)
        {
            return PickUpDirection.Left;
        }

        if(-135 <= angle && angle < -45)
        {
            return PickUpDirection.Right;
        }
    
        return PickUpDirection.None;

    }

    public void SetForkliftPostitionInParent(PickUpDirection direction)
    {
        GameObject otherPlayer = heldObjects[0].gameObject;
        otherPlayer.GetComponent<DrivingController>().togglePlayerLifted();
        otherPlayer.GetComponent<Rigidbody>().isKinematic = true;
        otherPlayer.GetComponent<BoxCollider>().enabled = false;

        if(direction == PickUpDirection.Left)
        {
            otherPlayer.transform.parent =   leftPickUpOffset;
            otherPlayer.transform.position = leftPickUpOffset.position;
            otherPlayer.transform.rotation = leftPickUpOffset.rotation;

            leftPickUpOffset.GetComponent<BoxCollider>().enabled = true;
        }
        else if(direction == PickUpDirection.Forward)
        {
            otherPlayer.transform.parent =   forwardPickUpOffset;
            otherPlayer.transform.position = forwardPickUpOffset.position;
            otherPlayer.transform.rotation = forwardPickUpOffset.rotation;

            forwardPickUpOffset.GetComponent<BoxCollider>().enabled = true;
        }
        else if (direction == PickUpDirection.Right)
        {
            otherPlayer.transform.parent =   rightPickUpOffset;
            otherPlayer.transform.position = rightPickUpOffset.position;
            otherPlayer.transform.rotation = rightPickUpOffset.rotation;

            rightPickUpOffset.GetComponent<BoxCollider>().enabled = true;
        }

    }
}

