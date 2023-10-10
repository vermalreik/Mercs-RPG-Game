using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;

    PartyMemberUI[] memberSlots;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
        // This funciton will return all the PartyMemberUI Components that are attached to the child objects of the PartyScreen
        // it retuns an array, not a list
    }

    public void SetPartyData(List<Pokemon> pokemons)
    {
        for(int i = 0; i < memberSlots.Length; i++)
        {
            if( i< pokemons.Count)
                memberSlots[i].SetData(pokemons[i]);
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a Pokemon";
    }
}
