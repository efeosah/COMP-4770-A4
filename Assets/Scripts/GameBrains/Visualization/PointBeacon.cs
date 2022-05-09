using GameBrains.Extensions;
using GameBrains.Extensions.ScriptableObjects;
using GameBrains.Extensions.Vectors;
using GameBrains.GameManagement;
using UnityEngine;

namespace GameBrains.Visualization
{
    public class PointBeacon : ExtendedScriptableObject
    {
        #region Members and Properties
        
        [SerializeField] GameObject pointBeaconPrefab;
        GameObject pointBeaconObject;
        Transform pointBeaconTransform;
        Renderer pointBeaconRenderer;

        float PointBeaconDropFromHeightOffset => Parameters.Instance.PointBeaconDropFromHeightOffset;
        float PointBeaconSurfaceOffset => Parameters.Instance.PointBeaconSurfaceOffset;
        
        #endregion Members and Properties

        #region Enable/Disable/Destroy

        public override void OnEnable()
        {
            base.OnEnable();
            if (pointBeaconObject == null) { Create(); }
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
            pointBeaconObject.CheckAndDestroy();
            pointBeaconRenderer = null;
            pointBeaconTransform = null;
            pointBeaconObject = null;
        }

        #endregion Enable/Disable/Destroy

        #region Methods

        public void Hide(bool shouldHide)
        {
            if (pointBeaconRenderer) { pointBeaconRenderer.enabled = !shouldHide; }
        }

        public void SetColor(Color color)
        {
            if (pointBeaconRenderer) { pointBeaconRenderer.material.color = color; }
        }

        public void Draw(VectorXZ location)
        {
            if (!pointBeaconObject) { return; }

            pointBeaconTransform.position = (Vector3)location;
            DropToSurface();
            Hide(false);
            pointBeaconObject.name = $"PointBeacon {pointBeaconTransform.position}";
        }

        #endregion Methods
        
        #region Private/Protected Methods
        
        void DropToSurface()
        {
            Vector3 dropFromPosition = pointBeaconTransform.position;
            dropFromPosition.y += PointBeaconDropFromHeightOffset;

            CastToCollider(dropFromPosition, Vector3.down, 0f);
        }

        #region Cast to Collider

        // Does a sphere cast to find a surface / obstacle on which to place a new point beacon
        void CastToCollider(
            Vector3 fromPosition,
            Vector3 direction,
            float maxDistance = 0)
        {
            RaycastHit hitInfo;

            var hit = maxDistance > 0f
                ? SphereCastToCollider(fromPosition, direction, out hitInfo, maxDistance)
                : SphereCastToCollider(fromPosition, direction, out hitInfo);

            if (hit) { pointBeaconTransform.position = hitInfo.point + Vector3.up * PointBeaconSurfaceOffset; }
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
                Parameters.Instance.PointBeaconRadius,
                direction,
                out hitInfo,
                maxDistance,
                Parameters.Instance.ObstacleLayerMask);
        }

        #endregion Cast to Collider

        #region Create

        void Create()
        {
            pointBeaconPrefab = Resources.Load<GameObject>("Prefabs/Visualization/PointBeaconPrefab");
            pointBeaconObject = Instantiate(pointBeaconPrefab);
            pointBeaconObject.hideFlags = HideFlags.HideInHierarchy;
            pointBeaconTransform = pointBeaconObject.transform;
            pointBeaconTransform.localScale =
                Parameters.Instance.PointBeaconRadius * Vector3.one + 
                Parameters.Instance.PointBeaconHeight * Vector3.up;
            pointBeaconObject.name = $"PointBeacon {pointBeaconTransform.position}";
            pointBeaconRenderer = pointBeaconObject.GetComponentInChildren<Renderer>();
        }

        #endregion Create
        
        #endregion Private/Protected Methods
    }
}