using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReplaySystem
{
    internal class TransformTracker : Tracker
    {
        [SerializeField]
        private Transform _transformToTrack;
        [SerializeField]
        private bool _trackChildren = false;
        [SerializeField]
        private bool _optimizeChildrenTracking = true;

        [SerializeField]
        private Animator _animatorToTrack;
        [SerializeField]
        private bool _setAnimatorCullingModeToAlwaysAnimateWhileRecording = true;
        [SerializeField]
        private bool _trackBones = false;

        private readonly List<TrackedTransform> _trackedTransforms = new();

        internal override void PrepareForReplayRecording()
        {
            if (_animatorToTrack == null) return;

            if (_setAnimatorCullingModeToAlwaysAnimateWhileRecording)
            {
                _animatorToTrack.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }
        }

        internal override IEnumerable<ReplayCommand> GetCommandsToRecord()
        {
            foreach (var trackedTransform in _trackedTransforms)
            {
                var changePositionCommand = trackedTransform.GetChangePositionCommand(_parentReplayableId, TrackerId);
                if (changePositionCommand.Changed) yield return changePositionCommand;

                var changeRotationCommand = trackedTransform.GetChangeRotationCommand(_parentReplayableId, TrackerId);
                if (changeRotationCommand.Changed) yield return changeRotationCommand;
            }
        }

        internal override void DoCommand(ReplayCommand command, ReplayPlayer replayPlayer)
        {
            try
            {
                if (command is ChangePosition changePosition)
                {
                    GetTrackedTransformById(changePosition.TransformId).DoCommand(changePosition);
                }
                if (command is ChangeRotation changeRotation)
                {
                    GetTrackedTransformById(changeRotation.TransformId).DoCommand(changeRotation);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Transform tracker do command exception: " + e.Message);
                if (command is TransformCommand tc)
                {
                    Debug.LogError("Transform tracker do command exception ids: " + tc.ReplayableObjectId + " " + tc.TrackerId + " " + tc.TransformId);
                }
            }
        }

        internal override void UndoCommand(ReplayCommand command, ReplayPlayer replayPlayer)
        {
            if (command is ChangePosition changePosition)
            {
                var trackedTransform = _trackedTransforms.Where(t => t.Id == changePosition.TransformId).FirstOrDefault();
                trackedTransform.UndoCommand(changePosition);
            }
            if (command is ChangeRotation changeRotation)
            {
                var trackedTransform = _trackedTransforms.Where(t => t.Id == changeRotation.TransformId).FirstOrDefault();
                trackedTransform.UndoCommand(changeRotation);
            }
        }

        protected override void Initialize()
        {
            _trackedTransforms.Add(new TrackedTransform((uint)_trackedTransforms.Count, _transformToTrack));

            if (_trackBones) TrackBones();

            if (_trackChildren) TrackChildren();
        }

        private void TrackBones()
        {
            if (_animatorToTrack == null) return;

            int lastBone = (int)HumanBodyBones.LastBone;
            for (int i = 0; i < lastBone; i++)
            {
                var boneTransform = _animatorToTrack.GetBoneTransform((HumanBodyBones)i);

                if (boneTransform == null) continue;
                if (boneTransform.gameObject == null) continue;

                _trackedTransforms.Add(new TrackedTransform((uint)_trackedTransforms.Count, boneTransform, false));
            }
        }

        private void TrackChildren()
        {
            var childrenTransforms = _transformToTrack.GetComponentsInChildren<Transform>();
            int childrenTransformsCount = childrenTransforms.Length;
            for (int i = 0; i < childrenTransformsCount; i++)
            {
                if (_optimizeChildrenTracking && childrenTransforms[i].GetComponentsInChildren<Renderer>().Count() <= 0) continue;

                _trackedTransforms.Add(new TrackedTransform((uint)_trackedTransforms.Count, childrenTransforms[i], false));
            }
        }

        private TrackedTransform GetTrackedTransformById(uint transformId)
        {
            int trackedTransformsCount = _trackedTransforms.Count;
            for (int i = 0; i < trackedTransformsCount; i++)
            {
                if (_trackedTransforms[i].Id == transformId) return _trackedTransforms[i];
            }

            return null;
        }
    }
}
