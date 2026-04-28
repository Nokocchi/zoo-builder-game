using Godot;

namespace ZooBuilder.globals.saveable;

public record Vector3Dto
{
    public float X { get; init; }
    public float Y { get; init; }
    public float Z { get; init; }

    public Vector3 AsVector3() => new(X, Y, Z);

    public static Vector3Dto FromVector3(Vector3 v) => new()
    {
        X = v.X,
        Y = v.Y,
        Z = v.Z
    };
}