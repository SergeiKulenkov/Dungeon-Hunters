using UnityEngine;

public class PlayerSO : ScriptableObject
{
    public RuntimeAnimatorController Animator;
    public int MaxHealth;
    
    public float Speed;
    public float Acceleration;
    public float Decceleration;

    [Header("Invulnerability")]
    public float IFramesTimer;
    public int NumberOfFlashes;
}
