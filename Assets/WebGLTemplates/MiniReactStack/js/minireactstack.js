ReactUnityWebGL = {
    mqttClient: null,
    MomentumLoaded: function() {

        this.getIntState = window.unityInstance.Module.cwrap('extGetIntState', 'number', ['string', 'string']);
        this.setIntState = window.unityInstance.Module.cwrap('extSetIntState', null, ['string', 'string','number']);
        this.getStrState = window.unityInstance.Module.cwrap('extGetStrState', 'string', ['string', 'string']);
        this.setStrState = window.unityInstance.Module.cwrap('extSetStrState', null, ['string', 'string','string']);

        window.unityInstance.SendMessage('UnityManager', 'setOverwriteDomain', domain);

        if(overwritePosbusURL) {
            window.unityInstance.SendMessage('UnityManager', 'setPosbusURL', posbusURL);
        }

        if(overwriteAddressablesURL) {
            window.unityInstance.SendMessage('UnityManager', 'setAddressablesURL', addressablesURL);
        }

        window.unityInstance.SendMessage('UnityManager', 'setToken', authToken);
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