using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Odyssey;

public class AlphaCineMode : MonoBehaviour, IRequiresContext
{
    public GameObject cineModeWindow;
    public GameObject wisp;
    public GameObject wispCore;
    public Text point1Text;
    public Text point2Text;
    public Text point3Text;
    private Vector3 point1 = new Vector3(0, 0, 0);
    private Vector3 point2 = new Vector3(0, 0, 0);
    private Vector3 point3 = new Vector3(0, 0, 0);
    private IEnumerator coroutine;
    public InputField durationInput;
    public InputField wispIDToFollowInput;
    public InputField wispFollowDistanceInput;
    public Toggle showWisp;
    public Dropdown movieMode;
    private bool movingForward = true;
    private Guid targetWispIDparsed = Guid.Empty;
    private Vector3 targetWispIDposition;
    private bool animationPlaying = false;
    public bool cineModeActive = false;

    IMomentumContext _c;

    public void Init(IMomentumContext context)
    {
        this._c = context;

        // TODO: delete - hardcoding this as copy and paste doesn't work inside a webgl build
        wispIDToFollowInput.text = "69ce3663-c917-49ce-a0e0-3373c5910448";
    }

    public void Dispose()
    {

    }

    void Update()
    {
        if (cineModeActive == true)
        {
            if (wisp == null)
            {
                wisp = GameObject.FindGameObjectWithTag("Player");
                wispCore = wisp.transform.GetChild(0).gameObject;
            }

            // don't pick up numbers if a text field is open
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    point1 = wisp.transform.position;
                    point1Text.text = point1.ToString();
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    point2 = wisp.transform.position;
                    point2Text.text = point2.ToString();
                }

                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    point3 = wisp.transform.position;
                    point3Text.text = point3.ToString();
                }
            }
        }
    }

    IEnumerator lightsCameraAction()
    {
        float timeToTake = float.Parse(durationInput.text);
        float wispFollowDistance = float.Parse(wispFollowDistanceInput.text);

        int mode = movieMode.value;
        float time = 0;
        float stepAngle = 0f;
        bool reachedTarget = false;

        if (showWisp.isOn == false)
        {
            wispCore.SetActive(false);
        }

        if (mode == 1 || mode == 3)
        {
            stepAngle = 360f / timeToTake;
        }

        while (true)
        {
            // move between two points (mode 0 and mode 2)
            if (mode == 0 || mode == 2)
            {
                if (movingForward == true)
                {
                    while (time < timeToTake)
                    {
                        wisp.transform.position = Vector3.Lerp(point1, point2, time / timeToTake);
                        time += Time.deltaTime;

                        // ping pong and look
                        if (mode == 2)
                        {
                            wisp.transform.LookAt(point3);
                        }

                        yield return null;
                    }

                    wisp.transform.position = point2;
                    movingForward = false;
                    time = 0;

                }
                else
                {
                    while (time < timeToTake)
                    {
                        wisp.transform.position = Vector3.Lerp(point2, point1, time / timeToTake);
                        time += Time.deltaTime;

                        // ping pong and look
                        if (mode == 2)
                        {
                            wisp.transform.LookAt(point3);
                        }

                        yield return null;
                    }

                    wisp.transform.position = point1;
                    movingForward = true;
                    time = 0;
                }
            }

            // orbit mode (mode 1)
            if (mode == 1)
            {
                time = 0;
                while (true)
                {

                    wisp.transform.RotateAround(point1, Vector3.up, stepAngle * Time.deltaTime);

                    yield return null;
                }

            }

            if (mode == 3)
            {

                Vector3 wispPos;

                while (reachedTarget == false)
                {
                    wispPos = _c.Get<IWispManager>().GetWispPosition(targetWispIDparsed);

                    wisp.transform.position = Vector3.Lerp(point1, wispPos, time / timeToTake);
                    wisp.transform.LookAt(wispPos);
                    time += Time.deltaTime;

                    if (Vector3.Distance(wisp.transform.position, wispPos) < wispFollowDistance)
                    {
                        reachedTarget = true;
                    }

                    yield return null;
                }

                if (reachedTarget == true)
                {
                    wispPos = _c.Get<IWispManager>().GetWispPosition(targetWispIDparsed);

                    wisp.transform.RotateAround(wispPos, Vector3.up, stepAngle * Time.deltaTime);
                    wisp.transform.LookAt(wispPos);
                    wisp.transform.position = (wisp.transform.position - wispPos).normalized * wispFollowDistance + wispPos;

                }

                yield return null;
            }
        }
    }

    public void playButton()
    {
        if (animationPlaying == false)
        {
            // check for valid wisp id
            if (movieMode.value == 3)
            {
                if (wispIDToFollowInput.text != "")
                {
                    if (Guid.TryParse(wispIDToFollowInput.text, out targetWispIDparsed) == false)
                    {
                        wispIDToFollowInput.text = "Unrecognised wisp ID";
                    }
                    else
                    {
                        if (_c.Get<IWispManager>().GetWispPosition(targetWispIDparsed) == new Vector3(0f, 0f, 0f))
                        {
                            wispIDToFollowInput.text = "Wisp is not logged in";
                        }
                        else
                        {
                            point1 = wisp.transform.position;
                            point1Text.text = wisp.transform.position.ToString();
                            coroutine = lightsCameraAction();
                            StartCoroutine(coroutine);
                        }
                    }

                }
            }
            else
            {
                animationPlaying = true;
                coroutine = lightsCameraAction();
                StartCoroutine(coroutine);
            }
        }
    }

    public void stopButton()
    {
        animationPlaying = false;
        wispCore.SetActive(true);
        StopCoroutine(coroutine);
    }

}
