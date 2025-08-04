using UnityEngine;

public static class AnimatorUtils
{
    public static void ChangeAnimUI(string triggerName, Animator anim)
    {
        anim.ResetTrigger(triggerName);
        anim.SetTrigger(triggerName);
    }
}
