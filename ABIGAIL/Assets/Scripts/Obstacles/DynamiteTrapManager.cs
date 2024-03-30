using UnityEngine;
using System.Collections.Generic;

public class DynamiteTrapManager : MonoBehaviour
{
    public static DynamiteTrapManager Instance { get; private set; }
    private List<DynamiteTrap> dynamiteTraps = new List<DynamiteTrap>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterTrap(DynamiteTrap trap)
    {
        if (!dynamiteTraps.Contains(trap))
        {
            dynamiteTraps.Add(trap);
        }
    }

    public void UnregisterTrap(DynamiteTrap trap)
    {
        if (dynamiteTraps.Contains(trap))
        {
            dynamiteTraps.Remove(trap);
        }
    }

    public void RespawnAllTraps()
    {
        foreach (var trap in dynamiteTraps)
        {
            trap.RespawnDynamite();
        }
    }
}
