/*
This file defines functions that will be available on the managed Unity side via DllImport.
Most of them function as a bridge between Unity's Emscripten side and the React layer of Momentum via ReactUnityWebGL object.
*/

mergeInto(LibraryManager.library, {
	SendReadyForTeleport: function() {
		ReactUnityWebGL.TeleportReady();
	},
    ProfileClickEvent: function(userID) {
        ReactUnityWebGL.ProfileHasBeenClicked(UTF8ToString(userID));
    },
	SendExterminateUnityRequest: function()	{
		ReactUnityWebGL.ExterminateUnity()
	},
    SendClickEvent: function(combinedMessage) {        
        ReactUnityWebGL.ClickEvent(UTF8ToString(combinedMessage));
    },
    MomentumLoaded: function() {
        ReactUnityWebGL.MomentumLoaded();
    },
    TeamPlasmaClickEvent: function(trackID) {
        ReactUnityWebGL.TeamPlasmaClickEvent(UTF8ToString(trackID));
    },
    DownloadFile: function(textString, fileNamePtr) {
        var fileName = UTF8ToString(fileNamePtr);
        var readableString = UTF8ToString(textString);
        var link = document.createElement('a');
        link.download = fileName;
        var blob = new Blob([readableString], {
            type: 'text/plain'
        });
        link.href = window.URL.createObjectURL(blob);
        link.click();
    },
    GetBrowserName: function() {
        function DuckTestBrowserName() {

            //var BrowserType = {
            //    OPERA: 1,
            //    FIREFOX: 2,
            //    SAFARI: 3,
            //    INTERNETEXPLORER: 4,
            //    EDGE: 5,
            //    CHROME: 6,
            //    EDGECHRONIUM: 7,
            //    BLINK: 8,
            //    OTHER: 9,
            //}

            //var browserType = BrowserType.OTHER;

            var output = "Other";

            var isOpera = false;
            // Opera 8.0+
            if ((!!window.opr && !!opr.addons) || !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0) {
                isOpera = true;
                output = "Opera"
            }

            var isFirefox = false;
            // Firefox 1.0+
            if (typeof InstallTrigger !== 'undefined') {
                output = "Firefox"
                isFirefox = true;
            }

            // Safari 3.0+ "[object HTMLElementConstructor]" 
            var isSafari = false;
            if (/constructor/i.test(window.HTMLElement) || (function(p) {
                    return p.toString() === "[object SafariRemoteNotification]";
                })(!window['safari'] || (typeof safari !== 'undefined' && safari.pushNotification))) {
                output = "Safari";
                isSafari = true;
            }

            var isIE = false;
            // Internet Explorer 6-11
            if ( /*@cc_on!@*/ false || !!document.documentMode) {
                output = "Internet Explorer"
                isIE = true;
            }

            // Edge 20+
            var isEdge = false;
            if (!isIE && !!window.StyleMedia) {
                isEdge = true;
                output = "Edge"
            }


            // Chrome 1 - 79
            var isChrome = false
            if (!!window.chrome && (!!window.chrome.webstore || !!window.chrome.runtime)) {
                output = "Chrome"
                isChrome = true;
            }

            // Edge (based on chromium) detection
            var isEdgeChromium = false;
            if (isChrome && (navigator.userAgent.indexOf("Edg") != -1)) {
                output = "Edge Chronium"
                isEdgeChromium = true;
            }

            //// Blink engine detection
            //var isBlink = false;
            //if ((isChrome || isOpera) && !!window.CSS)
            //{
            //    output = "Blink"
            //    isBlink = true;
            //}

            return output;
        }

        var message = DuckTestBrowserName();
        var bufferSize = lengthBytesUTF8(message) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(message, buffer, bufferSize);
        return buffer;
    },

    GetBrowserInfo: function() {
        var message = "Version: " + window.navigator.appVersion + "\n";
        message += "Language: " + window.navigator.language + "\n";
        message += "Is Browser online?: " + window.navigator.onLine + "\n";

        var bufferSize = lengthBytesUTF8(message) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(message, buffer, bufferSize);
        return buffer;
    },
    GetGraphicCard: function() {
        const glcontext = GL.currentContext.GLctx;
        const debugInfo = glcontext.getExtension('WEBGL_debug_renderer_info');
        const graphicCard = glcontext.getParameter(debugInfo.UNMASKED_RENDERER_WEBGL);

        // How to return strings: https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html
        var bufferSize = lengthBytesUTF8(graphicCard) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(graphicCard, buffer, bufferSize);
        return buffer;
    },
    WaypointReached: function(waypointIndex) {
        //TODO: implement
    },
    SimpleNotification: function(kind, flag, message) {
        ReactUnityWebGL.SimpleNotification(kind, flag, UTF8ToString(message));
    },
    RelayMessage: function(target, message) {
        ReactUnityWebGL.RelayMessage(UTF8ToString(target), UTF8ToString(message));
    }, 
   	PosBusConnected: function()	{
    	ReactUnityWebGL.PosBusConnected()
    }
});