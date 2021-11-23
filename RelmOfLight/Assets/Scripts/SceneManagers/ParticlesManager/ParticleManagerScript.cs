using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ParticleManagerScript : MonoBehaviour
{
    public static ParticleManagerScript Instance;
    public ScriptableObjectContainingAllParticles data = null;
    public ScriptableObjectParticle[] ListOfParticles => data.ListOfParticles;
    public List<AttackParticleInfoClass> AttackParticlesFired = new List<AttackParticleInfoClass>();
    public List<AttackParticleInfoClass> BaseAttackParticles = new List<AttackParticleInfoClass>();
    public List<FiredParticle> ParticlesFired = new List<FiredParticle>();
   // public List<AddressableParticleClass> BaseParticles = new List<AddressableParticleClass>();
    public Transform Container;

    AttackParticleInfoClass temp_FAP;
    GameObject temp_Go;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BattleManagerScript.Instance.CurrentBattleSpeedChangedEvent += Instance_CurrentBattleSpeedChangedEvent;
    }

    private void Instance_CurrentBattleSpeedChangedEvent(float currentBattleSpeed)
    {
        foreach (AttackParticleInfoClass item in AttackParticlesFired.Where(r => r.PS.activeInHierarchy).ToList())
        {
            ChangePsSpeed(item.PS, currentBattleSpeed);
        }

        foreach (FiredParticle item in ParticlesFired.Where(r => r.PS.activeInHierarchy).ToList())
        {
            ChangePsSpeed(item.PS, currentBattleSpeed);
        }
    }
    public void AddBaseAttackParticles(AttackParticleInfoClass psToAdd)
    {
        StartCoroutine(AddBaseAttackParticles_Co(psToAdd));
    }

    public IEnumerator AddBaseAttackParticles_Co(AttackParticleInfoClass psToAdd)
    {
        temp_FAP = BaseAttackParticles.GridFight_Where_FirstOrDefault(r => r.PS == psToAdd.PS && r.ParticleType == psToAdd.ParticleType && r.CharacterId == psToAdd.CharacterId && r.AttackInput == psToAdd.AttackInput);
        if(temp_FAP == null)
        {
            AsyncOperationHandle<GameObject> psAddrassable;

            if(psToAdd.Address == "Chapter02_TheBurg_Character_Valley_Debuff_Bullet_Left_Switch")
            {

            }
            if (psToAdd.Address == "Chapter02_TheBurg_Character_Desert_Debuff_Left_Loud")
            {

            }
            if (psToAdd.Address == "Chapter02_TheBurg_Character_Valley_Debuff_Hit_Left_Switch")
            {

            }



            psAddrassable = Addressables.LoadAssetAsync<GameObject>(psToAdd.Address);

            while (!psAddrassable.IsDone)
            {
                yield return null;
            }
            psToAdd.PS = psAddrassable.Result;
            BaseAttackParticles.Add(psToAdd);
        }
    }

    public GameObject FireAttackParticlesInPosition(GameObject ps, CharacterNameType characterId, AttackParticlePhaseTypes particleType, Vector3 pos, FacingType facing, AttackParticlesInputType attackInput, HitParticlesType hpt, float hitResizeMultiplier)
    {
        temp_FAP = AttackParticlesFired.GridFight_Where_FirstOrDefault(r => r.ParticleType == particleType && r.CharacterId == characterId &&
        !r.PS.gameObject.activeInHierarchy && r.AttackInput == attackInput);
        if (temp_FAP != null && (ps == null || temp_FAP.PS.name.Contains(ps.name)))
        {
            temp_FAP.PS.transform.position = pos;
            switch (hpt)
            {
                case HitParticlesType.Normal:
                    temp_FAP.PS.transform.localScale = Vector3.one;
                    break;
                case HitParticlesType.Resized:
                    temp_FAP.PS.transform.localScale = Vector3.one * hitResizeMultiplier;
                    break;
            }
            temp_FAP.PS.SetActive(true);
            ChangePsSpeed(temp_FAP.PS, BattleManagerScript.Instance.BattleSpeed);
            return temp_FAP.PS;
        }
        else
        {
            GameObject res = Instantiate(ps, pos, Quaternion.identity, Container);
            if (particleType == AttackParticlePhaseTypes.Hit && attackInput.ToString().Contains(AttackInputType.Weak.ToString()))
            {
                switch (hpt)
                {
                    case HitParticlesType.Normal:
                        break;
                    case HitParticlesType.Resized:
                        res.transform.localScale *= hitResizeMultiplier;
                        break;
                }
            }
            res.SetActive(true);
            AttackParticlesFired.Add(new AttackParticleInfoClass("", res, characterId, particleType, attackInput));
            ChangePsSpeed(res, BattleManagerScript.Instance.BattleSpeed);
            return res;
        }
    }

    

    public GameObject FireAttackParticlesInPosition(string address, CharacterNameType characterId, AttackParticlePhaseTypes particleType, Vector3 pos, FacingType facing, AttackParticlesInputType attackInput, HitParticlesType hpt, float hitResizeMultiplier)
    {
        temp_FAP = AttackParticlesFired.GridFight_Where_FirstOrDefault(r => r.ParticleType == particleType && r.CharacterId == characterId &&
        !r.PS.gameObject.activeInHierarchy && r.AttackInput == attackInput);
        if (temp_FAP != null)
        {
            temp_FAP.PS.transform.position = pos;
            switch (hpt)
            {
                case HitParticlesType.Normal:
                    temp_FAP.PS.transform.localScale = Vector3.one;
                    break;
                case HitParticlesType.Resized:
                    temp_FAP.PS.transform.localScale = Vector3.one * hitResizeMultiplier;
                    break;
            }
            temp_FAP.PS.transform.localScale = facing == FacingType.Left ? Vector3.one : new Vector3(-1, 1, 1);
            temp_FAP.PS.SetActive(true);
            ChangePsSpeed(temp_FAP.PS, BattleManagerScript.Instance.BattleSpeed);
            return temp_FAP.PS;
        }
        else
        {
            GameObject res =   Instantiate(BaseAttackParticles.GridFight_Where_FirstOrDefault(r => r.Address == address && r.ParticleType == particleType &&
            r.CharacterId == characterId && r.AttackInput == attackInput).PS, pos, Quaternion.identity, Container);
            if (particleType == AttackParticlePhaseTypes.Hit && attackInput.ToString().Contains(AttackInputType.Weak.ToString()))
            {
                switch (hpt)
                {
                    case HitParticlesType.Normal:
                        break;
                    case HitParticlesType.Resized:
                        res.transform.localScale *= hitResizeMultiplier;
                        break;
                }
            }
            res.transform.localScale = facing == FacingType.Left ? Vector3.one : new Vector3(-1, 1, 1);
            res.SetActive(true);
            AttackParticlesFired.Add(new AttackParticleInfoClass(address, res, characterId, particleType, attackInput));
            ChangePsSpeed(res, BattleManagerScript.Instance.BattleSpeed);
            return res;
        }
    }
  
    public GameObject FireAttackParticlesInTransform(GameObject ps, CharacterNameType characterId, AttackParticlePhaseTypes particleType, Transform parent, FacingType facing, AttackParticlesInputType attackInput, float timer = 0f)
    {

        temp_FAP = AttackParticlesFired.GridFight_Where_FirstOrDefault(r => r.ParticleType == particleType && r.CharacterId == characterId
        && !r.PS.gameObject.activeInHierarchy && r.AttackInput == attackInput);
        if (temp_FAP != null)
        {
            temp_FAP.PS.transform.parent = parent;
            temp_FAP.PS.transform.localPosition = Vector3.zero;
            temp_FAP.PS.transform.localRotation = Quaternion.identity;
            if(timer != 0f)
            {
                temp_FAP.PS.GetComponent<ParticleHelperScript>().UpdatePSTime(timer);
            }
            temp_FAP.PS.SetActive(true);
            temp_FAP.PS.SetActive(false);
            temp_FAP.PS.SetActive(true);
            ChangePsSpeed(temp_FAP.PS, BattleManagerScript.Instance.BattleSpeed);
            return temp_FAP.PS;
        }
        else
        {
            GameObject res = Instantiate(ps, parent.position, parent.rotation, parent);
            res.transform.localPosition = Vector3.zero;
            res.transform.localRotation = Quaternion.identity;
            AttackParticlesFired.Add(new AttackParticleInfoClass("", res, characterId, particleType, attackInput));
            if (timer != 0f)
            {
                res.GetComponent<ParticleHelperScript>().UpdatePSTime(timer);
            }
            res.SetActive(true);
            res.SetActive(false);
            res.SetActive(true);
            ChangePsSpeed(res, BattleManagerScript.Instance.BattleSpeed);
            return res;
        }
    }

    public GameObject FireAttackParticlesInTransform(string address, CharacterNameType characterId, AttackParticlePhaseTypes particleType, Transform parent, FacingType facing, AttackParticlesInputType attackInput, float timer = 0f)
    {

        temp_FAP = AttackParticlesFired.GridFight_Where_FirstOrDefault(r => r.ParticleType == particleType && r.CharacterId == characterId
        && !r.PS.gameObject.activeInHierarchy && r.AttackInput == attackInput);
        if (temp_FAP != null)
        {
            temp_FAP.PS.transform.parent = parent;
            temp_FAP.PS.transform.localPosition = Vector3.zero;
            temp_FAP.PS.transform.localRotation = Quaternion.identity;
            temp_FAP.PS.transform.localScale = facing == FacingType.Left ? Vector3.one : new Vector3(-1, 1, 1);

            OrientRotation or = temp_FAP.PS.GetComponentInChildren<OrientRotation>();
            if(or == null)
            {
                Debug.LogError("ACE you forgot the OrientRotation in this particle  " + address);
            }
            else
            {
                or.Adjustment = facing == FacingType.Left ? 180 : 0;
            }

            if (timer != 0f)
            {
                temp_FAP.PS.GetComponent<ParticleHelperScript>().UpdatePSTime(timer);
            }
            temp_FAP.PS.SetActive(true);
            temp_FAP.PS.SetActive(false);
            temp_FAP.PS.SetActive(true);
            ChangePsSpeed(temp_FAP.PS, BattleManagerScript.Instance.BattleSpeed);
            return temp_FAP.PS;
        }
        else
        {
            GameObject res = Instantiate(BaseAttackParticles.GridFight_Where_FirstOrDefault(r => r.Address == address && r.ParticleType == particleType &&
            r.CharacterId == characterId && r.AttackInput == attackInput).PS, parent.position, parent.rotation, parent);
            res.transform.localPosition = Vector3.zero;
            res.transform.localRotation = Quaternion.identity;
            res.transform.localScale = facing == FacingType.Left ? Vector3.one : new Vector3(-1, 1, 1);

            OrientRotation or = res.GetComponentInChildren<OrientRotation>();
            if (or == null)
            {
                Debug.LogError("ACE you forgot the OrientRotation in this particle  " + address);
            }
            else
            {
                or.Adjustment = facing == FacingType.Left ? 180 : 0;
            }

            AttackParticlesFired.Add(new AttackParticleInfoClass("", res, characterId, particleType, attackInput));
            if (timer != 0f)
            {
                res.GetComponent<ParticleHelperScript>().UpdatePSTime(timer);
            }
            res.SetActive(true);
            res.SetActive(false);
            res.SetActive(true);
            ChangePsSpeed(res, BattleManagerScript.Instance.BattleSpeed);
            return res;
        }
    }

    public void ChangePsSpeed(GameObject psG, float speed)
    {
        if (speed == 1)
        {
            if (psG.GetComponent<ParticleHelperScript>() == null)
            {
                Debug.LogError(psG.name + "   is missing particles helper script");
            }
            psG.GetComponent<ParticleHelperScript>().SetSimulationSpeedToBase();
        }
        else
        {
            psG.GetComponent<ParticleHelperScript>().SetSimulationSpeed(speed);
        }
    }

    public GameObject GetParticle(ParticlesType particle)
    {
        if(particle != ParticlesType.None)
        {
            //Debug.Log(particle);
            FiredParticle ps = ParticlesFired.Where(r => r.Particle == particle && !r.PS.activeInHierarchy).FirstOrDefault();
            if (ps == null)
            {
                ScriptableObjectParticle thaParticle = ListOfParticles.Where(r => r.PSType == particle).FirstOrDefault();
                if(thaParticle == null)
                {
                    Debug.LogError("Attempted to fire particle but it doesn't exist in the BATTLE MANAGER > PARTICLE MANAGER > Data Asset, Aborting... Particle Type: " + particle.ToString().ToUpper());
                }
                else
                {
                    ps = new FiredParticle(Instantiate(thaParticle.PS), particle);
                    ParticlesFired.Add(ps);
                }
            }
            ChangePsSpeed(ps.PS, BattleManagerScript.Instance.BattleSpeed);
            return ps.PS;
        }
        return null;
    }


   /* public IEnumerator GetParticle(AddressableParticleClass particle)
    {
        if (particle.Particle != ParticlesType.None)
        {
            FiredParticle ps = ParticlesFired.Where(r => r.Particle == particle.Particle && !r.PS.activeInHierarchy).FirstOrDefault();
            if (ps == null)
            {
                AddressableParticleClass res = BaseParticles.GridFight_Where_FirstOrDefault(r => r.Address == particle.Address && r.Particle == particle.Particle);

                if (res == null)
                {

                    AsyncOperationHandle<GameObject> addressableps;

                    addressableps = Addressables.LoadAssetAsync<GameObject>(particle.Address);

                    while (!addressableps.IsDone)
                    {
                        yield return null;
                    }

                    if (addressableps.Result != null)
                    {
                        particle.PS = addressableps.Result;
                    }

                }
                else
                {
                    particle = res;
                }


                ps = new FiredParticle(Instantiate(particle.PS), particle.Particle);
                ParticlesFired.Add(ps);
            }
           
            ChangePsSpeed(ps.PS, BattleManagerScript.Instance.BattleSpeed);
        }
    }*/

    public GameObject FireParticlesInPosition(ParticlesType particle, Vector3 pos)
    {
        temp_Go = GetParticle(particle);
        temp_Go.transform.position = pos;
        temp_Go.SetActive(true);
        return temp_Go;
    }

    public GameObject FireParticlesInTransform(ParticlesType particle, Transform parent, bool copyFacing = false, FacingType facing = FacingType.Left, bool attachToParent = true)
    {
        temp_Go = GetParticle(particle);
        if (attachToParent)
        {
            temp_Go.transform.SetParent(parent);
            temp_Go.transform.localPosition = Vector3.zero;
        }
        else
            temp_Go.transform.position = parent.position;

        if (copyFacing)
            temp_Go.transform.localScale = facing == FacingType.Right ? Vector3.one : new Vector3(-1f, 1f, 1f);

        temp_Go.SetActive(true);
        return temp_Go;
    }

    public GameObject FireParticlesInPosition(GameObject particle, Vector3 pos)
    {
        temp_Go = Instantiate(particle);
        ParticlesFired.Add(new FiredParticle(temp_Go, ParticlesType.None));
        temp_Go.transform.position = pos;
        return temp_Go;
    }


    public GameObject FireParticlesInTransform(GameObject particle, Transform parent)
    {
        temp_Go = Instantiate(particle);
        ParticlesFired.Add(new FiredParticle(temp_Go, ParticlesType.None));
        temp_Go.transform.SetParent(parent);
        temp_Go.transform.localPosition = Vector3.zero;
        return temp_Go;
    }

    public GameObject GetParticlePrefabByName(ParticlesType particle)
    {
        ScriptableObjectParticle ps = ListOfParticles.Where(r => r.PSType == particle).FirstOrDefault();
        return ps != null ? ps.PS : null;
    }

  

    public void SetParticlesLayer(GameObject ps, int sortingLayer)
    {
        foreach (ParticleSystemRenderer item in ps.GetComponentsInChildren<ParticleSystemRenderer>())
        {
            item.sortingOrder = sortingLayer;
        }
    }

    public GameObject SetEmissivePsInTransform(ParticlesType ps, FacingType facing, Transform t, MeshRenderer meshR, ref GameObject go, bool overrideGo = true)
    {
        #if TRAIL
        ps = ParticlesType.VOID;
        #endif

        //Debug.LogError("particles");
        if(overrideGo) go = FireParticlesInTransform(ps, t);
        go.transform.eulerAngles = new Vector3(0, facing == FacingType.Left ? 0 : 180, 0);
        go.transform.localPosition = Vector3.zero;
        #if !TRAIL
        go.GetComponent<ParticleHelperScript>().SetMeshInMeshEmitters(meshR, meshR.material.mainTexture as Texture2D);
        #endif
        go.SetActive(true);
        go.SetActive(false);
        go.SetActive(true);
        go.GetComponent<ParticleSystem>().Play();
        return go;
    }

    public GameObject SetEmissivePsInPosition(ParticlesType ps, FacingType facing, Vector3 pos, MeshRenderer meshR, ref GameObject go, bool overrideGo = true)
    {
        //Debug.LogError("particles");
        if (overrideGo) go = FireParticlesInPosition(ps, pos);
        go.transform.eulerAngles = new Vector3(0, facing == FacingType.Left ? 0 : 180, 0);
        go.GetComponent<ParticleHelperScript>().SetMeshInMeshEmitters(meshR, meshR.material.mainTexture as Texture2D);
        go.SetActive(true);
        go.SetActive(false);
        go.SetActive(true);
        go.GetComponent<ParticleSystem>().Play();
        return go;
    }

}

[System.Serializable]
public class FiredParticle
{
    public GameObject PS;
    public ParticlesType Particle;

    public FiredParticle()
    {
    }

    public FiredParticle(GameObject ps, ParticlesType particle)
    {
        PS = ps;
        Particle = particle;
    }

}


[System.Serializable]
public class AttackParticleInfoClass : IDisposable
{
    public string Address;
    public GameObject PS;
    public CharacterNameType CharacterId;
    public AttackParticlePhaseTypes ParticleType;
    public AttackParticlesInputType AttackInput;
    public AttackParticleInfoClass()
    {

    }

    public AttackParticleInfoClass(string address, GameObject ps, CharacterNameType characterId, AttackParticlePhaseTypes particleType, AttackParticlesInputType attackInput)
    {
        Address = address;
        PS = ps;
        CharacterId = characterId;
        ParticleType = particleType;
        AttackInput = attackInput;
    }

    public void Dispose()
    {
    }
}

[System.Serializable]
public class AddressableParticleClass
{
    public ParticlesType Particle;
    public string Address;
    public GameObject PS;

    public AddressableParticleClass()
    {

    }

    public AddressableParticleClass(ParticlesType particle, GameObject ps, string address)
    {
        Particle = particle;
        PS = ps;
        Address = address;
    }
}
