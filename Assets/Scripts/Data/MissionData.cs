using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class MissionData : BaseData
{
    public string Id { get; set; }
    public string Category { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ObjectiveType { get; set; }
    public string TargetId { get; set; }
    public int TargetCount { get; set; }
    public List<MissionRewardData> Rewards { get; set; }
}

[Serializable]
public class MissionRewardData
{
    public string ItemId { get; set; }
    public int Amount { get; set; }
}