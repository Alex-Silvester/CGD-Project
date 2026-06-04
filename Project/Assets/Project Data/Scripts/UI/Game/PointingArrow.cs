using UnityEngine;

public class PointingArrow : MonoBehaviour
{
	[Header("Cache")]
	[SerializeField] private Animator anim;
	
	private Transform target;
	
	private void Start()
	{
		// Find lorry (forklift spawned dynamically so can't be set in inspector)
		// Point at the front (marker) not the middle
		target = GameObject.Find("CrateCollector/Marker").transform;
		
	}
	
    private void Update()
    {
		if (!target)
			return;
		
		
		// Source - https://docs.unity3d.com/ScriptReference/Quaternion.LookRotation.html
        Vector3 relativePos = target.position - transform.position;

        // the second argument, upwards, defaults to Vector3.up
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = rotation;
    }
	
	public void Show()
	{
		if (anim)
			anim.SetBool("Show", true);
	}
	
	public void Hide()
	{
		if (anim)
			anim.SetBool("Show", false);
	}
}