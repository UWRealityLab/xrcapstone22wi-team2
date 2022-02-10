using UnityEngine;

namespace MedicalVisualizer
{
    /// <summary>
    /// Cross section plane.
    /// Used for cutting a model (cross section view).
    /// </summary>
    [ExecuteInEditMode]
    public class CrossSectionPlane : MonoBehaviour
    {
        /// <summary>
        /// Volume dataset to cross section.
        /// </summary>
        public VolumeRenderedObject targetObject;

        public GameObject sphere1;

        public GameObject sphere2;

        public GameObject sphere3;

        private void OnDisable()
        {
            if (targetObject != null)
                targetObject.meshRenderer.sharedMaterial.DisableKeyword("CUTOUT_PLANE");
        }

        private void Update()
        {
            if (targetObject == null)
                return;

            Material mat = targetObject.meshRenderer.sharedMaterial;

            mat.EnableKeyword("CUTOUT_PLANE");
            mat.SetMatrix("_CrossSectionMatrix", transform.worldToLocalMatrix * targetObject.transform.localToWorldMatrix);
            Vector3 ab = sphere1.transform.position - sphere2.transform.position;
            Vector3 bc = sphere2.transform.position - sphere3.transform.position;

            Vector3 center = (sphere1.transform.position + sphere2.transform.position + sphere3.transform.position) / 3;
            Vector3 direction = Vector3.Cross(ab, bc);
            transform.rotation = Quaternion.LookRotation(-direction);
            transform.position = center;
        }
        
        public void DestroyGameObject()
        {
            Destroy(gameObject);
        }
    }
}
