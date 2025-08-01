using UnityEngine;

public static class AnimatorUtils
{
    public static void ChangeAnim(string newAnimName, Animator anim, ref string currentAnimName)
    {
        if (currentAnimName != newAnimName)
        {
            if (!string.IsNullOrEmpty(currentAnimName))
            {
                anim.ResetTrigger(currentAnimName);
            }

            currentAnimName = newAnimName;
            anim.SetTrigger(currentAnimName);
        }
    }

}
