using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Map : MonoBehaviour
{
    // shader and map aka plane sclaing i guess
    public static Texture2D _mapTexture;

    public Material mapMaterial;
    private Renderer rend;

    private void Start()
    {
        int width = _mapTexture.width;
        int height = _mapTexture.height;

        Vector3 position = new Vector3((width / 2) - 0.5f, 0, (height / 2) - 0.5f);
        Vector3 scale = new Vector3((width / 10f), 1f, (height / 10f));

        transform.position = position;
        transform.localScale = scale;

        rend = GetComponent<Renderer>();
        rend.material.shader = mapMaterial.shader;

        //rend.material.SetFloat("CellAlpha", 0);
        rend.material.SetVector("GridSize", new Vector4(width, height, 0, 0));
        rend.material.SetTexture("MainTexture", _mapTexture);

        //Debug.Log(width + " " + height);
        //Debug.Log(position);
        //Debug.Log(scale);
        //Debug.Log(rend.material.shader.name);
    }
}