namespace App.Backend.DTO;

public interface IEntityConvertable<TEntity, TDto>
{
    abstract public static TDto FromEntity(TEntity item);
}