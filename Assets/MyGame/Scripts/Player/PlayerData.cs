using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientID;
    public int score;
    public float lifePoint;
    public bool playerPlaced;

    public PlayerData(ulong clientId, int score, float lifePoint, bool playerPlaced)
    {
        this.clientID = clientId;
        this.score = score;
        this.lifePoint = lifePoint;
        this.playerPlaced = playerPlaced;
    }

    public bool Equals(PlayerData other)
    {
        return (
            this.clientID == other.clientID &&
            this.score == other.score &&
            this.lifePoint == other.lifePoint &&
            this.playerPlaced == other.playerPlaced
            );
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) 
                                                            where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref score);
        serializer.SerializeValue(ref lifePoint);
        serializer.SerializeValue(ref playerPlaced);

    }
}

