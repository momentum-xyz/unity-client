using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Odyssey;

[CustomEditor(typeof(WorldEventsSimulator))]
public class WorldEventsSimulatorInspector : Editor
{

    WorldEventsSimulator worldEventsSimulator;

    private string searchBuildingName = "";
    private string[] searchResults;
    private int choice = 0;
    private WorldObject currentSelectedWorldObject = null;
    private string lastSelectedName = "";
    private string worldID = "";
    private string[] users;
    private int choiceUser = 0;
    private int availWorldsChoice = 0;

    string spaceName = "";
    string spaceType = "";

    private string[] testWorlds = new string[] { "d83670c7-a120-47a4-892d-f9ec75604f74", "4567abb3-95e4-46fb-bde4-45ce7491b1ad", "03facf88-c980-4052-a591-7d88c8d28b31", "8935a05e-d8b8-4965-97c5-49512df062f3", "a9467d01-0a2c-4637-bf5c-feda7b10a637", "d9592b99-b143-4b8e-ac1a-a1c02ffa45e5" };

    private string _bridgingEffectEmitter = "";
    private Vector3 _bridgingEffectSourcePos;
    private Vector3 _bridgingEffectDestPos;
    private int _bridgingEffectType = 0;

    private string _effectEmitter = "";
    private string _effectSource = "";
    private int _effectType = 0;

    private int _interactionMsgFlag = 0;
    private int _interactionMsgType = 0;
    private string _interactionMsgGuid = "";
    private string _interactionMsgString = "";

    private string _privacyGuid = "";
    private int _privacyMode = 0;

    private float _relayChainRadius = 300.0f;

    private string stageModeGUID = "";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        worldEventsSimulator = target as WorldEventsSimulator;

        EditorGUILayout.Space();
        GUILayout.Label("Structures Search", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Struct Name or GUID:", GUILayout.Width(130));
        string bName = GUILayout.TextField(searchBuildingName);

        if (bName != searchBuildingName)
        {
            choice = 0;
            searchBuildingName = bName;
            SearchForStructuresWithName(searchBuildingName);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Search Results: ");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        if (searchResults != null)
        {
            choice = EditorGUILayout.Popup(choice, searchResults);

            if (searchResults != null && searchResults.Length > 0)
            {
                string selectedName = searchResults[choice];

                if (selectedName != lastSelectedName)
                {
                    currentSelectedWorldObject = worldEventsSimulator.GetObjectByName(searchResults[choice]);
                    lastSelectedName = selectedName;
                }
            }
        }

        EditorGUILayout.EndHorizontal();


        if (searchResults != null && searchResults.Length > 0)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Fly To Space"))
            {
                worldEventsSimulator.FlyToSpaceWithName(searchResults[choice]);
            }
            EditorGUILayout.EndHorizontal();


            if (currentSelectedWorldObject != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Structure Info: ");
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("GUID:" + currentSelectedWorldObject.guid.ToString());
                EditorGUILayout.EndHorizontal();
            }

        }

        EditorGUILayout.Space();
        GUILayout.Label("World Events", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Avail Worlds: ", GUILayout.Width(130));
        availWorldsChoice = EditorGUILayout.Popup(availWorldsChoice, testWorlds);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("World ID: ", GUILayout.Width(130));
        worldID = GUILayout.TextField(worldID);
        GUILayout.EndHorizontal();


        if (GUILayout.Button("NewWorldEvent"))
        {
            if (worldID.Length > 0)
            {
                worldEventsSimulator.SimNewWorld(worldID);
            }
            else
            {
                worldEventsSimulator.SimNewWorld(testWorlds[availWorldsChoice]);
            }

        }

        EditorGUILayout.Space();
        GUILayout.Label("Search Users", EditorStyles.boldLabel);
        EditorGUILayout.Space();


        /*
        GUILayout.BeginHorizontal();
        string username = GUILayout.TextField(searchUsername);
        GUILayout.EndHorizontal();

        if(username != searchUsername)
        {
            searchUsername = username;
            SearchForUsersWithName(username);
        }
        */
        GUILayout.BeginHorizontal();

        if (worldEventsSimulator.usersGuidsArray != null)
        {
            choiceUser = EditorGUILayout.Popup(choiceUser, worldEventsSimulator.usersGuidsArray, GUILayout.Width(300));
        }

        if (GUILayout.Button("Teleport"))
        {
            worldEventsSimulator.TeleportToUser(worldEventsSimulator.usersGuidsArray[choiceUser].ToString());
        }

        if (GUILayout.Button("Follow"))
        {
            worldEventsSimulator.FollowUser(worldEventsSimulator.usersGuidsArray[choiceUser].ToString());
        }

        if (GUILayout.Button("Stop"))
        {
            worldEventsSimulator.StopFollowUser(worldEventsSimulator.usersGuidsArray[choiceUser].ToString());
        }

        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        GUILayout.Label("Add New Space", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Name: ", GUILayout.Width(130));
        spaceName = GUILayout.TextField(spaceName);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Space Type: ", GUILayout.Width(130));
        spaceType = GUILayout.TextField(spaceType);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Add"))
        {
            worldEventsSimulator.AddNewSpace(false, spaceName, spaceType);
        }

        EditorGUILayout.Space();
        GUILayout.Label("Effects", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        _bridgingEffectEmitter = EditorGUILayout.TextField("Emitter: ", _bridgingEffectEmitter);
        _bridgingEffectSourcePos = EditorGUILayout.Vector3Field("Source Pos: ", _bridgingEffectSourcePos);
        _bridgingEffectDestPos = EditorGUILayout.Vector3Field("Dest Pos: ", _bridgingEffectDestPos);
        _bridgingEffectType = EditorGUILayout.IntField("Effect Type: ", _bridgingEffectType);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Send Bridging Effect"))
        {
            worldEventsSimulator.TriggerBridgeEffect(Guid.Parse(_bridgingEffectEmitter), _bridgingEffectSourcePos, _bridgingEffectDestPos, _bridgingEffectType);
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        GUILayout.Label("Single Effect", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        _effectEmitter = EditorGUILayout.TextField("Emitter: ", _effectEmitter);
        _effectSource = EditorGUILayout.TextField("Source: ", _effectSource);
        _effectType = EditorGUILayout.IntField("Effect Type: ", _effectType);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Send Single Effect"))
        {
            worldEventsSimulator.TriggerEffect(Guid.Parse(_effectEmitter), Guid.Parse(_effectSource), _effectType);
        }
        GUILayout.EndHorizontal();


        EditorGUILayout.Space();
        GUILayout.Label("Trigger Interaction", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        _interactionMsgFlag = EditorGUILayout.IntField("Flag:", _interactionMsgFlag);
        _interactionMsgGuid = EditorGUILayout.TextField("Target: ", _interactionMsgGuid);
        _interactionMsgType = EditorGUILayout.IntField("Type: ", _interactionMsgType);
        _interactionMsgString = EditorGUILayout.TextField("Message: ", _interactionMsgString);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Send Interaction"))
        {
            worldEventsSimulator.TriggerInteractionMsg((uint)_interactionMsgType, Guid.Parse(_interactionMsgGuid), _interactionMsgFlag, _interactionMsgString);
        }

        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        GUILayout.Label("Relay Chain Radius", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        _relayChainRadius = EditorGUILayout.FloatField("Radius: ", _relayChainRadius);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Change Radius"))
        {
            worldEventsSimulator.UpdateRelayChainRadius(_relayChainRadius);
        }

        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        GUILayout.Label("StageMode Test", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        stageModeGUID = EditorGUILayout.TextField("GUID", stageModeGUID);
        if (GUILayout.Button("Enable"))
        {
            worldEventsSimulator.SetStageMode(stageModeGUID, true);
        }

        if (GUILayout.Button("Disable"))
        {
            worldEventsSimulator.SetStageMode(stageModeGUID, false);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        GUILayout.Label("Privacy Mode", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        _privacyGuid = EditorGUILayout.TextField("GUID", _privacyGuid);

        if (GUILayout.Button("0"))
        {
            worldEventsSimulator.UpdatePrivacy(Guid.Parse(_privacyGuid), 0);
        }

        if (GUILayout.Button("1"))
        {
            worldEventsSimulator.UpdatePrivacy(Guid.Parse(_privacyGuid), 1);
        }

        if (GUILayout.Button("2"))
        {
            worldEventsSimulator.UpdatePrivacy(Guid.Parse(_privacyGuid), 2);
        }

        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Application:");

        if (GUILayout.Button("Pause"))
        {
            worldEventsSimulator.SetPaused(true);
        }

        if (GUILayout.Button("Resume"))
        {
            worldEventsSimulator.SetPaused(false);
        }

        EditorGUILayout.EndHorizontal();

        serializedObject.Update();

        EditorUtility.SetDirty(target);
    }


    void SearchForStructuresWithName(string name)
    {
        if (!Application.isPlaying) return;

        List<string> structures = new List<string>();

        Dictionary<System.Guid, WorldObject> worldObjects = worldEventsSimulator.ShaderContext().Get<IWorldData>().WorldHierarchy;

        foreach (KeyValuePair<System.Guid, WorldObject> obj in worldObjects)
        {
            if (obj.Value.name.ToLower().Contains(name.ToLower()) || obj.Value.guid.ToString().ToLower().Contains(name.ToLower()))
            {
                structures.Add(obj.Value.name);
            }
        }

        searchResults = structures.ToArray();
    }

}
