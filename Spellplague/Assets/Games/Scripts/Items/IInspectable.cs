using UnityEngine;

namespace Spellplague.Items
{
    public interface IInspectable
    {
        string GetName();
        Transform GetInspectTransform();
    }
}