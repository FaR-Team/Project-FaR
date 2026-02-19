namespace FARUtils.Notes
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;
    using System;


    [CustomEditor(typeof(FaRNoteDatabase))]
    public class FaRNoteDatabaseEditor : Editor
    {
        private VisualElement root;
        private StyleSheet styleSheet;
        private FaRNoteStyleConfig styleConfig;
        private FaRNoteDatabase database;

        private void OnEnable()
        {
            database = (FaRNoteDatabase)target;
            LoadStyleConfig();
        }

        private void LoadStyleConfig()
        {
            string[] guids = AssetDatabase.FindAssets("t:FaRNoteStyleConfig");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                styleConfig = AssetDatabase.LoadAssetAtPath<FaRNoteStyleConfig>(path);
            }
        }

        private void LoadStyleSheet()
        {
            string[] guids = AssetDatabase.FindAssets("FaRNotesStyles t:StyleSheet");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
                if (styleSheet != null)
                {
                    root.styleSheets.Add(styleSheet);
                }
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();
            root.AddToClassList("far-inspector-root");
            LoadStyleSheet();

            if (styleConfig == null)
            {
                root.Add(new HelpBox("No Style Config found.", HelpBoxMessageType.Warning));
                return root;
            }

            // Bind to selectedIndex to refresh when changed from Scene view
            root.TrackPropertyValue(serializedObject.FindProperty("selectedIndex"), _ => RefreshUI());

            RefreshUI();
            return root;
        }

        private void RefreshUI()
        {
            root.Clear();
            serializedObject.Update();

            if (database.selectedIndex < 0 || database.selectedIndex >= database.notes.Count)
            {
                root.Add(new HelpBox("No note selected", HelpBoxMessageType.Info));
                return;
            }

            var note = database.notes[database.selectedIndex];
            var notesProp = serializedObject.FindProperty("notes");
            
            if (notesProp == null || notesProp.arraySize <= database.selectedIndex)
            {
                root.Add(new HelpBox("Error accessing note data", HelpBoxMessageType.Error));
                return;
            }

            var noteProperty = notesProp.GetArrayElementAtIndex(database.selectedIndex);

            // Note Card
            var card = new VisualElement();
            card.AddToClassList("far-card");

            if (note.isLocked)
            {
                var titleLabel = new Label(string.IsNullOrEmpty(note.title) ? "Untitled Note" : note.title);
                titleLabel.AddToClassList("far-title");
                card.Add(titleLabel);

                card.Add(CreateFieldRow("Category", note.category.ToString()));
                card.Add(CreateFieldRow("Status", note.status.ToString()));

                if (note.hasAttach)
                    card.Add(CreateFieldRow("Linked to", note.linkedObject.name));

                var textLabel = new Label(note.text);
                textLabel.style.whiteSpace = WhiteSpace.Normal;
                textLabel.style.marginTop = 10;
                textLabel.style.color = new Color(0.9f, 0.9f, 0.9f);
                card.Add(textLabel);
            }
            else
            {
                // Title Field
                var titleField = new TextField("Title");
                titleField.bindingPath = noteProperty.FindPropertyRelative("title").propertyPath;
                titleField.AddToClassList("far-field");
                card.Add(titleField);

                // Category Field
                var categoryField = new EnumField("Category", note.category);
                categoryField.bindingPath = noteProperty.FindPropertyRelative("category").propertyPath;
                categoryField.AddToClassList("far-field");
                card.Add(categoryField);

                // Status Field
                var statusField = new EnumField("Status", note.status);
                statusField.bindingPath = noteProperty.FindPropertyRelative("status").propertyPath;
                statusField.AddToClassList("far-field");
                card.Add(statusField);

                // Linked GameObject
                var transformField = new ObjectField("Linked GameObject") 
                { 
                    objectType = typeof(GameObject),
                    allowSceneObjects = true 
                };
                transformField.bindingPath = noteProperty.FindPropertyRelative("linkedObject").propertyPath;
                transformField.AddToClassList("far-field");
                card.Add(transformField);

                // Position
                var posField = new PropertyField(noteProperty.FindPropertyRelative("position"), note.hasAttach ? "Local Position" : "World Position");
                posField.AddToClassList("far-field");
                card.Add(posField);

                // Note Text
                var textField = new TextField("Note Text") { multiline = true };
                textField.bindingPath = noteProperty.FindPropertyRelative("text").propertyPath;
                textField.style.height = 80;
                textField.style.marginTop = 10;
                card.Add(textField);
                
                // Bind the card to the whole SerializedObject so bindingPath works
                card.Bind(serializedObject);
            }

            root.Add(card);

            // Metadata Section
            var metadataHeader = new Label("METADATA");
            metadataHeader.style.marginTop = 10;
            metadataHeader.style.fontSize = 9;
            metadataHeader.style.color = new Color(0.5f, 0.5f, 0.5f);
            root.Add(metadataHeader);

            var metaBox = new VisualElement();
            metaBox.style.paddingLeft = 10;
            metaBox.Add(CreateFieldRow("Author", note.author));
            metaBox.Add(CreateFieldRow("Created", note.Timestamp.ToString()));
            metaBox.Add(CreateFieldRow("Branch", note.gitBranch));
            root.Add(metaBox);

            // Action Buttons
            if (note.isLocked)
            {
                var editButton = new Button(() => {
                    Undo.RecordObject(database, "Unlock Note");
                    note.Unlock();
                    RefreshUI();
                }) { text = "Enable Editing" };
                editButton.AddToClassList("far-button");
                root.Add(editButton);
            }
            else
            {
                var applyButton = new Button(() => {
                    serializedObject.ApplyModifiedProperties();
                    Undo.RecordObject(database, "Apply Changes");
                    note.Lock();
                    RefreshUI();
                    EditorUtility.SetDirty(database);
                }) { text = "Apply Changes" };
                applyButton.AddToClassList("far-button");
                root.Add(applyButton);
            }

            // Comments Section
            var commentsHeader = new Label("COMMENTS");
            commentsHeader.style.marginTop = 15;
            commentsHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            commentsHeader.style.color = new Color(0.5f, 0.5f, 0.5f);
            root.Add(commentsHeader);

            var commentsContainer = new VisualElement();
            commentsContainer.AddToClassList("far-comment-container");
            foreach (var comment in note.comments)
            {
                commentsContainer.Add(CreateCommentBubble(comment));
            }
            root.Add(commentsContainer);

            // Add Comment Section
            var commentField = new TextField { multiline = true };
            commentField.style.marginTop = 10;
            root.Add(commentField);

            var postButton = new Button(() => {
                if (!string.IsNullOrEmpty(commentField.value)) {
                    Undo.RecordObject(database, "Add Comment");
                    note.AddComment(commentField.value, System.Environment.UserName);
                    commentField.value = "";
                    RefreshUI();
                    EditorUtility.SetDirty(database);
                }
            }) { text = "Post Comment" };
            postButton.AddToClassList("far-button");
            root.Add(postButton);

            // Delete Button at bottom
            var deleteButton = new Button(() => {
                if (EditorUtility.DisplayDialog("Delete Note", "Are you sure you want to delete this note?", "Yes", "No")) {
                    Undo.RecordObject(database, "Delete Note");
                    database.notes.RemoveAt(database.selectedIndex);
                    database.selectedIndex = -1;
                    RefreshUI();
                    EditorUtility.SetDirty(database);
                }
            }) { text = "Delete Note" };
            deleteButton.style.marginTop = 20;
            deleteButton.style.backgroundColor = new Color(0.8f, 0.2f, 0.2f, 0.6f);
            root.Add(deleteButton);
        }

        private VisualElement CreateFieldRow(string label, string value)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceBetween;
            row.style.marginBottom = 2;

            var l = new Label(label);
            l.AddToClassList("far-label");
            var v = new Label(value);
            v.AddToClassList("far-value");

            row.Add(l);
            row.Add(v);
            return row;
        }

        private VisualElement CreateCommentBubble(FaRNoteComment comment)
        {
            var bubble = new VisualElement();
            bubble.AddToClassList("far-comment-bubble");

            var author = new Label(comment.author);
            author.AddToClassList("far-comment-author");
            bubble.Add(author);

            var msg = new Label(comment.message);
            msg.AddToClassList("far-comment-text");
            bubble.Add(msg);

            var meta = new Label(comment.Timestamp.ToString("HH:mm - dd/MM"));
            meta.AddToClassList("far-comment-meta");
            bubble.Add(meta);

            return bubble;
        }
    }
#endif
}
