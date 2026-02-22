using UnityEngine;

public class AsymmetricalCamera : MonoBehaviour
{
    public PerspectiveProfile Profile;
    public SplitScreenManager SplitScreenManager;

    private void OnPreCull()
    {
        SplitScreenManager.ApplyPerspective(Profile);
    }
}