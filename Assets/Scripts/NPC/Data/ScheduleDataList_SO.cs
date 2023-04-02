using System.Collections.Generic;
using UnityEngine;
namespace NPC.Data
{
    [CreateAssetMenu(fileName = "ScheduleData",menuName = "NPC/ScheduleData_SO")]
    public class ScheduleDataList_SO : ScriptableObject
    {
        public List<ScheduleDetails> scheduleDetailsList = new List<ScheduleDetails>();
    }
}