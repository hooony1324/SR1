using System.Collections;
using Data;
using UnityEngine;
using static Define;

namespace Scripts.Components.Skill
{
    public class SpellIndicator : BaseObject
    {
        private Creature _owner;
        private SkillData _skillData;
        private EIndicatorType _indicatorType = EIndicatorType.Cone;
        private SpriteRenderer _coneSprite;
        private SpriteRenderer _rectangleRenderer;
        private Coroutine _coneFillCoroutine;
        private float _duration;
        
        private void Awake()
        {
            _coneSprite = Util.FindChild<SpriteRenderer>(gameObject, "Cone", true);
            _rectangleRenderer = Util.FindChild<SpriteRenderer>(gameObject, "Rectangle", true);
            _coneSprite.sortingOrder = SortingLayers.SPELL_INDICATOR;
            _rectangleRenderer.sortingOrder = SortingLayers.SPELL_INDICATOR;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Cancel();
        }

        public void SetInfo(Creature owner, SkillData skillData, EIndicatorType type)
        {
            _skillData = skillData;
            _indicatorType = type;
            _owner = owner;
            
            _coneSprite.gameObject.SetActive(false);
            _rectangleRenderer.gameObject.SetActive(false);
            _coneSprite.material.SetFloat("_Angle" , 0);
            _coneSprite.material.SetFloat("_Duration" , 0);
        }

        public void ShowCone(Vector3 startPos, Vector3 dir, float angleRange, float duration)
        {
            _coneSprite.gameObject.SetActive(true);
            transform.position = startPos;
            _duration = duration;
            _coneSprite.material.SetFloat("_Angle" , angleRange);
            _coneSprite.transform.localScale = Vector3.one * Util.GetEffectRadius(_skillData.EffectSize);
            transform.eulerAngles = GetLookAtRotation(dir);
            StartCoroutine(SetConeFill());
        }

        public void ShowRectangle(Vector3 startPos, Vector3 dir, float duration, float scale)
        {
            _rectangleRenderer.gameObject.SetActive(true);
            transform.position = startPos;
            _duration = duration;
            
            _rectangleRenderer.transform.localScale = new Vector3(_owner.CurrentCollider.radius * 2f, scale, 0);
            transform.eulerAngles = GetLookAtRotation(dir);
            StartCoroutine(SetRectFill());
        }

        private IEnumerator SetConeFill()
        {
            //AnimImpactDuration 속도에 맞춰서 Fill
            float elapsedTime = 0f;
            float value = 0;
            while (elapsedTime < _duration)
            {
                value = Mathf.Lerp(0f, 1f, elapsedTime / _duration);
                _coneSprite.material.SetFloat("_Duration" , value);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            _coneSprite.gameObject.SetActive(false);
        }
        
        private IEnumerator SetRectFill()
        {
            //AnimImpactDuration 속도에 맞춰서 Fill
            float elapsedTime = 0f;
            float value = 0;
            while (elapsedTime < _duration)
            {
                value = Mathf.Lerp(0f, 1f, elapsedTime / _duration);
                _rectangleRenderer.material.SetFloat("_Cutoff" , value);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            _rectangleRenderer.gameObject.SetActive(false);
        }

        public void Cancel()
        {
            StopAllCoroutines();
            _coneSprite.gameObject.SetActive(false);
            _rectangleRenderer.gameObject.SetActive(false);

        }
    }
}