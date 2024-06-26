using UnityEngine;

namespace ReplaySystem
{
    internal class TrackedTransform
    {
        internal readonly uint Id;

        private readonly Transform _transform;
        private readonly bool _globalTrackingMode;

        private Vector3? _previousPosition;
        private Quaternion? _previousRotation;

        private readonly string _debugName;
        private readonly string _debugParentName;

        internal TrackedTransform(uint id, Transform transform, bool globalTrackingMode = true)
        {
            Id = id;

            _transform = transform;
            _globalTrackingMode = globalTrackingMode;

            _previousPosition = null;
            _previousRotation = null;

            _debugName = transform.name;
            if (transform.parent != null) _debugParentName = transform.parent.name;
        }

        internal ChangePosition GetChangePositionCommand(string objectId, uint trackerId)
        {
            var command = new ChangePosition(objectId, trackerId, Id, _previousPosition, GetPosition());

            _previousPosition = GetPosition();

            return command;
        }

        internal ChangeRotation GetChangeRotationCommand(string objectId, uint trackerId)
        {
            var command = new ChangeRotation(objectId, trackerId, Id, _previousRotation, GetRotation());

            _previousRotation = GetRotation();

            return command;
        }

        internal void DoCommand(ChangePosition changePosition)
        {
            ChangePosition(changePosition.CurrentPosition);
        }

        internal void DoCommand(ChangeRotation changeRotation)
        {
            ChangeRotation(changeRotation.CurrentRotation);
        }

        internal void UndoCommand(ChangePosition changePosition)
        {
            if(changePosition.PreviousPosition.HasValue) ChangePosition(changePosition.PreviousPosition.Value);
        }

        internal void UndoCommand(ChangeRotation changeRotation)
        {
            if(changeRotation.PreviousRotation.HasValue) ChangeRotation(changeRotation.PreviousRotation.Value);
        }

        private Vector3 GetPosition()
        {
            if (_transform == null)
            {
                Debug.LogError("Transform is null: " + _debugName + ", " + _debugParentName);
                return Vector3.zero;
            }

            if (_globalTrackingMode) return _transform.position;
            else return _transform.localPosition;
        }

        private Quaternion GetRotation()
        {
            if (_transform == null)
            {
                Debug.LogError("Transform is null: " + _debugName + ", " + _debugParentName);
                return Quaternion.identity;
            }

            if (_globalTrackingMode) return _transform.rotation;
            else return _transform.localRotation;
        }

        private void ChangePosition(Vector3 newPosition)
        {
            if (_transform == null) Debug.LogError("Transform is null: " + _debugName + ", " + _debugParentName);

            if (_globalTrackingMode) _transform.position = newPosition;
            else _transform.localPosition = newPosition;
        }

        private void ChangeRotation(Quaternion newRotation)
        {
            if (_transform == null) Debug.LogError("Transform is null: " + _debugName + ", " + _debugParentName);

            if (_globalTrackingMode) _transform.rotation = newRotation;
            else _transform.localRotation = newRotation;
        }
    }
}
