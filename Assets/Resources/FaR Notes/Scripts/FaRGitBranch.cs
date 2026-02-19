namespace FARUtils.Notes
{
#if UNITY_EDITOR
    using System.IO;
    using UnityEngine;

    public static class GitHead
    {

        public static string GetGitBranch()
        {
            string gitHead = Path.Combine(Application.dataPath, "../.git/HEAD");
            if (!File.Exists(gitHead))
                return null;

            string content = File.ReadAllText(gitHead).Trim();
            if (content.StartsWith("ref:"))
            {
                return content["ref: refs/heads/".Length..].Trim();
            }

            return null;
        }
    }
#endif

}