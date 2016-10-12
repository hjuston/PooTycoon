using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildingProgressBar : MonoBehaviour
{
    public GameObject ProgressBar;
    public GameObject FullWarning;
    public Text ValueText; 
	
    public void SetValue(Building building)
    {
        float percentage = (float)building.CurrentShits / (float)building.MaximumShits;

        if (percentage > 1f) percentage = 1f;
        if (percentage < 0f) percentage = 0f;

        ProgressBar.transform.localScale = new Vector3(percentage, ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);
        ValueText.text = Helper.GetDisplayableValue(building.CurrentValue());

        // Jeżeli progress bar jest pełny to wyświetla obrazek
        if (percentage == 1f)
        {
            FullWarning.SetActive(true);
        }
        else
        {
            FullWarning.SetActive(false);
        }
    }


    // Śledzenie budynku
    private GameObject _followedBuilding;

    void Update()
    {
        if (_followedBuilding != null)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(_followedBuilding.transform.position);

            float positionAbove = -30f;

            transform.position = screenPosition + new Vector3(0f, positionAbove);
        }
    }

    public void SetFollowedBuilding(GameObject building)
    {
        _followedBuilding = building;

        Building buildingScript = building.GetComponent<Building>();
        if(buildingScript != null)
        {
            SetValue(buildingScript);
        }

    }

    public void UnsetFollowedBuilding()
    {
        _followedBuilding = null;
        transform.position = Vector3.zero;
    }

    public GameObject GetCurrentlyFollowingBuilding()
    {
        return _followedBuilding;
    }
}
