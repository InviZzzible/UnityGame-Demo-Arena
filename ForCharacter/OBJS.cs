using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBJS : MonoBehaviour
{
    private Rigidbody2D rbObj;
    private SpriteRenderer spriteRenderer;
    private float Height, Width;

    private AudioSource Hit;
    private AudioSource Block;
    private AudioSource HitFatality;
    private AudioSource BlockSword;

    private Transform target;

    private float accelFollowME = 1000f;
    private float accel = 35f;
    private float accelOwnLimbs = 100f;

    private Transform ObjectOfParticleSystem_Sparks;
    private ParticleSystem sparks;
    private Transform ObjectOfParticleSystem_Blood;
    private ParticleSystem blood;

    private List<string> listIgnore = new List<string>() {
        { "LeftWall" },
        { "Roof" },
        { "RightWall" },
        { "Ground" },
        { "CircleWall" },
        { "Body" },
        { "Head" },
        { "LeftShoulder" },
        { "RightShoulder" },
        { "LeftHand" },
        { "RightHand" },
        { "LeftLeg" },
        { "RightLeg" },
        { "LeftFoot" },
        { "RightFoot" },
        { "Horns" },
        { "Nimbus" },
        { "WeaponInLeftHand" }, // Knife, Sword, Mace, Shuriken, Staff, Stick, Shield, StickWithNails
        { "WeaponInRightHand" }, // Knife, Sword, Mace, Shuriken, Staff, Stick, Shield, StickWithNails
        { "WeaponInLeftFoot" }, // Knife, Shuriken
        { "WeaponInRightFoot" } // Knife, Shuriken
    };

    private List<string> __listIgnore = new List<string>() {
        { "LeftWall" },
        { "Roof" },
        { "RightWall" },
        { "Ground" },
        { "CircleWall" },
        { "__Body" },
        { "__Head" },
        { "__LeftShoulder" },
        { "__RightShoulder" },
        { "__LeftHand" },
        { "__RightHand" },
        { "__LeftLeg" },
        { "__RightLeg" },
        { "__LeftFoot" },
        { "__RightFoot" },
        { "__Horns" },
        { "__Nimbus" },
        { "__WeaponInLeftHand" }, // Knife, Sword, Mace, Shuriken, Staff, Stick, Shield, StickWithNails
        { "__WeaponInRightHand" }, // Knife, Sword, Mace, Shuriken, Staff, Stick, Shield, StickWithNails
        { "__WeaponInLeftFoot" }, // Knife, Shuriken
        { "__WeaponInRightFoot" } // Knife, Shuriken
    };


    void Start()
    {
        if(gameObject.name == "__Head") {
            target = GameObject.Find("Head").transform;
        }

        rbObj = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Height = spriteRenderer.sprite.rect.height;
        Width = spriteRenderer.sprite.rect.width;

        Hit = GameObject.Find("HitSound").GetComponent<AudioSource>();
        Block = GameObject.Find("BlockSound").GetComponent<AudioSource>();
        HitFatality = GameObject.Find("HitFatalitySound").GetComponent<AudioSource>();
        BlockSword = GameObject.Find("BlockSwordSound").GetComponent<AudioSource>();

        ObjectOfParticleSystem_Sparks = GameObject.Find("Sparks").transform;
        sparks = ObjectOfParticleSystem_Sparks.GetComponent<ParticleSystem>();
        ObjectOfParticleSystem_Blood = GameObject.Find("Blood").transform;
        blood = ObjectOfParticleSystem_Blood.GetComponent<ParticleSystem>();
    }

    void FixedUpdate()
    {
        if (target != null) {
            Vector3 move = target.position - transform.position;
            rbObj.AddForce(move.normalized * accelFollowME * Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }

    // Функция для определения обратного вектора в результате столкновения двух объектов:
    private Vector2 VectorDirect(Vector2 cont) {
        float koeff = 1f;
        Vector2 center = transform.position;
        Vector2 mirrow = cont - center;
        mirrow = -mirrow;
        return mirrow.normalized * koeff;
    }
	
    private void oncollisionBlockSword(Collider2D collision) {
        Vector3 cont = collision.transform.position;
        Vector2 move = VectorDirect(cont);
        rbObj.AddForce(move * accel, ForceMode2D.Impulse);
        BlockSword.Play();
        ShowEffect(new Color(230f, 130f, 50f, 255f));

        if (!sparks.isPlaying) {
            ObjectOfParticleSystem_Sparks.position = transform.position;
            sparks.Play();
        }
    }

    private void oncollisionBlock(Collider2D collision) {
        Vector3 cont = collision.transform.position;
        Vector2 move = VectorDirect(cont);
        rbObj.AddForce(move * accel, ForceMode2D.Impulse);
        Block.Play();
    }

    private void oncollisionHitFatality(Collider2D collision) {
        Vector3 cont = collision.transform.position;
        Vector2 move = VectorDirect(cont);
        rbObj.AddForce(move * accel, ForceMode2D.Impulse);
        HitFatality.Play();
        //ShowEffect(new Color(255f, 0f, 0f, 255f));

        if (!blood.isPlaying) {
            ObjectOfParticleSystem_Blood.position = transform.position;
            blood.Play();
        }
    }

    private void oncollisionEncrease(Collider2D collision) {
        print("Враг восстанавливает свое здоровье...");

        // ...

    }

    private void oncollisionHit(Collider2D collision) {
        Vector3 cont = collision.transform.position;
        Vector2 move = VectorDirect(cont);
        rbObj.AddForce(move * accel, ForceMode2D.Impulse);
        Hit.Play();
    }

    private void oncollisionGettingDamage(Collider2D collision) {
        Vector3 cont = collision.transform.position;
        Vector2 move = VectorDirect(cont);
        rbObj.AddForce(move * accel, ForceMode2D.Impulse);
        Hit.Play();
        ShowEffect(new Color(255f, 0f, 0f, 255f));
        if (!blood.isPlaying) {
            ObjectOfParticleSystem_Blood.position = transform.position;
            blood.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        GameObject oth = collision.gameObject;

        if(gameObject.name[0] != '_') {

            bool bFlag = false;
            foreach(string iter in listIgnore) {
                if (iter == oth.name) {
                    bFlag = true;
                    break;
                }
            }

            if (bFlag) {
                Vector3 cont = collision.transform.position;
                Vector2 move = VectorDirect(cont);
                rbObj.AddForce(move * accelOwnLimbs, ForceMode2D.Force);
            }
            else {
                if (gameObject.name == "Head") {
                    if (oth.name == "__Head") { // Столкновение головами.
                        Vector3 cont = collision.transform.position;
                        Vector2 move = VectorDirect(cont);
                        rbObj.AddForce(move * accel, ForceMode2D.Impulse);
                        Hit.Play();
                        ShowEffect(new Color(255f, 0f, 0f, 255f));
                    }
                    else if (oth.name == "__LeftHand" ||
                        oth.name == "__RightHand" ||
                        oth.name == "__LeftFoot" ||
                        oth.name == "__RightFoot") { // Столкновение головой с вражескими атакующими частями тела, кроме головы.
                        Vector3 cont = collision.transform.position;
                        Vector2 move = VectorDirect(cont);
                        rbObj.AddForce(move * accel, ForceMode2D.Impulse);
                        Hit.Play();
                        ShowEffect(new Color(255f, 0f, 0f, 255f));
                    }
                    else if(oth.name == "__LeftShoulder" ||
                            oth.name == "__RightShoulder" ||
                            oth.name == "__LeftLeg" ||
                            oth.name == "__RightLeg" ||
                            oth.name == "__Body") { // Столкновение головой по уязвимым частям тела врага.
                        Vector3 cont = collision.transform.position;
                        Vector2 move = VectorDirect(cont);
                        rbObj.AddForce(move * accel, ForceMode2D.Impulse);
                        Hit.Play();
                        ShowEffect(new Color(255f, 0f, 0f, 255f));
                    }
                    else if (oth.name == "__WeaponInLeftHand" ||
                             oth.name == "__WeaponInRightHand" ||
                             oth.name == "__WeaponInLeftFoot" ||
                             oth.name == "__WeaponInRightFoot" ||
                             oth.name == "__Horns") { // Столкновение головой об оружку или рога...
                        oncollisionHitFatality(collision);
                        ShowEffect(new Color(255f, 0f, 0f, 255f));
                    }
                    else if (oth.name == "__Nimbus") { // Столкновение головой о нимб
                        
                    }
                }
                else if (gameObject.name == "Body" ||
                         gameObject.name == "LeftShoulder" ||
                         gameObject.name == "RightShoulder" ||
                         gameObject.name == "RightLeg" ||
                         gameObject.name == "LeftLeg") {
                    if (oth.name == "__Head" ||
                        oth.name == "__LeftHand" ||
                        oth.name == "__RightHand" ||
                        oth.name == "__LeftFoot" ||
                        oth.name == "__RightFoot") { // Получаем повреждение
                        oncollisionGettingDamage(collision);
                    }
                    else if(oth.name == "__LeftShoulder" ||
                            oth.name == "__RightShoulder" ||
                            oth.name == "__LeftLeg" || 
                            oth.name == "__RightLeg" || 
                            oth.name == "__Body") { // Не получаем повреждений.
                        Vector3 cont = collision.transform.position;
                        Vector2 move = VectorDirect(cont);
                        rbObj.AddForce(move * accel, ForceMode2D.Impulse);
                        //Block.Play();
                    }
                    else if (oth.name == "__WeaponInLeftHand" ||
                             oth.name == "__WeaponInRightHand" ||
                             oth.name == "__WeaponInLeftFoot" ||
                             oth.name == "__WeaponInRightFoot" ||
                             oth.name == "__Horns") {
                        oncollisionHitFatality(collision);
                        ShowEffect(new Color(255f, 0f, 0f, 255f));
                    }
                    else if (oth.name == "__Nimbus") {

                    }
                }
                else if (gameObject.name == "RightFoot" ||
                         gameObject.name == "LeftFoot" ||
                         gameObject.name == "LeftHand" ||
                         gameObject.name == "RightHand") {
                    if (oth.name == "__LeftHand" ||
                        oth.name == "__RightHand" ||
                        oth.name == "__LeftFoot" ||
                        oth.name == "__RightFoot") { // Блок. Не получаем повреждений.
                        oncollisionBlock(collision);
                    }
                    else if(oth.name == "__Head" ||
                            oth.name == "__Body" ||
                            oth.name == "__LeftShoulder" ||
                            oth.name == "__RightShoulder" ||
                            oth.name == "__LeftLeg" ||
                            oth.name == "__RightLeg") { // Наносим удар. Без повреждений.
                        oncollisionHit(collision);
                    }
                    else if (oth.name == "__WeaponInLeftHand" ||
                             oth.name == "__WeaponInRightHand" ||
                             oth.name == "__WeaponInLeftFoot" ||
                             oth.name == "__WeaponInRightFoot" ||
                             oth.name == "__Horns") {
                        oncollisionHitFatality(collision);
                        ShowEffect(new Color(255f, 0f, 0f, 255f));
                    }
                    else if (oth.name == "__Nimbus") {

                    }
                }
                else if(gameObject.name == "WeaponInLeftHand" ||
                        gameObject.name == "WeaponInRightHand" ||
                        gameObject.name == "WeaponInLeftFoot" ||
                        gameObject.name == "WeaponInRightFoot" ||
                        gameObject.name == "Horns") {
                    if (oth.name == "__Head" || 
                        oth.name == "__Body" || 
                        oth.name == "__LeftShoulder" || 
                        oth.name == "__RightShoulder" || 
                        oth.name == "__LeftLeg" || 
                        oth.name == "__RightLeg" || 
                        oth.name == "__LeftHand" || 
                        oth.name == "__RightHand" || 
                        oth.name == "__LeftLeg" || 
                        oth.name == "__RightLeg") { // Наносим проникающий удар оружием
                        oncollisionHitFatality(collision);
                    }
                    else if(oth.name == "__WeaponInLeftHand" ||
                            oth.name == "__WeaponInRightHand" ||
                            oth.name == "__WeaponInLeftFoot" ||
                            oth.name == "__WeaponInRightFoot" ||
                            oth.name == "__Horns") { // Блок оружка - оружка:
                        oncollisionBlockSword(collision);
                    }
                    else if(oth.name == "__Nimbus") {
                        oncollisionEncrease(collision); // Враг будет восстанавливаться в здоровье.
                    }
                }
            }
        }
        else {

            bool bFlag = false;
            foreach (string iter in __listIgnore) {
                if (iter == oth.name) {
                    bFlag = true;
                    break;
                }
            }

            if (bFlag) {
                Vector3 cont = collision.transform.position;
                Vector2 move = VectorDirect(cont);
                rbObj.AddForce(move * accelOwnLimbs, ForceMode2D.Force);
            }
            else {
                if (gameObject.name == "__Head") {
                    if (oth.name == "Head") { // Столкновение головами.
                        Vector3 cont = collision.transform.position;
                        Vector2 move = VectorDirect(cont);
                        rbObj.AddForce(move * accel, ForceMode2D.Impulse);
                        Hit.Play();
                        ShowEffect(new Color(255f, 0f, 0f, 255f));
                    }
                    else if (oth.name == "LeftHand" ||
                        oth.name == "RightHand" ||
                        oth.name == "LeftFoot" ||
                        oth.name == "RightFoot") { // Столкновение головой с вражескими атакующими частями тела, кроме головы.
                        Vector3 cont = collision.transform.position;
                        Vector2 move = VectorDirect(cont);
                        rbObj.AddForce(move * accel, ForceMode2D.Impulse);
                        Hit.Play();
                        ShowEffect(new Color(255f, 0f, 0f, 255f));
                    }
                    else if (oth.name == "LeftShoulder" ||
                            oth.name == "RightShoulder" ||
                            oth.name == "LeftLeg" ||
                            oth.name == "RightLeg" ||
                            oth.name == "Body") { // Столкновение головой по уязвимым частям тела врага.
                        Vector3 cont = collision.transform.position;
                        Vector2 move = VectorDirect(cont);
                        rbObj.AddForce(move * accel, ForceMode2D.Impulse);
                        Hit.Play();
                        ShowEffect(new Color(255f, 0f, 0f, 255f));
                    }
                    else if (oth.name == "WeaponInLeftHand" ||
                             oth.name == "WeaponInRightHand" ||
                             oth.name == "WeaponInLeftFoot" ||
                             oth.name == "WeaponInRightFoot" ||
                             oth.name == "Horns") { // Столкновение головой об оружку или рога...
                        oncollisionHitFatality(collision);
                        ShowEffect(new Color(255f, 0f, 0f, 255f));
                    }
                    else if (oth.name == "__Nimbus") { // Столкновение головой о нимб

                    }
                }
                else if (gameObject.name == "__Body" ||
                         gameObject.name == "__LeftShoulder" ||
                         gameObject.name == "__RightShoulder" ||
                         gameObject.name == "__RightLeg" ||
                         gameObject.name == "__LeftLeg") {
                    if (oth.name == "Head" ||
                        oth.name == "LeftHand" ||
                        oth.name == "RightHand" ||
                        oth.name == "LeftFoot" ||
                        oth.name == "RightFoot") { // Получаем повреждение
                        oncollisionGettingDamage(collision);
                    }
                    else if (oth.name == "LeftShoulder" ||
                            oth.name == "RightShoulder" ||
                            oth.name == "LeftLeg" ||
                            oth.name == "RightLeg" ||
                            oth.name == "Body") { // Не получаем повреждений.
                        Vector3 cont = collision.transform.position;
                        Vector2 move = VectorDirect(cont);
                        rbObj.AddForce(move * accel, ForceMode2D.Impulse);
                        //Block.Play();
                    }
                    else if (oth.name == "WeaponInLeftHand" ||
                             oth.name == "WeaponInRightHand" ||
                             oth.name == "WeaponInLeftFoot" ||
                             oth.name == "WeaponInRightFoot" ||
                             oth.name == "Horns") {
                        oncollisionHitFatality(collision);
                        ShowEffect(new Color(255f, 0f, 0f, 255f));
                    }
                    else if (oth.name == "Nimbus") {

                    }
                }
                else if (gameObject.name == "__RightFoot" ||
                         gameObject.name == "__LeftFoot" ||
                         gameObject.name == "__LeftHand" ||
                         gameObject.name == "__RightHand") {
                    if (oth.name == "LeftHand" ||
                        oth.name == "RightHand" ||
                        oth.name == "LeftFoot" ||
                        oth.name == "RightFoot") { // Блок. Не получаем повреждений.
                        oncollisionBlock(collision);
                    }
                    else if (oth.name == "Head" ||
                            oth.name == "Body" ||
                            oth.name == "LeftShoulder" ||
                            oth.name == "RightShoulder" ||
                            oth.name == "LeftLeg" ||
                            oth.name == "RightLeg") { // Наносим удар. Без повреждений.
                        oncollisionHit(collision);
                    }
                    else if (oth.name == "WeaponInLeftHand" ||
                             oth.name == "WeaponInRightHand" ||
                             oth.name == "WeaponInLeftFoot" ||
                             oth.name == "WeaponInRightFoot" ||
                             oth.name == "Horns") {
                        oncollisionHitFatality(collision);
                        ShowEffect(new Color(255f, 0f, 0f, 255f));
                    }
                    else if (oth.name == "__Nimbus") {

                    }
                }
                else if (gameObject.name == "__WeaponInLeftHand" ||
                        gameObject.name == "__WeaponInRightHand" ||
                        gameObject.name == "__WeaponInLeftFoot" ||
                        gameObject.name == "__WeaponInRightFoot" ||
                        gameObject.name == "__Horns") {
                    if (oth.name == "Head" ||
                        oth.name == "Body" ||
                        oth.name == "LeftShoulder" ||
                        oth.name == "RightShoulder" ||
                        oth.name == "LeftLeg" ||
                        oth.name == "RightLeg" ||
                        oth.name == "LeftHand" ||
                        oth.name == "RightHand" ||
                        oth.name == "LeftLeg" ||
                        oth.name == "RightLeg") { // Наносим проникающий удар оружием
                        oncollisionHitFatality(collision);
                    }
                    else if (oth.name == "WeaponInLeftHand" ||
                            oth.name == "WeaponInRightHand" ||
                            oth.name == "WeaponInLeftFoot" ||
                            oth.name == "WeaponInRightFoot" ||
                            oth.name == "Horns") { // Блок оружка - оружка:
                        oncollisionBlockSword(collision);
                    }
                    else if (oth.name == "Nimbus") {
                        oncollisionEncrease(collision); // Враг будет восстанавливаться в здоровье.
                    }
                }
            }
        }
    }

    IEnumerator SetTimeout(float sec) {
        yield return new WaitForSeconds(sec);
        spriteRenderer.color = new Color(255f, 255f, 255f, 255f);
    }

    private void ShowEffect(Color color) {
        spriteRenderer.color = color;
        StartCoroutine(SetTimeout(0.1f));
    }

    private void ShowEffectAnother(Color color) {

    }

}
