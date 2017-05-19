using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Runtime.Serialization;


[DataContract(Name = "MAP_ENTRY")]
public class MapEntry {

    [DataMember(Name = "NAME", Order = 1)]
    string name;

    // It miiiight be a bit redundant but i'm lazy
    [DataMember(Name = "POSITION", Order = 2)]
    float[] position;

    [DataMember(Name = "ROTATION", Order = 3)]
    float[] rotation;

    [DataMember(Name = "SCALE", Order = 4)]
    float[] scale;

    [DataMember(Name = "noScaleRotation", Order = 5)]
    bool noScaleRotation;

    public void Spawn(Vector3 BasePosition)
    {
        // Yeah, i'll change stuff in here later
        if (noScaleRotation)
        {
            Pool.Spawn(name, BasePosition + GetPosition());
        }
        else
        {
            Pool.Spawn(name, BasePosition + GetPosition(), GetRotation(), GetScale());
        }
    }

    public void Spawn()
    {
        // Meh
        Spawn(Vector3.zero);
    }

    public MapEntry(string _name, Vector3 _position, Quaternion _rotation, Vector3 _scale, bool _noScaleRotation = false)
    {
        name = _name;

        position = new float[3] {_position.x, _position.y, _position.z};

        rotation = new float[4] {_rotation.x, _rotation.y, _rotation.z, _rotation.w};

        scale = new float[3] {_scale.x, _scale.y, _scale.z};

        noScaleRotation = _noScaleRotation;
    }

    public override string ToString()
    {
        return (name + "/(" + position[0] + ", " + position[1] + ", " + position[2] + ")/(" + rotation[0] + ", " + rotation[1] + ", " + rotation[2] + ", " + rotation[3] + ")/(" + scale[0] + ", " + scale[1] + ", " + scale[2] + ")\n");
    }

    public Vector3 GetPosition()
    {
        return new Vector3(position[0], position[1], position[2]);
    }

    public Quaternion GetRotation()
    {
        return new Quaternion(rotation[0], rotation[1], rotation[2], rotation[3]);
    }

    public Vector3 GetScale()
    {
        return new Vector3(scale[0], scale[1], scale[2]);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        MapEntry other = obj as MapEntry;

        if (other == null)
            return false;

        return name == other.name && position.SequenceEqual(other.position) && rotation.SequenceEqual(other.rotation) && scale.SequenceEqual(other.scale);
    }

    public bool Equals(MapEntry other)
    {
        if (other == null)
            return false;

        return name == other.name && position.SequenceEqual(other.position) && rotation.SequenceEqual(other.rotation) && scale.SequenceEqual(other.scale);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
