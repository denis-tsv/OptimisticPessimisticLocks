namespace LocksApi.Services;

public interface ICurrentUserService
{
    int CurrentUserId { get; }
}

public class CurrentUserService : ICurrentUserService
{
    public int CurrentUserId => 1;
}