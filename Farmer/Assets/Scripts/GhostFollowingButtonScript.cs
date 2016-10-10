using UnityEngine;

public class GhostFollowingButtonScript : MonoBehaviour
{
    private GameObject _ghostObject;
    public GameObject SellButtonPanel;

    void Update()
    {
        if (_ghostObject != null)
        { 
            Vector3 position = _ghostObject.transform.position;
            transform.position = Camera.main.WorldToScreenPoint(position) + new Vector3(0f, -50f);
        }
    }

    public void SetGhostObject(GameObject ghost)
    {
        _ghostObject = ghost;

        Building building = ghost.GetComponent<Building>();
        if (building != null)
        {
            SellButtonPanel.gameObject.SetActive(building.IsPlacedForReal);
        }
    }

    public void UnsetGhostObject()
    {
        _ghostObject = null;
        transform.position = Vector3.zero;
    }
}
