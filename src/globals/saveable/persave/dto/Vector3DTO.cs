using Godot;

namespace ZooBuilder.globals.saveable;

public class Vector3Dto
{
    public float X { get; init; }
    public float Y { get; init; }
    public float Z { get; init; }

    public Vector3 AsVector3()
    {
        return new Vector3(X, Y, Z);
    }

    public static Vector3Dto FromVector3(Vector3 v)
    {
        return new Vector3Dto()
        {
            X = v.X,
            Y = v.Y,
            Z = v.Z
        };
    }
}