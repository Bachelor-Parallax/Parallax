using UnityEngine;

public class DummyManager : MonoBehaviour, INetworkManager
{
    public GameObject cat, human;
    public PerspectiveProfile local;

    public GameObject GetLocalPlayerGameObject()
    {
        if (local == PerspectiveProfile.Cat) return cat;
        return human;
    }
}