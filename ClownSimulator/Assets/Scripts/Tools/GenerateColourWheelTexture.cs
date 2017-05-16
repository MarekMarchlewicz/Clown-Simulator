using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GenerateColourWheelTexture : MonoBehaviour
{
    private const int WIDTH = 256;
    private const int HEIGHT = 256;

    [MenuItem("Tools/Generate Colour Wheel Texture")]
    private static void Generate ()
    {
        Texture2D texture = new Texture2D(WIDTH, HEIGHT);

        for(int x = 0; x < WIDTH; x++)
        {
            for(int y = 0; y < HEIGHT; y++)
            {
                Vector2 current = new Vector2(x - WIDTH / 2, y - HEIGHT / 2);

                float angle = Vector2.Angle(Vector2.up, current) * Mathf.Sign(-current.x);
                if (angle < 0f) angle += 360f;
                angle /= 360f;

                float s = Mathf.Clamp01(current.magnitude / (WIDTH / 2f));
    
                Color colour = Color.HSVToRGB(angle, s, 1f);

                texture.SetPixel(x, y, colour);
            }
        }

        texture.Apply();

        File.WriteAllBytes(Application.dataPath + "/ColourWheel.png", texture.EncodeToPNG());

        AssetDatabase.Refresh();
	}
}
