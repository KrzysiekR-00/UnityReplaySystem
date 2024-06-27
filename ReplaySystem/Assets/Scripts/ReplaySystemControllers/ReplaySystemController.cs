using ReplaySystem;
using ReplaySystemFileDataManager;
using System;
using System.IO;
using UnityEngine;

public class ReplaySystemController : MonoBehaviour
{
    [SerializeField]
    private ReplayRecorder _recorder;

    [SerializeField]
    private ReplayPlayer _player;

    [SerializeField]
    private string _recordingDataFilePath = @"%userprofile%\Documents\ReplaySystemRecordingData.txt";

    public void InitializeAndStartRecording()
    {
        _recorder.Initialize(new ReplayFileDataWriter(_recordingDataFilePath));
        _recorder.Record();
    }

    public void StopRecording()
    {
        _recorder.StopAndSaveRecording();
    }

    public void InitializeAndStartPlaying()
    {
        _player.Initialize(new ReplayFileDataReader(_recordingDataFilePath));
        _player.Play();
    }

    public void ShowRecordingFile()
    {
        if (!File.Exists(_recordingDataFilePath)) return;

        string argument = "/select, \"" + _recordingDataFilePath + "\"";

        System.Diagnostics.Process.Start("explorer.exe", argument);
    }

    private void Awake()
    {
        _recordingDataFilePath = Environment.ExpandEnvironmentVariables(_recordingDataFilePath);
    }

    private void OnDestroy()
    {
        if (!File.Exists(_recordingDataFilePath)) return;
        File.Delete(_recordingDataFilePath);
    }
}
