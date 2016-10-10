using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class SerializableBuilding : MonoBehaviour
{
    public Building Building;

    public float PositionX;
    public float PositionY;
    public float PositionZ;

    public float RotationX;
    public float RotationY;
    public float RotationZ;
    public float RotationW;

    public SerializableBuilding(Building building, Vector3 position, Quaternion rotation)
    {
        Building = building;

        PositionX = position.x;
        PositionY = position.y;
        PositionZ = position.z;

        RotationX = rotation.x;
        RotationY = rotation.y;
        RotationZ = rotation.z;
        RotationW = rotation.w;
    }
}
