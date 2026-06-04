using UnityEngine;
using UnityEngine.UI;

public class ScrollTexture : MonoBehaviour
{
	[SerializeField] private RawImage rawImage;
	[SerializeField] private Vector2 scrollSpeed = new Vector2(0.1f, 0f);
	
	[SerializeField] private bool timeLimited;
	[SerializeField] private float scrollTime;
	
    private Vector2 offset;
	
	private void Update()
	{
		if (timeLimited && scrollTime <= 0)
			return;
		
        offset += scrollSpeed * Time.deltaTime;
        Rect r = rawImage.uvRect;
        r.position = offset;
        rawImage.uvRect = r;
		
		if (timeLimited)
		{
			scrollTime -= Time.deltaTime;
		}
	}
}