using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour {
    Planet planet;

    void Start()
    {
        planet = new Planet();
        planet.gameObject.transform.parent = this.transform;
    }
    void Update()
    {
        for(int f = 0; f< 6; f++)
        {
            StartCoroutine(planet.Update(f));
        }
    }
}
