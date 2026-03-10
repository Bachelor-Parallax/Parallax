using System.Linq;
using UnityEngine;

public class Test : MonoBehaviour
{
    public PerspectiveProfile Profile;
    private PerspectiveManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        manager = FindObjectsByType<PerspectiveManager>(FindObjectsSortMode.None).First();
    }

    // Update is called once per frame
    private void Update()
    {
        manager.ApplyPerspective(Profile);
    }
}