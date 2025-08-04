using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
	[SerializeField] Animator anim;
	private string animName;

	private void OnEnable()
	{
		GameEvents.OnFoundDeadlockObs += HelpDeadlock;
	}

	private void OnDisable()
	{
		GameEvents.OnFoundDeadlockObs -= HelpDeadlock;
	}

	private void Start()
	{
		ChangeAnim(GameCONST.Anim_CHAR_IDLE);
	}

	private void HelpDeadlock(Transform target)
	{
		this.transform.localScale = Vector3.one;
		StartCoroutine(ActionThenHide(0.3f, target));
	}

	private IEnumerator ActionThenHide(float time, Transform target)
	{
		this.transform.position = target.position;
		ChangeAnim(GameCONST.Anim_CHAR_JUMP);
		yield return new WaitForSeconds(time);
		ChangeAnim(GameCONST.Anim_CHAR_ATTACK);
		yield return new WaitForSeconds(time);
		ChangeAnim(GameCONST.Anim_CHAR_JUMP);
		yield return new WaitForSeconds(time);
		this.transform.localScale = Vector3.zero;
	}
	private void ChangeAnim(string animName)
	{
		if (this.animName != animName)
		{
			anim.ResetTrigger(this.animName);
			this.animName = animName;
			anim.SetTrigger(this.animName);
		}
	}
}
