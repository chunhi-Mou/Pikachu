using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Animator helpAnim;
    [SerializeField] private GameObject helpTxt;
    [SerializeField] private Animator letMeAnim;
    [SerializeField] private GameObject letMeTxt;

    private string animName = GameCONST.Anim_CHAR_IDLE;
    private float delayTime = 0.35f;

    private Animator currUIAnimator;
    private GameObject currUIText;

    private bool isBusy = false;

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
        helpAnim.gameObject.SetActive(false);
        letMeAnim.gameObject.SetActive(false);
        letMeTxt.SetActive(false);
        helpTxt.SetActive(false);

        StartCoroutine(RandomIdleTroll()); // bắt đầu troll ngẫu nhiên
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Test
        {
            HelpDeadlock(this.transform);
        }
    }

    private void HelpDeadlock(Transform target)
    {
        if (isBusy) return;
        isBusy = true;

        Debug.Log("Help Deadlock");

        bool useHelp = Random.value > 0.5f;
        currUIAnimator = useHelp ? helpAnim : letMeAnim;
        currUIText = useHelp ? helpTxt : letMeTxt;

        currUIAnimator.gameObject.SetActive(true);
        ShowPenguinUIOverlay(currUIAnimator);
        currUIText.SetActive(true);

        StartCoroutine(ActionThenHide(delayTime, target));
    }

    private IEnumerator ActionThenHide(float time, Transform target)
    {
        yield return new WaitForSeconds(time);
        transform.position = target.position;
        transform.localScale = Vector3.one;
        
        ChangeAnim(GameCONST.Anim_CHAR_JUMP);
        yield return new WaitForSeconds(time);
        SoundManager.Instance.PlayFx(FxID.BreakObs);
        ChangeAnim(GameCONST.Anim_CHAR_ATTACK);
        HidePenguinUIOverlay(currUIAnimator);
        yield return new WaitForSeconds(time);

        ChangeAnim(GameCONST.Anim_CHAR_JUMP);
        yield return new WaitForSeconds(time);

        currUIText.SetActive(false);
        transform.localScale = Vector3.zero;
        currUIAnimator.gameObject.SetActive(false);

        isBusy = false;
    }

    private IEnumerator RandomIdleTroll()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 12f));

            if (isBusy || !GameManager.IsState(GameState.GamePlay)) continue; // Không troll khi đang thực hiện nhiệm vụ

            bool useHelp = Random.value > 0.5f;
            Animator trollAnim = useHelp ? helpAnim : letMeAnim;

            trollAnim.gameObject.SetActive(true);
            ShowPenguinUIOverlay(trollAnim);

            yield return new WaitForSeconds(1.5f);

            HidePenguinUIOverlay(trollAnim);
            trollAnim.gameObject.SetActive(false);
        }
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

    private void ShowPenguinUIOverlay(Animator animator)
    {
        SoundManager.Instance.PlayFx(FxID.Pop);
        AnimatorUtils.ChangeAnimUI("look", animator);
    }

    private void HidePenguinUIOverlay(Animator animator)
    {
        AnimatorUtils.ChangeAnimUI("stop", animator);
    }
}
