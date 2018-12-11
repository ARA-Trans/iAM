import * as Msal from 'msal';

export default class AuthService {
    applicationConfig: { clientID: string; graphScopes: string[]; authority: string; };
    app: Msal.UserAgentApplication;
    logMessage(s: string) {
        console.log(s);
    }
    constructor() {
        let PROD_REDIRECT_URI = 'http://localhost:1337/';
        let redirectUri = window.location.origin;
        if (window.location.hostname !== '127.0.0.1') {
            redirectUri = PROD_REDIRECT_URI;
        }

        this.applicationConfig = {
            clientID: '6b4fef89-350c-4dfe-8aa2-5ed5fddb9c5b',
            graphScopes: ['https://aradomain.onmicrosoft.com/user/user_impersonation'],
            authority: 'https://login.microsoftonline.com/tfp/aradomain.onmicrosoft.com/b2c_1_su-si-pol'
        };
        this.app = new Msal.UserAgentApplication(
            this.applicationConfig.clientID, this.applicationConfig.authority,
            (errorDesc, token, error, tokenType) => {
                // callback for login redirect
            },
            {
                redirectUri
            }
        );
    }

    login() {
        return this.app.loginPopup(this.applicationConfig.graphScopes).then(
            idToken => {
                this.app.acquireTokenSilent(this.applicationConfig.graphScopes).then(
                    accessToken => {
                        this.updateUI();
                    }, error => {
                        this.app.acquireTokenPopup(this.applicationConfig.graphScopes).then(accessToken => {
                            this.updateUI();
                        }, error => {
                            this.logMessage("Error acquiring the popup:\n" + error);
                        });
                    })
                //const user = this.app.getUser();
                //if (user) {
                //    return user;
                //}
                //else {
                //    return null;
                //}
            },
            (error) => {
                this.logMessage("Error during login:\n" + error);
            }
        );
    };

    updateUI() {
        var userName = this.app.getUser().name;
        this.logMessage("User '" + userName + "' logged-in");
    }

    logout() {
        this.app.logout();
    };

    getToken() {
        return this.app.acquireTokenSilent(this.applicationConfig.graphScopes).then(
            accessToken => {
                return accessToken;
            },
            error => {
                return this.app
                    .acquireTokenPopup(this.applicationConfig.graphScopes)
                    .then(
                        accessToken => {
                            return accessToken;
                        },
                        err => {
                            console.error(err);
                        }
                    );
            }
        );
    };
}