using System.Collections;
using UnityEngine;
using TMPro;

public class WheelSpinner : MonoBehaviour
{
    [Header("Wheel")]
    public Transform wheel;
    public string[] prizes = { "Minori", "Shizuku", "Airi", "Haruka" };
    public float deceleration = 300f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip spinClip;
    public AudioClip resultClip;

    [Header("Result Display")]
    public TMP_Text resultText;

    [Header("Character Popup")]
    public SpriteRenderer leftImage, rightImage;
    public Sprite[] characterSprites;   
    public float hideDistance = 10f;    
    public float bounce = 0.6f;         
    public float stepTime = 0.15f;      

    float speed, startSpeed;
    bool spinning;
    Vector3 leftPos, rightPos;

    void Awake()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        spinning = false;
        if (resultText) resultText.text = "Click to spin!";

        if (leftImage) leftPos = leftImage.transform.position;
        if (rightImage) rightPos = rightImage.transform.position;
        SetPopY(-hideDistance);
    }

    void OnMouseDown()
    {
        if (spinning) return;

        startSpeed = speed = Random.Range(800f, 1200f);
        spinning = true;

        if (resultText) resultText.text = "";
        if (audioSource && spinClip)
        {
            audioSource.clip = spinClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (!spinning) return;

        wheel.Rotate(0, 0, speed * Time.deltaTime);
        speed -= deceleration * Time.deltaTime;
        if (audioSource) audioSource.pitch = Mathf.Lerp(0.6f, 1.2f, speed / startSpeed);

        if (speed <= 0) StopWheel();
    }

    void StopWheel()
    {
        spinning = false;
        if (audioSource)
        {
            audioSource.Stop();
            audioSource.pitch = 1f;
            if (resultClip) audioSource.PlayOneShot(resultClip);
        }

        float seg = 360f / prizes.Length;
        int index = (int)((wheel.eulerAngles.z + seg / 2f) / seg) % prizes.Length;

        if (resultText) resultText.text = "Result: " + prizes[index];
        if (index < characterSprites.Length/2) StartCoroutine(PopUp(characterSprites[2*index], characterSprites[2*index+1]));
    }

    IEnumerator PopUp(Sprite leftSprite, Sprite rightSprite)
    {
        if (leftImage) leftImage.sprite = leftSprite;
        if (rightImage) rightImage.sprite = rightSprite;

        float[] path = { -hideDistance, 0f, -bounce, 0f, -bounce, -hideDistance };

        for (int i = 1; i < path.Length; i++)
        {
            for (float t = 0; t < stepTime; t += Time.deltaTime)
            {
                SetPopY(Mathf.Lerp(path[i - 1], path[i], t / stepTime));
                yield return null;
            }
        }
        SetPopY(-hideDistance);
    }

    void SetPopY(float y)
    {
        if (leftImage) leftImage.transform.position = leftPos + Vector3.up * y;
        if (rightImage) rightImage.transform.position = rightPos + Vector3.up * y;
    }
}