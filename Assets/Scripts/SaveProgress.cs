using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Android;

public class SaveProgress : MonoBehaviour
{
    [SerializeField] private DifficultyTypes.Types difficulty = DifficultyTypes.Types.Casual;
    
    private List<Progress> _progresses;
    private string _filePath;
    private string _errorFilePath;

    private int _currentLevel = 0;
    private DifficultyTypes.Types _currentDifficulty = DifficultyTypes.Types.Casual;
    private void Awake()
    {
        _filePath = @$"{Application.persistentDataPath}/MoleDestroyer.json";
        _errorFilePath =  @$"{Application.consoleLogPath}/MoleDestroyerError";
        
        LoadData();
    }

    void Start()
    {
    }

    void Update()
    {
        
    }

    private List<Progress> JsonToObject(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<List<Progress>>(json);
        }
        catch (NullReferenceException)
        {
            return new List<Progress>();
        }
        catch (Exception e)
        {
            CreateErrorFile(e);
            return new List<Progress>();
        }
    }

    private string ObjectToJson(List<Progress> progresses)
    {
        try
        {
            return JsonConvert.SerializeObject(progresses);
        }
        catch (NullReferenceException)
        {
            return string.Empty;
        }
        catch (Exception e)
        {
            CreateErrorFile(e);
            return string.Empty;
        }
    }

    public void SaveData()
    {
        if(string.IsNullOrWhiteSpace(_filePath))
            return;
        ValidateFileIsCreated(_filePath);
        
        File.WriteAllText(_filePath,ObjectToJson(_progresses));
    }

    public void LoadData()
    {
        _progresses = new List<Progress>();
        
        if(string.IsNullOrWhiteSpace(_filePath))
            return;
        
        ValidateFileIsCreated(_filePath);
        
        string fileText = File.ReadAllText(_filePath);

        if (!string.IsNullOrWhiteSpace(fileText))
        {
            _progresses = JsonToObject(fileText);
        }
        
        _currentLevel = _progresses
            .LastOrDefault(p => p.IsCurrentLevel && !p.IsCompleted)?.Level ?? 0;
        
        _currentDifficulty = _progresses
            .LastOrDefault(p => p.IsCurrentLevel && !p.IsCompleted)?.DifficultyType ?? difficulty;
        
    }

    private void ValidateFileIsCreated(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                var fileStream = File.Create(filePath);
                fileStream.Close();
            }
        }
        catch (Exception e)
        {
            CreateErrorFile(e);
        }
    }

    private void CreateErrorFile(Exception e)
    {
        if(string.IsNullOrWhiteSpace(_errorFilePath))
            return;
        string errorFilePath = $"{_errorFilePath}_{DateTime.Now.Ticks}.txt";
        ValidateFileIsCreated(errorFilePath);
        File.WriteAllText(errorFilePath,JsonConvert.SerializeObject(e));
    }

    public int GetCurrentLevel()
    {
        return _currentLevel;
    }

    public DifficultyTypes.Types GetCurrentDifficulty()
    {
        return _currentDifficulty;
    }

    public void SetCurrentDifficulty(DifficultyTypes.Types difficulty)
    {
        _currentDifficulty = difficulty;
    }
    
    public void AddProgress(Progress progress)
    {
        bool add = _progresses.LastOrDefault() == null;
        
        if (!add)
        {
            _progresses.LastOrDefault().IsCurrentLevel = false;
        }

        bool update = !add && _progresses.Select(p => p.Level).Contains(progress.Level);

        if (update)
        {
            Progress oldProgress = _progresses.FirstOrDefault(p => p.Level == progress.Level);
            progress.IsCompleted = oldProgress.IsCompleted;
            progress.BestTime = oldProgress.BestTime;
            
            _progresses.Remove(oldProgress);
        }
        
        _progresses.Add(progress);
    }
}
