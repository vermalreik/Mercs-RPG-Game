using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;

    PartyMemberUI[] memberSlots;
    List<Pokemon> pokemons;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        // This funciton will return all the PartyMemberUI Components that are attached to the child objects of the PartyScreen
        // it retuns an array, not a list
        // "GetComponentsInChildren" will only return the one that is currently active
        // GetComponentsInChildren<>(true), will also return inactive or disabled member slots
    }

    public void SetPartyData(List<Pokemon> pokemons)
    {
        this.pokemons = pokemons;

        for(int i = 0; i < memberSlots.Length; i++)
        {
            if( i< pokemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(pokemons[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a Pokemon";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for(int i=0; i<pokemons.Count; i++)
        {
            if(i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
