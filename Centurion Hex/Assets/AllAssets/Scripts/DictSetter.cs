using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictSetter : MonoBehaviour
{
    //List of all the keys to set, and the datatypes and values to set them to. Designed to be set from the inspector.
    public List<KeyTypeValueTuple> keysToSet;
    //True if it's supposed to set values for all the child objects.
    //For example, you might create one DictSetter for water tiles and place it at the origin,
    //then create a bunch of children where their positions mark the actual water tiles.
    //And if you want to change how water works, you only change the parent object.
    //False if it's just for this one object.
    public bool operateOnChildren;
    //True if you want to destroy the object once it's loaded.
    //Use it if you're only creating the object to set values for that position, and don't want it to actually exist in gameplay.
    //Set it to false if you still want the object running this script to exist. Like a rock that you set to be an obstacle.
    public bool destroyOnLoad;
    //True if you want to center the object on load.
    //Maybe you placed a rock in roughly the right place, but want it moved to the exact center of the tile.
    public bool centerOnLoad;
    // Start is called before the first frame update
    void Start()
    {
        if(operateOnChildren) {
            foreach(Transform child in transform) {
                RunOn(child);
            }
        } else {
            RunOn(transform);
        }
        if(destroyOnLoad) {
            Destroy(gameObject);
        }
        Destroy(this);      //Even if we keep the object, we don't need to keep this script.
    }
    private void RunOn(Transform t) {
        HexPosition pos = new HexPosition(t.position);
        foreach(KeyTypeValueTuple ktv in keysToSet) {
            object value;
            switch(ktv.type) {
                case datatype.Flag:
                    if(ktv.value.ToLower() == "false")      //If you type false in there, it shouldn't set the flag.
                        continue;
                    value = null;
                    break;
                case datatype.String:
                    value = ktv.value;
                    break;
                case datatype.Int:
                    value = int.Parse(ktv.value);
                    break;
                case datatype.Double:
                    value = double.Parse(ktv.value);
                    break;
                default:
                    throw new System.Exception("ERROR: Type '" + ktv.type + "' not found.");
            }
            pos[ktv.key] = value;
        }
        if(centerOnLoad) {
            t.position = pos.getPosition();
        }
    }
}
