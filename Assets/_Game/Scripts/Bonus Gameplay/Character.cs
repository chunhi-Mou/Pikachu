using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
	[SerializeField] Animator anim;
	private string animName = GameCONST.Anim_CHAR_IDLE;
	private float delayTime = 0.3f;

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
		StartCoroutine(ActionThenHide(delayTime, target));
	}
	
	/// <summary>
	/// Thực hiện tại vị trí targer: Nhảy -> Đánh -> Nhảy -> Ẩn
	/// </summary>
	private IEnumerator ActionThenHide(float time, Transform target)
	{
		this.transform.position = target.position;
		this.transform.localScale = Vector3.one;

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
