using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using TMPro;

namespace UnityEngine.Localization.Samples
{
    public class LanguageTMPDropdown : MonoBehaviour
    {
        public GameObject m_dropdown;
        TMP_Dropdown m_Dropdown;
        AsyncOperationHandle m_InitializeOperation;
        public string identifier;

        void Start()
        {
            // First we setup the m_Dropdown component.
            m_Dropdown = m_dropdown.GetComponent<TMP_Dropdown>();
            m_Dropdown.onValueChanged.AddListener(OnSelectionChanged);

            // Clear the options an add a loading message while we wait for the localization system to initialize.
            m_Dropdown.ClearOptions();
            m_Dropdown.options.Add(new TMP_Dropdown.OptionData("Loading..."));
            m_Dropdown.interactable = false;

            // SelectedLocaleAsync will ensure that the locales have been initialized and a locale has been selected.
            m_InitializeOperation = LocalizationSettings.SelectedLocaleAsync;
            if (m_InitializeOperation.IsDone)
            {
                InitializeCompleted(m_InitializeOperation);
            }
            else
            {
                m_InitializeOperation.Completed += InitializeCompleted;
            }
        }

        void InitializeCompleted(AsyncOperationHandle obj)
        {
            // Create an option in the m_Dropdown for each Locale
            var options = new List<string>();
            int selectedOption = 0;
            var locales = LocalizationSettings.AvailableLocales.Locales;
            for (int i = 0; i < locales.Count; ++i)
            {
                var locale = locales[i];
                if (LocalizationSettings.SelectedLocale == locale)
                    selectedOption = i;

                var displayName = locales[i].Identifier.CultureInfo != null ? locales[i].Identifier.CultureInfo.NativeName : locales[i].ToString();
                options.Add(displayName);
            }

            // If we have no Locales then something may have gone wrong.
            if (options.Count == 0)
            {
                options.Add("No Locales Available");
                m_Dropdown.interactable = false;
            }
            else
            {
                m_Dropdown.interactable = true;
            }

            m_Dropdown.ClearOptions();
            m_Dropdown.AddOptions(options);
            m_Dropdown.SetValueWithoutNotify(selectedOption);
            if (m_Dropdown.value == 0)
                { identifier = "en"; }
            else if (m_Dropdown.value == 1)
                { identifier = "es"; }
            else if (m_Dropdown.value == 2)
                { identifier = "sw"; }

            LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
        }

        void OnSelectionChanged(int index)
        {
            LocalizationSettings.SelectedLocaleChanged -= LocalizationSettings_SelectedLocaleChanged;

            var locale = LocalizationSettings.AvailableLocales.Locales[index];
            LocalizationSettings.SelectedLocale = locale;

            LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;

            if (m_Dropdown.value == 0)
                { identifier = "en"; }
            else if (m_Dropdown.value == 1)
                { identifier = "es"; }
            else if (m_Dropdown.value == 2)
                { identifier = "sw"; }
        }

        void LocalizationSettings_SelectedLocaleChanged(Locale locale)
        {
            var selectedIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(locale);
            m_Dropdown.SetValueWithoutNotify(selectedIndex);
        }
    }
}
