using ReplaySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReplaySystemPlayerController : MonoBehaviour
{
    [SerializeField]
    private ReplayPlayer _player;

    [SerializeField]
    private TextMeshProUGUI _currentReplayTime;
    [SerializeField]
    private TextMeshProUGUI _replayLength;

    [SerializeField]
    private Slider _timelineSlider;

    private bool _playerIsInitialized = false;

    public void TogglePlayPause()
    {
        if (_player.IsPlaying()) _player.Pause();
        else _player.Play();
    }

    public void BackwardDown()
    {
        _player.SetReplaySpeed(-3);
    }

    public void BackwardUp()
    {
        _player.SetReplaySpeed(1);
    }

    public void ForwardDown()
    {
        _player.SetReplaySpeed(5);
    }

    public void ForwardUp()
    {
        _player.SetReplaySpeed(1);
    }

    private void Start()
    {
        _player.OnInitialize += PlayerInitialized;
    }

    private void PlayerInitialized()
    {
        _player.OnInitialize -= PlayerInitialized;

        _playerIsInitialized = true;

        _replayLength.text = _player.GetReplayLength().ToString(@"hh\:mm\:ss\.fff");
    }

    private void Update()
    {
        if (!_playerIsInitialized) return;

        _currentReplayTime.text = _player.GetCurrentReplayTime().ToString(@"hh\:mm\:ss\.fff");

        _timelineSlider.value = (float)(_player.GetCurrentReplayTime() / _player.GetReplayLength());
    }
}
