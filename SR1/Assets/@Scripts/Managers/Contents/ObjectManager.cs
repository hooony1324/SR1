using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public Hero Hero { get; private set; }
    public HashSet<Hero> Heroes { get; } = new HashSet<Hero>();
    public HashSet<Monster> Monsters { get; } = new HashSet<Monster>();
    public HashSet<Projectile> Projectiles { get; } = new HashSet<Projectile>();
    public HashSet<Env> Envs { get; } = new HashSet<Env>();
    public HashSet<Npc> Npcs { get; } = new HashSet<Npc>();
    public HashSet<ItemHolder> ItemHolders { get; } = new HashSet<ItemHolder>();

    public HeroCamp HeroCamp { get; set; }

    #region Roots
    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject { name = name };

        return root.transform;
    }
    public Transform HeroRoot { get { return GetRootTransform("@Heroes"); } }
    public Transform MonsterRoot { get { return GetRootTransform("@Monsters"); } }
    public Transform ProjectileRoot { get { return GetRootTransform("@Projectiles"); } }
    public Transform EnvRoot { get { return GetRootTransform("@Envs"); } }
    public Transform EffectRoot { get { return GetRootTransform("@Effects"); } }
    public Transform NpcRoot { get { return GetRootTransform("@Npc"); } }
    public Transform ItemHolderRoot { get { return GetRootTransform("@ItemHolders"); } }

    #endregion

    public ObjectManager()
    {
        Init();
    }

    public void Init()
    {
    }

    public void Clear()
    {
        Monsters.Clear();
    }

    public void LoadMap(string mapName)
    {
        GameObject objMap = Managers.Resource.Instantiate(mapName);
        objMap.transform.position = Vector3.zero;
        objMap.name = "@Map";
    }

    public void ShowDamageFont(Vector2 pos, float damage, Transform parent, EDamageResult result)
    {
        string prefabName = "DamageFont";

        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
        DamageFont damageText = go.GetComponent<DamageFont>();
        damageText.SetInfo(pos, damage, parent, result);
    }

    public T Spawn<T>(object position, int templateID = 0, string prefabName = "") where T : BaseObject
    {
        System.Type type = typeof(T);

        #region Set Position
        Vector3 spawnPos = new Vector3();
        if (position is Vector3)
        {
            spawnPos = (Vector3)position;
        }
        else if (position is Vector3Int)
        {
            spawnPos = Managers.Map.CellGrid.GetCellCenterWorld((Vector3Int)position);
        }
        #endregion

        if (type == typeof(Hero))
        {
            GameObject go = Managers.Resource.Instantiate("HeroPrefab");
            // go.name = Managers.Data.HeroDic[templateID].DescriptionTextID;
            go.transform.position = spawnPos;
            go.transform.parent = HeroRoot;
            Hero hc = go.GetOrAddComponent<Hero>();
            Heroes.Add(hc);
            hc.SetInfo(templateID);
            Hero = hc;
            return hc as T;
        }
        if (type == typeof(Env))
        {
            GameObject go = Managers.Resource.Instantiate(Managers.Data.EnvDic[templateID].PrefabLabel,
                pooling: true);
            go.transform.position = spawnPos;
            go.transform.parent = EnvRoot;
            Env gr = go.GetOrAddComponent<Env>();
            Envs.Add(gr);
            gr.SetInfo(templateID);
            return gr as T;
        }
        if (type == typeof(Npc))
        {
            GameObject go = Managers.Resource.Instantiate(Managers.Data.NpcDic[templateID].PrefabLabel, pooling: true);
            go.transform.position = spawnPos;
            go.transform.parent = NpcRoot;
            Npc npc = go.AddComponent<Npc>();
            Npcs.Add(npc);
            npc.SetInfo(templateID);

            return npc as T;
        }
        if (type == typeof(Monster))
        {
            GameObject go = Managers.Resource.Instantiate("MonsterPrefab", pooling: true);
            go.transform.position = spawnPos;
            go.transform.parent = MonsterRoot;
            Monster mc = go.GetOrAddComponent<Monster>();
            Monsters.Add(mc);
            mc.SetInfo(templateID);
            return mc as T;
        }
        if (type == typeof(ItemHolder))
        {
            GameObject go = Managers.Resource.Instantiate("ItemHolder", ItemHolderRoot, pooling: true);
            go.transform.position = spawnPos;
            ItemHolder itemHolder = go.GetOrAddComponent<ItemHolder>();
            ItemHolders.Add(itemHolder);
            return itemHolder as T;
        }
        if (type == typeof(AoEBase))
        {
            GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
            go.transform.position = spawnPos;
            AoEBase aoe = go.GetOrAddComponent<AoEBase>();
            go.transform.parent = ProjectileRoot;
            return aoe as T;
        }
        if (type == typeof(HeroCamp))
        {
            GameObject go = Managers.Resource.Instantiate("CampPrefab");
            go.transform.position = spawnPos;
            go.transform.parent = ProjectileRoot;
            go.name = "***CampPoint***";
            HeroCamp= go.GetOrAddComponent<HeroCamp>();
            return HeroCamp as T;
        }

        return null;
    }
    
    public void Despawn<T>(T obj) where T : BaseObject
    {
        System.Type type = typeof(T);

        if (type == typeof(Hero))
        {
            Heroes.Remove(obj as Hero);
            Managers.Resource.Destroy(obj.gameObject);
        }
        else if (type == typeof(Monster))
        {
            Monsters.Remove(obj as Monster);
            Managers.Resource.Destroy(obj.gameObject);
        }
        else if (type == typeof(Env))
        {
            Envs.Remove(obj as Env);
            Managers.Resource.Destroy(obj.gameObject);
        }
        else if (type == typeof(Npc))
        {
            Npcs.Remove(obj as Npc);
            Managers.Resource.Destroy(obj.gameObject);
        }
        else if (type == typeof(InteractionObject))
        {
            Managers.Resource.Destroy(obj.gameObject);
        }
        else if (type == typeof(ItemHolder))
        {
            Managers.Resource.Destroy(obj.gameObject);
        }
        Managers.Map.RemoveObject(obj);
    }

    public GameObject SpawnGameObject(Vector3 position, string prefabName)
    {
        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
        go.transform.position = position;
        
        return go;
    }

    public void DespawnGameObject<T>(T obj) where T : BaseObject
    {
        System.Type type = typeof(T);

        if (typeof(EffectBase).IsAssignableFrom(type))
        {
            Managers.Resource.Destroy(obj.gameObject);
        }
        else if (typeof(AoEBase).IsAssignableFrom(type))
        {
            Managers.Resource.Destroy(obj.gameObject);
        }
    }
    
    public GameObject SpawnProjectile(Vector3 position, string prefabName)
    {
        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
        go.transform.position = position;
        go.transform.parent = ProjectileRoot;
        return go;
    }

    public void DespawnProjectile(Projectile projectile)
    {
        Projectiles.Remove(projectile);
        Managers.Resource.Destroy(projectile.gameObject);
    }

    public void PerformMatching()
    {
        //1. 캠프위치에서 SCAN_RANGE거리 내에 있는 오브젝트
        List<Hero> Heroes = Managers.Map.GatherScreenObjects<Hero>().ToList();
        List<Monster> Monsters = Managers.Map
            .GatherScreenObjects<Monster>().ToList();
        List<Env> gatherObjects = Managers.Map.GatherScreenObjects<Env>().ToList();

        // 2. 몬스터 -> 영웅 
        foreach (Monster monster in Monsters)
        {
            if(monster.IsReturning == true)
                continue;
            Hero target = FindClosestTarget(monster, Heroes);
            monster.Target = target;
        }

        //조이스틱 조정 중에서는 몬스터만 타게팅 함
        if (Managers.Game.JoystickState == EJoystickState.Drag)
            return;
                
        // 3-1. 영웅 -> 몬스터 or 자원
        foreach (Hero hero in Heroes)
        {
            Monster target = FindClosestTarget(hero, Monsters);
            if (target.IsValid())
            {
                // hero.creatureAI.SetTarget(target);
                hero.Target = target;
            }
            else
            {
                // 3-2. 몬스터가 없으면 자원캐기
                Env env = FindClosestTarget(hero, gatherObjects);
                if (env != null)
                    hero.Target = env;
                // else// 3-3. 몬스터/자원이 없으면 캠프로 지정
                //     hero.Target = null;
            }
        }
    }

    private T FindClosestTarget<T>(Creature source, List<T> targets) where T : InteractionObject
    {
        float minDistance = float.MaxValue;
        T closestTarget = null;

        foreach (T target in targets)
        {
            float distance = (source.transform.position - target.transform.position).sqrMagnitude;
            
            if (target.IsValid() == false)
            {
                continue;
            }
               
            // float AttackRange = Mathf.Max(source.Skills.CurrentSkill.SkillData.SkillRange, SCAN_RANGE);
            //
            // if (Managers.Object.HeroCamp.CampState == ECampState.CmapMode && source.ObjectType == EObjectType.Monster)
            //     AttackRange = 100f;
            
            // if(distance > AttackRange * AttackRange)
            //     continue;
            
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTarget = target;
            }
        }

        return closestTarget;
    }

    public List<InteractionObject> FindConeRangeTargets(InteractionObject owner,Vector3 dir, float range, int angleRange, bool isAllies = false)
    {
        List<InteractionObject> targets = new List<InteractionObject>();
        List<InteractionObject> ret = new List<InteractionObject>();

        EObjectType targetObjectType = Util.DetermineTargetType(owner.ObjectType, isAllies);
        
        if (targetObjectType == Define.EObjectType.Monster)
        {
            targets = Managers.Map
                .GatherObjects<Monster>(owner.transform.position, range, range)
                .Cast<InteractionObject>()
                .ToList();
        }
        else if(targetObjectType == Define.EObjectType.Hero)
        {
            targets = Managers.Map
                .GatherObjects<Hero>(owner.transform.position, range, range)
                .Cast<InteractionObject>()
                .ToList();
        }
            
        foreach (var target in targets)
        {
            //1. 거리안에 있는지 확인
            var targetPos = target.transform.position;
            float distance = Vector3.Distance(targetPos, owner.transform.position);

            if (angleRange == 360)
            {
                if(distance < range)
                    ret.Add(target);
                continue;
            }

            // 2. 부채꼴 모양인 경우 각도 계산
            float dot = Vector3.Dot((targetPos - owner.transform.position).normalized, dir.normalized);
            dot = Mathf.Clamp(dot, -1f, 1f); // dot 값을 -1과 1 사이로 제한
            float degree = Mathf.Rad2Deg * Mathf.Acos(dot);

            if (degree <= angleRange / 2f && distance < range)
                ret.Add(target);
        }

        return ret;
    }
    
    public List<InteractionObject> FindCircleRangeTargets(Vector3 startPos, float range, EObjectType ownerType, bool isAlly = false)
    {
        List<InteractionObject> targets = new List<InteractionObject>();
        List<InteractionObject> ret = new List<InteractionObject>();

        EObjectType targetObjectType = Util.DetermineTargetType(ownerType, isAlly);
        
        if (targetObjectType == Define.EObjectType.Monster)
        {
            targets = Managers.Map
                .GatherObjects<Monster>(startPos, range, range)
                .Cast<InteractionObject>()
                .ToList();
        }
        else if(targetObjectType == Define.EObjectType.Hero)
        {
            targets = Managers.Map
                .GatherObjects<Hero>(startPos, range, range)
                .Cast<InteractionObject>()
                .ToList();
        }
            
        foreach (var target in targets)
        {
            //1. 거리안에 있는지 확인
            var targetPos = target.transform.position;
            float distSqr = (targetPos - startPos).sqrMagnitude;
            
            if(distSqr < range * range)
                ret.Add(target);
        }

        return ret;
    }
    
    public List<InteractionObject> FindRectRangeTargets(Vector3 startPos, float range, float width, float length, Vector3 dir, EObjectType ownerType, bool isAlly = false)
    {
        List<InteractionObject> targets = new List<InteractionObject>();
        List<InteractionObject> ret = new List<InteractionObject>();

        EObjectType targetObjectType = Util.DetermineTargetType(ownerType, isAlly);
        
        if (targetObjectType == EObjectType.Monster)
        {
            targets = Managers.Map
                .GatherObjects<Monster>(startPos, range, range)
                .Cast<InteractionObject>()
                .ToList();
        }
        else if(targetObjectType == EObjectType.Hero)
        {
            targets = Managers.Map
                .GatherObjects<Hero>(startPos, range, range)
                .Cast<InteractionObject>()
                .ToList();
        }
            
        foreach (var target in targets)
        {
            Vector2 toMonster = target.Position - startPos; 
            float distanceAlongDirection = Vector2.Dot(toMonster, dir); // 방향 벡터에 대한 프로젝션 거리
            Vector2 perpendicularDir = new Vector2(-dir.y, dir.x); // 방향 벡터에 수직인 벡터
            float distancePerpendicular = Mathf.Abs(Vector2.Dot(toMonster, perpendicularDir)); // 수직 거리

            // 몬스터가 직사각형 범위 내에 있는지 판단
            if (distanceAlongDirection > 0 && distanceAlongDirection <= length && distancePerpendicular <= width / 2)
            {
                ret.Add(target);
            }
        }

        return ret;
    }

}