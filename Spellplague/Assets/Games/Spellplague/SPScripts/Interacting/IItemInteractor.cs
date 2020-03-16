namespace Spellplague.Interacting
{
    public interface IObjectInteractor
    {
        string GetName();
        bool ShowSuffix();
        bool HasEvent();
        IHasInteractorEvent GetEvent();
    }
}