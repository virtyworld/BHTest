using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


public class PlayerDamageComponent : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer mesh;
    [SerializeField] private float colorCoolDown;

    private bool isInvulnerable;
    private Color currentMeshColor;
   
    public bool IsInvulnerable
    {
        get => isInvulnerable;
        private set => isInvulnerable = value;
    }


    private void Start()
    {
        currentMeshColor = mesh.material.color;
    }

    public void AddDamageByDash()
    {
        if (!isInvulnerable)
        {
            StartCoroutine(CurrentColorChanger());
            StartCoroutine(Invulnerable());
        }
    }
    
    private IEnumerator CurrentColorChanger()
    {
        mesh.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        yield return new WaitForSeconds(colorCoolDown);
        mesh.material.color = currentMeshColor;
    }

    private IEnumerator Invulnerable()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(3f);
        isInvulnerable = false;
    }
}
