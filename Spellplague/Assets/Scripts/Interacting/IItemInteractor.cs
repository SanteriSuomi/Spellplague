namespace Spellplague.Interacting
{
    public interface IItemInteractor
    {
        string GetName();
        bool ShowSuffix();
        bool HasEvent();
        IHasInteractorEvent GetEvent();
    }
}