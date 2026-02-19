namespace FARUtils.Notes
{

    using UnityEngine;

    [CreateAssetMenu(menuName = "Scene Notes/Style Config")]
    public class FaRNoteStyleConfig : ScriptableObject
    {
        [Header("Icon")]
        public float iconSize = 32f;
        public float iconHoverScale = 1.2f;
        public float iconSelectedScale = 1.3f;

        public bool constantIconSize = true;


        [Space(5)]
        [Header("Inspector Text")]
        public int inspectorFontSize = 13;
        public Color inspectorTextColor = Color.white;
        public bool constantTextSize = true;


        [Space(5)]
        [Header("Buttons")]
        public int buttonFontSize = 12;
        public float buttonHeight = 25f;
        public float buttonWidth = 0f;
        public Color buttonColor = Color.gray;
        public Color buttonTextColor = Color.white;


        [Space(5)]
        [Header("Comments")]
        public int commentFontSize = 11;
        public Color commentAuthorColor = Color.cyan;
        public Color commentTextColor = Color.white;
        public Color commentBackground = new Color(0.2f, 0.2f, 0.2f);

        [Space(5)]
        [Header("Spacing")]
        public float verticalSpacing = 5f;
        public Color commentPlaceholderColor;
    }
}