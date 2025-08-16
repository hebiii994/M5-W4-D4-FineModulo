using UnityEngine;

public class FootstepFX : MonoBehaviour
{
    [Header("Decal Prefabs")]
    [SerializeField] private GameObject _leftFootstepDecalPrefab;
    [SerializeField] private GameObject _rightFootstepDecalPrefab;


    public void PlaceFootstepDecal(string foot)
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 0.3f))
        {
            GameObject decalPrefabToUse = (foot == "Left") ? _leftFootstepDecalPrefab : _rightFootstepDecalPrefab;

            if (decalPrefabToUse != null)
            {
                Quaternion decalRotation = Quaternion.LookRotation(-hit.normal, transform.forward);
                Instantiate(decalPrefabToUse, hit.point, decalRotation);
            }
        }
    }
}