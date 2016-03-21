using UnityEngine;
using System.Collections;

public class ParticleEffect : StateMachineBehaviour
{
	[SerializeField]
	protected GameObject effectPrefab;
	[SerializeField]
	protected float minVelocity;

	private Transform effectTransform;
	private ParticleSystem effectInstance;

	public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (effectInstance == null) {
			effectTransform = GameObject.Instantiate (effectPrefab).transform;
			effectInstance = effectTransform.gameObject.GetComponentInChildren<ParticleSystem> ();
		}

		if (animator.GetFloat ("vVelocity") <= minVelocity) {
			effectTransform.position = animator.transform.position;
			effectInstance.Play (true);
		}
	}
}
