
public static class Definitions
{
    // strings
    public const string INPUT_HORIZONTAL = "Horizontal";
    public const string INPUT_VERTICAL = "Vertical";
    public const string UP = "Up";
    public const string DOWN = "Down";
    public const string LEFT = "Left";
    public const string RIGHT = "Right";

    public const string CONFIG = "Config";
    public const string MUSIC = "Music";
    public const string SOUNDS = "Sounds";
    public const string SPRITE = "Sprite";
    public const string SOUND_VOLUME = "FXVolume";
    public const string MUSIC_VOLUME = "MusicVolume";
    
    public const string SAVE_FOLDER_NAME = "/Saves/";
    
    public const string RIGHT_HAND_AIM_PATH = "RightHandAim";
    public const string LEFT_HAND_AIM_PATH = "LeftHandAim";
    public const string SHOOTING_POINT = "ShootingPoint";

    public const string PLAYER = "Player";
    public const string ROOM = "Room";
    public const string ENEMIES = "Enemies";
    public const string ROOM_ENEMIES_PATH = "Enemies";
    public const string ROOM_COVERS_PATH = "Covers";
    public const string ROOM_TRAPS_PATH = "Traps";
    public const string COIN_PICK_UP = "CoinPickUp";
    public const string CORRIDOR = "Corridor";
    public const string CORRIDOR_EDGE = "CorridorEdge";
    public const string BLOCKING_WALL = "BlockingWall";

    public const string ENTRANCE_ROOM = "EntranceRoom";
    public const string FINISH_ROOM = "FinishRoom";
    public const string WALLS = "Walls";
    public const string GROUND = "Ground";
    public const string LIGHTS = "Lights";

    public const string SCRIPTABLE_OBJECTS_PATH = "ScriptableObjects/";
    public const string FIRST_LOCATION_PATH = "FirstLocation/";
    public const string SECOND_LOCATION_PATH = "SecondLocation/";
    public const string FINAL_LOCATION_PATH = "FinalLocation/";
    public const string FIRST_LOCATION = "FirstLocation";
    public const string SECOND_LOCATION = "SecondLocation";
    public const string FINAL_LOCATION = "FinalLocaion";

    public const string LEVEL = "Level";
    public const string FINAL = "Final";
    public const string LEVEL_1 = LEVEL + "1";
    public const string LEVEL_2 = LEVEL + "2";
    public const string LEVEL_3 = LEVEL + "3";
    public const string LEVEL_4 = LEVEL + "4";
    public const string LAST_LEVEL = "Last" + LEVEL;
    public const string FINAL_LEVEL = FINAL + LEVEL;

    public const string RESTART_BUTTON = "RestartButton";
    public const string RESTART_TEXT = RESTART_BUTTON + "/RestartText";

    public const string PISTOL = "Pistol";
    public const string CANON = "Canon";
    public const string LASER_RIFLE = "LaserRifle";
    public const string AUTO_RIFLE = "AutoRifle";
    public const string SPACE_GUY = "SpaceGuy";
    public const string RANGER = "Ranger";
    
    public const string SPACE_GUY_CONFIG = SPACE_GUY + CONFIG;
    public const string RANGER_CONFIG = RANGER + CONFIG;
    public const string WEAPONS_CONFIG = "Weapons" + CONFIG;
    public const string MUSIC_CONFIG = MUSIC + CONFIG;
    public const string SOUNDS_CONFIG = SOUNDS + CONFIG;
    public const string HUD_CONFIG = "HUD" + CONFIG;
    public const string SPAWN_AREA_CONFIG = "SpawnArea" + CONFIG;
    public const string CAMERA_CONFIG = "Camera" + CONFIG;
    public const string ROOMS_CORRIDORS_PREFABS_CONFIG = "RoomsAndCorridorsPrefabs" + CONFIG;

    public const string DUMMY_CONFIG = "Dummy" + CONFIG;
    public const string STATUE_CONFIG = "Statue" + CONFIG;
    public const string SKELETON_CONFIG = "Skeleton" + CONFIG;
    public const string ROGUE_PISTOL_CONFIG = "RoguePistol" + CONFIG;
    public const string ROGUE_LASERRIFLE_CONFIG = "RogueLaserRifle" + CONFIG;
    public const string ROGUE_CANON_CONFIG = "RogueCanon" + CONFIG;
    public const string CACTUS_SMALL_CONFIG = "CactusSmall" + CONFIG;
    public const string CACTUS_BIG_CONFIG = "CactusBig" + CONFIG;
    public const string DRAGON_CONFIG = "Dragon" + CONFIG;

    public const string SPAWNER = "Spawner";
    public const string ROOMS_CONFIG = "Rooms" + CONFIG;
    public const string LEVEL_1_CONFIG = LEVEL_1 + SPAWNER + CONFIG;
    public const string LEVEL_2_CONFIG = LEVEL_2 + SPAWNER + CONFIG;
    public const string LEVEL_3_CONFIG = LEVEL_3 + SPAWNER + CONFIG;
    public const string LAST_LEVEL_CONFIG = LAST_LEVEL + SPAWNER + CONFIG;
    public const string FINAL_LEVEL_CONFIG = FINAL_LEVEL + SPAWNER + CONFIG;

    public const string CANON_CONFIG = CANON + CONFIG;
    public const string PISTOL_CONFIG = PISTOL + CONFIG;
    public const string LASER_RIFLE_CONFIG = LASER_RIFLE + CONFIG;
    public const string AUTO_RIFLE_CONFIG = AUTO_RIFLE + CONFIG;
    public const string HEALTH_UPGRADE_CONFIG = "HealthUpgrade" + CONFIG;
    public const string SPEED_UPGRADE_CONFIG = "SpeedUpgrade" + CONFIG;
    public const string FIRE_RATE_CONFIG = "FireRateUpgrade" + CONFIG;
    public const string NO_TRAP_DAMAGE_CONFIG = "NoTrapDamageUpgrade" + CONFIG;

    // Tags
    public const string TAG_FULL_HEART = "FullHeart";
    public const string TAG_HALF_HEART = "HalfHeart";
    public const string TAG_COIN = "Coin";

    // Numbers
    public const int ONE_HEALTH_POINT = 10;
    public const int HALF_HEALTH_POINT = 5;
    public const float MUZZLE_FLASH_TIME = 0.15f;

    public const int FULL_HEALTH_DROP_PROBABILITY = 18;
    public const int HALF_HEALTH_DROP_PROBABILITY = 30;
    public const int COIN_DROP_PROBABILITY = 25;

    public const float SPRITE_FADE_AMOUNT = 0.1f;
    public const int CLONE_STRING_LENGTH = 7;
    public const int LAST_LEVEL_INDEX = 4;

    public const int SPRITE_LAYER_BELOW_PLAYER = 1;
    public const int SPRITE_LAYER_ABOVE_PLAYER = 3;
    
    public const float SPRITE_FADE_DELAY = 0.03f;
    public const float SPRITE_FADE_TIME = Definitions.SPRITE_FADE_DELAY * (1 / Definitions.SPRITE_FADE_AMOUNT) + 0.1f;
    public const float DELAY_TO_DESTROY_PROJECTILE = 3f;

    public const float WIDTH_FOR_RESTART_FROM_SAVE_TEXT = 690f;

    // Layers
    public const int LAYER_ROOM = 6;
    public const int LAYER_PROJECTILE = 7;
    public const int LAYER_TRAP = 8;
    public const int LAYER_ITEM = 9;
    public const int LAYER_COVER = 10;
    public const int LAYER_ENEMY = 11;

    // enums
    public enum Sides { Up, Right, Down, Left, Limit }
    public enum InteractableItems { Save, Open, Buy, Limit }
    public enum Upgrades { Health, Speed, FireRate, NoTrapDamage, Limit }

    // sizes of the camera
    public enum RoomTypes { Size6, Size5, Size8, LongHorizontal, LongVertical, LastLevel, }
    // public enum EnemyTypes { Dummy, Statue, Skeleton, Rogue, Limit }

    public enum Sounds
    {
        PistolShot,
        CanonShot,
        LaserRifleShot,
        Enter,
        Exit,
        Hit,
        Item,
    }
}
