using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class CharacterFade : NetworkBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private float fadeDistance = 1.2f;
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float fadeSpeed = 8f;
    

    private Transform cameraTransform;
    private float currentAlpha = 1f;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true)
            .Where(r => r.gameObject.name != "Cube")
            .ToArray();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        TryAssignCamera();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (cameraTransform == null)
            TryAssignCamera();

        if (cameraTransform == null || renderers == null || renderers.Length == 0)
            return;

        Bounds combinedBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            combinedBounds.Encapsulate(renderers[i].bounds);

        Vector3 closestPoint = combinedBounds.ClosestPoint(cameraTransform.position);
        float dist = Vector3.Distance(cameraTransform.position, closestPoint);

        float targetAlpha = 1f;

        if (dist < fadeDistance)
            targetAlpha = Mathf.Lerp(minAlpha, 1f, dist / fadeDistance);

        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
        SetAlpha(currentAlpha);
    }

    private void TryAssignCamera()
    {
        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void SetAlpha(float alpha)
    {
        foreach (var r in renderers)
        {
            if (r == null) continue;

            foreach (var mat in r.materials)
            {
                if (mat == null || !mat.HasProperty("_Color")) continue;

                Color c = mat.color;
                c.a = alpha;
                mat.color = c;
            }
        }
    }
}