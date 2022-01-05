#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ResolutionScaler : MonoBehaviour
{
    [Range(2, 16)] [SerializeField] private float _scale = 2;

    private Camera _cameraComponent;
    private RenderTexture _texture;

    private void Start() 
    {
        CreateTexture();
    }

    private void CreateTexture()
    {
        int width = Mathf.RoundToInt(Screen.width / _scale);
        int height = Mathf.RoundToInt(Screen.height / _scale);

        _texture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        _texture.antiAliasing = 1;

        _cameraComponent = GetComponent<Camera>();
    }

#if UNITY_EDITOR
    
    private void Update() 
    {
        if (EditorApplication.isPlaying) return;
        CreateTexture();
    }

#endif

    private void OnPreRender() 
    {
        _cameraComponent.targetTexture = _texture;
    }

    private void OnPostRender() 
    {
        _cameraComponent.targetTexture = null;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest) 
    {
        src.filterMode = FilterMode.Point;

        Graphics.Blit(src, dest);
    }
}
