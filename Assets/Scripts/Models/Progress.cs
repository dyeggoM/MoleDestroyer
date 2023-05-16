using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Progress
{
    [SerializeField]
    public int Level { get; set; }
    [SerializeField]
    public DifficultyTypes.Types DifficultyType { get; set; }
    [SerializeField]
    public bool IsCurrentLevel { get; set; }
    [SerializeField]
    
    public bool IsCompleted { get; set; }
    [SerializeField]
    public TimeSpan? BestTime { get; set; }

    public Progress()
    {
        
    }
    
    public Progress(int level, DifficultyTypes.Types difficultyType, bool isCurrentLevel)
    {
        Level = level;
        DifficultyType = difficultyType;
        IsCurrentLevel = isCurrentLevel;
        IsCompleted = false;
        BestTime = null;
    }

    public Progress(TimeSpan? bestTime)
    {
        IsCompleted = true;
        BestTime = bestTime;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

