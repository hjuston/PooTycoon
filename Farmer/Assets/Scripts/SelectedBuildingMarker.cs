using UnityEngine;
using System.Collections;

public class SelectedBuildingMarker : MonoBehaviour
{
   private GameObject _followedBuilding;

   void Update()
   {
      if (_followedBuilding != null)
      {
         Vector3 screenPosition = Camera.main.WorldToScreenPoint(_followedBuilding.transform.position);

         float positionAbove = 50f;

         Collider collider = _followedBuilding.GetComponent<Collider>();
         if(collider != null)
         {
            positionAbove = (_followedBuilding.transform.localRotation.y == 0 ||
                           _followedBuilding.transform.localRotation.y == 180 ||
                           _followedBuilding.transform.localRotation.y == -180) ? collider.bounds.size.x / 2 : collider.bounds.size.y / 2;
         }

         transform.position = screenPosition + new Vector3(0f, positionAbove);
      }
   }

   public void SetFollowedBuilding(GameObject building)
   {
      _followedBuilding = building;
   }

   public void UnsetFollowedBuilding()
   {
      _followedBuilding = null;
      transform.position = Vector3.zero;
   }
}
