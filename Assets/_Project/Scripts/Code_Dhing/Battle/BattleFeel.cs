using System.Collections;
using UnityEngine;

public class BattleFeel : MonoBehaviour
{
    [SerializeField] private VfxPool hitVfx;
    [SerializeField] private PixelBurst burst;
    [SerializeField] private Transform cameraRig;
    [SerializeField] private SpriteRenderer playerBody;
    [SerializeField] private SpriteRenderer enemyBody;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.15f;
    
    private BattleSystem battle;
    private Vector3 cameraHome;

    public void Construct(BattleSystem battleSystem)
    {
        battle = battleSystem;
        cameraHome = cameraRig.localPosition;
        battle.Hit += OnHit;
        battle.Healed += OnHealed;
    }

    void OnDestroy()
    {
        if (battle != null)
        {
            battle.Hit -= OnHit;
            battle.Healed -= OnHealed;
        }
        Time.timeScale = 1f;
    }

    private void OnHit(MonsterInstance target, int damage, bool crit, Element? elem)
    {
        SpriteRenderer body = target == battle.Player ? playerBody : enemyBody;
        
        hitVfx.Play(body.bounds.center); 
        
        Color c = battle.LastHitElement.HasValue
            ? StarterGiver.ElementColor(battle.LastHitElement.Value)
            : Color.white;
        
        burst.Play(body.bounds.center, c,
                   battle.LastHitElement.HasValue ? 22 : 12, 90f, 0.5f);

        float shakeTime = battle.LastHitCritical ? shakeDuration * 1.5f : shakeDuration;
        StartCoroutine(Shake(shakeTime));
        StartCoroutine(HitStop());
    }

    private void OnHealed(MonsterInstance t)
    {
        burst.Play(playerBody.bounds.center, HpColors.High, 16, 50f, 0.6f);
    }

    IEnumerator Shake(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            Vector3 offset = Random.insideUnitSphere * shakeMagnitude;
            offset.z = 0;
            cameraRig.localPosition = cameraHome + offset;
            yield return null;
        }
        cameraRig.localPosition = cameraHome;
    }
    IEnumerator HitStop()
    {
        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(0.06f);
        Time.timeScale = 1f;
    }
}