# unity-client
Momentum Unity Client

## Development

### TL;DR

```bash
git clone git@github.com:OdysseyMomentumExperience/unity-client.git
cd unity-client
git checkout develop
git submodule update --init
# Open project in unity-editor
# Make sure to change the build settings to WebGL platform
```

## Running Momentum from the Unity Editor

In order to run Momentum in the Editor, you need an authentication token, which you usually get from the React layer while running in the browser. In the case of running in the Unity Editor, the Authentication Token is get directly by quering Keycloak service. Checkout the manual below on how to do that.

## Getting an Authentication Token in the Editor and Setting up Developer Credentials

On the top of every Networking Configuration data file, you will find a handy "Get Token" button. That essentialy will get a token from Keycloak, based on the credentials you have prodived. Credentials are put in a file called "develop_credentials" which should be put just outside the "Assets" folder. It is ignored, so your username/password is safe. The format for it is the following:

```
{
        "username": "your@email",
        "password": "password",
        "keycloak_url": "https://dev-x5u42do.odyssey.ninja/auth/realms/Momentum/protocol/openid-connect/token",
        "keycloak_client_id":"react-client"
}
```

The example uses the develop keycloak URL, but if you want to test on production or acceptance, you have to update it.

## Running the Unity Client from a local Web server

The unity-client project comes with it's own local web server service that you can run to test your WebGL builds. Here are the steps to do so.

1. Make sure you have Node.js installed and you know how to get an 
Authentication token in the Editor, mentioned above.

2. Go to your project's main folder in your favorite Terminal app and type: 

`npm install`

to install all the server's required modules.

3. Create a folder called *build* inside the project's main folder.

4. In the project's hierarchy find a data asset called *WebGLLocalConfiguration*, press the Get Token button, make sure you have setup your developer credentials (described above).

5. Make a WebGL build and point the *build* folder you created in step 2 as a target for your build

6. Go to your main project folder in the Terminal and type:

`npm start`

7. Open http://localhost:4040/ in your browser.

8. Go to step 3 for every new build

**Important:** One of the problems you may encounter when you run the WebGL build on a local server is that Textures and Backend requests may not work, because of CORS policy errors. To solve this issue you can use a browser extension that adds the needed CORS headers to your requests. 

A tested and recommened one is the [Moesif Origin & CORS Changer](https://chrome.google.com/webstore/detail/moesif-origin-cors-change/digfbfaphojjndkpccljibejjbppifbc) extension for Chrome. You need to Enable it when you are opening the WebGL app and keep it disabled in your other browser activities, because it may mess up a lot of things. 