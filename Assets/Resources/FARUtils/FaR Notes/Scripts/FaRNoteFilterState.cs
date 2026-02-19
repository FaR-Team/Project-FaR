namespace FARUtils.Notes
{
    using System.Collections.Generic;

    [System.Serializable]
    public class FaRNoteFilterState
    {
        public List<FaRNoteCategory> visibleCategories = new List<FaRNoteCategory>();

        public void EnsureInitialized()
        {
            if (visibleCategories.Count > 0)
                return;

            foreach (FaRNoteCategory category in System.Enum.GetValues(typeof(FaRNoteCategory)))
                visibleCategories.Add(category);
        }

        public bool IsVisible(FaRNoteCategory category)
        {
            return visibleCategories.Contains(category);
        }

        public void SetAll(bool value)
        {
            visibleCategories.Clear();

            if (value)
            {
                foreach (FaRNoteCategory category in System.Enum.GetValues(typeof(FaRNoteCategory)))
                    visibleCategories.Add(category);
            }
        }
        public bool AreAllActive()
        {
            foreach (FaRNoteCategory category in System.Enum.GetValues(typeof(FaRNoteCategory)))
            {
                if (!IsVisible(category))
                    return false;
            }
            return true;
        }
        public void Toggle(FaRNoteCategory category, bool value)
        {
            if (value)
            {
                if (!visibleCategories.Contains(category))
                    visibleCategories.Add(category);
            }
            else
            {
                visibleCategories.Remove(category);
            }
        }
    }
}