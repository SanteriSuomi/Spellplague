using UnityEngine;

namespace Spellplague.Interacting
{
    /// <summary>
    /// Game objects with item interactor will have names popup in the UI and can have special "interact events".
    /// </summary>
    public class ItemInteractor : MonoBehaviour, IItemInteractor
    {
        [SerializeField] [Tooltip("Name that shows when player is targeting the item.")]
        private string interactItemName = default;
        public string GetName() => interactItemName;

        [SerializeField] [Tooltip("Specify whether to show the interact suffix (e.g Key [E]).")]
        private bool showSuffix = default;
        public bool ShowSuffix() => showSuffix;

        [SerializeField] 
        [Tooltip("Event that happens when this object is interacted with. Object has to have a script that inherits IHasInteractEvent. " +
            "Only for special interact events, not the same as inspectable, takeable or item event interface/scripts.")]
        private bool hasInteractEvent = default;
        public bool HasEvent() => hasInteractEvent;

        private IHasInteractorEvent interactorEvent;
        public IHasInteractorEvent GetEvent() 
            => interactorEvent;

        private void Awake() => InitializeEvent();

        private void InitializeEvent()
        {
            if (hasInteractEvent)
            {
                bool gotComponent = TryGetComponent(out interactorEvent);
                if (!gotComponent)
                {
                    Debug.LogError("You have marked this as hasEvent, but there is not executable event.");
                }
            }
        }
    }
}