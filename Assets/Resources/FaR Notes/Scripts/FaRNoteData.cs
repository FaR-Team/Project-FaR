namespace FARUtils.Notes
{

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Serializable]
    public class FaRNoteData
    {
        public Vector3 position;

        public string title;
        public string text;

        public Sprite icon;
        public float iconScale = 1f;
        public float currentScale = 1f;

        public GameObject linkedObject; // cambiado de Transform para mayor compatibilidad

        public FaRNoteCategory category = FaRNoteCategory.Comment;
        public Status status = Status.Open;

        public string gitBranch;
        public string author;
        public long timestampTicks;

        public bool isLocked = true;
        public bool hasAttach => linkedObject != null;
        public List<FaRNoteComment> comments = new List<FaRNoteComment>();

        public DateTime Timestamp => new DateTime(timestampTicks);

        public Vector3 WorldPosition
        {
            get
            {
                if (linkedObject != null)
                    return linkedObject.transform.position + position; // relativo al transform
                return position;
            }
            set
            {
                if (linkedObject != null)
                    position = value - linkedObject.transform.position;
                else
                    position = value;
            }
        }
        public void Initialize(string userName, string gitBranch)
        {
            author = userName;
            timestampTicks = DateTime.Now.Ticks;
            status = Status.Open;
            isLocked = false;
            this.gitBranch = gitBranch;
        }

        public void Lock()
        {
            isLocked = true;
        }

        public void Unlock()
        {
            isLocked = false;
        }

        public void AddComment(string message, string userName)
        {
            FaRNoteComment comment = new FaRNoteComment();
            comment.Initialize(message, userName);
            comments.Add(comment);
        }

        public void AddSystemChangeComment(FaRNoteData before, string userName)
        {
            if (before == null) return;

            StringBuilder builder = new StringBuilder();

            if (before.text != text && !string.IsNullOrEmpty(before.text))
                builder.AppendLine($"Text changed: \"{before.text}\" → \"{text}\"");

            if (before.status != status)
                builder.AppendLine($"Status changed: {before.status} → {status}");

            if (before.position != position && before.position != Vector3.zero)
                builder.AppendLine($"Position changed: {before.position} → {position}");

            if (before.category != category)
                builder.AppendLine($"Category changed: {before.category} → {category}");

            if (builder.Length > 0)
                AddComment(builder.ToString(), userName);
        }

        public FaRNoteData Clone()
        {
            return new FaRNoteData
            {
                position = position,
                title = title,
                text = text,
                icon = icon,
                iconScale = iconScale,
                category = category,
                status = status,
                author = author,
                timestampTicks = timestampTicks,
                isLocked = isLocked
            };
        }
    }

    [Serializable]
    public class FaRNoteComment
    {
        public string message;
        public string author;
        public long timestampTicks;

        public DateTime Timestamp => new DateTime(timestampTicks);

        public void Initialize(string msg, string user)
        {
            message = msg;
            author = user;
            timestampTicks = DateTime.Now.Ticks;
        }
    }

    public enum FaRNoteCategory
    {
        Comment,
        Bug,
        Issue,
        Documentation,
        Other
    }

    public enum Status
    {
        Open,
        InProgress,
        Resolved,
        Closed
    }
    [System.Serializable]
    public class CategoryIcon
    {
        public FaRNoteCategory category;
        public Sprite icon;
    }
}
