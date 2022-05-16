using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary> Fixes the horrible setup Unity UI requires to simply fit a panel to its (TMPRo) child </summary>
[ExecuteInEditMode]
public class UITextChildFitterBecauseUnityUISucksSoMuch : MonoBehaviour
{
    public float MinSize = 32;
    public float Padding = 3;
    public bool OnlyUpdateOnChangeOfChild = true;
    TMP_Text _text;
    string _curString;
    RectTransform _rect;
    bool _doExtraUpdate;
	bool _forceUpdate;


    void Awake()
    {
        TryGetRefs();
    }

	void OnEnable()
	{
		_forceUpdate = true;
	}

    bool TryGetRefs()
    {
        // buncha boilerplate to make us performant within edit mode.
        if( _rect == null ) _rect = GetComponent<RectTransform>();
        if( !_rect ) return false;
        if( _text != null ) return true;
        _text = GetComponentInChildren<TMP_Text>();
        if( !_text ) return false;

        // some initial setup
        _curString = _text.text;
        var fitter = _text.GetComponent<ContentSizeFitter>();
        if( !fitter ) fitter = _text.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        return true;
    }


    void Update()
    {
        if( !TryGetRefs() ) return;
        if( 
            OnlyUpdateOnChangeOfChild==false 
            || _doExtraUpdate 
            || _curString != _text.text 
            || _text.havePropertiesChanged 
			|| _forceUpdate )
        {
            // var bounds = _text.bounds;
            _rect.SetSizeWithCurrentAnchors( 
                RectTransform.Axis.Horizontal, 
                Mathf.Max( _text.bounds.size.x + Padding, MinSize )
            );

            _curString = _text.text;
            // we need to force an update next frame
            if( OnlyUpdateOnChangeOfChild ) _doExtraUpdate = !_doExtraUpdate;
            else _doExtraUpdate = false;

			_forceUpdate = false;
        }
    }
}
