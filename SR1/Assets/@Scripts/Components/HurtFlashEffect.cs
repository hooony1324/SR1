using System.Collections;
using UnityEngine;

public class HurtFlashEffect : MonoBehaviour 
{
	private int _flashCount = 1;
	private Color _flashColor = Color.white;
	private float _interval = 0.1f;
	private string _fillPhaseProperty = "_FillPhase";
	private string _fillColorProperty = "_FillColor";

	[SerializeField]
	private ParticleSystem _particle;
	
	MaterialPropertyBlock _mpb;
	MeshRenderer _meshRenderer;

	public void Init()
	{
		if (_mpb == null) 
			_mpb = new MaterialPropertyBlock();
		if (_meshRenderer == null)
			_meshRenderer = gameObject.GetComponent<MeshRenderer>();
		if (_meshRenderer == null)
			_meshRenderer = Util.FindChild<MeshRenderer>(gameObject);

		
		//_particle = Util.FindChild<ParticleSystem>(gameObject, recursive: true);			//Hero의 경우 hitEffect가 없고 걸을때 먼지 이펙트가 있는데 이 먼지를 hitEffect로 인식하는 문제가 있음. 하여 SerializeField로 particle 처리하여 해결
		if (_particle)
			_particle.transform.localPosition = _meshRenderer.localBounds.center;
		
		int fillPhase = Shader.PropertyToID(_fillPhaseProperty);
		_mpb.SetFloat(fillPhase, 0f);
		_meshRenderer.SetPropertyBlock(_mpb);
	}

	public void Flash () 
	{
		if(_particle)
			_particle.Play();
		_meshRenderer.GetPropertyBlock(_mpb);
		StartCoroutine(FlashRoutine());
	}

	IEnumerator FlashRoutine ()
	{
		int fillPhase = Shader.PropertyToID(_fillPhaseProperty);
		int fillColor = Shader.PropertyToID(_fillColorProperty);

		WaitForSeconds wait = new WaitForSeconds(_interval);

        for (int i = 0; i < _flashCount; i++) 
		{
			_mpb.SetColor(fillColor, _flashColor);
			_mpb.SetFloat(fillPhase, 0.6f);
			_meshRenderer.SetPropertyBlock(_mpb);
			yield return wait;

			_mpb.SetFloat(fillPhase, 0f);
			_meshRenderer.SetPropertyBlock(_mpb);
			yield return wait;
		}

		yield return null;
	}

}
