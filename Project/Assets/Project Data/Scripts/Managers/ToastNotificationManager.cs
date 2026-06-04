using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToastNotificationManager : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private float showDuration = 3f;
	
	[Header("Cache")]
	[SerializeField] private TMP_Text titleText;
	[SerializeField] private TMP_Text descriptionText;
	[SerializeField] private Animator anim;
	
	private Queue<ToastNotificationData> toastNotifications = new Queue<ToastNotificationData>();
	
	// Singleton
	public static ToastNotificationManager instance;
	
	private void Start()
	{
		// Singleton
		if (instance)
			Destroy(gameObject);
		else
			instance = this;
		
		// Persist between scenes
		DontDestroyOnLoad(gameObject);
		
		// Continually check for notifications
		StartCoroutine(ToastNotificationCoroutine());
	}
	
	private IEnumerator ToastNotificationCoroutine()
	{
		while (true)
		{
			if (toastNotifications.Count > 0)
			{
				ToastNotificationData data = toastNotifications.Dequeue();
				
				titleText.text = data.title;
				descriptionText.text = data.description;
				
				anim.SetTrigger("Show");
				
				yield return new WaitForSeconds(showDuration);
				
				anim.SetTrigger("Hide");
			}
			
			// Don't check every frame for better performance
			yield return new WaitForSeconds(1f);
		}
	}
	
	#region Setters
	
	public void Add(string title, string description)
	{
		ToastNotificationData data = new ToastNotificationData();
		data.title = title;
		data.description = description;
		
		toastNotifications.Enqueue(data);
	}
	
	#endregion Setters
}

public class ToastNotificationData
{
	public string title;
	public string description;
}