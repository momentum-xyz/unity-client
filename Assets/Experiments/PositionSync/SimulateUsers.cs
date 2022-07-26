using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey;
using Odyssey.Networking;
using System;
using Random = UnityEngine.Random;

public class FakeMovingUser
{
    public Guid guid;
    public Vector3 position;
    public Vector3 moveDirection;
    public float internalTimer = 0.0f;
}

public class SimulateUsers : MonoBehaviour
{
    public HS.ThirdPersonController controller;
    public float lagSeconds = 0.5f;
    public Camera cam;
    public WispManager wispManager;
    private IMomentumContext _c;
    public float cruiseSpeed = 64.0f;

    private Guid playerGuid;
    List<FakeMovingUser> _fakeUsers = new List<FakeMovingUser>();
    float timer = 0.0f;


    void Awake()
    {
        InitContext();
    }

    void Start()
    {
        _c.Get<IWispManager>().InitWispsPrefabsPool();
        _c.Get<IWispManager>().StartRunning();

        //  CreateFakeUsers(5);
        playerGuid = Guid.NewGuid();

        _c.Get<ISessionData>().WorldIsTicking = true;
    }

    private void OnDestroy()
    {
        _c.Get<IWispManager>().Stop();
    }

    void InitContext()
    {
        _c = new MomentumContext();

        IPosBus posBus = new PosBus(new HybridWS());
        ISessionData sessionData = new SessionData();

        _c.RegisterService<ISessionData>(sessionData);
        _c.RegisterService<IPosBus>(posBus);
        _c.RegisterService<IWispManager>(wispManager);

        sessionData.AvatarCamera = cam;
    }

    void CreateFakeUsers(int num)
    {

        for (var i = 0; i < num; i++)
        {
            _fakeUsers.Add(new FakeMovingUser()
            {
                guid = Guid.NewGuid(),
                position = UnityEngine.Random.onUnitSphere * 5.0f,
                moveDirection = UnityEngine.Random.insideUnitSphere.normalized
            });
        }
    }

    private void Update()
    {

        UpdateUsersPositions();

        timer += Time.deltaTime;

        if (timer >= lagSeconds)
        {
            timer = 0.0f;
            SendUserPositions();
        }

    }

    void SendUserPositions()
    {
        foreach (var u in _fakeUsers)
        {
            _c.Get<IPosBus>().OnPosBusMessage.Invoke(new PosBusPosMsg()
            {
                userId = u.guid,
                position = u.position
            });
        }

        _c.Get<IPosBus>().OnPosBusMessage.Invoke(new PosBusPosMsg()
        {
            userId = playerGuid,
            position = controller.transform.position
        });
    }

    void UpdateUsersPositions()
    {
        foreach (var u in _fakeUsers)
        {
            u.internalTimer += Time.deltaTime;

            if (u.internalTimer >= UnityEngine.Random.Range(4.0f, 6.0f))
            {
                u.internalTimer = 0.0f;
                u.moveDirection += new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
                u.moveDirection.Normalize();
            }

            u.position += u.moveDirection * Time.deltaTime * cruiseSpeed;
        }
    }

}
