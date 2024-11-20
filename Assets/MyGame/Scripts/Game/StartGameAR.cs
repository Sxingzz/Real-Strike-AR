using Niantic.Lightship.SharedAR.Colocalization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StartGameAR : MonoBehaviour
{
    [SerializeField] private SharedSpaceManager _sharedSpaceManager;
    [SerializeField] private Texture2D _targetImage;
    [SerializeField] private float _targetImageSize;

    [SerializeField] private Button StartGameButton;
    [SerializeField] private Button CreateRoomButton;
    [SerializeField] private Button JoinRoomButton;

    private const int MAX_AMOUNT_CLIENTS_ROOM = 2;
    private bool isHost;

    private string RoomName = "Test Room";

    public static event Action OnStartSharedSpaceHost;
    public static event Action OnJoinSharedSpaceClient;
    public static event Action OnStartGame;
    public static event Action OnStartSharedSpace;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _sharedSpaceManager.sharedSpaceManagerStateChanged += 
            SharedSpaceManagerOnsharedSpaceManagerStateChanged;

        StartGameButton.onClick.AddListener(StartGame);
        CreateRoomButton.onClick.AddListener(CreateGameHost);
        JoinRoomButton.onClick.AddListener(JoinGameClient);

        StartGameButton.interactable = false;

        BlitImageForColocalization.OnTextureRendered += 
                                        BlitImageForColocalization_OnTextureRendered;
    }

    private void OnDestroy()
    {
        _sharedSpaceManager.sharedSpaceManagerStateChanged -=
            SharedSpaceManagerOnsharedSpaceManagerStateChanged;

        BlitImageForColocalization.OnTextureRendered -=
                                        BlitImageForColocalization_OnTextureRendered;
    }

    private void BlitImageForColocalization_OnTextureRendered(Texture2D texture)
    {
        SetTargetImage(texture);
        StartSharedSpace();
    }

    private void SetTargetImage(Texture2D texture2d)
    {
        _targetImage = texture2d;
    }

    private void SharedSpaceManagerOnsharedSpaceManagerStateChanged(
        SharedSpaceManager.SharedSpaceManagerStateChangeEventArgs obj)
    {
        if (obj.Tracking)
        {
            StartGameButton.interactable = true;
            CreateRoomButton.interactable = false;
            JoinRoomButton.interactable = false;
        }
    }

    private void StartGame()
    {
        OnStartGame?.Invoke();

        if (isHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }

    private void StartSharedSpace()
    {
        OnStartSharedSpace?.Invoke();

        if (_sharedSpaceManager.GetColocalizationType() == SharedSpaceManager.
            ColocalizationType.MockColocalization)
        {
            var mockTrackingArgs = ISharedSpaceTrackingOptions.
                                                        CreateMockTrackingOptions();
            var roomArgs = ISharedSpaceRoomOptions.CreateLightshipRoomOptions(
                RoomName,
                MAX_AMOUNT_CLIENTS_ROOM,
                "MockColocaLizationDemo"
                );

            _sharedSpaceManager.StartSharedSpace(mockTrackingArgs, roomArgs);
            return;
        }


        if (_sharedSpaceManager.GetColocalizationType() == SharedSpaceManager.
            ColocalizationType.ImageTrackingColocalization)
        {
            var imageTrackingOptions = ISharedSpaceTrackingOptions.
                                                        CreateImageTrackingOptions(
                _targetImage, _targetImageSize
                );

            var roomArgs = ISharedSpaceRoomOptions.CreateLightshipRoomOptions(
                RoomName,
                MAX_AMOUNT_CLIENTS_ROOM,
                "ImageColocaLization"
                );

            _sharedSpaceManager.StartSharedSpace(imageTrackingOptions, roomArgs);
            return;
        }

    }

    private void CreateGameHost()
    {
        isHost = true;
        OnStartSharedSpaceHost?.Invoke();
    }

    private void JoinGameClient()
    {
        isHost = false;
        OnJoinSharedSpaceClient?.Invoke();
    }





}
