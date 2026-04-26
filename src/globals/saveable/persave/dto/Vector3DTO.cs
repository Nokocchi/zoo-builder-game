using Godot;

namespace ZooBuilder.globals.saveable;

public class Vector3Dto(float x, float y, float z)
{
    public float X { get; } = x;
    public float Y { get; } = y;
    public float Z { get; } = z;

    public Vector3 AsVector3()
    {
        return new Vector3(X, Y, Z);
    }

    public static Vector3Dto FromVector3(Vector3 v)
    {
        return new Vector3Dto(v.X, v.Y, v.Z);
    }
}