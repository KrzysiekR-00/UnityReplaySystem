using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReplaySystem
{
    [RequireComponent(typeof(ReplayableObjectCollection))]
    [DefaultExecutionOrder(20000)]
    public class ReplayRecorder : MonoBehaviour
    {
        [SerializeField]
        private int _recordingFPS = 30;

        private IReplayDataRecorder _recorder;
        private ReplayableObjectCollection _replayableObjectCollection;

        private bool _isRecording = false;
        private TimeSpan _currentRecordingTimeStamp = TimeSpan.Zero;
        private TimeSpan _nextTimeStampToRecord = TimeSpan.Zero;

        private readonly List<TimeSpan> _updateTimes = new();

        public void Initialize(IReplayDataRecorder recorder)
        {
            _replayableObjectCollection.ReplayableObjectRegistered = (r) => { AddCommand(r.GetSpawnObjectCommand()); r.PrepareForReplayRecording(); };
            _replayableObjectCollection.ReplayableObjectUnregistered = (r) => { AddCommand(r.GetDestroyObjectCommand()); };
            foreach (var r in _replayableObjectCollection.GetReplayableObjects())
            {
                r.PrepareForReplayRecording();
            }

            _recorder = recorder;
            _recorder.Open();

            _currentRecordingTimeStamp = TimeSpan.Zero;
            _nextTimeStampToRecord = TimeSpan.Zero;
        }

        public void Record()
        {
            _isRecording = true;
        }

        public void Pause()
        {
            _isRecording = false;
        }

        public bool IsRecording()
        {
            return _isRecording;
        }

        public void AddCommand(ReplayCommand eventToSave, bool force = false)
        {
            if (!_isRecording && !force) return;

            eventToSave.SetTimeStamp(_currentRecordingTimeStamp);
            _recorder.RecordCommand(eventToSave);
        }

        public void StopAndSaveRecording()
        {
            _isRecording = false;

            _recorder?.Close();
        }

        private bool IsTimeStampProperToRecord(TimeSpan timeStamp)
        {
            if (timeStamp < _nextTimeStampToRecord) return false;
            return true;
        }

        private TimeSpan GetNextTimeStampToRecord()
        {
            var delay = _currentRecordingTimeStamp - _nextTimeStampToRecord;
            return _currentRecordingTimeStamp.Add(TimeSpan.FromSeconds(1 / (float)_recordingFPS) - delay);
        }

        private void Awake()
        {
            _replayableObjectCollection = GetComponent<ReplayableObjectCollection>();
        }

        private void LateUpdate()
        {
            var updateStart = DateTime.Now;

            if (!_isRecording) return;

            _currentRecordingTimeStamp = _currentRecordingTimeStamp.Add(TimeSpan.FromSeconds(Time.deltaTime));

            if (!IsTimeStampProperToRecord(_currentRecordingTimeStamp)) return;

            var replayableObjects = _replayableObjectCollection.GetReplayableObjects();
            if (replayableObjects.Count() <= 0) return;

            _nextTimeStampToRecord = GetNextTimeStampToRecord();

            foreach (var replayableObject in replayableObjects)
            {
                var replayableCommands = replayableObject.GetCommandsToRecord();
                foreach (var command in replayableCommands)
                {
                    command.SetTimeStamp(_currentRecordingTimeStamp);
                    _recorder.RecordCommand(command);
                }
            }

#if UNITY_EDITOR
            var updateTime = DateTime.Now - updateStart;
            _updateTimes.Add(updateTime);
#endif
        }

        private void OnDisable()
        {
            StopAndSaveRecording();

#if UNITY_EDITOR
            if (_updateTimes.Count <= 0) return;
            Debug.Log("_updateTimes: " + _updateTimes.Count);
            Debug.Log("Average recorder update time ms: " + _updateTimes.Average(t => t.Milliseconds));
            Debug.Log("Max recorder update time ms: " + _updateTimes.Max(t => t.Milliseconds));
#endif
        }
    }
}