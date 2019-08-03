using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
public class EventHandlerComponent : IComponent
{
    [PrimaryEntityIndex]
    public int id;
}
