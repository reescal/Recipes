namespace Recipes.Shared.Models;

public class IndexHashSet<T> : HashSet<T>
{
    public T this[int Index] => this.ElementAt(Index);
}
