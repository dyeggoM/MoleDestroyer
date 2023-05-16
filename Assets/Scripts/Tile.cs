using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private MapGenerator _mapGenerator;
    private Vector3 _position;
    private List<Tile> _neighbors;
    private List<Vector2> directions = new List<Vector2>()
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right,
        new (-1,-1),
        new (1,-1),
        new (-1,1),
        new (1,1),
    };

    private readonly string _increaseTxt = Enum.GetName(typeof(ChangeTypes.Types), ChangeTypes.Types.Increase);
    private readonly string _decreaseTxt = Enum.GetName(typeof(ChangeTypes.Types), ChangeTypes.Types.Decrease);
    private string holeTag = Enum.GetName(typeof(TileTypes.Types), TileTypes.Types.Hole);
    private string dynamiteTag = Enum.GetName(typeof(TileTypes.Types), TileTypes.Types.Dynamite);

    void Start()
    {
        _position = transform.position;
        
        _mapGenerator = FindObjectOfType<MapGenerator>();
        
        GetNeighbors();
    }

    void GetNeighbors()
    {
        _neighbors = new List<Tile>();
        foreach (Vector2 direction in directions)
        {
            Vector3 position = _position + (Vector3)direction;
            if(_mapGenerator.IsPositionInMap(position) 
               && !_mapGenerator.IsPositionAMole(position))
                _neighbors.Add(_mapGenerator.GetTileByPosition(position));
        }
    }

    private void OnMouseDown()
    {
        if(_mapGenerator.HasMoleBeenSelected()) return;
        
        if (_mapGenerator.GetButtonSelection() == (int)TileTypes.Types.Grass)
        {
            LeftClick();
            if(_mapGenerator.IsPositionAMoleNeighbor(_position)
               || _mapGenerator.IsPositionAMole(_position)) return;
            AllNeighbors();
        }
        
        if(_mapGenerator.GetButtonSelection() == (int)TileTypes.Types.Dynamite)
            RightClick();
    }
    
    private void LeftClick()
    {
        InstantiateTile(_position);
        if(gameObject.CompareTag(holeTag))
            gameObject.GetComponent<Tile>().DestroyTile(_position, holeTag);
    }
    
    private void InstantiateTile(Vector3 position)
    {
        if(!gameObject.CompareTag(holeTag)) return;
        
        if (_mapGenerator.IsPositionAMole(position))
        {
            _mapGenerator.InstantiateMole(position);
            DestroyTile(_position,holeTag);
            
            return;
        }
        
        if (_mapGenerator.IsPositionAMoleNeighbor(position))
        {
            _mapGenerator.InstantiateNumber(position);
            DestroyTile(_position,holeTag);
            
            return;
        }

        _mapGenerator.InstantiateGrass(position);
    }

    private void RightClick()
    {
        InstantiateDynamite(_position);
    }
    
    private void InstantiateDynamite(Vector3 position)
    {
        if (gameObject.CompareTag(holeTag) )
        {
            _mapGenerator.ChangeTnTCounter(_increaseTxt);
            
            _mapGenerator.InstantiateDynamite(position);
            
            DestroyTile(position,holeTag);
            
            return;
        }

        if (!gameObject.CompareTag(dynamiteTag)) return;
        
        _mapGenerator.ChangeTnTCounter(_decreaseTxt);
        
        _mapGenerator.InstantiateHole(position);
            
        DestroyTile(position,dynamiteTag);
    }

    private void DestroyTile(Vector3 position, string tag)
    {
        if(position == transform.position && gameObject.CompareTag(tag))
            Destroy(gameObject);
    }

    void AllNeighbors()
    {
        if(_mapGenerator.IsPositionAMole(_position)) return;
        if(_mapGenerator.IsPositionAMoleNeighbor(_position)) return;

        List<Tile> allNeighbors = new List<Tile>();
        if (_mapGenerator.IsPositionGrass(_position))
        {
            _mapGenerator.currentGrass = _mapGenerator.GetGrass();
            allNeighbors.AddRange(GetAllNeighbors(gameObject.GetComponent<Tile>()));
        }

        foreach (Tile tile in allNeighbors.Distinct()) 
        {
            if(tile._position == _position) continue;
            
            tile.InstantiateTile(tile._position);
            
            if(tile.CompareTag(holeTag))
                tile.DestroyTile(tile._position, holeTag);
        }
    }

    List<Tile> GetAllNeighbors(Tile tile)
    {
        tile.GetNeighbors();
        
        List<Tile> allNeighbors = new List<Tile>();
        
        if (!_mapGenerator.IsPositionGrass(tile._position) || !_mapGenerator.currentGrass.Contains(tile._position)) 
            return allNeighbors;
        
        allNeighbors.Add(tile);
        _mapGenerator.currentGrass.Remove(tile._position);
            
        foreach (Tile neighbor in tile._neighbors)
        {
            if(neighbor == null) continue;
                
            Vector3 position = neighbor._position;
            if (_mapGenerator.IsPositionAMoleNeighbor(position))
            {
                allNeighbors.Add(neighbor);
                continue;
            }
                
            if(!_mapGenerator.currentGrass.Contains(position)) continue;
            allNeighbors.AddRange(neighbor.GetAllNeighbors(neighbor)
                .Where(n=>!allNeighbors.Contains(n)).ToList());
        }

        return allNeighbors;
    }
}
