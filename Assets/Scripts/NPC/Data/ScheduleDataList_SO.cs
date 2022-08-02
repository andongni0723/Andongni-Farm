using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScheduleDataList_SO", menuName = "NPC Schedule/ScheduleDataList", order = 0)]
public class ScheduleDataList_SO : ScriptableObject
{
    public List<ScheduleDetails> ScheduleLists;
}
