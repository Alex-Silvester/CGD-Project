using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class BackMenuManager : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private BackData[] backData;
	[SerializeField] private string rootBackScene;
	
	private void Update()
	{
        if (Gamepad.current.buttonEast.wasPressedThisFrame)
        {
			GoBack();
        }
	}
	
	private void GoBack()
	{
		// Prioritise game object panels before the root back scene
		// For example, if the Warehouse 1-1 is up, do that instead of going back to Lobby Menu
		
		// Check all the potential panels to see if a valid one is active
		foreach(var data in backData)
		{
			// Is the required panel open?
			if (data.activePanel.activeInHierarchy)
			{
				// Deactivate current panel
				data.activePanel.SetActive(false);
				
				// Activate back panel
				data.backPanel.SetActive(true);
				
				// Select the first button in the activated panel, otherwise the gamepad won't be selected
				EventSystem.current.SetSelectedGameObject(data.backPanelFirstButton);
				
				// We've found what we were looking for
				// Don't continue looking for more go back opportunities
				return;
			}
		}

		if (FindAnyObjectByType<LoadingVariables>())
			FindAnyObjectByType<LoadingVariables>().allowVariableDestruction();
		
		// If we didn't find a valid active panel, it means we are at the root
		// Therefore, go back to the previous scene
		SceneManager.LoadScene(rootBackScene);
	}
}

[System.Serializable]
public struct BackData
{
	public GameObject activePanel;
	
	public GameObject backPanel;
	public GameObject backPanelFirstButton;
}