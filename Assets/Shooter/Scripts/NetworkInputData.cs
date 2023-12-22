using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector3 Direction;
    public const byte MouseButton0 = 0x01;
    public const byte MouseButton1 = 0x02;

    public byte buttons;
}
