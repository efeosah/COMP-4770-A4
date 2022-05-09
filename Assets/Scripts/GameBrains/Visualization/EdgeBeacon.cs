using GameBrains.Extensions;
using GameBrains.Extensions.ScriptableObjects;
using GameBrains.Extensions.Vectors;
using GameBrains.GameManagement;
using UnityEngine;

namespace GameBrains.Visualization
{
    public class EdgeBeacon : ExtendedScriptableObject
    {
        #region Members and Properties

        [SerializeField] GameObject edgeBeaconPrefab;
        GameObject edgeBeaconObject;
        Transform edgeBeaconTransform;
        Renderer edgeBeaconRenderer;

        float EdgeBeaconDropFromHeightOffset => Parameters.Instance.EdgeBeaconDropFromHeightOffset;
        float EdgeBeaconSurfaceOffset => Parameters.Instance.EdgeBeaconSurfaceOffset;

        #endregion Members and Properties

        #region Enable/Disable/Destroy

        public override void OnEnable()
        {
            base.OnEnable();
            if (edgeBeaconObject == null) { Create(); }
            Hide(true);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Hide(true);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            edgeBeaconObject.CheckAndDestroy();
            edgeBeaconRenderer = null;
            edgeBeaconTransform = null;
            edgeBeaconObject = null;
        }

        #endregion Enable/Disable/Destroy

        #region Methods

        public void Hide(bool shouldHide)
        {
            if (edgeBeaconRenderer) { edgeBeaconRenderer.enabled = !shouldHide; }
        }

        public void SetColor(Color color)
        {
            if (edgeBeaconRenderer) { edgeBeaconRenderer.material.color = color; }
        }

        public void Draw(VectorXZ startLocation, VectorXZ endLocation)
        {
            if (!edgeBeaconObject) { return; }

            edgeBeaconTransform.position = (Vector3)(startLocation + endLocation) / 2f;
            DropToSurface();
            var length = (endLocation - startLocation).magnitude;
            edgeBeaconTransform.localScale = new Vector3(0.3f, 0.3f, length);
            Vector2 direction = endLocation - startLocation;
            float angle = Vector2.Angle(Vector2.up, direction);
            Vector3 cross = Vector3.Cross(Vector2.up, direction);
            if (cross.z > 0) { angle = 360f - angle; }
            edgeBeaconTransform.eulerAngles = new Vector3(0, angle, 0);
            edgeBeaconObject.name = $"EdgeBeacon {startLocation} --[{length}]--> {endLocation}";
            Hide(false);
        }

        #endregion Methods

        #region Private/Protected Methods

        void DropToSurface()
        {
            Vector3 dropFromPosition = edgeBeaconTransform.position;
            dropFromPosition.y += EdgeBeaconDropFromHeightOffset;

            CastToCollider(dropFromPosition, Vector3.down, 0f);
        }

        #region Cast to Collider

        // Does a sphere cast to find a surface / obstacle on which to place a new edge beacon
        void CastToCollider(
            Vector3 fromPosition,
            Vector3 direction,
            float maxDistance = 0)
        {
            RaycastHit hitInfo;

            var hit = maxDistance > 0f
                ? SphereCastToCollider(fromPosition, direction, out hitInfo, maxDistance)
                : SphereCastToCollider(fromPosition, direction, out hitInfo);

            if (hit) { edgeBeaconTransform.position = hitInfo.point + Vector3.up * EdgeBeaconSurfaceOffset; }
        }
        
        // Sphere cast for obstacles
        bool SphereCastToCollider(
            Vector3 origin,
            Vector3 direction,
            out RaycastHit hitInfo,
            float maxDistance = float.MaxValue
        )
        {
            return Physics.SphereCast(
                origin,
                Parameters.Instance.EdgeBeaconRadius,
                direction,
                out hitInfo,
                maxDistance,
                Parameters.Instance.ObstacleLayerMask);
        }

        #endregion Cast to Collider

        #region Create

        void Create()
        {
            edgeBeaconPrefab = Resources.Load<GameObject>("Prefabs/Visualization/EdgeBeaconPrefab");
            edgeBeaconObject = Instantiate(edgeBeaconPrefab);
            edgeBeaconObject.hideFlags = HideFlags.HideInHierarchy;
            edgeBeaconTransform = edgeBeaconObject.transform;
            edgeBeaconObject.name = $"EdgeBeacon {edgeBeaconTransform.position}";
            edgeBeaconRenderer = edgeBeaconObject.GetComponentInChildren<Renderer>();
        }

        #endregion Create

        #endregion Private/Protected Methods
    }
}