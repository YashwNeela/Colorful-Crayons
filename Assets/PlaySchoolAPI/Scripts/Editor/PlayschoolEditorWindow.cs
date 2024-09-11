using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class PlayschoolEditorWindow : EditorWindow
{
    private string inputText = System.IO.Path.GetDirectoryName(Application.dataPath); // This will store the project root directory
    private Texture2D image; // This will store the image to be displayed
    private Texture2D developerPhoto; // This will store the developer's photo

    private const int ImageSize = 250; // Size for the image and frame

    // Add a menu item to open this window
    [MenuItem("PlaySchool/Update Submodule")]
    private static void ShowWindow()
    {
        // Show the window with a title
        GetWindow<PlayschoolEditorWindow>("Custom Window");
    }

    // Create the GUI for the window
    private void OnEnable()
    {
        // Set inputText to the project's root folder path (without Assets folder)
        inputText = System.IO.Path.GetDirectoryName(Application.dataPath);

        // Load an image from the Resources folder (or any other path)
        image = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/PlaySchoolAPI/Images/myImage.jpg", typeof(Texture2D));

        // Load developer's photo from the Resources folder (or any other path)
        developerPhoto = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Resources/developerPhoto.jpg", typeof(Texture2D));
    }

    private void OnGUI()
    {
        // Add a "Developed by" label
        GUILayout.Label("Developed by:", EditorStyles.boldLabel);

        if (image != null)
        {
            // Define the frame area with fixed size for the main image
            Rect imageRect = GUILayoutUtility.GetRect(ImageSize, ImageSize, GUILayout.Width(ImageSize + 10), GUILayout.Height(ImageSize + 10));

            // Create a custom GUIStyle for the frame around the image
            GUIStyle imageFrameStyle = new GUIStyle();
            imageFrameStyle.normal.background = Texture2D.whiteTexture; // Set frame color (white in this case)
            imageFrameStyle.border = new RectOffset(5, 5, 5, 5); // Set border size

            // Draw the frame around the main image
            GUI.Box(imageRect, GUIContent.none, imageFrameStyle);

            // Draw the image inside the frame, scaled to fit within the frame
            GUI.DrawTexture(imageRect, image, ScaleMode.ScaleToFit, true);
        }
        else
        {
            GUILayout.Label("Main image not found");
        }

        GUILayout.Space(50);
        // Add a label for the project location
        GUILayout.Label("Project Location", EditorStyles.boldLabel);

        // Display the non-editable label with the project root location
        EditorGUILayout.LabelField("Project Path:", inputText);

        // Add a button, and perform action when clicked
        if (GUILayout.Button("Submit"))
        {
            
            // Determine the platform and run the appropriate command
            #if UNITY_EDITOR_WIN
            // Windows
            string gitCommand = "git submodule update --remote";
            string fullCommand = $"/k cd /d \"{inputText}\" && {gitCommand}";
            Process.Start("cmd.exe", fullCommand);
            #elif UNITY_EDITOR_OSX
            // macOS
            string gitCommand = "git submodule update --remote";
            string fullCommand = $"cd \"{inputText}\"; {gitCommand}";
            string terminalCommand = $"/bin/zsh -c \"{fullCommand}\"";
            Process.Start("open", $"-a Terminal \"{terminalCommand}\"");
         //   Process.Start("open", "-a Terminal");
            #endif

            UnityEngine.Debug.Log("Command executed for platform.");
        }
    }
}
