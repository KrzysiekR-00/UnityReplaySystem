using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace ReplaySystem
{
    [RequireComponent(typeof(ReplayableObjectCollection))]
    [RequireComponent(typeof(PrefabsCollection))]
    [DefaultExecutionOrder(-20000)]
    public class ReplayPlayer : MonoBehaviour
    {
        public UnityAction OnInitialize;
        public UnityAction<float> OnAudioVolumeChanged;
        public UnityAction<float> OnReplaySpeedChanged;

        internal ReplayableObjectCollection ReplayableObjectCollection { get; private set; }
        internal PrefabsCollection PrefabsCollection { get; private set; }

        private IReplayDataReader _loader;

        private bool _isInitialized = false;
        private TimeSpan _lastCommandTimeStamp = TimeSpan.Zero;
        private TimeSpan _targetReplayTimeStamp = TimeSpan.Zero;
        private bool _isPlaying = false;
        private float _replaySpeed = 1f;
        private float _audioVolume = 1f;

        private ReplayCommand[] _commandsToDo = Array.Empty<ReplayCommand>();
        private Thread _commandsLoadingThread;
        private bool _playForward = true;

        private DateTime? _loadingStart = null;
        private bool _isLoading = false;

        public void Initialize(IReplayDataReader loader)
        {
            ReplayableObjectCollection.ReplayableObjectRegistered = (replayable) => { replayable.PrepareForReplayPlaying(); };
            foreach (var replayableObject in ReplayableObjectCollection.GetReplayableObjects())
            {
                replayableObject.PrepareForReplayPlaying();
            }

            _loader = loader;

            Pause();

            _isInitialized = true;
            OnInitialize?.Invoke();
        }

        public void Play()
        {
            _isPlaying = true;

            ChangeTimeScale(_replaySpeed);
        }

        public void Pause()
        {
            _isPlaying = false;

            ChangeTimeScale(0);
        }

        public bool IsPlaying()
        {
            return _isPlaying;
        }

        public bool IsLoading()
        {
            return _isLoading;
        }

        public DateTime? LoadingStart { get { return _loadingStart; } }

        public void SetReplaySpeed(float replaySpeed)
        {
            _replaySpeed = replaySpeed;

            ChangeTimeScale(_replaySpeed);

            OnReplaySpeedChanged?.Invoke(replaySpeed);
        }

        public float GetReplaySpeed()
        {
            return _replaySpeed;
        }

        public TimeSpan GetReplayLength()
        {
            return _loader.GetReplayLength();
        }

        public TimeSpan GetCurrentReplayTime()
        {
            return _targetReplayTimeStamp;
        }

        public void MoveToTimeStamp(TimeSpan targetTimeStamp)
        {
            _targetReplayTimeStamp = targetTimeStamp;
        }

        public void SetAudioVolume(float audioVolume)
        {
            _audioVolume = audioVolume;

            OnAudioVolumeChanged?.Invoke(audioVolume);
        }

        public float GetAudioVolume()
        {
            return _audioVolume;
        }

        private void RequestCommands(TimeSpan from, TimeSpan to)
        {
            if (to == from) return;
            if (_commandsLoadingThread != null && _commandsLoadingThread.IsAlive) return;

            _commandsLoadingThread = new Thread(() => LoadCommands(from, to));
            _commandsLoadingThread.Start();
        }

        private void LoadCommands(TimeSpan from, TimeSpan to)
        {
            _loadingStart = DateTime.Now;
            _isLoading = true;

            _commandsToDo = Array.Empty<ReplayCommand>();

            if (from > to)
            {
                (to, from) = (from, to);
                _playForward = false;
            }
            else _playForward = true;

            var commandsToHandle = _loader.ReadCommandsBetweenTimeStamps(from, to);
            _commandsToDo = _commandsToDo.Concat(commandsToHandle).ToArray();

            _isLoading = false;
            _loadingStart = null;
        }

        private void Awake()
        {
            ReplayableObjectCollection = GetComponent<ReplayableObjectCollection>();
            PrefabsCollection = GetComponent<PrefabsCollection>();
        }

        private void Update()
        {
            if (!_isInitialized) return;

            if (_isLoading)
            {
                //frame skipped by loading
                return;
            }

            HandleCommands();

            if (_isPlaying && _replaySpeed != 0)
            {
                _targetReplayTimeStamp = _targetReplayTimeStamp.Add(TimeSpan.FromSeconds(Time.unscaledDeltaTime * _replaySpeed));
                if (_targetReplayTimeStamp < TimeSpan.Zero) _targetReplayTimeStamp = TimeSpan.Zero;
                if (_targetReplayTimeStamp > GetReplayLength()) _targetReplayTimeStamp = GetReplayLength();
            }

            RequestCommands(_lastCommandTimeStamp, _targetReplayTimeStamp);

            if (_isPlaying)
            {
                if (_replaySpeed > 0) _lastCommandTimeStamp = _targetReplayTimeStamp.Add(new TimeSpan(1));
                else if (_replaySpeed < 0) _lastCommandTimeStamp = _targetReplayTimeStamp.Add(new TimeSpan(-1));
            }
        }

        private void ChangeTimeScale(float targetTimeScale)
        {
            Time.timeScale = Math.Abs(targetTimeScale);
        }

        private void HandleCommands()
        {
            if (_commandsToDo.Length <= 0) return;

            if (_playForward)
            {
                for (int i = 0; i < _commandsToDo.Length; i++)
                {
                    _commandsToDo[i].Do(this);
                }
            }
            else
            {
                for (int i = _commandsToDo.Length - 1; i >= 0; i--)
                {
                    _commandsToDo[i].Undo(this);
                }
            }

            _commandsToDo = Array.Empty<ReplayCommand>();
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
        }
    }
}