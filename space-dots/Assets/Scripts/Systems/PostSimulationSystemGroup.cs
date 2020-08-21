using Unity.Entities;

[UnityEngine.ExecuteAlways]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
[UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
public class PostSimulationSystemGroup : ComponentSystemGroup { }
