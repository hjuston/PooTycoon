using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public class SerializableBuilding
{
    //public Building Building;

    public float PositionX;
    public float PositionY;
    public float PositionZ;

    public float RotationX;
    public float RotationY;
    public float RotationZ;
    public float RotationW;
    
    public int BuildingLevel;
    public SerializableUpgrade[] Upgrades;
    public bool IsPlacedForReal = false;
    public BuildingType BuildingType;
    public string BuildingName;

    public SerializableBuilding(Building building, Vector3 position, Quaternion rotation)
    {
        PositionX = position.x;
        PositionY = position.y;
        PositionZ = position.z;

        RotationX = rotation.x;
        RotationY = rotation.y;
        RotationZ = rotation.z;
        RotationW = rotation.w;


        BuildingLevel = building.BuildingLevel;

        // Zapisywanie upgradeów
        List<SerializableUpgrade> temp = new List<SerializableUpgrade>();
        foreach(Upgrade upgrade in building.Upgrades)
        {
            temp.Add(new SerializableUpgrade() { Name = upgrade.Name, HasBeenBought = upgrade.HasBeenBought });
        }
        Upgrades = temp.ToArray();


        IsPlacedForReal = building.IsPlacedForReal;
        BuildingType = building.BuildingType;
        BuildingName = building.Name;
    }
}
