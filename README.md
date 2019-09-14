# wireless-display-switch
Automated wireless display connection

This UWP Application allows to select one of the available miracast devices using the UWP DevicePicker class. Once a device is selected, establishing a connection can be triggered by using the start-button. 

The Connection procedure is also triggered during application start. After successful connection, the app is closed.

Automated starting of the UWP-Application can easily be triggered by appending the app to start menu (via contect menu of the running app) and moving the saved link to autostart folder.

### This application is currently in _Proof of Concept_ state. ###
It's not is not at all mature or fully developed.
It is mostly based on [Windows Universal Samples](https://github.com/microsoft/Windows-universal-samples.git)

Raised exceptions during application start and connecting are ignored without functional impact.
(This should be changed in future or productive releases.) 