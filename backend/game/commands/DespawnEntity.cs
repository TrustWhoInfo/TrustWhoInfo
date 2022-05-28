namespace backend;

public class DespawnEntity : BaseCommand
{
    public DespawnEntity(Entity entity) 
    {
        Entity = entity;
    }

    public Entity Entity {get;set;}
}