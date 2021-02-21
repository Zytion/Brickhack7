using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationValues : MonoBehaviour
{
    public int numHouses;
    public void NumHouses (Slider s) { numHouses = (int)s.value; }
    public int maxHouseDensity;
    public void MaxHouseDensity(Slider s) { maxHouseDensity = (int)s.value; }
    public float initallyInfected;
    public void InitallyInfected(Slider s) { initallyInfected = s.value; }
    public float socialDistance;
    public void SocialDistance(Slider s) { socialDistance = (int)s.value; }
    public float populationSocialDistance;
    public void PopulationSocialDistance(Slider s) { populationSocialDistance = s.value; }
    public float maskWearingPercentage;
    public void MaskWearingPercentage(Slider s) { maskWearingPercentage = s.value; }
    public float placesOfInterest;
    public void PlacesOfInterest(Slider s) { placesOfInterest = (int)s.value; }
    public int numOfHospitals;
    public void NumOfHospitals(Slider s) { numOfHospitals = (int)s.value;}
}
