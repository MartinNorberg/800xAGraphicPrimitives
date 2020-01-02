## PG2 VNC Viewer

Initiation
----------
This Program can be used to display remote computer screens into 800xA PG2 graphic displays by using the old, but relyable VNC protocol.

Background Story
----------------
On my plant we have some engines with independent controllers. This controllers are common industrial PC's, running Windows. All of them have also some small and ugly operator panels. This panels are also small PC's running some kind of Windows. The pannels are connected to the controllers over ethernet, using the VNC protocol. This operator panels are difficult to use because no keyboard, only a inaccurate touchscreen. This is why we decide to give our operators access to this engines from 800xA. We connect this engines to the 800xA network, and now we can access the engines by using a standard VNC client. That was already very handy, but we won't give our operators access to a full grown VNC client. So we integrated a VNC client into some 800xA PG2 graphics. Now the 800xA graphic display shows the engine's operator panel.

Disclaimer
----------
The author of this program is not liable for damage to software or hardware or property damage incurred by the use of the program.

License
-------
This program is freeware. My parts of the source code are subject to the 2-clause BSD license, the parts of David Humphrey's VncSharp are subject to the GPL2 license.

Prerequisites
-------------
You will need:
- 800xA 5.1 with PG2 graphics
or
- 800xA 6.0 or 6.1

Compatibility
-------------
The program has been successfully tested with the 800xA versions:
- 5.1 FP4 (Win7)	Use the installer in 5.1 path!
- 6.0 and 6.1 (Win8.1)	Use the installer in 6.0 path!
and with this VNC-Servers:
- RealVNC (Windows)
- x11vnc (Linux)
- TightVNC (Linux & Windows)

VNC and Security
----------------
VNC is a very old protocol. Encryption and secure login are not mandatory features of VNC, and the encryption of VNC does not count as secure enymore. So this VNC Client does not support encryption, or everything else than basic authentication. If you want to use this VNC client, you should keep in mind:
- No encryption. It is possible to spy keystrokes and screencaptures send trough the network. 
- No secure login. VNC does not know usernames, only a few passwords for different access levels. The password will be transfered encrypted, but this kind of encryption is weak.
- The VNC Password is (maybe) visible into PG2 Graphic Editor, and Graphics Documentations.

Summary:

**DO  NOT  USE  VNC  OVER  THE  INTERNET!**

**DO  NOT  USE  VNC  OVER  UNPROTECTED  LAN  NETWORKS!**

**DO  NOT  USE  VNC  FOR  SECURE  CRITICAL  APPLICATIONS!**

If you really need to use VNC over insecure networks, you should consider to use additional secutity options like VPN or SSH Tunneling.

Known Issues
------------
- Empty passwords are not supported.
- Some keys does not working. Some keys are catched by 800xA, some keys are maybe missing in the keycode conversion table.
- Keyboard encoding defaults to german.  Most keys should be equal on other keyboards. If you find some wrong keys, you can change the encoding with the Keyconvert property.
- No Sound support. (VNC does not support sound at all)
- No clipboard support yet. If someone needs it, it should be easy to add.
      
Installation, Uninstallation, Configuration and Examples
-------------------------------------------
... are described in the [pg2 vnc client.pdf](pg2 vnc client.pdf) file.
