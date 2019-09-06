using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class RippleEffect : MonoBehaviour
{
    public AnimationCurve Waveform;

    [Range(0.01f, 1.0f)]
    public float RefractionStrength = 0.5f;

    public Color ReflectionColor = Color.gray;

    [Range(0.01f, 1.0f)]
    public float ReflectionStrength = 0.7f;

    [Range(1.0f, 5.0f)]
    public float WaveSpeed = 1.25f;

    [Range(0.0f, 2.0f)]
    public float DropInterval = 0.5f;

    [SerializeField]
    private Shader _shader = null;

    private Camera _camera;
    private Droplet[] _droplets;
    private Texture2D _gradTexture;
    private Material _material;
    private float _timer;
    private int _dropCount;

    void Awake()
    {
        _droplets = new Droplet[3] { new Droplet(), new Droplet(), new Droplet() };
        _camera = GetComponent<Camera>();

        _gradTexture = new Texture2D(2048, 1, TextureFormat.Alpha8, false);
        _gradTexture.wrapMode = TextureWrapMode.Clamp;
        _gradTexture.filterMode = FilterMode.Bilinear;
        for (var i = 0; i < _gradTexture.width; i++)
        {
            var x = 1.0f / _gradTexture.width * i;
            var a = Waveform.Evaluate(x);
            _gradTexture.SetPixel(i, 0, new Color(a, a, a, a));
        }
        _gradTexture.Apply();
        _material = new Material(_shader) { hideFlags = HideFlags.DontSave };
        _material.SetTexture("_GradTex", _gradTexture);
        UpdateShaderParameters();
    }

    void Update()
    {
        if (DropInterval > 0)
        {
            _timer += Time.deltaTime;
            while (_timer > DropInterval)
            {
                //Emit();
                _timer -= DropInterval;
            }
        }

        foreach (var d in _droplets) d.Update();
        UpdateShaderParameters();
    }

    private void UpdateShaderParameters()
    {
        _material.SetVector("_Drop1", _droplets[0].MakeShaderParameter(_camera.aspect));
        _material.SetVector("_Drop2", _droplets[1].MakeShaderParameter(_camera.aspect));
        _material.SetVector("_Drop3", _droplets[2].MakeShaderParameter(_camera.aspect));

        _material.SetColor("_Reflection", ReflectionColor);
        _material.SetVector("_Params1", new Vector4(_camera.aspect, 1, 1 / WaveSpeed, 0));
        _material.SetVector("_Params2", new Vector4(1, 1 / _camera.aspect, RefractionStrength, ReflectionStrength));
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _material);
    }

    public void Emit(Vector2 pos)
    {
        _droplets[_dropCount++ % _droplets.Length].Reset(pos);
    }

    private IEnumerator Stop()
    {
        yield return new WaitForSeconds(.3f);
    }

}
