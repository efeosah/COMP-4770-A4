using GameBrains.Extensions;
using GameBrains.Extensions.ScriptableObjects;
using GameBrains.Extensions.Vectors;
using GameBrains.GameManagement;
using UnityEngine;

namespace GameBrains.Visualization
{
    public class PointMarker : ExtendedScriptableObject
    {
        #region Members and Properties
        
        [SerializeField] GameObject pointMarkerPrefab;
        GameObject pointMarkerObject;
        Transform pointMarkerTransform;
        Renderer pointMarkerRenderer;
        float PointMarkerDropFromHeightOffset => Parameters.Instance.PointMarkerDropFromHeightOffset;
        float PointMarkerSurfaceOffset => Parameters.Instance.PointMarkerSurfaceOffset;
        
        #endregion Members and Properties

        #region Enable/Disable/Destroy

        public override void OnEnable()
        {
            base.OnEnable();
            if (pointMarkerObject == null) { Create(); }
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
            pointMarkerObject.CheckAndDestroy();
            pointMarkerRenderer = null;
            pointMarkerTransform = null;
            pointMarkerObject = null;
        }
        
        #endregion Enable/Disable/Destroy

        #region Methods

        public void Hide(bool shouldHide)
        {
            if (pointMarkerRenderer) { pointMarkerRenderer.enabled = !shouldHide; }
        }

        public void SetColor(Color color)
        {
            if (pointMarkerRenderer) { pointMarkerRenderer.material.color = color; }
        }

        public void Draw(VectorXZ location)
        {
            if (!pointMarkerObject) { return; }
            
            pointMarkerTransform.position = (Vector3)location;
            DropToSurface();
            Hide(false);
            pointMarkerObject.name = $"PointMarker {pointMarkerTransform.position}";
        }
        
        #endregion Methods
        
        #region Private/Protected Methods
        
        void DropToSurface()
        {
            Vector3 dropFromPosition = pointMarkerTransform.position;
            dropFromPosition.y += PointMarkerDropFromHeightOffset;

            CastToCollider(dropFromPosition, Vector3.down, 0f);
        }
        
        #region Cast to Collider
        
        // Does a sphere cast to find a surface / obstacle on which to place a new point marker
        void CastToCollider(
            Vector3 fromPosition,
            Vector3 direction,
            float maxDistance = 0)
        {
            RaycastHit hitInfo;

            var hit = maxDistance > 0f
                ? SphereCastToCollider(fromPosition, direction, out hitInfo, maxDistance)
                : SphereCastToCollider(fromPosition, direction, out hitInfo);

            if (hit) { pointMarkerTransform.position = hitInfo.point + Vector3.up * PointMarkerSurfaceOffset; }
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
                Parameters.Instance.PointMarkerRadius,
                direction,
                out hitInfo,
                maxDistance,
                Parameters.Instance.ObstacleLayerMask);
        }
        
        #endregion Cast to Collider

        #region Create
        void Create()
        {
            pointMarkerPrefab = Resources.Load<GameObject>("Prefabs/Visualization/PointMarkerPrefab");
            pointMarkerObject = Instantiate(pointMarkerPrefab);
            pointMarkerObject.hideFlags = HideFlags.HideInHierarchy;
            pointMarkerTransform = pointMarkerObject.transform;
            pointMarkerTransform.localScale = 2f * Parameters.Instance.PointMarkerRadius * Vector3.one;
            pointMarkerObject.name = $"PointMarker {pointMarkerTransform.position}";
            pointMarkerRenderer = pointMarkerObject.GetComponentInChildren<Renderer>();
        }
        
        #endregion Create
        
        #endregion Private/Protected Methods
    }
}