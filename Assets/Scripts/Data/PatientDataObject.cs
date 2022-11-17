using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatientDataObject", menuName = "ScriptableObjects/PatientDataObject", order = 1)]
public class PatientDataObject : ScriptableObject
{
    public List<SavePatientData.PatientDataEntry> data;
}
