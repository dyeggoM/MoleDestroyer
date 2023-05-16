using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private ButtonHide redoButton;
    [SerializeField] private TextMeshProUGUI levelText;
    
    private MapGenerator _mapGenerator;
    private readonly string _increaseTxt = Enum.GetName(typeof(ChangeTypes.Types), ChangeTypes.Types.Increase);
    private readonly string _decreaseTxt = Enum.GetName(typeof(ChangeTypes.Types), ChangeTypes.Types.Decrease);
    void Start()
    {
        _mapGenerator = FindObjectOfType<MapGenerator>();
    }
    
    public void ActivateRedoButton(bool isActive)
    {
        redoButton.SetButtonActive(isActive);
    }
    
    public void ChangeLevel(string direction)
    {
        int seed = _mapGenerator.GetSeed();

        if (direction == _increaseTxt)
        {
            if (seed < int.MaxValue)
                seed++;
        }
        else if (direction == _decreaseTxt)
        {
            if (seed > int.MinValue)
                seed--;
        }

        ActivateRedoButton(false);
        
        _mapGenerator.ChangeLevelSeed(seed);
        
        ChangeLevelText($"{seed}");
    }

    public void ChangeLevelText(string level)
    {
        levelText.text = $"{level}";
    }
}
