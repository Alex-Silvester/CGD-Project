using UnityEngine;
using UnityEngine.Events;

public class TrafficCone : MonoBehaviour
{
	private static readonly int TOTAL_CONES = 20;
	private static int totalConesHit = 0;
	
	public static UnityEvent onAllHit = new UnityEvent();
	
	// Prevent the same cone being hit repeatedly for the achievement
	bool hit = false;
	
	private void Start()
	{
		// Reset cone hits between playsessions / levels
		totalConesHit = 0;
	}
	
	private void OnCollisionEnter(Collision other)
	{
		if (hit)
			return;
		
		if (other.gameObject.CompareTag("Player"))
		{
			totalConesHit++;
						
			hit = true;
			
			if (totalConesHit == TOTAL_CONES)
				onAllHit?.Invoke();
		}
	}
}