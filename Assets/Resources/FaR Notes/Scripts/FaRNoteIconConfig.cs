namespace FARUtils.Notes
{
#if UNITY_EDITOR
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Scene Notes/Icon Config")]
    public class FaRNoteIconConfig : ScriptableObject
    {
        public Sprite commentIcon;
        public Sprite bugIcon;
        public Sprite issueIcon;
        public Sprite documentationIcon;
        public Sprite otherIcon;

        public List<CategoryIcon> icons;

        public Sprite GetIcon(FaRNoteCategory category)
        {
            for (int i = 0; i < icons.Count; i++)
            {
                if (icons[i].category == category)
                    return icons[i].icon;
            }

            return null;
        }

    }

#endif
}