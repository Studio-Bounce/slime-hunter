using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTransition : MonoBehaviour
{
    public void ExploreToVillage()
    {
        AudioManager.Instance.ExploreToVillage();
    }

    public void VillageToExplore()
    {
        AudioManager.Instance.VillageToExplore();
    }
}
