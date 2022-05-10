using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FileSystem : MonoBehaviour
{
    string currentPath = "Untitled.png";
    void Awake()
    {
        GetComponent<Canvas>().enabled = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            Texture2D texture = GetScreenShot();
            SaveScreenshotAndOpenFile(texture);
        }

        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Texture2D texture = GetScreenShot();
            string path = EditorUtility.SaveFilePanel("Save texture as PNG","","","png");

            if (path.Length != 0)
            {
                currentPath = path;
                SaveScreenshotAndOpenFile(texture);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetComponent<Canvas>().enabled = false;
        }
    }

    Texture2D GetScreenShot()
    {
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();

        texture = CropToSquare(texture);

        texture = ResizeTexture(texture, 2048, 2048);
        return texture;
    }

    void SaveScreenshotAndOpenFile(Texture2D texture)
    {
       

        SaveTexture(currentPath, texture);

        OpenImageFile(currentPath);
    }

    Texture2D CropToSquare(Texture2D source)
    {
        Color[] colors;
        int textureSize;
        if (source.width > source.height)
        {
            textureSize = source.height;
            colors = source.GetPixels((source.width - source.height) / 2, 0, source.height, source.height);
        }
        else
        {
            textureSize = source.height;
            colors = source.GetPixels(0, (source.height - source.width) / 2, source.width, source.width);
        }

        Texture2D texture = new Texture2D(textureSize, textureSize);
        texture.SetPixels(colors);

        return texture;
    }

    Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        Color32[] colors = texture.GetPixels32();
        int l = colors.Length;
        for (int i = 0; i < l; i++)
        {
            colors[i] = source.GetPixelBilinear(1f * (i % width) / width, Mathf.Floor(i / width) / height);
        }

        texture.SetPixels32(colors);
        texture.Apply();
        return texture;
    }

    void SaveTexture(string path, Texture2D texture)
    {
        if (File.Exists(path))
        {
            OpenImageCompareCanvas(texture);
        }


        File.WriteAllBytes(path, texture.EncodeToPNG());
    }

    void OpenImageCompareCanvas(Texture2D texture)
    {
        Texture2D oldTexture = LoadPNGAsTexture(currentPath);
        ApplyTextureToImageUI(GameObject.Find("OldImage"), oldTexture);
        ApplyTextureToImageUI(GameObject.Find("NewImage"), texture);
        GetComponent<Canvas>().enabled = true;
    }

    void ApplyTextureToImageUI(GameObject rawImage, Texture2D texture)
    {
        rawImage.GetComponent<RawImage>().texture = texture;
    }

    Texture2D LoadPNGAsTexture(string path)
    {
        Texture2D texture = new Texture2D(2048,2048);
        byte[] bytes = File.ReadAllBytes(path);
        texture.LoadImage(bytes);
        return texture;
    }

    void OpenImageFile(string path)
    {
        string openPath = path.Replace('/', '\\');
        Debug.Log("Openning :" + openPath);
        System.Diagnostics.Process.Start("explorer.exe", openPath);
    }
}
