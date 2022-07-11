using System.Collections;
using OdinNative.Odin.Room;
using UnityEngine;

public class PositionUpdateTest : MonoBehaviour
{
    [SerializeField] private float Scaling = 0.25f;
    [SerializeField] private float PositionUpdate = 3.0f;


    [SerializeField] private string[] connectedOdinRooms;

    private void OnEnable()
    {
        StartCoroutine(DelayedEnable());
    }

    private void OnDisable()
    {
        if (OdinHandler.Instance) OdinHandler.Instance.OnRoomJoined.RemoveListener(OnRoomJoined);

        StopCoroutine(ManualUpdatePosition());
    }

    public IEnumerator ManualUpdatePosition()
    {
        while (enabled)
        {
            if (OdinHandler.Instance)
                foreach (var odinRoomName in connectedOdinRooms)
                    if (OdinHandler.Instance.Rooms.Contains(odinRoomName))
                        UpdateRoomPosition(OdinHandler.Instance.Rooms[odinRoomName]);
            yield return new WaitForSeconds(PositionUpdate);
        }
    }

    private IEnumerator DelayedEnable()
    {
        while (!OdinHandler.Instance || null == OdinHandler.Instance.Rooms)
            yield return null;

        foreach (Room room in OdinHandler.Instance.Rooms) InitRoom(room);
        OdinHandler.Instance.OnRoomJoined.AddListener(OnRoomJoined);

        StartCoroutine(ManualUpdatePosition());
    }

    private void OnRoomJoined(RoomJoinedEventArgs eventArgs)
    {
        if (null != eventArgs && null != eventArgs.Room) InitRoom(eventArgs.Room);
    }

    private void UpdateRoomPosition(Room toUpdate)
    {
        if (null != toUpdate && toUpdate.IsJoined)
        {
            var position = transform.position;
            bool success = toUpdate.UpdatePosition(position.x, position.z);
            Debug.Log($"Updated to position {position.x}, {position.z}, Success: {success}");
        }
    }

    private void InitRoom(Room toInit)
    {
        bool wasPositionScaledInitialized = toInit.SetPositionScale(Scaling);
        Debug.Log($"Initialized Position Scale: {wasPositionScaledInitialized}");
    }
}