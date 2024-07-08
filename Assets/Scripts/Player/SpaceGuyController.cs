using UnityEngine;

public class SpaceGuyController : Player
{
    protected override void Start()
    {
        PlayerSO config = ConfigManager.GetPlayerConfig(Definitions.SPACE_GUY);
        invulnerabilityController.SetNumberOfFlashes(config.NumberOfFlashes);
        invulnerabilityController.SetFramesTimer(config.IFramesTimer);
        MaxHealth = (GameState.MaxHealthUpgraded) ? GameState.PlayerSettings.MaxHealth : config.MaxHealth;

        transform.GetComponent<MovementController>().InitializeMovement(config);
        transform.GetComponent<CharacterAnimationController>().InitializeAnimator(config.Animator);
        
        base.Start();
    }
}
