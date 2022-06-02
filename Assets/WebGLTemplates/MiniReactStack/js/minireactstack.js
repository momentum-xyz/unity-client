ReactUnityWebGL = {
    MomentumLoaded: function() {
        window.unityInstance.Module.PosBusRelay.OnConnected("c400b120-dc1f-4154-9f4a-e40de785b0c4");  
        
        let a = new ArrayBuffer(5);
        let ab = new Uint8Array(a);
        console.log(Uint8Array.BYTES_PER_ELEMENT);

        ab[0] = 1;
        ab[1] = 55;
        ab[2] = 10;
        ab[3] = 1
        ab[4] = 12;

        window.unityInstance.Module.PosBusRelay.RelayMessage(a)
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

    }
}