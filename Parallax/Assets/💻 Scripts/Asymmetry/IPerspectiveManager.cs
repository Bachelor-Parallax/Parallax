using UnityEngine;

public enum PerspectiveProfile
{
    Human,
    Cat
}

public interface IPerspectiveManager
{
    /// <summary>
    /// Applies all perspective settings for a profile
    /// </summary>
    public void ApplyPerspective(PerspectiveProfile profile);
}