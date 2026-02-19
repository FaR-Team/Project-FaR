using System.Text.RegularExpressions;

namespace jandd661.EditorTools.SearchAndReplace
{
    public class SearchUtils
    {
        public enum SearchType { StartsWith, EndsWith, Contains}
        public static bool IsASearchMatch(string objectName, string searchString, SearchType searchType)
        {
            bool outBool = false;
            switch (searchType)
            {
                case SearchType.StartsWith:
                    outBool = objectName.StartsWith(searchString, true, System.Globalization.CultureInfo.CurrentCulture);
                    break;
                case SearchType.EndsWith:
                    outBool = objectName.EndsWith(searchString, true, System.Globalization.CultureInfo.CurrentCulture);
                    break;
                case SearchType.Contains:
                    string obName = objectName.ToLower();
                    outBool = obName.Contains(searchString.ToLower());
                    break;
                default:
                    outBool = false;
                    break;
            }
            return outBool;
        }
    }
}
