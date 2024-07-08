using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private CameraSO config;
    private Transform playerTarget;
    private Vector3 target;
    private Vector2 smoothedPosition;
    private Camera cameraObject;

    private float positionZ;
    private float speed;
    private bool stopMoving;
    private FollowingTypes followingType;

    private bool isClamping;
    private bool isEnteringLongRoom;
    private float clampedPosition;
    private float clampMin;
    private float clampMax;

    private enum FollowingTypes { Following, FollowingX, FollowingY, }

    ///////////////////////////////////////////
    // Methods

    private void Awake()
    {
        cameraObject = transform.GetComponent<Camera>();
        positionZ = transform.position.z;
        stopMoving = true;
    }

    private void Start()
    {
        playerTarget = GameObject.FindObjectOfType<Player>().transform;

        speed = config.DefaultSpeed;

        RoomController.OnRoomEntered += OnRoomEntered;
        RoomController.OnRoomExited += OnRoomExited;
        RoomWithEnemiesController.OnLastRoomEntered += OnLastRoomEntered;
        Player.OnPlayerDied += () => playerTarget = null;
    }

    private void OnDestroy()
    {
        Player.OnPlayerDied -= () => playerTarget = null;
        RoomController.OnRoomEntered -= OnRoomEntered;
        RoomController.OnRoomExited -= OnRoomExited;
        RoomWithEnemiesController.OnLastRoomEntered -= OnLastRoomEntered;
    }

    private void LateUpdate()
    {
        if (!stopMoving && (playerTarget != null))
        {
            switch (followingType)
            {
                case FollowingTypes.Following: target = playerTarget.position;
                    break;
                case FollowingTypes.FollowingX: target.x = playerTarget.position.x;
                    break;
                case FollowingTypes.FollowingY: target.y = playerTarget.position.y;
                    break;
            }
        }

        if (!isEnteringLongRoom)
        {
            if (transform.position != target)
            {
                smoothedPosition = Vector2.Lerp(transform.position, target, speed * Time.fixedDeltaTime);
                if (!isClamping) transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, positionZ);
                else
                {
                    clampedPosition = 0;
                    switch (followingType)
                    {
                        case FollowingTypes.FollowingX:
                            clampedPosition = Mathf.Clamp(smoothedPosition.x, clampMin, clampMax);
                            transform.position = new Vector3(clampedPosition, transform.position.y, positionZ);
                            break;
                        case FollowingTypes.FollowingY:
                            clampedPosition = Mathf.Clamp(smoothedPosition.y, clampMin, clampMax);
                            transform.position = new Vector3(transform.position.x, clampedPosition, positionZ);
                            break;
                    }
                }
            }
        }
    }

    private void StopMoving(Vector2 targetPosition)
    {
        stopMoving = true;
        target = targetPosition;
        speed = config.DefaultSpeed;
    }

    private float GetCameraSize(Definitions.RoomTypes roomType)
    {
        float size = 0;
        switch (roomType)
        {
            case Definitions.RoomTypes.LongHorizontal:
            case Definitions.RoomTypes.LongVertical:
            case Definitions.RoomTypes.Size6: size = config.CameraSize6;
                break;
            case Definitions.RoomTypes.Size5: size = config.CameraSize5;
                break;
            case Definitions.RoomTypes.Size8: size = config.CameraSize8;
                break;
            case Definitions.RoomTypes.LastLevel: size = config.CameraSizeLastLevel;
                break;
        }

        return size;
    }

    private void OnRoomEntered(Vector2 roomPosition, Definitions.RoomTypes roomType)
    {
        float newSize = GetCameraSize(roomType);
        if (cameraObject.orthographicSize != newSize) StartCoroutine(ZoomCamera(newSize));

        if (roomType == Definitions.RoomTypes.LongHorizontal)
        {
            StartCoroutine(MoveToSpecificPositionInLongRoom(roomPosition, newSize, FollowingTypes.FollowingX));
            followingType = FollowingTypes.FollowingX;
            target.y = roomPosition.y;
        }
        else if (roomType == Definitions.RoomTypes.LongVertical)
        {
            StartCoroutine(MoveToSpecificPositionInLongRoom(roomPosition, newSize, FollowingTypes.FollowingY));
            followingType = FollowingTypes.FollowingY;
            target.x = roomPosition.x;
        }
        else StopMoving(roomPosition);
    }

    private void OnRoomExited()
    {
        isClamping = false;
        float positionX = Mathf.Abs(transform.position.x - playerTarget.position.x);
        float positionY = Mathf.Abs(transform.position.y - playerTarget.position.y);

        if (((positionX <= 1) && (positionX >= 0)) ||
            ((followingType == FollowingTypes.FollowingX) &&
            Mathf.Abs(transform.position.y - playerTarget.position.y) > 2))
        {
            followingType = FollowingTypes.FollowingY;
            target.x = transform.position.x;
        }
        else if (((positionY <= 2) && (positionY >= 0)) ||
                ((followingType == FollowingTypes.FollowingY) &&
                Mathf.Abs(transform.position.x - playerTarget.position.x) > 1))
        {
            followingType = FollowingTypes.FollowingX;
            target.y = transform.position.y;
        }

        stopMoving = false;
        speed = config.PlayerFollowingSpeed;
    }

    private void OnLastRoomEntered(Vector2 roomPosition)
    {
        StopMoving(roomPosition);
        StartCoroutine(ZoomCamera(config.CameraSizeLastLevel));
    }

    private IEnumerator ZoomCamera(float targetSize)
    {
        float currentTime = 0;
        float currentSize = cameraObject.orthographicSize;

        while (currentTime < config.ZoomTime)
        {
            currentTime += Time.unscaledDeltaTime;
            cameraObject.orthographicSize = Mathf.Lerp(currentSize, targetSize, currentTime / config.ZoomTime);
            yield return null;
        }
    }

    private IEnumerator MoveToSpecificPositionInLongRoom(Vector2 position, float cameraSize, FollowingTypes newFollowingType)
    {
        isClamping = true;
        isEnteringLongRoom = true;

        Vector2 newPosition = transform.position;
        if (followingType == FollowingTypes.FollowingX)
        {
            newPosition.x = playerTarget.position.x;
            if (newFollowingType == FollowingTypes.FollowingX)
            {
                if (transform.position.x > position.x) newPosition.x -= config.LongHorizontalRoomEntranceOffset;
                else if (transform.position.x < position.x) newPosition.x += config.LongHorizontalRoomEntranceOffset;
            }
        }
        else if (followingType == FollowingTypes.FollowingY)
        {
            newPosition.y = playerTarget.position.y;
            if (newFollowingType == FollowingTypes.FollowingY)
            {
                if (transform.position.y > position.y) newPosition.y -= config.LongVerticalRoomEntranceOffset;
                else if (transform.position.y < position.y) newPosition.y += config.LongVerticalRoomEntranceOffset + 1;
            }
        }
        RaycastHit2D rayHit = Physics2D.Raycast(playerTarget.position, position - (Vector2) playerTarget.position, 2f, 1 << Definitions.LAYER_ROOM);
        float maxOffset = 0;

        if (newFollowingType == FollowingTypes.FollowingX)
        {
            newPosition.y = position.y;
            if (rayHit.transform != null) maxOffset = rayHit.transform.GetComponent<Room>().GetSize().x / 2;

            if (transform.position.x > position.x)
            {
                clampMax = newPosition.x + 1;
                clampMin = newPosition.x - maxOffset - 1;
            }
            else if (transform.position.x < position.x)
            {
                clampMin = newPosition.x - 1;
                clampMax = newPosition.x + maxOffset + 1;
            }
        }
        else if (newFollowingType == FollowingTypes.FollowingY)
        {
            newPosition.x = position.x;
            if (rayHit.transform != null) maxOffset = rayHit.transform.GetComponent<Room>().GetSize().y / 2;

            if (transform.position.y > position.y)
            {
                clampMax = newPosition.y;
                clampMin = newPosition.y - maxOffset;
            }
            else if (transform.position.y < position.y)
            {
                clampMin = newPosition.y;
                clampMax = newPosition.y + maxOffset;
            }
        }

        float currentTime = 0;
        Vector2 startPos = transform.position;
        while (isClamping && currentTime < config.MoveWhenEnteredLongRoomTime)
        {
            currentTime += Time.unscaledDeltaTime;
            smoothedPosition = Vector2.Lerp(startPos, newPosition, currentTime / config.MoveWhenEnteredLongRoomTime);
            transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, positionZ);
            yield return null;
        }
        
        isEnteringLongRoom = false;
    }
}
