using UnityEngine;

public class PlayerSprite : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer; //The players sprite renderer
    public SpriteRenderer SpriteRenderer
    {
        get { return _spriteRenderer; }
        private set
        {
            _spriteRenderer = value;
        }
    }

    //The facing direction of the player, default Right
    private CharacterFacing characterFacing = CharacterFacing.Right;
    public CharacterFacing CurrentFacing
    { get { return characterFacing; } }

    private void Start()
    {
        if ((SpriteRenderer = GetComponent<SpriteRenderer>()) != true)
        { Utils.PrintMissingComponentMsg("SpriteRenderer component", this); }
    }

    /// <summary>
    /// Flip the player game object according to the CharacterFacing passed.
    /// If the passed CharacterFacing is the same as the current facing direction, do nothing
    /// </summary>
    /// <param name="faceTo">The direction to face to</param>
    public void FlipCharacter(CharacterFacing faceTo)
    {
        if (characterFacing != faceTo)
        {
            Vector3 tempFacing = transform.localScale;
            tempFacing.x *= -1;
            transform.localScale = tempFacing;

            characterFacing = faceTo;
        }
    }
}
