ReactUnityWebGL = {
    mqttClient: null,
    MomentumLoaded: function() {

      //  window.unityInstance.SendMessage('UnityManager', 'setOverwriteDomain', domain);

        // if(overwritePosbusURL) {
        //     window.unityInstance.SendMessage('UnityManager', 'setPosbusURL', posbusURL);
        // }

        // if(overwriteAddressablesURL) {
        //     window.unityInstance.SendMessage('UnityManager', 'setAddressablesURL', addressablesURL);
        // }

        window.unityInstance.Module.UnityAPI.setToken(authToken);
    },
    TeleportReady: function() {

    },
    ProfileHasBeenClicked: function(userID) {

    },
    ClickEvent: function(combined) {
        var str = combined.split('|');
        console.log(str[0]);
        console.log(str[1]);    
    },
    ExterminateUnity: function() {

    },
    HideLogo: function() {

    },
    RelayMessage: function(target, msg) {
        console.log('Got relay msg: '+target+','+msg);
    }
}