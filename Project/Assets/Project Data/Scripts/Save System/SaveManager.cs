using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages saving and loading of save data
/// Uses built-in Unity XML serializer
/// Tries to load any existing save file when game starts
/// Save will be called after a level is complete
/// Uses Singleton design pattern so there is only ever one
/// It can be accessed from anywhere with SaveSystem.instance
/// Using a Singleton instead of static means we can use the inspecctor but still get the benefits of being static (such as only ever being one and accessed without instance reference)
/// Can save any data type Unity can serialize (e.g. string, int, float, enum, lists)
/// Based on code from this tutorial - https://www.udemy.com/course/unity-save-system/
/// </summary>
public class SaveManager : MonoBehaviour
{
	// Name of save file (no file extension needed)
	private readonly string saveFileName = "save1";
	// File extension of the save file. Recommend three or less characters for compatability.
	private readonly string saveFileExtension = ".sav";
	
	// Singleton design pattern
	// Allows the save system to be accessed from anywhere without an instance reference. E.g. SaveSystem.instance.TheFunction()
	// https://gamedevbeginner.com/singletons-in-unity-the-right-way/
	public static SaveManager instance;
	
	// In case we want to add multiple save slots in the future
	[HideInInspector] // Needs to be accessed by other scripts but not in the inspector
	public SaveData currentSaveData;
	
	// Events
	public static UnityEvent onSaved = new UnityEvent();
	// Other scripts can update their values after loading
	public static UnityEvent onLoaded = new UnityEvent();
	
	/// <summary>
	/// Setup a Singeton so there is only one SaveManager that can be acccessed in any other script
	/// </summary>
	private void Awake()
	{
		// Check for existing Singleton
		if (instance == null)
		{
			// Assign Singleton
			instance = this;
			
			// Survive scene change
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			// There can only be one save manager
			Destroy(this);
			
			Debug.LogWarning("Tried to create a second Save Manager. Only one is allowed.");
		}
	}
	
	private void Start()
	{
		// Load any existing save file when the game starts
		Load();
	}
	
	#region Saving and Loading
	
	/// <summary>
	/// Convert save data variables to an XML file in the player's save data folder
	/// </summary>
	public void Save()
	{
		// Create XML Serializer that serializes from the data type of SaveData
        var serializer = new XmlSerializer(typeof(SaveData));
		
		// Create data to save file with data stream
		// Parameter is save file location e.g. "%userprofile%\AppData\LocalLow\RuntimeError\CGD-Project" (requires show hidden folders)
		// https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Application-persistentDataPath.html
		// FileMode.Create will overwrite an existing save
		var stream = new FileStream(Application.persistentDataPath + "/" + saveFileName + saveFileExtension, FileMode.Create);
		
		// Convert SaveData variable into XML
		// First parameter is where and how, second is what
		serializer.Serialize(stream, currentSaveData);
		
		// Close file so Windows doesn't think it's still in use
		stream.Close();
		
		// Let other scripts know we just finished saving
		onSaved?.Invoke();
	}
	
	/// <summary>
	/// Load save data from an XML file and use it to fill in save data variables
	/// </summary>
	private void Load()
	{
		// Does a save file exist?
		if (DoesSaveExist())
		{
			// Create XML Serializer that serializes from the data type of SaveData
			var serializer = new XmlSerializer(typeof(SaveData));
			
			// Load save file to SaveData with data stream
			// Parameter is save file location
			var stream = new FileStream(Application.persistentDataPath + "/" + saveFileName + saveFileExtension, FileMode.Open);
			
			// Use XML to fill in save data variable
			// First parameter is where and how, second is what
			currentSaveData = serializer.Deserialize(stream) as SaveData;
			
			// Close file so Windows doesn't think it's still in use
			stream.Close();
			
			// Let other scripts know we just finished loading
			onLoaded?.Invoke();
			
			// Init values based on the data we just loaded
			AfterLoadSetup();
		}
	}
	
	/// <summary>
	/// Initialise anything that needs to be setup based on the data we just loaded
	/// Alternatively, the onLoaded UnityEvent can be used by subscribing in code
	/// This function is for small things that don't have their own script or manager
	/// </summary>
	private void AfterLoadSetup()
	{
		
	}
	
	#endregion Saving and Loading
	
	#region Getters
	
	/// <summary>
	/// Returns whether there is an existing save file
	/// </summary>
	public bool DoesSaveExist()
	{
		return (File.Exists(Application.persistentDataPath + "/" + saveFileName + saveFileExtension));
	}
	
	#endregion Getters
	
	#region Setters
	
	/// <summary>
	/// Delete any existing save file.
	/// Careful using this as there is no confirm dialog (you need to make your own then call this)
	/// </summary>
	public void ClearSave()
	{
		// Delete Unity PlayerPrefs
		PlayerPrefs.DeleteAll();
		
		// Delete save file
		File.Delete(Application.persistentDataPath + "/" + saveFileName + saveFileExtension);
		
		// Delete data in memory
		currentSaveData = new SaveData();
	}
	
	#endregion Setters
}