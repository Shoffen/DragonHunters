using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DamageLabel : MonoBehaviour
{
    public TMP_Text damage;
    public float hitDamage;
    public float speed;
    float random;

    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        transform.SetParent(null);
        random = random = Random.Range(1, 4);
        speed = speed * random;
    }

    // Update is called once per frame
    void Update()
    {
        damage.text = Mathf.RoundToInt(hitDamage).ToString();
        transform.Translate(0, speed * Time.deltaTime, 0);
    }
}
