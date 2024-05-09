using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class SkillComponent : MonoBehaviour
{
    [SerializeField] private List<SkillBase> _skillList = new List<SkillBase>();

    public List<SkillBase> SkillList
    {
        get { return _skillList; }
    }

    [SerializeField] public List<SkillBase> ReadySkills = new List<SkillBase>();

    public SkillBase _defaultSkill;
    public SkillBase DefaultSkill 
    {
        get
        {
            //평타로 스킬이 나가는경우
            if (ASkill != null && _defaultSkill.SkillData.TempleteId == ASkill.SkillData.TempleteId)
            {
                return ASkill;
            }

            return _defaultSkill;
        }
        private set => _defaultSkill = value;
    }
    public SkillBase EnvSkill { get; private set; } //Gathering
    public SkillBase ASkill { get; private set; }
    public SkillBase BSkill { get; private set; }
    public SkillBase CurrentSkill;
    public Creature _owner;

    public void Awake()
    {
        _owner = GetComponent<Creature>();
    }

    public void AddSkill(int skillId, ESkillSlot skillSlot)
    {
        if (skillId == 0)
            return;

        string className = Managers.Data.SkillDic[skillId].ClassName;
        SkillBase skill = gameObject.AddComponent(Type.GetType(className)) as SkillBase;

        if (!skill)
            return;

        skill.SetInfo(skillId);
        SkillList.Add(skill);

        switch (skillSlot)
        {
            case ESkillSlot.Default:
                DefaultSkill = skill;
                break;
            case ESkillSlot.Env:
                EnvSkill = skill;
                break;
            case ESkillSlot.A:
                ASkill = skill;
                AddReadySkill(skill);
                break;
            case ESkillSlot.B:
                BSkill = skill;
                AddReadySkill(skill);
                break;
        }
    }

    public void UpdateSkill(int skillId, ESkillSlot skillSlot)
    {
        if (skillId == 0)
            return;
        switch (skillSlot)
        {
            case ESkillSlot.Default:
                SkillList.Remove(DefaultSkill);
                ReadySkills.Remove(DefaultSkill);
                break;
            case ESkillSlot.Env:
                SkillList.Remove(EnvSkill);
                ReadySkills.Remove(EnvSkill);
                break;
            case ESkillSlot.A:
                SkillList.Remove(ASkill);
                ReadySkills.Remove(ASkill);
                break;
            case ESkillSlot.B:
                SkillList.Remove(BSkill);
                ReadySkills.Remove(BSkill);
                break;
        }

        string className = Managers.Data.SkillDic[skillId].ClassName;
        SkillBase skill = gameObject.GetComponent(Type.GetType(className)) as SkillBase;
        Destroy(skill);

        AddSkill(skillId, skillSlot);
    }

    private void AddReadySkill(SkillBase skill)
    {
        if (skill.SkillType != ESkillType.PassiveSkill)
        {
            ReadySkills.Add(skill);
        }
    }

    public SkillBase GetReadySkill()
    {
        if (_owner.Target.IsValid())
        {
            if (_owner.Target.ObjectType == EObjectType.Env)
                return EnvSkill;
        }

        if (ReadySkills.Count == 0)
        {
            return DefaultSkill;
        }
        SkillBase skill = ReadySkills.FirstOrDefault();
        if (skill != null)
         return ReadySkills.FirstOrDefault();

        return DefaultSkill;
    }

    public void Clear()
    {
        ReadySkills.Clear();
        if (ASkill != null)
            ASkill.Clear();
        if (BSkill != null)
            BSkill.Clear();
    }
}