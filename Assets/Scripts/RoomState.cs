using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomState
{
    // inner class for individual obejct states
    public class ObjectState
    {
        // variables
        public string name;
        public Vector3 position;
        public Quaternion rotation;
        public bool isActive;

        // constructor
        public ObjectState(string name, Vector3 position, Quaternion rotation, bool isActive)
        {
            this.name = name;
            this.position = position;
            this.rotation = rotation;
            this.isActive = isActive;
        }
    }

    // variables
    public bool gravityWasReversed; // whether gravity was reversed when the player left
    public List<ObjectState> objectStates;

    // constructor
    public RoomState(bool gravityWasReversed, List<ObjectState> objectStates)
    {
        this.gravityWasReversed = gravityWasReversed;
        this.objectStates = objectStates;
    }
}
