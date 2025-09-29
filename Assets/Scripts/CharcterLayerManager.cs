using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharcterLayerManager : MonoBehaviour
{
    public List<Character> CharactersInRange;

    public List<Character> CharactersBelowPlayer;
    
    public List<Character> CharactersAbovePlayer;


    List<Character> SortedCharactersAfterPlayer;
    private void FixedUpdate()
    {
        foreach (Character character in CharactersInRange)
        {
            if (IsCharacterAbovePlayer(character))
            {
                if (CharactersBelowPlayer.Contains(character)) { CharactersBelowPlayer.Remove(character); }
                if (!CharactersAbovePlayer.Contains(character)) { CharactersAbovePlayer.Add(character); }
            }

            else
            {
                if (!CharactersBelowPlayer.Contains(character)) { CharactersBelowPlayer.Add(character); }
                if (CharactersAbovePlayer.Contains(character)) { CharactersAbovePlayer.Remove(character); }
                
            }
        }
        CharactersAbovePlayer = CharactersAbovePlayer.OrderBy(c => c.pivot.transform.position.y).ToList();
        CharactersBelowPlayer = CharactersBelowPlayer.OrderByDescending(c => c.pivot.transform.position.y).ToList();

        for(int i = 0; i < CharactersAbovePlayer.Count; i++)
        {
            int val = (-10 - i);
            CharactersAbovePlayer[i].SetSortingOrder(val, false);
            
        }
        for (int i = 0; i < CharactersBelowPlayer.Count; i++)
        {
            int val = (10 + i);
            CharactersBelowPlayer[i].SetSortingOrder(val, true);
        }

    }

    void SetSortingLayerAndOrder(Character charac)
    {
        if (CharactersAbovePlayer.Contains(charac))
        {
            foreach (SpriteRenderer rend in charac.renderers)
            {
                Debug.Log(CharactersAbovePlayer.IndexOf(charac));
                rend.sortingOrder = 10 + CharactersAbovePlayer.IndexOf(charac);
            }
        }
        else
        {
            
        }
        
    }

    

    public bool IsCharacterAbovePlayer(Character character)
    {
        return (Game.Instance.player.GetPlayerPosition().y <= character.pivot.transform.position.y);
    }

}
