using UnityEngine;

[DefaultExecutionOrder(380)]
public class PlayerAnimations : MonoBehaviour
{
    Animator _playerAnimator; //The animator attached to the player
    public Animator PlayerAnimator
    {
        get { return _playerAnimator; }
        private set { _playerAnimator = value; }
    }

    private void Awake()
    {
        PlayerAnimator = GetComponent<Animator>();
    }

    /// <summary>
    /// Call to forcefully play the TutorialEntrance animation on the player animator
    /// </summary>
    public void PlayEntranceAnimation()
    {
        PlayerAnimator.Play("TutorialEntrance", 0);
    }

    /// <summary>
    /// Call to set the isActive transition boolean to the given state
    /// </summary>
    public void SetActiveBool(bool state)
    {
        PlayerAnimator.SetBool("isActive", state);
    }

    /// <summary>
    /// Call to set the isWalking transition boolean to the given state
    /// </summary>
    public void SetWalkingAnimation(bool state)
    {
        PlayerAnimator.SetBool("isWalking", state);
    }

    /// <summary>
    /// Call to set the isJumping transition boolean to the given state
    /// </summary>
    public void SetJumpingAnimation(bool state)
    {
        PlayerAnimator.SetBool("isJumping", state);
    }

    /// <summary>
    /// Call to forcefully play the BasicAttack animation on the player animator
    /// </summary>
    public void PlayAttackAnimation()
    {
        PlayerAnimator.Play("BasicAttack");
    }

    /// <summary>
    /// Call to forcefully play the DeathAnimation animation on the player animator
    /// </summary>
    public void PlayDeathAnimation()
    {
        PlayerAnimator.Play("DeathAnimation");
    }
}

